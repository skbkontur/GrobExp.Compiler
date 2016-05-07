using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;

using GrEmit;

namespace GrobExp.Compiler
{
    internal class ExpressionClosureBuilder : ExpressionVisitor
    {
        public ExpressionClosureBuilder(LambdaExpression lambda, ModuleBuilder module)
        {
            this.lambda = lambda;
            var id = (uint)Interlocked.Increment(ref closureId);
            closureTypeBuilder = module.DefineType("Closure_" + id, TypeAttributes.Public | TypeAttributes.Class);
            constantsTypeBuilder = module.DefineType("Constants_" + id, TypeAttributes.Public | TypeAttributes.Class);
        }

        public Result Build(bool dynamic)
        {
            var visitedLambda = (LambdaExpression)Visit(lambda);
            if(hasSubLambdas && dynamic)
                constantsTypeBuilder.DefineField("delegates", typeof(Delegate[]), FieldAttributes.Public);
            Type closureType = closureTypeBuilder.CreateType();
            var closureParameter = parameters.Count > 0 ? Expression.Parameter(closureType) : null;
            Type constantsType = constantsTypeBuilder.CreateType();
            var constantsParameter = constants.Count > 0 || switches.Count > 0 || (hasSubLambdas && dynamic) ? Expression.Parameter(constantsType) : null;
            var parsedParameters = parameters.ToDictionary(item => item.Key, item => closureType.GetField(item.Value.Name));
            var parsedConstants = constants.ToDictionary(item => item.Key, item => constantsType.GetField(item.Value.Name));
            var parsedSwitches = switches.ToDictionary(item => item.Key, item => new Tuple<FieldInfo, FieldInfo, int>(constantsType.GetField(item.Value.Item1.Name), constantsType.GetField(item.Value.Item2.Name), item.Value.Item3));
            var func = constantsParameter != null ? BuildConstants(constantsType, new ClosureSubstituter(closureParameter, parsedParameters)) : (() => null);
            return new Result
                {
                    Lambda = visitedLambda,
                    ClosureType = closureType,
                    ClosureParameter = closureParameter,
                    ConstantsType = constantsType,
                    ConstantsParameter = constantsParameter,
                    ParsedParameters = parsedParameters,
                    ParsedConstants = parsedConstants,
                    ParsedSwitches = parsedSwitches,
                    Constants = func()
                };
        }

        public class Result
        {
            public LambdaExpression Lambda { get; set; }
            public Type ClosureType { get; set; }
            public Type ConstantsType { get; set; }
            public ParameterExpression ClosureParameter { get; set; }
            public ParameterExpression ConstantsParameter { get; set; }
            public object Constants { get; set; }
            public Dictionary<ParameterExpression, FieldInfo> ParsedParameters { get; set; }
            public Dictionary<ConstantExpression, FieldInfo> ParsedConstants { get; set; }
            public Dictionary<SwitchExpression, Tuple<FieldInfo, FieldInfo, int>> ParsedSwitches { get; set; }
        }

        protected override Expression VisitSwitch(SwitchExpression node)
        {
            object switchCaseValues;
            object switchCaseIndexes;
            int count;
            var type = node.SwitchValue.Type;
            if(type.IsNullable())
                type = type.GetGenericArguments()[0];
            if(node.Cases.All(@case => @case.TestValues.All(expression => expression.NodeType == ExpressionType.Constant)) && TryBuildSwitchCaseValues(type, node.Cases, out switchCaseValues, out switchCaseIndexes, out count))
            {
                switches.Add(node, new Tuple<FieldBuilder, FieldBuilder, int>(BuildConstField(type.MakeArrayType(), switchCaseValues), BuildConstField(typeof(int[]), switchCaseIndexes), count));
                Visit(node.SwitchValue);
                Visit(node.DefaultBody);
                foreach(var @case in node.Cases)
                    Visit(@case.Body);
                return node;
            }
            return base.VisitSwitch(node);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if(node.NodeType != ExpressionType.Quote)
                return base.VisitUnary(node);
            ++quoteDepth;
            localParameters.Push(new HashSet<ParameterExpression>());
            var result = base.VisitUnary(node);
            localParameters.Pop();
            --quoteDepth;
            if(quoteDepth == 0)
                result = Visit(Expression.Constant(((UnaryExpression)result).Operand, result.Type));
            return result;
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            Expression res;
            if(quoteDepth == 0)
            {
                if(node != lambda)
                    hasSubLambdas = true;
                localParameters.Push(new HashSet<ParameterExpression>(node.Parameters));
                res = base.VisitLambda(node);
                localParameters.Pop();
            }
            else
            {
                var peek = localParameters.Peek();
                foreach(var parameter in node.Parameters)
                    peek.Add(parameter);
                res = base.VisitLambda(node);
                foreach(var parameter in node.Parameters)
                    peek.Remove(parameter);
            }
            return res;
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            var peek = localParameters.Peek();
            var variables = node.Variables.Where(variable => !peek.Contains(variable)).ToArray();
            foreach(var variable in variables)
                peek.Add(variable);
            var res = base.VisitBlock(node);
            foreach(var variable in variables)
                peek.Remove(variable);
            return res;
        }

        protected override CatchBlock VisitCatchBlock(CatchBlock node)
        {
            var peek = localParameters.Peek();
            var variable = node.Variable;
            if(variable != null && peek.Contains(variable))
                variable = null;
            if(variable != null)
                peek.Add(variable);
            var res = base.VisitCatchBlock(node);
            if(variable != null)
                peek.Remove(variable);
            return res;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if(quoteDepth > 0 || node.Value == null || node.Type.IsPrimitive /*|| (node.Type.IsNullable() && node.Type.GetGenericArguments()[0].IsPrimitive)*/|| node.Type == typeof(string))
                return node;
            if(!constants.ContainsKey(node))
                constants.Add(node, BuildConstField(node.Type, node.Value));
            return node;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            var peek = localParameters.Peek();
            if(!peek.Contains(node) && !parameters.ContainsKey(node))
            {
                FieldBuilder field = closureTypeBuilder.DefineField(GetFieldName(node.Type), GetFieldType(node.Type), FieldAttributes.Public);
                parameters.Add(node, field);
            }
            return base.VisitParameter(node);
        }

        private bool TryBuildSwitchCaseValues(Type type, IEnumerable<SwitchCase> cases, out object switchCaseValues, out object switchCaseIndexes, out int count)
        {
            switchCaseValues = null;
            switchCaseIndexes = null;
            count = 0;
            var typeCode = Type.GetTypeCode(type);
            switch(typeCode)
            {
            case TypeCode.DBNull:
            case TypeCode.Boolean:
            case TypeCode.DateTime:
            case TypeCode.Decimal:
            case TypeCode.Double:
            case TypeCode.Single:
            case TypeCode.Empty:
                return false;
            }
            var hashCodes = new Dictionary<ulong, object>();
            foreach(var @case in cases)
            {
                foreach(ConstantExpression constant in @case.TestValues)
                {
                    var value = constant.Value;
                    if(value == null)
                        continue;
                    ulong hashCode;
                    unchecked
                    {
                        switch(typeCode)
                        {
                        case TypeCode.Byte:
                            hashCode = (byte)value;
                            break;
                        case TypeCode.Char:
                            hashCode = (char)value;
                            break;
                        case TypeCode.Int16:
                            hashCode = (ulong)(short)value;
                            break;
                        case TypeCode.Int32:
                            hashCode = (ulong)(int)value;
                            break;
                        case TypeCode.Int64:
                            hashCode = (ulong)(long)value;
                            break;
                        case TypeCode.SByte:
                            hashCode = (ulong)(sbyte)value;
                            break;
                        case TypeCode.UInt16:
                            hashCode = (ushort)value;
                            break;
                        case TypeCode.UInt32:
                            hashCode = (uint)value;
                            break;
                        case TypeCode.UInt64:
                            hashCode = (ulong)value;
                            break;
                        default:
                            hashCode = (ulong)value.GetHashCode();
                            break;
                        }
                    }
                    if(hashCodes.ContainsKey(hashCode))
                        return false;
                    hashCodes.Add(hashCode, value);
                }
            }
            var was = new HashSet<uint>();
            for(int length = hashCodes.Count; length < 100000; ++length)
            {
                bool ok = true;
                was.Clear();
                foreach(var entry in hashCodes)
                {
                    unchecked
                    {
                        var index = (uint)(entry.Key % (uint)length);
                        if(was.Contains(index))
                        {
                            ok = false;
                            break;
                        }
                        was.Add(index);
                    }
                }
                if(!ok) continue;
                var values = new object[length];
                var indexes = new int[length];
                for(int k = 0; k < length; ++k)
                {
                    unchecked
                    {
                        switch(typeCode)
                        {
                        case TypeCode.Byte:
                            values[k] = (byte)(k + 1);
                            break;
                        case TypeCode.Char:
                            values[k] = (char)(k + 1);
                            break;
                        case TypeCode.Int16:
                            values[k] = (short)(k + 1);
                            break;
                        case TypeCode.Int32:
                            values[k] = k + 1;
                            break;
                        case TypeCode.Int64:
                            values[k] = (long)(k + 1);
                            break;
                        case TypeCode.SByte:
                            values[k] = (sbyte)(k + 1);
                            break;
                        case TypeCode.UInt16:
                            values[k] = (ushort)(k + 1);
                            break;
                        case TypeCode.UInt32:
                            values[k] = (uint)(k + 1);
                            break;
                        case TypeCode.UInt64:
                            values[k] = (ulong)(k + 1);
                            break;
                        default:
                            values[k] = null;
                            break;
                        }
                    }
                    indexes[k] = -1;
                }
                int i = 0;
                foreach(var entry in hashCodes)
                {
                    unchecked
                    {
                        var index = (uint)(entry.Key % (uint)length);
                        values[index] = entry.Value;
                        indexes[index] = i;
                    }
                    ++i;
                }
                switchCaseValues = convertMethod.MakeGenericMethod(type).Invoke(null, new object[] {values});
                switchCaseIndexes = indexes;
                count = length;
                return true;
            }
            return false;
        }

        private static T[] Convert<T>(object[] arr)
        {
            return arr.Cast<T>().ToArray();
        }

        private FieldBuilder BuildConstField(Type type, object value)
        {
            var key = new KeyValuePair<Type, object>(type, value);
            var field = (FieldBuilder)hashtable[key];
            if(field == null)
            {
                field = constantsTypeBuilder.DefineField(GetFieldName(type), GetFieldType(type), FieldAttributes.Public);
                hashtable[key] = field;
            }
            return field;
        }

        private Func<object> BuildConstants(Type type, ClosureSubstituter closureSubstituter)
        {
            var method = new DynamicMethod("Construct_" + type.Name, typeof(object), new[] {typeof(object[])}, typeof(ExpressionClosureBuilder), true);
            var consts = new object[hashtable.Count];
            using(var il = new GroboIL(method))
            {
                il.Newobj(type.GetConstructor(Type.EmptyTypes));
                int index = 0;
                foreach(DictionaryEntry entry in hashtable)
                {
                    var pair = (KeyValuePair<Type, object>)entry.Key;
                    var constType = pair.Key;
                    consts[index] = pair.Value is Expression ? closureSubstituter.Visit((Expression)pair.Value) : pair.Value;
                    il.Dup();
                    il.Ldarg(0);
                    il.Ldc_I4(index++);
                    il.Ldelem(typeof(object));
                    string name = ((FieldInfo)entry.Value).Name;
                    var field = type.GetField(name);
                    if(field == null)
                        throw new MissingFieldException(type.Name, name);
                    if(!constType.IsValueType)
                        il.Castclass(field.FieldType);
                    else
                    {
                        il.Unbox_Any(constType);
                        if(field.FieldType != constType)
                        {
                            var constructor = field.FieldType.GetConstructor(new[] {constType});
                            if(constructor == null)
                                throw new InvalidOperationException("Missing constructor of type '" + Format(field.FieldType) + "' with parameter of type '" + Format(constType) + "'");
                            il.Newobj(constructor);
                        }
                    }
                    il.Stfld(field);
                }
                il.Ret();
            }
            var func = (Func<object[], object>)method.CreateDelegate(typeof(Func<object[], object>));
            return () => func(consts);
        }

        private static bool IsPrivate(Type type)
        {
            if(type.IsNestedPrivate || type.IsNotPublic)
                return true;
            return type.IsGenericType && type.GetGenericArguments().Any(IsPrivate);
        }

        private static Type GetFieldType(Type type)
        {
            return IsPrivate(type) && type.IsValueType
                       ? typeof(StrongBox<>).MakeGenericType(new[] {type})
                       : type;
        }

        private static string Format(Type type)
        {
            if(!type.IsGenericType)
                return type.Name;
            return type.Name + "<" + string.Join(", ", type.GetGenericArguments().Select(Format)) + ">";
        }

        private string GetFieldName(Type type)
        {
            return Format(type) + "_" + fieldId++;
        }

        private static readonly MethodInfo convertMethod = ((MethodCallExpression)((Expression<Func<object[], int[]>>)(arr => Convert<int>(arr))).Body).Method.GetGenericMethodDefinition();

        private int quoteDepth;

        private bool hasSubLambdas;

        private static int closureId;
        private int fieldId;

        private readonly LambdaExpression lambda;
        private readonly Stack<HashSet<ParameterExpression>> localParameters = new Stack<HashSet<ParameterExpression>>();

        private readonly Hashtable hashtable = new Hashtable();
        private readonly Dictionary<ConstantExpression, FieldBuilder> constants = new Dictionary<ConstantExpression, FieldBuilder>();
        private readonly Dictionary<ParameterExpression, FieldBuilder> parameters = new Dictionary<ParameterExpression, FieldBuilder>();
        private readonly Dictionary<SwitchExpression, Tuple<FieldBuilder, FieldBuilder, int>> switches = new Dictionary<SwitchExpression, Tuple<FieldBuilder, FieldBuilder, int>>();

        private readonly TypeBuilder closureTypeBuilder;
        private readonly TypeBuilder constantsTypeBuilder;
    }
}
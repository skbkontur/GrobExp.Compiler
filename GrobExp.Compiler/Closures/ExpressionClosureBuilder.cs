using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

using GrEmit;
using GrEmit.Utils;

namespace GrobExp.Compiler.Closures
{
    internal class ExpressionClosureBuilder : ExpressionVisitor
    {
        public ExpressionClosureBuilder(LambdaExpression lambda, IClosureBuilder closureBuilder, IClosureBuilder constantsBuilder)
        {
            this.lambda = lambda;
            this.closureBuilder = closureBuilder;
            this.constantsBuilder = constantsBuilder;
        }

        public Result Build(bool dynamic)
        {
            var visitedLambda = (LambdaExpression)Visit(lambda);
            int delegatesFieldId = -1;
            if (hasSubLambdas && dynamic)
                delegatesFieldId = constantsBuilder.DefineField(typeof(Delegate[]));
            Type closureType = closureBuilder.Create();
            var closureParameter = parameters.Count > 0 ? Expression.Parameter(closureType) : null;
            Type constantsType = constantsBuilder.Create();
            var constantsParameter = constants.Count > 0 || switches.Count > 0 || (hasSubLambdas && dynamic) ? Expression.Parameter(constantsType) : null;
            var func = constantsParameter == null ? (() => null) : BuildConstants(constantsType, new ClosureSubstituter(closureParameter, closureBuilder, parameters));
            return new Result
                {
                    Lambda = visitedLambda,
                    ClosureType = closureType,
                    ClosureParameter = closureParameter,
                    ConstantsType = constantsType,
                    ConstantsParameter = constantsParameter,
                    ParsedParameters = parameters,
                    ParsedConstants = constants,
                    ParsedSwitches = switches,
                    DelegatesFieldId = delegatesFieldId,
                    Constants = func()
                };
        }

        protected override Expression VisitSwitch(SwitchExpression node)
        {
            object switchCaseValues;
            object switchCaseIndexes;
            int count;
            var type = node.SwitchValue.Type;
            if (type.IsNullable())
                type = type.GetGenericArguments()[0];
            if (node.Cases.All(@case => @case.TestValues.All(expression => expression.NodeType == ExpressionType.Constant)) && TryBuildSwitchCaseValues(type, node.Cases, out switchCaseValues, out switchCaseIndexes, out count))
            {
                switches.Add(node, new Tuple<int, int, int>(BuildConstField(type.MakeArrayType(), switchCaseValues), BuildConstField(typeof(int[]), switchCaseIndexes), count));
                Visit(node.SwitchValue);
                Visit(node.DefaultBody);
                foreach (var @case in node.Cases)
                    Visit(@case.Body);
                return node;
            }
            return base.VisitSwitch(node);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType != ExpressionType.Quote)
                return base.VisitUnary(node);
            ++quoteDepth;
            localParameters.Push(new HashSet<ParameterExpression>());
            var result = base.VisitUnary(node);
            localParameters.Pop();
            --quoteDepth;
            if (quoteDepth == 0)
                result = Visit(Expression.Constant(((UnaryExpression)result).Operand, result.Type));
            return result;
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            Expression res;
            if (quoteDepth == 0)
            {
                if (node != lambda)
                    hasSubLambdas = true;
                localParameters.Push(new HashSet<ParameterExpression>(node.Parameters));
                res = base.VisitLambda(node);
                localParameters.Pop();
            }
            else
            {
                var peek = localParameters.Peek();
                foreach (var parameter in node.Parameters)
                    peek.Add(parameter);
                res = base.VisitLambda(node);
                foreach (var parameter in node.Parameters)
                    peek.Remove(parameter);
            }
            return res;
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            var peek = localParameters.Peek();
            var variables = node.Variables.Where(variable => !peek.Contains(variable)).ToArray();
            foreach (var variable in variables)
                peek.Add(variable);
            var res = base.VisitBlock(node);
            foreach (var variable in variables)
                peek.Remove(variable);
            return res;
        }

        protected override CatchBlock VisitCatchBlock(CatchBlock node)
        {
            var peek = localParameters.Peek();
            var variable = node.Variable;
            if (variable != null && peek.Contains(variable))
                variable = null;
            if (variable != null)
                peek.Add(variable);
            var res = base.VisitCatchBlock(node);
            if (variable != null)
                peek.Remove(variable);
            return res;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (quoteDepth > 0 || node.Value == null || IsPrimitiveConstantType(node.Type))
                return node;
            if (!constants.ContainsKey(node))
                constants.Add(node, BuildConstField(node.Type, node.Value));
            return node;
        }

        private static bool IsPrimitiveConstantType(Type type)
        {
            if (type.IsNullable())
                type = type.GetGenericArguments()[0];
            return type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(decimal);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            var peek = localParameters.Peek();
            if (!peek.Contains(node) && !parameters.ContainsKey(node))
            {
                var field = closureBuilder.DefineField(node.Type);
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
            switch (typeCode)
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
            foreach (var @case in cases)
            {
                foreach (ConstantExpression constant in @case.TestValues)
                {
                    var value = constant.Value;
                    if (value == null)
                        continue;
                    ulong hashCode;
                    unchecked
                    {
                        switch (typeCode)
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
                    if (hashCodes.ContainsKey(hashCode))
                        return false;
                    hashCodes.Add(hashCode, value);
                }
            }
            var was = new HashSet<uint>();
            for (int length = hashCodes.Count; length < 100000; ++length)
            {
                bool ok = true;
                was.Clear();
                foreach (var entry in hashCodes)
                {
                    unchecked
                    {
                        var index = (uint)(entry.Key % (uint)length);
                        if (was.Contains(index))
                        {
                            ok = false;
                            break;
                        }
                        was.Add(index);
                    }
                }
                if (!ok) continue;
                var values = new object[length];
                var indexes = new int[length];
                for (int k = 0; k < length; ++k)
                {
                    unchecked
                    {
                        switch (typeCode)
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
                foreach (var entry in hashCodes)
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

        private int BuildConstField(Type type, object value)
        {
            var key = new Tuple<Type, object>(type, value);
            var field = (int?)hashtable[key];
            if (field == null)
            {
                field = constantsBuilder.DefineField(type);
                hashtable[key] = field;
            }
            return field.Value;
        }

        private Func<object> BuildConstants(Type type, ClosureSubstituter closureSubstituter)
        {
            var method = new DynamicMethod(Guid.NewGuid().ToString(), typeof(object), new[] {typeof(object[])},
                                           typeof(ExpressionClosureBuilder), true);
            var root = Expression.Parameter(type);
            var consts = new object[hashtable.Count];
            using (var il = new GroboIL(method))
            {
                il.Newobj(type.GetConstructor(Type.EmptyTypes)); // stack: [new Constants()]
                var result = il.DeclareLocal(type, "result");
                il.Stloc(result); // result = new Constants(); stack: []
                int index = 0;
                foreach (DictionaryEntry entry in hashtable)
                {
                    var pair = (Tuple<Type, object>)entry.Key;
                    var constType = pair.Item1;
                    consts[index] = pair.Item2 is Expression ? closureSubstituter.Visit((Expression)pair.Item2) : pair.Item2;

                    var fieldAccessor = constantsBuilder.MakeAccess(root, ((int?)entry.Value).Value);
                    var pathToField = new List<FieldInfo>();
                    while (fieldAccessor.NodeType != ExpressionType.Parameter)
                    {
                        var memberExpression = (MemberExpression)fieldAccessor;
                        pathToField.Add((FieldInfo)memberExpression.Member);
                        fieldAccessor = memberExpression.Expression;
                    }
                    pathToField.Reverse();
                    il.Ldloc(result);
                    for (int i = 0; i < pathToField.Count - 1; ++i)
                    {
                        il.Ldflda(pathToField[i]); // stack: [ref result.field]
                        il.Dup(); // stack: [ref result.field, ref result.field]
                        var fieldType = pathToField[i].FieldType;
                        il.Ldind(fieldType); // stack: [ref result.field, result.field]
                        var notNullLabel = il.DefineLabel("notNull");
                        il.Brtrue(notNullLabel); // if(result.field != null) goto notNull; stack: [ref result.field]
                        il.Dup(); // stack: [ref result.field, ref result.field]
                        il.Newobj(fieldType.GetConstructor(Type.EmptyTypes)); // stack: [ref result.field, ref result.field, new field()]
                        il.Stind(fieldType); // result.field = new field(); stack: [ref result.field]
                        il.MarkLabel(notNullLabel);
                        il.Ldind(fieldType); // stack: [result.field]
                    }
                    il.Ldarg(0); // stack: [path, args]
                    il.Ldc_I4(index++); // stack: [path, args, index]
                    il.Ldelem(typeof(object)); // stack: [path, args[index]]
                    var field = pathToField.Last();
                    if (!constType.IsValueType)
                        il.Castclass(field.FieldType); // stack: [path, (FieldType)args[index]]
                    else
                    {
                        il.Unbox_Any(constType);
                        if (field.FieldType != constType)
                        {
                            var constructor = field.FieldType.GetConstructor(new[] {constType});
                            if (constructor == null)
                                throw new InvalidOperationException(string.Format("Missing constructor of type '{0}' with parameter of type '{1}'",
                                                                                  Formatter.Format(field.FieldType), Formatter.Format(constType)));
                            il.Newobj(constructor);
                        }
                    }
                    il.Stfld(field); // path.field = (FieldType)args[index]; stack: []
                }
                il.Ldloc(result);
                il.Ret();
            }
            var func = (Func<object[], object>)method.CreateDelegate(typeof(Func<object[], object>));
            return () => func(consts);
        }

        private static bool IsPrivate(Type type)
        {
            if (type.IsNestedPrivate || type.IsNotPublic)
                return true;
            return type.IsGenericType && type.GetGenericArguments().Any(IsPrivate);
        }

        private static readonly MethodInfo convertMethod = ((MethodCallExpression)((Expression<Func<object[], int[]>>)(arr => Convert<int>(arr))).Body).Method.GetGenericMethodDefinition();

        private int quoteDepth;

        private bool hasSubLambdas;

        private readonly LambdaExpression lambda;
        private readonly IClosureBuilder closureBuilder;
        private readonly IClosureBuilder constantsBuilder;
        private readonly Stack<HashSet<ParameterExpression>> localParameters = new Stack<HashSet<ParameterExpression>>();

        private readonly Hashtable hashtable = new Hashtable();
        private readonly Dictionary<ConstantExpression, int> constants = new Dictionary<ConstantExpression, int>();
        private readonly Dictionary<ParameterExpression, int> parameters = new Dictionary<ParameterExpression, int>();
        private readonly Dictionary<SwitchExpression, Tuple<int, int, int>> switches = new Dictionary<SwitchExpression, Tuple<int, int, int>>();

        public class Result
        {
            public LambdaExpression Lambda { get; set; }
            public Type ClosureType { get; set; }
            public Type ConstantsType { get; set; }
            public ParameterExpression ClosureParameter { get; set; }
            public ParameterExpression ConstantsParameter { get; set; }
            public object Constants { get; set; }
            public int DelegatesFieldId { get; set; }
            public Dictionary<ParameterExpression, int> ParsedParameters { get; set; }
            public Dictionary<ConstantExpression, int> ParsedConstants { get; set; }
            public Dictionary<SwitchExpression, Tuple<int, int, int>> ParsedSwitches { get; set; }
        }
    }
}
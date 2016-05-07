using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

using GrEmit;
using GrEmit.Utils;

namespace GrobExp.Compiler.ExpressionEmitters
{
    internal class ArrayIndexExpressionEmitter : ExpressionEmitter<BinaryExpression>
    {
        protected override bool EmitInternal(BinaryExpression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            return Emit(node.Left, node.Right, context, returnDefaultValueLabel, whatReturn, extend, out resultType);
        }

        public static bool Emit(Expression zarr, Expression zindex, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            var arrayType = zarr.Type;
            var isArray = arrayType.IsArray;
            if(!isArray && !arrayType.IsList())
                throw new InvalidOperationException("Unable to perform array index operator to type '" + arrayType + "'");
            var itemType = isArray ? arrayType.GetElementType() : arrayType.GetGenericArguments()[0];
            GroboIL il = context.Il;
            EmittingContext.LocalHolder arrayIndex = null;
            bool extendArray = extend && CanAssign(zarr) || !isArray;
            bool extendArrayElement = extend && itemType.IsClass;
            var result = false;
            if(!extendArray)
            {
                result |= ExpressionEmittersCollection.Emit(zarr, context, returnDefaultValueLabel, ResultType.Value, extend, out arrayType); // stack: [array]
                if(context.Options.HasFlag(CompilerOptions.CheckNullReferences))
                {
                    result = true;
                    il.Dup(); // stack: [array, array]
                    il.Brfalse(returnDefaultValueLabel); // if(array == null) goto returnDefaultValue; stack: [array]
                }
                EmitLoadIndex(zindex, context, arrayType); // stack: [array, arrayIndex]
                if(context.Options.HasFlag(CompilerOptions.CheckArrayIndexes))
                {
                    result = true;
                    arrayIndex = context.DeclareLocal(typeof(int));
                    il.Stloc(arrayIndex); // arrayIndex = index; stack: [array]
                    il.Dup(); // stack: [array, array]
                    if(isArray)
                        il.Ldlen(); // stack: [array, array.Length]
                    else
                        EmitLoadField(context, arrayType, arrayType.GetField("_size", BindingFlags.Instance | BindingFlags.NonPublic));
                    il.Ldloc(arrayIndex); // stack: [array, array.Length, arrayIndex]
                    il.Ble(returnDefaultValueLabel, false); // if(array.Length <= arrayIndex) goto returnDefaultValue; stack: [array]
                    il.Ldloc(arrayIndex); // stack: [array, arrayIndex]
                    il.Ldc_I4(0); // stack: [array, arrayIndex, 0]
                    il.Blt(returnDefaultValueLabel, false); // if(arrayIndex < 0) goto returnDefaultValue; stack: [array]
                }
                else if(extendArrayElement)
                {
                    arrayIndex = context.DeclareLocal(typeof(int));
                    il.Stloc(arrayIndex); // arrayIndex = index; stack: [array]
                }
            }
            else
            {
                EmittingContext.LocalHolder arrayOwner = null;
                switch(zarr.NodeType)
                {
                case ExpressionType.Parameter:
                case ExpressionType.ArrayIndex:
                case ExpressionType.Index:
                    Type type;
                    ExpressionEmittersCollection.Emit(zarr, context, returnDefaultValueLabel, ResultType.ByRefAll, true, out type); // stack: [ref array]
                    arrayOwner = context.DeclareLocal(type);
                    il.Dup(); // stack: [ref array, ref array]
                    il.Stloc(arrayOwner); // arrayOwner = ref array; stack: [ref array]
                    il.Ldind(zarr.Type); // stack: [array]
                    break;
                case ExpressionType.MemberAccess:
                    var memberExpression = (MemberExpression)zarr;
                    Type memberType;
                    context.EmitMemberAccess(memberExpression, returnDefaultValueLabel, context.Options.HasFlag(CompilerOptions.CheckNullReferences), true, ResultType.ByRefValueTypesOnly, out memberType, out arrayOwner); // stack: [array]
                    break;
                default:
                    throw new InvalidOperationException("Cannot extend array for expression with node type '" + zarr.NodeType + "'");
                }
                if(context.Options.HasFlag(CompilerOptions.CheckNullReferences))
                {
                    il.Dup(); // stack: [array, array]
                    il.Brfalse(returnDefaultValueLabel); // if(array == null) goto returnDefaultValue; stack: [array]
                }
                EmitLoadIndex(zindex, context, arrayType);
                result = true;
                arrayIndex = context.DeclareLocal(typeof(int));
                il.Stloc(arrayIndex); // arrayIndex = index; stack: [array]
                il.Ldloc(arrayIndex); // stack: [array, arrayIndex]
                il.Ldc_I4(0); // stack: [array, arrayIndex, 0]
                il.Blt(returnDefaultValueLabel, false); // if(arrayIndex < 0) goto returnDefaultValue; stack: [array]
                il.Dup(); // stack: [array, array]
                if(isArray)
                    il.Ldlen(); // stack: [array, array.Length]
                else
                    EmitLoadField(context, arrayType, arrayType.GetField("_size", BindingFlags.Instance | BindingFlags.NonPublic));
                il.Ldloc(arrayIndex); // stack: [array, array.Length, arrayIndex]
                var bigEnoughLabel = il.DefineLabel("bigEnough");
                il.Bgt(bigEnoughLabel, false); // if(array.Length > arrayIndex) goto bigEnough; stack: [array]
                using(var array = context.DeclareLocal(arrayType))
                {
                    il.Stloc(array); // stack: []
                    if(!isArray)
                        EnsureCount(context, array, arrayIndex, arrayType);
                    else
                    {
                        il.Ldloca(array); // stack: [ref array]
                        il.Ldloc(arrayIndex); // stack: [ref array, arrayIndex]
                        il.Ldc_I4(1); // stack: [ref array, arrayIndex, 1]
                        il.Add(); // stack: [ref array, arrayIndex + 1]
                        il.Call(arrayResizeMethod.MakeGenericMethod(arrayType.GetElementType())); // Array.Resize(ref array, 1 + arrayIndex); stack: []

                        switch(zarr.NodeType)
                        {
                        case ExpressionType.Parameter:
                        case ExpressionType.ArrayIndex:
                        case ExpressionType.Index:
                            il.Ldloc(arrayOwner); // stack: [ref parameter]
                            il.Ldloc(array); // stack: [ref parameter, array]
                            il.Stind(arrayType); // parameter = array; stack: []
                            break;
                        case ExpressionType.MemberAccess:
                            var memberExpression = (MemberExpression)zarr;
                            if(memberExpression.Expression != null)
                                il.Ldloc(arrayOwner);
                            il.Ldloc(array);
                            switch(memberExpression.Member.MemberType)
                            {
                            case MemberTypes.Field:
                                il.Stfld((FieldInfo)memberExpression.Member);
                                break;
                            case MemberTypes.Property:
                                var propertyInfo = (PropertyInfo)memberExpression.Member;
                                var setter = propertyInfo.GetSetMethod(context.SkipVisibility);
                                if(setter == null)
                                    throw new MissingMethodException(propertyInfo.ReflectedType.ToString(), "set_" + propertyInfo.Name);
                                il.Call(setter, memberExpression.Expression == null ? null : memberExpression.Expression.Type);
                                break;
                            default:
                                throw new NotSupportedException("Member type '" + memberExpression.Member.MemberType + "' is not supported");
                            }
                            break;
                        default:
                            throw new InvalidOperationException("Unable to assign array to an expression with node type '" + zarr.NodeType);
                        }
                    }
                    il.Ldloc(array);
                    context.MarkLabelAndSurroundWithSP(bigEnoughLabel);
                }
            }

            if(!isArray)
            {
                // TODO: это злобно, лист при всех операциях меняет _version, а мы нет
                EmitLoadField(context, arrayType, arrayType.GetField("_items", BindingFlags.Instance | BindingFlags.NonPublic));
                arrayType = itemType.MakeArrayType();
            }

            if(extendArrayElement)
            {
                // stack: [array]
                var constructor = itemType.GetConstructor(Type.EmptyTypes);
                if(itemType.IsArray || constructor != null)
                {
                    using(var array = context.DeclareLocal(arrayType))
                    {
                        il.Dup(); // stack: [array, array]
                        il.Stloc(array); // stack: [array]
                        il.Ldloc(arrayIndex); // stack: [array, arrayIndex]
                        il.Ldelem(itemType); // stack: [array[arrayIndex]]
                        var elementIsNotNullLabel = il.DefineLabel("elementIsNotNull");
                        il.Brtrue(elementIsNotNullLabel);
                        il.Ldloc(array);
                        il.Ldloc(arrayIndex);
                        context.Create(itemType);
                        il.Stelem(itemType);
                        context.MarkLabelAndSurroundWithSP(elementIsNotNullLabel);
                        il.Ldloc(array);
                    }
                }
            }
            if(arrayIndex != null)
            {
                il.Ldloc(arrayIndex);
                arrayIndex.Dispose();
            }
            switch(whatReturn)
            {
            case ResultType.ByRefAll:
                il.Ldelema(itemType);
                resultType = itemType.MakeByRefType();
                break;
            case ResultType.ByRefValueTypesOnly:
                if(itemType.IsValueType)
                {
                    il.Ldelema(itemType);
                    resultType = itemType.MakeByRefType();
                }
                else
                {
                    il.Ldelem(itemType); // stack: [array[arrayIndex]]
                    resultType = itemType;
                }
                break;
            default:
                il.Ldelem(itemType); // stack: [array[arrayIndex]]
                resultType = itemType;
                break;
            }
            return result;
        }

        private static void EmitLoadField(EmittingContext context, Type type, FieldInfo field)
        {
            var il = context.Il;
            if(context.SkipVisibility || field.IsPublic)
            {
                il.Ldfld(field);
            }
            else
            {
                // todo убрать Reflection
                var extractor = (Tuple<Delegate, IntPtr>)extractFieldMethod.MakeGenericMethod(type, field.FieldType).Invoke(null, new[] {(object)field});
                il.Ldc_IntPtr(extractor.Item2);
                il.Calli(CallingConventions.Standard, field.FieldType, new [] {type});
            }
        }

        private static void EnsureCount(EmittingContext context, GroboIL.Local list, GroboIL.Local index, Type type)
        {
            var il = context.Il;
            if(context.SkipVisibility)
            {
                il.Ldloc(list); // stack: [list]
                il.Ldloc(index); // stack: [list, arrayIndex]
                il.Ldc_I4(1); // stack: [list, arrayIndex, 1]
                il.Add(); // stack: [list, arrayIndex + 1]
                il.Call(type.GetMethod("EnsureCapacity", BindingFlags.Instance | BindingFlags.NonPublic)); // list.EnsureCapacity(arrayIndex + 1); stack: []
                il.Ldloc(list); // stack: [list]
                il.Ldloc(index); // stack: [list, arrayIndex]
                il.Ldc_I4(1); // stack: [list, arrayIndex, 1]
                il.Add(); // stack: [list, arrayIndex + 1]
                il.Stfld(type.GetField("_size", BindingFlags.Instance | BindingFlags.NonPublic)); // list.Count = arrayIndex + 1; stack: []
            }
            else
            {
                // todo убрать Reflection
                var resizer = (Tuple<Delegate, IntPtr>)resizeListMethod.MakeGenericMethod(type.GetGenericArguments()[0]).Invoke(null, new object[0]);
                il.Ldloc(list);
                il.Ldloc(index);
                il.Ldc_IntPtr(resizer.Item2);
                il.Calli(CallingConventions.Standard, typeof(void), new[] {type, typeof(int)});
            }
        }

        private static readonly Hashtable hashtable = new Hashtable();
        private static readonly object lockObject = new object();

        private static Tuple<Delegate, IntPtr> GetFieldExtractor<T, TValue>(FieldInfo field)
        {
            var result = (Tuple<Delegate, IntPtr>)hashtable[field];
            if (result == null)
            {
                lock (lockObject)
                {
                    result = (Tuple<Delegate, IntPtr>)hashtable[field];
                    if (result == null)
                    {
                        hashtable[field] = result = EmitFieldExtractor<T, TValue>(field);
                    }
                }
            }
            return result;
        }

        private static Tuple<Delegate, IntPtr> GetListResizer<T>()
        {
            var key = typeof(T);
            var result = (Tuple<Delegate, IntPtr>)hashtable[key];
            if(result == null)
            {
                lock(lockObject)
                {
                    result = (Tuple<Delegate, IntPtr>)hashtable[key];
                    if(result == null)
                    {
                        hashtable[key] = result = EmitListResizer<T>();
                    }
                }
            }
            return result;
        }

        private static Tuple<Delegate, IntPtr> EmitListResizer<T>()
        {
            var method = new DynamicMethod(Guid.NewGuid().ToString(), typeof(void), new [] {typeof(List<T>), typeof(int)}, typeof(string), true);
            using(var il = new GroboIL(method))
            {
                il.Ldarg(0); // stack: [list]
                il.Ldarg(1); // stack: [list, arrayIndex]
                il.Ldc_I4(1); // stack: [list, arrayIndex, 1]
                il.Add(); // stack: [list, arrayIndex + 1]
                il.Call(typeof(List<T>).GetMethod("EnsureCapacity", BindingFlags.Instance | BindingFlags.NonPublic)); // list.EnsureCapacity(arrayIndex + 1); stack: []
                il.Ldarg(0); // stack: [list]
                il.Ldarg(1); // stack: [list, arrayIndex]
                il.Ldc_I4(1); // stack: [list, arrayIndex, 1]
                il.Add(); // stack: [list, arrayIndex + 1]
                il.Stfld(typeof(List<T>).GetField("_size", BindingFlags.Instance | BindingFlags.NonPublic)); // list.Count = arrayIndex + 1; stack: []
                il.Ret();
            }
            return new Tuple<Delegate, IntPtr>((Action<List<T>, int>)method.CreateDelegate(typeof(Action<List<T>, int>)), DynamicMethodInvokerBuilder.DynamicMethodPointerExtractor(method));
        }

        private static Tuple<Delegate, IntPtr> EmitFieldExtractor<T, TValue>(FieldInfo field)
        {
            var method = new DynamicMethod(Guid.NewGuid().ToString(), typeof(TValue), new [] {typeof(T)}, typeof(string), true);
            using(var il = new GroboIL(method))
            {
                il.Ldarg(0); // stack: [list]
                il.Ldfld(field);
                il.Ret();
            }
            return new Tuple<Delegate, IntPtr>((Func<T, TValue>)method.CreateDelegate(typeof(Func<T, TValue>)), DynamicMethodInvokerBuilder.DynamicMethodPointerExtractor(method));
        }

        private static readonly MethodInfo extractFieldMethod = HackHelpers.GetMethodDefinition<int>(x => GetFieldExtractor<string, int>(null)).GetGenericMethodDefinition();
        private static readonly MethodInfo resizeListMethod = HackHelpers.GetMethodDefinition<int>(x => GetListResizer<int>()).GetGenericMethodDefinition();

        private static bool CanAssign(MemberInfo member)
        {
            return member.MemberType == MemberTypes.Field || (member.MemberType == MemberTypes.Property && ((PropertyInfo)member).CanWrite);
        }

        private static bool CanAssign(IndexExpression indexExpression)
        {
            return indexExpression.Object != null && indexExpression.Object.Type.IsArray && indexExpression.Object.Type.GetArrayRank() == 1;
        }

        private static bool CanAssign(Expression node)
        {
            return node.NodeType == ExpressionType.Parameter
                   || node.NodeType == ExpressionType.ArrayIndex
                   || (node.NodeType == ExpressionType.Index && CanAssign((IndexExpression)node))
                   || (node.NodeType == ExpressionType.MemberAccess && CanAssign(((MemberExpression)node).Member));
        }

        private static void EmitLoadIndex(Expression index, EmittingContext context, Type arrayType)
        {
            GroboIL il = context.Il;
            GroboIL.Label indexIsNullLabel = context.CanReturn ? il.DefineLabel("indexIsNull") : null;
            Type indexType;
            bool labelUsed = ExpressionEmittersCollection.Emit(index, context, indexIsNullLabel, out indexType); // stack: [array, index]
            if(indexType != typeof(int))
                throw new InvalidOperationException("Unable to perform array index operator to type '" + arrayType + "'");
            if(labelUsed && context.CanReturn)
            {
                var indexIsNotNullLabel = il.DefineLabel("indexIsNotNull");
                il.Br(indexIsNotNullLabel);
                context.MarkLabelAndSurroundWithSP(indexIsNullLabel);
                il.Pop();
                il.Ldc_I4(0);
                context.MarkLabelAndSurroundWithSP(indexIsNotNullLabel);
            }
        }

        private static readonly MethodInfo arrayResizeMethod = ((MethodCallExpression)((Expression<Action<int[], int>>)((ints, len) => Array.Resize(ref ints, len))).Body).Method.GetGenericMethodDefinition();
    }
}
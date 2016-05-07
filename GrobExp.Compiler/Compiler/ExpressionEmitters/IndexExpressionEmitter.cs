using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using GrEmit;

namespace GrobExp.Compiler.ExpressionEmitters
{
    internal class IndexExpressionEmitter : ExpressionEmitter<IndexExpression>
    {
        protected override bool EmitInternal(IndexExpression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            if (node.Object == null)
                throw new InvalidOperationException("Indexing of null object is invalid");
            if (node.Object.Type.IsArray && node.Object.Type.GetArrayRank() == 1)
                return ExpressionEmittersCollection.Emit(Expression.ArrayIndex(node.Object, node.Arguments.Single()), context, returnDefaultValueLabel, whatReturn, extend, out resultType);
            if(node.Object.Type.IsList())
                return ArrayIndexExpressionEmitter.Emit(node.Object, node.Arguments.Single(), context, returnDefaultValueLabel, whatReturn, extend, out resultType);
            Type objectType;
            bool result = ExpressionEmittersCollection.Emit(node.Object, context, returnDefaultValueLabel, ResultType.ByRefValueTypesOnly, extend, out objectType);
            if(objectType.IsValueType)
            {
                using(var temp = context.DeclareLocal(objectType))
                {
                    context.Il.Stloc(temp);
                    context.Il.Ldloca(temp);
                }
            }
            if(context.Options.HasFlag(CompilerOptions.CheckNullReferences))
                result |= context.EmitNullChecking(objectType, returnDefaultValueLabel);
            if(node.Indexer != null)
            {
                context.EmitLoadArguments(node.Arguments.ToArray());
                MethodInfo getter = node.Indexer.GetGetMethod(context.SkipVisibility);
                if(getter == null)
                    throw new MissingMethodException(node.Indexer.ReflectedType.ToString(), "get_" + node.Indexer.Name);
                GroboIL.Label doneLabel = null;
                if(node.Object.Type.IsDictionary())
                {
                    var valueType = node.Object.Type.GetGenericArguments()[1];
                    var constructor = valueType.GetConstructor(Type.EmptyTypes);
                    extend &= (valueType.IsClass && constructor != null) || valueType.IsArray || valueType.IsValueType || valueType == typeof(string);
                    doneLabel = context.Il.DefineLabel("done");
                    using(var dict = context.DeclareLocal(node.Object.Type))
                    using(var key = context.DeclareLocal(node.Arguments.Single().Type))
                    using(var value = context.DeclareLocal(valueType))
                    {
                        context.Il.Stloc(key); // key = arg0; stack: [dict]
                        context.Il.Dup(); // stack: [dict, dict]
                        context.Il.Stloc(dict); // stack: [dict]
                        context.Il.Ldloc(key); // stack: [dict, key]
                        context.Il.Ldloca(value); // stack: [dict, key, ref value]
                        var tryGetValueMethod = node.Object.Type.GetMethod("TryGetValue", BindingFlags.Public | BindingFlags.Instance);
                        if(tryGetValueMethod == null)
                            throw new MissingMethodException(node.Object.Type.Name, "TryGetValue");
                        context.Il.Call(tryGetValueMethod, node.Object.Type); // stack: [dict.TryGetValue(key, out value)]
                        var loadResultLabel = context.Il.DefineLabel("loadResult");
                        context.Il.Brtrue(loadResultLabel); // if(dict.TryGetValue(key, out result)) goto loadResult; stack: []
                        if(valueType.IsValueType)
                        {
                            context.Il.Ldloca(value);
                            context.Il.Initobj(valueType); // value = default(valueType)
                            context.Il.Ldloc(value); // stack: [default(valueType)]
                        }
                        else if(valueType == typeof(string))
                            context.Il.Ldstr("");
                        else
                            context.Create(valueType);
                        context.Il.Stloc(value);
                        if(extend)
                        {
                            context.Il.Ldloc(dict); // stack: [dict]
                            context.Il.Ldloc(key); // stack: [dict, key]
                            context.Il.Ldloc(value); // stack: [dict, key, value]
                            var setter = node.Indexer.GetSetMethod(context.SkipVisibility);
                            if(setter == null)
                                throw new MissingMethodException(node.Indexer.ReflectedType.ToString(), "set_" + node.Indexer.Name);
                            context.Il.Call(setter, node.Object.Type); // dict.set_Item(key, default(valueType)); stack: []
                        }
                        context.MarkLabelAndSurroundWithSP(loadResultLabel);
                        context.Il.Ldloc(value);
                    }
                }
                if(doneLabel != null)
                    context.MarkLabelAndSurroundWithSP(doneLabel);
                else
                    context.Il.Call(getter, node.Object.Type);
            }
            else
            {
                Type arrayType = node.Object.Type;
                if(!arrayType.IsArray)
                    throw new InvalidOperationException("An array expected");
                int rank = arrayType.GetArrayRank();
                if(rank != node.Arguments.Count)
                    throw new InvalidOperationException("Incorrect number of indeces '" + node.Arguments.Count + "' provided to access an array with rank '" + rank + "'");
                Type indexType = node.Arguments.First().Type;
                if(indexType != typeof(int))
                    throw new InvalidOperationException("Indexing array with an index of type '" + indexType + "' is not allowed");
                context.EmitLoadArguments(node.Arguments.ToArray());
                MethodInfo getMethod = arrayType.GetMethod("Get");
                if(getMethod == null)
                    throw new MissingMethodException(arrayType.ToString(), "Get");
                context.Il.Call(getMethod, arrayType);
            }
            resultType = node.Type;
            return result;
        }
    }
}
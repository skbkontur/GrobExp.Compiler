using System;
using System.Linq;
using System.Linq.Expressions;

using GrEmit;

namespace GrobExp.Compiler.ExpressionEmitters
{
    internal class BlockExpressionEmitter : ExpressionEmitter<BlockExpression>
    {
        protected override bool EmitInternal(BlockExpression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            var variables = node.Variables.Where(variable => !context.VariablesToLocals.ContainsKey(variable)).ToArray();
            foreach(var variable in variables)
            {
                var local = string.IsNullOrEmpty(variable.Name)
                                ? context.Il.DeclareLocal(variable.Type)
                                : context.Il.DeclareLocal(variable.Type, variable.Name, appendUniquePrefix : false);
                if(context.DebugInfoGenerator != null)
                    local.SetLocalSymInfo(local.Name);
                context.VariablesToLocals.Add(variable, local);
                context.Variables.Push(variable);
            }
            resultType = typeof(void);
            for(var index = 0; index < node.Expressions.Count; index++)
            {
                var expression = node.Expressions[index];
                var il = context.Il;
                var valueIsNullLabel = il.DefineLabel("valueIsNull");
                var labelUsed = ExpressionEmittersCollection.Emit(expression, context, valueIsNullLabel, index < node.Expressions.Count - 1 ? ResultType.Void : whatReturn, extend, out resultType);
                if(resultType != typeof(void) && index < node.Expressions.Count - 1)
                {
                    // eat results of all expressions except the last one
                    if(resultType.IsStruct())
                    {
                        using(var temp = context.DeclareLocal(resultType))
                            context.Il.Stloc(temp);
                    }
                    else context.Il.Pop();
                }
                if(labelUsed)
                {
                    var doneLabel = il.DefineLabel("done");
                    il.Br(doneLabel);
                    context.MarkLabelAndSurroundWithSP(valueIsNullLabel);
                    il.Pop();
                    if(resultType != typeof(void) && index == node.Expressions.Count - 1)
                    {
                        // return default value for the last expression in the block
                        context.EmitLoadDefaultValue(resultType);
                    }
                    context.MarkLabelAndSurroundWithSP(doneLabel);
                }
            }
            if(node.Type == typeof(bool) && resultType == typeof(bool?))
            {
                resultType = typeof(bool);
                context.ConvertFromNullableBoolToBool();
            }
            else if(node.Type == typeof(void) && resultType != typeof(void))
            {
                // eat result of the last expression if the result of block is void
                if(resultType.IsStruct())
                {
                    using(var temp = context.DeclareLocal(resultType))
                        context.Il.Stloc(temp);
                }
                else context.Il.Pop();
                resultType = typeof(void);
            }
            foreach(var variable in variables)
            {
                context.VariablesToLocals.Remove(variable);
                context.Variables.Pop();
            }
            return false;
        }
    }
}
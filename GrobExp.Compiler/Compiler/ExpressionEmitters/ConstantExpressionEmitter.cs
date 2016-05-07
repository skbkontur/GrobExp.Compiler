using System;
using System.Linq.Expressions;

using GrEmit;

namespace GrobExp.Compiler.ExpressionEmitters
{
    internal class ConstantExpressionEmitter : ExpressionEmitter<ConstantExpression>
    {
        protected override bool EmitInternal(ConstantExpression node, EmittingContext context, GroboIL.Label returnDefaultValueLabel, ResultType whatReturn, bool extend, out Type resultType)
        {
            if(node.Value == null)
                context.EmitLoadDefaultValue(node.Type);
            else
            {
                var typeCode = Type.GetTypeCode(node.Type.IsNullable() ? node.Type.GetGenericArguments()[0] : node.Type);
                switch(typeCode)
                {
                case TypeCode.Boolean:
                    context.Il.Ldc_I4(((bool)node.Value) ? 1 : 0);
                    break;
                case TypeCode.Byte:
                    context.Il.Ldc_I4((byte)node.Value);
                    break;
                case TypeCode.Char:
                    context.Il.Ldc_I4((char)node.Value);
                    break;
                case TypeCode.Int16:
                    context.Il.Ldc_I4((short)node.Value);
                    break;
                case TypeCode.Int32:
                    context.Il.Ldc_I4((int)node.Value);
                    break;
                case TypeCode.SByte:
                    context.Il.Ldc_I4((sbyte)node.Value);
                    break;
                case TypeCode.UInt16:
                    context.Il.Ldc_I4((ushort)node.Value);
                    break;
                case TypeCode.UInt32:
                    unchecked
                    {
                        context.Il.Ldc_I4((int)(uint)node.Value);
                    }
                    break;
                case TypeCode.Int64:
                    context.Il.Ldc_I8((long)node.Value);
                    break;
                case TypeCode.UInt64:
                    unchecked
                    {
                        context.Il.Ldc_I8((long)(ulong)node.Value);
                    }
                    break;
                case TypeCode.Single:
                    context.Il.Ldc_R4((float)node.Value);
                    break;
                case TypeCode.Double:
                    context.Il.Ldc_R8((double)node.Value);
                    break;
                case TypeCode.String:
                    context.Il.Ldstr((string)node.Value);
                    break;
                default:
                    throw new NotSupportedException("Constant of type '" + node.Type + "' is not supported");
                }
                if(node.Type.IsNullable())
                    context.Il.Newobj(node.Type.GetConstructor(new[] {node.Type.GetGenericArguments()[0]}));
            }
            resultType = node.Type;
            return false;
        }
    }
}
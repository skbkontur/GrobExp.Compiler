using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Compiler.Tests.ExtensionTests
{
    public class ForEachExpression : Expression
    {
        public ForEachExpression(Expression enumerable, Type elementType, LambdaExpression body)
        {
            Enumerable = enumerable;
            ElementType = elementType;
            Body = body;
        }

        public override Expression Reduce()
        {
            Type enumeratorType = typeof(IEnumerator<>).MakeGenericType(ElementType);
            ParameterExpression enumerator = Parameter(enumeratorType, "enumerator");
            MethodInfo getEnumeratorMethod = typeof(IEnumerable<>).MakeGenericType(ElementType).GetMethod("GetEnumerator");
            MethodInfo resetMethod = typeof(IEnumerator).GetMethod("Reset");
            MethodInfo moveNextMethod = typeof(IEnumerator).GetMethod("MoveNext");
            PropertyInfo currentProperty = enumeratorType.GetProperty("Current");
            LabelTarget breakLabel = Label();
            return Block(
                new[] {enumerator},
                Assign(enumerator, Call(Enumerable, getEnumeratorMethod)),
                Call(enumerator, resetMethod),
                Loop(
                    Block(
                        IfThen(Not(Call(enumerator, moveNextMethod)), Break(breakLabel)),
                        Invoke(Body, Call(enumerator, currentProperty.GetGetMethod()))
                        ),
                    breakLabel
                    )
                );
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public Expression Enumerable { get; private set; }
        public Type ElementType { get; set; }
        public LambdaExpression Body { get; private set; }

        public override ExpressionType NodeType { get { return ExpressionType.Extension; } }

        public override Type Type { get { return typeof(void); } }

        public override bool CanReduce { get { return true; } }

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            return base.VisitChildren(visitor);
        }

        protected override Expression Accept(ExpressionVisitor visitor)
        {
            return base.Accept(visitor);
        }
    }
}
using System;
using System.Linq.Expressions;

namespace GrobExp.Compiler
{
    public class TypedDebugInfoExpression : Expression
    {
        public TypedDebugInfoExpression(SymbolDocumentInfo document, Expression expression, int startLine, int startColumn, int endLine, int endColumn)
        {
            Document = document;
            this.Expression = expression;
            StartLine = startLine;
            StartColumn = startColumn;
            EndLine = endLine;
            EndColumn = endColumn;
            DebugInfo = DebugInfo(Document, StartLine, StartColumn, EndLine, EndColumn);
        }

        public TypedDebugInfoExpression(Expression expression, DebugInfoExpression debugInfo)
        {
            Document = debugInfo.Document;
            this.Expression = expression;
            StartLine = debugInfo.StartLine;
            StartColumn = debugInfo.StartColumn;
            EndLine = debugInfo.EndLine;
            EndColumn = debugInfo.EndColumn;
            this.DebugInfo = debugInfo;
        }

        /// <summary>
        ///     Gets the static type of the expression that this <see cref="Expression" /> represents. (Inherited from <see cref="Expression" />.)
        /// </summary>
        /// <returns>The <see cref="Type" /> that represents the static type of the expression.</returns>
        public sealed override Type Type { get { return Expression.Type; } }

        public Expression Expression { get; }

        public new DebugInfoExpression DebugInfo { get; }

        /// <summary>
        ///     Returns the node type of this <see cref="Expression" />. (Inherited from <see cref="Expression" />.)
        /// </summary>
        /// <returns>The <see cref="ExpressionType" /> that represents this expression.</returns>
        public sealed override ExpressionType NodeType { get { return ExpressionType.DebugInfo; } }

        public int StartLine { get; }

        public int StartColumn { get; }

        public int EndLine { get; }

        public int EndColumn { get; }

        /// <summary>
        ///     Gets the <see cref="SymbolDocumentInfo" /> that represents the source file.
        /// </summary>
        public SymbolDocumentInfo Document { get; }

        public override Expression Reduce()
        {
            return Expression;
        }

        protected override Expression Accept(ExpressionVisitor visitor)
        {
            return this;
        }

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            return this;
        }

        public override bool CanReduce { get { return true; } }
    }
}
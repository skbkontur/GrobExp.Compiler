using System;
using System.Linq.Expressions;

namespace GrobExp.Compiler
{
    public class TypedDebugInfoExpression : Expression
    {
        private readonly SymbolDocumentInfo _document;
        private readonly Expression expression;
        private readonly int _startLine, _startColumn, _endLine, _endColumn;
        private readonly DebugInfoExpression debugInfo;

        public TypedDebugInfoExpression(SymbolDocumentInfo document, Expression expression, int startLine, int startColumn, int endLine, int endColumn)
        {
            _document = document;
            this.expression = expression;
            _startLine = startLine;
            _startColumn = startColumn;
            _endLine = endLine;
            _endColumn = endColumn;
            debugInfo = Expression.DebugInfo(Document, StartLine, StartColumn, EndLine, EndColumn);
        }

        public TypedDebugInfoExpression(Expression expression, DebugInfoExpression debugInfo)
        {
            _document = debugInfo.Document;
            this.expression = expression;
            _startLine = debugInfo.StartLine;
            _startColumn = debugInfo.StartColumn;
            _endLine = debugInfo.EndLine;
            _endColumn = debugInfo.EndColumn;
            this.debugInfo = debugInfo;
        }

        /// <summary>
        /// Gets the static type of the expression that this <see cref="Expression" /> represents. (Inherited from <see cref="Expression"/>.)
        /// </summary>
        /// <returns>The <see cref="Type"/> that represents the static type of the expression.</returns>
        public sealed override Type Type
        {
            get { return expression.Type; }
        }

        public Expression Expression { get {  return expression;} }

        public new DebugInfoExpression DebugInfo { get { return debugInfo; } }

        /// <summary>
        /// Returns the node type of this <see cref="Expression" />. (Inherited from <see cref="Expression" />.)
        /// </summary>
        /// <returns>The <see cref="ExpressionType"/> that represents this expression.</returns>
        public sealed override ExpressionType NodeType
        {
            get { return ExpressionType.DebugInfo; }
        }

        public int StartLine
        {
            get
            {
                return _startLine;
            }
        }

        public int StartColumn
        {
            get
            {
                return _startColumn;
            }
        }

        public int EndLine
        {
            get
            {
                return _endLine;
            }
        }

        public int EndColumn
        {
            get
            {
                return _endColumn;
            }
        }

        /// <summary>
        /// Gets the <see cref="SymbolDocumentInfo"/> that represents the source file.
        /// </summary>
        public SymbolDocumentInfo Document
        {
            get { return _document; }
        }

        public override Expression Reduce()
        {
            return expression;
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
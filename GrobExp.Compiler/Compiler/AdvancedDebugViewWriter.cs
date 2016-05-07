/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the  Apache License, Version 2.0, please send an email to 
 * dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 *
 * ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

using GrEmit.Utils;

#if CLR2
namespace Microsoft.Scripting.Ast {
#else
namespace GrobExp.Compiler
{
    public sealed class LambdaTraverser : ExpressionVisitor
    {
        private readonly Dictionary<LambdaExpression, LambdaExpression> newLambdaReference;

        public LambdaTraverser(Dictionary<LambdaExpression, LambdaExpression> newLambdaReference)
        {
            this.newLambdaReference = newLambdaReference;
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            var lambda = node as LambdaExpression;
            lambda = newLambdaReference[lambda];
            var newBody = Visit(lambda.Body);
            return Expression.Lambda<T>(newBody, lambda.Name, lambda.TailCall, lambda.Parameters);
        }
    }

    public static class MethodExtensions
    {
        public static bool IsExtension(this MethodInfo method)
        {
            return method.GetCustomAttributes(typeof(ExtensionAttribute), false).Any();
        }

        public static Type GetGenericArgument(this Type type)
        {
            var args = type.GetGenericArguments();
            if (args.Length == 0)
                return null;
            return args[0];
        }
    }

    public enum BlockType
    {
        None, Body, Return
    }

#endif
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    public sealed class AdvancedDebugViewWriter : ExpressionVisitor
    {
        [Flags]
        private enum Flow
        {
            None,
            Space,
            NewLine,

            Break = 0x8000      // newline if column > MaxColumn
        };

        private const int breakArgsCount = 2;

        private const int Tab = 4;
        private const int MaxColumn = 120;

        private TextWriter _out;
        private int _column;

        private Stack<int> _stack = new Stack<int>();
        private int _delta;
        private Flow _flow;

        // All the unique lambda expressions in the ET, will be used for displaying all
        // the lambda definitions.
        private Queue<LambdaExpression> _lambdas;

        // Associate every unique anonymous LambdaExpression in the tree with an integer.
        // The id is used to create a name for the anonymous lambda.
        //
        private Dictionary<LambdaExpression, int> _lambdaIds;

        // Associate every unique anonymous parameter or variable in the tree with an integer.
        // The id is used to create a name for the anonymous parameter or variable.
        //
        private Dictionary<ParameterExpression, int> _paramIds;

        // Associate every unique anonymous LabelTarget in the tree with an integer.
        // The id is used to create a name for the anonymous LabelTarget.
        //
        private Dictionary<LabelTarget, int> _labelIds;

        // Result string
        public StringBuilder result;

        // Constants object
        private readonly Type constantsType;
        private readonly ParameterExpression constantsParam;
        private readonly object constants;

        // DebugInfo section
        private class SelectionItem
        {
            public int StartRow { get; set; }
            public int StartColumn { get; set; }
            public bool PassSpaces { get; set; }

            public SelectionItem(int startRow, int startColumn)
            {
                StartRow = startRow;
                StartColumn = startColumn;
                PassSpaces = true;
            }
        }

        private int row;
        private readonly Stack<SelectionItem> selectionStack = new Stack<SelectionItem>();
        private readonly SymbolDocumentInfo symbolDocument;

        private void StartSelection()
        {
            selectionStack.Push(new SelectionItem(row, _column));
        }

        DebugInfoExpression GenerateDebugInfo(int r, int c, int nr, int nc)
        {
            return Expression.DebugInfo(symbolDocument, r + 1, c + 1, nr + 1, nc + 1);
        }

        DebugInfoExpression EndSelection()
        {
            if (selectionStack.Count == 0)
                throw new NotSupportedException();
            var current = selectionStack.Pop();
            return GenerateDebugInfo(current.StartRow, current.StartColumn, row, _column);
        }

        Expression GetBlock(Expression e)
        {
            var sel = EndSelection();
            if(e.NodeType == ExpressionType.Block ||
               e.NodeType == ExpressionType.Conditional)
                return e;
            return Expression.Block(sel, e);
        }
        // End DebugInfo section

        private AdvancedDebugViewWriter(Type constantsType, ParameterExpression constantsParam, object constants, string filename)
        {
            symbolDocument = Expression.SymbolDocument(filename, Guid.Empty, Guid.Empty, Guid.Empty);
            this.constantsType = constantsType;
            this.constantsParam = constantsParam;
            this.constants = constants;
            this.result = new StringBuilder();
        }

        private int Base
        {
            get
            {
                return _stack.Count > 0 ? _stack.Peek() : 0;
            }
        }

        private int Delta
        {
            get { return _delta; }
        }

        private int Depth
        {
            get { return Base + Delta; }
        }

        private void Indent()
        {
            _delta += Tab;
        }
        private void Dedent()
        {
            _delta -= Tab;
        }

        private void NewLine()
        {
            _flow = Flow.NewLine;
        }

        private static int GetId<T>(T e, ref Dictionary<T, int> ids)
        {
            if (ids == null)
            {
                ids = new Dictionary<T, int>();
                ids.Add(e, 1);
                return 1;
            }
            else
            {
                int id;
                if (!ids.TryGetValue(e, out id))
                {
                    // e is met the first time
                    id = ids.Count + 1;
                    ids.Add(e, id);
                }
                return id;
            }
        }

        private int GetLambdaId(LambdaExpression le)
        {
            Debug.Assert(String.IsNullOrEmpty(le.Name));
            return GetId(le, ref _lambdaIds);
        }

        private int GetParamId(ParameterExpression p)
        {
            Debug.Assert(String.IsNullOrEmpty(p.Name));
            return GetId(p, ref _paramIds);
        }

        private int GetLabelTargetId(LabelTarget target)
        {
            Debug.Assert(String.IsNullOrEmpty(target.Name));
            return GetId(target, ref _labelIds);
        }

        public static LambdaExpression WriteToModifying(Expression node, Type constantsType, ParameterExpression constantsParam,
            object constants, string filename)
        {
            var visitor = new AdvancedDebugViewWriter(constantsType, constantsParam, constants, filename);
            var newNode = visitor.WriteTo(node);
            var debugOutput = visitor.result;

            try
            {
                File.WriteAllText(filename, debugOutput.ToString());
            }
            catch(IOException)
            {
                //nothing
            }

            return newNode;
        }

        private LambdaExpression WriteTo(Expression node)
        {
            var lambda = node as LambdaExpression;
            LambdaExpression res;
            if (lambda != null)
            {
                res = WriteLambda(lambda);
            }
            else
            {
                throw new NotSupportedException("Only lambdas are allowed");
                /*
                Visit(node);
                Debug.Assert(_stack.Count == 0);
                */
            }

            var newLambdaReference = new Dictionary<LambdaExpression, LambdaExpression>();
            newLambdaReference[res] = res;

            //
            // Output all lambda expression definitions.
            // in the order of their appearances in the tree.
            //
            while (_lambdas != null && _lambdas.Count > 0)
            {
                WriteLine();
                WriteLine();
                var oldLambda = _lambdas.Dequeue();
                newLambdaReference[oldLambda] = WriteLambda(oldLambda);
            }

            res = new LambdaTraverser(newLambdaReference).Visit(res) as LambdaExpression;
            return res;
        }

        #region The printing code

        private void Out(string s)
        {
            Out(Flow.None, s, Flow.None);
        }

        private void Out(Flow before, string s)
        {
            Out(before, s, Flow.None);
        }

        private void Out(string s, Flow after)
        {
            Out(Flow.None, s, after);
        }

        private void Out(Flow before, string s, Flow after)
        {
            switch (GetFlow(before))
            {
                case Flow.None:
                    break;
                case Flow.Space:
                    Write(" ");
                    break;
                case Flow.NewLine:
                    WriteLine();
                    Write(new String(' ', Depth));
                    break;
            }
            Write(s);
            _flow = after;
        }

        private void WriteLine()
        {
            //_out.WriteLine();
            result.Append('\n');
            row++;
            _column = 0;
            foreach (var element in selectionStack)
                if (element.PassSpaces)
                {
                    element.StartRow = row;
                    element.StartColumn = _column;
                }
        }

        private void Write(string s)
        {
            int i = 0;
            for (i = 0; i < s.Length && s[i] == ' '; i++)
                foreach (var element in selectionStack)
                    if (element.PassSpaces)
                        element.StartColumn++;
            if (i < s.Length)
                foreach (var element in selectionStack)
                    element.PassSpaces = false;
            result.Append(s);
            //_out.Write(s);
            _column += s.Length;
        }

        private Flow GetFlow(Flow flow)
        {
            Flow last;

            last = CheckBreak(_flow);
            flow = CheckBreak(flow);

            // Get the biggest flow that is requested None < Space < NewLine
            return (Flow)Math.Max((int)last, (int)flow);
        }

        private Flow CheckBreak(Flow flow)
        {
            if ((flow & Flow.Break) != 0)
            {
                if (_column > (MaxColumn/* + Depth*/))
                {
                    flow = Flow.NewLine;
                }
                else
                {
                    flow &= ~Flow.Break;
                }
            }
            return flow;
        }

        #endregion

        #region The AST Output

        // More proper would be to make this a virtual method on Action
        private static string FormatBinder(CallSiteBinder binder)
        {
            ConvertBinder convert;
            GetMemberBinder getMember;
            SetMemberBinder setMember;
            DeleteMemberBinder deleteMember;
            InvokeMemberBinder call;
            UnaryOperationBinder unary;
            BinaryOperationBinder binary;

            if ((convert = binder as ConvertBinder) != null)
            {
                return "Convert " + Formatter.Format(convert.Type);
            }
            else if ((getMember = binder as GetMemberBinder) != null)
            {
                return "GetMember " + getMember.Name;
            }
            else if ((setMember = binder as SetMemberBinder) != null)
            {
                return "SetMember " + setMember.Name;
            }
            else if ((deleteMember = binder as DeleteMemberBinder) != null)
            {
                return "DeleteMember " + deleteMember.Name;
            }
            else if (binder is GetIndexBinder)
            {
                return "GetIndex";
            }
            else if (binder is SetIndexBinder)
            {
                return "SetIndex";
            }
            else if (binder is DeleteIndexBinder)
            {
                return "DeleteIndex";
            }
            else if ((call = binder as InvokeMemberBinder) != null)
            {
                return "Call " + call.Name;
            }
            else if (binder is InvokeBinder)
            {
                return "Invoke";
            }
            else if (binder is CreateInstanceBinder)
            {
                return "Create";
            }
            else if ((unary = binder as UnaryOperationBinder) != null)
            {
                return "UnaryOperation " + unary.Operation;
            }
            else if ((binary = binder as BinaryOperationBinder) != null)
            {
                return "BinaryOperation " + binary.Operation;
            }
            else
            {
                return binder.ToString();
            }
        }

        //ignore TypedDebugInfoExpression
        public override Expression Visit(Expression node)
        {
            if(node is TypedDebugInfoExpression)
            {
                throw new NotSupportedException("Nobody should use TypedDebugInfoExpression, it is dangerous!");
                /*
                base.Visit(((TypedDebugInfoExpression)node).Expression);
                return node;
                */
            }
            return base.Visit(node);
        }

        private List<object> VisitExpressions<T>(char open, IList<T> expressions) where T : Expression
        {
            return VisitExpressions<T>(open, ',', expressions);
        }

        private List<object> VisitExpressions<T>(char open, char separator, IList<T> expressions,
            BlockType blockType = BlockType.None) where T : Expression
        {
            return VisitExpressions(open, separator, expressions, Visit, blockType);
        }

        private List<object> VisitDeclarations(IList<ParameterExpression> expressions)
        {
            return VisitExpressions('(', ',', expressions, variable =>
            {
                Out(Formatter.Format(variable.Type));
                if(variable.IsByRef)
                {
                    Out("&");
                }
                Out(" ");
                return VisitParameter(variable);
            });
        }

        //open = 0 means no brackets
        private List<object> VisitExpressions<T>(char open, char separator, IList<T> expressions,
            Func<T, object> visit, BlockType blockType = BlockType.None)
        {
            var nonSemicolonFlow = Flow.NewLine;
            if(open != '{' && expressions.Count < breakArgsCount)
                nonSemicolonFlow = Flow.Space | Flow.Break;

            if (open != '0' && blockType != BlockType.Return)
                Out(open.ToString());

            var newBlock = new List<object>();

            if (expressions != null)
            {
                //if (separator == ';')
                    Indent();
                bool isFirst = true;
                foreach (T e in expressions)
                {
                    if (isFirst)
                    {
                        if (open == '{' || expressions.Count >= breakArgsCount)
                            NewLine();
                        isFirst = false;
                    }
                    else
                    {
                        if(separator != ';')
                        {
                            _flow &= ~Flow.Break;
                            Out(separator.ToString(), nonSemicolonFlow);
                        }
                    }
                    bool isComplex = IsComplexArgument(e);

                    bool needSelection = false;
                    var eAsExp = e as Expression;
                    if(eAsExp != null)
                    {
                        needSelection = eAsExp.NodeType != ExpressionType.Conditional &&
                                        eAsExp.NodeType != ExpressionType.Block;
                    }

                    if (separator == ';' || isComplex)
                        StartSelection();
                    if (blockType == BlockType.Return)
                        Out("return ");
                    var cursorDump = Tuple.Create(row, _column);
                    var newExp = visit(e);
                    bool anyText = !Equals(cursorDump, Tuple.Create(row, _column));
                    if(separator == ';' && anyText)
                    {
                        _flow &= ~Flow.Break;
                        Out(separator.ToString(), Flow.NewLine);
                    }
                    DebugInfoExpression debugInfo = null;
                    if (separator == ';' || isComplex)
                        debugInfo = EndSelection();
                    if((separator == ';' || isComplex) && anyText && newExp is Expression && needSelection)
                        newBlock.Add(Expression.Block(debugInfo, newExp as Expression));
                    else
                        newBlock.Add(newExp);
                }
                //if (separator == ';')
                    Dedent();
            }

            char close = '0';
            switch (open)
            {
                case '(': close = ')'; break;
                case '{': close = '}'; break;
                case '[': close = ']'; break;
                case '<': close = '>'; break;
                //default:
            }

            if(close != '0' && blockType != BlockType.Body)
            {
                _flow &= ~Flow.Break;
                Out(nonSemicolonFlow == Flow.NewLine ? Flow.NewLine : Flow.None, close.ToString(), Flow.Break);
            }

            return newBlock;
        }

        private bool IsComplexArgument(object obj)
        {
            var node = obj as Expression;
            if(node == null)
                return false;

            if(node.NodeType == ExpressionType.Constant || node.NodeType == ExpressionType.Parameter)
                return false;

            if(node.NodeType == ExpressionType.Convert)
                return IsComplexArgument((node as UnaryExpression).Operand);
            if(node.NodeType == ExpressionType.MemberAccess)
            {
                var bin = node as MemberExpression;
                return IsComplexArgument(bin.Expression) || IsComplexArgument(bin.Member);
            }

            return true;
        }

        protected override Expression VisitDynamic(DynamicExpression node)
        {
            Out("dynamic", Flow.Space);
            Out(FormatBinder(node.Binder));
            var newArguments = VisitExpressions('(', node.Arguments).Cast<Expression>();
            return node.Update(newArguments);
        }

        private bool? GetBooleanConstantValue(Expression node)
        {
            var constant = node as ConstantExpression;
            if(constant == null)
                return null;
            var value = constant.Value as bool?;
            return value;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        protected override Expression VisitBinary(BinaryExpression node)
        {
            Expression newLeft = node.Left, newRight = node.Right;
            if (node.NodeType == ExpressionType.ArrayIndex)
            {
                newLeft = ParenthesizedVisit(node, node.Left);
                Out("[");
                newRight = Visit(node.Right);
                Out("]");
            }
            else if(node.NodeType == ExpressionType.Equal && GetBooleanConstantValue(node.Right) == true) //TODO redundant brackets
            {
                newLeft = ParenthesizedVisit(node, node.Left);
            }
            else
            {
                bool parenthesizeLeft = NeedsParentheses(node, node.Left);
                bool parenthesizeRight = NeedsParentheses(node, node.Right);

                string op;
                bool isChecked = false;
                Flow beforeOp = Flow.Space;
                switch (node.NodeType)
                {
                    case ExpressionType.Assign: op = "="; break;
                    case ExpressionType.Equal: op = "=="; break;
                    case ExpressionType.NotEqual: op = "!="; break;
                    case ExpressionType.AndAlso: op = "&&"; beforeOp = Flow.Break | Flow.Space; break;
                    case ExpressionType.OrElse: op = "||"; beforeOp = Flow.Break | Flow.Space; break;
                    case ExpressionType.GreaterThan: op = ">"; break;
                    case ExpressionType.LessThan: op = "<"; break;
                    case ExpressionType.GreaterThanOrEqual: op = ">="; break;
                    case ExpressionType.LessThanOrEqual: op = "<="; break;
                    case ExpressionType.Add: op = "+"; break;
                    case ExpressionType.AddAssign: op = "+="; break;
                    case ExpressionType.AddAssignChecked: op = "+="; isChecked = true; break;
                    case ExpressionType.AddChecked: op = "+"; isChecked = true; break;
                    case ExpressionType.Subtract: op = "-"; break;
                    case ExpressionType.SubtractAssign: op = "-="; break;
                    case ExpressionType.SubtractAssignChecked: op = "-="; isChecked = true; break;
                    case ExpressionType.SubtractChecked: op = "-"; isChecked = true; break;
                    case ExpressionType.Divide: op = "/"; break;
                    case ExpressionType.DivideAssign: op = "/="; break;
                    case ExpressionType.Modulo: op = "%"; break;
                    case ExpressionType.ModuloAssign: op = "%="; break;
                    case ExpressionType.Multiply: op = "*"; break;
                    case ExpressionType.MultiplyAssign: op = "*="; break;
                    case ExpressionType.MultiplyAssignChecked: op = "*="; isChecked = true; break;
                    case ExpressionType.MultiplyChecked: op = "*"; isChecked = true; break;
                    case ExpressionType.LeftShift: op = "<<"; break;
                    case ExpressionType.LeftShiftAssign: op = "<<="; break;
                    case ExpressionType.RightShift: op = ">>"; break;
                    case ExpressionType.RightShiftAssign: op = ">>="; break;
                    case ExpressionType.And: op = "&"; break;
                    case ExpressionType.AndAssign: op = "&="; break;
                    case ExpressionType.Or: op = "|"; break;
                    case ExpressionType.OrAssign: op = "|="; break;
                    case ExpressionType.ExclusiveOr: op = "^"; break;
                    case ExpressionType.ExclusiveOrAssign: op = "^="; break;
                    case ExpressionType.Power: op = "**"; break;
                    case ExpressionType.PowerAssign: op = "**="; break;
                    case ExpressionType.Coalesce: op = "??"; break;

                    default:
                        throw new InvalidOperationException();
                }

                if (parenthesizeLeft)
                {
                    Out("(", Flow.None);
                }

                newLeft = Visit(node.Left);
                if (parenthesizeLeft)
                {
                    Out(Flow.None, ")", Flow.Break);
                }

                // prepend # to the operator to represent checked op
                if (isChecked)
                {
                    op = String.Format(
                            CultureInfo.CurrentCulture,
                            "{0}",
                            op
                    );
                }
                Out(beforeOp, op, Flow.Space | Flow.Break);

                if (parenthesizeRight)
                {
                    Out("(", Flow.None);
                }
                newRight = Visit(node.Right);
                if (parenthesizeRight)
                {
                    Out(Flow.None, ")", Flow.Break);
                }
            }
            return node.Update(newLeft, node.Conversion, newRight);
        }

        private readonly Dictionary<string, int> nameParametersCounter = new Dictionary<string, int>();
        private readonly Dictionary<ParameterExpression, string> parameterNames = new Dictionary<ParameterExpression, string>();
        private readonly FieldInfo field = typeof(ParameterExpression).GetField("_name", BindingFlags.Instance | BindingFlags.NonPublic);

        private string GetName(ParameterExpression param)
        {
            if(!parameterNames.ContainsKey(param))
            {
                string typeName = BeutifyName(Formatter.Format(param.Type));
                int counter;
                if (!nameParametersCounter.TryGetValue(typeName, out counter))
                    nameParametersCounter.Add(typeName, counter = 1);
                var newName = typeName + (counter == 1 ? "" : "_" + counter);
                nameParametersCounter[typeName]++;
                parameterNames.Add(param, newName);
                return newName;
            }
            return parameterNames[param];
        }

        private string BeutifyName(string name)
        {
            if(char.IsUpper(name[0]))
                name = char.ToLower(name[0]) + name.Substring(1);

            int position = name.LastIndexOf('_');
            if(position != -1)
                name = name.Substring(0, position);

            position = name.IndexOf('<');
            if(position != -1)
                name = name.Substring(0, position);

            return name;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if(string.IsNullOrEmpty(node.Name))
            {
                var name = GetName(node);
                field.SetValue(node, name);
            }

            // Have '$' for the DebugView of ParameterExpressions
            //Out("$");
            if (String.IsNullOrEmpty(node.Name))
            {
                // If no name if provided, generate a name as $var1, $var2.
                // No guarantee for not having name conflicts with user provided variable names.
                //
                int id = GetParamId(node);
                Out("var" + id);
            }
            else
            {
                Out(GetDisplayName(node.Name));
            }
            return node;
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            Out(
                String.Format(CultureInfo.CurrentCulture,
                    "{0} {1}<{2}>",
                    "LAMBDA",
                    GetLambdaName(node),
                    Formatter.Format(node.Type)
                )
            );

            if (_lambdas == null)
            {
                _lambdas = new Queue<LambdaExpression>();
            }

            // N^2 performance, for keeping the order of the lambdas.
            if (!_lambdas.Contains(node))
            {
                _lambdas.Enqueue(node);
            }

            return node;
        }

        private static bool IsSimpleExpression(Expression node)
        {
            var binary = node as BinaryExpression;
            if (binary != null)
            {
                return !(binary.Left is BinaryExpression || binary.Right is BinaryExpression);
            }

            return false;
        }

        private bool HasEmptyBody(Expression node)
        {
            var defaultNode = node as DefaultExpression;
            if(defaultNode == null)
                return false;
            return defaultNode.Type == typeof(void);
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            Expression newTest = node.Test, newTrue = node.IfTrue, newFalse = node.IfFalse;

            if(node.Type != typeof(void))
            {
                StartSelection();
                var ifTestBody = Visit(node.Test);
                newTest = GetBlock(ifTestBody);

                Out(Flow.NewLine, "?", Flow.Space);
                StartSelection();
                var ifTrueBody = Visit(node.IfTrue);
                newTrue = GetBlock(ifTrueBody);

                Out(Flow.NewLine, ":", Flow.Space);
                StartSelection();
                var ifFalseBody = Visit(node.IfFalse);
                newFalse = GetBlock(ifFalseBody);

                return node.Update(newTest, newTrue, newFalse);
            }

            Out("IF (");
            StartSelection();
            var newTestBody = Visit(node.Test);
            newTest = GetBlock(newTestBody);
            Out(") {", Flow.NewLine);

            Indent();
            StartSelection();
            var newTrueBody = Visit(node.IfTrue);
            newTrue = GetBlock(newTrueBody);
            Dedent();
            if(!HasEmptyBody(node.IfFalse))
            {
                Out(Flow.NewLine, "} ELSE {", Flow.NewLine);
                Indent();
                StartSelection();
                var newFalseBody = Visit(node.IfFalse);
                newFalse = GetBlock(newFalseBody);
                Dedent();
                Out(Flow.NewLine, "}");
            }
            else
            {
                Out(Flow.NewLine, "}");
            }
            return node.Update(newTest, newTrue, newFalse);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            object value = node.Value;

            if (value == null)
            {
                Out("null");
            }
            else if ((value is string) && node.Type == typeof(string))
            {
                Out(String.Format(
                    CultureInfo.CurrentCulture,
                    "\"{0}\"",
                    value));
            }
            else if ((value is char) && node.Type == typeof(char))
            {
                Out(String.Format(
                    CultureInfo.CurrentCulture,
                    "'{0}'",
                    value));
            }
            else if ((value is int) && node.Type == typeof(int)
              || (value is bool) && node.Type == typeof(bool))
            {
                Out(value.ToString());
            }
            else
            {
                string suffix = GetConstantValueSuffix(node.Type);
                if (suffix != null)
                {
                    Out(value.ToString());
                    Out(suffix);
                }
                else
                {
                    Out(String.Format(
                        CultureInfo.CurrentCulture,
                        "const<{0}>({1})",
                        Formatter.Format(node.Type),
                        value));
                }
            }
            return node;
        }

        private static string GetConstantValueSuffix(Type type)
        {
            if (type == typeof(UInt32))
            {
                return "U";
            }
            if (type == typeof(Int64))
            {
                return "L";
            }
            if (type == typeof(UInt64))
            {
                return "UL";
            }
            if (type == typeof(Double))
            {
                return "D";
            }
            if (type == typeof(Single))
            {
                return "F";
            }
            if (type == typeof(Decimal))
            {
                return "M";
            }
            return null;
        }

        protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            Out(".RuntimeVariables");
            var newVariables = VisitExpressions('(', node.Variables);
            return node.Update(newVariables.Cast<ParameterExpression>());
        }

        // Prints ".instanceField" or "declaringType.staticField"
        private Expression OutMember(Expression node, Expression instance, MemberInfo member)
        {
            if (instance != null)
            {
                var res = ParenthesizedVisit(node, instance);
                Out("." + member.Name);
                return res;
            }
            else
            {
                // For static members, include the type name
                Out(Formatter.Format(member.DeclaringType) + "." + member.Name);
            }
            return instance;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            /*
            if(node.Expression != null && node.Expression.NodeType == ExpressionType.Parameter)
            {
                var param = (ParameterExpression)node.Expression;
                if(param == constantsParam && node.Member.Name.StartsWith("<>c__"))
                    return node;
            }
             */
            var newExp = OutMember(node, node.Expression, node.Member);
            return node.Update(newExp);
        }

        protected override Expression VisitInvocation(InvocationExpression node)
        {
            if(node.Expression.NodeType == ExpressionType.Convert)
            {
                var convert = (UnaryExpression)node.Expression;
                if(convert.Operand.NodeType == ExpressionType.MemberAccess)
                {
                    var access = (MemberExpression)convert.Operand;
                    if(access.Expression.NodeType == ExpressionType.Parameter)
                    {
                        var param = (ParameterExpression)access.Expression;
                        if(param == constantsParam)
                        {
                            var funcName = ExtractNameUsingReflection(access.Member.Name);
                            var result = GetRealPrivateName(funcName, "MethodInvoker$");

                            if(result != null)
                            {
                                var realName = result.Item2;
                                var className = result.Item1;
                                var arguments = node.Arguments.ToArray();

                                if(className == null)
                                {
                                    var obj = arguments[0];
                                    arguments = arguments.Skip(1).ToArray();
                                    var newArguments = new[] {Visit(obj)}.ToList();
                                    Out(".");
                                    Out(realName);
                                    newArguments.AddRange(VisitExpressions('(', arguments).Cast<Expression>());
                                    return node.Update(node.Expression, newArguments);
                                }
                                else
                                {
                                    Out(className);
                                    Out(".");
                                    Out(realName);
                                    var newArguments = VisitExpressions('(', arguments).Cast<Expression>().ToArray();
                                    return node.Update(node.Expression, newArguments);
                                }
                            }
                        }
                    }
                }
            }

            var newExp = ParenthesizedVisit(node, node.Expression);
            var newArgs = VisitExpressions('(', node.Arguments).Cast<Expression>();
            return node.Update(newExp, newArgs);
        }

        private static Tuple<string, string> GetRealPrivateName(string name, string prefix)
        {
            if(!name.StartsWith(prefix))
                return null;
            int firstPos = name.IndexOf('$');
            int secondPos = name.IndexOf('$', firstPos + 1);
            int thirdPos = name.IndexOf('$', secondPos + 1);
            if(thirdPos == -1)
                return Tuple.Create<string, string>(null, name.Substring(firstPos + 1, secondPos - firstPos - 1));
            return Tuple.Create(name.Substring(firstPos + 1, secondPos - firstPos - 1),
                name.Substring(secondPos + 1, thirdPos - secondPos - 1));
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private static bool NeedsParentheses(Expression parent, Expression child)
        {
            Debug.Assert(parent != null);
            if (child == null)
            {
                return false;
            }

            // Some nodes always have parentheses because of how they are
            // displayed, for example: ".Unbox(obj.Foo)"
            switch (parent.NodeType)
            {
                case ExpressionType.Increment:
                case ExpressionType.Decrement:
                case ExpressionType.IsTrue:
                case ExpressionType.IsFalse:
                case ExpressionType.Unbox:
                    return true;
            }

            int childOpPrec = GetOperatorPrecedence(child);
            int parentOpPrec = GetOperatorPrecedence(parent);

            if (childOpPrec == parentOpPrec)
            {
                // When parent op and child op has the same precedence,
                // we want to be a little conservative to have more clarity.
                // Parentheses are not needed if
                // 1) Both ops are &&, ||, &, |, or ^, all of them are the only
                // op that has the precedence.
                // 2) Parent op is + or *, e.g. x + (y - z) can be simplified to
                // x + y - z.
                // 3) Parent op is -, / or %, and the child is the left operand.
                // In this case, if left and right operand are the same, we don't
                // remove parenthesis, e.g. (x + y) - (x + y)
                // 
                switch (parent.NodeType)
                {
                    case ExpressionType.AndAlso:
                    case ExpressionType.OrElse:
                    case ExpressionType.And:
                    case ExpressionType.Or:
                    case ExpressionType.ExclusiveOr:
                        // Since these ops are the only ones on their precedence,
                        // the child op must be the same.
                        Debug.Assert(child.NodeType == parent.NodeType);
                        // We remove the parenthesis, e.g. x && y && z
                        return false;
                    case ExpressionType.Add:
                    case ExpressionType.AddChecked:
                    case ExpressionType.Multiply:
                    case ExpressionType.MultiplyChecked:
                        return false;
                    case ExpressionType.Subtract:
                    case ExpressionType.SubtractChecked:
                    case ExpressionType.Divide:
                    case ExpressionType.Modulo:
                        BinaryExpression binary = parent as BinaryExpression;
                        Debug.Assert(binary != null);
                        // Need to have parenthesis for the right operand.
                        return child == binary.Right;
                    case ExpressionType.MemberAccess:
                    case ExpressionType.Call:
                    case ExpressionType.ArrayLength:
                    case ExpressionType.ArrayIndex:
                    case ExpressionType.Index:
                    case ExpressionType.Convert:
                        return false;
                }
                return true;
            }

            // Special case: negate of a constant needs parentheses, to
            // disambiguate it from a negative constant.
            if (child != null && child.NodeType == ExpressionType.Constant &&
                (parent.NodeType == ExpressionType.Negate || parent.NodeType == ExpressionType.NegateChecked))
            {
                return true;
            }

            // If the parent op has higher precedence, need parentheses for the child.
            return childOpPrec < parentOpPrec;
        }

        // the greater the higher
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private static int GetOperatorPrecedence(Expression node)
        {
            // Roughly matches C# operator precedence, with some additional
            // operators. Also things which are not binary/unary expressions,
            // such as conditional and type testing, don't use this mechanism.
            switch (node.NodeType)
            {
                // Assignment
                case ExpressionType.Assign:
                case ExpressionType.ExclusiveOrAssign:
                case ExpressionType.AddAssign:
                case ExpressionType.AddAssignChecked:
                case ExpressionType.SubtractAssign:
                case ExpressionType.SubtractAssignChecked:
                case ExpressionType.DivideAssign:
                case ExpressionType.ModuloAssign:
                case ExpressionType.MultiplyAssign:
                case ExpressionType.MultiplyAssignChecked:
                case ExpressionType.LeftShiftAssign:
                case ExpressionType.RightShiftAssign:
                case ExpressionType.AndAssign:
                case ExpressionType.OrAssign:
                case ExpressionType.PowerAssign:
                case ExpressionType.Coalesce:
                    return 1;

                // Conditional (?:) would go here

                // Conditional OR
                case ExpressionType.OrElse:
                    return 2;

                // Conditional AND
                case ExpressionType.AndAlso:
                    return 3;

                // Logical OR
                case ExpressionType.Or:
                    return 4;

                // Logical XOR
                case ExpressionType.ExclusiveOr:
                    return 5;

                // Logical AND
                case ExpressionType.And:
                    return 6;

                // Equality
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    return 7;

                // Relational, type testing
                case ExpressionType.GreaterThan:
                case ExpressionType.LessThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.TypeAs:
                case ExpressionType.TypeIs:
                case ExpressionType.TypeEqual:
                    return 8;

                // Shift
                case ExpressionType.LeftShift:
                case ExpressionType.RightShift:
                    return 9;

                // Additive
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return 10;

                // Multiplicative
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return 11;

                // Unary
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.UnaryPlus:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.PreIncrementAssign:
                case ExpressionType.PreDecrementAssign:
                case ExpressionType.OnesComplement:
                case ExpressionType.Increment:
                case ExpressionType.Decrement:
                case ExpressionType.IsTrue:
                case ExpressionType.IsFalse:
                case ExpressionType.Unbox:
                case ExpressionType.Throw:
                    return 12;

                // Power, which is not in C#
                // But VB/Python/Ruby put it here, above unary.
                case ExpressionType.Power:
                    return 13;

                // Primary, which includes all other node types:
                //   member access, calls, indexing, new.
                case ExpressionType.PostIncrementAssign:
                case ExpressionType.PostDecrementAssign:
                default:
                    return 14;

                // These aren't expressions, so never need parentheses:
                //   constants, variables
                case ExpressionType.Constant:
                case ExpressionType.Parameter:
                    return 15;
            }
        }

        private Expression ParenthesizedVisit(Expression parent, Expression nodeToVisit)
        {
            if (NeedsParentheses(parent, nodeToVisit))
            {
                Out("(");
                var res = Visit(nodeToVisit);
                Out(")");
                return res;
            }
            else
            {
                return Visit(nodeToVisit);
            }
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            //Out(".Call ");

            var newArguments = node.Arguments;
            var newObject = node.Object;

            if (node.Method.IsExtension())
            {
                newArguments = new ReadOnlyCollection<Expression>(VisitExpressions('0', new[] {node.Arguments[0]}).Cast<Expression>().ToList());
                Out(".");
                Out(node.Method.Name);
                newArguments = new ReadOnlyCollection<Expression>(
                    newArguments.Concat(new ReadOnlyCollection<Expression>(
                        VisitExpressions('(', node.Arguments.Skip(1).ToArray())
                            .Cast<Expression>().ToList()).ToList()).ToList());
            }
            else
            {
                if(node.Object != null)
                {
                    newObject = ParenthesizedVisit(node, node.Object);
                }
                else if(node.Method.DeclaringType != null)
                {
                    Out(Formatter.Format(node.Method.DeclaringType));
                }
                else
                {
                    Out("<UnknownType>");
                }
                Out(".");
                Out(node.Method.Name);
                newArguments = new ReadOnlyCollection<Expression>(VisitExpressions('(', node.Arguments)
                    .Cast<Expression>().ToList());
            }
            return node.Update(newObject, newArguments);
        }

        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            var newExpressions = node.Expressions;
            if (node.NodeType == ExpressionType.NewArrayBounds)
            {
                // .NewArray MyType[expr1, expr2]
                Out("new " + Formatter.Format(node.Type.GetElementType()));
                newExpressions = new ReadOnlyCollection<Expression>(VisitExpressions('[', node.Expressions).Cast<Expression>().ToList());
            }
            else
            {
                // .NewArray MyType {expr1, expr2}
                Out("new " + Formatter.Format(node.Type), Flow.Space);
                newExpressions = new ReadOnlyCollection<Expression>(VisitExpressions('{', node.Expressions).Cast<Expression>().ToList());
            }
            return node.Update(newExpressions);
        }

        protected override Expression VisitNew(NewExpression node)
        {
            Out("new " + Formatter.Format(node.Type));
            var newArguments = VisitExpressions('(', node.Arguments);
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if(node.Constructor == null)
                return Expression.New(node.Type);
            return node.Update(newArguments.Cast<Expression>().ToList());
        }

        protected override ElementInit VisitElementInit(ElementInit node)
        {
            var newArg = node.Arguments;
            if (node.Arguments.Count == 1)
            {
                newArg = new ReadOnlyCollection<Expression>(new[] {Visit(node.Arguments[0])});
            }
            else
            {
                newArg = new ReadOnlyCollection<Expression>(VisitExpressions('{', node.Arguments).Cast<Expression>().ToList());
            }
            return node.Update(newArg);
        }

        protected override Expression VisitListInit(ListInitExpression node)
        {
            var newExp = Visit(node.NewExpression) as NewExpression;
            var newInits = VisitExpressions('{', ',', node.Initializers, VisitElementInit).Cast<ElementInit>();
            return node.Update(newExp, newInits);
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            bool isComplex = IsComplexArgument(assignment.Expression);
            if (isComplex)
                StartSelection();
            Out(assignment.Member.Name);
            Out(Flow.Space, "=", Flow.Space);
            var newExp = Visit(assignment.Expression);
            if(isComplex)
                return assignment.Update(GetBlock(newExp));
            return assignment.Update(newExp);
        }

        protected override MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            Out(binding.Member.Name);
            Out(Flow.Space, "=", Flow.Space);
            var newInits = VisitExpressions('{', ',', binding.Initializers, VisitElementInit).Cast<ElementInit>();
            return binding.Update(newInits);
        }

        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            Out(binding.Member.Name);
            Out(Flow.Space, "=", Flow.Space);
            var newBinds = VisitExpressions('{', ',', binding.Bindings, VisitMemberBinding).Cast<MemberBinding>();
            return binding.Update(newBinds);
        }

        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            var newExp = Visit(node.NewExpression) as NewExpression;
            Out(Flow.Space, "");
            var newBinds = VisitExpressions('{', ',', node.Bindings, VisitMemberBinding).Cast<MemberBinding>();
            return node.Update(newExp, newBinds);
        }

        protected override Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            var newExp = ParenthesizedVisit(node, node.Expression);
            switch (node.NodeType)
            {
                case ExpressionType.TypeIs:
                    Out(Flow.Space, ".Is", Flow.Space);
                    break;
                case ExpressionType.TypeEqual:
                    Out(Flow.Space, ".TypeEqual", Flow.Space);
                    break;
            }
            Out(Formatter.Format(node.TypeOperand));
            return node.Update(newExp);
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        protected override Expression VisitUnary(UnaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Convert:

                if(node.Operand.NodeType == ExpressionType.Invoke)
                {
                    var invoke = (InvocationExpression)node.Operand;
                    if(invoke.Expression.NodeType == ExpressionType.MemberAccess)
                    {
                        var access = (MemberExpression)invoke.Expression;
                        if(access.Expression.NodeType == ExpressionType.Parameter)
                        {
                            var param = (ParameterExpression)access.Expression;
                            if(param == constantsParam)
                            {
                                var funcName = ExtractNameUsingReflection(access.Member.Name);
                                var result = GetRealPrivateName(funcName, "FieldExtractor$");

                                if(result != null)
                                {
                                    var realName = result.Item2;
                                    var className = result.Item1;

                                    if(className == null)
                                    {
                                        var cursorDump = Tuple.Create(row, _column);
                                        var obj = Visit(invoke.Arguments[0]);
                                        if (!Equals(cursorDump, Tuple.Create(row, _column)))
                                            Out(".");
                                        Out(realName);
                                        return node.Update(invoke.Update(invoke.Expression, new[] {obj}));
                                    }
                                    else
                                    {
                                        Out(className);
                                        Out(".");
                                        Out(realName);
                                        return node;
                                    }
                                }
                            }
                        }
                    }
                }

                if(node.Type != node.Operand.Type &&
                   !(node.Type.IsGenericType &&
                     node.Type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                     node.Type.GetGenericArgument() == node.Operand.Type))
                {
                    Out("(" + Formatter.Format(node.Type) + ")");
                }
                break;

                case ExpressionType.ConvertChecked:
                    Out("(" + Formatter.Format(node.Type) + ")");
                    break;
                case ExpressionType.TypeAs:
                    break;
                case ExpressionType.Not:
                    Out(node.Type == typeof(bool) ? "!" : "~");
                    break;
                case ExpressionType.OnesComplement:
                    Out("~");
                    break;
                case ExpressionType.Negate:
                    Out("-");
                    break;
                case ExpressionType.NegateChecked:
                    Out("-");
                    break;
                case ExpressionType.UnaryPlus:
                    Out("+");
                    break;
                case ExpressionType.ArrayLength:
                    break;
                case ExpressionType.Quote:
                    Out("'");
                    break;
                case ExpressionType.Throw:
                    if (node.Operand == null)
                    {
                        Out("RETHROW");
                    }
                    else
                    {
                        Out("THROW", Flow.Space);
                    }
                    break;
                case ExpressionType.IsFalse:
                    Out(".IsFalse");
                    break;
                case ExpressionType.IsTrue:
                    Out(".IsTrue");
                    break;
                case ExpressionType.Decrement:
                    Out(".Decrement");
                    break;
                case ExpressionType.Increment:
                    Out(".Increment");
                    break;
                case ExpressionType.PreDecrementAssign:
                    Out("--");
                    break;
                case ExpressionType.PreIncrementAssign:
                    Out("++");
                    break;
                case ExpressionType.Unbox:
                    Out(".Unbox");
                    break;
            }

            var newOperand = ParenthesizedVisit(node, node.Operand);

            switch (node.NodeType)
            {
                case ExpressionType.TypeAs:
                    Out(Flow.Space, ".As", Flow.Space | Flow.Break);
                    Out(Formatter.Format(node.Type));
                    break;

                case ExpressionType.ArrayLength:
                    Out(".Length");
                    break;

                case ExpressionType.PostDecrementAssign:
                    Out("--");
                    break;

                case ExpressionType.PostIncrementAssign:
                    Out("++");
                    break;
            }
            return node.Update(newOperand);
        }

        private string ExtractNameUsingReflection(string memberName)
        {
            var reflectionA = constantsType.GetField(memberName);
            var reflectionB = reflectionA.GetValue(constants);
            var reflectionC = reflectionB.GetType().GetProperty("Method");
            var reflectionD = reflectionC.GetValue(reflectionB, null);
            var reflectionE = typeof(MethodInfo).GetProperty("Name");
            return (string)reflectionE.GetValue(reflectionD, null);
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            Out("BLOCK");

            // Display <type> if the type of the BlockExpression is different from the
            // last expression's type in the block.
            if (node.Type != node.Expressions.Last().Type)
                Out(String.Format(CultureInfo.CurrentCulture, "<{0}>", Formatter.Format(node.Type)));

            List<Expression> newBlock;

            var newVars = VisitDeclarations(node.Variables).Cast<ParameterExpression>();
            Out(" ");
            // Use ; to separate expressions in the block
            if(node.Type != typeof(void))
            {
                newBlock = VisitExpressions('{', ';', node.Expressions.Take(node.Expressions.Count - 1).ToArray(), BlockType.Body)
                    .Cast<Expression>().ToList();
                newBlock = newBlock.Concat(VisitExpressions('{', ';', new[] { node.Expressions.Last() }, BlockType.Return)
                    .Cast<Expression>().ToList()).ToList();
            }
            else
            {
                newBlock = VisitExpressions('{', ';', node.Expressions).Cast<Expression>().ToList();
            }

            return node.Update(newVars, newBlock);
        }

        protected override Expression VisitDefault(DefaultExpression node)
        {
            if (node.Type != typeof(void))
                Out("default(" + Formatter.Format(node.Type) + ")");
            return node;
        }

        protected override Expression VisitLabel(LabelExpression node)
        {
            Out("LABEL", Flow.Space);
            Indent();
            var newDefault = Visit(node.DefaultValue);
            Dedent();
            NewLine();
            DumpLabel(node.Target);
            return node.Update(node.Target, newDefault);
        }

        protected override Expression VisitGoto(GotoExpression node)
        {
            Out(node.Kind.ToString().ToUpper(), Flow.Space);
            Out(GetLabelTargetName(node.Target), Flow.Space);
            Out("{", Flow.Space);
            var newValue = Visit(node.Value);
            Out(Flow.Space, "}");
            return node.Update(node.Target, newValue);
        }

        protected override Expression VisitLoop(LoopExpression node)
        {
            Out("LOOP", Flow.Space);
            if (node.ContinueLabel != null)
            {
                DumpLabel(node.ContinueLabel);
            }
            Out("{", Flow.NewLine);
            Indent();
            var newBody = Visit(node.Body);
            Dedent();
            Out(Flow.NewLine, "}");
            if (node.BreakLabel != null)
            {
                Out("", Flow.NewLine);
                DumpLabel(node.BreakLabel);
            }
            return node.Update(node.BreakLabel, node.ContinueLabel, newBody);
        }

        protected override SwitchCase VisitSwitchCase(SwitchCase node)
        {
            var newBody = node.Body;
            foreach (var test in node.TestValues)
            {
                Out("CASE (");
                newBody = Visit(test);
                Out("):", Flow.NewLine);
            }
            Indent(); Indent();
            newBody = Visit(node.Body);
            Dedent(); Dedent();
            NewLine();
            return node.Update(node.TestValues, newBody);
        }

        protected override Expression VisitSwitch(SwitchExpression node)
        {
            Out("SWITCH ");
            Out("(");
            var newSwitchValue = Visit(node.SwitchValue);
            Out(") {", Flow.NewLine);
            var newCases = Visit(node.Cases, VisitSwitchCase);
            var newDefault = node.DefaultBody;
            if (node.DefaultBody != null)
            {
                Out("DEFAULT:", Flow.NewLine);
                Indent(); Indent();
                newDefault = Visit(node.DefaultBody);
                Dedent(); Dedent();
                NewLine();
            }
            Out("}");
            return node.Update(newSwitchValue, newCases, newDefault);
        }

        protected override CatchBlock VisitCatchBlock(CatchBlock node)
        {
            Out(Flow.NewLine, "} CATCH (" + node.Test.ToString());
            var newVar = node.Variable;
            var newFilter = node.Filter;
            if (node.Variable != null)
            {
                Out(Flow.Space, "");
                newVar = VisitParameter(node.Variable) as ParameterExpression;
            }
            if (node.Filter != null)
            {
                Out(") IF (", Flow.Break);
                newFilter = Visit(node.Filter);
            }
            Out(") {", Flow.NewLine);
            Indent();
            var newBody = Visit(node.Body);
            Dedent();
            return node.Update(newVar, newFilter, newBody);
        }

        protected override Expression VisitTry(TryExpression node)
        {
            Out("TRY {", Flow.NewLine);
            Indent();
            var newBody = Visit(node.Body);
            Dedent();
            var newHandlers = Visit(node.Handlers, VisitCatchBlock);
            var newFinally = node.Finally;
            var newFault = node.Fault;
            if (node.Finally != null)
            {
                Out(Flow.NewLine, "} FINALLY {", Flow.NewLine);
                Indent();
                newFinally = Visit(node.Finally);
                Dedent();
            }
            else if (node.Fault != null)
            {
                Out(Flow.NewLine, "} FAULT {", Flow.NewLine);
                Indent();
                newFault = Visit(node.Fault);
                Dedent();
            }

            Out(Flow.NewLine, "}");
            return node.Update(newBody, newHandlers, newFinally, newFault);
        }

        protected override Expression VisitIndex(IndexExpression node)
        {
            var newObj = node.Object;
            var newArgs = node.Arguments;

            if (node.Indexer != null)
            {
                newObj = OutMember(node, node.Object, node.Indexer);
            }
            else
            {
                newObj = ParenthesizedVisit(node, node.Object);
            }

            newArgs = new ReadOnlyCollection<Expression>(VisitExpressions('[', node.Arguments).Cast<Expression>().ToList());
            return node.Update(newObj, newArgs);
        }

        //TODO
        protected override Expression VisitExtension(Expression node)
        {
            Out(String.Format(CultureInfo.CurrentCulture, ".Extension<{0}>", Formatter.Format(node.GetType())));

            if (node.CanReduce)
            {
                Out(Flow.Space, "{", Flow.NewLine);
                Indent();
                Visit(node.Reduce());
                Dedent();
                Out(Flow.NewLine, "}");
            }

            return node;
        }

        protected override Expression VisitDebugInfo(DebugInfoExpression node)
        {
            /*
            Out(String.Format(
                CultureInfo.CurrentCulture,
                ".DebugInfo({0}: {1}, {2} - {3}, {4})",
                node.Document.FileName,
                node.StartLine,
                node.StartColumn,
                node.EndLine,
                node.EndColumn)
            );
             */
            return node;
        }


        private void DumpLabel(LabelTarget target)
        {
            Out(String.Format(CultureInfo.CurrentCulture, "{0}:", GetLabelTargetName(target)));
        }

        private string GetLabelTargetName(LabelTarget target)
        {
            if (string.IsNullOrEmpty(target.Name))
            {
                // Create the label target name as #Label1, #Label2, etc.
                return String.Format(CultureInfo.CurrentCulture, "#Label{0}", GetLabelTargetId(target));
            }
            else
            {
                return GetDisplayName(target.Name);
            }
        }

        private LambdaExpression WriteLambda(LambdaExpression lambda)
        {
            Out(
                String.Format(
                    CultureInfo.CurrentCulture,
                    "LAMBDA {0}<{1}>",
                    GetLambdaName(lambda),
                    Formatter.Format(lambda.Type))
            );

            var newParams = VisitDeclarations(lambda.Parameters).Cast<ParameterExpression>();

            var body = lambda.Body;
            if (body.NodeType != ExpressionType.Block)
                body = Expression.Block(body);

            Out(Flow.Space, "{", Flow.NewLine);
            Indent();
            var newBody = Visit(body);
            Dedent();
            Out(Flow.NewLine, "}");
            Debug.Assert(_stack.Count == 0);

            return Expression.Lambda(lambda.Type, newBody, lambda.Name, lambda.TailCall, newParams);
        }

        private string GetLambdaName(LambdaExpression lambda)
        {
            if (String.IsNullOrEmpty(lambda.Name))
            {
                return "#Lambda" + GetLambdaId(lambda);
            }
            return GetDisplayName(lambda.Name);
        }

        /// <summary>
        /// Return true if the input string contains any whitespace character.
        /// Otherwise false.
        /// </summary>
        private static bool ContainsWhiteSpace(string name)
        {
            foreach (char c in name)
            {
                if (Char.IsWhiteSpace(c))
                {
                    return true;
                }
            }
            return false;
        }

        private static string QuoteName(string name)
        {
            return String.Format(CultureInfo.CurrentCulture, "'{0}'", name);
        }

        private static string GetDisplayName(string name)
        {
            if (ContainsWhiteSpace(name))
            {
                // if name has whitespaces in it, quote it
                return QuoteName(name);
            }
            else
            {
                return name;
            }
        }

        #endregion
    }
}
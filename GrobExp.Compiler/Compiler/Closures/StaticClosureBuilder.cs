using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace GrobExp.Compiler.Closures
{
    internal class StaticClosureBuilder : IClosureBuilder
    {
        public int DefineField(Type type)
        {
            if(tree == null)
                tree = new Node();
            var fieldId = tree.Count;
            var isPrivateType = DynamicClosureBuilder.IsPrivate(type) && type.IsValueType;
            var fieldType = !isPrivateType ? type : typeof(StrongBox<>).MakeGenericType(type);

            var child = tree.Add(fields, new Field
                {
                    Type = fieldType,
                    Id = fieldId,
                    IsPrivateType = isPrivateType
                });
            fields.Add(child);
            return fieldId;
        }

        private static readonly Type[] tupleTypes = typeof(StaticClosures).GetNestedTypes()
                                                                          .OrderBy(type => type.GetGenericArguments().Length).ToArray();

        public Type Create()
        {
            return tree == null ? typeof(object) : tree.Create();
        }

        public Expression MakeAccess(ParameterExpression root, int id)
        {
            var result = MakeAccessInternal(root, id);
            var node = fields[id];
            if (node.Field.IsPrivateType)
                result = Expression.Field(result, node.Field.Type.GetField("Value", BindingFlags.Public | BindingFlags.Instance));
            return result;
        }

        private Expression MakeAccessInternal(ParameterExpression root, int id)
        {
            var node = fields[id];
            var path = new List<Node>();
            var cur = node;
            while (cur.Parent != null)
            {
                path.Add(cur);
                cur = cur.Parent;
            }
            path.Add(cur);
            path.Reverse();
            Expression result = root;
            for (int i = 1; i < path.Count; ++i)
            {
                var idx = path[i - 1].Children.IndexOf(path[i]);
                result = Expression.Field(result, result.Type.GetField("item" + (idx + 1)));
            }
            return result;
        }

        public Expression Assign(ParameterExpression root, int id, Expression value)
        {
            var result = MakeAccessInternal(root, id);
            var node = fields[id];
            if (node.Field.IsPrivateType)
                value = Expression.New(node.Field.Type.GetConstructor(new[] { value.Type }), value);
            return Expression.Assign(result, value);
        }

        public Expression Init(ParameterExpression root)
        {
            return Expression.Assign(root, Init(root.Type));
        }

        private static Expression Init(Type type)
        {
            return Expression.MemberInit(Expression.New(type),
                                         from field in type.GetFields(BindingFlags.Instance | BindingFlags.Public)
                                         where field.FieldType.IsNested && field.FieldType.DeclaringType == typeof(StaticClosures)
                                         select Expression.Bind(field, Init(field.FieldType))
                );
        }

        private class Node
        {
            private readonly List<Node> children = new List<Node>();
            public List<Node> Children { get { return children; } }
            public Field Field { get; set; }
            public int Depth { get; set; }
            public int Count { get; set; }
            public Node Parent { get; set; }

            public Type Create()
            {
                if(Field != null)
                    return Field.Type;
                return tupleTypes[children.Count - 1].MakeGenericType(children.Select(child => child.Create()).ToArray());
            }

            public Node Add(List<Node> fields, Field field)
            {
                Count++;
                if (children.Count < tupleTypes.Length)
                {
                    var node = new Node
                        {
                            Parent = this,
                            Field = field,
                            Depth = 0,
                            Count = 1
                        };
                    children.Add(node);
                    node.UpdateDepth();
                    return node;
                }
                foreach(var child in children)
                {
                    if(!child.Saturated())
                        return child.Add(fields, field);
                }
                int minCount = int.MaxValue;
                Node minimalChild = null;
                foreach(var child in children)
                    if(child.Count < minCount)
                    {
                        minCount = child.Count;
                        minimalChild = child;
                    }
                var leaf = minimalChild.FindLeaf();
                return leaf.Split(fields, field);
            }

            private Node Split(List<Node> fields, Field field)
            {
                children.Add(new Node
                    {
                        Field = Field,
                        Parent = this,
                        Depth = 0,
                        Count = 1
                    });
                fields[children[0].Field.Id] = children[0];
                Field = null;
                var node = new Node
                    {
                        Parent = this,
                        Field = field,
                        Depth = 0,
                        Count = 1
                    };
                children.Add(node);
                Count = 2;
                node.UpdateDepth();
                return node;
            }

            private void UpdateDepth()
            {
                var node = this;
                while (node.Parent != null)
                {
                    if (node.Parent.Depth < node.Depth + 1)
                        node.Parent.Depth = node.Depth + 1;
                    node = node.Parent;
                }
            }

            private Node FindLeaf()
            {
                if(Field != null) return this;
                int minDepth = int.MaxValue;
                Node minimalChild = null;
                foreach(var child in children)
                {
                    if(child.Depth < minDepth)
                    {
                        minDepth = child.Depth;
                        minimalChild = child;
                    }
                }
                return minimalChild.FindLeaf();
            }

            private bool Saturated()
            {
                if(Field != null) return true;
                if(children.Count != tupleTypes.Length)
                    return false;
                return children.All(child => child.Saturated());
            }
        }

        private Node tree;
        private readonly List<Node> fields = new List<Node>();

        private class Field
        {
            public Type Type { get; set; }
            public int Id { get; set; }
            public bool IsPrivateType { get; set; }
        }
    }
}
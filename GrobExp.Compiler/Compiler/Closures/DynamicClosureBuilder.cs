using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;

namespace GrobExp.Compiler.Closures
{
    internal class DynamicClosureBuilder : IClosureBuilder
    {
        private readonly ModuleBuilder module;
        private TypeBuilder typeBuilder;

        public DynamicClosureBuilder(ModuleBuilder module)
        {
            this.module = module;
        }

        public Type Create()
        {
            return typeBuilder == null ? typeof(object) : typeBuilder.CreateType();
        }

        public Expression MakeAccess(ParameterExpression root, int id)
        {
            var field = fields[id];
            var fieldInfo = root.Type.GetField(field.Name);
            var result = Expression.Field(root, fieldInfo);
            if(field.IsPrivateType)
                result = Expression.Field(result, fieldInfo.FieldType.GetField("Value", BindingFlags.Public | BindingFlags.Instance));
            return result;
        }

        public Expression Assign(ParameterExpression root, int id, Expression value)
        {
            var field = fields[id];
            var fieldInfo = root.Type.GetField(field.Name);
            if(field.IsPrivateType)
                value = Expression.New(fieldInfo.FieldType.GetConstructor(new[] {value.Type}), value);
            return Expression.Assign(Expression.Field(root, fieldInfo), value);
        }

        public Expression Init(ParameterExpression root)
        {
            return Expression.Assign(root, Expression.New(root.Type));
        }

        public int DefineField(Type type)
        {
            if(typeBuilder == null)
            {
                var id = (uint)Interlocked.Increment(ref closureId);
                typeBuilder = module.DefineType("Closure_" + id, TypeAttributes.Public | TypeAttributes.Class);
            }
            int fieldId = fields.Count;
            var fieldName = GrEmit.Utils.Formatter.Format(type) + "_" + fieldId;
            var isPrivateType = IsPrivate(type) && type.IsValueType;
            var fieldType = !isPrivateType ? type : typeof(StrongBox<>).MakeGenericType(type);
            typeBuilder.DefineField(fieldName, fieldType, FieldAttributes.Public);
            fields.Add(new Field
                {
                    Name = fieldName,
                    IsPrivateType = isPrivateType
                });
            return fieldId;
        }
        
        public static bool IsPrivate(Type type)
        {
            // TODO check this method
            if (type.IsNestedPrivate || type.IsNotPublic)
                return true;
            return type.IsGenericType && type.GetGenericArguments().Any(IsPrivate);
        }

        private readonly List<Field> fields = new List<Field>();
        private static int closureId;

        private class Field
        {
            public string Name { get; set; }
            public bool IsPrivateType { get; set; }
        }
    }
}
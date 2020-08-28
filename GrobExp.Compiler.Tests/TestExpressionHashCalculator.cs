using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NUnit.Framework;

using static GrobExp.Compiler.ExpressionHashCalculator;

namespace GrobExp.Compiler.Tests
{
    [TestFixture]
    public class TestExpressionHashCalculator
    {
        [Test]
        public void TestHashParameterSameTypes()
        {
            var parameter = Expression.Parameter(typeof(int), "parameter");
            var otherParameterOfSameType = Expression.Parameter(typeof(int), "other_parameter");

            TestHashEquivalent(Expression.Parameter(typeof(int)), Expression.Parameter(typeof(int)));
            TestHashEquivalent(parameter, otherParameterOfSameType, strictly : false);
            TestHashNotEquivalent(parameter, otherParameterOfSameType, strictly : true);
        }

        [Test]
        public void TestHashParameterDifferentTypes()
        {
            var parameter = Expression.Parameter(typeof(int));
            var otherParameter = Expression.Parameter(typeof(string));

            TestHashNotEquivalent(parameter, otherParameter);
        }

        [Test]
        public void TestHashParameterDifferentGenericTypes()
        {
            var parameter = Expression.Parameter(typeof(List<string>));
            var otherParameter = Expression.Parameter(typeof(List<int>));

            TestHashNotEquivalent(parameter, otherParameter);
        }

        [Test]
        public void TestHashParameterDifferentArrayTypes()
        {
            var parameter = Expression.Parameter(typeof(int[]));
            var otherParameter = Expression.Parameter(typeof(string[]));

            TestHashNotEquivalent(parameter, otherParameter);
        }

        [Test]
        public void TestHashParameterDifferentArrayRankTypes()
        {
            var parameter = Expression.Parameter(typeof(int[,]));
            var otherParameter = Expression.Parameter(typeof(int[]));

            TestHashNotEquivalent(parameter, otherParameter);
        }

        [Test]
        public void TestHashConstant()
        {
            var constant = Expression.Constant(1, typeof(int));
            var otherConstant = Expression.Constant(2, typeof(int));
            var otherTypeConstant = Expression.Constant(1L, typeof(long));

            TestHashEquivalent(Expression.Constant(1), Expression.Constant(1));
            TestHashNotEquivalent(constant, otherConstant);
            TestHashNotEquivalent(constant, otherTypeConstant);
        }

        [Test]
        public void TestHashRespectNodeType()
        {
            var parameter = Expression.Parameter(typeof(int));
            var increment = Expression.Increment(parameter);
            var decrement = Expression.Decrement(parameter);

            TestHashNotEquivalent(increment, decrement);
        }

        [Test]
        public void TestHashRespectType()
        {
            var incrementLong = Expression.Increment(Expression.Parameter(typeof(long)));
            var incrementInt = Expression.Increment(Expression.Parameter(typeof(int)));

            TestHashNotEquivalent(incrementInt, incrementLong);
        }

        [Test]
        public void TestHashFromNull()
        {
            Assert.DoesNotThrow(() => CalcHashCode(null, true));
            Assert.DoesNotThrow(() => CalcHashCode(null, false));
        }

        [Test]
        public void TestHashBinaryArithmetic()
        {
            TestHashBinaryExpression(typeof(double),
                                     Expression.Add,
                                     Expression.AddChecked,
                                     Expression.AddAssign,
                                     Expression.AddAssignChecked,
                                     Expression.Divide,
                                     Expression.DivideAssign,
                                     Expression.Modulo,
                                     Expression.ModuloAssign,
                                     Expression.Multiply,
                                     Expression.MultiplyChecked,
                                     Expression.MultiplyAssign,
                                     Expression.MultiplyAssignChecked,
                                     Expression.Power,
                                     Expression.PowerAssign,
                                     Expression.Subtract,
                                     Expression.SubtractChecked,
                                     Expression.SubtractAssign,
                                     Expression.SubtractAssignChecked,
                                     Expression.Assign
            );
        }

        [Test]
        public void TestHashBinaryBit()
        {
            TestHashBinaryExpression(typeof(int),
                                     Expression.And,
                                     Expression.AndAssign,
                                     Expression.Or,
                                     Expression.OrAssign,
                                     Expression.ExclusiveOr,
                                     Expression.ExclusiveOrAssign,
                                     Expression.LeftShift,
                                     Expression.LeftShiftAssign,
                                     Expression.RightShift,
                                     Expression.RightShiftAssign,
                                     Expression.Assign
            );
        }

        [Test]
        public void TestHashBinaryComparison()
        {
            TestHashBinaryExpression(typeof(int),
                                     Expression.Equal,
                                     Expression.NotEqual,
                                     Expression.GreaterThan,
                                     Expression.GreaterThanOrEqual,
                                     Expression.LessThan,
                                     Expression.LessThanOrEqual
            );
        }

        [Test]
        public void TestHashBinaryConditional()
        {
            TestHashBinaryExpression(typeof(bool),
                                     Expression.AndAlso,
                                     Expression.OrElse
            );
        }

        [Test]
        public void TestHashBinaryArrayIndex()
        {
            var array = Expression.Parameter(typeof(string[]), "array");
            var otherArray = Expression.Parameter(typeof(string[]), "other_array");
            var index = Expression.Parameter(typeof(int));

            TestHashEquivalent(Expression.ArrayIndex(Expression.Parameter(typeof(string[])), index),
                               Expression.ArrayIndex(Expression.Parameter(typeof(string[])), index));
            TestHashEquivalent(Expression.ArrayIndex(array, index), Expression.ArrayIndex(otherArray, index), strictly : false);
            TestHashNotEquivalent(Expression.ArrayIndex(array, index), Expression.ArrayIndex(otherArray, index), strictly : true);
        }

        [Test]
        public void TestHashMakeIndex()
        {
            var indexers = typeof(TestObject).GetProperties().Where(p => p.GetIndexParameters().Length > 0).ToList();
            Assert.That(indexers.Count, Is.EqualTo(2));
            var firstIndexer = indexers[0];
            var secondIndexer = indexers[1];

            TestHashNotEquivalent(Expression.MakeIndex(Expression.Parameter(typeof(TestObject)), firstIndexer, new[] {Expression.Constant(1)}),
                                  Expression.MakeIndex(Expression.Parameter(typeof(TestObject)), secondIndexer, new[] {Expression.Constant(1L)}));
        }

        [Test]
        public void TestHashBinaryWithExplicitMethod()
        {
            var a = Expression.Parameter(typeof(int), "a");
            var b = Expression.Parameter(typeof(int), "b");
            var c = Expression.Parameter(typeof(int), "c");

            var add = typeof(TestObject).GetMethod(nameof(TestObject.Add));
            var otherAdd = typeof(TestObject).GetMethod(nameof(TestObject.OtherAdd));

            TestHashEquivalent(Expression.Add(a, b, add), Expression.Add(a, b, add));
            TestHashNotEquivalent(Expression.Add(a, b, add), Expression.Add(a, b));
            TestHashNotEquivalent(Expression.Add(a, b, add), Expression.Add(a, b, otherAdd));
            TestHashEquivalent(Expression.Add(a, b, add), Expression.Add(a, c, add), strictly : false);
            TestHashNotEquivalent(Expression.Add(a, b, add), Expression.Add(a, c, add), strictly : true);
        }

        [Test]
        public void TestHashBinaryCoalesce()
        {
            var first = Expression.Parameter(typeof(int?), "first");
            var otherFirst = Expression.Parameter(typeof(int?), "other_first");
            var second = Expression.Parameter(typeof(int));

            TestHashEquivalent(Expression.Coalesce(first, second), Expression.Coalesce(first, second));
            TestHashEquivalent(Expression.Coalesce(first, second), Expression.Coalesce(otherFirst, second), strictly : false);
            TestHashNotEquivalent(Expression.Coalesce(first, second), Expression.Coalesce(otherFirst, second), strictly : true);
        }

        [Test]
        public void TestHashBinaryCoalesceWithConversion()
        {
            var first = Expression.Parameter(typeof(string));
            var second = Expression.Parameter(typeof(int));
            Expression<Func<string, int>> conversion = x => 1;
            Expression<Func<string, int>> otherConversion = x => int.Parse(x);

            TestHashNotEquivalent(Expression.Coalesce(first, second, conversion), Expression.Coalesce(first, second, otherConversion));
        }

        [Test]
        public void TestHashUnaryArithmetic()
        {
            TestHashUnaryExpression(typeof(int),
                                    Expression.Negate,
                                    Expression.NegateChecked,
                                    Expression.UnaryPlus,
                                    Expression.Increment,
                                    Expression.PreIncrementAssign,
                                    Expression.PostIncrementAssign,
                                    Expression.Decrement,
                                    Expression.PreDecrementAssign,
                                    Expression.PostDecrementAssign,
                                    Expression.OnesComplement
            );
        }

        [Test]
        public void TestHashUnaryLogical()
        {
            TestHashUnaryExpression(typeof(bool),
                                    Expression.IsTrue,
                                    Expression.IsFalse,
                                    Expression.Not
            );
        }

        [Test]
        public void TestHashUnaryArrayLength()
        {
            var array = Expression.Parameter(typeof(int[]), "array");
            var otherArray = Expression.Parameter(typeof(int[]), "other_array");

            TestHashEquivalent(Expression.ArrayLength(array), Expression.ArrayLength(array));
            TestHashNotEquivalent(Expression.ArrayLength(array), Expression.ArrayLength(Expression.TypeAs(array, typeof(object[]))));
            TestHashEquivalent(Expression.ArrayLength(array), Expression.ArrayLength(otherArray), strictly : false);
            TestHashNotEquivalent(Expression.ArrayLength(array), Expression.ArrayLength(otherArray), strictly : true);
        }

        [Test]
        public void TestHashUnaryTypeConversion()
        {
            TestHashUnaryExpression(typeof(object),
                                    parameter => Expression.Convert(parameter, typeof(double)),
                                    parameter => Expression.Convert(parameter, typeof(string)),
                                    parameter => Expression.TypeAs(parameter, typeof(string)),
                                    parameter => Expression.TypeAs(parameter, typeof(int[]))
            );

            TestHashUnaryExpression(typeof(long), parameter => Expression.ConvertChecked(parameter, typeof(int)));
        }

        [Test]
        public void TestHashUnaryUnbox()
        {
            TestHashUnaryExpression(typeof(object),
                                    parameter => Expression.Unbox(parameter, typeof(int)),
                                    parameter => Expression.Unbox(parameter, typeof(long))
            );
        }

        [Test]
        public void TestHashUnaryThrow()
        {
            TestHashUnaryExpression(typeof(Expression), Expression.Throw);
        }

        [Test]
        public void TestHashUnaryQuote()
        {
            var parameter = Expression.Parameter(typeof(int));
            var firstLambda = Expression.Lambda(parameter, parameter);
            var secondLambda = Expression.Lambda(Expression.Add(parameter, Expression.Constant(2)), parameter);

            TestHashEquivalent(Expression.Quote(firstLambda), Expression.Quote(firstLambda));
            TestHashNotEquivalent(Expression.Quote(firstLambda), Expression.Quote(secondLambda));
        }

        [Test]
        public void TestHashCall()
        {
            var instance = Expression.Parameter(typeof(string), "instance");
            var otherInstance = Expression.Parameter(typeof(string), "other_instance");
            var method = typeof(string).GetMethod(nameof(string.PadLeft), new[] {typeof(int)});
            Assert.NotNull(method);
            var otherMethod = typeof(string).GetMethod(nameof(string.PadRight), new[] {typeof(int)});
            Assert.NotNull(otherMethod);
            var parameter = Expression.Constant(1);
            var otherParameter = Expression.Constant(2);

            TestHashEquivalent(Expression.Call(instance, method, parameter), Expression.Call(instance, method, parameter));
            TestHashEquivalent(Expression.Call(instance, method, parameter), Expression.Call(otherInstance, method, parameter), strictly : false);
            TestHashNotEquivalent(Expression.Call(instance, method, parameter), Expression.Call(otherInstance, method, parameter), strictly : true);
            TestHashNotEquivalent(Expression.Call(instance, method, parameter), Expression.Call(instance, otherMethod, parameter));
            TestHashNotEquivalent(Expression.Call(instance, method, parameter), Expression.Call(instance, method, otherParameter));
        }

        [Test]
        public void TestHashConditional()
        {
            var condition = Expression.Parameter(typeof(bool));
            var otherCondition = Expression.Constant(true);
            var ifTrue = Expression.Parameter(typeof(string));
            var otherIfTrue = Expression.Constant("zzz");
            var ifFalse = Expression.Parameter(typeof(string));
            var otherIfFalse = Expression.Constant("qxx");

            TestHashEquivalent(Expression.Condition(condition, ifTrue, ifFalse), Expression.Condition(condition, ifTrue, ifFalse));
            TestHashNotEquivalent(Expression.Condition(otherCondition, ifTrue, ifFalse), Expression.Condition(condition, ifTrue, ifFalse));
            TestHashNotEquivalent(Expression.Condition(condition, otherIfTrue, ifFalse), Expression.Condition(condition, ifTrue, ifFalse));
            TestHashNotEquivalent(Expression.Condition(condition, ifTrue, otherIfFalse), Expression.Condition(condition, ifTrue, ifFalse));
        }

        [Test]
        public void TestHashInvoke()
        {
            Expression<Func<int, int>> lambda = x => x + 2;
            Expression<Func<int, int>> otherLambda = x => x - 2;
            var parameter = Expression.Parameter(typeof(int), "parameter");
            var otherParameter = Expression.Parameter(typeof(int), "other_parameter");

            TestHashEquivalent(Expression.Invoke(lambda, parameter), Expression.Invoke(lambda, parameter));
            TestHashEquivalent(Expression.Invoke(lambda, parameter), Expression.Invoke(lambda, otherParameter), strictly : false);
            TestHashNotEquivalent(Expression.Invoke(lambda, parameter), Expression.Invoke(lambda, otherParameter), strictly : true);
            TestHashNotEquivalent(Expression.Invoke(lambda, parameter), Expression.Invoke(otherLambda, parameter));
        }

        [Test]
        public void TestHashListInit()
        {
            var constructor = typeof(List<int>).GetConstructor(Type.EmptyTypes);
            Assert.NotNull(constructor);
            var listAddMethod = typeof(List<int>).GetMethod("Add");

            TestHashNotEquivalent(Expression.ListInit(Expression.New(constructor), listAddMethod, Expression.Constant(1), Expression.Constant(2)),
                                  Expression.ListInit(Expression.New(constructor), listAddMethod, Expression.Constant(2), Expression.Constant(1)));
        }

        [Test]
        public void TestHashNew()
        {
            var constructor = typeof(List<int>).GetConstructor(Type.EmptyTypes);
            Assert.NotNull(constructor);
            var otherConstructor = typeof(List<int>).GetConstructor(new[] {typeof(int)});
            Assert.NotNull(otherConstructor);

            TestHashNotEquivalent(Expression.New(constructor), Expression.New(otherConstructor, Expression.Constant(1)));
        }

        [Test]
        public void TestHashTypeIs()
        {
            TestHashNotEquivalent(Expression.TypeIs(Expression.Parameter(typeof(int)), typeof(int)),
                                  Expression.TypeIs(Expression.Parameter(typeof(int)), typeof(decimal)));

            TestHashNotEquivalent(Expression.TypeIs(Expression.Parameter(typeof(int)), typeof(int)),
                                  Expression.TypeIs(Expression.Constant(1), typeof(int)));
        }

        [Test]
        public void TestHashTypeEqual()
        {
            TestHashNotEquivalent(Expression.TypeEqual(Expression.Parameter(typeof(int)), typeof(int)),
                                  Expression.TypeEqual(Expression.Parameter(typeof(long)), typeof(long)));

            TestHashNotEquivalent(Expression.TypeEqual(Expression.Parameter(typeof(long)), typeof(int)),
                                  Expression.TypeEqual(Expression.Parameter(typeof(long)), typeof(long)));
        }

        [Test]
        public void TestHashLabel()
        {
            TestHashNotEquivalent(Expression.Label(Expression.Label(typeof(int)), Expression.Constant(1)),
                                  Expression.Label(Expression.Label(typeof(int)), Expression.Constant(2)));

            TestHashNotEquivalent(Expression.Label(Expression.Label(typeof(long)), Expression.Constant(1L)),
                                  Expression.Label(Expression.Label(typeof(int)), Expression.Constant(1)));
        }

        [Test]
        public void TestHashLambda()
        {
            TestHashEquivalent((Expression<Func<int, int, int>>)((x, y) => x + y),
                               (Expression<Func<int, int, int>>)((a, b) => a + b), strictly : false);

            TestHashNotEquivalent((Expression<Func<int, int, int>>)((x, y) => x + y),
                                  (Expression<Func<int, int, int>>)((a, b) => a + b), strictly : true);

            TestHashNotEquivalent((Expression<Func<int, int, int>>)((x, y) => x + y),
                                  (Expression<Func<int, int, int>>)((x, y) => y + x));

            TestHashNotEquivalent((Expression<Func<int, int, int>>)((x, y) => x + y),
                                  (Expression<Func<int, int>>)(x => x + x));
        }

        [Test]
        public void TestHashMemberAccess()
        {
            var arrayLength = typeof(int[]).GetProperty(nameof(Array.Length));
            Assert.NotNull(arrayLength);
            var strLength = typeof(string).GetProperty(nameof(string.Length));
            Assert.NotNull(strLength);

            TestHashNotEquivalent(Expression.MakeMemberAccess(Expression.Constant(new int[0]), arrayLength),
                                  Expression.MakeMemberAccess(Expression.Constant("str"), strLength));
        }

        [Test]
        public void TestHashNewArrayInit()
        {
            TestHashNotEquivalent(Expression.NewArrayInit(typeof(int), Expression.Constant(1), Expression.Constant(2)),
                                  Expression.NewArrayInit(typeof(int), Expression.Constant(2), Expression.Constant(1)));

            TestHashNotEquivalent(Expression.NewArrayInit(typeof(long), Expression.Constant(1L), Expression.Constant(2L)),
                                  Expression.NewArrayInit(typeof(int), Expression.Constant(1), Expression.Constant(2)));
        }

        [Test]
        public void TestHashNewArrayBounds()
        {
            TestHashNotEquivalent(Expression.NewArrayBounds(typeof(int), Expression.Constant(1), Expression.Constant(2)),
                                  Expression.NewArrayBounds(typeof(int), Expression.Constant(2), Expression.Constant(1)));

            TestHashNotEquivalent(Expression.NewArrayBounds(typeof(long), Expression.Constant(1L), Expression.Constant(2L)),
                                  Expression.NewArrayBounds(typeof(int), Expression.Constant(1), Expression.Constant(2)));
        }

        [Test]
        public void TestHashBlock()
        {
            var firstVariable = Expression.Variable(typeof(int));
            var secondVariable = Expression.Variable(typeof(object));

            TestHashEquivalent(Expression.Block(typeof(void), new[] {firstVariable}, Expression.Constant(1)),
                               Expression.Block(typeof(void), new[] {secondVariable}, Expression.Constant(1)), strictly : false);

            TestHashNotEquivalent(Expression.Block(typeof(void), new[] {firstVariable}, Expression.Constant(1)),
                                  Expression.Block(typeof(void), new[] {secondVariable}, Expression.Constant(1)), strictly : true);

            TestHashNotEquivalent(Expression.Block(typeof(void), Expression.Constant(1), Expression.Constant(2)),
                                  Expression.Block(typeof(void), Expression.Constant(2), Expression.Constant(1)));

            TestHashNotEquivalent(Expression.Block(typeof(void), Expression.Constant(1L)),
                                  Expression.Block(typeof(void), Expression.Constant(1)));

            TestHashNotEquivalent(Expression.Block(typeof(void), Expression.Constant(1), Expression.Constant(2)),
                                  Expression.Block(typeof(void), Expression.Constant(1)));
        }

        [Test]
        public void TestHashDefault()
        {
            TestHashNotEquivalent(Expression.Default(typeof(int)), Expression.Default(typeof(long)));
            TestHashNotEquivalent(Expression.Default(typeof(object)), Expression.Default(typeof(string)));
        }

        [Test]
        public void TestHashArrayAccess()
        {
            TestHashNotEquivalent(Expression.ArrayAccess(Expression.Parameter(typeof(int[,])), Expression.Constant(1), Expression.Constant(1)),
                                  Expression.ArrayAccess(Expression.Parameter(typeof(long[,])), Expression.Constant(1), Expression.Constant(1)));

            TestHashNotEquivalent(Expression.ArrayAccess(Expression.Parameter(typeof(int[,])), Expression.Constant(1), Expression.Constant(1)),
                                  Expression.ArrayAccess(Expression.Parameter(typeof(int[])), Expression.Constant(1)));
        }

        [Test]
        public void TestHashGoto()
        {
            var gotoVoidCreators = new Func<LabelTarget, GotoExpression>[]
                {
                    Expression.Goto,
                    Expression.Return,
                    Expression.Break,
                    Expression.Continue
                };

            foreach (var pair in AllPairs(gotoVoidCreators))
                TestHashNotEquivalent(pair.Item1(Expression.Label(typeof(void))),
                                      pair.Item2(Expression.Label(typeof(void))));

            var gotoWithValueCreators = new Func<LabelTarget, Expression, GotoExpression>[]
                {
                    Expression.Goto,
                    Expression.Return,
                    Expression.Break,
                };

            foreach (var pair in AllPairs(gotoWithValueCreators))
                TestHashNotEquivalent(pair.Item1(Expression.Label(typeof(int)), Expression.Parameter(typeof(int))),
                                      pair.Item2(Expression.Label(typeof(int)), Expression.Parameter(typeof(int))));

            foreach (var gotoWithValueCreator in gotoWithValueCreators)
            {
                TestHashNotEquivalent(gotoWithValueCreator(Expression.Label(typeof(bool)), Expression.Constant(true)),
                                      gotoWithValueCreator(Expression.Label(typeof(bool)), Expression.Constant(false)));

                TestHashNotEquivalent(gotoWithValueCreator(Expression.Label(typeof(long)), Expression.Constant(1L)),
                                      gotoWithValueCreator(Expression.Label(typeof(int)), Expression.Constant(1)));
            }
        }

        [Test]
        public void TestHashLoop()
        {
            TestHashNotEquivalent(Expression.Loop(Expression.Constant(1)),
                                  Expression.Loop(Expression.Constant(2)));

            TestHashNotEquivalent(Expression.Loop(Expression.Constant(1)),
                                  Expression.Loop(Expression.Constant(2), Expression.Label()));

            TestHashNotEquivalent(Expression.Loop(Expression.Constant(1), Expression.Label()),
                                  Expression.Loop(Expression.Constant(2), Expression.Label(), Expression.Label()));

            TestHashNotEquivalent(Expression.Loop(Expression.Constant(1), Expression.Label(typeof(bool))),
                                  Expression.Loop(Expression.Constant(2), Expression.Label(typeof(string))));
        }

        [Test]
        public void TestHashSwitch()
        {
            var one = Expression.Constant(1);
            var two = Expression.Constant(2);

            TestHashNotEquivalent(Expression.Switch(one, one, Expression.SwitchCase(one, one)),
                                  Expression.Switch(two, one, Expression.SwitchCase(one, one)));

            TestHashNotEquivalent(Expression.Switch(one, one, Expression.SwitchCase(one, one)),
                                  Expression.Switch(one, two, Expression.SwitchCase(one, one)));

            TestHashNotEquivalent(Expression.Switch(one, one, Expression.SwitchCase(one, one)),
                                  Expression.Switch(one, one, Expression.SwitchCase(two, one)));

            TestHashNotEquivalent(Expression.Switch(one, one, Expression.SwitchCase(one, one)),
                                  Expression.Switch(one, one, Expression.SwitchCase(one, two)));

            TestHashNotEquivalent(Expression.Switch(one, one, Expression.SwitchCase(one, one)),
                                  Expression.Switch(one, one, Expression.SwitchCase(one, one, one)));

            TestHashNotEquivalent(Expression.Switch(one, one, Expression.SwitchCase(one, one)),
                                  Expression.Switch(one, one, Expression.SwitchCase(one, one), Expression.SwitchCase(one, one)));
        }

        [Test]
        public void TestHashTry()
        {
            var one = Expression.Constant(1);
            var two = Expression.Constant(2);
            var filter = Expression.Constant(true);
            var otherFilter = Expression.Constant(false);
            var catchExpression = Expression.Catch(Expression.Parameter(typeof(int)), one, filter);

            TestHashNotEquivalent(Expression.MakeTry(typeof(void), one, one, null, null),
                                  Expression.MakeTry(typeof(int), one, one, null, null));

            TestHashNotEquivalent(Expression.MakeTry(null, one, one, null, null),
                                  Expression.MakeTry(null, two, one, null, null));

            TestHashNotEquivalent(Expression.MakeTry(null, one, one, null, null),
                                  Expression.MakeTry(null, one, two, null, null));

            TestHashNotEquivalent(Expression.MakeTry(null, one, null, one, null),
                                  Expression.MakeTry(null, one, null, two, null));

            TestHashNotEquivalent(Expression.MakeTry(null, one, null, null, new[] {Expression.Catch(Expression.Parameter(typeof(int)), one)}),
                                  Expression.MakeTry(null, one, null, null, new[] {Expression.Catch(Expression.Parameter(typeof(int)), two)}));

            TestHashNotEquivalent(Expression.MakeTry(null, one, null, null, new[] {Expression.Catch(Expression.Parameter(typeof(int)), one, filter)}),
                                  Expression.MakeTry(null, one, null, null, new[] {Expression.Catch(Expression.Parameter(typeof(int)), two, otherFilter)}));

            TestHashNotEquivalent(Expression.MakeTry(null, one, null, null, new[] {Expression.Catch(Expression.Parameter(typeof(int)), one, filter)}),
                                  Expression.MakeTry(null, one, null, null, new[] {Expression.Catch(Expression.Parameter(typeof(int)), two, otherFilter)}));

            TestHashNotEquivalent(Expression.MakeTry(null, one, null, null, new[] {catchExpression, catchExpression}),
                                  Expression.MakeTry(null, one, null, null, new[] {catchExpression}));
        }

        [Test]
        public void TestHashMemberInit()
        {
            // ReSharper disable ObjectCreationAsStatement
            TestMemberInitNotEquivalent(() => new TestObject {field = "1"},
                                        () => new TestObject {});

            TestMemberInitNotEquivalent(() => new TestObject {field = "1"},
                                        () => new TestObject {otherField = "1"});

            TestMemberInitNotEquivalent(() => new TestObject {field = "1"},
                                        () => new TestObject {field = "2"});

            TestMemberInitNotEquivalent(() => new TestObject {field = "1"},
                                        () => new OtherTestObject {field = "1"});

            TestMemberInitNotEquivalent(() => new TestObject {complexObjectField = new OtherTestObject {field = "1"}},
                                        () => new TestObject {complexObjectField = new OtherTestObject {field = "2"}});

            TestMemberInitNotEquivalent(() => new TestObject {listField = {1, 2}},
                                        () => new TestObject {listField = {2, 1}});

            TestMemberInitNotEquivalent(() => new TestObject {complexObjectField = {field = "1"}},
                                        () => new TestObject {complexObjectField = {field = "2"}});
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void TestHashNotSupportedExpression()
        {
            TestNotSupported(Expression.RuntimeVariables(Expression.Parameter(typeof(int))));
            // TestNotSupported(Expression.DebugInfo());
            // TestNotSupported(Expression.Dynamic());
        }

        private void TestMemberInitNotEquivalent(Expression<Action> firstMemberInit, Expression<Action> secondMemberInit)
        {
            Assert.That(firstMemberInit.Body, Is.TypeOf<MemberInitExpression>());
            Assert.That(secondMemberInit.Body, Is.TypeOf<MemberInitExpression>());

            TestHashNotEquivalent(firstMemberInit.Body, secondMemberInit.Body);
        }

        private void TestHashUnaryExpression(Type parameterType, params Func<Expression, UnaryExpression>[] unaryOperations)
        {
            foreach (var operation in unaryOperations)
            {
                var parameter = Expression.Parameter(parameterType, "parameter");
                var otherParameter = Expression.Parameter(parameterType, "other_parameter");

                TestHashEquivalent(operation(parameter), operation(parameter));
                TestHashEquivalent(operation(parameter), operation(otherParameter), strictly : false);
                TestHashNotEquivalent(operation(parameter), operation(otherParameter), strictly : true);
            }

            foreach (var pair in AllPairs(unaryOperations))
            {
                var parameter = Expression.Parameter(parameterType);
                TestHashNotEquivalent(pair.Item1(parameter), pair.Item2(parameter));
            }
        }

        private static void TestHashBinaryExpression(Type parameterType, params Func<Expression, Expression, BinaryExpression>[] binaryOperations)
        {
            foreach (var operation in binaryOperations)
            {
                var a = Expression.Parameter(parameterType, "a");
                var b = Expression.Parameter(parameterType, "b");
                var c = Expression.Parameter(parameterType, "c");

                TestHashEquivalent(operation(Expression.Parameter(parameterType), Expression.Parameter(parameterType)),
                                   operation(Expression.Parameter(parameterType), Expression.Parameter(parameterType)));

                TestHashEquivalent(operation(a, b), operation(a, c), strictly : false);
                TestHashEquivalent(operation(a, b), operation(c, b), strictly : false);

                TestHashNotEquivalent(operation(a, b), operation(a, c), strictly : true);
                TestHashNotEquivalent(operation(a, b), operation(c, b), strictly : true);
            }

            foreach (var pair in AllPairs(binaryOperations))
            {
                var parameter = Expression.Parameter(parameterType);
                TestHashNotEquivalent(pair.Item1(parameter, parameter), pair.Item2(parameter, parameter));
            }
        }

        private static void TestHashEquivalent(Expression first, Expression second, bool strictly)
        {
            Assert.AreEqual(CalcHashCode(first, strictly), CalcHashCode(second, strictly));
        }

        private static void TestHashEquivalent(Expression first, Expression second)
        {
            TestHashEquivalent(first, second, strictly : true);
            TestHashEquivalent(first, second, strictly : false);
        }

        private static void TestHashNotEquivalent(Expression first, Expression second, bool strictly)
        {
            Assert.AreNotEqual(CalcHashCode(first, strictly), CalcHashCode(second, strictly));
        }

        private static void TestHashNotEquivalent(Expression first, Expression second)
        {
            TestHashNotEquivalent(first, second, strictly : true);
            TestHashNotEquivalent(first, second, strictly : false);
        }

        private static void TestNotSupported(Expression expr)
        {
            Assert.Throws<NotSupportedException>(() => CalcHashCode(expr, strictly : true));
            Assert.Throws<NotSupportedException>(() => CalcHashCode(expr, strictly : false));
        }

        private static IEnumerable<Tuple<TValue, TValue>> AllPairs<TValue>(IEnumerable<TValue> values)
        {
            var array = values.ToArray();
            for (var i = 0; i < array.Length; ++i)
            {
                for (var j = 0; j < i; ++j)
                    yield return Tuple.Create(array[i], array[j]);
            }
        }

        private class TestObject
        {
            public static int Add(int a, int b)
            {
                return a + b;
            }

            public static int OtherAdd(int a, int b)
            {
                return a + b;
            }

            public int this[int i] { get => 1; set {} }
            public int this[long j] { get => 1; set {} }

            public string field;
            public string otherField;
            public OtherTestObject complexObjectField;
            public List<int> listField;
        }

        private class OtherTestObject
        {
            public string field;
        }
    }
}
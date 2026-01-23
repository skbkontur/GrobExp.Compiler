#if NET7_0 || NET45
// ReSharper disable once CheckNamespace
namespace NUnit.Framework.Legacy
{
    public static class ClassicAssert
    {
        public static void NotNull(object anObject)
        {
            Assert.NotNull(anObject);
        }

        public static void IsNull(object anObject)
        {
            Assert.IsNull(anObject);
        }

        public static void IsNotNull(object anObject)
        {
            Assert.IsNotNull(anObject);
        }

        public static void AreEqual(object expected, object actual)
        {
            Assert.AreEqual(expected, actual);
        }

        public static void AreNotEqual(object expected, object actual)
        {
            Assert.AreNotEqual(expected, actual);
        }

        public static void IsTrue(bool condition)
        {
            Assert.IsTrue(condition);
        }

        public static void IsFalse(bool condition)
        {
            Assert.IsFalse(condition);
        }
    }
}

#endif
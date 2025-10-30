using NUnit.Framework;

namespace TestWorkFlow
{
    public class Test
    {
        [Test]
        public void Addition_Works()
        {
            Assert.AreEqual(4, 2 + 2);
        }

        [Test]
        public void Subtraction_Works()
        {
            Assert.AreEqual(0, 2 - 2);
        }
    }
}

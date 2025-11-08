using NUnit.Framework;

namespace TestWorkFlow
{
    public class Test
    {
        [Test]
        public void Addition()
        {
            Assert.AreEqual(4, 2 + 2);
        }

        [Test]
        public void Subtraction()
        {
            Assert.AreEqual(0, 2 - 2);
        }
    }
}

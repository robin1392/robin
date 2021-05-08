using NUnit.Framework;
using Photon.Deterministic;
using Assert = NUnit.Framework.Assert;

namespace quantum.code.test
{
    public class RNGSessionTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void RNGTest()
        {
            var rng = new RNGSession();
            
            bool minAppeared = false;
            bool maxAppeared = false;
            var min = 10;
            var max = 12;
            for (var i = 0; i < 100; ++i)
            {
                
                var result = rng.Next(min, max);
                if (result == min)
                {
                    minAppeared = true;
                }
                
                if (result == max)
                {
                    maxAppeared = true;
                }
            }
            
            Assert.IsTrue(minAppeared);
            Assert.IsFalse(maxAppeared);
        }
    }
}
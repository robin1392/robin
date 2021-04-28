using System.IO;
using NUnit.Framework;
using Quantum;

namespace quantum.code.test
{
    [TestFixture]
    public class QuantumFixture
    {
        protected TestRunner _runner;
        
        public QuantumFixture()
        {
            _runner = new TestRunner("../../../../../quantum_unity/Assets/Photon/Quantum/Resources/LUT", "../../../../../quantum_code/quantum.console.runner/DBFromUnity/assetDB.json");
        }
    }
}
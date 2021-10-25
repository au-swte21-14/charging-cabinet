using Ladeskab.Interfaces;
using NUnit.Framework;

namespace Ladeskab.Unit.Test
{
    [TestFixture]
    public class RfidReaderTests
    {
        private IRfidReader _uut;
        private int _lastId;

        [SetUp]
        public void Setup()
        {
            _lastId = -1;
            _uut = new RfidReader();
            _uut.OnRfidDetectedEvent += (_, args) => OnRfidDetected(args.Id);
        }

        private void OnRfidDetected(int id)
        {
            _lastId = id;
        }

        [Test]
        public void RfidReaderInitialized()
        {
            Assert.AreEqual(-1, _lastId);
        }

        [Test]
        public void OnRfidRead()
        {
            _uut.OnRfidRead(1);
            Assert.AreEqual(1, _lastId);
        }

        [Test]
        public void OnRfidReadMultiple()
        {
            _uut.OnRfidRead(1);
            Assert.AreEqual(1, _lastId);
            _uut.OnRfidRead(2);
            Assert.AreEqual(2, _lastId);
        }
    }
}
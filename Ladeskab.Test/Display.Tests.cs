using NUnit.Framework;
using Ladeskab.Interfaces;


namespace Ladeskab.Unit.Test
{
    [TestFixture]
    public class Display_Tests
    {
        private IDisplay _uut;

        [SetUp]
        public void Setup()
        {
            _uut = new Display();
        }

        [Test]
        public void DisplayInitialized()
        {
            Assert.AreEqual("",_uut.ChargingMessage);
            Assert.AreEqual("",_uut.StationMessage);
        }

        [Test]
        public void SetChargingMessage()
        {
            _uut.ChargingMessage = "Test";
            Assert.AreEqual("Test",_uut.ChargingMessage);
        }

        [Test]
        public void SetStationMessage()
        {
            _uut.StationMessage = "Test";
            Assert.AreEqual("Test",_uut.StationMessage);

        }
    }
}
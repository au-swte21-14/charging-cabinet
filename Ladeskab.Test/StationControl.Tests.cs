using System.Linq;
using Ladeskab.Interfaces;
using NSubstitute;
using NSubstitute.Extensions;
using NUnit.Framework;
using UsbSimulator;

namespace Ladeskab.Unit.Test
{
    [TestFixture]
    public class StationControlTests
    {
        private IDoor _door;
        private IRfidReader _rfidReader;
        private IUsbCharger _usbCharger;
        private IChargeControl _charger;
        private IDisplay _display;
        private ILogger _logger;

        [SetUp]
        public void Setup()
        {
            _usbCharger = Substitute.For<IUsbCharger>();
            _display = new Display();
            _logger = Substitute.For<ILogger>();
            _charger = new ChargeControl(_usbCharger, _display);
            _door = new Door();
            _rfidReader = new RfidReader();
            var unused = new StationControl(_door, _rfidReader, _charger, _display, _logger);

            // When charging starts, fake 500 mA and send a event
            _usbCharger.When(x => x.StartCharge()).Do(_ =>
            {
                _usbCharger.Configure().CurrentValue.Returns(500);
                _usbCharger.CurrentValueEvent += Raise.EventWith(_usbCharger,
                    new CurrentEventArgs {Current = _usbCharger.CurrentValue});
            });
            // When charging stops, fake 0 mA and send a event
            _usbCharger.When(x => x.StopCharge()).Do(_ =>
            {
                _usbCharger.Configure().CurrentValue.Returns(0);
                _usbCharger.CurrentValueEvent += Raise.EventWith(_usbCharger,
                    new CurrentEventArgs {Current = _usbCharger.CurrentValue});
            });
        }
        
        [Test]
        public void StationAvailableChargerNotConnected()
        {
            _door.OnDoorOpen();
            _door.OnDoorClose();
            _rfidReader.OnRfidRead(1);
            Assert.AreEqual("Din telefon er ikke ordentlig tilsluttet. Prøv igen.", _display.StationMessage);
            Assert.AreEqual("", _display.ChargingMessage);
        }
        
        [Test]
        public void StationAvailableChargerConnected()
        {
            _usbCharger.Configure().Connected.Returns(true);
            _rfidReader.OnRfidRead(1);
            Assert.AreEqual("Skabet er låst og din telefon lades. Brug dit RFID tag til at låse op.", _display.StationMessage);
            Assert.AreEqual("Lader...", _display.ChargingMessage);
            StringAssert.Contains("Skab låst med RFID", _logger.ReceivedCalls().Last().GetArguments().First().ToString());
        }
        
        [Test]
        public void StationLockedUnlock()
        {
            _usbCharger.Configure().Connected.Returns(true);
            _rfidReader.OnRfidRead(1);
            _rfidReader.OnRfidRead(1);
            Assert.AreEqual("Tag din telefon ud af skabet og luk døren.", _display.StationMessage);
            Assert.AreEqual("", _display.ChargingMessage);
            StringAssert.Contains("Skab låst op med RFID", _logger.ReceivedCalls().Last().GetArguments().First().ToString());
        }
        
        [Test]
        public void StationLockedInvalidRfid()
        {
            _usbCharger.Configure().Connected.Returns(true);
            _rfidReader.OnRfidRead(1);
            _rfidReader.OnRfidRead(2);
            Assert.AreEqual("Forkert RFID tag", _display.StationMessage);
            Assert.AreEqual("Lader...", _display.ChargingMessage);
        }
        
        [Test]
        public void StationOpen()
        {
            _door.OnDoorOpen();
            _rfidReader.OnRfidRead(1);
            Assert.AreEqual("", _display.StationMessage);
            Assert.AreEqual("", _display.ChargingMessage);
        }
    }
}
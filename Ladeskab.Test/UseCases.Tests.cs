using System.Linq;
using Ladeskab.Interfaces;
using NSubstitute;
using NSubstitute.Extensions;
using NUnit.Framework;
using UsbSimulator;

namespace Ladeskab.Unit.Test
{
    [TestFixture]
    public class UseCasesTests
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
            _charger = new ChargeControl(_usbCharger, _display);
            _door = new Door();
            _rfidReader = new RfidReader();
            _logger = Substitute.For<ILogger>();
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
        public void UseCase1()
        {
            _door.OnDoorOpen(); // Brugeren åbner lågen på ladeskabet
            _usbCharger.Configure().Connected.Returns(true); // Brugeren tilkobler sin mobiltelefon til ladekablet.
            _door.OnDoorClose(); // Brugeren lukker lågen på ladeskabet.
            _rfidReader.OnRfidRead(1); // Brugeren holder sit RFID tag op til systemets RFID-læser.
            StringAssert.Contains("Skab låst med RFID", _logger.ReceivedCalls().Last().GetArguments().First().ToString());

            // Systemet aflæser RFID-tagget. Hvis ladekablet er forbundet, låser systemet lågen på ladeskabet,
            // og låsningen logges. Skabet er nu optaget. Opladning påbegyndes.
            Assert.IsFalse(_door.DoorOpen);
            Assert.IsTrue(_door.DoorLocked);
            Assert.AreEqual(ChargeStateChangedEventArgs.ChargeState.Charging, _charger.State);
            _usbCharger.Received().StartCharge();

            // Brugeren kommer tilbage til ladeskabet.
            _rfidReader.OnRfidRead(1); // Brugeren holder sit RFID tag op til systemets RFID-læser.
            StringAssert.Contains("Skab låst op med RFID", _logger.ReceivedCalls().Last().GetArguments().First().ToString());

            // Systemet aflæser RFID-tagget. Hvis RFID er identisk med det, der blev brugt til at låse skabet
            // med, stoppes opladning, ladeskabets låge låses op og oplåsningen logges.
            Assert.AreEqual(ChargeStateChangedEventArgs.ChargeState.NotCharging, _charger.State);
            _usbCharger.Received().StopCharge();
            Assert.IsFalse(_door.DoorLocked);

            _door.OnDoorOpen(); // Brugeren åbner ladeskabet
            _usbCharger.Configure().Connected
                .Returns(false); // fjerner ladekablet fra sin telefon og tager telefonen ud af ladeskabet.
            _door.OnDoorClose(); // Brugeren lukker skabet. Skabet er nu ledigt.
        }
    }
}
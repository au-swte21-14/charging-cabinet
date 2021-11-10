using System.Linq;
using Ladeskab.Interfaces;
using NSubstitute;
using NSubstitute.Extensions;
using NUnit.Framework;

namespace Ladeskab.Unit.Test
{
    [TestFixture]
    public class StationControlTests
    {
        private IDoor _door;
        private IRfidReader _rfidReader;
        private IChargeControl _charger;
        private IDisplay _display;
        private ILogger _logger;

        [SetUp]
        public void Setup()
        {
            _display = Substitute.For<IDisplay>();
            _logger = Substitute.For<ILogger>();
            _charger = Substitute.For<IChargeControl>();
            _door = Substitute.For<IDoor>();
            _rfidReader = Substitute.For<IRfidReader>();
            var unused = new StationControl(_door, _rfidReader, _charger, _display, _logger);
        }

        [Test]
        public void StationOpen()
        {
            _door.OnDoorChangedEvent += Raise.EventWith(_door,
                new DoorChangedEventArgs() {ChangeType = DoorChangedEventArgs.ChangeTypeEnum.DoorOpened});
            Assert.AreEqual("Tilslut telefon", _display.StationMessage);
        }

        [Test]
        public void StationClosed()
        {
            _door.OnDoorChangedEvent += Raise.EventWith(_door,
                new DoorChangedEventArgs() {ChangeType = DoorChangedEventArgs.ChangeTypeEnum.DoorOpened});
            _door.OnDoorChangedEvent += Raise.EventWith(_door,
                new DoorChangedEventArgs() {ChangeType = DoorChangedEventArgs.ChangeTypeEnum.DoorClosed});
            Assert.AreEqual("Indlæs RFID", _display.StationMessage);
        }

        [Test]
        public void StationAvailableChargerNotConnected()
        {
            _door.OnDoorChangedEvent += Raise.EventWith(_door,
                new DoorChangedEventArgs() {ChangeType = DoorChangedEventArgs.ChangeTypeEnum.DoorOpened});
            _door.OnDoorChangedEvent += Raise.EventWith(_door,
                new DoorChangedEventArgs() {ChangeType = DoorChangedEventArgs.ChangeTypeEnum.DoorClosed});
            Assert.AreEqual("Indlæs RFID", _display.StationMessage);
            _rfidReader.OnRfidDetectedEvent += Raise.EventWith(_door, new RfidDetectedEventArgs {Id = 1});
            Assert.AreEqual("Tilslutningsfejl", _display.StationMessage);
        }

        [Test]
        public void StationAvailableChargerConnected()
        {
            _charger.Configure().Connected.Returns(true);
            _rfidReader.OnRfidDetectedEvent += Raise.EventWith(_door, new RfidDetectedEventArgs {Id = 1});
            // This event should be ignored
            _door.OnDoorChangedEvent += Raise.EventWith(_door,
                new DoorChangedEventArgs() {ChangeType = DoorChangedEventArgs.ChangeTypeEnum.DoorOpened});
            Assert.AreEqual("Ladeskab optaget", _display.StationMessage);
            StringAssert.Contains("Skab låst med RFID",
                _logger.ReceivedCalls().Last().GetArguments().First().ToString());
            _door.Received().LockDoor();
            _charger.Received().StartCharge();
        }

        [Test]
        public void StationLockedUnlock()
        {
            _charger.Configure().Connected.Returns(true);
            _rfidReader.OnRfidDetectedEvent += Raise.EventWith(_door, new RfidDetectedEventArgs {Id = 1});
            _rfidReader.OnRfidDetectedEvent += Raise.EventWith(_door, new RfidDetectedEventArgs {Id = 1});
            Assert.AreEqual("Fjern telefon", _display.StationMessage);
            StringAssert.Contains("Skab låst op med RFID",
                _logger.ReceivedCalls().Last().GetArguments().First().ToString());
            _door.Received().UnlockDoor();
            _charger.Received().StopCharge();
        }

        [Test]
        public void StationLockedInvalidRfid()
        {
            _charger.Configure().Connected.Returns(true);
            _rfidReader.OnRfidDetectedEvent += Raise.EventWith(_door, new RfidDetectedEventArgs {Id = 1});
            _rfidReader.OnRfidDetectedEvent += Raise.EventWith(_door, new RfidDetectedEventArgs {Id = 2});
            Assert.AreEqual("Forkert RFID tag", _display.StationMessage);
            _door.DidNotReceive().UnlockDoor();
            _charger.DidNotReceive().StopCharge();
        }

        [Test]
        public void StationOpenTryRfid()
        {
            _door.OnDoorChangedEvent += Raise.EventWith(_door,
                new DoorChangedEventArgs() {ChangeType = DoorChangedEventArgs.ChangeTypeEnum.DoorOpened});
            _rfidReader.OnRfidDetectedEvent += Raise.EventWith(_door, new RfidDetectedEventArgs {Id = 1});
            _door.DidNotReceive().LockDoor();
            _charger.DidNotReceive().StartCharge();
            _door.DidNotReceive().UnlockDoor();
            _charger.DidNotReceive().StopCharge();
        }
    }
}
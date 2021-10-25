using Ladeskab.Interfaces;
using NSubstitute;
using NSubstitute.Extensions;
using NUnit.Framework;
using UsbSimulator;

namespace Ladeskab.Unit.Test
{
    [TestFixture]
    public class ChargeControlTests
    {
        private IChargeControl _uut;
        private IUsbCharger _usbCharger;
        private IDisplay _display;
        private ChargeStateChangedEventArgs.ChargeState _lastState;

        [SetUp]
        public void Setup()
        {
            _lastState = ChargeStateChangedEventArgs.ChargeState.Unknown;
            _usbCharger = Substitute.For<IUsbCharger>();
            _display = new Display();
            _uut = new ChargeControl(_usbCharger, _display);

            _uut.OnChargeStateChangedEvent += (_, args) => OnChargeStateChanged(args.State);

            // When charging starts, fake 498 + 500 mA and send a event
            _usbCharger.When(x => x.StartCharge()).Do(_ =>
            {
                _usbCharger.Configure().CurrentValue.Returns(500);
                _usbCharger.CurrentValueEvent += Raise.EventWith(_usbCharger,
                    new CurrentEventArgs {Current = _usbCharger.CurrentValue});
                _usbCharger.Configure().CurrentValue.Returns(498);
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

        private void OnChargeStateChanged(ChargeStateChangedEventArgs.ChargeState state)
        {
            _lastState = state;
        }


        [Test]
        public void ChargeControlInitialized()
        {
            Assert.IsFalse(_uut.Connected);
            Assert.AreEqual(ChargeStateChangedEventArgs.ChargeState.Unknown, _uut.State);
            Assert.AreEqual("", _display.ChargingMessage);
        }

        [Test]
        public void StartCharge()
        {
            _usbCharger.Configure().Connected.Returns(true); // Fake connected device
            Assert.IsTrue(_uut.Connected); // Validate connection
            _uut.StartCharge(); // Start charging
            Assert.AreEqual(ChargeStateChangedEventArgs.ChargeState.Charging, _lastState); // Validate charging state
            Assert.AreEqual("Lader...", _display.ChargingMessage); // Validate display output
        }

        [Test]
        public void StopCharge()
        {
            _usbCharger.Configure().Connected.Returns(true); // Fake connected device
            _uut.StartCharge(); // Start charging
            _uut.StopCharge(); // Stop charging
            Assert.AreEqual(ChargeStateChangedEventArgs.ChargeState.NotCharging, _lastState); // Validate charging state
            Assert.AreEqual("", _display.ChargingMessage); // Validate display output
        }

        [Test]
        public void OnChargeComplete()
        {
            _usbCharger.Configure().Connected.Returns(true); // Fake connected device
            _uut.StartCharge(); // Start charging
            _usbCharger.Configure().CurrentValue.Returns(5); // Fake 5 mA
            _usbCharger.CurrentValueEvent += Raise.EventWith(_usbCharger,
                new CurrentEventArgs {Current = _usbCharger.CurrentValue}); // Send charge current event
            Assert.AreEqual(ChargeStateChangedEventArgs.ChargeState.ChargeCompleted,
                _lastState); // Validate charge complete state
            Assert.AreEqual("Fuldt opladt", _display.ChargingMessage); // Validate display output
        }

        [Test]
        public void OnNotCharging()
        {
            _usbCharger.Configure().Connected.Returns(true); // Fake connected device
            _uut.StartCharge(); // Start charging
            _usbCharger.Configure().CurrentValue.Returns(0); // Fake 0 mA
            _usbCharger.CurrentValueEvent +=
                Raise.EventWith(_usbCharger,
                    new CurrentEventArgs {Current = _usbCharger.CurrentValue}); // Send charge current event
            Assert.AreEqual(ChargeStateChangedEventArgs.ChargeState.NotCharging,
                _lastState); // Validate not charging state
            Assert.AreEqual("", _display.ChargingMessage); // Validate display output
        }

        [Test]
        public void OnChargeFailure()
        {
            _usbCharger.Configure().Connected.Returns(true); // Fake connected device
            _uut.StartCharge(); // Start charging
            _usbCharger.Configure().CurrentValue.Returns(501); // Fake 501 mA
            _usbCharger.CurrentValueEvent +=
                Raise.EventWith(_usbCharger,
                    new CurrentEventArgs {Current = _usbCharger.CurrentValue}); // Send charge current event
            Assert.AreEqual(ChargeStateChangedEventArgs.ChargeState.ChargeFailure,
                _lastState); // Validate charge failure state
            Assert.AreEqual("Fejl opstået", _display.ChargingMessage); // Validate display output
        }
    }
}
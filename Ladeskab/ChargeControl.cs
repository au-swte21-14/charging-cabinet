using System;
using Ladeskab.Interfaces;
using UsbSimulator;

namespace Ladeskab
{
    public class ChargeControl : IChargeControl
    {
        private readonly IUsbCharger _usbCharger;
        private readonly IDisplay _display;
        private ChargeStateChangedEventArgs.ChargeState _state;

        public ChargeStateChangedEventArgs.ChargeState State
        {
            get => _state;
            private set
            {
                if (_state == value) return;
                _state = value;
                OnChargeStateChangedEvent?.Invoke(this, new ChargeStateChangedEventArgs {State = value});
                switch (value)
                {
                    case ChargeStateChangedEventArgs.ChargeState.Unknown:
                    case ChargeStateChangedEventArgs.ChargeState.NotCharging:
                        _display.ChargingMessage = "";
                        break;
                    case ChargeStateChangedEventArgs.ChargeState.ChargeCompleted:
                        _display.ChargingMessage = "Fuldt opladt";
                        break;
                    case ChargeStateChangedEventArgs.ChargeState.Charging:
                        _display.ChargingMessage = "Lader...";
                        break;
                    case ChargeStateChangedEventArgs.ChargeState.ChargeFailure:
                        _display.ChargingMessage = "Fejl opstået";
                        break;
                }
            }
        }

        public event EventHandler<ChargeStateChangedEventArgs> OnChargeStateChangedEvent;

        public ChargeControl(IUsbCharger usbCharger, IDisplay display)
        {
            _usbCharger = usbCharger;
            _display = display;
            _usbCharger.CurrentValueEvent += (_, args) => OnCurrentChanged(args.Current);
        }

        public bool Connected => _usbCharger.Connected;

        public void StartCharge()
        {
            _usbCharger.StartCharge();
        }

        public void StopCharge()
        {
            _usbCharger.StopCharge();
        }

        private void OnCurrentChanged(double current)
        {
            switch (current)
            {
                case 0:
                    State = ChargeStateChangedEventArgs.ChargeState.NotCharging;
                    break;
                case <= 5:
                    State = ChargeStateChangedEventArgs.ChargeState.ChargeCompleted;
                    break;
                case <= 500:
                    State = ChargeStateChangedEventArgs.ChargeState.Charging;
                    break;
                default:
                    State = ChargeStateChangedEventArgs.ChargeState.ChargeFailure;
                    break;
            }
        }
    }
}
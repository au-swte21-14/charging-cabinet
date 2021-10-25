using System;

namespace Ladeskab.Interfaces
{
    public class ChargeStateChangedEventArgs : EventArgs
    {
        public enum ChargeState
        {
            Unknown,
            NotCharging,
            ChargeCompleted,
            Charging,
            ChargeFailure
        }

        public ChargeState State { init; get; }
    }

    public interface IChargeControl
    {
        public bool Connected { get; }
        public ChargeStateChangedEventArgs.ChargeState State { get; }

        event EventHandler<ChargeStateChangedEventArgs> OnChargeStateChangedEvent;

        public void StartCharge();
        public void StopCharge();
    }
}
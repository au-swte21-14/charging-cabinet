using System;
using Ladeskab.Interfaces;

namespace Ladeskab
{
    public class RfidReader : IRfidReader
    {
        public event EventHandler<RfidDetectedEventArgs> OnRfidDetectedEvent;

        public void OnRfidRead(int id)
        {
            OnRfidDetectedEvent?.Invoke(this, new RfidDetectedEventArgs {Id = id});
        }
    }
}
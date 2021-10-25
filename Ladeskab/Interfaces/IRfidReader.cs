using System;

namespace Ladeskab.Interfaces
{
    public class RfidDetectedEventArgs : EventArgs
    {
        // Rfid ID
        public int Id { init; get; }
    }
    public interface IRfidReader
    {
        event EventHandler<RfidDetectedEventArgs> OnRfidDetectedEvent;
        public void OnRfidRead(int id);
    }
}
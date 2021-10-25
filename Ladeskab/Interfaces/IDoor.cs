using System;

namespace Ladeskab.Interfaces
{
    public class DoorChangedEventArgs : EventArgs
    {
        public enum ChangeTypeEnum
        {
            Unknown,
            DoorOpened,
            DoorClosed,
            DoorLocked,
            DoorUnlocked
        }

        public ChangeTypeEnum ChangeType { init; get; }
    }

    public interface IDoor
    {
        event EventHandler<DoorChangedEventArgs> OnDoorChangedEvent;
        public bool DoorOpen { get; }
        public bool DoorLocked { get; }
        public void LockDoor();
        public void UnlockDoor();
        public void OnDoorOpen();
        public void OnDoorClose();
    }
}
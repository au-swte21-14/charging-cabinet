using System;
using Ladeskab.Interfaces;

namespace Ladeskab
{
    public class Door : IDoor
    {
        private bool _doorOpen;
        private bool _doorLocked;

        public bool DoorOpen
        {
            get => _doorOpen;
            private set
            {
                if (_doorOpen == value) return;
                if (value && DoorLocked)
                {
                    return;
                }
                _doorOpen = value;
                OnDoorChangedEvent?.Invoke(this,
                    new DoorChangedEventArgs
                    {
                        ChangeType = _doorOpen
                            ? DoorChangedEventArgs.ChangeTypeEnum.DoorOpened
                            : DoorChangedEventArgs.ChangeTypeEnum.DoorClosed
                    });
            }
        }

        public bool DoorLocked
        {
            get => _doorLocked;
            private set
            {
                if (_doorLocked == value) return;
                if (value && DoorOpen) return;
                    
                _doorLocked = value;
                OnDoorChangedEvent?.Invoke(this,
                    new DoorChangedEventArgs
                    {
                        ChangeType = _doorLocked
                            ? DoorChangedEventArgs.ChangeTypeEnum.DoorLocked
                            : DoorChangedEventArgs.ChangeTypeEnum.DoorUnlocked
                    });
            }
        }

        public event EventHandler<DoorChangedEventArgs> OnDoorChangedEvent;

        public void LockDoor()
        {
            DoorLocked = true;
        }

        public void UnlockDoor()
        {
            DoorLocked = false;
        }

        public void OnDoorOpen()
        {
            DoorOpen = true;
        }

        public void OnDoorClose()
        {
            DoorOpen = false;
        }
    }
}
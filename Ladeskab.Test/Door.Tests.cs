using Ladeskab.Interfaces;
using NUnit.Framework;

namespace Ladeskab.Unit.Test
{
    [TestFixture]
    public class DoorTests
    {
        private IDoor _uut;
        private DoorChangedEventArgs.ChangeTypeEnum _lastChange;

        [SetUp]
        public void Setup()
        {
            _lastChange = DoorChangedEventArgs.ChangeTypeEnum.Unknown;
            _uut = new Door();
            _uut.OnDoorChangedEvent += (_, args) => OnDoorChanged(args.ChangeType);
        }

        private void OnDoorChanged(DoorChangedEventArgs.ChangeTypeEnum change)
        {
            _lastChange = change;
        }

        [Test]
        public void DoorInitialized()
        {
            Assert.AreEqual(DoorChangedEventArgs.ChangeTypeEnum.Unknown, _lastChange);
            Assert.IsFalse(_uut.DoorLocked);
            Assert.IsFalse(_uut.DoorOpen);
        }
        
        [Test]
        public void LockDoor()
        {
            _uut.LockDoor();
            Assert.AreEqual(DoorChangedEventArgs.ChangeTypeEnum.DoorLocked,_lastChange);
            Assert.IsTrue(_uut.DoorLocked);
            Assert.IsFalse(_uut.DoorOpen);
        }
        
        [Test]
        public void UnlockDoor()
        {
            _uut.LockDoor();
            _uut.UnlockDoor();
            Assert.AreEqual(DoorChangedEventArgs.ChangeTypeEnum.DoorUnlocked,_lastChange);
            Assert.IsFalse(_uut.DoorLocked);
            Assert.IsFalse(_uut.DoorOpen);
        }
        
        [Test]
        public void OnDoorOpen()
        {
            _uut.OnDoorOpen();
            Assert.AreEqual(DoorChangedEventArgs.ChangeTypeEnum.DoorOpened,_lastChange);
            Assert.IsTrue(_uut.DoorOpen);
            Assert.IsFalse(_uut.DoorLocked);
        }
        
        [Test]
        public void OnDoorClose()
        {
            _uut.OnDoorOpen();
            _uut.OnDoorClose();
            Assert.AreEqual(DoorChangedEventArgs.ChangeTypeEnum.DoorClosed,_lastChange);
            Assert.IsFalse(_uut.DoorOpen);
            Assert.IsFalse(_uut.DoorLocked);
        }
        
        [Test]
        public void OnDoorCloseTwice()
        {
            _uut.OnDoorClose();
            Assert.AreEqual(DoorChangedEventArgs.ChangeTypeEnum.Unknown,_lastChange);
            Assert.IsFalse(_uut.DoorOpen);
            Assert.IsFalse(_uut.DoorLocked);
        }
        
        [Test]
        public void OnDoorOpenTwice()
        {
            _uut.OnDoorOpen();
            _lastChange = DoorChangedEventArgs.ChangeTypeEnum.Unknown;
            _uut.OnDoorOpen();
            Assert.AreEqual(DoorChangedEventArgs.ChangeTypeEnum.Unknown,_lastChange);
            Assert.IsTrue(_uut.DoorOpen);
            Assert.IsFalse(_uut.DoorLocked);
        }
        
        [Test]
        public void OnDoorOpenIsLocked()
        {
            _uut.LockDoor();
            _uut.OnDoorOpen();
            Assert.AreEqual(DoorChangedEventArgs.ChangeTypeEnum.DoorLocked,_lastChange);
            Assert.IsFalse(_uut.DoorOpen);
            Assert.IsTrue(_uut.DoorLocked);
        }
        
        [Test]
        public void OnDoorLockIsOpen()
        {
            _uut.OnDoorOpen();
            _uut.LockDoor();
            Assert.AreEqual(DoorChangedEventArgs.ChangeTypeEnum.DoorOpened,_lastChange);
            Assert.IsTrue(_uut.DoorOpen);
            Assert.IsFalse(_uut.DoorLocked);
        }
        
        [Test]
        public void LockDoorTwice()
        {
            _uut.LockDoor();
            _lastChange = DoorChangedEventArgs.ChangeTypeEnum.Unknown;
            _uut.LockDoor();
            Assert.AreEqual(DoorChangedEventArgs.ChangeTypeEnum.Unknown,_lastChange);
            Assert.IsTrue(_uut.DoorLocked);
            Assert.IsFalse(_uut.DoorOpen);
        }
        
        [Test]
        public void UnlockDoorTwice()
        {
            _uut.LockDoor();
            _uut.UnlockDoor();
            _lastChange = DoorChangedEventArgs.ChangeTypeEnum.Unknown;
            _uut.UnlockDoor();
            Assert.AreEqual(DoorChangedEventArgs.ChangeTypeEnum.Unknown,_lastChange);
            Assert.IsFalse(_uut.DoorLocked);
            Assert.IsFalse(_uut.DoorOpen);
        }
    }
}
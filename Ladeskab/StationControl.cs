using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ladeskab.Interfaces;

namespace Ladeskab
{
    public class StationControl
    {
        // Enum med tilstande ("states") svarende til tilstandsdiagrammet for klassen
        private enum LadeskabState
        {
            Available,
            Locked,
            DoorOpen
        };

        // Her mangler flere member variable
        private LadeskabState _state;
        private IChargeControl _charger;
        private int _oldId;
        private readonly IDoor _door;
        private readonly IDisplay _display;
        private readonly ILogger _logger;

        // Her mangler constructor
        public StationControl(IDoor door, IRfidReader rfidReader, IChargeControl charger, IDisplay display,
            ILogger logger)
        {
            _charger = charger;
            _door = door;
            _display = display;
            _logger = logger;
            _door.OnDoorChangedEvent += (_, args) => OnDoorChanged(args.ChangeType);
            rfidReader.OnRfidDetectedEvent += (_, args) => RfidDetected(args.Id);
        }

        // Eksempel på event handler for eventet "RFID Detected" fra tilstandsdiagrammet for klassen
        private void RfidDetected(int id)
        {
            switch (_state)
            {
                case LadeskabState.Available:
                    // Check for ladeforbindelse
                    if (_charger.Connected)
                    {
                        _door.LockDoor();
                        _charger.StartCharge();
                        _oldId = id;
                        _logger.WriteLine(DateTime.Now + ": Skab låst med RFID: {0}", id);
                        _display.StationMessage = "Ladeskab optaget";
                        _state = LadeskabState.Locked;
                    }
                    else
                    {
                        _display.StationMessage = "Tilslutningsfejl";
                    }

                    break;

                case LadeskabState.DoorOpen:
                    // Ignore
                    break;

                case LadeskabState.Locked:
                    // Check for correct ID
                    if (id == _oldId)
                    {
                        _charger.StopCharge();
                        _door.UnlockDoor();
                        _logger.WriteLine(DateTime.Now + ": Skab låst op med RFID: {0}", id);
                        _display.StationMessage = "Fjern telefon";
                        _state = LadeskabState.Available;
                    }
                    else
                    {
                        _display.StationMessage = "Forkert RFID tag";
                    }

                    break;
            }
        }

        // Her mangler de andre trigger handlere
        private void OnDoorChanged(DoorChangedEventArgs.ChangeTypeEnum change)
        {
            if (_state == LadeskabState.Locked) return;

            if (change == DoorChangedEventArgs.ChangeTypeEnum.DoorOpened)
            {
                _state = LadeskabState.DoorOpen;
                _display.StationMessage = "Tilslut telefon";
            }
            else if (change == DoorChangedEventArgs.ChangeTypeEnum.DoorClosed)
            {
                _state = LadeskabState.Available;
                _display.StationMessage = "Indlæs RFID";
            }
        }
    }
}
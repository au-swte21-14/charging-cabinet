using System;
using System.IO;
using Ladeskab.Interfaces;

namespace Ladeskab
{
    public class Display : IDisplay
    {
        private string _stationMessage = "";
        private string _chargingMessage = "";

        public string StationMessage
        {
            get => _stationMessage;
            set
            {
                _stationMessage = value;
                Update();
            }
        }

        public string ChargingMessage
        {
            get => _chargingMessage;
            set
            {
                _chargingMessage = value;
                Update();
            }
        }

        private void Update()
        {
            // CI doesn't support this function
            // Console.Clear();
            Console.WriteLine(_stationMessage);
            Console.WriteLine(_chargingMessage);
        }
    }
}
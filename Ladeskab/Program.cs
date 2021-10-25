using System;
using Ladeskab.Interfaces;
using UsbSimulator;

namespace Ladeskab
{
    class Program
    {
        static void Main(string[] args)
        {
            // Assemble your system here from all the classes
            IDoor door = new Door();
            IRfidReader rfidReader = new RfidReader();
            IDisplay display = new Display();
            IChargeControl chargeControl = new ChargeControl(new UsbChargerSimulator(), display);
            ILogger logger = new Logger();
            
            var unused = new StationControl(door, rfidReader, chargeControl, display, logger);
            bool finish = false;
            do
            {
                string input;
                System.Console.WriteLine("Indtast E, O, C, R: ");
                input = Console.ReadLine();
                if (string.IsNullOrEmpty(input)) continue;

                switch (input[0])
                {
                    case 'E':
                        finish = true;
                        break;

                    case 'O':
                        door.OnDoorOpen();
                        break;

                    case 'C':
                        door.OnDoorClose();
                        break;

                    case 'R':
                        System.Console.WriteLine("Indtast RFID id: ");
                        string idString = System.Console.ReadLine();

                        int id = Convert.ToInt32(idString);
                        rfidReader.OnRfidRead(id);
                        break;

                    default:
                        break;
                }
            } while (!finish);
        }
    }
}
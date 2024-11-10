﻿using System;
using System.Collections.Generic;
using static BleWinrt;


namespace DebugBle
{
    class Program
    {
        static readonly BleWinrt ble = new BleWinrt();
        static readonly HashSet<ulong> set = new HashSet<ulong>();

		static void Main(string[] args)
        {
			Console.WriteLine($"scan started");

            ble.Advertisement += OnAdvertisement;
			ble.ScanStopped += OnStopped;
            ble.StartScan();

			Console.WriteLine("Press enter to exit the program...");
            Console.ReadLine();
        }

        static async void OnAdvertisement(BleWinrt.BleAdvert ad)
        {
            Console.WriteLine(ad);

            //check if added it
            if (set.Add(ad.mac))
            {
                List<BleService> services = await ble.GetServices(ad.mac);

				string str = ad + " >>> " + services.Count + " service(s)";

				if (services.Count > 0)
					str += "\n";

				for (int i = 0; i < services.Count; i++)
                {
                    Guid serviceUuid = services[i].serviceUuid;

					str += $"- {serviceUuid}\n";

					List<BleCharacteristic> characteristics = await ble.GetCharacteristics(ad.mac, serviceUuid);

					for (int j = 0; j < characteristics.Count; j++)
					{
						Guid charUuid = characteristics[j].serviceUuid;

						str += $"  {charUuid}\n";
                    }

					str += "\n";
				}

				Console.WriteLine(str);
			}
		}

		static void OnCompleted()
		{
			Console.WriteLine($"scan completed");
		}

		static void OnStopped()
        {
			Console.WriteLine($"scan stopped");
		}
	}
}

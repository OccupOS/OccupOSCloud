﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="OccupOS">
//   This file is part of OccupOS.
//   OccupOS is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//   OccupOS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//   You should have received a copy of the GNU General Public License along with OccupOS.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OccupOSCloud
{
    using System;

    internal class Program
    {
        private static void Main(string[] args)
        {
            SQLServerHelper helper;
            DateTime now = DateTime.Parse("2013-04-13 01:05:22.0000000");
            int data;
            int count = 10;

            // helper = new SQLServerHelper("tcp:dndo40zalb.database.windows.net,1433", "comp2014@dndo40zalb", "20041908kjH", "TestSQLDB");
            helper = new SQLServerHelper("Data Source=DANS-PC; Database=OccupOS;Trusted_Connection=True;");
            while (true)
            {
                data = 40 + new Random().Next(5);

                // if (helper.InsertSensorData(1, 1, (data).ToString(), now.AddSeconds(count), 3) > 0)
                if (helper.InsertSensorData(1, 1, data.ToString(), DateTime.Now, DateTime.Now, 3) > 0)
                {
                    // System.Diagnostics.Debug.WriteLine("Light Data is inserted");
                    Console.WriteLine("Light Data is inserted");
                }
                else
                {
                    // System.Diagnostics.Debug.WriteLine("There was a problem inserting Light Data");
                    Console.WriteLine("There was a problem inserting Light Data");
                }

                // System.Diagnostics.Debug.WriteLine(data);
                Console.WriteLine(DateTime.Now);
                Console.WriteLine(data);
                data = 20 + new Random().Next(10);

                // if (helper.InsertSensorData(1, 1, (data).ToString(), now.AddSeconds(count), 9) > 0)
                if (helper.InsertSensorData(1, 1, data.ToString(), DateTime.Now, DateTime.Now, 9) > 0)
                {
                    // System.Diagnostics.Debug.WriteLine("Temperature Data is inserted");
                    Console.WriteLine("Temperature Data is inserted");
                }
                else
                {
                    // System.Diagnostics.Debug.WriteLine("There was a problem inserting Temperature data");
                    Console.WriteLine("There was a problem inserting Temperature data");
                }

                count += 10;

                // System.Diagnostics.Debug.WriteLine(data);
                Console.WriteLine(DateTime.Now);
                Console.WriteLine(data);
                System.Threading.Thread.Sleep(10000);
            }
        }
    }
}
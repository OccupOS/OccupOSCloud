// --------------------------------------------------------------------------------------------------------------------
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
    using System.Net.Sockets;
    using System.Text;

    using OccupOSNode;

    internal class Program
    {
        private const ushort LISTENER_PORT = 1333;

        private static SQLServerHelper helper;
        private static Listener listener;

        public static void Main(string[] args)
        {
            listener = new Listener(LISTENER_PORT);
            listener.SocketAccepted += new Listener.SocketAcceptedHandler(l_SocketAccepted);
            listener.Start();

            helper = new SQLServerHelper("tcp:dndo40zalb.database.windows.net,1433", "comp2014@dndo40zalb", "20041908kjH", "TestSQLDB");

            Console.WriteLine("Listening for connections on port {0}...", LISTENER_PORT);
            Console.Read();
        }

        private static void client_Disconnected(Client sender)
        {
        }

        private static void client_Received(Client sender, byte[] rawData)
        {
            //Backup demo option:
            //createPacketDemo(sender, rawData);

            string decodedString = Encoding.UTF8.GetString(rawData);
            ReturnPacket packet = DemoDeserializer.DeserializeJSON(decodedString);

        }


        private static void createPacketDemo(Client sender, byte[] rawData) {
            char[] delimiter = new[] { ',' };
            string[] decodedData = Encoding.UTF8.GetString(rawData).Split(delimiter);

            if (decodedData[1] != "0" && decodedData[1] != "0.0") {
                DateTime dt = DateTime.Now;
                helper.InsertSensorData(1, 1, decodedData[1], dt, 3);

                // 3 for LightSensor (Note: humid: 5, pressure: 7, temp: 9)
                Console.WriteLine(
                    "Message from {0}:\nAnalogLight: {1}\nPolled at: {2}\n", sender.ID, decodedData[1], dt);
            } else if ((decodedData[3] != "0" && decodedData[3] != "0.0")
                       || (decodedData[4] != "0" && decodedData[4] != "0.0")
                       || (decodedData[5] != "0" && decodedData[5] != "0.0")) {
                DateTime dt = DateTime.Now;
                helper.InsertSensorData(1, 1, decodedData[3], dt, 5);
                helper.InsertSensorData(1, 1, decodedData[4], dt, 7);
                helper.InsertSensorData(1, 1, decodedData[5], dt, 9);
                Console.WriteLine(
                    "Message from {0}:\nHumidity: {2}\nPressure: {3}\nTemperature: {4}\nPolled at: {1}\n",
                    sender.ID,
                    dt,
                    decodedData[3],
                    decodedData[4],
                    decodedData[5]);
            }
        }

        private static void l_SocketAccepted(Socket e)
        {
            Console.WriteLine("Connection established!");
            Client client = new Client(e);
            client.Received += client_Received;
            client.Disconnected += client_Disconnected;
        }
    }
}
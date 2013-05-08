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

           // helper = new SQLServerHelper("tcp:dndo40zalb.database.windows.net,1433", "comp2014@dndo40zalb", "20041908kjH", "TestSQLDB");
            helper = new SQLServerHelper("Data Source=DANS-PC; Database=OccupOS;Trusted_Connection=True;");

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

            for (int k = 0; k < packet.data.Length; k++) {
                ReturnSensor currentdata = packet.data[k];
                //DateTime rdt = currentdata.ReadTime;
                DateTime rdt = DateTime.Now;
                //DateTime pdt = currentdata.PollTime;
                DateTime pdt = DateTime.Now;
                if (currentdata.Humidity != -1) {
                    helper.InsertSensorData(1, 1, currentdata.Humidity.ToString(), rdt, pdt, 5);
                    Console.WriteLine("Inserted Humidity: " + currentdata.Humidity.ToString() + " polled at: " + rdt);
                }
                if (currentdata.Pressure != -1) {
                    helper.InsertSensorData(1, 1, currentdata.Pressure.ToString(), rdt, pdt, 7);
                    Console.WriteLine("Inserted Pressure: " + currentdata.Pressure.ToString() + " polled at: " + rdt);
                }
                if (currentdata.Temperature != -1) {
                    helper.InsertSensorData(1, 1, currentdata.Temperature.ToString(), rdt, pdt, 9);
                    Console.WriteLine("Inserted Temperature: " + currentdata.Temperature.ToString() + " polled at: " + rdt);
                }
                if (currentdata.EntityCount != -1) {
                    helper.InsertSensorData(3, 1, currentdata.EntityCount.ToString(), rdt, pdt, 0);
                    Console.WriteLine("Inserted EntityCount: " + currentdata.Temperature.ToString() + " polled at: " + rdt);
                }
                if (packet.data[k].EntityPositions != null) {
                    for (int h = 0; h < currentdata.EntityPositions.Length; h++) {
                        Position currentposition = currentdata.EntityPositions[h];
                        if (currentposition.X != -1 && currentposition.Y != -1 && currentposition.Depth != -1) {
                            String s_pos = currentposition.X.ToString() + "," +
                                currentposition.Y.ToString() + "," + currentposition.Depth.ToString();
                            helper.InsertSensorData(3, 1, s_pos, rdt, pdt, 1);
                            Console.WriteLine("Inserted Position: " + s_pos + " polled at: " + rdt);
                        }
                    }
                }
                if (currentdata.AnalogLight != -1) {
                    helper.InsertSensorData(1, 1, currentdata.AnalogLight.ToString(), rdt, pdt, 3);
                    Console.WriteLine("Inserted Light: " + currentdata.AnalogLight.ToString() + " polled at: " + rdt);
                }
            }
        }


        private static void createPacketDemo(Client sender, byte[] rawData) {
            char[] delimiter = new[] { ',' };
            string[] decodedData = Encoding.UTF8.GetString(rawData).Split(delimiter);

            if (decodedData[1] != "0" && decodedData[1] != "0.0") {
                DateTime dt = DateTime.Now;
                helper.InsertSensorData(1, 1, decodedData[1], dt, dt, 3);

                // 3 for LightSensor (Note: humid: 5, pressure: 7, temp: 9)
                Console.WriteLine(
                    "Message from {0}:\nAnalogLight: {1}\nPolled at: {2}\n", sender.ID, decodedData[1], dt);
            } else if ((decodedData[3] != "0" && decodedData[3] != "0.0")
                       || (decodedData[4] != "0" && decodedData[4] != "0.0")
                       || (decodedData[5] != "0" && decodedData[5] != "0.0")) {
                DateTime dt = DateTime.Now;
                helper.InsertSensorData(1, 1, decodedData[3], dt, dt, 5);
                helper.InsertSensorData(1, 1, decodedData[4], dt, dt, 7);
                helper.InsertSensorData(1, 1, decodedData[5], dt, dt, 9);
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
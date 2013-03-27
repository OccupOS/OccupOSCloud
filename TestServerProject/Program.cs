using System;
using System.Text;
using OccupOSNode;
using System.Globalization;

namespace OccupOSCloud
{
    internal class Program
    {
        private static Listener listener;
        private static SQLServerHelper helper;

        public static void Main(string[] args)
        {
            listener = new Listener(1333);
            listener.SocketAccepted += new Listener.SocketAcceptedHandler(l_SocketAccepted);
            listener.Start();

            helper = new SQLServerHelper("tcp:dndo40zalb.database.windows.net,1433", "comp2014@dndo40zalb", "20041908kjH", "TestSQLDB");

            Console.WriteLine("Listening for connections...");
            Console.Read();
        }

        private static void l_SocketAccepted(System.Net.Sockets.Socket e)
        {
            Console.WriteLine("Connection established!");
            Client client = new Client(e);
            client.Received += client_Received;
            client.Disconnected += client_Disconnected;
        }

        private static void client_Received(Client sender, byte[] rawData) {
            char[] delimiter = new char[1] { ',' };
            string[] decodedData = Encoding.UTF8.GetString(rawData).Split(delimiter);

            if (decodedData[1] != "0" && decodedData[1] != "0.0")
            {
                System.DateTime dt = DateTime.Now;
                helper.InsertSensorData(1, 1, decodedData[1], dt, 3); //3 for LightSensor (Note: humid: 5, pressure: 7, temp: 9)
                Console.WriteLine("Message from {0}:\nAnalogLight: {1}\nPolled at: {2}\n", sender.ID, decodedData[1], dt);
            }
            else if ((decodedData[3] != "0" && decodedData[3] != "0.0") || (decodedData[4] != "0" && decodedData[4] != "0.0") || (decodedData[5] != "0" && decodedData[5] != "0.0"))
            {
                System.DateTime dt = DateTime.Now;
                helper.InsertSensorData(1, 1, decodedData[3], dt, 5);
                helper.InsertSensorData(1, 1, decodedData[4], dt, 7);
                helper.InsertSensorData(1, 1, decodedData[5], dt, 9);
                Console.WriteLine("Message from {0}:\nHumidity: {2}\nPressure: {3}\nTemperature: {4}\nPolled at: {1}\n", sender.ID, dt, decodedData[3], decodedData[4], decodedData[5]);
            }
        }

        private static void client_Disconnected(Client sender)
        {

        }
    }
}

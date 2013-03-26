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

        private static void client_Received(Client sender, byte[] rawData)
        {
            char[] delimiter = new char[1] {','};
            string[] decodedData = Encoding.UTF8.GetString(rawData).Split(delimiter);

            Console.WriteLine("Message from {0}:\nAnalogLight: {2}\nMeasuredAt: {1}", sender.ID, decodedData[0], decodedData[1]);

            helper.InsertSensorData(1, 1, decodedData[1], DateTime.Parse(decodedData[0], new CultureInfo("en-US")));
        }

        private static void client_Disconnected(Client sender)
        {

        }
    }
}

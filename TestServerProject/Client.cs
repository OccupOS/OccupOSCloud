// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Client.cs" company="OccupOS">
//   This file is part of OccupOS.
//   OccupOS is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//   OccupOS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//   You should have received a copy of the GNU General Public License along with OccupOS.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OccupOSNode
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    internal class Client
    {
        private Socket sck;

        public Client(Socket accepted)
        {
            this.sck = accepted;
            this.ID = Guid.NewGuid().ToString();
            this.EndPoing = (IPEndPoint)this.sck.RemoteEndPoint;
            this.sck.BeginReceive(new byte[] { 0 }, 0, 0, 0, this.callback, null);
        }

        public delegate void ClientDisconnectedHandler(Client sender);

        public delegate void ClientReceivedHandler(Client sender, byte[] data);

        public event ClientDisconnectedHandler Disconnected;

        public event ClientReceivedHandler Received;

        public IPEndPoint EndPoing { get; private set; }

        public string ID { get; private set; }

        public void Close()
        {
            this.sck.Close();
            this.sck.Dispose();
        }

        private void callback(IAsyncResult ar)
        {
            try
            {
                this.sck.EndReceive(ar);

                byte[] buf = new byte[8192];

                int rec = this.sck.Receive(buf, buf.Length, 0);

                if (rec < buf.Length)
                {
                    Array.Resize<byte>(ref buf, rec);
                }

                if (this.Received != null)
                {
                    this.Received(this, buf);
                }

                this.sck.BeginReceive(new byte[] { 0 }, 0, 0, 0, this.callback, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                this.Close();

                if (this.Disconnected != null)
                {
                    this.Disconnected(this);
                }
            }
        }
    }
}
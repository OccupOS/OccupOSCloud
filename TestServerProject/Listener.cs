// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Listener.cs" company="OccupOS">
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

    internal class Listener
    {
        private Socket s;

        public Listener(int port)
        {
            this.Port = port;
            this.s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public delegate void SocketAcceptedHandler(Socket e);

        public event SocketAcceptedHandler SocketAccepted;

        public bool Listening { get; private set; }

        public int Port { get; private set; }

        public void Start()
        {
            if (this.Listening)
            {
                return;
            }

            this.s.Bind(new IPEndPoint(IPAddress.Any, this.Port));
            this.s.Listen(0);
            this.s.BeginAccept(this.callback, null);
            this.Listening = true;
        }

        public void Stop()
        {
            if (!this.Listening)
            {
                return;
            }

            this.s.Close();
            this.s.Dispose();
            this.s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        private void callback(IAsyncResult ar)
        {
            try
            {
                Socket s = this.s.EndAccept(ar);
                if (this.SocketAccepted != null)
                {
                    this.SocketAccepted(s);
                }

                this.s.BeginAccept(this.callback, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
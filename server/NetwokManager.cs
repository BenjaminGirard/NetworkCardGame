using System;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.TCP;
using System.Net;
using System.Collections.Concurrent;

namespace server
{
    public class NetworkManager
    {
        ConcurrentBag<ClientInfos> clients;

        public NetworkManager(string ip, int port)
        {
            Connection.StartListening(ConnectionType.TCP, new System.Net.IPEndPoint(BitConverter.ToInt32(IPAddress.Parse(ip).GetAddressBytes(), 0), port));
            //Print out the IPs and ports we are now listening on
            Console.WriteLine("Server listening for TCP connection on:");
            foreach (System.Net.IPEndPoint localEndPoint in Connection.ExistingLocalListenEndPoints(ConnectionType.TCP))
                Console.WriteLine("{0}:{1}", localEndPoint.Address, localEndPoint.Port);

            NetworkComms.AppendGlobalConnectionEstablishHandler(NewClientConnectionHandler);
            NetworkComms.AppendGlobalConnectionCloseHandler(ShutdownConnectionHandler);

//            NetworkComms.GetExistingConnection();
        }

        ~NetworkManager()
        {
            //We have used NetworkComms so we should ensure that we correctly call shutdown
            NetworkComms.Shutdown();
            Console.WriteLine("\nServer closing...");
        }

        /// <summary>
        /// Writes the provided message to the console window
        /// </summary>
        /// <param name="header">The packet header associated with the incoming message</param>
        /// <param name="connection">The connection used by the incoming message</param>
        /// <param name="message">The message to be printed to the console</param>
        private static void PrintIncomingMessage(PacketHeader header, Connection connection, string message)
        {
            Console.WriteLine("\nA message was received from " + connection.ToString() + " which said '" + message + "'.");
        }

        private static void NewClientConnectionHandler(Connection connection)
        {
            Console.WriteLine("\nNew client Incomming ! id : " + connection.ToString());
        }

        private static void ShutdownConnectionHandler(Connection connection)
        {
            Console.WriteLine("\nA client disconnected ! id : " + connection.ToString());
        }

    }
}
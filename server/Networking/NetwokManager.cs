using System;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.TCP;
using System.Net;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Protocol;
using Protocol.ConnectionRequests.ClientConnection;
using Protocol.ConnectionRequests.RoomConnection;
using Protocol.ConnectionRequests.Notifications;
using Protocol.Game.Notifications;
using Protocol.Game.Player_actions;
using server.Networking;

namespace sever
{
    public class NetworkManager
    {
        private int _port;
        private List<ClientInfo> clients;

        internal List<ClientInfo> Clients { get => clients; set => clients = value; }

        public NetworkManager(int port)
        {
            _port = port;
            Clients = new List<ClientInfo>();

            addPacketHandlers();
        }

        ~NetworkManager()
        {
            //We have used NetworkComms so we should ensure that we correctly call shutdown
            NetworkComms.Shutdown();
            Console.WriteLine("\nServer closing...");
        }


        public void StartListening()
        {
            Connection.StartListening(ConnectionType.TCP, new System.Net.IPEndPoint(IPAddress.Any, _port));
            //Print out the IPs and ports we are now listening on
            Console.WriteLine("Server listening for TCP connection on:");
            foreach (System.Net.IPEndPoint localEndPoint in Connection.ExistingLocalListenEndPoints(ConnectionType.TCP))
                Console.WriteLine("{0}:{1}", localEndPoint.Address, localEndPoint.Port);

        }

        /// <summary>
        /// Writes the provided message to the console window
        /// </summary>
        /// <param name="header">The packet header associated with the incoming message</param>
        /// <param name="connection">The connection used by the incoming message</param>
        /// <param name="message">The Object to be sent to the packet handler of the client</param>
        private void StackClientRequestToQueue(PacketHeader header, Connection connection, Object request)
        {
            lock (Clients)
            {
                ClientInfo client = GetClientByConnection(connection);

                client.Requests.Add(new Tuple<string, Object>(header.PacketType, request));
                Monitor.PulseAll(Clients);
            }
        }

        private void NewClientConnectionHandler(Connection connection)
        {
            Console.WriteLine("\nNew client Incomming ! id : " + connection.ToString());
            Clients.Add(new ClientInfo(connection));
            connection.SendObject("UsernameAskRequest", new UsernameAskRequest());
        }

        private void ShutdownConnectionHandler(Connection connection)
        {
            Console.WriteLine("\nA client disconnected ! id : " + connection.ToString());
            lock (Clients)
            {
                ClientInfo client = GetClientByConnection(connection);
                client.Requests.Add(new Tuple<string, Object>("Disconnect", new DisconnectRequest()));
                Monitor.PulseAll(Clients);
            }
        }

        private ClientInfo GetClientByConnection(Connection connection)
        {
            foreach (ClientInfo client in Clients)
                if (client.Instance == connection)
                    return client;
            return null;
        }

        public Connection GetConnectionByClientId(string id)
        {
            foreach (ClientInfo client in Clients)
                if (client.Id.Equals(id))
                    return client.Instance;

            return null;
        }
        
        public void SendAllClientsRequest(string header, Object request)
        {
            foreach (ClientInfo client in Clients)
                client.Instance.SendObject(header, request);
        }
        private void addPacketHandlers()
        {
            // Conection
            //\\ Client Connection
            NetworkComms.AppendGlobalIncomingPacketHandler<ClientConnectionPing>("ClientConnectionPing", StackClientRequestToQueue);
            NetworkComms.AppendGlobalIncomingPacketHandler<UsernameAuth>("UsernameAuth", StackClientRequestToQueue);
 
            //\\ RoomConnection
            NetworkComms.AppendGlobalIncomingPacketHandler<AskCreateRoom>("AskCreateRoom", StackClientRequestToQueue);
            NetworkComms.AppendGlobalIncomingPacketHandler<AskMovingIntoRoom>("AskMovingIntoRoom", StackClientRequestToQueue);
            NetworkComms.AppendGlobalIncomingPacketHandler<LeaveRoom>("LeaveRoom", StackClientRequestToQueue);

            //\\Notifications
            NetworkComms.AppendGlobalIncomingPacketHandler<ClientConnectionRoomEvent>("ClientConnectionRoomEvent", StackClientRequestToQueue);
            NetworkComms.AppendGlobalIncomingPacketHandler<RoomCreated>("RoomCreated", StackClientRequestToQueue);

            // </connection>

            // Game
            //\\ Notifications

            NetworkComms.AppendGlobalIncomingPacketHandler<IAmReady>("IAmReady", StackClientRequestToQueue);

            //\\ Player actions
            NetworkComms.AppendGlobalIncomingPacketHandler<Call>("Call", StackClientRequestToQueue);
            NetworkComms.AppendGlobalIncomingPacketHandler<CardPlayed>("CardPlayed", StackClientRequestToQueue);
            NetworkComms.AppendGlobalIncomingPacketHandler<ManagedDogReturn>("ManagedDogReturn", StackClientRequestToQueue);


            // </Game>


            // Connection Disconnection handlers
            NetworkComms.AppendGlobalConnectionEstablishHandler(NewClientConnectionHandler);
            NetworkComms.AppendGlobalConnectionCloseHandler(ShutdownConnectionHandler);
        }
    }
}
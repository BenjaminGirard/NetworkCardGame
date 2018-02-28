using System;
using System.Collections.Generic;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.TCP;
using Protocol;
using Protocol.ConnectionRequests.ClientConnection;
using Protocol.ConnectionRequests.Notifications;
using Protocol.ConnectionRequests.RoomConnection;
using Protocol.Game.Notifications;
using Protocol.Game.Player_actions;

namespace client
{
    public class NetworkManager
    {
        private string ServerIp { get; }
        private int ServerPort { get; }
        public Connection Server { get; }
        private User _user;

        public NetworkManager(string serverIp, int serverPort, User user)
        {
            NetworkComms.AppendGlobalConnectionEstablishHandler(OnConnectionEtablished);
            NetworkComms.AppendGlobalConnectionCloseHandler(OnConnectionClosed);
            addPacketHandlers();
            ServerPort = serverPort;
            _user = user;
            ServerIp = serverIp;
            Server = TCPConnection.GetConnection(new ConnectionInfo(ServerIp, ServerPort));
        }

        private void OnConnectionEtablished(Connection connection)
        {
            Console.WriteLine("Connection established");
        }

        private void OnConnectionClosed(Connection connection)
        {
            Console.WriteLine("Connection closed");
        }

        private void Prompt()
        {
            lock (_user)
                Console.Write("Tarot [" + _user.Username + "] > ");
        }

        private void AskUsernameHandler(PacketHeader header, Connection connection, UsernameAskRequest incomingObject)
        {
            lock (_user)
                Server.SendObject("UsernameAuth", new UsernameAuth(_user.Username));
        }

        private void UsernameAuthAnswerHandler(PacketHeader header, Connection connection,
            UsernameAuthAnswer incomingObject)
        {
            if (incomingObject.IsAuthenticationValid) return;
            Console.WriteLine();
            Console.WriteLine("Username is already taken please choose another one");
            lock (_user)
            {
                while (String.IsNullOrWhiteSpace(_user.Username = Console.ReadLine()))
                    Console.WriteLine("Please enter valid username");
                Server.SendObject("UsernameAuth", new UsernameAuth(_user.Username));
            }
            Prompt();
        }

        private void DataFromEstablishedConnectionHandler(PacketHeader header, Connection connection,
            DataFromEstablishedConnection incomingObject)
        {
            Console.WriteLine();
            lock (_user)
            {
                Console.WriteLine("Successfully connected to server as " + _user.Username);
                _user.RoomsHandler = new RoomsHandler(incomingObject.Rooms);
                _user.PlayerInfo = incomingObject.Player;
                _user.IsAuthenticated = true;
            }
            Prompt();
        }

        private void CreateRoomAnswerHandler(PacketHeader header, Connection connection,
            CreateRoomRequestAnswer incomingObject)
        {
            Console.WriteLine();
            Console.WriteLine(incomingObject.IsRoomCreated ? "Room created" : "Failed to create room");
            Prompt();
        }

        private void MovingIntoRoomAnswerHandler(PacketHeader header, Connection connection,
            MovingIntoRoomAnswer incomingObject)
        {
            Console.WriteLine();
            switch (incomingObject.Response)
            {
                case MovingIntoRoomAnswerType.OK:
                    Console.WriteLine("Sucessfully moved into room");
                    break;
                case MovingIntoRoomAnswerType.FAIL:
                    Console.WriteLine("Failed to move into room");
                    break;
                case MovingIntoRoomAnswerType.ROOM_FULL:
                    Console.WriteLine("Romm is full");
                    break;
                default:
                    Console.WriteLine("Error: Incorrect server response");
                    break;
            }
            Prompt();
        }

        private void ClientConnectionRoomEventHandler(PacketHeader header, Connection connection,
            ClientConnectionRoomEvent incomingObject)
        {
            lock (_user)
            {
                if (incomingObject.Type == ConnectionInfoType.CONNECT)
                {
                    _user.RoomsHandler?.AddPlayerToRoom(incomingObject.Player, incomingObject.Room);
                    if (incomingObject.Player.Id.Equals(_user.PlayerInfo.Id) ||
                        incomingObject.Room.Id.Equals(_user.RoomInfo.Id))
                        _user.RoomInfo = incomingObject.Room;
                }
                else if (incomingObject.Type == ConnectionInfoType.DISCONNECT)
                {
                    _user.RoomsHandler?.RemovePlayerToRoom(incomingObject.Player, incomingObject.Room);
                    _user.RoomInfo = incomingObject.Player.Id.Equals(_user.PlayerInfo.Id) ? null : incomingObject.Room;
                }
            }
        }

        private void RoomCreatedHandler(PacketHeader header, Connection connection, RoomCreated incomingObject)
        {
            lock (_user)
                _user.RoomsHandler?.CreateRoom(incomingObject.Room);
        }

        private void AreYouReadyHandler(PacketHeader header, Connection connection, AreYouReady incomingObject)
        {
            Console.WriteLine();
            Console.WriteLine("Your room is full are you ready to start the game ? if so enter the \"ready\" command");
            Prompt();
        }

        private void CardDistributedHandler(PacketHeader header, Connection connection, CardDistributed incomingObject)
        {
            lock (_user)
                _user.CardsHandler.AddCardToHand(incomingObject.Card);
        }

        private void YourTurnCallHandler(PacketHeader header, Connection connection, YourTurnCall incomingObject)
        {
            Console.WriteLine();
            Console.WriteLine("It's your turn to make a call ! make a call with the \"call\" command");
            Console.WriteLine("To see your hand use the \"hand\" command");
            lock (_user)
            {
                _user.CallHandler.PrintCall();
                _user.MyTurnToCall = true;
            }
            Prompt();
        }

        private void YourTurnPlayHandler(PacketHeader header, Connection connection, YourTurnPlayCard incomingObject)
        {
            Console.WriteLine();
            Console.WriteLine("It's your turn to play a card ! make a call with the \"play_card\" command");
            Prompt();
            lock (_user)
            {
                _user.MyTurnToPlay = true;
            }
        }

        private void CallHandler(PacketHeader header, Connection connection, Call incomingObject)
        {
            lock (_user)
            {
                if (!_user.PlayerInfo.Id.Equals(incomingObject.Player.Id))
                {
                    Console.WriteLine();
                    _user.CallHandler.PrintCall(incomingObject);
                    Prompt();
                }
                _user.CallHandler.AddCall(incomingObject);
            }
        }

        private void CardPlayedHandler(PacketHeader header, Connection connection, CardPlayed incomingObject)
        {
            lock (_user)
            {
                if (!_user.PlayerInfo.Id.Equals(incomingObject.Player.Id))
                {
                    Console.WriteLine();
                    Console.WriteLine("Player " + incomingObject.Player.Username + " played " +
                                      _user.CardsHandler.CardToReadableString(incomingObject.Card));
                    Prompt();
                }
                _user.CardsHandler.AddCardToBoard(incomingObject);
            }
        }

        private void TurnOverHandler(PacketHeader header, Connection connection, TurnOver incomingObject)
        {
            lock (_user)
                _user.CardsHandler.ClearBoard();
        }

        private void GameOverHandler(PacketHeader header, Connection connection, GameOver incomingObject)
        {
            Console.WriteLine();
            Console.WriteLine("Game is over ! Winners are :");
            foreach (var player in incomingObject.Winners)
                Console.WriteLine("\t" + player.Username);
            Console.WriteLine("Scores :");
            foreach (var playerScore in incomingObject.Scores)
                Console.WriteLine("\t" + playerScore.Item1.Username + " : " + playerScore.Item1);
            Prompt();
            lock (_user)
            {
                _user.CardsHandler.ClearBoard();
                _user.CardsHandler.ClearHand();
                _user.CardsHandler.ClearDog();
                _user.CallHandler.Clear();
            }
        }

        private void PartyCancelledHandler(PacketHeader header, Connection connection, PartyCancelled incomingObject)
        {
            lock (_user)
            {
                _user.CardsHandler.ClearBoard();
                _user.CardsHandler.ClearHand();
                _user.CardsHandler.ClearDog();
                _user.CallHandler.Clear();
            }
        }

        private void GameCancelledHandler(PacketHeader header, Connection connection, GameCancelled
            incomingObject)
        {
            Console.WriteLine();
            Console.WriteLine("Everyone has passed cards are re-distribued");
            Prompt();
            lock (_user)
            {
                _user.CardsHandler.ClearHand();
                _user.CardsHandler.ClearDog();
                _user.CallHandler.Clear();
            }
        }

        void PrintDog(List<Card> dog)
        {
            foreach (var card in dog)
                Console.WriteLine("\t" + _user.CardsHandler.CardToReadableString(card));
        }

        private void PlayerWinningCallInfoHandler(PacketHeader header, Connection connection,
            PlayerWinningCallInfo incomingObject)
        {
            lock (_user)
            {
                if (!incomingObject.Player.Id.Equals(_user.PlayerInfo.Id))
                {
                    Console.WriteLine(incomingObject.Player.Username + " took a " +
                                      _user.CallHandler.GetCallString(incomingObject.CallType) + ", dog composition :");
                    PrintDog(incomingObject.Dog);
                }
                else
                {
                    _user.HasWinCall = true;
                    Console.WriteLine("You took a " + _user.CallHandler.GetCallString(incomingObject.CallType) +
                                      ", dog composition :");
                    _user.CardsHandler.AddDogToHand(incomingObject.Dog);
                    PrintDog(incomingObject.Dog);
                    Console.WriteLine("Please make your dog using the \"add_dog\" command");
                }
            }
            Prompt();
        }

        private void addPacketHandlers()
        {
            // Conection

            //\\ Client Connection
            NetworkComms.AppendGlobalIncomingPacketHandler<UsernameAskRequest>("UsernameAskRequest",
                AskUsernameHandler);
            NetworkComms.AppendGlobalIncomingPacketHandler<UsernameAuthAnswer>("UsernameAuthAnswer",
                UsernameAuthAnswerHandler);
            NetworkComms.AppendGlobalIncomingPacketHandler<DataFromEstablishedConnection>(
                "DataFromEstablishedConnection", DataFromEstablishedConnectionHandler);

            //\\ RoomConnection
            NetworkComms.AppendGlobalIncomingPacketHandler<CreateRoomRequestAnswer>("CreateRoomRequestAnswer",
                CreateRoomAnswerHandler);
            NetworkComms.AppendGlobalIncomingPacketHandler<MovingIntoRoomAnswer>("MovingIntoRoomAnswer",
                MovingIntoRoomAnswerHandler);

            //\\Notifications
            NetworkComms.AppendGlobalIncomingPacketHandler<ClientConnectionRoomEvent>("ClientConnectionRoomEvent",
                ClientConnectionRoomEventHandler);
            NetworkComms.AppendGlobalIncomingPacketHandler<RoomCreated>("RoomCreated", RoomCreatedHandler);

            // </connection>

            // Game

            //\\ Notifications
            NetworkComms.AppendGlobalIncomingPacketHandler<AreYouReady>("AreYouReady", AreYouReadyHandler);
            NetworkComms.AppendGlobalIncomingPacketHandler<CardDistributed>("CardDistributed",
                CardDistributedHandler);
            NetworkComms.AppendGlobalIncomingPacketHandler<PartyCancelled>("PartyCancelled", PartyCancelledHandler);
            NetworkComms.AppendGlobalIncomingPacketHandler<GameCancelled>("GameCancelled", GameCancelledHandler);
            NetworkComms.AppendGlobalIncomingPacketHandler<Call>("Call", CallHandler);
            NetworkComms.AppendGlobalIncomingPacketHandler<CardPlayed>("CardPlayed", CardPlayedHandler);
            NetworkComms.AppendGlobalIncomingPacketHandler<TurnOver>("TurnOver", TurnOverHandler);
            NetworkComms.AppendGlobalIncomingPacketHandler<GameOver>("GameOver", GameOverHandler);
            NetworkComms.AppendGlobalIncomingPacketHandler<PlayerWinningCallInfo>("PlayerWinningCallInfo",
                PlayerWinningCallInfoHandler);

            //\\ Player actions
            NetworkComms.AppendGlobalIncomingPacketHandler<YourTurnCall>("YourTurnCall", YourTurnCallHandler);
            NetworkComms.AppendGlobalIncomingPacketHandler<YourTurnPlayCard>("YourTurnPlayCard",
                YourTurnPlayHandler);

            // </Game>
        }
    }
}
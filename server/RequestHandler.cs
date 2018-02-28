using Protocol;
using Protocol.ConnectionRequests.ClientConnection;
using Protocol.ConnectionRequests.CommonRessources;
using Protocol.ConnectionRequests.RoomConnection;
using Protocol.Game.Notifications;
using Protocol.Game.Player_actions;
using server.GameCraft;
using sever;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class RequestHandler
    {
        NetworkManager Net;
        RoomsEntitiesManager Rooms;
        Dictionary<string, Handler> Handlers;

        delegate void Handler(ClientInfo client, Object request);
        public RequestHandler(NetworkManager net, RoomsEntitiesManager rooms)
        {
            Net = net;
            Rooms = rooms;
            Handlers = new Dictionary<string, Handler>()
            {
                { "AskCreateRoom", AskCreateRoomHandler},
                { "AskMovingIntoRoom", AskMovingIntoRoomHandler },
                { "Call", CallHandler},
                { "CardPlayed", CardPlayedHandler},
                { "Disconnect", DisconnectRequestHandler },
                { "IAmReady", IAmReadyHandler},
                { "LeaveRoom", LeaveRoomHandler},
                { "ManagedDogReturn", ManagedDogReturnHandler},
                { "UsernameAuth", UsernameAuthHandler},

            };
        }

        public void HandleClientRequest(ClientInfo client, Tuple<string, Object> request)
        {
            if (!Handlers.ContainsKey(request.Item1))
            {
                Console.WriteLine("\nError : wrong Command for client {" + client.Id + "}.");
                return;
            }
            Handlers[request.Item1](client, request.Item2);
        }

        private void AskCreateRoomHandler(ClientInfo client, Object request)
        {
            AskCreateRoom pckt = (AskCreateRoom)request;

            RoomDealer NewRoom = Rooms.CreateRoom(client);
        }

        private void AskMovingIntoRoomHandler(ClientInfo client, Object request)
        {
            AskMovingIntoRoom pckt = (AskMovingIntoRoom)request;
            RoomDealer room = Rooms.GetRoomFromRoomId(pckt.RoomId);

            if (room == null)
            {
                client.Instance.SendObject("MovingIntoRoomAnswer", new MovingIntoRoomAnswer(MovingIntoRoomAnswerType.FAIL));
                return;
            }
            if (room.RoomInfo.Players.Count() == 4)
            {
                client.Instance.SendObject("MovingIntoRoomAnswer", new MovingIntoRoomAnswer(MovingIntoRoomAnswerType.ROOM_FULL));
                return;
            }
            client.Instance.SendObject("MovingIntoRoomAnswer", new MovingIntoRoomAnswer(MovingIntoRoomAnswerType.OK));
            room.AddPlayer(client);
        }

        private void CallHandler(ClientInfo client, Object request)
        {
            Call pckt = (Call)request;
            RoomDealer room = Rooms.FindRoomFromClientId(client.Id);

            room.GameInfo.SetPlayerCall(client.Id, pckt.Type);
            room.SendMessageToRoom("Call", pckt);
            if (room.GameInfo.IsAllPlayersSkiped())
                room.GameCancelled();
            else if (room.GameInfo.IsGameCallOver())
                room.StartPlayerDogHandling();
            else
                Net.GetConnectionByClientId(room.GetNextPlayerIdTurnById(client.Id)).SendObject("YourTurnCall", new YourTurnCall());

        }

        private void CardPlayedHandler(ClientInfo client, Object request)
        {
            CardPlayed pckt = (CardPlayed)request;
            RoomDealer RoomOfClient = Rooms.FindRoomFromClientId(client.Id);

            RoomOfClient.GameInfo.PlayersInfo[client.Id].CardPlayedOnTable = pckt.Card;
            RoomOfClient.GameInfo.PlayersInfo[client.Id].RemoveCardPlayed(pckt.Card);
            RoomOfClient.SendMessageToRoom("CardPlayed", pckt);
            if (RoomOfClient.GameInfo.IsTurnOver())
                RoomOfClient.HandleTurnOver();
            else
            {
                if (RoomOfClient.GameInfo.IsFirstCardToPlayed())
                    RoomOfClient.GameInfo.ColorPlayed = pckt.Card.Color;
                Net.GetConnectionByClientId(RoomOfClient.GetNextPlayerIdTurnById(client.Id)).SendObject("YourTurnPlayCard", new YourTurnPlayCard());
            }
        }

        private void DisconnectRequestHandler(ClientInfo client, Object request)
        {
            LeaveRoomHandler(client, request);
            Net.Clients.Remove(client);
        }

        private void IAmReadyHandler(ClientInfo client, Object request)
        {
            RoomDealer RoomOfClient = Rooms.FindRoomFromClientId(client.Id);
            RoomOfClient.GameInfo.PlayersInfo[client.Id].IsReady = true;
            if (RoomOfClient.IsAllPlayersReady())
                RoomOfClient.StartGame();
        }

        private void LeaveRoomHandler(ClientInfo client, Object request)
        {
            Rooms.TryRemoveClient(client);
        }

        private void ManagedDogReturnHandler(ClientInfo client, Object request)
        {
            ManagedDogReturn pckt = (ManagedDogReturn)request;
            RoomDealer RoomOfClient = Rooms.FindRoomFromClientId(client.Id);

            RoomOfClient.GameInfo.Dog = pckt.Dog;
            RoomOfClient.GameInfo.DropCardsDogFromPlayerHand(pckt.Dog);
            Net.GetConnectionByClientId(RoomOfClient.RoomInfo.Players[RoomOfClient.StartingPlayer].Id).SendObject("YourTurnPlayCard", new YourTurnPlayCard());
        }

        private void UsernameAuthHandler(ClientInfo client, Object request)
        {
            UsernameAuth pckt = (UsernameAuth)request;

            foreach (ClientInfo c in Net.Clients)
            {
                if (c.Username.Equals(pckt.Username))
                {
                    client.Instance.SendObject("UsernameAuthAnswer", new UsernameAuthAnswer(false));
                    return;
                }
            }
            client.Username = pckt.Username;
            client.Instance.SendObject("UsernameAuthAnswer", new UsernameAuthAnswer(true));
            client.Instance.SendObject("DataFromEstablishedConnection", new DataFromEstablishedConnection(new Player(client.Id, client.Username), Rooms.GetRoomInfoList()));
        }
    }
}

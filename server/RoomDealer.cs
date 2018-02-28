using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using Protocol;
using Protocol.ConnectionRequests.CommonRessources;
using Protocol.ConnectionRequests.Notifications;
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
    class RoomDealer
    {
        NetworkManager Net;
        public RoomInfo RoomInfo { get; set; }
        public GameInfo GameInfo { get; set; }
        public int StartingPlayer { get; set; }

        public RoomDealer(NetworkManager net, ClientInfo player, int id)
        {
            Net = net;
            StartingPlayer = 0;
            RoomInfo = new RoomInfo(id, new List<Player>());
            GameInfo = new GameInfo();
            Net.SendAllClientsRequest("RoomCreated", new RoomCreated(RoomInfo));
            Net.GetConnectionByClientId(player.Id)?.SendObject("CreateRoomRequestAnswer", new CreateRoomRequestAnswer(true));
            AddPlayer(player);
        }

        public void NextStartingPlayer()
        {
            StartingPlayer = (StartingPlayer + 1) % 4;
        }

        public void AddPlayer(ClientInfo player)
        {
            Player NewPlayer = new Player(player.Id, player.Username);
            RoomInfo.Players.Add(NewPlayer);
            GameInfo.PlayersInfo.Add(player.Id, new PlayerGameInfo());
            Net.SendAllClientsRequest("ClientConnectionRoomEvent", new ClientConnectionRoomEvent(ConnectionInfoType.CONNECT, RoomInfo, NewPlayer));

            if (RoomInfo.Players.Count == 4)
                SendMessageToRoom("AreYouReady", new AreYouReady());
        }
        
        public Boolean TryRemovePlayer(ClientInfo Client)
        {
            foreach (Player player in RoomInfo.Players)
                if (player.Id.Equals(Client.Id))
                {
                    if (RoomInfo.Players.Count == 4)
                    {
                        GameInfo.ResetParty();
                        SendMessageToRoom("PartyCancelled", new PartyCancelled());
                    }
                    GameInfo.PlayersInfo.Remove(player.Id);
                    RoomInfo.Players.Remove(player);
                    Net.SendAllClientsRequest("ClientConnectionRoomEvent", new ClientConnectionRoomEvent(ConnectionInfoType.DISCONNECT, RoomInfo, player));
                    return true;
                }
            return false;
        }

        public void SendMessageToRoom(string header, Object request)
        {
            foreach (Player player in RoomInfo.Players)
                Net.GetConnectionByClientId(player.Id)?.SendObject(header, request);
        }

        public bool IsRoomEmpty()
        {
            return (RoomInfo.Players.Count() == 0);
        }

        public Player GetPlayerById(string Id)
        {
            foreach (Player player in RoomInfo.Players)
            {
                if (player.Id.Equals(Id))
                    return player;
            }
            return null;
        }

        public bool IsAllPlayersReady()
        {
            foreach (Player player in RoomInfo.Players)
            {
                if (GameInfo.PlayersInfo[player.Id].IsReady == false)
                    return false;
            }
            return true;
        }

        public void StartGame()
        {
            GameInfo.SetStartingGameInfo();
            Distribute();
            Net.GetConnectionByClientId(RoomInfo.Players[StartingPlayer].Id)?.SendObject("YourTurnCall", new YourTurnCall());
        }

        public void Distribute()
        {
           while (GameInfo.Deck.Cards.Count > 6)
                foreach (Player player in RoomInfo.Players)
                {
                    Card DealtCard = GameInfo.Deck.DealCard();
                    GameInfo.PlayersInfo[player.Id].DealCard(DealtCard);
                    Net.GetConnectionByClientId(player.Id)?.SendObject("CardDistributed", new CardDistributed(DealtCard));
                }
            while (GameInfo.Deck.Cards.Count > 0)
                GameInfo.Dog.Add(GameInfo.Deck.DealCard());
        }

        public void GameCancelled()
        {
            SendMessageToRoom("GameCancelled", new GameCancelled());
            NextStartingPlayer();
            StartGame();
        }

        public void StartPlayerDogHandling()
        {
            GameInfo.ResetOtherPlayersCall();
            SendMessageToRoom("PlayerWinningCallInfo", new PlayerWinningCallInfo(GetPlayerById(GameInfo.GetCallingPlayerId()), GameInfo.GetWinningCall(), GameInfo.Dog));
            GameInfo.DropDog();
        }

        public string GetNextPlayerIdTurnById(string Id)
        {
            int idx = 0;
            foreach (Player player in RoomInfo.Players)
            {
                if (player.Id.Equals(Id))
                    return RoomInfo.Players[(idx + 1) % 4].Id;
                idx++;
            }
            return null;
        }

        public void HandleTurnOver()
        {
            string WinnerId = GameInfo.HandleWinningPlayerTurn();

            if (GameInfo.IsGameOver())
                HandleGameOver();
            else
            {
                SendMessageToRoom("TurnOver", new TurnOver());
                Net.GetConnectionByClientId(WinnerId)?.SendObject("YourTurnPlayCard", new YourTurnPlayCard());
            }
        }


        public void GetAWinner()
        {
            List<Player> Winners = new List<Player>();
            List<Tuple<Player, double>> Scores = new List<Tuple<Player, double>>();

            string CallerId = GameInfo.GetCallingPlayerId();
            double IsCallerWinnerPoints = GameInfo.PlayersInfo[CallerId].IsPlayerCallerWon();
            int CoeffPointsWinner = IsCallerWinnerPoints > 0 ? 1 : -1;
            if (CoeffPointsWinner > 0)
                Winners.Add(GetPlayerById(CallerId));
            IsCallerWinnerPoints = IsCallerWinnerPoints < 0 ? IsCallerWinnerPoints * (-1) : IsCallerWinnerPoints;
            foreach (KeyValuePair<string, PlayerGameInfo> entry in GameInfo.PlayersInfo)
            {
                if (CoeffPointsWinner < 0 && !entry.Key.Equals(CallerId))
                    Winners.Add(GetPlayerById(entry.Key));
                int IsCallerCoeff = CallerId.Equals(entry.Key) ? 3 * CoeffPointsWinner : CoeffPointsWinner * (-1);
                entry.Value.PartyPoints += (((25 + IsCallerWinnerPoints) * (int)entry.Value.CallType) * IsCallerCoeff);

                Scores.Add(new Tuple<Player, double>(GetPlayerById(entry.Key), entry.Value.PartyPoints));
            }
            SendMessageToRoom("GameOver", new GameOver(Winners, Scores));
        }

        public void HandleGameOver()
        {
            GameInfo.CountPointsPlayers();
            GetAWinner();
            GameInfo.SetStartingGameInfo();
            NextStartingPlayer();
            SendMessageToRoom("AreYouReady", new AreYouReady());
        }
    }
}

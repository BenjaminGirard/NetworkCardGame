using Protocol;
using Protocol.Game.Player_actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.GameCraft
{
    class GameInfo
    {
        public Deck Deck;
        public List<Card> Dog;
        public CardColor ColorPlayed;
        public Dictionary<string, PlayerGameInfo> PlayersInfo;

        public GameInfo()
        {
            Deck = new Deck();
            Dog = new List<Card>();
            PlayersInfo = new Dictionary<string, PlayerGameInfo>();
            ColorPlayed = CardColor.trump;
        }

        public void SetStartingGameInfo()
        {
            Dog.Clear();
            foreach (KeyValuePair<string, PlayerGameInfo> entry in PlayersInfo)
            {
                entry.Value.SetStartingGameInfo();
            }
            Deck.GenerateCards();
            Deck.ShuffleCards();
            ColorPlayed = CardColor.trump;
        }

        public void ResetParty()
        {
            SetStartingGameInfo();
            foreach (KeyValuePair<string, PlayerGameInfo> entry in PlayersInfo)
            {
                entry.Value.ResetPartyInfo();
            }
        }

        public Boolean IsAllPlayersSkiped()
        {
            foreach (KeyValuePair<string, PlayerGameInfo> entry in PlayersInfo)
                if (entry.Value.CallType != CallType.SKIP)
                    return false;
            return true;
        }

        public Boolean IsGameCallOver()
        {
            foreach (KeyValuePair<string, PlayerGameInfo> entry in PlayersInfo)
            {
                if (entry.Value.CallType == CallType.NONE)
                    return false;
            }
            return true;
        }

        public void ResetOtherPlayersCall()
        {
            string CallingPlayerId = GetCallingPlayerId();
            foreach (KeyValuePair<string, PlayerGameInfo> entry in PlayersInfo)
            {
                if (entry.Key.Equals(CallingPlayerId))
                    continue;
                entry.Value.CallType = CallType.SKIP;
            }
        }

        public void SetPlayerCall(string ClientId, CallType type)
        {
            Console.WriteLine("\ntype = " + type);
            PlayersInfo[ClientId].CallType = type;
        }

        public string GetCallingPlayerId()
        {
            string PlayerId = null;
            CallType BestCall = CallType.NONE;
            foreach (KeyValuePair<string, PlayerGameInfo> entry in PlayersInfo)
            {
                if (entry.Value.CallType > BestCall)
                {
                    PlayerId = entry.Key;
                    BestCall = entry.Value.CallType;
                }
            }
            return PlayerId;
        }

        public CallType GetWinningCall()
        {
            foreach (KeyValuePair<string, PlayerGameInfo> entry in PlayersInfo)
            {
                if (entry.Value.CallType != CallType.SKIP && entry.Value.CallType != CallType.NONE)
                    return entry.Value.CallType;
            }
            return CallType.NONE;
        }
        public void DropDog()
        {
            string Caller = GetCallingPlayerId();
            foreach (Card card in Dog)
                PlayersInfo[Caller].Cards.Add(card);
            Dog.Clear();
        }

        public Boolean IsTurnOver()
        {
            foreach (KeyValuePair<string, PlayerGameInfo> entry in PlayersInfo)
            {
                if (entry.Value.CardPlayedOnTable == null)
                    return false;
            }
            return true;
        }

        public Boolean IsFirstCardToPlayed()
        {
            foreach (KeyValuePair<string, PlayerGameInfo> entry in PlayersInfo)
            {
                if (entry.Value.CardPlayedOnTable != null)
                    return false;
            }
            return true;

        }

        public void DropCardsDogFromPlayerHand(List<Card> dog)
        {
            string Caller = GetCallingPlayerId();

            foreach (Card card in dog)
            {
               PlayersInfo[Caller].RemoveCardPlayed(card);
            }
        }

        public string HandleWinningPlayerTurn()
        {
            PlayerGameInfo Winner = null;
            string id = null;


            foreach (KeyValuePair<string, PlayerGameInfo> entry in PlayersInfo)
            {
                if (Winner == null || Deck.IsCardBetterThan(entry.Value.CardPlayedOnTable, Winner.CardPlayedOnTable, ColorPlayed))
                {
                    Winner = entry.Value;
                    id = entry.Key;
                }
            }
            foreach (KeyValuePair<string, PlayerGameInfo> entry in PlayersInfo)
            {
                if (entry.Value.CardPlayedOnTable.Number == 0)
                {
                    entry.Value.GamePoints -= 1;
                    entry.Value.WonCards.Add(entry.Value.CardPlayedOnTable);
                    entry.Value.CardPlayedOnTable = null;
                    Winner.GamePoints += 0.5;
                }
                else
                {
                    Winner.WonCards.Add(entry.Value.CardPlayedOnTable);
                    entry.Value.CardPlayedOnTable = null;
                }
            }
            return id;
        }

        public Boolean IsGameOver()
        {
            foreach (KeyValuePair<string, PlayerGameInfo> entry in PlayersInfo)
            {
                if (entry.Value.Cards.Count == 0)
                    return true;
            }
            return false;
        }

        public void CountPointsPlayers()
        {
            foreach (KeyValuePair<string, PlayerGameInfo> entry in PlayersInfo)
            {
                foreach (Card card in entry.Value.WonCards)
                    entry.Value.GamePoints += Deck.CardValue(card);
                entry.Value.WonCards.Clear();
                if (entry.Value.CallType != CallType.SKIP && entry.Value.CallType != CallType.GUARD_AGAINST)
                    foreach (Card card in Dog)
                        entry.Value.GamePoints += Deck.CardValue(card);
            }
        }
    }
}

using Protocol;
using Protocol.Game.Player_actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.GameCraft
{
    class PlayerGameInfo
    {
        public List<Card> Cards { get; set; }

        public Card CardPlayedOnTable { get; set; }

        public Boolean IsReady { get; set; }

        public double GamePoints { get; set; }
        public double PartyPoints { get; set; }

        public CallType CallType { get; set; }

        public List<Card> WonCards;

        public PlayerGameInfo(Boolean isReady = false)
        {
            Cards = new List<Card>();
            CardPlayedOnTable = null;
            WonCards = new List<Card>();
            IsReady = isReady;
            GamePoints = 0;
            PartyPoints = 0;
            CallType = CallType.NONE;
        }

        public void DealCard(Card card)
        {
            Cards.Add(card);
        }

        public void SetStartingGameInfo()
        {
            Cards.Clear();
            WonCards.Clear();
            CardPlayedOnTable = null;
            GamePoints = 0;
        }

        public void ResetPartyInfo()
        {
            IsReady = false;
            PartyPoints = 0;
            SetStartingGameInfo();
        }

        public void RemoveCardPlayed(Card card)
        {
            for (int i = 0; i < Cards.Count; i++)
            {
                if (Cards[i].Color == card.Color && Cards[i].Number == card.Number)
                {
                    Cards.RemoveAt(i);
                    return;
                }
            }
        }

        private int HowManyOudlers()
        {
            int Oudlers = 0;
            foreach (Card card in WonCards)
            {
                if (card.Color == CardColor.trump && (card.Number <= 1 || card.Number == 21))
                    Oudlers += 1;
            }
            return Oudlers;
        }

        public double IsPlayerCallerWon()
        {
            int NbOudlers = HowManyOudlers();
            Dictionary<int, int> PointsToWin = new Dictionary<int, int>()
            {
                {0, 56 },
                {1, 51 },
                {2, 41 },
                {3, 36 }
            };

            return GamePoints - PointsToWin[NbOudlers] ;

        }
    }
}

using Protocol;
using Protocol.Game.Player_actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client
{
    class CardRules
    {
        Card CardToPlay;
        List<Card> Hand;
        List<CardPlayed> Board;
        CardColor ColorPlayed;
        public CardRules() { }

        public Boolean IsMyCardPlayable(Card cardToPlay, List<Card> hand, List<CardPlayed> board, CardColor colorPlayed)
        {
            CardToPlay = cardToPlay;
            Hand = hand;
            Board = board;
            ColorPlayed = colorPlayed;

            if (Board.Count == 0)
                return true;
            return IsMyCardFool();
        }

        public Boolean IsMyCardFool()
        {
            if (CardToPlay.Color == CardColor.trump && CardToPlay.Number == 0)
                return true;
            return IsCardSameColorPlayed();
        }

        public Boolean IsCardSameColorPlayed()
        {
            if (CardToPlay.Color == ColorPlayed)
                return true;
            return DoIHaveSameColor();
        }

        public Boolean DoIHaveSameColor()
        {
            foreach (Card card in Hand)
            {
                if (card.Color == ColorPlayed)
                {
                    Console.WriteLine("You can't play this card right now...");
                    return false;
                }
            }
            return IsMyCardTrump();
        }

        public Boolean IsMyCardTrump()
        {
            if (CardToPlay.Color == CardColor.trump)
                return IsMyTrumpBest();
            return DoIHaveTrumpInHand();
        }

        public Boolean IsMyTrumpBest()
        {
            foreach (CardPlayed cardplayed in Board)
            {
                if (cardplayed.Card.Color == CardColor.trump && cardplayed.Card.Number > CardToPlay.Number)
                    return DoIHaveBetterTrump();
            }
            return true;
        }

        public Boolean DoIHaveBetterTrump()
        {
            foreach (Card card in Hand)
            {
                if (card.Color == CardColor.trump && card.Number > CardToPlay.Number)
                {
                    Console.WriteLine("You can't play this card right now...");
                    return false;
                }
            }
            return true;
        }

        public Boolean DoIHaveTrumpInHand()
        {
            foreach (Card card in Hand)
            {
                if (card.Color == CardColor.trump)
                {
                    Console.WriteLine("You can't play this card right now...");
                    return false;
                }
            }
            return true;

        }
    }
}

using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.GameCraft
{
    class Deck
    {
        public List<Card> Cards;

        public Deck()
        {
            Cards = new List<Card>();
            GenerateCards();
        }

        public void GenerateCards()
        {
            Cards.Clear();
            for (CardColor c = CardColor.clubs; (int)c <= (int)CardColor.spades; c++)
                for (int i = 1; i < 15; i++)
                    Cards.Add(new Card(i, c));
            for (int i = 0; i <= 21; i++)
                Cards.Add(new Card(i, CardColor.trump));
        }

        public void ShuffleCards()
        {
            Cards.OrderBy(item => new Random().Next());
        }

        public Card DealCard()
        {
            Card dealCard;
            Random rdm = new Random();
            int idx = Cards.Count > 1 ? rdm.Next() % (Cards.Count - 1) : 0;
            dealCard = Cards[idx];
            Cards.RemoveAt(idx);
            return dealCard;

        }

        public void TakeCardBack(Card cardBack)
        {
            Cards.Add(cardBack);
        }

        public Boolean IsCardBetterThan(Card c1, Card c2, CardColor ColorCalled)
        {
            if (c1 == null || c2 == null)
                return c2 == null;
            if (c1.Color == CardColor.trump && c1.Number == 0)
                return false;
            if (c2.Color == CardColor.trump && c2.Number == 0)
                return true;
            if (c1.Color == CardColor.trump && c2.Color != CardColor.trump)
                return true;
            if (c1.Color != CardColor.trump && c2.Color == CardColor.trump)
                return false;
            if (c1.Color == CardColor.trump && c2.Color == CardColor.trump)
                return c1.Number > c2.Number;
            if (c1.Color == ColorCalled && c2.Color != ColorCalled)
                return true;
            if (c1.Color != ColorCalled && c2.Color == ColorCalled)
                return false;
            if (c1.Color == ColorCalled && c2.Color == ColorCalled)
                return c1.Number > c2.Number;
            return false;
        }

        public double CardValue(Card card)
        {
            if (card.Color == CardColor.trump)
            {
                if (card.Number <= 1 || card.Number == 21)
                    return 4.5;
                return 0.5;
            }
            if (card.Number > 10)
                return (double)card.Number - 9.5;
            return 0.5;
        }
    }
}

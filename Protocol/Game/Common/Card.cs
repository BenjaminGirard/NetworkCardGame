using ProtoBuf;
using System;

namespace Protocol
{
    public enum CardColor { trump, clubs, diamonds, hearts, spades }

    [ProtoContract]
    public class Card
    {
        // trumps are from 0 to 21 - 0 being the fool
        [ProtoMember(1)]
        public int Number { get; set; }

        [ProtoMember(2)]
        public CardColor Color { get; set; }

        protected Card() { }
        public Card(int _number, CardColor _color)
        {
            Number = _number;
            Color = _color;
        }

        public bool IsOudler()
        {
            return (Color.Equals(CardColor.trump) && (Number < 2 || Number == 21));
        }

/*        public bool IsBetterThan(Card card)
        {
            if (Type.Equals(CardType.trump) && card.Type.Equals(CardType.any))
                return true;
            if (Type.Equals(CardType.any) && card.Type.Equals(CardType.trump))
                return false;
            if (Type.Equals(CardType.trump) && card.Type.Equals(CardType.trump))
                return Number > card.Number ? true : false;

            return false;
            // mettre dans le deck avec la carte color picked
        }*/ 

    }
}

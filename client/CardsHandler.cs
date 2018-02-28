using System;
using System.Collections.Generic;
using System.Linq;
using Protocol;
using Protocol.Game.Player_actions;

namespace client
{
    public class CardsHandler
    {
        private CardRules rules;
        private List<Card> _hand = new List<Card>();
        private List<CardPlayed> _board = new List<CardPlayed>();
        private List<Card> _dog = new List<Card>();
        private Dictionary<int, string> _specificNames = new Dictionary<int, string>();
        private Dictionary<string, int> _specificIds = new Dictionary<string, int>();

        public CardsHandler()
        {
            rules = new CardRules();
            _specificIds["jack"] = 11;
            _specificIds["knight"] = 12;
            _specificIds["queen"] = 13;
            _specificIds["king"] = 14;
            _specificIds["trump"] = (int) CardColor.trump;
            _specificIds["clubs"] = (int) CardColor.clubs;
            _specificIds["diamonds"] = (int) CardColor.diamonds;
            _specificIds["spades"] = (int) CardColor.spades;
            _specificIds["hearts"] = (int) CardColor.hearts;
            _specificIds["club"] = (int) CardColor.clubs;
            _specificIds["diamond"] = (int) CardColor.diamonds;
            _specificIds["spade"] = (int) CardColor.spades;
            _specificIds["heart"] = (int) CardColor.hearts;
            _specificNames[11] = "Jack";
            _specificNames[12] = "Knight";
            _specificNames[13] = "Queen";
            _specificNames[14] = "King";
            _specificNames[(int) CardColor.trump + 40] = "Trump";
            _specificNames[(int) CardColor.clubs + 40] = "Clubs";
            _specificNames[(int) CardColor.diamonds + 40] = "Diamonds";
            _specificNames[(int) CardColor.spades + 40] = "Spades";
            _specificNames[(int) CardColor.hearts + 40] = "Hearts";
        }

        public bool IsValidCard(string cardName, string cardColor)
        {
            if (!_specificIds.ContainsKey(cardColor.ToLower()))
                return false;
            var cardValue = 0;
            if (!_specificIds.ContainsKey(cardName.ToLower()) && !int.TryParse(cardName, out cardValue))
                return false;
            if (_specificIds[cardColor.ToLower()] != (int) CardColor.trump)
            {
                if (!_specificIds.ContainsKey(cardName.ToLower()))
                    if (cardValue < 1 || cardValue > 10)
                        return false;                
            }
            else if (cardValue < 0 || cardValue > 21)
                return false;
            return true;
        }

        public string CardToReadableString(Card card)
        {
            string cardName;
            if (card.Color == CardColor.trump)
                cardName = card.Number == 0 ? "Fool" : "Trump " + card.Number;
            else
                cardName = _specificNames.ContainsKey(card.Number)
                    ? _specificNames[card.Number] + " of " + _specificNames[(int) (card.Color + 40)]
                    : card.Number + " of " + _specificNames[(int) (card.Color + 40)];
            return cardName;
        }

        public Card GetCard(string cardName, string cardColor)
        {
            var number = !_specificIds.ContainsKey(cardName.ToLower())
                ? int.Parse(cardName)
                : _specificIds[cardName.ToLower()];
            var color = (CardColor) _specificIds[cardColor.ToLower()];
            return new Card(number, color);
        }

        public void AddCardToHand(Card card)
        {
            _hand.Add(card);
        }

        public void RemoveFromHand(Card card)
        {
            foreach (var tmp in _hand)
                if (tmp.Color == card.Color && tmp.Number == card.Number)
                {
                    _hand.Remove(tmp);
                    break;
                }
        }

        public void AddCardToBoard(CardPlayed card)
        {
            _board.Add(card);
        }

        public void ClearBoard()
        {
            _board.Clear();
        }

        public void ClearHand()
        {
            _hand.Clear();
        }

        public void PrintBoard()
        {
            if (!_board.Any())
            {
                Console.WriteLine("The board is empty");
                return;
            }
            Console.WriteLine("Current cards played on the board (by played order) :");
            foreach (var card in _board)
                Console.WriteLine("\t" + card.Player.Username + " played " + CardToReadableString(card.Card));
        }

        public void PrintHand()
        {
            if (!_hand.Any())
            {
                Console.WriteLine("You got an empty hand");
                return;
            }
            Console.WriteLine("You got the following hand :");
            foreach (var card in _hand)
                Console.WriteLine("\t" + CardToReadableString(card));
        }

        public bool IsCardInHand(Card cardPlayed)
        {
            return _hand.Any(card => card.Color == cardPlayed.Color && card.Number == cardPlayed.Number);
        }

        private CardColor WhatColorPlayed()
        {
            if (_board.Count == 0)
                return CardColor.trump;
            return _board[0].Card.Color;
        }

        public bool IsCardPlayable(Card card)
        {
            if (!IsCardInHand(card))
            {
                Console.WriteLine("This card is not in your hand");
                return false;
            }
            return (rules.IsMyCardPlayable(card, _hand, _board, WhatColorPlayed()));
        }

        public bool IsDogComplete()
        {
            return _dog.Count == 6;
        }

        public void ClearDog()
        {
            _dog.Clear();
        }

        public void AddDogToHand(List<Card> dog)
        {
            foreach (var card in dog)
                _hand.Add(card);
        }
        
        public void AddDogCard(Card card)
        {
            if (_dog.Count < 6)
                _dog.Add(card);
        }

        public List<Card> GetDog()
        {
            return _dog;
        }

        public void SortHand()
        {
            _hand = new List<Card>(_hand.OrderBy(player => player.Number));
            _hand = new List<Card>(_hand.OrderBy(player => player.Color));
        }
    }
}
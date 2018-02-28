using ProtoBuf;
using Protocol.ConnectionRequests.CommonRessources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Game.Player_actions
{
    [ProtoContract]
    public class CardPlayed
    {
        [ProtoMember(1)]
        public Player Player { get; set; }

        [ProtoMember(2)]
        public Card Card { get; set; }

        protected CardPlayed() { }

        public CardPlayed(Player player, Card card)
        {
            Player = player;
            Card = card;
        }
    }
}

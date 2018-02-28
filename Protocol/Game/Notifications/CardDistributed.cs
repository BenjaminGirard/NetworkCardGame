using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Game.Notifications
{
    [ProtoContract]
    public class CardDistributed
    {
        [ProtoMember(1)]
        public Card Card { get; set; }

        protected CardDistributed() { }

        public CardDistributed(Card card)
        {
            Card = card;
        }
    }
}

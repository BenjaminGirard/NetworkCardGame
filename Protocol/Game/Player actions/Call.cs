using ProtoBuf;
using Protocol.ConnectionRequests.CommonRessources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Game.Player_actions
{
    public enum CallType { SKIP, NONE, SMALL, GUARD, GUARD_WITHOUT, GUARD_AGAINST }

    [ProtoContract]
    public class Call
    {
        [ProtoMember(1)]
        public Player Player { get; set; }

        [ProtoMember(2)]
        public CallType Type { get; set; }

        protected Call() { }

        public Call(Player player, CallType type)
        {
            Player = player;
            Type = type;
        }
    }
}

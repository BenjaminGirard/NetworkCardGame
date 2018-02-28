using ProtoBuf;
using Protocol.ConnectionRequests.CommonRessources;
using Protocol.Game.Player_actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Game.Notifications
{
    [ProtoContract]
    public class PlayerWinningCallInfo
    {
        [ProtoMember(1)]
        public Player Player { get; set; }

        [ProtoMember(2)]
        public CallType CallType { get; set; }

        [ProtoMember(3)]
        public List<Card> Dog { get; set; }
        protected PlayerWinningCallInfo() { }

        public PlayerWinningCallInfo(Player player, CallType calltype, List<Card> dog)
        {
            Player = player;
            CallType = calltype;
            Dog = dog;
        }
    }
}

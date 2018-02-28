using ProtoBuf;
using Protocol.ConnectionRequests.CommonRessources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Game.Notifications
{
    [ProtoContract]
    public class GameOver
    {
        [ProtoMember(1)]
        public List<Player> Winners { get; set; }

        [ProtoMember(2)]
        public List<Tuple<Player, double>> Scores { get; set; }

        protected GameOver() { }

        public GameOver(List<Player> winners, List<Tuple<Player, double>> scores)
        {
            Winners = winners;
            Scores = scores;
        }
    }
}

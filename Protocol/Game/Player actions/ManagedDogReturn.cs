using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Game.Player_actions
{
    [ProtoContract]
    public class ManagedDogReturn
    {
        [ProtoMember(1)]
        public List<Card> Dog { get; set; }

        protected ManagedDogReturn() { }

        public ManagedDogReturn(List<Card> dog)
        {
            Dog = dog;
        }
    }
}

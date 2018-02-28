using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.ConnectionRequests.ClientConnection
{
    [ProtoContract]
    public class UsernameAuthAnswer
    {
        [ProtoMember(1)]
        public Boolean IsAuthenticationValid { get; set; }

        protected UsernameAuthAnswer() { }
        public UsernameAuthAnswer(Boolean _isAuth) { IsAuthenticationValid = _isAuth; }
    }
}

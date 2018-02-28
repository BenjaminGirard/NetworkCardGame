using Protocol.ConnectionRequests.CommonRessources;

namespace client
{
    public class User
    {
        public Player PlayerInfo { get; set; }
        public string Username { get; set; }
        public bool IsAuthenticated { get; set; } = false;
        public RoomInfo RoomInfo { get; set; }
        public RoomsHandler RoomsHandler { get; set; }
        public CardsHandler CardsHandler { get; } = new CardsHandler();
        public CallHandler CallHandler { get; } = new CallHandler();
        public bool MyTurnToCall { get; set; } = false;
        public bool MyTurnToPlay { get; set; } = false;
        public bool HasWinCall { get; set; } = false;
    }
}
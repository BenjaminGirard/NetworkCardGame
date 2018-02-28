using Protocol.ConnectionRequests.CommonRessources;
using sever;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class RoomsEntitiesManager
    {
        public NetworkManager Net;
        public List<RoomDealer> Rooms;

        private static int RoomIdentifier = 0;

        public RoomsEntitiesManager(NetworkManager net)
        {
            Net = net;
            Rooms = new List<RoomDealer>();
        }

        public RoomDealer CreateRoom(ClientInfo client)
        {
            RoomDealer RoomCreated = new RoomDealer(Net, client, RoomIdentifier);
            RoomIdentifier += 1;
            Rooms.Add(RoomCreated);
            return RoomCreated;
        }

        public void DeleteRoom(RoomDealer room)
        {
            Rooms.Remove(room);
        }

        public void TryRemoveClient(ClientInfo client)
        {
            foreach (RoomDealer room in Rooms)
                if (room.TryRemovePlayer(client))
                {
                    if (room.IsRoomEmpty())
                        DeleteRoom(room);
                    return;
                }
        }

        public List<RoomInfo> GetRoomInfoList()
        {
            List<RoomInfo> RoomInfo = new List<RoomInfo>();

            foreach (RoomDealer dealer in Rooms)
                RoomInfo.Add(dealer.RoomInfo);
            return RoomInfo;
        }

        public RoomDealer GetRoomFromRoomId(int Id)
        {
            foreach (RoomDealer dealer in Rooms)
            {
                if (Id == dealer.RoomInfo.Id)
                    return dealer;
            }
            return null;
        }

        public RoomDealer FindRoomFromClientId(string id)
        {
            foreach (RoomDealer dealer in Rooms)
            {
                foreach (Player player in dealer.RoomInfo.Players)
                    if (player.Id.Equals(id))
                        return dealer;
            }
            return null;
        }

        public Player FindClientByClientId(string id)
        {
            foreach (RoomDealer dealer in Rooms)
            {
                foreach (Player player in dealer.RoomInfo.Players)
                    if (player.Id.Equals(id))
                        return player;
            }
            return null;
        }
    }
}

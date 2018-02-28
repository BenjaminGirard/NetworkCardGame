using System;
using System.Collections.Generic;
using System.Linq;
using Protocol.ConnectionRequests.CommonRessources;

namespace client
{
    public class RoomsHandler
    {
        private List<RoomInfo> Rooms { get; set; }

        public RoomsHandler(List<RoomInfo> rooms)
        {
            Rooms = rooms ?? new List<RoomInfo>();
        }

        public void ListRoom()
        {
            if (Rooms.Any())
            {
                Console.WriteLine("Available rooms :");
                foreach (var room in Rooms)
                    Console.WriteLine("\t- Room number " + room.Id + " (" + room.Players.Count + "/4)");
            }
            else
                Console.WriteLine("No room currently available");
        }

        public void CreateRoom(RoomInfo room)
        {
            if (room.Players == null)
                room.Players = new List<Player>();
            Rooms.Add(room);
        }

        public void AddPlayerToRoom(Player player, RoomInfo roomInfo)
        {
            foreach (var room in Rooms)
                if (roomInfo.Id == room.Id)
                {
                    room.Players.Add(player);
                    break;
                }
        }

        private void RemoveRoomPlayerById(RoomInfo room, string id)
        {
            foreach (var player in room.Players)
                if (player.Id.Equals(id))
                {
                    room.Players.Remove(player);
                    break;
                }
        }

        public void RemovePlayerToRoom(Player player, RoomInfo roomInfo)
        {
            foreach (var room in Rooms)
                if (roomInfo.Id == room.Id)
                {
                    RemoveRoomPlayerById(room, player.Id);
                    if (!room.Players.Any())
                        Rooms.Remove(room);
                    break;
                }
        }
    }
}
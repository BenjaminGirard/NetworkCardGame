using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using InTheHand.Net.Bluetooth;
using NetworkCommsDotNet;
using Protocol;
using Protocol.ConnectionRequests.RoomConnection;
using Protocol.Game.Notifications;
using Protocol.Game.Player_actions;

namespace client
{
    public class CommandHandler
    {
        private Dictionary<string, Command> _commands = new Dictionary<string, Command>();
        private User _user;
        public NetworkManager NetworkManager { private get; set; }

        delegate bool Command(string userInput);

        public CommandHandler(User user)
        {
            _user = user;
            _commands["help"] = HelpCommand;
            _commands["quit"] = QuitCommand;
            _commands["info_room"] = InfoRoomCommand;
            _commands["leave_room"] = LeaveRoomCommand;
            _commands["list_room"] = ListRoomCommmand;
            _commands["create_room"] = CreateRoomCommand;
            _commands["join_room"] = JoinRoomCommand;
            _commands["hand"] = ShowHandCommand;
            _commands["board"] = ShowBoardCommand;
            _commands["calls"] = ShowCallsCommand;
            _commands["ready"] = ReadyCommand;
            _commands["make_call"] = CallCommand;
            _commands["play_card"] = PlayCardCommand;
            _commands["add_dog"] = AddDogCommand;
            _commands["sort_hand"] = SortHandCommand;
        }

        public bool HandleCommand(string userInput)
        {
            if (_commands.ContainsKey(userInput.Split(' ').First().ToLower()))
                return _commands[userInput.Split(' ').First().ToLower()](userInput);
            Console.WriteLine("Unknown command : " + userInput.Split(' ').First());
            Console.WriteLine("Type \"help\"  to see availables commands");
            return (true);
        }

        private bool HelpCommand(string userInput)
        {
            Console.WriteLine("Available commands ([] is for command parameters):");
            Console.WriteLine("\t- help : display help");
            Console.WriteLine("\t- quit : quit");
            Console.WriteLine("\t- info_room : show current room info");
            Console.WriteLine("\t- leave_room : leave current room");
            Console.WriteLine("\t- list_room : show all current rooms");
            Console.WriteLine("\t- create_room : create a new room");
            Console.WriteLine("\t- join_room [room_id] : create a new room");
            Console.WriteLine("\t- hand : show your current hand");
            Console.WriteLine("\t- board : show the current board");
            Console.WriteLine("\t- calls : list available calls");
            Console.WriteLine("\t- ready : notify your room that you are ready");
            Console.WriteLine("\t- make_call [call] : make a call");
            Console.WriteLine("\t- play_card [card_name] [card_color] : play a card");
            Console.WriteLine("\t- add_dog [card_name] [card_color] : add a cart to your dog");
            Console.WriteLine("\t- sort_hand : sort your hand");
            return true;
        }

        private bool QuitCommand(string userInput)
        {
            return false;
        }

        private bool InfoRoomCommand(string userInput)
        {
            if (_user.RoomInfo == null)
            {
                Console.WriteLine("You are not in a room");
                return true;
            }
            Console.Write("You are in room number " + _user.RoomInfo.Id);
            if (_user.RoomInfo.Players.Any())
            {
                Console.WriteLine(" with the following players :");
                foreach (var player in _user.RoomInfo.Players)
                    Console.WriteLine("\t- " + player.Username);
            }
            else
                Console.WriteLine(", you are the only player in the room");
            return true;
        }

        private bool LeaveRoomCommand(string userInput)
        {
            if (_user.RoomInfo == null)
            {
                Console.Write("You are not in a room");
                return true;
            }
            NetworkManager.Server.SendObject("LeaveRoom", new LeaveRoom());
            Console.WriteLine("Leaving room " + _user.RoomInfo.Id);
            _user.RoomInfo = null;
            return true;
        }

        private bool ListRoomCommmand(string userInput)
        {
            _user.RoomsHandler.ListRoom();
            return true;
        }

        private bool CreateRoomCommand(string userInput)
        {
            if (_user.RoomInfo == null)
                NetworkManager.Server.SendObject("AskCreateRoom", new AskCreateRoom());
            else
                Console.WriteLine("You can't create a room while already being in one");
            return true;
        }

        private bool JoinRoomCommand(string userInput)
        {
            if (_user.RoomInfo != null)
            {
                Console.WriteLine("You can't join a room while already being in a one");
                return true;
            }
            var words = userInput.Split(' ');
            if (words.Length < 2 || !int.TryParse(words[1], out var roomId))
            {
                Console.WriteLine("Invalid or missing room id");
                return true;
            }
            NetworkManager.Server.SendObject("AskMovingIntoRoom", new AskMovingIntoRoom(roomId));
            return true;
        }

        private bool ShowHandCommand(string userInput)
        {
            _user.CardsHandler.PrintHand();
            return true;
        }

        private bool ShowBoardCommand(string userInput)
        {
            _user.CardsHandler.PrintBoard();
            return true;
        }

        private bool ShowCallsCommand(string userInput)
        {
            Console.WriteLine("Available calls : Small, Guard, Guard_without, Guard_against, Pass");
            return true;
        }

        private bool ReadyCommand(string userInput)
        {
            if (_user.RoomInfo != null)
                NetworkManager.Server.SendObject("IAmReady", new IAmReady());
            return true;
        }

        private bool CallCommand(string userInput)
        {
            if (!_user.MyTurnToCall)
            {
                Console.WriteLine("It's not your turn to make a call");
                return true;
            }
            var words = userInput.Split(' ');
            if (words.Length < 2 || _user.CallHandler.GetCallEnum(words[1]) == CallType.NONE)
            {
                Console.WriteLine("Invalid or missing call");
                return true;
            }
            NetworkManager.Server.SendObject("Call",
                new Call(_user.PlayerInfo, _user.CallHandler.GetCallEnum(words[1])));
            _user.MyTurnToCall = false;
            return true;
        }

        private bool PlayCardCommand(string userInput)
        {
            if (!_user.MyTurnToPlay)
            {
                Console.WriteLine("It's not your turn to play a card");
                return true;
            }
            var words = userInput.Split(' ');
            if (words.Length < 3 || !_user.CardsHandler.IsValidCard(words[1], words[2]))
            {
                Console.WriteLine("Invalid or missing card specifiers");
                return true;
            }
            Card cardPlayed = _user.CardsHandler.GetCard(words[1], words[2]);
            if (!_user.CardsHandler.IsCardPlayable(cardPlayed))
                return true;
            _user.CardsHandler.RemoveFromHand(cardPlayed);
            NetworkManager.Server.SendObject("CardPlayed", new CardPlayed(_user.PlayerInfo, cardPlayed));
            _user.MyTurnToPlay = false;
            return true;
        }

        private bool AddDogCommand(string userInput)
        {
            if (!_user.HasWinCall)
            {
                Console.WriteLine("You didn't take");
                return true;
            }
            var words = userInput.Split(' ');
            if (words.Length < 3 || !_user.CardsHandler.IsValidCard(words[1], words[2]))
            {
                Console.WriteLine("Invalid or missing card specifiers");
                return true;
            }
            Card cardPlayed = _user.CardsHandler.GetCard(words[1], words[2]);
            if (!_user.CardsHandler.IsCardInHand(cardPlayed) || cardPlayed.IsOudler())
            {
                Console.WriteLine("This card is not in your hand or you tried to put an oudler in the dog");
                return true;                
            }
            _user.CardsHandler.RemoveFromHand(cardPlayed);
            _user.CardsHandler.AddDogCard(cardPlayed);
            if (!_user.CardsHandler.IsDogComplete()) return true;
            _user.HasWinCall = false;
            NetworkManager.Server.SendObject("ManagedDogReturn", new ManagedDogReturn(_user.CardsHandler.GetDog()));
            return true;
        }

        private bool SortHandCommand(string userInput)
        {
            _user.CardsHandler.SortHand();
            return true;
        }
    }
}
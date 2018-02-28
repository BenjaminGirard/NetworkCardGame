using System;
using System.Linq;

namespace client
{
    public class Core
    {
        private bool _quit;
        private string _userInput;
        private User _user = new User();
        private NetworkManager _networkManager;
        private CommandHandler _commandHandler;

        public Core()
        {
            _commandHandler = new CommandHandler(_user);
        }

        private void AskUsername()
        {
            Console.WriteLine("Please enter your username:");
            lock (_user)
                while (String.IsNullOrWhiteSpace(_user.Username = Console.ReadLine()))
                    Console.WriteLine("Please enter valid username");
        }

        private void AskServerAddress()
        {
            Console.WriteLine("Please enter the server IP and port in the format 192.168.0.1:10000 and press return:");
            string serverInfo = Console.ReadLine();
            if (String.IsNullOrWhiteSpace(serverInfo))
            {
                _quit = true;
                Console.WriteLine("Empty server address");
            }
            else
            {
                try
                {
                    _networkManager = new NetworkManager(serverInfo.Split(':').First(),
                        int.Parse(serverInfo.Split(':').Last()), _user);
                    _commandHandler.NetworkManager = _networkManager;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    _quit = true;
                }
            }
        }

        public void StartClient()
        {
            AskUsername();
            AskServerAddress();
            while (!_quit)
            {
                lock (_user)
                    Console.Write("Tarot [" + _user.Username + "] > ");
                _userInput = Console.ReadLine();
                if (String.IsNullOrWhiteSpace(_userInput)) continue;
                lock (_user)
                    if (_user.IsAuthenticated)
                    {
                        if (!_commandHandler.HandleCommand(_userInput))
                            _quit = true;
                    }
                    else
                        Console.WriteLine("You are not authentified by the server yet, please wait");
            }
        }
    }
}
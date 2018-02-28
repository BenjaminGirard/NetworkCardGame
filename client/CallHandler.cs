using System;
using System.Collections.Generic;
using System.Linq;
using Protocol.Game.Player_actions;

namespace client
{
    public class CallHandler
    {
        private Dictionary<string, CallType> _callNames = new Dictionary<string, CallType>();
        private Dictionary<CallType, string> _callTypes = new Dictionary<CallType, string>();
        private List<Call> _calls = new List<Call>();

        public CallHandler()
        {
            _callNames["small"] = CallType.SMALL;
            _callNames["guard"] = CallType.GUARD;
            _callNames["guard_without"] = CallType.GUARD_WITHOUT;
            _callNames["guard_against"] = CallType.GUARD_AGAINST;
            _callNames["pass"] = CallType.SKIP;
            _callTypes[CallType.SMALL] = "small";
            _callTypes[CallType.GUARD] = "guard";
            _callTypes[CallType.GUARD_WITHOUT] = "guard_without";
            _callTypes[CallType.GUARD_AGAINST] = "guard_against";
            _callTypes[CallType.SKIP] = "pass";
        }

        public void Clear()
        {
            _calls.Clear();
        }

        public void AddCall(Call call)
        {
            _calls.Add(call);
        }

        public CallType GetCallEnum(string call)
        {
            return _callNames.ContainsKey(call.ToLower()) ? _callNames[call.ToLower()] : CallType.NONE;
        }

        public string GetCallString(CallType callType)
        {
            return _callTypes[callType];
        }

        public void PrintCall()
        {
            if (!_calls.Any())
                Console.WriteLine("You are the first one to make a call");
            else
            {
                Console.WriteLine("The following calls have been made :");
                foreach (var call in _calls)
                {
                    Console.Write("\tPlayer " + call.Player.Username + " : ");
                    if (call.Type != CallType.NONE)
                        Console.WriteLine(_callTypes[call.Type]);
                }
            }
        }

        public void PrintCall(Call call)
        {
            if (call.Type != CallType.NONE)
                Console.WriteLine("Player " + call.Player.Username + " made " + _callTypes[call.Type] + " call");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC
{
    public static class Statics
    {
        public static Dictionary<string, FiniteStateMachine> MachinesNames;
        public static List<State> AllStates;
        public static List<string> keys;
        public static int StateNumber;
        public static bool Any;
        static Statics()
        {
            Any = false;
            StateNumber = 0;
            AllStates = new List<State>();
            MachinesNames = new Dictionary<string, FiniteStateMachine>();
            keys = new List<string>();
        }
        public static List<List<Token>> Split(this List<Token> l, string key)
        {
            List<List<Token>> Ret = new List<List<Token>>();
            int index = 0, holder = 0, Stack = 0;
            foreach (var item in l)
            {
                if (item.Type == TokenType.paropen) Stack++;
                if (item.Type == TokenType.parclose) Stack--;
                if (item.Value == key && Stack == 0)
                {
                    Ret.Add(l.GetRange(holder, index - holder));
                    holder = index + 1;
                }
                index++;
            }
            Ret.Add(l.GetRange(holder, index - holder));
            return Ret;
        }
        public static List<Token> GetRng(this List<Token> t, int StartIndex, int LastIndex)
        {
            return t.GetRange(StartIndex, (LastIndex - StartIndex) + 1);
        }
    }
}

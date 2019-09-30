using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace CC
{
    public class Engine
    {
        public List<FiniteStateMachine> FSMlist { get; set; }
        public Engine()
        {
            FSMlist = new List<FiniteStateMachine>();
        }
        public void Create(string s)
        {
            var f = s.Split(':');
            f[0] = f[0].Trim();
            f[1] = f[1].Remove(0, 1);
            FSMlist.Add(new FiniteStateMachine(f[0], f[1]));
        }
        public ObservableCollection<CC.CompilerToken> Tokenizer(string s)
        {
            ObservableCollection<CompilerToken> obs = new ObservableCollection<CompilerToken>();
            if (s == string.Empty) return obs;
            var v = CompilerToken.Tokenizer(s);
            foreach (var item in v)
            {
                if (item != string.Empty && item != "\n" && item != " " && item != "\r")
                {
                    obs.Add(new CompilerToken(item, FSMlist));
                }
            }
            return obs;
        }
    }
    public class View
    {
        public string Name { get; set; }
        public string RegEx { get; set; }
        public string States;
    }

}

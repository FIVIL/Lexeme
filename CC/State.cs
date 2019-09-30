using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC
{
    public class State
    {
        public int Name { get; set; }
        public List<State> FSMStates
        {
            get => allState;
        }
        public bool IsFinal { get; set; }
        private List<State> allState;
        private List<State> epsilon;
        private Dictionary<string, State> nexts;
        private List<State> Parrents;
        private State Any;
        public List<string> keys;
        public bool AnyEvaluate;
        public State()
        {
            AnyEvaluate = false;
            Name = Statics.StateNumber++;
            IsFinal = false;
            allState = new List<State>();
            epsilon = new List<State>();
            nexts = new Dictionary<string, State>();
            Parrents = new List<State>();
            Any = null;
            Statics.AllStates.Add(this);
            keys = new List<string>();
        }
        public void AddState(string key, State Value)
        {
            nexts.Add(key, Value);
            allState.Add(Value);
            Value.AddParent(this);
            //AddKey(key, Name);
            //Statics.AllStates.Add(Value);
            Statics.keys.Add(key);
        }
        public void AddEpsilon(State ep)
        {
            epsilon.Add(ep);
            allState.Add(ep);
            ep.AddParent(this);
            //Statics.AllStates.Add(ep);
        }
        private void AddParent(State s)
        {
            Parrents.Add(s);
        }
        public void AddAny(State s)
        {
            Any = s;
            allState.Add(s);
            s.AddParent(this);
            s.AddEpsilon(this);
            Statics.Any = true;
            //SetAny(Name);
            //Statics.AllStates.Add(s);
        }
        private void AddKey(string key, int caller)
        {
            if (Name == caller) return;
            foreach (var item in Parrents)
            {
                if (item.epsilon.Any(x => x.Name == Name))
                {
                    item.keys.Add(key);
                    item.AddKey(key, caller);
                }
            }
        }
        private void SetAny(int caller)
        {
            if (Name == caller) return;
            foreach (var item in Parrents)
            {
                if (item.epsilon.Any(x => x.Name == Name))
                {
                    item.AnyEvaluate = true;
                    item.SetAny(caller);
                }
            }
        }
        public State GoNext(string key)
        {
            if (!keys.Contains(key) && !AnyEvaluate) return null;
            if (Any != null) return Any;
            else
            {
                if (nexts.Keys.Contains(key)) return nexts[key];
                else
                {
                    if (epsilon.Count > 0)
                    {
                        State temp = null;
                        foreach (var item in epsilon)
                        {
                            temp = item.NextEpsilon(key, this);
                            if (temp != null) break;
                        }
                        return temp;
                    }
                    else return null;
                }
            }
        }
        private State NextEpsilon(string key, State Caller)
        {
            if (Caller.Name == Name) return null;
            else
            {
                if (Any != null) return Any;
                else
                {
                    if (nexts.Keys.Contains(key)) return nexts[key];
                    else
                    {
                        if (epsilon.Count > 0)
                        {
                            State temp = null;
                            foreach (var item in epsilon)
                            {
                                temp = item.NextEpsilon(key, Caller);
                                if (temp != null) break;
                            }
                            return temp;
                        }
                        else return null;
                    }
                }
            }
        }
        public bool cheked = false;
        public bool CheckForFinal()
        {
            if (IsFinal) return IsFinal;
            if (cheked) return false;
            cheked = true;
            bool b = false;
            foreach (var item in epsilon)
            {
                if (item.IsFinal) return true;
                b = item.CheckForFinal();
            }
            return b;
        }
        bool printed = false;
        public override string ToString()
        {
            if (printed) return "";
            string s=string.Empty;
            printed = true;
            s+=("S" + Name + ":\n");
            foreach (var item in nexts)
            {
                s+=(item.Key.ToString().PadLeft(5) + "   ");
            }
            s+=("\n");
            foreach (var item in nexts)
            {
                s+=(item.Value.Name.ToString().PadLeft(5) + "   ");
            }
            s+=("\n");
            s+=("Ep:\n");
            foreach (var item in epsilon)
            {
                s+=(item.Name.ToString().PadLeft(5) + "   ");
            }
            s+=("\n");
            if (Any != null)
            {
                s+=("Any:\n");
                s+=(Any.Name);
                s+=("\n");
            }
            s+=("________________________________________________________\n");
            return s;
        }
    }
}

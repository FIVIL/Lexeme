using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC
{
    public class FiniteStateMachine
    {
        public string Name { get; set; }
        public string RegEx { get; set; }
        public State StartState { get; set; }
        public State FinalState { get; set; }
        private List<Token> TokenList;
        private List<State> AllState;
        public FiniteStateMachine(string name, string regex)
        {
            Statics.StateNumber = 0;
            Name = name;
            for (int i = 0; i < regex.Length; i++)
            {
                if (regex[i] != '\\') RegEx += regex[i].ToString();
                else
                {
                    bool Flag = false;
                    for (int j = i + 1; j < regex.Length + 1; j++)
                    {
                        if (Statics.MachinesNames.Keys.Contains(regex.Substring(i + 1, j - (i + 1))))
                        {
                            Flag = true;
                            RegEx += ("(" + Statics.MachinesNames[regex.Substring(i + 1, j - (i + 1))].RegEx + ")");
                            i = j - 1;
                            break;
                        }
                    }
                    if (!Flag) RegEx += regex[i].ToString();
                }
            }
            TokenList = Token.Tokenizer("(" + RegEx + ")");
            Create();
            FinalState.IsFinal = true;
            AllState = Statics.AllStates;
            Statics.AllStates = new List<State>();
            foreach (var item in AllState)
            {
                item.keys = Statics.keys;
                item.AnyEvaluate = Statics.Any;
            }
            Statics.keys = new List<string>();
            Statics.Any = false;
        }
        public override string ToString()
        {
            string s = string.Empty;
            foreach (var item in AllState.OrderBy(x => x.Name))
            {
                s += item.ToString();
            }
            return s;
        }
        private void Create()
        {
            StartState = new State();
            FinalState = Par(TokenList, StartState);
            Statics.MachinesNames.Add(Name, this);
        }
        private State Normal(Token t, State head)
        {
            State End = new State();
            if (t.Value[0] == '[' && t.Escaped == false)
            {
                State cur;
                for (int j = t.Value[1]; j <= t.Value[3]; j++)
                {
                    cur = new State();
                    head.AddState(((char)j).ToString(), cur);
                    cur.AddEpsilon(End);
                }
                return End;
            }
            switch (t.Type)
            {
                case TokenType.any:
                    head.AddAny(End);
                    return End;
                case TokenType.normal:
                    if (t.Clouser == TokenClouser.plus)
                    {
                        head.AddState(t.Value, End);
                        End.AddEpsilon(head);
                    }
                    else if (t.Clouser == TokenClouser.star)
                    {
                        head.AddState(t.Value, End);
                        End.AddEpsilon(head);
                        head.AddEpsilon(End);
                    }
                    else head.AddState(t.Value, End);
                    return End;
                default:
                    return null;
            }
        }
        private State Par(List<Token> t, State head)
        {
            if (t.Last().Clouser == TokenClouser.star) return ParS(t.GetRng(1, t.Count - 2), head);
            else if (t.Last().Clouser == TokenClouser.plus) return ParP(t.GetRng(1, t.Count - 2), head);
            else return ParN(t.GetRng(1, t.Count - 2), head);
        }
        private State ParS(List<Token> t, State head)
        {
            State End = ParN(t, head);
            head.AddEpsilon(End);
            End.AddEpsilon(head);
            return End;
        }
        private State ParP(List<Token> t, State head)
        {
            State End = ParN(t, head);
            End.AddEpsilon(head);
            return End;
        }
        private State ParN(List<Token> t, State s)
        {
            int Stack = 0;
            foreach (var item in t)
            {
                if (item.Type == TokenType.paropen) Stack++;
                if (item.Type == TokenType.parclose) Stack--;
                if (item.Type == TokenType.or && Stack == 0)
                {
                    return Or(t, s);
                }
            }
            State cur = s;
            for (int i = 0; i < t.Count; i++)
            {
                if (t[i].Type == TokenType.paropen)
                {
                    int Stackii = 0;
                    for (int j = i; j < t.Count; j++)
                    {
                        if (t[j].Type == TokenType.paropen) Stackii++;
                        if (t[j].Type == TokenType.parclose) Stackii--;
                        if (t[j].Type == TokenType.parclose && Stackii == 0)
                        {
                            cur = Par(t.GetRng(i, j), cur);
                            i = j;
                            break;
                        }
                    }
                }
                else
                {
                    cur = Normal(t[i], cur);
                }
            }
            return cur;
        }
        private State Or(List<Token> t, State head)
        {
            var res = t.Split("|");
            State End = new State();
            foreach (var item in res)
            {
                State temp = new State();
                head.AddEpsilon(temp);
                ParN(item, temp).AddEpsilon(End);
            }
            return End;
        }
        private void Uncheck()
        {
            foreach (var item in AllState)
            {
                item.cheked = false;
            }
        }
        string Evaluated = string.Empty;
        public bool Evaluate(string s)
        {
            Evaluated = string.Empty;
            State cur = StartState;
            int StartIndex = 0, length = 1;
            while (true)
            {
                if (StartIndex + length > s.Length) return false;
                string LookUp = s.Substring(StartIndex, length);
                if (LookUp == "+" && s.Length > StartIndex + length
                    && s[StartIndex + length] == '+')
                {
                    LookUp = "++";
                    length++;
                }
                if (LookUp == "-" && s.Length > StartIndex + length
                   && s[StartIndex + length] == '-')
                {
                    LookUp = "--";
                    length++;
                }
                if (LookUp == ">" && s.Length > StartIndex + length
                   && s[StartIndex + length] == '=')
                {
                    LookUp = ">=";
                    length++;
                }
                if (LookUp == "<" && s.Length > StartIndex + length
                   && s[StartIndex + length] == '=')
                {
                    LookUp = "<=";
                    length++;
                }
                if (cur.GoNext(LookUp) != null)
                {
                    cur = cur.GoNext(LookUp);
                    if (StartIndex + length >= s.Length && cur.CheckForFinal()) return true;
                    Uncheck();
                    StartIndex += length;
                    length = 1;
                    Evaluated += LookUp;
                }
                else
                {
                    length++;
                }

            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CC.Statics;
namespace CC
{
    public enum TokenType
    {
        paropen,
        parclose,
        or,
        any,
        normal
    }
    public enum TokenClouser
    {
        plus,
        star,
        none
    }
    public class Token
    {
        public string Value { get; set; }
        public bool Escaped { get; set; }
        public TokenType Type { get; set; }
        public TokenClouser Clouser { get; set; }
        public Token OpenParAddress { get; set; }

        public Token(string s)
        {
            Escaped = false;
            Clouser = TokenClouser.none;
            Type = TokenType.normal;
            if (s == "(") Type = TokenType.paropen;
            else if (s == "|") Type = TokenType.or;
            else if (s == ")") Type = TokenType.parclose;
            else if (s == ")*")
            {
                Type = TokenType.parclose;
                Clouser = TokenClouser.star;
                s = ")";
            }
            else if (s == ")+")
            {
                Type = TokenType.parclose;
                Clouser = TokenClouser.plus;
                s = ")";
            }
            else if (s == ".*") Type = TokenType.any;
            else if (s.Length > 1)
            {
                if (s[0] == '\\') Escaped = true;
                if (s.Last() == '*' && s[s.Length - 2] != '\\')
                {
                    Clouser = TokenClouser.star;
                    s = s.Replace("*", "");
                }
                if (s.Last() == '+' && s[s.Length - 2] != '\\')
                {
                    Clouser = TokenClouser.plus;
                    s = s.Replace("*", "");
                }
            }
            s = s.Replace("\\", "");
            Value = s;
        }
        public static List<Token> Tokenizer(string s)
        {
            var Tokens = new List<Token>();
            s = s.Replace(" ", "");
            int LastIndex = 0, i;
            for (i = 0; i < s.Length; i++)
            {
                int Length = i - LastIndex;
                if (s[i] == '\\')
                {
                    if ((s.Length - 1) - i >= 3 && s[i + 2] == '\\' && s[i + 3] == '+')
                    {
                        if (Length > 0)
                        {
                            Tokens.Add(new Token(s.Substring(LastIndex, Length)));
                        }
                        Tokens.Add(new Token(s.Substring(i, 4)));
                        LastIndex = i + 4;
                        i += 3;
                    }
                    else
                    {
                        if (Length > 0)
                        {
                            Tokens.Add(new Token(s.Substring(LastIndex, Length)));
                        }
                        Tokens.Add(new Token(s.Substring(i, 2)));
                        LastIndex = i + 2;
                        i++;
                    }
                }
                else if (s[i] == '(')
                {
                    if (Length > 0)
                    {
                        Tokens.Add(new Token(s.Substring(LastIndex, Length)));
                    }
                    Tokens.Add(new Token("("));
                    LastIndex = i + 1;
                }
                else if (s[i] == '|')
                {
                    if (Length > 0)
                    {
                        Tokens.Add(new Token(s.Substring(LastIndex, Length)));
                    }
                    Tokens.Add(new Token("|"));
                    LastIndex = i + 1;
                }
                else if (s[i] == ')')
                {
                    if (Length > 0)
                    {
                        Tokens.Add(new Token(s.Substring(LastIndex, Length)));
                    }
                    if (i < s.Length - 1 && (s[i + 1] == '+' || s[i + 1] == '*'))
                    {
                        Tokens.Add(new Token(s.Substring(i, 2)));
                        LastIndex = i + 2;
                        i++;
                    }
                    else
                    {
                        Tokens.Add(new Token(")"));
                        LastIndex = i + 1;
                    }
                }
                else if (s[i] == '.' && i < s.Length - 1 && s[i + 1] == '*')
                {
                    if (Length > 0)
                    {
                        Tokens.Add(new Token(s.Substring(LastIndex, Length)));
                    }
                    Tokens.Add(new Token(s.Substring(i, 2)));
                    LastIndex = i + 2;
                    i++;
                }
                else if (s[i] == '*')
                {
                    if (Length > 0)
                    {
                        Tokens.Add(new Token(s.Substring(LastIndex, Length + 1)));
                    }
                    LastIndex = i + 1;
                }
                else if (s[i] == '+')
                {
                    if (Length > 0)
                    {
                        Tokens.Add(new Token(s.Substring(LastIndex, Length + 1)));
                    }
                    LastIndex = i + 1;
                }
            }
            if (LastIndex < i) Tokens.Add(new Token(s.Substring(LastIndex, (s.Length) - LastIndex)));
            return Tokens;
        }
    }
}

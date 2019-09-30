using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC
{
    public class CompilerToken
    {
        public string TokenName { get; set; }
        public string TokenValue { get; set; }
        public CompilerToken(string value, List<FiniteStateMachine> fsm)
        {
            value = value.Trim();
            TokenName = string.Empty;
            TokenValue = value;
            foreach (var item in fsm)
            {
                if (item.Evaluate(value))
                {
                    TokenName = item.Name;
                    break;
                }
            }
            if (TokenName == string.Empty) TokenName = "Wrong Token";
        }
        public CompilerToken(string value, string name)
        {
            TokenName = name;
            TokenValue = value;
        }
        public static List<string> Tokenizer(string s)
        {
            List<string> retValue = new List<string>();
            List<char> Delimeters = new List<char>
            {
                ',' , ';' , ':' , ' ' , '(' , '[' , ')' , ']' , '{' , '}',
                '\n'
            };
            int StartIndex = 0, i;
            for (i = 0; i < s.Length; i++)
            {
                bool flag = false;
                foreach (var item in Delimeters)
                {
                    if (s[i] == item)
                    {
                        try
                        {
                            retValue.Add(s.Substring(StartIndex, i - StartIndex));
                        }
                        catch { }
                        StartIndex = i + 1;
                        if (item != '\n' || item != ' ') retValue.Add(item.ToString());
                        flag = true;
                        break;
                    }
                }
                if (flag) continue;
                else if (s[i] == '+')
                {
                    try { retValue.Add(s.Substring(StartIndex, i - StartIndex)); } catch { }
                    if (i + 1 < s.Length && s[i + 1] == '+')
                    {
                        retValue.Add("++");
                        StartIndex = i + 2; i++;
                    }
                    else if (i + 1 < s.Length && s[i + 1] == '=')
                    {
                        retValue.Add("+=");
                        StartIndex = i + 2; i++;
                    }
                    else
                    {
                        retValue.Add("+");
                        StartIndex = i + 1;
                    }
                }
                else if (s[i] == '-')
                {
                    try { retValue.Add(s.Substring(StartIndex, i - StartIndex)); } catch { }
                    if (i + 1 < s.Length && s[i + 1] == '-')
                    {
                        retValue.Add("--");
                        StartIndex = i + 2; i++;
                    }
                    if (i + 1 < s.Length && s[i + 1] == '=')
                    {
                        retValue.Add("-=");
                        StartIndex = i + 2; i++;
                    }
                    else
                    {
                        retValue.Add("-");
                        StartIndex = i + 1;
                    }
                }
                else if (s[i] == '*')
                {
                    try { retValue.Add(s.Substring(StartIndex, i - StartIndex)); } catch { }
                    if (i + 1 < s.Length && s[i + 1] == '=')
                    {
                        retValue.Add("*=");
                        StartIndex = i + 2; i++;
                    }
                    else
                    {
                        retValue.Add("*");
                        StartIndex = i + 1;
                    }
                }
                else if (s[i] == '/')
                {
                    try { retValue.Add(s.Substring(StartIndex, i - StartIndex)); } catch { }
                    if (i + 1 < s.Length && s[i + 1] == '=')
                    {
                        retValue.Add("/=");
                        StartIndex = i + 2; i++;
                    }
                    if (i + 1 < s.Length && s[i + 1] == '/')
                    {
                        for (int j = i + 1; j < s.Length; j++)
                        {
                            if (s[j] == '\n')
                            {
                                retValue.Add(s.Substring(i, (j - i)));
                                StartIndex = j + 1;
                                i = j;
                                break;
                            }
                        }
                    }
                    else
                    {
                        retValue.Add("/");
                        StartIndex = i + 1;
                    }
                }
                else if (s[i] == '<')
                {
                    try { retValue.Add(s.Substring(StartIndex, i - StartIndex)); } catch { }
                    if (i + 1 < s.Length && s[i + 1] == '=')
                    {
                        retValue.Add("<=");
                        StartIndex = i + 2; i++;
                    }
                    else
                    {
                        retValue.Add("<");
                        StartIndex = i + 1;
                    }
                }
                else if (s[i] == '>')
                {
                    try { retValue.Add(s.Substring(StartIndex, i - StartIndex)); } catch { }
                    if (i + 1 < s.Length && s[i + 1] == '=')
                    {
                        retValue.Add(">=");
                        StartIndex = i + 2; i++;
                    }
                    else
                    {
                        retValue.Add(">");
                        StartIndex = i + 1;
                    }
                }
                else if (s[i] == '!')
                {
                    try { retValue.Add(s.Substring(StartIndex, i - StartIndex)); } catch { }
                    if (i + 1 < s.Length && s[i + 1] == '=')
                    {
                        retValue.Add("!=");
                        StartIndex = i + 2; i++;
                    }
                    else
                    {
                        retValue.Add("!");
                        StartIndex = i + 1;
                    }
                }
                else if (s[i] == '=')
                {
                    try { retValue.Add(s.Substring(StartIndex, i - StartIndex)); } catch { }
                    if (i + 1 < s.Length && s[i + 1] == '=')
                    {
                        retValue.Add("==");
                        StartIndex = i + 2; i++;
                    }
                    else
                    {
                        retValue.Add("=");
                        StartIndex = i + 1;
                    }
                }
                else if (s[i] == '"')
                {
                    try { retValue.Add(s.Substring(StartIndex, i - StartIndex)); } catch { }
                    for (int j = i + 1; j < s.Length; j++)
                    {
                        if (s[j] == '"')
                        {
                            retValue.Add(s.Substring(i, (j - i) + 1));
                            StartIndex = j + 1;
                            i = j;
                            break;
                        }
                    }
                }
                else if (s[i] == '\'')
                {
                    try { retValue.Add(s.Substring(StartIndex, i - StartIndex)); } catch { }
                    for (int j = i + 1; j < s.Length; j++)
                    {
                        if (s[j] == '\'')
                        {
                            retValue.Add(s.Substring(i, (j - i) + 1));
                            StartIndex = j + 1;
                            i = j;
                            break;
                        }
                    }
                }
                else if (s[i] == '@')
                {
                    try { retValue.Add(s.Substring(StartIndex, i - StartIndex)); } catch { }
                    bool f = false;
                    for (int j = i; j < s.Length; j++)
                    {
                        if (s[j] == '"' && f)
                        {
                            retValue.Add(s.Substring(i, (j - i) + 1));
                            StartIndex = j + 1;
                            i = j;
                            break;
                        }
                        if (s[j] == '"' && !f) f = true;
                    }
                }
            }
            try { retValue.Add(s.Substring(StartIndex, i - StartIndex)); } catch { }
            return retValue;
        }
    }
}

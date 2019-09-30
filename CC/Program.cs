using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CC
{
    class Program
    {
        static void Main(string[] args)
        {
            FiniteStateMachine fs1 = new FiniteStateMachine("DIGIT", "a*b*");
            FiniteStateMachine fs2 = new FiniteStateMachine("NUMBER_REAL", @"\DIGIT*\.\DIGIT+");
            FiniteStateMachine fs3 = new FiniteStateMachine("NUMBER_INTEGER", @"\DIGIT+");
            FiniteStateMachine fs4 = new FiniteStateMachine("LETTER", "[a-z]|[A-Z]");
            FiniteStateMachine fs5 = new FiniteStateMachine("ID", @"\LETTER(\LETTER|_|\DIGIT)*");
            FiniteStateMachine fs6 = new FiniteStateMachine("OBJECT", @"(\ID\.)+\ID");
            FiniteStateMachine fs7 = new FiniteStateMachine("OPERATOR", @"<=|>=|<|>|!=|==");
            FiniteStateMachine fs8 = new FiniteStateMachine("KEY_WORD", @"if|else|while|int|char|real|string|list|array|empty|class|public|and|or|assign|less|more|equal|not|plus|minus|mod|inc");
            FiniteStateMachine fs9 = new FiniteStateMachine("DOUBLE_QUOTATION", "\"");
            FiniteStateMachine fs10 = new FiniteStateMachine("COMMENT", @"@@#(\NUMBER_INTEGER)-(\DOUBLE_QUOTATION).*(\DOUBLE_QUOTATION)");
            while (true)
            {
                 string s = Console.ReadLine();
                fs1 = new FiniteStateMachine("a", s);
                Console.WriteLine(fs1.ToString());
                s = Console.ReadLine();
                Console.WriteLine(fs1.Evaluate(s));
                //Console.WriteLine(fs1.ToString());
                //Console.ReadKey();
            }
        }
    }
}

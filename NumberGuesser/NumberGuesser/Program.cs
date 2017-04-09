using System;
using System.Collections.Generic;

namespace NumberGuesser
{
    class Program
    {
        private const int MaxNumber = 100;
        private const string ExitWord = "q";

        private const int SwearingFreq = 4;
        private static readonly string[] Swearings =
        {
            "You're a looser, {0}...", 
            "Ha-ha-ha, everyone, look at {0}'s face!",
            "You are sooo stuid, {0}...",
            "I bet you're much quicker in your girlfriend's bed.",
            "{0}, are you kidding me?",
            "Can anybody sit here while I run to {0}'s mother?"
        };

        static void Main(string[] args)
        {
            Console.WriteLine("Enter user name: ");
            var userName = Console.ReadLine();
            if (string.IsNullOrEmpty(userName))
            {
                userName = Environment.UserName;
            }

            var random = new Random();
            var value = random.Next(MaxNumber + 1);
            Console.WriteLine("I made a number from {0} to {1}, try to guess it! " +
                              "(or type \"{2}\" to exit)", 0, MaxNumber, ExitWord);

            var history = new List<int>();
            var errors = 0;
            var startTime = DateTime.Now;

            for (;;)
            {
                var answer = Console.ReadLine();
                int res;
                if (int.TryParse(answer, out res))
                {
                    if (res == value)
                    {
                        ShowHistory(history, value);
                        break;
                    }

                    history.Add(res);

                    if (++errors % SwearingFreq == 0)
                        Console.WriteLine(Swearings[random.Next(Swearings.Length)], userName);
                    Console.WriteLine("Try number {0} than this", res > value ? "less" : "bigger");
                }
                else if (answer == ExitWord)
                {
                    Console.WriteLine("I'm so sorry, that you're not able to guess the number, " +
                                      "{0}", userName);
                    break;
                }
                else
                {
                    Console.WriteLine("What do you mean?");
                }
            }
            
            Console.WriteLine("You spend {0} seconds for this stupid task", Math.Round(DateTime.Now.Subtract(startTime).Duration().TotalSeconds));
            Console.ReadLine();
        }

        static void ShowHistory(IReadOnlyCollection<int> history, int value)
        {
            if (history.Count == 0)
            {
                Console.WriteLine("You're very good in numbers guessig!");
                return;
            }

            Console.WriteLine("History of your attempts:");
            foreach (var i in history)
            {
                Console.WriteLine("{0} ({1} than {2})", i, i > value ? "bigger" : "less", value);        
            }
        }
    }
}

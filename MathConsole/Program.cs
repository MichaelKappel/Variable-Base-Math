using Math;
using System;

namespace MathConsole
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Set Base");
            String rawKey = Console.ReadLine();
            
            Console.WriteLine("First Number");
            String firstNumberRaw = Console.ReadLine();

            Console.WriteLine("Second Number");
            String secondNumberRaw = Console.ReadLine();

            String addedNumbers = Number.Add(rawKey, firstNumberRaw, secondNumberRaw);

            Console.WriteLine("Addition Result");
            Console.WriteLine(addedNumbers);

            String multipliedNumbers = Number.Multiply(rawKey, firstNumberRaw, secondNumberRaw);

            Console.WriteLine("Multiplication Result");
            Console.WriteLine(multipliedNumbers);

            Main(args);
        }
    }
}

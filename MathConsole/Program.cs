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

            var environment = new MathEnvironment(rawKey);

            Console.WriteLine("First Number");
            Number firstNumberRaw = new Number(environment, Console.ReadLine());

            Console.WriteLine("Second Number");
            Number secondNumberRaw = new Number(environment, Console.ReadLine());

            Console.WriteLine("Addition Result");

            String addedNumbers = (firstNumberRaw + secondNumberRaw).ToString();

            Console.WriteLine(addedNumbers);

            Console.WriteLine("Multiplication Result");

            String multipliedNumbers = (firstNumberRaw * secondNumberRaw).ToString(); ;

            Console.WriteLine(multipliedNumbers);

            Main(args);
        }
    }
}

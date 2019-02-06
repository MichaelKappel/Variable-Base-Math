using VariableBase.Mathematics;
using System;

namespace VariableBase.MathematicsConsole
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Set Base");
            String rawKey = Console.ReadLine();

            var environment = new CharMathEnvironment(rawKey);

            Console.WriteLine("First Number");
            Number firstNumberRaw = environment.GetNumber(Console.ReadLine());

            Console.WriteLine("Second Number");
            Number secondNumberRaw = environment.GetNumber(Console.ReadLine());

            Console.WriteLine("Addition Result");

            String addedNumbers = (firstNumberRaw + secondNumberRaw).ToString();

            Console.WriteLine(addedNumbers);

            Console.WriteLine("Multiplication Result");

            String multipliedNumbers = (firstNumberRaw * secondNumberRaw).ToString(); 

            Console.WriteLine(multipliedNumbers);

            Main(args);
        }
    }
}

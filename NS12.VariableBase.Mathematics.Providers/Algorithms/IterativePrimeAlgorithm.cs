using System;


using System.Diagnostics;
using System.IO;
using System.Linq;


using NS12.VariableBase.Mathematics.Common.Models;
using NS12.VariableBase.Mathematics.Common.Interfaces;

using static NS12.VariableBase.Mathematics.Common.Models.NumberSegmentDictionary;

namespace NS12.VariableBase.Mathematics.Providers.Algorithms
{
    /// <summary>
    ///  Prime Algorithm
    ///  By: Michael Kappel, MCPD
    ///  Date: 1/23/2019
    /// </summary>
    public class IterativePrimeAlgorithm : IPrimeAlgorithm<Number>
    {
        public readonly DateTime Started;
        public NumberSegments MaxPrimeTested = new NumberSegments(new decimal[] { 1 });
        public decimal[][] PrimeNumbers = default;

        public IterativePrimeAlgorithm()
        {
            Started = DateTime.Now;
        }

        public bool SavePrimes { get; set; }

        public bool IsPrime(Number number)
        {
            bool isPrime = true;
            if (number.Size > 3)
            {
                throw new NotImplementedException();
            }
            else
            {
                decimal savedNumberNumber = number.Segments[0];

                if (number.Segments.Length <= 3)
                {
                    if (number.Segments.Length > 1)
                    {
                        savedNumberNumber = savedNumberNumber + number.Segments[1] * number.Environment.Base;
                    }

                    if (number.Segments.Length > 2)
                    {
                        savedNumberNumber = savedNumberNumber + number.Segments[2] * (number.Environment.Base * number.Environment.Base);
                    }

                    decimal squareRootOfPrime = (decimal)Math.Sqrt((double)savedNumberNumber);
                    for (int i = 2; i < squareRootOfPrime; i++)
                    {
                        if (savedNumberNumber % i == 0)
                        {
                            isPrime = false;
                            break;
                        }
                    }

                    if (isPrime)
                    {
                        Debug.WriteLine(string.Format("Verified prime: {0}", savedNumberNumber));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("Bad data in prime file: {0}", savedNumberNumber));
                    }
                }
            }

            return isPrime;
        }

        public (Number Numerator, Number Denominator) GetComposite(Number a)
        {
            IMathEnvironment<Number> environment = a.Environment;
            (Number Numerator, Number Denominator) result = default;
            if (a <= environment.GetNumber(3))
            {

            }
            else if (a.IsEven())
            {
                result = (a / environment.GetNumber(2), environment.GetNumber(2));
            }
            else
            {
                Number halfPrime = environment.GetNumber(Math.Ceiling(a.Segments[0] / 2));
                Number testNumber = environment.GetNumber(1);
                while (testNumber <= halfPrime)
                {
                    Number currentNumber = a / testNumber;
                    if (currentNumber.Fragment == default)
                    {
                        result = (currentNumber, testNumber);
                        break;
                    }
                    testNumber = testNumber + environment.GetNumber(2);
                }
            }
#if DEBUG
            if (a.Size == 1)
            {
                bool isPrime = true;
                decimal squareRootOfPrime = (decimal)Math.Sqrt((double)a.Segments[0]);
                int i = 2;
                for (; i <= squareRootOfPrime; i++)
                {
                    if (a.Segments[0] % i == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }

                if (isPrime == false && result == default)
                {
                    throw new Exception(string.Format("Math Error in GetComposite {0} should have had a Composite of {1}", a.Segments[0], i));
                }
                else if (isPrime == true && result != default)
                {
                    throw new Exception(string.Format("Math Error in GetComposite {0} should NOT have a Composite of {1} x {2}", a.Segments[0], result.Numerator.Segments[0], result.Denominator.Segments[0]));
                }
            }

            if (result != default)
            {
                foreach (decimal segment in result.Numerator.Segments)
                {
                    if (segment > environment.Base)
                    {
                        throw new Exception("Bad GetComposite segment larger than base Item 1");
                    }
                    else if (segment < 0)
                    {
                        throw new Exception("Bad GetComposite  segment less than zero Item 1");
                    }
                }

                foreach (decimal segment in result.Denominator.Segments)
                {
                    if (segment > environment.Base)
                    {
                        throw new Exception("Bad GetComposite segment larger than base Item 2");
                    }
                    else if (segment < 0)
                    {
                        throw new Exception("Bad GetComposite  segment less than zero Item 2");
                    }
                }
            }
#endif
            return result;
        }
    }
}

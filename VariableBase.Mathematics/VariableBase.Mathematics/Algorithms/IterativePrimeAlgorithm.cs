using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using VariableBase.Mathematics.Interfaces;
using VariableBase.Mathematics.Models;

namespace VariableBase.Mathematics
{
    /// <summary>
    ///  Prime Algorithm
    ///  By: Michael Kappel, MCPD
    ///  Date: 1/23/2019
    /// </summary>
    public class IterativePrimeAlgorithm : IPrimeAlgorithm
    {
        public readonly DateTime Started;
        public NumberSegments MaxPrimeTested = new NumberSegments(new Decimal[] { 1 });
        public Decimal[][] PrimeNumbers = default(Decimal[][]);

        public IterativePrimeAlgorithm(IMathEnvironment environment)
        {
            this.Started = DateTime.Now;
        }

        public Boolean SavePrimes { get; set; }

        public Boolean IsPrime(IMathEnvironment environment, IBasicMathAlgorithm basicMath, NumberSegments number)
        {
            if (number.Size > 1)
            {
                throw new NotImplementedException();
            }
            else
            {
                Decimal halfPrime = number[0] / 2;
                for (Int32 i = 2; i < halfPrime; i++)
                {
                    if (number[0] % i == 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        
        public Tuple<NumberSegments, NumberSegments> GetComposite(IMathEnvironment environment, IBasicMathAlgorithm basicMath, NumberSegments a)
        {
            Tuple<NumberSegments, NumberSegments> result = default(Tuple<NumberSegments, NumberSegments>);
            if (Number.IsBottom(a) || Number.IsFirst(a) || basicMath.IsEqual(environment, a, environment.SecondNumber.Segments))
            {

            }
            else if (basicMath.IsEven(environment, a))
            {
                Tuple<NumberSegments, NumberSegments, NumberSegments> half = basicMath.Divide(environment, a, environment.SecondNumber.Segments);
                if (half.Item2 != default(NumberSegments) || half.Item2 != default(NumberSegments))
                {
                    throw new Exception("Math error in GetDivisor IsEven half");
                }
                result = new Tuple<NumberSegments, NumberSegments>(half.Item1, environment.SecondNumber.Segments);
            }
            else
            {
                var halfPrime = new NumberSegments(new Decimal[]{ System.Math.Ceiling(a[0] / 2) });
                var testNumber = new NumberSegments(new Decimal[]{ 1 });
                while (basicMath.IsLessThanOrEqualTo(environment, testNumber, halfPrime))
                {
                    Tuple<NumberSegments, NumberSegments, NumberSegments> currentNumber = basicMath.Divide(environment, a, testNumber);
                    if (currentNumber.Item2 == default(NumberSegments))
                    {
                        result = new Tuple<NumberSegments, NumberSegments>(currentNumber.Item1, testNumber);
                        break;
                    }
                    testNumber = basicMath.Add(environment, testNumber, environment.SecondNumber.Segments);
                }
            }
#if DEBUG
            if (a.Size == 1)
            {
                Boolean isPrime = true;
                Decimal halfPrime = a[0] / 2;
                Int32 i = 2;
                for (; i <= halfPrime; i++)
                {
                    if (a[0] % i == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }

                if (isPrime == false && result == default(Tuple<NumberSegments, NumberSegments>))
                {
                    throw new Exception(String.Format("Math Error in GetComposite {0} should have had a Composite of {1}", a[0], i));
                }
                else if (isPrime == true && result != default(Tuple<NumberSegments, NumberSegments>))
                {
                    throw new Exception(String.Format("Math Error in GetComposite {0} should NOT have a Composite of {1} x {2}", a[0], result.Item1[0], result.Item2[0]));
                }
            }
            if (result != default(Tuple<NumberSegments, NumberSegments>))
            {
                foreach (Decimal segment in result.Item1)
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

                foreach (Decimal segment in result.Item2)
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

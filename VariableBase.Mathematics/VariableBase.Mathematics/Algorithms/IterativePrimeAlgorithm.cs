using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Common.Interfaces;
using Common.Models;
using static Common.Models.NumberSegmentDictionary;

namespace VariableBase.Mathematics.Algorithms
{
    /// <summary>
    ///  Prime Algorithm
    ///  By: Michael Kappel, MCPD
    ///  Date: 1/23/2019
    /// </summary>
    public class IterativePrimeAlgorithm : IPrimeAlgorithm<Number>
    {
        public readonly DateTime Started;
        public NumberSegments MaxPrimeTested = new NumberSegments(new Decimal[] { 1 });
        public Decimal[][] PrimeNumbers = default(Decimal[][]);

        public IterativePrimeAlgorithm()
        {
            this.Started = DateTime.Now;
        }

        public Boolean SavePrimes { get; set; }

        public Boolean IsPrime(Number number)
        {
            Boolean isPrime = true;
            if (number.Size > 3)
            {
                throw new NotImplementedException();
            }
            else
            {
                Decimal savedNumberNumber = number.Segments[0];

                if (number.Segments.Length <= 3)
                {
                    if (number.Segments.Length > 1)
                    {
                        savedNumberNumber = savedNumberNumber + (number.Segments[1] * number.Environment.Base);
                    }

                    if (number.Segments.Length > 2)
                    {
                        savedNumberNumber = savedNumberNumber + (number.Segments[2] * (number.Environment.Base * number.Environment.Base));
                    }

                    Decimal squareRootOfPrime = (Decimal)Math.Sqrt((Double)savedNumberNumber);
                    for (Int32 i = 2; i < squareRootOfPrime; i++)
                    {
                        if (savedNumberNumber % i == 0)
                        {
                            isPrime = false;
                            break;
                        }
                    }

                    if (isPrime)
                    {
                        Debug.WriteLine(String.Format("Verified prime: {0}", savedNumberNumber));
                    }
                    else
                    {
                        Debug.WriteLine(String.Format("Bad data in prime file: {0}", savedNumberNumber));
                    }
                }
            }

            return isPrime;
        }
        
        public Tuple<Number, Number> GetComposite(Number a)
        {
            IMathEnvironment<Number> environment = a.Environment;
            Tuple<Number, Number> result = default(Tuple<Number, Number>);
            if (a <= environment.GetNumber(3))
            {

            }
            else if (a.IsEven())
            {
                result = new Tuple<Number, Number>(a / environment.GetNumber(2), environment.GetNumber(2));
            }
            else
            {
                Number halfPrime = environment.GetNumber(System.Math.Ceiling(a.Segments[0] / 2));
                Number testNumber = environment.GetNumber(1);
                while (testNumber <= halfPrime)
                {
                    Number currentNumber =  a / testNumber;
                    if (currentNumber.Fragment == default(Fraction))
                    {
                        result = new Tuple<Number, Number>(currentNumber, testNumber);
                        break;
                    }
                    testNumber = testNumber + environment.GetNumber(2);
                }
            }
#if DEBUG
            if (a.Size == 1)
            {
                Boolean isPrime = true;
                Decimal squareRootOfPrime = (Decimal)Math.Sqrt((Double)a.Segments[0]);
                Int32 i = 2;
                for (; i <= squareRootOfPrime; i++)
                {
                    if (a.Segments[0] % i == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }

                if (isPrime == false && result == default(Tuple<Number, Number>))
                {
                    throw new Exception(String.Format("Math Error in GetComposite {0} should have had a Composite of {1}", a.Segments[0], i));
                }
                else if (isPrime == true && result != default(Tuple<Number, Number>))
                {
                    throw new Exception(String.Format("Math Error in GetComposite {0} should NOT have a Composite of {1} x {2}", a.Segments[0], result.Item1.Segments[0], result.Item2.Segments[0]));
                }
            }
            if (result != default(Tuple<Number, Number>))
            {
                foreach (Decimal segment in result.Item1.Segments)
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

                foreach (Decimal segment in result.Item2.Segments)
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

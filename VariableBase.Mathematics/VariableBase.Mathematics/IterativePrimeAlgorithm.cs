using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using VariableBase.Mathematics.Interfaces;

namespace VariableBase.Mathematics
{
    /// <summary>
    ///  Prime Algorithm
    ///  By: Michael Kappel, MCPD
    ///  Date: 1/23/2019
    /// </summary>
    public class IterativePrimeAlgorithm : IPrimeAlgorithm
    {
        internal IMathEnvironment Environment;
        internal IBasicMathAlgorithm BasicMath;

        public readonly DateTime Started;
        public ReadOnlyCollection<Double> MaxPrimeTested = new ReadOnlyCollection<Double>(new Double[] { 1 });
        public Double[][] PrimeNumbers = default(Double[][]);

        public IterativePrimeAlgorithm(DecimalMathEnvironment environment)
        {
            this.Started = DateTime.Now;
            this.Environment = environment;
            this.BasicMath = environment.BasicMath;
        }

        public Boolean SavePrimes { get; set; }

        public Boolean IsPrime(ReadOnlyCollection<Double> number)
        {
            if (number.Count > 1)
            {
                throw new NotImplementedException();
            }
            else
            {
                Double halfPrime = number[0] / 2;
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
        
        public Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>> GetComposite(ReadOnlyCollection<Double> a)
        {
            Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>> result = default(Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>>);
            if (this.BasicMath.IsBottom(a) || this.BasicMath.IsFirst(a) || this.BasicMath.IsEqual(a, this.Environment.SecondNumber.Segments))
            {

            }
            else if (this.BasicMath.IsEven(a))
            {
                Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>, ReadOnlyCollection<Double>> half = this.BasicMath.Divide(a, this.Environment.SecondNumber.Segments);
                if (half.Item2 != default(ReadOnlyCollection<Double>) || half.Item2 != default(ReadOnlyCollection<Double>))
                {
                    throw new Exception("Math error in GetDivisor IsEven half");
                }
                result = new Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>>(half.Item1, this.Environment.SecondNumber.Segments);
            }
            else
            {
                var halfPrime = new ReadOnlyCollection<Double>(new Double[]{ System.Math.Ceiling(a[0] / 2) });
                var testNumber = new ReadOnlyCollection<Double>(new Double[]{ 1 });
                while (this.BasicMath.IsLessThanOrEqualTo(testNumber, halfPrime))
                {
                    Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>, ReadOnlyCollection<Double>> currentNumber = this.BasicMath.Divide(a, testNumber);
                    if (currentNumber.Item2 == default(ReadOnlyCollection<Double>))
                    {
                        result = new Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>>(currentNumber.Item1, testNumber);
                        break;
                    }
                    testNumber = this.BasicMath.Add(testNumber, Environment.SecondNumber.Segments);
                }
            }
#if DEBUG
            if (a.Count == 1)
            {
                Boolean isPrime = true;
                Double halfPrime = a[0] / 2;
                Int32 i = 2;
                for (; i <= halfPrime; i++)
                {
                    if (a[0] % i == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }

                if (isPrime == false && result == default(Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>>))
                {
                    throw new Exception(String.Format("Math Error in GetComposite {0} should have had a Composite of {1}", a[0], i));
                }
                else if (isPrime == true && result != default(Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>>))
                {
                    throw new Exception(String.Format("Math Error in GetComposite {0} should NOT have a Composite of {1} x {2}", a[0], result.Item1[0], result.Item2[0]));
                }
            }
            if (result != default(Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>>))
            {
                foreach (Double segment in result.Item1)
                {
                    if (segment > this.Environment.Base)
                    {
                        throw new Exception("Bad GetComposite segment larger than base Item 1");
                    }
                    else if (segment < 0)
                    {
                        throw new Exception("Bad GetComposite  segment less than zero Item 1");
                    }
                }

                foreach (Double segment in result.Item2)
                {
                    if (segment > this.Environment.Base)
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

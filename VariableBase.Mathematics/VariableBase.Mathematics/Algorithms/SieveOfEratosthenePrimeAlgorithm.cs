using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VariableBase.Mathematics.Interfaces;
using VariableBase.Mathematics.Models;
using static VariableBase.Mathematics.Models.NumberSegmentDictionary;

namespace VariableBase.Mathematics.Algorithms
{
    /// <summary>
    ///  Prime Algorithm
    ///  Based on the Sieve Of EratostheneD:\git\Math\VariableBase.Mathematics\VariableBase.Mathematics\Algorithms\SieveOfEratosthenePrimeAlgorithm.cs
    ///  By: Michael Kappel, MCPD
    ///  Date: 1/23/2019
    /// </summary>
    public class SieveOfEratosthenePrimeAlgorithm: IPrimeAlgorithm
    {
        public readonly DateTime Started;
        public NumberSegments PrimeListMaxNumber;
        public NumberSegmentDictionary PrimeNumberTree = new NumberSegmentDictionary(null);
        public IList<NumberSegments> PrimeNumbers = new List<NumberSegments>();
        public Action<NumberSegments> OnPrimeFound;

        public SieveOfEratosthenePrimeAlgorithm(Action<NumberSegments> onPrimeFound = default(Action<NumberSegments>))
        {
            this.Started = DateTime.Now;
            this.OnPrimeFound = onPrimeFound;
            this.LoadSupplementalPrimes(new List<Char[]>() { new Char[] { (Char)2 } , new Char[] { (Char)3 } });
            this.PrimeListMaxNumber = PrimeNumbers[1];
        }

        public void LoadSupplementalPrimes(IList<Char[]> primeNumbersRaw)
        {
            foreach (var primeRaw in primeNumbersRaw)
            {
                NumberSegments prime = new NumberSegments(primeRaw);
                this.PrimeNumberTree.Add(prime, NumberTypes.Prime);
                this.PrimeNumbers.Add(prime);
            }
        }

        public void LoadSeedPrimes(IList<Char[]> primeNumbersRaw, IMathEnvironment environment, IBasicMathAlgorithm basicMath)
        {
            foreach (var primeRaw in primeNumbersRaw)
            {
                NumberSegments prime = new NumberSegments(primeRaw);
                this.PrimeNumberTree.Add(prime, NumberTypes.Prime);
                this.PrimeNumbers.Add(prime);

                if (basicMath.IsGreaterThan(environment, prime, this.PrimeListMaxNumber)) {
                    this.PrimeListMaxNumber = prime;
                }
            }
        }

        public Boolean IsPrime(IMathEnvironment environment, IBasicMathAlgorithm basicMath, NumberSegments number)
        {
            NumberTypes numberType = this.GetNumberType(environment, basicMath, number);
            if (numberType == NumberTypes.Prime)
            {
                return true;
            }
            else if (numberType != NumberTypes.Unknown)
            {
                return false;
            }
            else
            {
                Tuple<NumberSegments, NumberSegments> numberComposite = this.GetComposite(environment, basicMath, number);
                if (numberComposite == default(Tuple<NumberSegments, NumberSegments>))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        public NumberTypes GetNumberType(IMathEnvironment environment, IBasicMathAlgorithm basicMath, NumberSegments number)
        {
            if (basicMath.IsGreaterThan(environment, number, environment.SecondNumber.Segments) && basicMath.IsEven(environment, number))
            {
                return NumberTypes.Composite;
            }

            NumberTypes currentNumberType = this.PrimeNumberTree.GetNumberType(number);
            if (currentNumberType == NumberTypes.Unknown && basicMath.IsLessThanOrEqualTo(environment, number, this.PrimeListMaxNumber))
            {
                return NumberTypes.Composite;
            }
            else if (currentNumberType != NumberTypes.Unknown)
            {
                return currentNumberType;
            }
            
            Tuple<NumberSegments, NumberSegments> numberComposite = this.GetComposite(environment, basicMath, number);
            if (numberComposite == default(Tuple<NumberSegments, NumberSegments>))
            {
                this.PrimeNumberTree.Add(number, NumberTypes.Prime);
                this.PrimeNumbers.Add(number);
                if (this.OnPrimeFound != default(Action<NumberSegments>))
                {
                    this.OnPrimeFound(number);
                }
                return NumberTypes.Prime;
            }
            else
            {
                return NumberTypes.Composite;
            }
        }

        public Tuple<NumberSegments, NumberSegments> GetComposite(IMathEnvironment environment, IBasicMathAlgorithm basicMath, NumberSegments a)
        {
            Tuple<NumberSegments, NumberSegments> result = default(Tuple<NumberSegments, NumberSegments>);
            if (Number.IsBottom(a) || Number.IsFirst(a) || basicMath.IsEqual(environment, basicMath.AsSegments(environment, 3), a) || basicMath.IsEqual(environment, a, environment.SecondNumber.Segments))
            {

            }
            else if (basicMath.IsEven(environment, a))
            {
                Tuple<NumberSegments, NumberSegments, NumberSegments> half = basicMath.Divide(environment, a, environment.SecondNumber.Segments);
                if (half.Item2 != default(NumberSegments) || half.Item3 != default(NumberSegments))
                {
                    throw new Exception("Math error in GetDivisor IsEven half");
                }
                result = new Tuple<NumberSegments, NumberSegments>(half.Item1, environment.SecondNumber.Segments);
            }
            else
            {
                var maxNumberRaw = basicMath.SquareRoot(environment, a);
                if (maxNumberRaw.Item2 == default(NumberSegments))
                {
                    result = new Tuple<NumberSegments, NumberSegments>(maxNumberRaw.Item1, environment.KeyNumber[1].Segments);
                }
                else
                {
                    NumberSegments maxNumber = basicMath.Add(environment, maxNumberRaw.Item1, environment.SecondNumber.Segments);

                    NumberSegments lastResultWholeNumber = default(NumberSegments);

                    Int32 iPrime = 1;

                    Boolean usePrime = true;

                    NumberSegments currentNumberToTry = this.PrimeNumbers[iPrime];
                    while (basicMath.IsLessThanOrEqualTo(environment, currentNumberToTry, maxNumber))
                    {
                        Tuple<NumberSegments, NumberSegments, NumberSegments> currentNumber = basicMath.Divide(environment, a, currentNumberToTry, lastResultWholeNumber);
                        if (currentNumber.Item2 == default(NumberSegments))
                        {
                            result = new Tuple<NumberSegments, NumberSegments>(currentNumber.Item1, currentNumberToTry);
                            break;
                        }
                        lastResultWholeNumber = currentNumber.Item1;

                        if (usePrime)
                        {
                            iPrime++;
                            if (iPrime >= this.PrimeNumbers.Count)
                            {
                                usePrime = false;
                            }

                            if (usePrime)
                            {
                                var nextNumberToTry = this.PrimeNumbers[iPrime];
                                if (basicMath.IsGreaterThanOrEqualTo(environment, nextNumberToTry, this.PrimeListMaxNumber))
                                {
                                    usePrime = false;
                                }
                                else
                                {
                                    currentNumberToTry = nextNumberToTry;
                                }
                            }
                        }

                        if (!usePrime)
                        {
                            currentNumberToTry = basicMath.Add(environment, currentNumberToTry, environment.SecondNumber.Segments);
                        }

                    }
                }
            }

            return result;
        }
    }
}

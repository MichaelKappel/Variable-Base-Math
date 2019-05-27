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
using Common.Interfaces;
using Common.Models;
using static Common.Models.NumberSegmentDictionary;

namespace VariableBase.Mathematics.Algorithms
{
    /// <summary>
    ///  Prime Algorithm
    ///  Based on the Sieve Of Eratosthene 
    ///  By: Michael Kappel, MCPD
    ///  Date: 1/23/2019
    /// </summary>
    public class SieveOfEratosthenePrimeAlgorithm: IPrimeAlgorithm<Number>
    {
        public readonly DateTime Started;
        public Number PrimeListMaxNumber;
        public IDictionary<Int32, (IList<Number> Numbers, NumberSegmentDictionary Segments)> PrimeNumberTree = new Dictionary<Int32, (IList<Number> NumberList, NumberSegmentDictionary NumberDictionary)>();
        public Action<Number> OnPrimeFound;

        public void SavePrimesFile(String fileNameWithoutExtension)
        {
            foreach (KeyValuePair<Int32, (IList<Number> Numbers, NumberSegmentDictionary Segments)> primesInMemory in this.PrimeNumberTree)
            {
                IEnumerable<String> primes = primesInMemory.Value.Numbers.Select(x => x.GetCharArray()).OrderBy(x => x).Distinct();
                using (FileStream fs = File.Open(fileNameWithoutExtension + ".p" + primesInMemory.Key, FileMode.Create))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        foreach (String prime in primes)
                        {
                            sw.WriteLine(prime);
                        }
                    }
                }
            }
        }

        public void AddToFile(String fileName, String prime)
        {
            using (FileStream fs = File.Open(fileName, FileMode.Append))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(prime);
                }
            }
        }
        
        public SieveOfEratosthenePrimeAlgorithm(IList<Number> seedPrimeNumbers, Action<Number> onPrimeFound = default(Action<Number>))
        {
            foreach (Number prime in seedPrimeNumbers)
            {
                Int32 environmentBase = (Int32)prime.Environment.Base;
                if (!this.PrimeNumberTree.ContainsKey(environmentBase))
                {
                    this.PrimeNumberTree.Add(environmentBase, (new List<Number>(), new NumberSegmentDictionary(null)));
                }

                this.PrimeNumberTree[environmentBase].Numbers.Add(prime);
                this.PrimeNumberTree[environmentBase].Segments.Add(prime.Segments, NumberTypes.Prime);

                if (onPrimeFound != default(Action<Number>))
                {
                    onPrimeFound(prime);
                }
            }

            this.Started = DateTime.Now;
            this.OnPrimeFound = onPrimeFound;
        }


        public SieveOfEratosthenePrimeAlgorithm(IMathEnvironment<Number> environment, IList<String> primeNumbersRaw, Action<Number> onPrimeFound = default(Action<Number>))
        {
            this.PrimeNumberTree.Add((Int32)environment.Base, (new List<Number>(), new NumberSegmentDictionary(null)));

            for (var i = 0; i < primeNumbersRaw.Count; i++)
            {
                Number prime = environment.GetNumber(primeNumbersRaw[i]);
                this.PrimeNumberTree[(Int32)environment.Base].Numbers.Add(prime);
                this.PrimeNumberTree[(Int32)environment.Base].Segments.Add(prime.Segments, NumberTypes.Prime);

                if (prime > this.PrimeListMaxNumber)
                {
                    this.PrimeListMaxNumber = prime;
                }
                
                if (onPrimeFound != default(Action<Number>))
                {
                    onPrimeFound(prime);
                }
            }

            this.Started = DateTime.Now;
            this.OnPrimeFound = onPrimeFound;
        }

        public Boolean IsPrime(Number number)
        {
            NumberTypes numberType = this.GetNumberType(number);
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
                (Number Numerator, Number Denominator) numberComposite = this.GetComposite(number);
                if (numberComposite == default((Number Numerator, Number Denominator)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        public NumberTypes GetNumberType(Number number)
        {
            IMathEnvironment<Number> environment = number.Environment;
            if ((number > number.Environment.GetNumber(2) && number.IsEven()) || number == environment.GetNumber(0))
            {
                return NumberTypes.Composite;
            }

            NumberTypes currentNumberType = this.PrimeNumberTree[(Int32)environment.Base].Segments.GetNumberType(number.Segments);
            if (currentNumberType == NumberTypes.Unknown &&  number < this.PrimeListMaxNumber)
            {
                return NumberTypes.Composite;
            }
            else if (currentNumberType != NumberTypes.Unknown)
            {
                return currentNumberType;
            }
            
            (Number Numerator, Number Denominator) numberComposite = this.GetComposite(number);
            if (numberComposite == default((Number Numerator, Number Denominator)))
            {
                this.PrimeNumberTree[(Int32)environment.Base].Segments.Add(number.Segments, NumberTypes.Prime);
                this.PrimeNumberTree[(Int32)environment.Base].Numbers.Add(number);
                this.PrimeListMaxNumber = number;
                if (this.OnPrimeFound != default(Action<Number>))
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

        public (Number Numerator, Number Denominator) GetComposite(Number a)
        {
            (Number Numerator, Number Denominator) result = default((Number Numerator, Number Denominator));
            IMathEnvironment<Number> environment = a.Environment;
            if (Number.IsBottom(a) || Number.IsFirst(a) || a == environment.GetNumber(2) || a == environment.GetNumber(3))
            {

            }
            else if (a.IsEven())
            {
                result = (a / environment.GetNumber(2), environment.GetNumber(2)); 
            }
            else
            {
                Number maxNumber = a.SquareRoot();
                if (maxNumber.Fragment == default(Fraction))
                {
                    result = (maxNumber, maxNumber);
                }
                else
                {
                    Number lastResultWholeNumber = default(Number);

                    Int32 iPrime = 0;

                    Boolean usePrime = true;

                    Number currentNumberToTry = this.PrimeNumberTree[(Int32)environment.Base].Numbers[iPrime];
                    while (currentNumberToTry <= maxNumber)
                    {
                        Number currentNumber = Number.Operator.Divide(a, currentNumberToTry, lastResultWholeNumber);
                        if (currentNumber.Fragment == default(Fraction))
                        {
                            result = (currentNumber, currentNumberToTry);
                            break;
                        }
                        lastResultWholeNumber = environment.GetNumber(currentNumber.Segments);

                        if (usePrime)
                        {
                            iPrime++;
                            if (iPrime >= this.PrimeNumberTree[(Int32)environment.Base].Numbers.Count)
                            {
                                usePrime = false;
                            }

                            if (usePrime)
                            {
                                var nextNumberToTry = this.PrimeNumberTree[(Int32)environment.Base].Numbers[iPrime];
                                if (nextNumberToTry >= this.PrimeListMaxNumber)
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
                            currentNumberToTry = currentNumberToTry + a.Environment.GetNumber(2);
                        }

                    }
                }
            }

            return result;
        }
    }
}

using System;


using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

using System.Threading;
using System.Threading.Tasks;

using NS12.VariableBase.Mathematics.Common.Models;
using NS12.VariableBase.Mathematics.Common.Interfaces;

using static NS12.VariableBase.Mathematics.Common.Models.NumberSegmentDictionary;
using System.Collections.Generic;

namespace NS12.VariableBase.Mathematics.Providers.Algorithms
{
    /// <summary>
    ///  Prime Algorithm
    ///  Based on the Sieve Of Eratosthene 
    ///  By: Michael Kappel, MCPD
    ///  Date: 1/23/2019
    /// </summary>
    public class SieveOfEratosthenePrimeAlgorithm : IPrimeAlgorithm<Number>
    {
        public readonly DateTime Started;
        public Number PrimeListMaxNumber;
        public IDictionary<int, (IList<Number> Numbers, NumberSegmentDictionary Segments)> PrimeNumberTree = new Dictionary<int, (IList<Number> NumberList, NumberSegmentDictionary NumberDictionary)>();
        public Action<Number> OnPrimeFound;

        public void SavePrimesFile(string fileNameWithoutExtension)
        {
            foreach (KeyValuePair<int, (IList<Number> Numbers, NumberSegmentDictionary Segments)> primesInMemory in PrimeNumberTree)
            {
                IEnumerable<string> primes = primesInMemory.Value.Numbers.Select(x => x.GetCharArray()).OrderBy(x => x).Distinct();
                using (FileStream fs = File.Open(fileNameWithoutExtension + ".p" + primesInMemory.Key, FileMode.Create))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        foreach (string prime in primes)
                        {
                            sw.WriteLine(prime);
                        }
                    }
                }
            }
        }

        public void AddToFile(string fileName, string prime)
        {
            using (FileStream fs = File.Open(fileName, FileMode.Append))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(prime);
                }
            }
        }

        public SieveOfEratosthenePrimeAlgorithm(IList<Number> seedPrimeNumbers, Action<Number> onPrimeFound = default)
        {
            foreach (Number prime in seedPrimeNumbers)
            {
                int environmentBase = (int)prime.Environment.Base;
                if (!PrimeNumberTree.ContainsKey(environmentBase))
                {
                    PrimeNumberTree.Add(environmentBase, (new List<Number>(), new NumberSegmentDictionary(null)));
                }

                PrimeNumberTree[environmentBase].Numbers.Add(prime);
                PrimeNumberTree[environmentBase].Segments.Add(prime.Segments, NumberTypes.Prime);

                if (onPrimeFound != default)
                {
                    onPrimeFound(prime);
                }
            }

            Started = DateTime.Now;
            OnPrimeFound = onPrimeFound;
        }


        public SieveOfEratosthenePrimeAlgorithm(IMathEnvironment<Number> environment, IList<string> primeNumbersRaw, Action<Number> onPrimeFound)
        {
            PrimeNumberTree.Add((int)environment.Base, (new List<Number>(), new NumberSegmentDictionary()));

            for (var i = 0; i < primeNumbersRaw.Count; i++)
            {
                Number prime = environment.GetNumber(primeNumbersRaw[i]);
                PrimeNumberTree[(int)environment.Base].Numbers.Add(prime);
                PrimeNumberTree[(int)environment.Base].Segments.Add(prime.Segments, NumberTypes.Prime);

                if (prime > PrimeListMaxNumber)
                {
                    PrimeListMaxNumber = prime;
                }

                if (onPrimeFound != default)
                {
                    onPrimeFound(prime);
                }
            }

            Started = DateTime.Now;
            OnPrimeFound = onPrimeFound;
        }

        public bool IsPrime(Number number)
        {
            NumberTypes numberType = GetNumberType(number);
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
                (Number Numerator, Number Denominator) numberComposite = GetComposite(number);
                if (numberComposite == default)
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
            if (number > number.Environment.GetNumber(2) && number.IsEven() || number == environment.GetNumber(0))
            {
                return NumberTypes.Composite;
            }

            NumberTypes currentNumberType = PrimeNumberTree[(int)environment.Base].Segments.GetNumberType(number.Segments);
            if (currentNumberType == NumberTypes.Unknown && number < PrimeListMaxNumber)
            {
                return NumberTypes.Composite;
            }
            else if (currentNumberType != NumberTypes.Unknown)
            {
                return currentNumberType;
            }

            (Number Numerator, Number Denominator) numberComposite = GetComposite(number);
            if (numberComposite == default)
            {
                PrimeNumberTree[(int)environment.Base].Segments.Add(number.Segments, NumberTypes.Prime);
                PrimeNumberTree[(int)environment.Base].Numbers.Add(number);
                PrimeListMaxNumber = number;
                if (OnPrimeFound != default)
                {
                    OnPrimeFound(number);
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
            (Number Numerator, Number Denominator) result = default;
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
                if (maxNumber.Fragment == default)
                {
                    result = (maxNumber, maxNumber);
                }
                else
                {
                    Number lastResultWholeNumber = default;

                    int iPrime = 0;

                    bool usePrime = true;

                    Number currentNumberToTry = PrimeNumberTree[(int)environment.Base].Numbers[iPrime];
                    while (currentNumberToTry <= maxNumber)
                    {
                        Number currentNumber = Number.Operator.Divide(a, currentNumberToTry, lastResultWholeNumber);
                        if (currentNumber.Fragment == default)
                        {
                            result = (currentNumber, currentNumberToTry);
                            break;
                        }
                        lastResultWholeNumber = environment.GetNumber(currentNumber.Segments);

                        if (usePrime)
                        {
                            iPrime++;
                            if (iPrime >= PrimeNumberTree[(int)environment.Base].Numbers.Count)
                            {
                                usePrime = false;
                            }

                            if (usePrime)
                            {
                                var nextNumberToTry = PrimeNumberTree[(int)environment.Base].Numbers[iPrime];
                                if (nextNumberToTry >= PrimeListMaxNumber)
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

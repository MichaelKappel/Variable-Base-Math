using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using VariableBase.Mathematics.Interfaces;
using VariableBase.Mathematics.Models;
using static VariableBase.Mathematics.Models.NumberSegmentDictionary;

namespace VariableBase.Mathematics
{
  
   

    /// <summary>
    ///  Prime Algorithm
    ///  Based on the Sieve Of Eratosthene
    ///  By: Michael Kappel, MCPD
    ///  Date: 1/23/2019
    /// </summary>
    public class SieveOfEratosthenePrimeAlgorithm: IPrimeAlgorithm
    {
        public readonly DateTime Started;
        public Boolean SavePrimes { get; set; }
        public NumberSegments MaxPrimeTested = new NumberSegments(new Decimal[] { 1 });
        public NumberSegmentDictionary PrimeNumbers = default(NumberSegmentDictionary);

        public SieveOfEratosthenePrimeAlgorithm(Boolean savePrimes)
        {
            this.Started = DateTime.Now;
            this.SavePrimes = savePrimes;
        }

        public Boolean IsPrime(IMathEnvironment environment, IBasicMathAlgorithm basicMath, NumberSegments number)
        {
            NumberTypes numberType = this.GetNumberType(environment, basicMath, number);
            if (numberType == NumberTypes.Prime)
            {
                return true;
            }
            else if (numberType == NumberTypes.Composite)
            {
                return false;
            }
            else
            {
                //This should olny happen is saving is off
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
            String primeCacheFolder = String.Format("../../../../Primes/{0}/", environment.Base);
            if (this.PrimeNumbers == default(NumberSegmentDictionary))
            {
                if (!Directory.Exists(primeCacheFolder))
                {
                    Directory.CreateDirectory(primeCacheFolder);
                }
                if (!File.Exists(String.Format("{0}Primes.txt", primeCacheFolder)))
                {
                    File.Create(String.Format("{0}Primes.txt", primeCacheFolder));
                }
                else
                {
                    File.Copy(String.Format("{0}Primes.txt", primeCacheFolder), String.Format("{0}{1}.txt", primeCacheFolder, this.Started.Ticks));
                }
                this.PrimeNumbers = new NumberSegmentDictionary(null);
                using (FileStream fs = File.Open(String.Format("{0}{1}.txt", primeCacheFolder, this.Started.Ticks), FileMode.OpenOrCreate))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        NumberSegments lastprime = default(NumberSegments);
                        while (sr.Peek() >= 0)
                        {
                            String csv = sr.ReadLine();
                            lastprime = new NumberSegments(csv.Split(',').Reverse().Select(x => Decimal.Parse(x)).ToArray());

                            //#if DEBUG
                            //                                Boolean isPrime = true;
                            //                                Decimal halfPrime = prime[0] / 2;
                            //                                for (Int32 i = 2; i < halfPrime; i++)
                            //                                {
                            //                                    if (prime[0] % i == 0)
                            //                                    {
                            //                                        isPrime = false;
                            //                                        break;
                            //                                    }
                            //                                }

                            //                                if (!isPrime)
                            //                                {
                            //                                    throw new Exception(String.Format("Bad data in prime file: {0}", prime[0]));
                            //                                }
                            //                                Debug.WriteLine(String.Format("Verified prime: {0}", prime[0]));
                            //#endif

                            this.PrimeNumbers.Add(lastprime, NumberTypes.Prime);
                        }
                        if (lastprime == default(NumberSegments))
                        {
                            lastprime = new NumberSegments(new Decimal[]{ 2M });
                            this.PrimeNumbers.Add(lastprime, NumberTypes.Prime);
                        }
                        else
                        {
                            this.MaxPrimeTested = lastprime;
                        }
                    }
                }
            }

            if (this.SavePrimes)
            {
                while (basicMath.IsGreaterThan(environment, number, this.MaxPrimeTested))
                {
                    Tuple<NumberSegments, NumberSegments> numberComposite = this.GetComposite(environment, basicMath, number);
                    if (numberComposite == default(Tuple<NumberSegments, NumberSegments>))
                    {
#if DEBUG
                        if (basicMath.IsGreaterThan(environment, number, environment.SecondNumber.Segments) && basicMath.IsEven(environment, number))
                        {
                            throw new Exception(String.Format("Math Error in IsPrime {0} even number over 2 should never be prime", number.ToString()));
                        }
#endif
                        this.PrimeNumbers.Add(number, NumberTypes.Prime);

                        using (FileStream fs = File.Open(String.Format("../../../../Primes/{0}/{1}.txt", environment.Base, this.Started.Ticks), FileMode.Append))
                        {
                            using (StreamWriter sw = new StreamWriter(fs))
                            {
                                String primeRaw = number.GetActualValue();
                                Debug.WriteLine(String.Format("Prime Found:{0}", primeRaw));
                                sw.WriteLine(primeRaw);
                            }
                        }
                    }

                    this.MaxPrimeTested = number;

                }
            }

            if (basicMath.IsGreaterThan(environment, number, this.MaxPrimeTested))
            {
                return NumberTypes.Unknown;
            }
            else
            {
                if (this.PrimeCacheContains(number))
                {

                    return NumberTypes.Prime;
                }
                else
                {
                    return NumberTypes.Composite;
                }
            }
        }

        public Boolean PrimeCacheContains(NumberSegments number)
        {
            if (this.PrimeNumbers.GetNumberType(number) == NumberTypes.Prime)
            {
                return true;
            }
            return false;
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
                if (half.Item2 != default(NumberSegments) || half.Item2 != default(NumberSegments))
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
                    NumberSegments testNumber = basicMath.AsSegments(environment, 3);

                    while (basicMath.IsLessThanOrEqualTo(environment, testNumber, maxNumber))
                    {
                        NumberTypes testNumberumberType = this.GetNumberType(environment, basicMath, testNumber);

                        Tuple<NumberSegments, NumberSegments, NumberSegments> currentNumber = basicMath.Divide(environment, a, testNumber, lastResultWholeNumber);
                        if (currentNumber.Item2 == default(NumberSegments))
                        {
                            result = new Tuple<NumberSegments, NumberSegments>(currentNumber.Item1, testNumber);
                            break;
                        }

                        lastResultWholeNumber = currentNumber.Item1;

                        testNumber = basicMath.Add(environment, testNumber, environment.SecondNumber.Segments);
                    }
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

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
        public enum SavePrimesTypes
        {
            All,
            None,
            Segment1,
            Segment2,
            Segment3,
            Segment4,
            Segment5,
            Segment6,
            Segment7,
            Segment8,
            Segment9,
            Segment10
        }

        public readonly DateTime Started;
        public SavePrimesTypes SavePrimesType { get; set; }
        public NumberSegments PrimeListMaxNumber = new NumberSegments(new Decimal[] { 1 });
        public NumberSegmentDictionary PrimeNumbers = default(NumberSegmentDictionary);

        public SieveOfEratosthenePrimeAlgorithm(SavePrimesTypes savePrimes)
        {
            this.Started = DateTime.Now;
            this.SavePrimesType = savePrimes;
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
                this.PrimeNumbers = new NumberSegmentDictionary(null);

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
                    //Byte[] random = new Byte[1];
                    //using (var crypto = new RNGCryptoServiceProvider())
                    //{
                    //    crypto.GetBytes(random);
                    //}
                    //Thread.Sleep(((Int32)random[0]) * 3);
                    if (!File.Exists(String.Format("{0}{1}.txt", primeCacheFolder, this.Started.Ticks)))
                    {
                        File.Copy(String.Format("{0}Primes.txt", primeCacheFolder), String.Format("{0}{1}.txt", primeCacheFolder, this.Started.Ticks));
                    }
                }
                List<NumberSegments> uncheckedPrimes = new List<NumberSegments>();

                using (FileStream fs = File.Open(String.Format("{0}{1}.txt", primeCacheFolder, this.Started.Ticks), FileMode.OpenOrCreate))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        while (sr.Peek() >= 0)
                        {
                            uncheckedPrimes.Add(new NumberSegments(sr.ReadLine().Split(',').Reverse().Select(x => Decimal.Parse(x)).ToArray()));
                        }
                    }
                }

#if DEBUG
                var badPrimes = new List<NumberSegments>();

                Parallel.ForEach(uncheckedPrimes, new ParallelOptions() { MaxDegreeOfParallelism = -1 }, (NumberSegments uncheckedPrime) => {
                    var doubleCheckAlgorithm = new IterativePrimeAlgorithm(environment);
                    if (doubleCheckAlgorithm.IsPrime(environment, basicMath, uncheckedPrime))
                    {
                        lock (this.PrimeNumbers)
                        {
                            this.PrimeNumbers.Add(uncheckedPrime, NumberTypes.Prime);
                        }
                    }
                    else
                    {
                        lock (this.PrimeNumbers)
                        {
                            badPrimes.Add(uncheckedPrime);
                        }
                    }
                });

                Debug.Write(String.Format("{0} badPrimes", badPrimes.Count));
                foreach (NumberSegments numberSegments in uncheckedPrimes)
                {
                    if (basicMath.IsGreaterThan(environment, numberSegments, this.PrimeListMaxNumber))
                    {
                        this.PrimeListMaxNumber = numberSegments;
                    }
                }
                Debug.Write(String.Format("Top Saved Prime: {0}", this.PrimeListMaxNumber));

#else
                foreach (NumberSegments numberSegments in uncheckedPrimes)
                {
                    this.PrimeNumbers.Add(number, NumberTypes.Prime);
                    if (basicMath.IsGreaterThan(environment, numberSegments, this.PrimeListMaxNumber))
                    {
                        this.PrimeListMaxNumber = numberSegments;
                    }
                }
#endif


                using (FileStream fs = File.Open(String.Format("{0}supplemental.txt", primeCacheFolder), FileMode.OpenOrCreate))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        while (sr.Peek() >= 0)
                        {
                            String csv = sr.ReadLine();
                            var supplementalPrime = new NumberSegments(csv.Split(',').Reverse().Select(x => Decimal.Parse(x)).ToArray());

#if DEBUG
                            var doubleCheckAlgorithm = new IterativePrimeAlgorithm(environment);
                            if (doubleCheckAlgorithm.IsPrime(environment, basicMath, supplementalPrime))
                            {
                                this.PrimeNumbers.Add(supplementalPrime, NumberTypes.Prime);
                            }
#else
                            this.PrimeNumbers.Add(supplementalPrime, NumberTypes.Prime);
#endif

                        }
                    }
                }

            }

            if (basicMath.IsGreaterThan(environment, number, environment.SecondNumber.Segments) && basicMath.IsEven(environment, number))
            {
                return NumberTypes.Composite;
            }

            NumberTypes currentNumberType = this.PrimeNumbers.GetNumberType(number);
            if (currentNumberType != NumberTypes.Unknown || this.SavePrimesType == SavePrimesTypes.None)
            {
                return currentNumberType;
            }
            else if (basicMath.IsLessThanOrEqualTo(environment, number, this.PrimeListMaxNumber))
            {
                return NumberTypes.Composite;
            }
            
            Tuple<NumberSegments, NumberSegments> numberComposite = this.GetComposite(environment, basicMath, number);
            if (numberComposite == default(Tuple<NumberSegments, NumberSegments>))
            {
#if DEBUG
                    if (basicMath.IsGreaterThan(environment, number, environment.SecondNumber.Segments) && basicMath.IsEven(environment, number))
                    {
                        throw new Exception(String.Format("Math Error in IsPrime {0} even number over 2 should never be prime", number.ToString()));
                    }
#endif

                if (this.SavePrimesType == SavePrimesTypes.All)
                {
                    while (basicMath.IsLessThan(environment, this.PrimeListMaxNumber, basicMath.Subtract(environment, number, environment.KeyNumber[1].Segments)))
                    { 
                        Tuple<NumberSegments, NumberSegments> backFillNumber = this.GetComposite(environment, basicMath, this.PrimeListMaxNumber);
                        if (backFillNumber == default(Tuple<NumberSegments, NumberSegments>))
                        {
                            this.SavePrime(environment, this.PrimeListMaxNumber);
                        }
                        this.PrimeListMaxNumber = basicMath.Add(environment, this.PrimeListMaxNumber, new NumberSegments(new Decimal[] { 1 }));
                    }
                }

                this.SavePrime(environment, number);
                return NumberTypes.Prime;
            }
            else
            {
                return NumberTypes.Composite;
            }
        }

        public void SavePrimes(IMathEnvironment environment)
        {
            using (FileStream fs = File.Open(String.Format("../../../../Save/{0}/{1}.txt", environment.Base, this.Started.Ticks), FileMode.Append))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    foreach (var prime in this.PrimeNumbers.ToList(NumberTypes.Prime))
                    {
                        String primeRaw = prime.GetActualValue();
                        sw.WriteLine(primeRaw);
                    }
                }
            }
        }

        public void SavePrime(IMathEnvironment environment, NumberSegments number)
        {
            this.PrimeNumbers.Add(number, NumberTypes.Prime);

            if (this.SavePrimesType == SavePrimesTypes.All
                || (this.SavePrimesType == SavePrimesTypes.Segment1 && number.Length == 1)
                || (this.SavePrimesType == SavePrimesTypes.Segment2 && number.Length == 2)
                || (this.SavePrimesType == SavePrimesTypes.Segment3 && number.Length == 3)
                || (this.SavePrimesType == SavePrimesTypes.Segment4 && number.Length == 4)
                || (this.SavePrimesType == SavePrimesTypes.Segment5 && number.Length == 5)
                || (this.SavePrimesType == SavePrimesTypes.Segment6 && number.Length == 6)
                || (this.SavePrimesType == SavePrimesTypes.Segment7 && number.Length == 7)
                || (this.SavePrimesType == SavePrimesTypes.Segment8 && number.Length == 8)
                || (this.SavePrimesType == SavePrimesTypes.Segment9 && number.Length == 9)
                || (this.SavePrimesType == SavePrimesTypes.Segment10 && number.Length == 10))
            {
                try
                {
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
                catch (Exception ex)
                {
                    Thread.Sleep(100);
                    this.SavePrime(environment, number);
                }
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

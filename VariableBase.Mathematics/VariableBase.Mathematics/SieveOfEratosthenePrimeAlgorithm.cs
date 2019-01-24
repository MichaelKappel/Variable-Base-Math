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
    ///  Based on the Sieve Of Eratosthene
    ///  By: Michael Kappel, MCPD
    ///  Date: 1/23/2019
    /// </summary>
    public class SieveOfEratosthenePrimeAlgorithm: IPrimeAlgorithm
    {
        internal IMathEnvironment Environment;
        internal IBasicMathAlgorithm BasicMath;

        public readonly DateTime Started;
        public ReadOnlyCollection<Double> MaxPrimeTested = new ReadOnlyCollection<Double>(new Double[] { 1 });
        public Double[][] PrimeNumbers = default(Double[][]);

        public SieveOfEratosthenePrimeAlgorithm(DecimalMathEnvironment environment)
        {
            this.Started = DateTime.Now;
            this.Environment = environment;
            this.BasicMath = environment.BasicMath;
        }

        public enum NumberTypes
        {
            Unknown,
            Composite,
            Prime
        }

        public Boolean SavePrimes { get; set; }

        public Boolean IsPrime(ReadOnlyCollection<Double> number)
        {
            NumberTypes numberType = this.GetNumberType(number);
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
                Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>> numberComposite = this.GetComposite(number);
                if (numberComposite == default(Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>>))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        public NumberTypes GetNumberType(ReadOnlyCollection<Double> number)
        {
            String primeCacheFolder = String.Format("../../../../Primes/{0}/", this.Environment.Base);
            if (this.PrimeNumbers == default(Double[][]))
            {
                var primeNumbers = new List<Double[]>();
                primeNumbers.Add(this.BasicMath.AsSegments(2).ToArray());
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

                using (FileStream fs = File.Open(String.Format("{0}{1}.txt", primeCacheFolder, this.Started.Ticks), FileMode.OpenOrCreate))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        while (sr.Peek() >= 0)
                        {
                            String csv = sr.ReadLine();
                            Double[] prime = csv.Split(',').Select(x => Double.Parse(x)).ToArray();
                            if (!this.PrimesContain(primeNumbers, prime))
                            {
#if DEBUG
                                Boolean isPrime = true;
                                Double halfPrime = prime[0] / 2;
                                for (Int32 i = 2; i < halfPrime; i++)
                                {
                                    if (prime[0] % i == 0)
                                    {
                                        isPrime = false;
                                        break;
                                    }
                                }

                                if (!isPrime)
                                {
                                    throw new Exception(String.Format("Bad data in prime file: {0}", prime[0]));
                                }
                                Debug.WriteLine(String.Format("Verified prime: {0}", prime[0]));
#endif
                                primeNumbers.Add(prime);
                            }
                        }
                    }
                }
                this.PrimeNumbers = primeNumbers.ToArray();
                this.MaxPrimeTested = this.GetTopPrime();
            }

            if (this.SavePrimes)
            {
                while (this.BasicMath.IsGreaterThan(number, this.MaxPrimeTested))
                {
                    this.MaxPrimeTested = this.BasicMath.Add(this.MaxPrimeTested, this.Environment.KeyNumber[1].Segments);

                    Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>> numberComposite = this.GetComposite(this.MaxPrimeTested);
                    if (numberComposite == default(Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>>))
                    {
                        if (this.BasicMath.IsGreaterThan(this.MaxPrimeTested, this.Environment.SecondNumber.Segments) && this.BasicMath.IsEven(this.MaxPrimeTested))
                        {
                            throw new Exception(String.Format("Math Error in IsPrime {0} even number over 2 should never be prime", String.Join(',', this.MaxPrimeTested)));
                        }

                        IList<Double[]> tempList = this.PrimeNumbers.ToList();
                        tempList.Add(this.MaxPrimeTested.ToArray());
                        this.PrimeNumbers = tempList.ToArray();

                        using (FileStream fs = File.Open(String.Format("../../../../Primes/{0}/{1}.txt", this.Environment.Base, this.Started.Ticks), FileMode.Append))
                        {
                            using (StreamWriter sw = new StreamWriter(fs))
                            {
                                String primeRaw = String.Join(',', this.MaxPrimeTested);
                                Debug.WriteLine(String.Format("Prime Found:{0}", primeRaw));
                                sw.WriteLine(primeRaw);
                            }
                        }
                    }
                }
            }

            if (this.BasicMath.IsGreaterThan(number, this.MaxPrimeTested))
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

        public Boolean PrimesContain(IList<Double[]> primes, Double[] number)
        {
            for (Int32 i = 0; i < primes.Count; i++)
            {
                if (primes[i].Length == number.Length)
                {
                    for (Int32 i2 = 0; i2 < primes[i].Length; i2++)
                    {
                        if (primes[i][i2] != number[i2])
                        {
                            break;
                        }
                        if (i2 == primes[i2].Length)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public Boolean PrimeCacheContains(ReadOnlyCollection<Double> number)
        {
            for (Int32 i = 0; i < this.PrimeNumbers.Length; i++)
            {
                if (this.PrimeNumbers[i].Length == number.Count)
                {
                    for (Int32 i2 = 0; i2 < this.PrimeNumbers[i].Length; i2++)
                    {
                        if (this.PrimeNumbers[i][i2] != number[i2])
                        {
                            break;
                        }
                        if (i2 == this.PrimeNumbers[i2].Length)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public ReadOnlyCollection<Double> GetTopPrime()
        {
            Double[] result = this.PrimeNumbers[0];

            foreach (Double[] prime in this.PrimeNumbers)
            {
                result = prime;
            }

            return new ReadOnlyCollection<Double>(result);
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

                ReadOnlyCollection<Double> maxNumber;
                Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>, ReadOnlyCollection<Double>> maxNumberRaw = this.BasicMath.SquareRoot(a);
                if (maxNumberRaw.Item2 == default(ReadOnlyCollection<Double>))
                {
                    result = new Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>>(maxNumberRaw.Item1, maxNumberRaw.Item1);
                }
                else
                {
                    maxNumber = this.BasicMath.Add(maxNumberRaw.Item1, this.Environment.KeyNumber[1].Segments);

                    ReadOnlyCollection<Double> lastResultWholeNumber = default(ReadOnlyCollection<Double>);
                    ReadOnlyCollection<Double> testNumber = new ReadOnlyCollection<Double>(this.BasicMath.AsSegments(3));

                    while (this.BasicMath.IsLessThanOrEqualTo(testNumber, maxNumber))
                    {
                        NumberTypes testNumberumberType = this.GetNumberType(testNumber);
                        if (testNumberumberType == NumberTypes.Prime || testNumberumberType == NumberTypes.Unknown)
                        {
                            Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>, ReadOnlyCollection<Double>> currentNumber = this.BasicMath.Divide(a, testNumber, lastResultWholeNumber);
                            if (currentNumber.Item2 == default(ReadOnlyCollection<Double>))
                            {
                                result = new Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>>(currentNumber.Item1, testNumber);
                                break;
                            }
                            lastResultWholeNumber = currentNumber.Item1;
                        }
                        testNumber = this.BasicMath.Add(testNumber, Environment.SecondNumber.Segments);
                    }
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

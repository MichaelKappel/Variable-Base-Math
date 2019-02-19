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
        public IDictionary<Int32, Tuple<IList<Number>, NumberSegmentDictionary>> PrimeNumberTree = new Dictionary<Int32, Tuple<IList<Number>, NumberSegmentDictionary>>();
        public Action<Number> OnPrimeFound;


        public SieveOfEratosthenePrimeAlgorithm(IList<Number> seedPrimeNumbers, Action<Number> onPrimeFound = default(Action<Number>))
        {
            foreach (Number prime in seedPrimeNumbers)
            {
                Int32 environmentBase = (Int32)prime.Environment.Base;
                if (!this.PrimeNumberTree.ContainsKey(environmentBase))
                {
                    this.PrimeNumberTree.Add(environmentBase, new Tuple<IList<Number>, NumberSegmentDictionary>(new List<Number>(), new NumberSegmentDictionary(null)));
                }

                this.PrimeNumberTree[environmentBase].Item1.Add(prime);
                this.PrimeNumberTree[environmentBase].Item2.Add(prime.Segments, NumberTypes.Prime);

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
            this.PrimeNumberTree.Add((Int32)environment.Base, new Tuple<IList<Number>, NumberSegmentDictionary>(new List<Number>(), new NumberSegmentDictionary(null)));

            for (var i = 0; i < primeNumbersRaw.Count; i++)
            {
                Number prime = environment.GetNumber(primeNumbersRaw[i]);
                this.PrimeNumberTree[(Int32)environment.Base].Item1.Add(prime);
                this.PrimeNumberTree[(Int32)environment.Base].Item2.Add(prime.Segments, NumberTypes.Prime);

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
                Tuple<Number, Number> numberComposite = this.GetComposite(number);
                if (numberComposite == default(Tuple<Number, Number>))
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

            NumberTypes currentNumberType = this.PrimeNumberTree[(Int32)environment.Base].Item2.GetNumberType(number.Segments);
            if (currentNumberType == NumberTypes.Unknown &&  number < this.PrimeListMaxNumber)
            {
                return NumberTypes.Composite;
            }
            else if (currentNumberType != NumberTypes.Unknown)
            {
                return currentNumberType;
            }
            
            Tuple<Number, Number> numberComposite = this.GetComposite(number);
            if (numberComposite == default(Tuple<Number, Number>))
            {
                this.PrimeNumberTree[(Int32)environment.Base].Item2.Add(number.Segments, NumberTypes.Prime);
                this.PrimeNumberTree[(Int32)environment.Base].Item1.Add(number);
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

        public Tuple<Number, Number> GetComposite(Number a)
        {
            Tuple<Number, Number> result = default(Tuple<Number, Number>);
            IMathEnvironment<Number> environment = a.Environment;
            if (Number.IsBottom(a) || Number.IsFirst(a) || a == environment.GetNumber(2) || a == environment.GetNumber(3))
            {

            }
            else if (a.IsEven())
            {
                result = new Tuple<Number, Number>(a / environment.GetNumber(2), environment.GetNumber(2)); 
            }
            else
            {
                Number maxNumberRaw = a.SquareRoot();
                if (maxNumberRaw.Fragment == default(Fraction))
                {
                    result = new Tuple<Number, Number>(maxNumberRaw, maxNumberRaw);
                }
                else
                {
                    Number maxNumber =  a + environment.GetNumber(1);

                    Number lastResultWholeNumber = default(Number);

                    Int32 iPrime = 1;

                    Boolean usePrime = true;

                    Number currentNumberToTry = this.PrimeNumberTree[(Int32)environment.Base].Item1[iPrime];
                    while (currentNumberToTry > maxNumber)
                    {
                        Number currentNumber = Number.Operator.Divide(a, currentNumberToTry, lastResultWholeNumber);
                        if (currentNumber.Fragment == default(Fraction))
                        {
                            result = new Tuple<Number, Number>(currentNumber, currentNumberToTry);
                            break;
                        }
                        lastResultWholeNumber = environment.GetNumber(currentNumber.Segments);

                        if (usePrime)
                        {
                            iPrime++;
                            if (iPrime >= this.PrimeNumberTree[(Int32)environment.Base].Item1.Count)
                            {
                                usePrime = false;
                            }

                            if (usePrime)
                            {
                                var nextNumberToTry = this.PrimeNumberTree[(Int32)environment.Base].Item1[iPrime];
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

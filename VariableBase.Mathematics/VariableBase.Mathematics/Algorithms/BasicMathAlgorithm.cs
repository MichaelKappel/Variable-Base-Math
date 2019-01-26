using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Threading;
using VariableBase.Mathematics.Interfaces;
using VariableBase.Mathematics.Models;

namespace VariableBase.Mathematics
{
    public class BasicMathAlgorithm : IBasicMathAlgorithm
    {

        public Number AsFraction(IMathEnvironment environment, UInt64 numberRaw, UInt64 numeratorNumber, UInt64 denominatorRaw)
        {
            Number result;
            NumberSegments number = this.AsSegments(environment, numberRaw);
            if (numeratorNumber > 0)
            {
                NumberSegments numerator = this.AsSegments(environment, numeratorNumber);
                NumberSegments denominator = this.AsSegments(environment, denominatorRaw);

                result = new Number(environment, number, numerator, denominator, false);
            }
            else
            {
                result = new Number(environment, number, default(Fraction), false);
            }
            return result;
        }


        #region Add
        public NumberSegments Add(IMathEnvironment environment, Decimal a, Decimal b)
        {
            Decimal resultRaw = a + b;

            return this.AsSegments(environment, resultRaw);
        }

        public NumberSegments Add(IMathEnvironment environment, NumberSegments a, NumberSegments b)
        {
            NumberSegments result = default(NumberSegments);

            if (this.IsLessThan(environment,a, b))
            {
                result = this.Add(environment,b, a);
            }

            if (a.Size == 1 && b.Size == 1)
            {
                result = this.Add(environment, a[0], b[0]);
            }

            if (result == default(NumberSegments))
            {
                var resultNumber = new List<Decimal>();
                Decimal carryOver = 0;
                Int32 position = 0;
                while (position < a.Size)
                {
                    Decimal columnValue = carryOver;

                    if (position < a.Size)
                    {
                        columnValue += a[position];
                    }

                    if (position < b.Size)
                    {
                        columnValue += b[position];
                    }

                    Decimal columnResult;
                    if (columnValue >= environment.Base)
                    {
                        Decimal columnResultRaw = columnValue % environment.Base;
                        columnResult = columnResultRaw;

                        carryOver = ((columnValue - columnResultRaw) / environment.Base);
                    }
                    else
                    {
                        columnResult = columnValue;
                        carryOver = 0;
                    }

                    resultNumber.Add(columnResult);
                    position++;
                }

                if (carryOver != 0)
                {
                    Decimal columnResult;
                    while (carryOver >= environment.Base)
                    {
                        Decimal columnResultRaw = carryOver % environment.Base;
                        columnResult = columnResultRaw;
                        resultNumber.Add(columnResult);

                        carryOver = columnResultRaw / environment.Base;
                    }

                    if (carryOver > 0)
                    {
                        columnResult = carryOver;
                        resultNumber.Add(columnResult);
                    }
                }

                result = new NumberSegments(resultNumber);
            }

#if DEBUG
            if (this.IsLessThan(environment,result, a) || this.IsLessThan(environment,result, b))
            {
                throw new Exception("MathAlgorithm addtion error");
            }

            foreach (Decimal segment in result)
            {
                if (segment > environment.Base)
                {
                    throw new Exception("Bad addtion segment larger than base");
                }
                else if (segment < 0)
                {
                    throw new Exception("Bad addition segment less than zero");
                }
                else if (segment % 1 != 0)
                {
                    throw new Exception("Bad addition segment has remainder");
                }
            }

            NumberSegments reversResult = this.Subtract(environment,result, a);
            if (this.IsNotEqual(environment, reversResult, b))
            {
                throw new Exception("Bad addition result could not reverse");
            }
#endif
            return result;
        }

        #endregion

        public NumberSegments GetWholeNumberSomewhereBetween(IMathEnvironment environment, NumberSegments a, NumberSegments b, Decimal variance = 0)
        {
            NumberSegments result;

            if (a == b)
            {
                result = a;
            }
            else
            {

                NumberSegments largerNumber;
                NumberSegments smallerNumber;

                if (this.IsGreaterThan(environment,a, b))
                {
                    largerNumber = a;
                    smallerNumber = b;
                }
                else
                {
                    largerNumber = b;
                    smallerNumber = a;
                }

                Decimal firstIndexOfLargerNumber = largerNumber[largerNumber.Size - 1];
                Decimal firstIndexOfSmallerNumber = smallerNumber[smallerNumber.Size - 1];

                Decimal firstIndexOfResultRaw = (firstIndexOfLargerNumber + firstIndexOfSmallerNumber) / 2M;

                Decimal firstIndexOfResult;
                Decimal halfBase;

                if (variance > 0)
                {
                    halfBase = (environment.Base) / 2;
                    firstIndexOfResult = System.Math.Ceiling(firstIndexOfResultRaw);
                }
                else
                {
                    halfBase = (environment.Base) / 2;
                    firstIndexOfResult = System.Math.Floor(firstIndexOfResultRaw);
                }



                if ((largerNumber.Size - smallerNumber.Size <= 1)
                    || (largerNumber.Size - smallerNumber.Size == 2 && firstIndexOfResult <= 1))
                {
                    NumberSegments combinedValue = this.Add(environment,largerNumber, smallerNumber);
                    result = this.GetAboutHalf(environment, combinedValue, variance);
                }
                else
                {
                    Decimal somewhereBetweenPower = ((largerNumber.Size - smallerNumber.Size) / 2M) + smallerNumber.Size;
                   
                    Decimal power;
                    if (variance > 0)
                    {
                        power = System.Math.Ceiling(somewhereBetweenPower);
                    }
                    else
                    {
                        power = System.Math.Floor(somewhereBetweenPower);
                    }
                    Decimal powerWithVariance = (power + variance);
                    result = this.PowerOfBase(environment, firstIndexOfResult, powerWithVariance);
                    while (this.IsGreaterThan(environment,result, a))
                    {
                        powerWithVariance -= 1;
                        result = this.PowerOfBase(environment, firstIndexOfResult, powerWithVariance);
                    }
                }
            }
#if DEBUG
            if (this.IsGreaterThan(environment,a, b) && (this.IsGreaterThan(environment,result, a) || this.IsLessThan(environment,result, b)))
            {
                throw new Exception("MathAlgorithm GetWholeNumberSomewhereBetween error 1");
            }
            else if (this.IsLessThan(environment,a, b) && (this.IsGreaterThan(environment,result, b) || this.IsLessThan(environment,result, a)))
            {
                throw new Exception("MathAlgorithm GetWholeNumberSomewhereBetween Error 2");
            }

            foreach (Decimal segment in result)
            {
                if (segment > environment.Base)
                {
                    throw new Exception("Bad GetWholeNumberSomewhereBetween segment larger than base");
                }
                else if (segment < 0)
                {
                    throw new Exception("Bad GetWholeNumberSomewhereBetween segment less than zero");
                }
            }

#endif
            return result;
        }

        public NumberSegments GetAboutHalf(IMathEnvironment environment, NumberSegments number, Decimal variance)
        {
            Decimal halfFirstCharIndexDetail = (number[number.Size - 1]) / 2M;

            Decimal halfBaseIndexDetailed = (environment.Base) / 2M;

            Decimal[] resultSegments;

            Decimal remainder = 0M;


            if (halfFirstCharIndexDetail >= 1)
            {
                resultSegments = new Decimal[number.Length];
            }
            else
            {
                resultSegments = new Decimal[number.Length - 1];
                remainder = halfBaseIndexDetailed;
            }

            for (var i = resultSegments.Length - 1; i >= 0; i--)
            {
                Decimal charIndex = number[i];
                Decimal halfCharIndexWithRemainder = (charIndex / 2M) + remainder;
                
                if (i == 0)
                {
                    if (variance > 0 && System.Math.Ceiling(halfCharIndexWithRemainder) < environment.Base)
                    {
                        resultSegments[0] = System.Math.Ceiling(halfCharIndexWithRemainder);
                    }
                    else
                    {
                        resultSegments[0] = System.Math.Floor(halfCharIndexWithRemainder);
                    }
                }
                else
                {
                    Decimal halfCharIndexWithRemainderIndex = System.Math.Floor(halfCharIndexWithRemainder);
                    if (halfCharIndexWithRemainderIndex >= environment.Base)
                    {
                        Decimal currentSegmentIndex = System.Math.Floor(halfBaseIndexDetailed);
                        resultSegments[i] = currentSegmentIndex;
                        remainder = 0M;
                    }
                    else
                    {
                        resultSegments[i] = halfCharIndexWithRemainderIndex;
                        remainder = (halfCharIndexWithRemainder - halfCharIndexWithRemainderIndex) * environment.Base;
                    }
                }
            }

            while (resultSegments[resultSegments.Length-1] == 0)
            {
                resultSegments = resultSegments.Take(resultSegments.Length - 1).ToArray();
            }
            var result = new NumberSegments(resultSegments);

#if DEBUG
            if (this.IsGreaterThan(environment,result, number))
            {
                throw new Exception("MathAlgorithm GetAboutHalf error");
            }
            foreach (Decimal segment in result)
            {
                if (segment > environment.Base)
                {
                    throw new Exception("Bad GetAboutHalf segment larger than base");
                }
                else if (segment < 0)
                {
                    throw new Exception("Bad GetAboutHalf segment less than zero");
                }
            }
#endif

            return result;
        }

        public NumberSegments GetAboutHalf(IMathEnvironment environment, NumberSegments a, NumberSegments b, Decimal variance)
        {
            NumberSegments x = this.Add(environment,a, b);
            Tuple<NumberSegments,NumberSegments,NumberSegments> rawResult = this.Divide(environment,x, environment.SecondNumber.Segments);

            NumberSegments result;
            if (variance == 1 && rawResult.Item2 != default(NumberSegments))
            {
                result = this.Add(environment,rawResult.Item1, environment.KeyNumber[1].Segments);
            }
            else if (variance == -1 || rawResult.Item2 == default(NumberSegments))
            {
                result = rawResult.Item1;
            }
            else
            {
                NumberSegments doubleNumerator = this.Multiply(environment,rawResult.Item2, 2);
                if (this.IsGreaterThanOrEqualTo(environment, doubleNumerator, rawResult.Item2))
                {
                    result = this.Add(environment,rawResult.Item1, environment.KeyNumber[1].Segments);
                }
                else
                {
                    result = rawResult.Item1;
                }
            }

#if DEBUG
            if (this.IsGreaterThan(environment,a, b) && (this.IsGreaterThan(environment,result, a) || this.IsLessThan(environment,result, b)))
            {
                throw new Exception("MathAlgorithm GetWholeNumberSomewhereBetween error 1");
            }
            else if (this.IsLessThan(environment,a, b) && (this.IsLessThan(environment,result, a) || this.IsGreaterThan(environment,result, b)))
            {
                throw new Exception("MathAlgorithm GetWholeNumberSomewhereBetween Error 2");
            }

            foreach (Decimal segment in result)
            {
                if (segment > environment.Base)
                {
                    throw new Exception("Bad GetWholeNumberSomewhereBetween segment larger than base");
                }
                else if (segment < 0)
                {
                    throw new Exception("Bad GetWholeNumberSomewhereBetween segment less than zero");
                }
            }
#endif

            return result;
        }

        public NumberSegments AsSegments(IMathEnvironment environment, Decimal rawDouble)
        {
            var resultRaw = new List<Decimal>();
            if (rawDouble == 0)
            {
                resultRaw.Add(0);
            }
            else
            {

                Decimal carryOver = rawDouble;
                while (carryOver > 0)
                {
                    if (carryOver >= environment.Base)
                    {
                        Decimal columnResultRaw = 0;
                        columnResultRaw = carryOver % environment.Base;
                        resultRaw.Add(columnResultRaw);
                        carryOver = ((carryOver - columnResultRaw) / environment.Base);
                    }
                    else
                    {
                        resultRaw.Add(carryOver);
                        carryOver = 0;
                    }
                }
            }
            return new NumberSegments(resultRaw);
        }

        public NumberSegments PowerOfBase(IMathEnvironment environment, Decimal a, Decimal times)
        {
            return this.PowerOfBase(environment,  a , times);
        }

        public NumberSegments PowerOfBase(IMathEnvironment environment, NumberSegments a, Decimal times)
        {
            if (a.Size == 1 && a[0] == 0)
            {
                return a;
            }

            var segments = new Decimal[(Int32)(a.Size + times)];
            for (Int32 i = segments.Length - 1; i >= 0; i--)
            {
                segments[i] = 0;
            }
            a.CopyTo(segments, times);
#if DEBUG
            foreach (Decimal segment in segments)
            {
                if (segment > environment.Base)
                {
                    throw new Exception("Bad PowerOfBase segment larger than base");
                }
                else if (segment < 0)
                {
                    throw new Exception("Bad PowerOfBase segment less than zero");
                }
            }
#endif
            return new NumberSegments(segments);
        }

        public bool IsOdd(IMathEnvironment environment, NumberSegments a)
        {
            return !this.IsEven(environment, a);
        }

        public bool IsEven(IMathEnvironment environment, NumberSegments a)
        {
            Boolean isEven = false;
            if (environment.Base % 2 == 0 || a.Size == 1)
            {
                if (a[0] % 2 == 0)
                {
                    isEven = true;
                }
                else
                {
                    isEven = false;
                }
            }
            else
            {
                Tuple<NumberSegments, NumberSegments, NumberSegments> half = this.Divide(environment, a, environment.SecondNumber.Segments);
                if (half.Item2 == default(NumberSegments))
                {
                    isEven = true;
                }
                else
                {
                    isEven = false;
                }

                //Decimal x = 0;
                //for (var i = 0; i < a.Count; i++)
                //{
                //    if (a[i] % 2 != 0)
                //    {
                //        x++;
                //    }
                //}
                //if (x % 2 == 0 && a.Count % 2 != 0)
                //{
                //    isEven = false;
                //}
                //else if (x % 2 != 0 && a.Count % 2 == 0)
                //{
                //    isEven = false;
                //}
                //else
                //{
                //    isEven = true;
                //}

                    //if (a[0] % 2 == 0 && a[1] % 2 == 0)
                    //{
                    //    return true;
                    //}
                    //else if(a[0] % 2 != 0 && a[1] % 2 != 0)
                    //{
                    //    return true;
                    //}
                    //else
                    //{
                    //    return false;
                    //}
            }
//#if DEBUG
//            Tuple<NumberSegments, NumberSegments, NumberSegments> half = this.Divide(environment,a, environment.SecondNumber.Segments);
//            if (half.Item2 == default(NumberSegments) && !isEven)
//            {
//                throw new Exception("IsEven should be even but is not");
//            }
//            else if(half.Item2 != default(NumberSegments) && isEven)
//            {
//                throw new Exception("IsEven should NOT be even but is");
//            }
//#endif
            return isEven;
        }
        
        public NumberSegments Square(IMathEnvironment environment, NumberSegments a)
        {
            return this.Multiply(environment,a, a);
        }

        public Tuple<NumberSegments, NumberSegments, NumberSegments> SquareRoot(IMathEnvironment environment, Decimal number)
        {
            Decimal x = (Decimal)System.Math.Sqrt((Double)number);
            if (x % 1 == 0)
            {
                return new Tuple<NumberSegments, NumberSegments, NumberSegments>(new NumberSegments(new Decimal[] { x }), null, null);
            }
            else
            {
                Decimal remainder = (x % 1);
                return new Tuple<NumberSegments, NumberSegments, NumberSegments>(new NumberSegments(new Decimal[] { Math.Floor(x) }), new NumberSegments(new Decimal[] { Math.Floor(remainder * 100000) }), new NumberSegments(new Decimal[] { number * 100000 } ));

            }
        }

        public Tuple<NumberSegments, NumberSegments, NumberSegments> SquareRoot(IMathEnvironment environment, NumberSegments number)
        {

            if (this.IsBottom(number))
            {
                return default(Tuple<NumberSegments, NumberSegments, NumberSegments>);
            }
            else if (number.Size == 1)
            {
                return this.SquareRoot(environment, number[0]);
            }
            else if (this.IsLessThanOrEqualTo(environment, number, this.AsSegments(environment, 3)))
            {
                return new Tuple<NumberSegments, NumberSegments, NumberSegments>(environment.KeyNumber[1].Segments, null, null);
            }
            
            NumberSegments floor = environment.SecondNumber.Segments;
            NumberSegments ceiling = this.Divide(environment,number, environment.SecondNumber.Segments).Item1;

            NumberSegments lastNumberTried = this.GetWholeNumberSomewhereBetween(environment, ceiling, floor);
            NumberSegments squareTestResult = this.Square(environment, lastNumberTried);

            NumberSegments maxDifference = this.Subtract(environment,lastNumberTried, new NumberSegments(new Decimal[] { 1 }));

            while (this.IsNotEqual(environment, squareTestResult, number) && this.IsGreaterThan(environment,this.Subtract(environment,ceiling, floor), environment.KeyNumber[1].Segments))
            {
                NumberSegments numberBeforLast = lastNumberTried;
                if (this.IsLessThan(environment,squareTestResult, number))
                {
                    floor = lastNumberTried;
                    lastNumberTried = this.GetWholeNumberSomewhereBetween(environment, ceiling, floor);
                    if(this.IsEqual(environment, numberBeforLast, lastNumberTried))
                    {
                        lastNumberTried = this.Add(environment,lastNumberTried, environment.KeyNumber[1].Segments);
                    }
                }
                else if (this.IsGreaterThan(environment,squareTestResult, number))
                {
                    ceiling = lastNumberTried;
                    lastNumberTried = this.GetWholeNumberSomewhereBetween(environment, ceiling, floor, -1);
                    if (this.IsEqual(environment, numberBeforLast, lastNumberTried))
                    {
                        lastNumberTried = this.Subtract(environment,lastNumberTried, environment.KeyNumber[1].Segments);
                    }
                }
                squareTestResult = this.Square(environment, lastNumberTried);
                
            }

            Tuple<NumberSegments, NumberSegments, NumberSegments> result;

            if (this.IsGreaterThan(environment,number, squareTestResult))
            {
                var leftOver = this.Subtract(environment,number, squareTestResult);
                result = new Tuple<NumberSegments, NumberSegments, NumberSegments>(lastNumberTried, leftOver, number);

            }
            else if (this.IsGreaterThan(environment,squareTestResult, number))
            {
                var wholeNumber = this.Subtract(environment,lastNumberTried, environment.KeyNumber[0].Segments);
                var leftOver = this.Subtract(environment,squareTestResult, number);
                result = new Tuple<NumberSegments, NumberSegments, NumberSegments>(wholeNumber, leftOver, number);
            }
            else
            {
                result = new Tuple<NumberSegments, NumberSegments, NumberSegments>(lastNumberTried, null, null);
            }
#if DEBUG
            foreach (Decimal segment in result.Item1)
            {
                if (segment > environment.Base)
                {
                    throw new Exception("Bad SuareRoot segment larger than base Item 1");
                }
                else if (segment < 0)
                {
                    throw new Exception("Bad SuareRoot  segment less than zero Item 1");
                }
                else if (segment % 1 != 0)
                {
                    throw new Exception("Bad SuareRoot segment has remainder");
                }
            }

            if (result.Item2 != default(NumberSegments))
            {
                foreach (Decimal segment in result.Item2)
                {
                    if (segment > environment.Base)
                    {
                        throw new Exception("Bad SuareRoot segment larger than base Item 2");
                    }
                    else if (segment < 0)
                    {
                        throw new Exception("Bad SuareRoot  segment less than zero Item 2");
                    }
                    else if (segment % 1 != 0)
                    {
                        throw new Exception("Bad SuareRoot segment has remainder Item 2");
                    }
                }

                foreach (Decimal segment in result.Item3)
                {
                    if (segment > environment.Base)
                    {
                        throw new Exception("Bad SuareRoot segment larger than base Item 3");
                    }
                    else if (segment < 0)
                    {
                        throw new Exception("Bad SuareRoot  segment less than zero Item 3");
                    }
                    else if (segment % 1 != 0)
                    {
                        throw new Exception("Bad SuareRoot segment has remainder Item 2");
                    }
                }
            }
#endif
            return result;
        }

        public Boolean IsEqual(IMathEnvironment environment, NumberSegments a, NumberSegments b)
        {
            if (this.CompareTo(environment, a, b) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean IsNotEqual(IMathEnvironment environment, NumberSegments a, NumberSegments b)
        {
            if (this.CompareTo(environment, a, b) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean IsGreaterThan(IMathEnvironment environment, NumberSegments a, NumberSegments b)
        {
            if (this.CompareTo(environment, a, b) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean IsLessThan(IMathEnvironment environment, NumberSegments a, NumberSegments b)
        {
            if (this.CompareTo(environment, a, b) < 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean IsGreaterThanOrEqualTo(IMathEnvironment environment, NumberSegments a, NumberSegments b)
        {
            if (this.CompareTo(environment, a, b) >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean IsLessThanOrEqualTo(IMathEnvironment environment, NumberSegments a, NumberSegments b)
        {
            if (this.CompareTo(environment, a, b) <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int CompareTo(IMathEnvironment environment, NumberSegments a, NumberSegments b)
        {
            Int32 result = 0;
            if (Object.ReferenceEquals(a, default(NumberSegments)) || a.Size == 0)
            {
                a = new NumberSegments(new Decimal[] { 0 });
            }

            if (Object.ReferenceEquals(b, default(NumberSegments)) || b.Size == 0)
            {
                b = new NumberSegments(new Decimal[] { 0 });
            }

            if (a.Size > b.Size)
            {
                result = 1;
            }
            else if (a.Size < b.Size)
            {
                result = -1;
            }

            if (result == 0)
            {
                for (var i = a.Size - 1; i >= 0; i--)
                {
                    if (a[i] != b[i])
                    {
                        if (a[i] > b[i])
                        {
                            result = 1;
                            break;
                        }
                        else if (a[i] < b[i])
                        {
                            result = -1;
                            break;
                        }
                    }
                }
            }
            return result;
        }




        #region Multiply

        public NumberSegments Multiply(IMathEnvironment environment, NumberSegments a, Decimal b)
        {
            if (b == 0)
            {
                return new NumberSegments(new Decimal[] { 0 });
            }
            var resultRaw = new List<Decimal>();

            Decimal numberIndex = b;

            Decimal carryOver = 0;
            for (var i = 0; i < a.Size; i++)
            {
                Decimal segmentIndex = a[i];

                Decimal columnTotal = (numberIndex * segmentIndex) + carryOver;

                Decimal columnPositionResult;
                if (columnTotal >= environment.Base)
                {
                    Decimal remainder = columnTotal % environment.Base;
                    columnPositionResult = remainder;
                    carryOver = (columnTotal - remainder) / environment.Base;
                }
                else
                {
                    columnPositionResult = columnTotal;
                    carryOver = 0;
                }

                resultRaw.Add(columnPositionResult);
            }

            while (carryOver > 0)
            {
                Decimal carryOverResult;
                if (carryOver > environment.Base)
                {
                    Decimal remainder = carryOver % environment.Base;
                    carryOverResult = remainder;
                    carryOver = (carryOver - remainder) / environment.Base;
                }
                else
                {
                    carryOverResult = carryOver;
                    carryOver = 0;
                }
                resultRaw.Add(carryOverResult);
            }
#if DEBUG
            foreach (Decimal segment in resultRaw)
            {
                if (segment > environment.Base)
                {
                    throw new Exception("Bad Multiply segment larger than base Item");
                }
                else if (segment < 0)
                {
                    throw new Exception("Bad Multiply segment less than zero Item");
                }
            }
#endif
            return new NumberSegments(resultRaw);
        }

        public NumberSegments Multiply(IMathEnvironment environment, NumberSegments a, NumberSegments b)
        {
            NumberSegments result = new NumberSegments(new Decimal[] { 0 });
            if (this.IsBottom(a) || this.IsBottom(b))
            {
                return a;
            }
            else if (this.IsFirst(a))
            {
                return b;
            }
            else if (this.IsFirst(b))
            {
                return a;
            }
            else if (a.Size == 1 && b.Size == 1)
            {
                return this.Multiply(environment, a[0], b[0]);
            }

            for (Int32 i = 0; i < a.Size; i++)
            {
                Decimal numberSegment = a[i];
                NumberSegments currentResult = this.Multiply(environment,b, numberSegment);

                if (i > 0)
                {
                    currentResult = this.PowerOfBase(environment, currentResult, i);
                }

                result = this.Add(environment,currentResult, result);
            }

#if DEBUG
            if (this.IsLessThan(environment,result, a) || this.IsLessThan(environment,result, b))
            {
                throw new Exception("MathAlgorithm Multiplication error");
            }
            else if (result.Size > 1 && result[result.Size - 1] == 0)
            {
                throw new Exception("MathAlgorithm Multiplication leading zero error");
            }

            foreach (Decimal segment in result)
            {
                if (segment > environment.Base)
                {
                    throw new Exception("Bad Multiply segment larger than base Item");
                }
                else if (segment < 0)
                {
                    throw new Exception("Bad Multiply segment less than zero Item");
                }
            }
#endif

            return result;
        }

        public NumberSegments Multiply(IMathEnvironment environment, Decimal a, Decimal b)
        {
            Decimal[] result;

            Decimal resultIndex = a * b;

            if (resultIndex >= environment.Base)
            {
                Decimal firstNumber =   resultIndex % environment.Base;
                Decimal secondNumber = (resultIndex - firstNumber) / environment.Base;

                result = new Decimal[] { firstNumber, secondNumber};
            }
            else
            {
                result = new Decimal[] { resultIndex };
            }

#if DEBUG
            foreach (Decimal segment in result)
            {
                if (segment > environment.Base)
                {
                    throw new Exception("Bad Multiply segment larger than base Item");
                }
                else if (segment < 0)
                {
                    throw new Exception("Bad Miltiply segment less than zero Item");
                }
            }
#endif
            return new NumberSegments(result);
        }

        #endregion

        #region Divide

        public Tuple<NumberSegments, NumberSegments, NumberSegments> Divide(IMathEnvironment environment, NumberSegments numerator, NumberSegments denominator, NumberSegments hint = default(NumberSegments))
        {

#if DEBUG
            Debug.WriteLine("Division start {0} / {1}", String.Join(',', numerator.Reverse()), String.Join(',', denominator.Reverse()));
            foreach (Decimal segment in numerator)
            {
                if (segment > environment.Base)
                {
                    throw new Exception("Bad Divide numerator larger than base");
                }
                else if (segment < 0)
                {
                    throw new Exception("Bad Divide numerator larger than zero Item 1");
                }
                else if (segment % 1 != 0)
                {
                    throw new Exception("Bad Divide numerator has remainder Item 1");
                }
            }

            foreach (Decimal segment in denominator)
            {
                if (segment > environment.Base)
                {
                    throw new Exception("Bad Divide denominator larger than base");
                }
                else if (segment < 0)
                {
                    throw new Exception("Bad Divide denominator larger than zero Item 1");
                }
                else if (segment % 1 != 0)
                {
                    throw new Exception("Bad Divide denominator has remainder Item 1");
                }
            }
#endif
            if (this.IsBottom(denominator))
            {
                return new Tuple<NumberSegments, NumberSegments, NumberSegments>(numerator, null, null);
            }
            else if (this.IsEqual(environment, numerator, denominator))
            {
                return new Tuple<NumberSegments, NumberSegments, NumberSegments>(new NumberSegments(new Decimal[] { 1 }), null, null);
            }
            else if (this.IsLessThan(environment,numerator, denominator))
            {
                return new Tuple<NumberSegments, NumberSegments, NumberSegments>(new NumberSegments(new Decimal[] { 0 }), numerator, denominator);
            }
            else if (numerator.Size == 1 && denominator.Size == 1)
            {
                return this.Divide(environment,numerator[0], denominator[0]);
            }
            else if (denominator.Size == 1)
            {
                return this.Divide(environment,numerator, denominator[0]);
            }

            NumberSegments floor = (hint == default(NumberSegments) || this.IsGreaterThan(environment,denominator, hint)) ? environment.KeyNumber[1].Segments : this.GetAboutHalf(environment, hint, environment.KeyNumber[1].Segments, -1);
            NumberSegments ceiling = (hint == default(NumberSegments) || this.IsGreaterThan(environment,denominator, hint)) ? numerator : this.GetAboutHalf(environment, hint, numerator, 1);

            NumberSegments lastNumberTried = this.GetWholeNumberSomewhereBetween(environment,  ceiling, floor);
            NumberSegments numeratorTestResult = this.Multiply(environment,lastNumberTried, denominator);

            NumberSegments maxDifference = this.Subtract(environment,denominator, new NumberSegments(new Decimal[] { 1 }));
            NumberSegments minimumTestResult = this.Subtract(environment,numerator, maxDifference);

            NumberSegments lastNumeratorTestResult = environment.KeyNumber[0].Segments;

            while (this.IsLessThan(environment,numeratorTestResult, minimumTestResult) || this.IsGreaterThan(environment,numeratorTestResult, numerator))
            { 
                lastNumeratorTestResult = numeratorTestResult;
                if (this.IsLessThan(environment,numeratorTestResult, numerator))
                {
                    floor = lastNumberTried;
                    lastNumberTried = this.GetWholeNumberSomewhereBetween(environment, ceiling, floor);
                }
                else if (this.IsGreaterThan(environment,numeratorTestResult, numerator))
                {
                    ceiling = lastNumberTried;
                    lastNumberTried = this.GetWholeNumberSomewhereBetween(environment, ceiling, floor, -1);
                }

                if (this.IsGreaterThanOrEqualTo(environment, floor, ceiling))
                {
                    floor = this.GetWholeNumberSomewhereBetween(environment, ceiling, environment.KeyNumber[1].Segments);
                }
                else if (this.IsLessThanOrEqualTo(environment, ceiling, floor))
                {
                    ceiling = this.GetWholeNumberSomewhereBetween(environment, floor, numerator);
                }

                numeratorTestResult = this.Multiply(environment,lastNumberTried, denominator);
            }

            Tuple<NumberSegments, NumberSegments, NumberSegments> result;

            if (this.IsEqual(environment, numeratorTestResult, numerator))
            {
                result = new Tuple<NumberSegments, NumberSegments, NumberSegments>(lastNumberTried, null, null);
            }
            else
            {
                var leftOver = this.Subtract(environment,numerator, numeratorTestResult);
                result = new Tuple<NumberSegments, NumberSegments, NumberSegments>(lastNumberTried, leftOver, denominator);
            }

#if DEBUG
            if (this.IsGreaterThan(environment,result.Item1, numerator) && this.IsGreaterThan(environment,result.Item1, denominator))
            {
                throw new Exception("MathAlgorithm Division error");
            }
            else if (result.Item1 != default(NumberSegments) && result.Item1.Size > 1 && result.Item1[result.Item1.Size - 1] == 0)
            {
                throw new Exception("MathAlgorithm Division leading zero error whole number");
            }
            else if (result.Item2 != default(NumberSegments) && result.Item2.Size > 1 && result.Item2[result.Item2.Size - 1] == 0)
            {
                throw new Exception("MathAlgorithm Division leading zero error numerator");
            }
            else if (result.Item3 != default(NumberSegments) && result.Item3.Size > 1 && result.Item3[result.Item3.Size - 1] == 0)
            {
                throw new Exception("MathAlgorithm Division leading zero error denominator");
            }

            foreach (Decimal segment in result.Item1)
            {
                if (segment > environment.Base)
                {
                    throw new Exception("Bad Divide segment larger than base Item 1");
                }
                else if (segment < 0)
                {
                    throw new Exception("Bad Divide  segment less than zero Item 1");
                }
                else if (segment % 1 != 0)
                {
                    throw new Exception("Bad Divide segment has remainder Item 1");
                }
            }

            if (result.Item2 != default(NumberSegments))
            {
                foreach (Decimal segment in result.Item2)
                {
                    if (segment > environment.Base)
                    {
                        throw new Exception("Bad Divide segment larger than base Item 2");
                    }
                    else if (segment < 0)
                    {
                        throw new Exception("Bad Divide  segment less than zero Item 2");
                    }
                }

                foreach (Decimal segment in result.Item3)
                {
                    if (segment > environment.Base)
                    {
                        throw new Exception("Bad Divide segment larger than base Item 3");
                    }
                    else if (segment < 0)
                    {
                        throw new Exception("Bad Divide  Divide less than zero Item 3");
                    }
                }
            }

            Debug.WriteLine("Division result {0}", String.Join(',', result.Item1.Reverse()));

            if (result.Item1 != default(NumberSegments))
            {
                Debug.WriteLine("Division remainder {0} / {1}", String.Join(',', result.Item1.Reverse()), String.Join(',', result.Item2.Reverse()));
            }
#endif
            return result;
        }


        public Tuple<NumberSegments, NumberSegments, NumberSegments> Divide(IMathEnvironment environment, Decimal dividend, Decimal divisor)
        {
            Tuple<NumberSegments, NumberSegments, NumberSegments> result;

            Decimal remainder;

            if (dividend > divisor)
            {
                remainder = (dividend % divisor);

                Decimal resultRaw = System.Math.Floor(dividend / divisor);

                if (remainder == 0)
                {
                    result = new Tuple<NumberSegments, NumberSegments, NumberSegments>(new NumberSegments(new Decimal[] { resultRaw }), null, null);
                }
                else
                {
                    result = new Tuple<NumberSegments, NumberSegments, NumberSegments>(new NumberSegments(new Decimal[] { resultRaw }), new NumberSegments(new Decimal[] { remainder }), new NumberSegments(new Decimal[] { divisor }));
                }

           }
            else
            {
                result = new Tuple<NumberSegments, NumberSegments, NumberSegments>(new NumberSegments(new Decimal[] { 0 }), new NumberSegments(new Decimal[] { dividend }), new NumberSegments(new Decimal[] { divisor }));
            }

#if DEBUG
            foreach (Decimal segment in result.Item1)
            {
                if (segment > environment.Base)
                {
                    throw new Exception("Bad Divide segment larger than base Item 1");
                }
                else if (segment < 0)
                {
                    throw new Exception("Bad Divide  segment less than zero Item 1");
                }
                else if (segment % 1 != 0)
                {
                    throw new Exception("Bad Divide segment has remainder Item 1");
                }
            }

            if (result.Item2 != default(NumberSegments))
            {
                foreach (Decimal segment in result.Item2)
                {
                    if (segment > environment.Base)
                    {
                        throw new Exception("Bad Divide segment larger than base Item 2");
                    }
                    else if (segment < 0)
                    {
                        throw new Exception("Bad Divide  segment less than zero Item 2");
                    }
                    else if (segment % 1 != 0)
                    {
                        throw new Exception("Bad Divide segment has remainder");
                    }
                }

                foreach (Decimal segment in result.Item3)
                {
                    if (segment > environment.Base)
                    {
                        throw new Exception("Bad Divide segment larger than base Item 3");
                    }
                    else if (segment < 0)
                    {
                        throw new Exception("Bad SuareRoot  Divide less than zero Item 3");
                    }
                    else if (segment % 1 != 0)
                    {
                        throw new Exception("Bad Divide segment has remainder");
                    }
                }
            }
#endif


            return result;
        }

        public Tuple<NumberSegments, NumberSegments, NumberSegments> Divide(IMathEnvironment environment, NumberSegments dividend, Decimal divisor)
        {
            if (dividend.Size == 1)
            {
                return this.Divide(environment, dividend[0], divisor);
            }

            Tuple<NumberSegments, NumberSegments, NumberSegments> result;

            Decimal remainder = 0M;

            Decimal[] workingTotal = new Decimal[dividend.Length];

            for (Int32 i = dividend.Length - 1; i >= 0; i--)
            {
                Decimal currentTotal = (dividend[i] / divisor) + (remainder * environment.Base);

                workingTotal[i] = System.Math.Floor(currentTotal);

                remainder = currentTotal - workingTotal[i];
            }
            var resultRaw = new List<Decimal>();

            Boolean skip = true;
            for (Int32 i = workingTotal.Count() - 1; i >= 0; i--)
            {
                if (skip && workingTotal[i] != 0)
                {
                    skip = false;
                }
                if (!skip || i == 0)
                {
                    resultRaw.Insert(0, workingTotal[i]);
                }
            }

            if (remainder == 0)
            {
                result = new Tuple<NumberSegments, NumberSegments, NumberSegments>(new NumberSegments(resultRaw), null, null);
            }
            else
            {
                result = new Tuple<NumberSegments, NumberSegments, NumberSegments>(new NumberSegments(resultRaw), new NumberSegments(new Decimal[] { remainder }), new NumberSegments(new Decimal[] { divisor }));
            }

#if DEBUG
            foreach (Decimal segment in result.Item1)
            {
                if (segment > environment.Base)
                {
                    throw new Exception("Bad Divide segment larger than base Item 1");
                }
                else if (segment < 0)
                {
                    throw new Exception("Bad Divide  segment less than zero Item 1");
                }
            }

            if (result.Item2 == default(NumberSegments))
            {
                NumberSegments reverseCheck = this.Multiply(environment,result.Item1, divisor);
                if (!this.IsEqual(environment, reverseCheck, dividend))
                {
                    throw new Exception(String.Format("Division Error {0} != {1} ", String.Join(',', reverseCheck.Reverse()), String.Join(',', dividend.Reverse())));
                }
            }
            else
            { 
                foreach (Decimal segment in result.Item2)
                {
                    if (segment > environment.Base)
                    {
                        throw new Exception("Bad Divide segment larger than base Item 2");
                    }
                    else if (segment < 0)
                    {
                        throw new Exception("Bad Divide  segment less than zero Item 2");
                    }
                    else if (segment % 1 != 0)
                    {
                        throw new Exception("Bad Divide segment has remainder Item 2");
                    }
                }

                foreach (Decimal segment in result.Item3)
                {
                    if (segment > environment.Base)
                    {
                        throw new Exception("Bad Divide segment larger than base Item 3");
                    }
                    else if (segment < 0)
                    {
                        throw new Exception("Bad SuareRoot  Divide less than zero Item 3");
                    }
                    else if (segment % 1 != 0)
                    {
                        throw new Exception("Bad Divide segment has remainder Item 2");
                    }
                }

                Decimal reverseFraction = remainder * divisor;

                NumberSegments reverseCheck = this.Add(environment,this.Multiply(environment,result.Item1, divisor), new NumberSegments(new Decimal[] { reverseFraction }));
                if (this.IsNotEqual(environment, reverseCheck, dividend))
                {
                    throw new Exception(String.Format("Division Error {0} != {1} ", String.Join(',', reverseCheck.Reverse()), String.Join(',', dividend.Reverse())));
                }
            }

#endif


            return result;
        }
        #endregion


        #region Subtract
        public Decimal Subtract(IMathEnvironment environment, Decimal a, Decimal b)
        {
            Decimal resultRaw = a - b;

            return resultRaw;
        }

        public NumberSegments Subtract(IMathEnvironment environment, NumberSegments a, NumberSegments b)
        {
            if (this.IsLessThan(environment,a, b))
            {
                throw new Exception("Negetive numbers not supported in MathAlgorithm subtract");
            }
            else if (a.Size == 1 && b.Size == 1)
            {
                return new NumberSegments(new Decimal[] { this.Subtract(environment, a[0], b[0]) });
            }

            Decimal maxPosition = a.Size;
            if (b.Size > maxPosition)
            {
                maxPosition = b.Size;
            }

            // 60 - 90
            var resultSegments = new List<Decimal>();
            Decimal borrow = 0;
            Decimal position = 0;
            while (position < maxPosition)
            {
                Decimal columnValue = borrow;
                borrow = 0;


                if (position < a.Size)
                {
                    columnValue += a[position];
                }

                if (position < b.Size)
                {
                    columnValue -= b[position];
                }

                if (columnValue < 0)
                {
                    borrow -= 1;
                    columnValue += environment.Base;
                }

                resultSegments.Add(columnValue);
                position++;
            }

            while (resultSegments.Count > 1 && resultSegments[resultSegments.Count - 1] == 0)
            {
                resultSegments.RemoveAt(resultSegments.Count - 1);
            }
            
            var result = new NumberSegments(resultSegments);

#if DEBUG
            if (this.IsGreaterThan(environment,result, a) && this.IsGreaterThan(environment,result, b))
            {
                throw new Exception("MathAlgorithm Subtraction error");
            }
            else if (result.Size > 1 && result[result.Size - 1] == 0)
            {
                throw new Exception("MathAlgorithm Subtraction leading zero error");
            }

            foreach (Decimal segment in result)
            {
                if (segment > environment.Base)
                {
                    throw new Exception("Bad Subtract segment larger than base Item 1");
                }
                else if (segment < 0)
                {
                    throw new Exception("Bad Subtract segment less than zero Item 1");
                }
                else if (segment % 1 != 0)
                {
                    throw new Exception("Bad Subtract segment has remainder Item 1");
                }
            }
#endif
            return result;

        }

        #endregion

        public Boolean IsFirst(NumberSegments number)
        {
            if (number != default(NumberSegments) && number.Size == 1 && number[0] == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean IsBottom(NumberSegments number)
        {
            if (number == default(NumberSegments) || number.Size == 0 || (number.Size == 1 && number[0] == 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}


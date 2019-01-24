using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Threading;
using VariableBase.Mathematics.Interfaces;

namespace VariableBase.Mathematics
{
    public class BasicMathAlgorithm : IBasicMathAlgorithm
    {
        protected DecimalMathEnvironment Environment { get; set; }

        public BasicMathAlgorithm(DecimalMathEnvironment environment)
        {
            this.Environment = environment;
        }


        #region Add
        public IList<Decimal> Add(Decimal a, Decimal b)
        {
            Decimal resultRaw = a + b;

            return this.AsSegments(resultRaw);
        }

        public ReadOnlyCollection<Decimal> Add(ReadOnlyCollection<Decimal> a, ReadOnlyCollection<Decimal> b)
        {
            ReadOnlyCollection<Decimal> result = default(ReadOnlyCollection<Decimal>);
            if (this.IsLessThan(a, b))
            {
                result = this.Add(b, a);
            }

            if (a.Count == 1 && b.Count == 1)
            {
                result = new ReadOnlyCollection<Decimal>(this.Add(a[0], b[0]));
            }

            if (result == default(ReadOnlyCollection<Decimal>))
            {
                var resultNumber = new List<Decimal>();
                Decimal carryOver = 0;
                Int32 position = 0;
                while (position < a.Count)
                {
                    Decimal columnValue = carryOver;

                    if (position < a.Count)
                    {
                        columnValue += a[position];
                    }

                    if (position < b.Count)
                    {
                        columnValue += b[position];
                    }

                    Decimal columnResult;
                    if (columnValue >= this.Environment.Base)
                    {
                        Decimal columnResultRaw = columnValue % this.Environment.Base;
                        columnResult = columnResultRaw;

                        carryOver = ((columnValue - columnResultRaw) / this.Environment.Base);
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
                    while (carryOver >= this.Environment.Base)
                    {
                        Decimal columnResultRaw = carryOver % this.Environment.Base;
                        columnResult = columnResultRaw;
                        resultNumber.Add(columnResult);

                        carryOver = columnResultRaw / this.Environment.Base;
                    }

                    if (carryOver > 0)
                    {
                        columnResult = carryOver;
                        resultNumber.Add(columnResult);
                    }
                }

                result = new ReadOnlyCollection<Decimal>(resultNumber);
            }

#if DEBUG
            if (this.IsLessThan(result, a) || this.IsLessThan(result, b))
            {
                throw new Exception("MathAlgorithm addtion error");
            }

            foreach (Decimal segment in result)
            {
                if (segment > this.Environment.Base)
                {
                    throw new Exception("Bad addtion segment larger than base");
                }
                else if (segment < 0)
                {
                    throw new Exception("Bad addition segment less than zero");
                }
            }

            ReadOnlyCollection<Decimal> reversResult = this.Subtract(result, a);
            if (this.IsNotEqual(reversResult, b))
            {
                throw new Exception("Bad addition result could not reverse");
            }
#endif
            return result;
        }

        #endregion

        public ReadOnlyCollection<Decimal> GetWholeNumberSomewhereBetween(ReadOnlyCollection<Decimal> a, ReadOnlyCollection<Decimal> b, Decimal variance = 0)
        {
            ReadOnlyCollection<Decimal> result;

            if (a == b)
            {
                result = a;
            }
            else
            {

                ReadOnlyCollection<Decimal> largerNumber;
                ReadOnlyCollection<Decimal> smallerNumber;

                if (this.IsGreaterThan(a, b))
                {
                    largerNumber = a;
                    smallerNumber = b;
                }
                else
                {
                    largerNumber = b;
                    smallerNumber = a;
                }

                Decimal firstIndexOfLargerNumber = largerNumber[largerNumber.Count - 1];
                Decimal firstIndexOfSmallerNumber = smallerNumber[smallerNumber.Count - 1];

                Decimal firstIndexOfResultRaw = (firstIndexOfLargerNumber + firstIndexOfSmallerNumber) / 2M;

                Decimal firstIndexOfResult;
                Decimal halfBase;

                if (variance > 0)
                {
                    halfBase = (this.Environment.Base) / 2;
                    firstIndexOfResult = System.Math.Ceiling(firstIndexOfResultRaw);
                }
                else
                {
                    halfBase = (this.Environment.Base) / 2;
                    firstIndexOfResult = System.Math.Floor(firstIndexOfResultRaw);
                }



                if ((largerNumber.Count - smallerNumber.Count <= 1)
                    || (largerNumber.Count - smallerNumber.Count == 2 && firstIndexOfResult <= 1))
                {
                    ReadOnlyCollection<Decimal> combinedValue = this.Add(largerNumber, smallerNumber);
                    result = this.GetAboutHalf(combinedValue, variance);
                }
                else
                {
                    Decimal somewhereBetweenPower = ((largerNumber.Count - smallerNumber.Count) / 2M) + smallerNumber.Count;
                   
                    Int32 power;
                    if (variance > 0)
                    {
                        power = (Int32)System.Math.Ceiling(somewhereBetweenPower);
                    }
                    else
                    {
                        power = (Int32)System.Math.Floor(somewhereBetweenPower);
                    }
                    Int32 powerWithVariance = (Int32)(power + variance);
                    result = this.PowerOfBase(firstIndexOfResult, powerWithVariance);
                    while (this.IsGreaterThan(result, a))
                    {
                        powerWithVariance -= 1;
                        result = this.PowerOfBase(firstIndexOfResult, powerWithVariance);
                    }
                }
            }
#if DEBUG
            if (this.IsGreaterThan(a, b) && (this.IsGreaterThan(result, a) || this.IsLessThan(result, b)))
            {
                throw new Exception("MathAlgorithm GetWholeNumberSomewhereBetween error 1");
            }
            else if (this.IsLessThan(a, b) && (this.IsGreaterThan(result, b) || this.IsLessThan(result, a)))
            {
                throw new Exception("MathAlgorithm GetWholeNumberSomewhereBetween Error 2");
            }

            foreach (Decimal segment in result)
            {
                if (segment > this.Environment.Base)
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

        public ReadOnlyCollection<Decimal> GetAboutHalf(ReadOnlyCollection<Decimal> number, Decimal variance)
        {
            Decimal halfFirstCharIndexDetail = (number[number.Count - 1]) / 2M;

            Decimal halfBaseIndexDetailed = (this.Environment.Base) / 2M;

            Decimal[] resultSegments;

            Decimal remainder = 0M;


            if (halfFirstCharIndexDetail >= 1M)
            {
                resultSegments = new Decimal[number.Count];
            }
            else
            {
                resultSegments = new Decimal[number.Count - 1];
                remainder = halfBaseIndexDetailed;
            }

            for (var i = resultSegments.Length - 1; i >= 0; i--)
            {
                Decimal charIndex = number[i];
                Decimal halfCharIndexWithRemainder = (charIndex / 2M) + remainder;
                
                if (i == 0)
                {
                    if (variance > 0 && System.Math.Ceiling(halfCharIndexWithRemainder) < this.Environment.Base)
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
                    if (halfCharIndexWithRemainderIndex >= this.Environment.Base)
                    {
                        Int32 currentSegmentIndex = (Int32)System.Math.Floor(halfBaseIndexDetailed);
                        resultSegments[i] = currentSegmentIndex;
                        remainder = 0M;
                    }
                    else
                    {
                        resultSegments[i] = halfCharIndexWithRemainderIndex;
                        remainder = (halfCharIndexWithRemainder - halfCharIndexWithRemainderIndex) * this.Environment.Base;
                    }
                }
            }

            while (resultSegments[resultSegments.Length-1] == 0)
            {
                resultSegments = resultSegments.Take(resultSegments.Length - 1).ToArray();
            }
            var result = new ReadOnlyCollection<Decimal>(resultSegments);

#if DEBUG
            if (this.IsGreaterThan(result, number))
            {
                throw new Exception("MathAlgorithm GetAboutHalf error");
            }
            foreach (Decimal segment in result)
            {
                if (segment > this.Environment.Base)
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

        public ReadOnlyCollection<Decimal> GetAboutHalf(ReadOnlyCollection<Decimal> a, ReadOnlyCollection<Decimal> b, Decimal variance)
        {
            ReadOnlyCollection<Decimal> x = this.Add(a, b);
            Tuple<ReadOnlyCollection<Decimal>,ReadOnlyCollection<Decimal>,ReadOnlyCollection<Decimal>> rawResult = this.Divide(x, this.Environment.SecondNumber.Segments);

            ReadOnlyCollection<Decimal> result;
            if (variance == 1 && rawResult.Item2 != default(ReadOnlyCollection<Decimal>))
            {
                result = this.Add(rawResult.Item1, this.Environment.KeyNumber[1].Segments);
            }
            else if (variance == -1 || rawResult.Item2 == default(ReadOnlyCollection<Decimal>))
            {
                result = rawResult.Item1;
            }
            else
            {
                ReadOnlyCollection<Decimal> doubleNumerator = this.Multiply(rawResult.Item2, 2);
                if (this.IsGreaterThanOrEqualTo(doubleNumerator, rawResult.Item2))
                {
                    result = this.Add(rawResult.Item1, this.Environment.KeyNumber[1].Segments);
                }
                else
                {
                    result = rawResult.Item1;
                }
            }

#if DEBUG
            if (this.IsGreaterThan(a, b) && (this.IsGreaterThan(result, a) || this.IsLessThan(result, b)))
            {
                throw new Exception("MathAlgorithm GetWholeNumberSomewhereBetween error 1");
            }
            else if (this.IsLessThan(a, b) && (this.IsLessThan(result, a) || this.IsGreaterThan(result, b)))
            {
                throw new Exception("MathAlgorithm GetWholeNumberSomewhereBetween Error 2");
            }

            foreach (Decimal segment in result)
            {
                if (segment > this.Environment.Base)
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

        public IList<Decimal> AsSegments(Decimal rawDouble)
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
                    if (carryOver >= this.Environment.Base)
                    {
                        Decimal columnResultRaw = 0;
                        columnResultRaw = carryOver % this.Environment.Base;
                        resultRaw.Add(columnResultRaw);
                        carryOver = ((carryOver - columnResultRaw) / this.Environment.Base);
                    }
                    else
                    {
                        resultRaw.Add(carryOver);
                        carryOver = 0;
                    }
                }
            }
            return resultRaw;
        }

        public ReadOnlyCollection<Decimal> PowerOfBase(Decimal a, Int32 times)
        {
            return this.PowerOfBase(new ReadOnlyCollection<Decimal>(new Decimal[] { a }), times);
        }

        public ReadOnlyCollection<Decimal> PowerOfBase(ReadOnlyCollection<Decimal> a, Int32 times)
        {
            if (a.Count == 1 && a[0] == 0)
            {
                return a;
            }

            var segments = new Decimal[(a.Count + times)];
            for (Int32 i = segments.Length - 1; i >= 0; i--)
            {
                segments[i] = 0;
            }
            a.CopyTo(segments, times);
#if DEBUG
            foreach (Decimal segment in segments)
            {
                if (segment > this.Environment.Base)
                {
                    throw new Exception("Bad PowerOfBase segment larger than base");
                }
                else if (segment < 0)
                {
                    throw new Exception("Bad PowerOfBase segment less than zero");
                }
            }
#endif
            return new ReadOnlyCollection<Decimal>(segments);
        }

        public bool IsOdd(ReadOnlyCollection<Decimal> a)
        {
            return !this.IsEven(a);
        }

        public bool IsEven(ReadOnlyCollection<Decimal> a)
        {
            Boolean isEven = false;
            if (this.Environment.Base % 2 == 0 || a.Count == 1)
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
                Tuple<ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>> half = this.Divide(a, this.Environment.SecondNumber.Segments);
                if (half.Item2 == default(ReadOnlyCollection<Decimal>))
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
//            Tuple<ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>> half = this.Divide(a, this.Environment.SecondNumber.Segments);
//            if (half.Item2 == default(ReadOnlyCollection<Decimal>) && !isEven)
//            {
//                throw new Exception("IsEven should be even but is not");
//            }
//            else if(half.Item2 != default(ReadOnlyCollection<Decimal>) && isEven)
//            {
//                throw new Exception("IsEven should NOT be even but is");
//            }
//#endif
            return isEven;
        }
        
        public ReadOnlyCollection<Decimal> Square(ReadOnlyCollection<Decimal> a)
        {
            return this.Multiply(a, a);
        }

        public Tuple<ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>> SquareRoot(Decimal number)
        {
            Decimal x = (Decimal)System.Math.Sqrt((Double)number);
            if (x % 1 == 0)
            {
                return new Tuple<ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>>(new ReadOnlyCollection<Decimal>(new Decimal[] { x }), null, null);
            }
            else
            {
                Decimal remainder = (number - (x * x));
                return new Tuple<ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>>(new ReadOnlyCollection<Decimal>(new Decimal[] { x }), new ReadOnlyCollection<Decimal>(new Decimal[] { remainder }), new ReadOnlyCollection<Decimal>(new Decimal[] { number }));
            }
        }

        public Tuple<ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>> SquareRoot(ReadOnlyCollection<Decimal> number)
        {

            if (this.IsBottom(number))
            {
                return default(Tuple<ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>>);
            }
            else if (number.Count == 1)
            {
                return this.SquareRoot(number[0]);
            }
            else if (this.IsLessThanOrEqualTo(number, new ReadOnlyCollection<Decimal>(this.AsSegments(3))))
            {
                return new Tuple<ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>>(this.Environment.KeyNumber[1].Segments, null, null);
            }
            
            ReadOnlyCollection<Decimal> floor = this.Environment.SecondNumber.Segments;
            ReadOnlyCollection<Decimal> ceiling = this.Divide(number, this.Environment.SecondNumber.Segments).Item1;

            ReadOnlyCollection<Decimal> lastNumberTried = this.GetWholeNumberSomewhereBetween(ceiling, floor);
            ReadOnlyCollection<Decimal> squareTestResult = this.Square(lastNumberTried);

            ReadOnlyCollection<Decimal> maxDifference = this.Subtract(lastNumberTried, new ReadOnlyCollection<Decimal>(new Decimal[] { 1 }));

            while (this.IsNotEqual(squareTestResult, number) && this.IsGreaterThan(this.Subtract(ceiling, floor), this.Environment.KeyNumber[1].Segments))
            {
                ReadOnlyCollection<Decimal> numberBeforLast = lastNumberTried;
                if (this.IsLessThan(squareTestResult, number))
                {
                    floor = lastNumberTried;
                    lastNumberTried = this.GetWholeNumberSomewhereBetween(ceiling, floor);
                    if(this.IsEqual(numberBeforLast, lastNumberTried))
                    {
                        lastNumberTried = this.Add(lastNumberTried, this.Environment.KeyNumber[1].Segments);
                    }
                }
                else if (this.IsGreaterThan(squareTestResult, number))
                {
                    ceiling = lastNumberTried;
                    lastNumberTried = this.GetWholeNumberSomewhereBetween(ceiling, floor, -1);
                    if (this.IsEqual(numberBeforLast, lastNumberTried))
                    {
                        lastNumberTried = this.Subtract(lastNumberTried, this.Environment.KeyNumber[1].Segments);
                    }
                }
                squareTestResult = this.Square(lastNumberTried);
                
            }

            Tuple<ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>> result;

            if (this.IsGreaterThan(number, squareTestResult))
            {
                var leftOver = this.Subtract(number, squareTestResult);
                result = new Tuple<ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>>(lastNumberTried, leftOver, number);

            }
            else if (this.IsGreaterThan(squareTestResult, number))
            {
                var wholeNumber = this.Subtract(lastNumberTried, this.Environment.KeyNumber[0].Segments);
                var leftOver = this.Subtract(squareTestResult, number);
                result = new Tuple<ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>>(wholeNumber, leftOver, number);
            }
            else
            {
                result = new Tuple<ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>>(lastNumberTried, null, null);
            }
#if DEBUG
            foreach (Decimal segment in result.Item1)
            {
                if (segment > this.Environment.Base)
                {
                    throw new Exception("Bad SuareRoot segment larger than base Item 1");
                }
                else if (segment < 0)
                {
                    throw new Exception("Bad SuareRoot  segment less than zero Item 1");
                }
            }

            if (result.Item2 != default(ReadOnlyCollection<Decimal>))
            {
                foreach (Decimal segment in result.Item2)
                {
                    if (segment > this.Environment.Base)
                    {
                        throw new Exception("Bad SuareRoot segment larger than base Item 2");
                    }
                    else if (segment < 0)
                    {
                        throw new Exception("Bad SuareRoot  segment less than zero Item 2");
                    }
                }

                foreach (Decimal segment in result.Item3)
                {
                    if (segment > this.Environment.Base)
                    {
                        throw new Exception("Bad SuareRoot segment larger than base Item 3");
                    }
                    else if (segment < 0)
                    {
                        throw new Exception("Bad SuareRoot  segment less than zero Item 3");
                    }
                }
            }
#endif
            return result;
        }

        public Boolean IsEqual(ReadOnlyCollection<Decimal> a, ReadOnlyCollection<Decimal> b)
        {
            if (this.CompareTo(a, b) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean IsNotEqual(ReadOnlyCollection<Decimal> a, ReadOnlyCollection<Decimal> b)
        {
            if (this.CompareTo(a, b) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean IsGreaterThan(ReadOnlyCollection<Decimal> a, ReadOnlyCollection<Decimal> b)
        {
            if (this.CompareTo(a, b) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean IsLessThan(ReadOnlyCollection<Decimal> a, ReadOnlyCollection<Decimal> b)
        {
            if (this.CompareTo(a, b) < 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean IsGreaterThanOrEqualTo(ReadOnlyCollection<Decimal> a, ReadOnlyCollection<Decimal> b)
        {
            if (this.CompareTo(a, b) >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean IsLessThanOrEqualTo(ReadOnlyCollection<Decimal> a, ReadOnlyCollection<Decimal> b)
        {
            if (this.CompareTo(a, b) <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int CompareTo(ReadOnlyCollection<Decimal> a, ReadOnlyCollection<Decimal> b)
        {
            Int32 result = 0;
            if (Object.ReferenceEquals(a, default(ReadOnlyCollection<Decimal>)) || a.Count == 0)
            {
                a = new ReadOnlyCollection<Decimal>(new Decimal[] { 0 });
            }

            if (Object.ReferenceEquals(b, default(ReadOnlyCollection<Decimal>)) || b.Count == 0)
            {
                b = new ReadOnlyCollection<Decimal>(new Decimal[] { 0 });
            }

            if (a.Count > b.Count)
            {
                result = 1;
            }
            else if (a.Count < b.Count)
            {
                result = -1;
            }

            if (result == 0)
            {
                for (var i = a.Count - 1; i >= 0; i--)
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

        public ReadOnlyCollection<Decimal> Multiply(ReadOnlyCollection<Decimal> a, Decimal b)
        {
            if (b == 0)
            {
                return new ReadOnlyCollection<Decimal>(new Decimal[] { 0 });
            }
            var resultRaw = new List<Decimal>();

            Decimal numberIndex = b;

            Decimal carryOver = 0;
            for (var i = 0; i < a.Count; i++)
            {
                Decimal segmentIndex = a[i];

                Decimal columnTotal = (numberIndex * segmentIndex) + carryOver;

                Decimal columnPositionResult;
                if (columnTotal >= this.Environment.Base)
                {
                    Decimal remainder = columnTotal % this.Environment.Base;
                    columnPositionResult = remainder;
                    carryOver = (columnTotal - remainder) / this.Environment.Base;
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
                if (carryOver > this.Environment.Base)
                {
                    Decimal remainder = carryOver % this.Environment.Base;
                    carryOverResult = remainder;
                    carryOver = (carryOver - remainder) / this.Environment.Base;
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
                if (segment > this.Environment.Base)
                {
                    throw new Exception("Bad Multiply segment larger than base Item");
                }
                else if (segment < 0)
                {
                    throw new Exception("Bad Multiply segment less than zero Item");
                }
            }
#endif
            return new ReadOnlyCollection<Decimal>(resultRaw);
        }

        public ReadOnlyCollection<Decimal> Multiply(ReadOnlyCollection<Decimal> a, ReadOnlyCollection<Decimal> b)
        {
            ReadOnlyCollection<Decimal> result = new ReadOnlyCollection<Decimal>(new Decimal[] { 0 });
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
            else if (a.Count == 1 && b.Count == 1)
            {
                return new ReadOnlyCollection<Decimal>(this.Multiply(a[0], b[0]));
            }

            for (Int32 i = 0; i < a.Count; i++)
            {
                Decimal numberSegment = a[i];
                ReadOnlyCollection<Decimal> currentResult = this.Multiply(b, numberSegment);

                if (i > 0)
                {
                    currentResult = this.PowerOfBase(currentResult, i);
                }

                result = this.Add(currentResult, result);
            }

#if DEBUG
            if (this.IsLessThan(result, a) || this.IsLessThan(result, b))
            {
                throw new Exception("MathAlgorithm Multiplication error");
            }
            else if (result.Count > 1 && result[result.Count - 1] == 0)
            {
                throw new Exception("MathAlgorithm Multiplication leading zero error");
            }

            foreach (Decimal segment in result)
            {
                if (segment > this.Environment.Base)
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

        public Decimal[] Multiply(Decimal a, Decimal b)
        {
            Decimal[] result;

            Decimal resultIndex = a * b;

            if (resultIndex >= this.Environment.Base)
            {
                Decimal firstNumber =   resultIndex % this.Environment.Base;
                Decimal secondNumber = (resultIndex - firstNumber) / this.Environment.Base;

                result = new Decimal[] { firstNumber, secondNumber};
            }
            else
            {
                result = new Decimal[] { resultIndex };
            }

#if DEBUG
            foreach (Decimal segment in result)
            {
                if (segment > this.Environment.Base)
                {
                    throw new Exception("Bad Multiply segment larger than base Item");
                }
                else if (segment < 0)
                {
                    throw new Exception("Bad Miltiply segment less than zero Item");
                }
            }
#endif
            return result;
        }

        #endregion

        #region Divide

        public Tuple<ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>> Divide(ReadOnlyCollection<Decimal> numerator, ReadOnlyCollection<Decimal> denominator, ReadOnlyCollection<Decimal> hint = default(ReadOnlyCollection<Decimal>))
        {

#if DEBUG
            Debug.WriteLine("Division start {0} / {1}", String.Join(',', numerator.Reverse()), String.Join(',', denominator.Reverse()));
            foreach (Decimal segment in numerator)
            {
                if (segment > this.Environment.Base)
                {
                    throw new Exception("Bad Divide numerator larger than base");
                }
                else if (segment < 0)
                {
                    throw new Exception("Bad Divide numerator larger than zero Item 1");
                }
            }

            foreach (Decimal segment in denominator)
            {
                if (segment > this.Environment.Base)
                {
                    throw new Exception("Bad Divide denominator larger than base");
                }
                else if (segment < 0)
                {
                    throw new Exception("Bad Divide denominator larger than zero Item 1");
                }
            }
#endif
            if (this.IsBottom(denominator))
            {
                return new Tuple<ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>>(numerator, null, null);
            }
            else if (this.IsEqual(numerator, denominator))
            {
                return new Tuple<ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>>(new ReadOnlyCollection<Decimal>(new Decimal[] { 1 }), null, null);
            }
            else if (this.IsLessThan(numerator, denominator))
            {
                return new Tuple<ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>>(new ReadOnlyCollection<Decimal>(new Decimal[] { 0 }), numerator, denominator);
            }
            else if (numerator.Count == 1 && denominator.Count == 1)
            {
                return this.Divide(numerator[0], denominator[0]);
            }
            else if (denominator.Count == 1)
            {
                return this.Divide(numerator, denominator[0]);
            }

            ReadOnlyCollection<Decimal> floor = (hint == default(ReadOnlyCollection<Decimal>) || this.IsGreaterThan(denominator, hint)) ? this.Environment.KeyNumber[1].Segments : this.GetAboutHalf(hint, this.Environment.KeyNumber[1].Segments, -1);
            ReadOnlyCollection<Decimal> ceiling = (hint == default(ReadOnlyCollection<Decimal>) || this.IsGreaterThan(denominator, hint)) ? numerator : this.GetAboutHalf(hint, numerator, 1);

            ReadOnlyCollection<Decimal> lastNumberTried = this.GetWholeNumberSomewhereBetween(ceiling, floor);
            ReadOnlyCollection<Decimal> numeratorTestResult = this.Multiply(lastNumberTried, denominator);

            ReadOnlyCollection<Decimal> maxDifference = this.Subtract(denominator, new ReadOnlyCollection<Decimal>(new Decimal[] { 1 }));
            ReadOnlyCollection<Decimal> minimumTestResult = this.Subtract(numerator, maxDifference);

            ReadOnlyCollection<Decimal> lastNumeratorTestResult = this.Environment.KeyNumber[0].Segments;

            while (this.IsLessThan(numeratorTestResult, minimumTestResult) || this.IsGreaterThan(numeratorTestResult, numerator))
            { 
                lastNumeratorTestResult = numeratorTestResult;
                if (this.IsLessThan(numeratorTestResult, numerator))
                {
                    floor = lastNumberTried;
                    lastNumberTried = this.GetWholeNumberSomewhereBetween(ceiling, floor);
                }
                else if (this.IsGreaterThan(numeratorTestResult, numerator))
                {
                    ceiling = lastNumberTried;
                    lastNumberTried = this.GetWholeNumberSomewhereBetween(ceiling, floor, -1);
                }

                if (this.IsGreaterThanOrEqualTo(floor, ceiling))
                {
                    floor = this.GetWholeNumberSomewhereBetween(ceiling, this.Environment.KeyNumber[1].Segments);
                }
                else if (this.IsLessThanOrEqualTo(ceiling, floor))
                {
                    ceiling = this.GetWholeNumberSomewhereBetween(floor, numerator);
                }

                numeratorTestResult = this.Multiply(lastNumberTried, denominator);
            }

            Tuple<ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>> result;

            if (this.IsEqual(numeratorTestResult, numerator))
            {
                result = new Tuple<ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>>(lastNumberTried, null, null);
            }
            else
            {
                var leftOver = this.Subtract(numerator, numeratorTestResult);
                result = new Tuple<ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>>(lastNumberTried, leftOver, denominator);
            }

#if DEBUG
            if (this.IsGreaterThan(result.Item1, numerator) && this.IsGreaterThan(result.Item1, denominator))
            {
                throw new Exception("MathAlgorithm Division error");
            }
            else if (result.Item1 != default(ReadOnlyCollection<Decimal>) && result.Item1.Count > 1 && result.Item1[result.Item1.Count - 1] == 0)
            {
                throw new Exception("MathAlgorithm Division leading zero error whole number");
            }
            else if (result.Item2 != default(ReadOnlyCollection<Decimal>) && result.Item2.Count > 1 && result.Item2[result.Item2.Count - 1] == 0)
            {
                throw new Exception("MathAlgorithm Division leading zero error numerator");
            }
            else if (result.Item3 != default(ReadOnlyCollection<Decimal>) && result.Item3.Count > 1 && result.Item3[result.Item3.Count - 1] == 0)
            {
                throw new Exception("MathAlgorithm Division leading zero error denominator");
            }

            foreach (Decimal segment in result.Item1)
            {
                if (segment > this.Environment.Base)
                {
                    throw new Exception("Bad Divide segment larger than base Item 1");
                }
                else if (segment < 0)
                {
                    throw new Exception("Bad Divide  segment less than zero Item 1");
                }
            }

            if (result.Item2 != default(ReadOnlyCollection<Decimal>))
            {
                foreach (Decimal segment in result.Item2)
                {
                    if (segment > this.Environment.Base)
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
                    if (segment > this.Environment.Base)
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

            if (result.Item1 != default(ReadOnlyCollection<Decimal>))
            {
                Debug.WriteLine("Division remainder {0} / {1}", String.Join(',', result.Item1.Reverse()), String.Join(',', result.Item2.Reverse()));
            }
#endif
            return result;
        }


        public Tuple<ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>> Divide(Decimal dividend, Decimal divisor)
        {
            Tuple<ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>> result;

            Decimal remainder;

            if (dividend > divisor)
            {
                remainder = (dividend % divisor);

                Decimal resultRaw = System.Math.Floor(dividend / divisor);

                if (remainder == 0)
                {
                    result = new Tuple<ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>>(new ReadOnlyCollection<Decimal>(new Decimal[] { resultRaw }), null, null);
                }
                else
                {
                    result = new Tuple<ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>>(new ReadOnlyCollection<Decimal>(new Decimal[] { resultRaw }), new ReadOnlyCollection<Decimal>(new Decimal[] { remainder }), new ReadOnlyCollection<Decimal>(new Decimal[] { divisor }));
                }

           }
            else
            {
                result = new Tuple<ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>>(new ReadOnlyCollection<Decimal>(new Decimal[] { 0 }), new ReadOnlyCollection<Decimal>(new Decimal[] { dividend }), new ReadOnlyCollection<Decimal>(new Decimal[] { divisor }));
            }

#if DEBUG
            foreach (Decimal segment in result.Item1)
            {
                if (segment > this.Environment.Base)
                {
                    throw new Exception("Bad Divide segment larger than base Item 1");
                }
                else if (segment < 0)
                {
                    throw new Exception("Bad Divide  segment less than zero Item 1");
                }
            }

            if (result.Item2 != default(ReadOnlyCollection<Decimal>))
            {
                foreach (Decimal segment in result.Item2)
                {
                    if (segment > this.Environment.Base)
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
                    if (segment > this.Environment.Base)
                    {
                        throw new Exception("Bad Divide segment larger than base Item 3");
                    }
                    else if (segment < 0)
                    {
                        throw new Exception("Bad SuareRoot  Divide less than zero Item 3");
                    }
                }
            }
#endif


            return result;
        }

        public Tuple<ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>> Divide(ReadOnlyCollection<Decimal> dividend, Decimal divisor)
        {
            if (dividend.Count == 1)
            {
                return this.Divide(dividend[0], divisor);
            }

            Tuple<ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>> result;

            Decimal remainder = 0M;

            Decimal[] workingTotal = new Decimal[dividend.Count];

            for (Int32 i = dividend.Count - 1; i >= 0; i--)
            {
                Decimal currentTotal = (dividend[i] / divisor) + (remainder * this.Environment.Base);

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
                result = new Tuple<ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>>(new ReadOnlyCollection<Decimal>(resultRaw), null, null);
            }
            else
            {
                result = new Tuple<ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>>(new ReadOnlyCollection<Decimal>(resultRaw), new ReadOnlyCollection<Decimal>(new Decimal[] { remainder }), new ReadOnlyCollection<Decimal>(new Decimal[] { divisor }));
            }

#if DEBUG
            foreach (Decimal segment in result.Item1)
            {
                if (segment > this.Environment.Base)
                {
                    throw new Exception("Bad Divide segment larger than base Item 1");
                }
                else if (segment < 0)
                {
                    throw new Exception("Bad Divide  segment less than zero Item 1");
                }
            }

            if (result.Item2 == default(ReadOnlyCollection<Decimal>))
            {
                ReadOnlyCollection<Decimal> reverseCheck = this.Multiply(result.Item1, divisor);
                if (!this.IsEqual(reverseCheck, dividend))
                {
                    throw new Exception(String.Format("Division Error {0} != {1} ", String.Join(',', reverseCheck.Reverse()), String.Join(',', dividend.Reverse())));
                }
            }
            else
            { 
                foreach (Decimal segment in result.Item2)
                {
                    if (segment > this.Environment.Base)
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
                    if (segment > this.Environment.Base)
                    {
                        throw new Exception("Bad Divide segment larger than base Item 3");
                    }
                    else if (segment < 0)
                    {
                        throw new Exception("Bad SuareRoot  Divide less than zero Item 3");
                    }
                }

                Decimal reverseFraction = remainder * divisor;

                ReadOnlyCollection<Decimal> reverseCheck = this.Add(this.Multiply(result.Item1, divisor), new ReadOnlyCollection<Decimal>(new Decimal[] { reverseFraction }));
                if (this.IsNotEqual(reverseCheck, dividend))
                {
                    throw new Exception(String.Format("Division Error {0} != {1} ", String.Join(',', reverseCheck.Reverse()), String.Join(',', dividend.Reverse())));
                }
            }

#endif


            return result;
        }
        #endregion


        #region Subtract
        public Decimal Subtract(Decimal a, Decimal b)
        {
            Decimal resultRaw = a - b;

            return resultRaw;
        }

        public ReadOnlyCollection<Decimal> Subtract(ReadOnlyCollection<Decimal> a, ReadOnlyCollection<Decimal> b)
        {
            if (this.IsLessThan(a, b))
            {
                throw new Exception("Negetive numbers not supported in MathAlgorithm subtract");
            }
            else if (a.Count == 1 && b.Count == 1)
            {
                return new ReadOnlyCollection<Decimal>(new Decimal[] { this.Subtract(a[0], b[0]) });
            }

            Decimal maxPosition = a.Count;
            if (b.Count > maxPosition)
            {
                maxPosition = b.Count;
            }

            // 60 - 90
            var resultSegments = new List<Decimal>();
            Decimal borrow = 0;
            Decimal position = 0;
            while (position < maxPosition)
            {
                Decimal columnValue = borrow;
                borrow = 0;


                if (position < a.Count)
                {
                    columnValue += a[(Int32)position];
                }

                if (position < b.Count)
                {
                    columnValue -= b[(Int32)position];
                }

                if (columnValue < 0)
                {
                    borrow -= 1;
                    columnValue += this.Environment.Base;
                }

                resultSegments.Add(columnValue);
                position++;
            }

            while (resultSegments.Count > 1 && resultSegments[resultSegments.Count - 1] == 0)
            {
                resultSegments.RemoveAt(resultSegments.Count - 1);
            }
            
            var result = new ReadOnlyCollection<Decimal>(resultSegments);

#if DEBUG
            if (this.IsGreaterThan(result, a) && this.IsGreaterThan(result, b))
            {
                throw new Exception("MathAlgorithm Subtraction error");
            }
            else if (result.Count > 1 && result[result.Count - 1] == 0)
            {
                throw new Exception("MathAlgorithm Subtraction leading zero error");
            }

            foreach (Decimal segment in result)
            {
                if (segment > this.Environment.Base)
                {
                    throw new Exception("Bad Subtract segment larger than base Item 1");
                }
                else if (segment < 0)
                {
                    throw new Exception("Bad Subtract segment less than zero Item 1");
                }
            }
#endif
            return result;

        }

        #endregion

        public Boolean IsFirst(ReadOnlyCollection<Decimal> number)
        {
            if (number != default(ReadOnlyCollection<Decimal>) && number.Count == 1 && number[0] == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean IsBottom(ReadOnlyCollection<Decimal> number)
        {
            if (number == default(ReadOnlyCollection<Decimal>) || number.Count == 0 || (number.Count == 1 && number[0] == 0))
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


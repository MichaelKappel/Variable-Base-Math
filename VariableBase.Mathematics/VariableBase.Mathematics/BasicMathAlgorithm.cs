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
        public IList<Double> Add(Double a, Double b)
        {
            Double resultRaw = a + b;

            return this.AsSegments(resultRaw);
        }

        public ReadOnlyCollection<Double> Add(ReadOnlyCollection<Double> a, ReadOnlyCollection<Double> b)
        {
            ReadOnlyCollection<Double> result = default(ReadOnlyCollection<Double>);
            if (this.IsLessThan(a, b))
            {
                result = this.Add(b, a);
            }

            if (a.Count == 1 && b.Count == 1)
            {
                result = new ReadOnlyCollection<Double>(this.Add(a[0], b[0]));
            }

            if (result == default(ReadOnlyCollection<Double>))
            {
                var resultNumber = new List<Double>();
                Double carryOver = 0;
                Int32 position = 0;
                while (position < a.Count)
                {
                    Double columnValue = carryOver;

                    if (position < a.Count)
                    {
                        columnValue += a[position];
                    }

                    if (position < b.Count)
                    {
                        columnValue += b[position];
                    }

                    Double columnResult;
                    if (columnValue >= this.Environment.Base)
                    {
                        Double columnResultRaw = columnValue % this.Environment.Base;
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
                    Double columnResult;
                    while (carryOver >= this.Environment.Base)
                    {
                        Double columnResultRaw = carryOver % this.Environment.Base;
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

                result = new ReadOnlyCollection<Double>(resultNumber);
            }

#if DEBUG
            if (this.IsLessThan(result, a) || this.IsLessThan(result, b))
            {
                throw new Exception("MathAlgorithm addtion error");
            }

            foreach (Double segment in result)
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

            ReadOnlyCollection<Double> reversResult = this.Subtract(result, a);
            if (this.IsNotEqual(reversResult, b))
            {
                throw new Exception("Bad addition result could not reverse");
            }
#endif
            return result;
        }

        #endregion

        public ReadOnlyCollection<Double> GetWholeNumberSomewhereBetween(ReadOnlyCollection<Double> a, ReadOnlyCollection<Double> b, Double variance = 0)
        {
            ReadOnlyCollection<Double> result;

            if (a == b)
            {
                result = a;
            }
            else
            {

                ReadOnlyCollection<Double> largerNumber;
                ReadOnlyCollection<Double> smallerNumber;

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

                Double firstIndexOfLargerNumber = largerNumber[largerNumber.Count - 1];
                Double firstIndexOfSmallerNumber = smallerNumber[smallerNumber.Count - 1];

                Double firstIndexOfResultRaw = (firstIndexOfLargerNumber + firstIndexOfSmallerNumber) / 2D;

                Double firstIndexOfResult;
                Double halfBase;

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
                    ReadOnlyCollection<Double> combinedValue = this.Add(largerNumber, smallerNumber);
                    result = this.GetAboutHalf(combinedValue, variance);
                }
                else
                {
                    Double somewhereBetweenPower = ((largerNumber.Count - smallerNumber.Count) / 2D) + smallerNumber.Count;
                   
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

            foreach (Double segment in result)
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

        public ReadOnlyCollection<Double> GetAboutHalf(ReadOnlyCollection<Double> number, Double variance)
        {
            Double halfFirstCharIndexDetail = (number[number.Count - 1]) / 2D;

            Double halfBaseIndexDetailed = (this.Environment.Base) / 2D;

            Double[] resultSegments;

            Double remainder = 0D;


            if (halfFirstCharIndexDetail >= 1D)
            {
                resultSegments = new Double[number.Count];
            }
            else
            {
                resultSegments = new Double[number.Count - 1];
                remainder = halfBaseIndexDetailed;
            }

            for (var i = resultSegments.Length - 1; i >= 0; i--)
            {
                Double charIndex = number[i];
                Double halfCharIndexWithRemainder = (charIndex / 2D) + remainder;
                
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
                    Double halfCharIndexWithRemainderIndex = System.Math.Floor(halfCharIndexWithRemainder);
                    if (halfCharIndexWithRemainderIndex >= this.Environment.Base)
                    {
                        Int32 currentSegmentIndex = (Int32)System.Math.Floor(halfBaseIndexDetailed);
                        resultSegments[i] = currentSegmentIndex;
                        remainder = 0D;
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
            var result = new ReadOnlyCollection<Double>(resultSegments);

#if DEBUG
            if (this.IsGreaterThan(result, number))
            {
                throw new Exception("MathAlgorithm GetAboutHalf error");
            }
            foreach (Double segment in result)
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

        public ReadOnlyCollection<Double> GetAboutHalf(ReadOnlyCollection<Double> a, ReadOnlyCollection<Double> b, Double variance)
        {
            ReadOnlyCollection<Double> x = this.Add(a, b);
            Tuple<ReadOnlyCollection<Double>,ReadOnlyCollection<Double>,ReadOnlyCollection<Double>> rawResult = this.Divide(x, this.Environment.SecondNumber.Segments);

            ReadOnlyCollection<Double> result;
            if (variance == 1 && rawResult.Item2 != default(ReadOnlyCollection<Double>))
            {
                result = this.Add(rawResult.Item1, this.Environment.KeyNumber[1].Segments);
            }
            else if (variance == -1 || rawResult.Item2 == default(ReadOnlyCollection<Double>))
            {
                result = rawResult.Item1;
            }
            else
            {
                ReadOnlyCollection<Double> doubleNumerator = this.Multiply(rawResult.Item2, 2);
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

            foreach (Double segment in result)
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

        public IList<Double> AsSegments(Double rawDouble)
        {
            var resultRaw = new List<Double>();
            if (rawDouble == 0)
            {
                resultRaw.Add(0);
            }
            else
            {

                Double carryOver = rawDouble;
                while (carryOver > 0)
                {
                    if (carryOver >= this.Environment.Base)
                    {
                        Double columnResultRaw = 0;
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

        public ReadOnlyCollection<Double> PowerOfBase(Double a, Int32 times)
        {
            return this.PowerOfBase(new ReadOnlyCollection<Double>(new Double[] { a }), times);
        }

        public ReadOnlyCollection<Double> PowerOfBase(ReadOnlyCollection<Double> a, Int32 times)
        {
            if (a.Count == 1 && a[0] == 0)
            {
                return a;
            }

            var segments = new Double[(a.Count + times)];
            for (Int32 i = segments.Length - 1; i >= 0; i--)
            {
                segments[i] = 0;
            }
            a.CopyTo(segments, times);
#if DEBUG
            foreach (Double segment in segments)
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
            return new ReadOnlyCollection<Double>(segments);
        }

        public bool IsOdd(ReadOnlyCollection<Double> a)
        {
            return !this.IsEven(a);
        }

        public bool IsEven(ReadOnlyCollection<Double> a)
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
                Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>, ReadOnlyCollection<Double>> half = this.Divide(a, this.Environment.SecondNumber.Segments);
                if (half.Item2 == default(ReadOnlyCollection<Double>))
                {
                    isEven = true;
                }
                else
                {
                    isEven = false;
                }

                //Double x = 0;
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
//            Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>, ReadOnlyCollection<Double>> half = this.Divide(a, this.Environment.SecondNumber.Segments);
//            if (half.Item2 == default(ReadOnlyCollection<Double>) && !isEven)
//            {
//                throw new Exception("IsEven should be even but is not");
//            }
//            else if(half.Item2 != default(ReadOnlyCollection<Double>) && isEven)
//            {
//                throw new Exception("IsEven should NOT be even but is");
//            }
//#endif
            return isEven;
        }
        
        public ReadOnlyCollection<Double> Square(ReadOnlyCollection<Double> a)
        {
            return this.Multiply(a, a);
        }

        public Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>, ReadOnlyCollection<Double>> SquareRoot(Double number)
        {
            Double x = System.Math.Sqrt(number);
            if (x % 1 == 0)
            {
                return new Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>, ReadOnlyCollection<Double>>(new ReadOnlyCollection<Double>(new Double[] { x }), null, null);
            }
            else
            {
                Double remainder = (number - (x * x));
                return new Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>, ReadOnlyCollection<Double>>(new ReadOnlyCollection<Double>(new Double[] { x }), new ReadOnlyCollection<Double>(new Double[] { remainder }), new ReadOnlyCollection<Double>(new Double[] { number }));
            }
        }

        public Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>, ReadOnlyCollection<Double>> SquareRoot(ReadOnlyCollection<Double> number)
        {

            if (this.IsBottom(number))
            {
                return default(Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>, ReadOnlyCollection<Double>>);
            }
            else if (number.Count == 1)
            {
                return this.SquareRoot(number[0]);
            }
            else if (this.IsLessThanOrEqualTo(number, new ReadOnlyCollection<Double>(this.AsSegments(3))))
            {
                return new Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>, ReadOnlyCollection<Double>>(this.Environment.KeyNumber[1].Segments, null, null);
            }
            
            ReadOnlyCollection<Double> floor = this.Environment.SecondNumber.Segments;
            ReadOnlyCollection<Double> ceiling = this.Divide(number, this.Environment.SecondNumber.Segments).Item1;

            ReadOnlyCollection<Double> lastNumberTried = this.GetWholeNumberSomewhereBetween(ceiling, floor);
            ReadOnlyCollection<Double> squareTestResult = this.Square(lastNumberTried);

            ReadOnlyCollection<Double> maxDifference = this.Subtract(lastNumberTried, new ReadOnlyCollection<Double>(new Double[] { 1 }));

            while (this.IsNotEqual(squareTestResult, number) && this.IsGreaterThan(this.Subtract(ceiling, floor), this.Environment.KeyNumber[1].Segments))
            {
                ReadOnlyCollection<Double> numberBeforLast = lastNumberTried;
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

            Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>, ReadOnlyCollection<Double>> result;

            if (this.IsGreaterThan(number, squareTestResult))
            {
                var leftOver = this.Subtract(number, squareTestResult);
                result = new Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>, ReadOnlyCollection<Double>>(lastNumberTried, leftOver, number);

            }
            else if (this.IsGreaterThan(squareTestResult, number))
            {
                var wholeNumber = this.Subtract(lastNumberTried, this.Environment.KeyNumber[0].Segments);
                var leftOver = this.Subtract(squareTestResult, number);
                result = new Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>, ReadOnlyCollection<Double>>(wholeNumber, leftOver, number);
            }
            else
            {
                result = new Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>, ReadOnlyCollection<Double>>(lastNumberTried, null, null);
            }
#if DEBUG
            foreach (Double segment in result.Item1)
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

            if (result.Item2 != default(ReadOnlyCollection<Double>))
            {
                foreach (Double segment in result.Item2)
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

                foreach (Double segment in result.Item3)
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

        public Boolean IsEqual(ReadOnlyCollection<Double> a, ReadOnlyCollection<Double> b)
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

        public Boolean IsNotEqual(ReadOnlyCollection<Double> a, ReadOnlyCollection<Double> b)
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

        public Boolean IsGreaterThan(ReadOnlyCollection<Double> a, ReadOnlyCollection<Double> b)
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

        public Boolean IsLessThan(ReadOnlyCollection<Double> a, ReadOnlyCollection<Double> b)
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

        public Boolean IsGreaterThanOrEqualTo(ReadOnlyCollection<Double> a, ReadOnlyCollection<Double> b)
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

        public Boolean IsLessThanOrEqualTo(ReadOnlyCollection<Double> a, ReadOnlyCollection<Double> b)
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

        public int CompareTo(ReadOnlyCollection<Double> a, ReadOnlyCollection<Double> b)
        {
            Int32 result = 0;
            if (Object.ReferenceEquals(a, default(ReadOnlyCollection<Double>)) || a.Count == 0)
            {
                a = new ReadOnlyCollection<Double>(new Double[] { 0 });
            }

            if (Object.ReferenceEquals(b, default(ReadOnlyCollection<Double>)) || b.Count == 0)
            {
                b = new ReadOnlyCollection<Double>(new Double[] { 0 });
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

        public ReadOnlyCollection<Double> Multiply(ReadOnlyCollection<Double> a, Double b)
        {
            if (b == 0)
            {
                return new ReadOnlyCollection<Double>(new Double[] { 0 });
            }
            var resultRaw = new List<Double>();

            Double numberIndex = b;

            Double carryOver = 0;
            for (var i = 0; i < a.Count; i++)
            {
                Double segmentIndex = a[i];

                Double columnTotal = (numberIndex * segmentIndex) + carryOver;

                Double columnPositionResult;
                if (columnTotal >= this.Environment.Base)
                {
                    Double remainder = columnTotal % this.Environment.Base;
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
                Double carryOverResult;
                if (carryOver > this.Environment.Base)
                {
                    Double remainder = carryOver % this.Environment.Base;
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
            foreach (Double segment in resultRaw)
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
            return new ReadOnlyCollection<Double>(resultRaw);
        }

        public ReadOnlyCollection<Double> Multiply(ReadOnlyCollection<Double> a, ReadOnlyCollection<Double> b)
        {
            ReadOnlyCollection<Double> result = new ReadOnlyCollection<Double>(new Double[] { 0 });
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
                return new ReadOnlyCollection<Double>(this.Multiply(a[0], b[0]));
            }

            for (Int32 i = 0; i < a.Count; i++)
            {
                Double numberSegment = a[i];
                ReadOnlyCollection<Double> currentResult = this.Multiply(b, numberSegment);

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

            foreach (Double segment in result)
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

        public Double[] Multiply(Double a, Double b)
        {
            Double[] result;

            Double resultIndex = a * b;

            if (resultIndex >= this.Environment.Base)
            {
                Double firstNumber =   resultIndex % this.Environment.Base;
                Double secondNumber = (resultIndex - firstNumber) / this.Environment.Base;

                result = new Double[] { firstNumber, secondNumber};
            }
            else
            {
                result = new Double[] { resultIndex };
            }

#if DEBUG
            foreach (Double segment in result)
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

        public Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>, ReadOnlyCollection<Double>> Divide(ReadOnlyCollection<Double> numerator, ReadOnlyCollection<Double> denominator, ReadOnlyCollection<Double> hint = default(ReadOnlyCollection<Double>))
        {

#if DEBUG
            Debug.WriteLine("Division start {0} / {1}", String.Join(',', numerator.Reverse()), String.Join(',', denominator.Reverse()));
            foreach (Double segment in numerator)
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

            foreach (Double segment in denominator)
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
                return new Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>, ReadOnlyCollection<Double>>(numerator, null, null);
            }
            else if (this.IsEqual(numerator, denominator))
            {
                return new Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>, ReadOnlyCollection<Double>>(new ReadOnlyCollection<Double>(new Double[] { 1 }), null, null);
            }
            else if (this.IsLessThan(numerator, denominator))
            {
                return new Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>, ReadOnlyCollection<Double>>(new ReadOnlyCollection<Double>(new Double[] { 0 }), numerator, denominator);
            }
            else if (numerator.Count == 1 && denominator.Count == 1)
            {
                return this.Divide(numerator[0], denominator[0]);
            }
            else if (denominator.Count == 1)
            {
                return this.Divide(numerator, denominator[0]);
            }

            ReadOnlyCollection<Double> floor = (hint == default(ReadOnlyCollection<Double>) || this.IsGreaterThan(denominator, hint)) ? this.Environment.KeyNumber[1].Segments : this.GetAboutHalf(hint, this.Environment.KeyNumber[1].Segments, -1);
            ReadOnlyCollection<Double> ceiling = (hint == default(ReadOnlyCollection<Double>) || this.IsGreaterThan(denominator, hint)) ? numerator : this.GetAboutHalf(hint, numerator, 1);

            ReadOnlyCollection<Double> lastNumberTried = this.GetWholeNumberSomewhereBetween(ceiling, floor);
            ReadOnlyCollection<Double> numeratorTestResult = this.Multiply(lastNumberTried, denominator);

            ReadOnlyCollection<Double> maxDifference = this.Subtract(denominator, new ReadOnlyCollection<Double>(new Double[] { 1 }));
            ReadOnlyCollection<Double> minimumTestResult = this.Subtract(numerator, maxDifference);

            ReadOnlyCollection<Double> lastNumeratorTestResult = this.Environment.KeyNumber[0].Segments;

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

            Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>, ReadOnlyCollection<Double>> result;

            if (this.IsEqual(numeratorTestResult, numerator))
            {
                result = new Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>, ReadOnlyCollection<Double>>(lastNumberTried, null, null);
            }
            else
            {
                var leftOver = this.Subtract(numerator, numeratorTestResult);
                result = new Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>, ReadOnlyCollection<Double>>(lastNumberTried, leftOver, denominator);
            }

#if DEBUG
            if (this.IsGreaterThan(result.Item1, numerator) && this.IsGreaterThan(result.Item1, denominator))
            {
                throw new Exception("MathAlgorithm Division error");
            }
            else if (result.Item1 != default(ReadOnlyCollection<Double>) && result.Item1.Count > 1 && result.Item1[result.Item1.Count - 1] == 0)
            {
                throw new Exception("MathAlgorithm Division leading zero error whole number");
            }
            else if (result.Item2 != default(ReadOnlyCollection<Double>) && result.Item2.Count > 1 && result.Item2[result.Item2.Count - 1] == 0)
            {
                throw new Exception("MathAlgorithm Division leading zero error numerator");
            }
            else if (result.Item3 != default(ReadOnlyCollection<Double>) && result.Item3.Count > 1 && result.Item3[result.Item3.Count - 1] == 0)
            {
                throw new Exception("MathAlgorithm Division leading zero error denominator");
            }

            foreach (Double segment in result.Item1)
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

            if (result.Item2 != default(ReadOnlyCollection<Double>))
            {
                foreach (Double segment in result.Item2)
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

                foreach (Double segment in result.Item3)
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

            if (result.Item1 != default(ReadOnlyCollection<Double>))
            {
                Debug.WriteLine("Division remainder {0} / {1}", String.Join(',', result.Item1.Reverse()), String.Join(',', result.Item2.Reverse()));
            }
#endif
            return result;
        }


        public Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>, ReadOnlyCollection<Double>> Divide(Double dividend, Double divisor)
        {
            Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>, ReadOnlyCollection<Double>> result;

            Double remainder;

            if (dividend > divisor)
            {
                remainder = (dividend % divisor);

                Double resultRaw = System.Math.Floor(dividend / divisor);

                if (remainder == 0)
                {
                    result = new Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>, ReadOnlyCollection<Double>>(new ReadOnlyCollection<Double>(new Double[] { resultRaw }), null, null);
                }
                else
                {
                    result = new Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>, ReadOnlyCollection<Double>>(new ReadOnlyCollection<Double>(new Double[] { resultRaw }), new ReadOnlyCollection<Double>(new Double[] { remainder }), new ReadOnlyCollection<Double>(new Double[] { divisor }));
                }

           }
            else
            {
                result = new Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>, ReadOnlyCollection<Double>>(new ReadOnlyCollection<Double>(new Double[] { 0 }), new ReadOnlyCollection<Double>(new Double[] { dividend }), new ReadOnlyCollection<Double>(new Double[] { divisor }));
            }

#if DEBUG
            foreach (Double segment in result.Item1)
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

            if (result.Item2 != default(ReadOnlyCollection<Double>))
            {
                foreach (Double segment in result.Item2)
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

                foreach (Double segment in result.Item3)
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

        public Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>, ReadOnlyCollection<Double>> Divide(ReadOnlyCollection<Double> dividend, Double divisor)
        {
            if (dividend.Count == 1)
            {
                return this.Divide(dividend[0], divisor);
            }

            Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>, ReadOnlyCollection<Double>> result;

            Double remainder = 0D;

            Double[] workingTotal = new Double[dividend.Count];

            for (Int32 i = dividend.Count - 1; i >= 0; i--)
            {
                Double currentTotal = (dividend[i] / divisor) + (remainder * this.Environment.Base);

                workingTotal[i] = System.Math.Floor(currentTotal);

                remainder = currentTotal - workingTotal[i];
            }
            var resultRaw = new List<Double>();

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
                result = new Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>, ReadOnlyCollection<Double>>(new ReadOnlyCollection<Double>(resultRaw), null, null);
            }
            else
            {
                result = new Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>, ReadOnlyCollection<Double>>(new ReadOnlyCollection<Double>(resultRaw), new ReadOnlyCollection<Double>(new Double[] { remainder }), new ReadOnlyCollection<Double>(new Double[] { divisor }));
            }

#if DEBUG
            foreach (Double segment in result.Item1)
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

            if (result.Item2 == default(ReadOnlyCollection<Double>))
            {
                ReadOnlyCollection<Double> reverseCheck = this.Multiply(result.Item1, divisor);
                if (!this.IsEqual(reverseCheck, dividend))
                {
                    throw new Exception(String.Format("Division Error {0} != {1} ", String.Join(',', reverseCheck.Reverse()), String.Join(',', dividend.Reverse())));
                }
            }
            else
            { 
                foreach (Double segment in result.Item2)
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

                foreach (Double segment in result.Item3)
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

                Double reverseFraction = remainder * divisor;

                ReadOnlyCollection<Double> reverseCheck = this.Add(this.Multiply(result.Item1, divisor), new ReadOnlyCollection<Double>(new Double[] { reverseFraction }));
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
        public Double Subtract(Double a, Double b)
        {
            Double resultRaw = a - b;

            return resultRaw;
        }

        public ReadOnlyCollection<Double> Subtract(ReadOnlyCollection<Double> a, ReadOnlyCollection<Double> b)
        {
            if (this.IsLessThan(a, b))
            {
                throw new Exception("Negetive numbers not supported in MathAlgorithm subtract");
            }
            else if (a.Count == 1 && b.Count == 1)
            {
                return new ReadOnlyCollection<Double>(new Double[] { this.Subtract(a[0], b[0]) });
            }

            Double maxPosition = a.Count;
            if (b.Count > maxPosition)
            {
                maxPosition = b.Count;
            }

            // 60 - 90
            var resultSegments = new List<Double>();
            Double borrow = 0;
            Double position = 0;
            while (position < maxPosition)
            {
                Double columnValue = borrow;
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
            
            var result = new ReadOnlyCollection<Double>(resultSegments);

#if DEBUG
            if (this.IsGreaterThan(result, a) && this.IsGreaterThan(result, b))
            {
                throw new Exception("MathAlgorithm Subtraction error");
            }
            else if (result.Count > 1 && result[result.Count - 1] == 0)
            {
                throw new Exception("MathAlgorithm Subtraction leading zero error");
            }

            foreach (Double segment in result)
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

        public Boolean IsFirst(ReadOnlyCollection<Double> number)
        {
            if (number != default(ReadOnlyCollection<Double>) && number.Count == 1 && number[0] == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean IsBottom(ReadOnlyCollection<Double> number)
        {
            if (number == default(ReadOnlyCollection<Double>) || number.Count == 0 || (number.Count == 1 && number[0] == 0))
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


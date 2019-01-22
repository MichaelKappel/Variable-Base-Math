using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace VariableBase.Mathematics
{
    public class MathAlgorithm
    {
        protected MathEnvironment Environment { get; set; }

        public MathAlgorithm(MathEnvironment environment)
        {
            this.Environment = environment;

        }


        #region Add

        public ReadOnlyCollection<UInt16> Add(ReadOnlyCollection<UInt16> a, ReadOnlyCollection<UInt16> b)
        {

            Int32 maxPosition = a.Count;
            if (b.Count > maxPosition)
            {
                maxPosition = b.Count;
            }

            var resultNumber = new List<UInt16>();
            UInt64 carryOver = 0;
            Int32 position = 0;
            while (position < maxPosition)
            {
                UInt64 columnValue = carryOver;

                if (position < a.Count)
                {
                    columnValue += a[position];
                }

                if (position < b.Count)
                {
                    columnValue += b[position];
                }

                UInt16 columnResult;
                if (columnValue >= (UInt64)this.Environment.Key.Count)
                {
                    UInt16 columnResultRaw = (UInt16)(columnValue % (UInt64)this.Environment.Key.Count);
                    columnResult = columnResultRaw;

                    carryOver = (UInt64)(((Decimal)columnValue - (Decimal)columnResultRaw) / (Decimal)this.Environment.Key.Count);
                }
                else
                {
                    columnResult = (UInt16)columnValue;
                    carryOver = 0;
                }

                resultNumber.Add(columnResult);
                position++;
            }

            if (carryOver != 0)
            {
                UInt16 columnResult;
                while (carryOver >= (UInt64)this.Environment.Key.Count)
                {
                    UInt16 columnResultRaw = (UInt16)(carryOver % (UInt64)this.Environment.Key.Count);
                    columnResult = columnResultRaw;
                    resultNumber.Add(columnResult);

                    carryOver = (UInt16)((Decimal)columnResultRaw / (Decimal)this.Environment.Key.Count);
                }

                if (carryOver > 0)
                {
                    columnResult = (UInt16)carryOver;
                    resultNumber.Add(columnResult);
                }
            }

            var result = new ReadOnlyCollection<UInt16>(resultNumber);

#if DEBUG
            if (this.IsLessThan(result, a) || this.IsLessThan(result, b))
            {
                throw new Exception("MathAlgorithm addtion error");
            }
#endif
            return result;
        }

        #endregion

        public ReadOnlyCollection<UInt16> GetWholeNumberSomewhereBetween(ReadOnlyCollection<UInt16> a, ReadOnlyCollection<UInt16> b, Int64 variance = 0)
        {
            ReadOnlyCollection<UInt16> result;

            if (a == b)
            {
                result = a;
            }
            else
            {

                ReadOnlyCollection<UInt16> largerNumber;
                ReadOnlyCollection<UInt16> smallerNumber;

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

                UInt16 firstIndexOfResult;
                Decimal halfBase;

                if (variance > 0)
                {
                    halfBase = ((Decimal)this.Environment.Key.Count) / 2;
                    firstIndexOfResult = (UInt16)System.Math.Ceiling(firstIndexOfResultRaw);
                }
                else
                {
                    halfBase = ((Decimal)this.Environment.Key.Count) / 2;
                    firstIndexOfResult = (UInt16)System.Math.Floor(firstIndexOfResultRaw);
                }



                if ((largerNumber.Count - smallerNumber.Count <= 1)
                    || (largerNumber.Count - smallerNumber.Count == 2 && firstIndexOfResult <= 1))
                {
                    ReadOnlyCollection<UInt16> combinedValue = this.Add(largerNumber, smallerNumber);
                    result = this.GetAboutHalf(combinedValue, variance);
                }
                else
                {
                    Decimal somewhereBetweenPower = ((largerNumber.Count - smallerNumber.Count) / 2M) + smallerNumber.Count;

                    Decimal newIndex = largerNumber.Count - smallerNumber.Count;

                    Int32 power;
                    if (variance > 0)
                    {
                        power = (Int32)System.Math.Ceiling(somewhereBetweenPower);
                    }
                    else
                    {
                        power = (Int32)System.Math.Floor(somewhereBetweenPower);
                    }

                    if ((Decimal)power + variance > 1 && (Decimal)power + variance > 0)
                    {
                        result = this.PowerOfBase(firstIndexOfResult, (UInt16)(power + variance));
                    }
                    else
                    {
                        result = this.PowerOfBase(firstIndexOfResult, (UInt16)(power));
                    }

                }
            }
            return result;
        }

        public ReadOnlyCollection<UInt16> GetAboutHalf(ReadOnlyCollection<UInt16> number, Int64 variance)
        {
            Decimal halfFirstCharIndexDetail = ((Decimal)number[number.Count - 1]) / 2M;

            Decimal halfBaseIndexDetailed = ((Decimal)this.Environment.Key.Count) / 2M;

            UInt16[] resultSegments;

            Decimal remainder = 0M;


            if (halfFirstCharIndexDetail >= 1M)
            {
                resultSegments = new UInt16[number.Count];
            }
            else
            {
                resultSegments = new UInt16[number.Count - 1];
                remainder = halfBaseIndexDetailed;
            }

            for (var i = resultSegments.Length - 1; i >= 0; i--)
            {
                Decimal charIndex = number[i];
                Decimal halfCharIndexWithRemainder = (charIndex / 2M) + remainder;
                
                if (i == 0)
                {
                    if (variance > 0 && (UInt16)System.Math.Ceiling(halfCharIndexWithRemainder) < this.Environment.Key.Count)
                    {
                        resultSegments[0] = (UInt16)System.Math.Ceiling(halfCharIndexWithRemainder);
                    }
                    else
                    {
                        resultSegments[0] = (UInt16)(Int32)System.Math.Floor(halfCharIndexWithRemainder);
                    }
                }
                else
                {
                    UInt16 halfCharIndexWithRemainderIndex = (UInt16)System.Math.Floor(halfCharIndexWithRemainder);
                    if (halfCharIndexWithRemainderIndex >= this.Environment.Key.Count)
                    {
                        Int32 currentSegmentIndex = (Int32)System.Math.Floor(halfBaseIndexDetailed);
                        resultSegments[i] = (UInt16)currentSegmentIndex;
                        remainder = 0M;
                    }
                    else
                    {
                        resultSegments[i] = halfCharIndexWithRemainderIndex;
                        remainder = (halfCharIndexWithRemainder - ((Decimal)halfCharIndexWithRemainderIndex)) * this.Environment.Key.Count;
                    }
                }
            }

            while (resultSegments[resultSegments.Length-1] == 0)
            {
                resultSegments = resultSegments.Take(resultSegments.Length - 1).ToArray();
            }

            return new ReadOnlyCollection<UInt16>(resultSegments);
        }

        public ReadOnlyCollection<UInt16> GetAboutHalf(ReadOnlyCollection<UInt16> a, ReadOnlyCollection<UInt16> b, Int64 variance)
        {
            ReadOnlyCollection<UInt16> x = this.Add(a, b);
            Tuple<ReadOnlyCollection<UInt16>,ReadOnlyCollection<UInt16>,ReadOnlyCollection<UInt16>> rawResult = this.Divide(x, this.Environment.SecondNumber.Segments);

            ReadOnlyCollection<UInt16> result;
            if (variance == 1 && rawResult.Item2 != default(ReadOnlyCollection<UInt16>))
            {
                result = this.Add(rawResult.Item1, this.Environment.KeyNumber[1].Segments);
            }
            else if (variance == -1 || rawResult.Item2 == default(ReadOnlyCollection<UInt16>))
            {
                result = rawResult.Item1;
            }
            else
            {
                ReadOnlyCollection<UInt16> doubleNumerator = this.Multiply(rawResult.Item2, 2);
                if (this.IsGreaterThanOrEqualTo(doubleNumerator, rawResult.Item2))
                {
                    result = this.Add(rawResult.Item1, this.Environment.KeyNumber[1].Segments);
                }
                else
                {
                    result = rawResult.Item1;
                }
            }
            return result;
        }

        internal List<ushort> AsSegments(ulong rawUInt64)
        {
            var resultRaw = new List<UInt16>();
            if (rawUInt64 == 0)
            {
                resultRaw.Add(0);
            }
            else
            {

                UInt64 carryOver = rawUInt64;
                while (carryOver > 0)
                {
                    if (carryOver >= (UInt64)this.Environment.Key.Count)
                    {
                        UInt64 columnResultRaw = 0;
                        columnResultRaw = (carryOver % (UInt64)this.Environment.Key.Count);
                        resultRaw.Add((UInt16)columnResultRaw);
                        carryOver = (UInt64)(((Decimal)carryOver - (Decimal)columnResultRaw) / (Decimal)this.Environment.Key.Count);
                    }
                    else
                    {
                        resultRaw.Add((UInt16)carryOver);
                        carryOver = 0;
                    }
                }
            }
            return resultRaw;
        }

        public ReadOnlyCollection<UInt16> PowerOfBase(UInt16 a, UInt16 times)
        {
            return this.PowerOfBase(new ReadOnlyCollection<UInt16>(new UInt16[] { a }), times);
        }

        public ReadOnlyCollection<UInt16> PowerOfBase(ReadOnlyCollection<UInt16> a, UInt16 times)
        {
            if (a.Count == 1 && a[0] == 0)
            {
                return a;
            }

            var segments = new UInt16[(a.Count + times)];
            for (Int32 i = segments.Length - 1; i >= 0; i--)
            {
                segments[i] = 0;
            }
            a.CopyTo(segments, times);
            return new ReadOnlyCollection<UInt16>(segments);
        }

        public bool IsOdd(ReadOnlyCollection<UInt16> a)
        {
            return !this.IsEven(a);
        }

        public bool IsEven(ReadOnlyCollection<UInt16> a)
        {
            Boolean isEven = false;
            if (this.Environment.Key.Count % 2 == 0 || a.Count == 1)
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
                Tuple<ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>> half = this.Divide(a, this.Environment.SecondNumber.Segments);
                if (half.Item2 == default(ReadOnlyCollection<UInt16>))
                {
                    isEven = true;
                }
                else
                {
                    isEven = false;
                }

                //Int64 x = 0;
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
//            Tuple<ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>> half = this.Divide(a, this.Environment.SecondNumber.Segments);
//            if (half.Item2 == default(ReadOnlyCollection<UInt16>) && !isEven)
//            {
//                throw new Exception("IsEven should be even but is not");
//            }
//            else if(half.Item2 != default(ReadOnlyCollection<UInt16>) && isEven)
//            {
//                throw new Exception("IsEven should NOT be even but is");
//            }
//#endif
            return isEven;
        }

        internal bool IsPrime(ReadOnlyCollection<UInt16> a)
        {
            Tuple<ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>> divisor = this.GetComposite(a);
            if (divisor != default(Tuple<ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>>))
            {
                if (this.IsBottom(divisor.Item1) || this.IsBottom(divisor.Item2) || this.IsFirst(divisor.Item1) || this.IsFirst(divisor.Item2))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }


    internal Tuple<ReadOnlyCollection<UInt16>,ReadOnlyCollection<UInt16>> GetComposite(ReadOnlyCollection<UInt16> a)
    {
            Tuple<ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>> result = default(Tuple<ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>>);
            if (this.IsBottom(a) || this.IsFirst(a) || this.IsEqual(a, this.Environment.SecondNumber.Segments))
            {

            }
            else if (this.IsEven(a))
            {
                Tuple<ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>> half = this.Divide(a, this.Environment.SecondNumber.Segments);
                if (half.Item2 != default(ReadOnlyCollection<UInt16>) || half.Item2 != default(ReadOnlyCollection<UInt16>))
                {
                    throw new Exception("Math error in GetDivisor IsEven half");
                }
                result = new Tuple<ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>>(half.Item1, this.Environment.SecondNumber.Segments);
            }
            else
            {

                ReadOnlyCollection<UInt16> maxNumber;
                Tuple<ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>> maxNumberRaw = this.SquareRoot(a);
                if (maxNumberRaw.Item2 == default(ReadOnlyCollection<UInt16>))
                {
                    maxNumber = maxNumberRaw.Item1;
                }
                else
                {
                    maxNumber = this.Add(maxNumberRaw.Item1, this.Environment.KeyNumber[1].Segments);
                }
                ReadOnlyCollection<UInt16> lastResultWholeNumber = default(ReadOnlyCollection<UInt16>);
                ReadOnlyCollection<UInt16> testNumber = new ReadOnlyCollection<UInt16>(this.Environment.AsSegments(3));
                while (this.IsLessThanOrEqualTo(testNumber, maxNumber))
                {
                    Debug.WriteLine(String.Format("testNumber Divide {0} by {1}", String.Join(',', a.Select(x => x.ToString()).Reverse().ToArray()), String.Join(',', testNumber.Select(x=>x.ToString()).Reverse().ToArray())));
                    
                    Tuple<ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>> currentNumber = this.Divide(a, testNumber, lastResultWholeNumber);
                    if (currentNumber.Item2 == default(ReadOnlyCollection<UInt16>))
                    {
                        result = new Tuple<ReadOnlyCollection<ushort>, ReadOnlyCollection<ushort>>(currentNumber.Item1, testNumber);
                        break;
                    }

                    lastResultWholeNumber = currentNumber.Item1;
                    testNumber = this.Add(testNumber, Environment.SecondNumber.Segments);
                }
            }
#if DEBUG
            if (a.Count == 1)
            {
                int i = 2;
                Boolean isPrime = true;
                for (; i < a[0]-1; i++)
                {
                    if (a[0] % i == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }

                if (isPrime == false && result == default(Tuple<ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>>))
                {
                    throw new Exception(String.Format("Math Error in GetComposite {0} should have had a Composite of {1}", a[0], i));
                }
                else if (isPrime == true && result != default(Tuple<ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>>))
                {
                    throw new Exception(String.Format("Math Error in GetComposite {0} should NOT have a Composite of {1} x {2}", a[0], result.Item1[0], result.Item2[0]));
                }
            }
#endif
            return result;
        }

        public ReadOnlyCollection<UInt16> Square(ReadOnlyCollection<UInt16> a)
        {
            return this.Multiply(a, a);
        }

        public Tuple<ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>> SquareRoot(ReadOnlyCollection<UInt16> number)
        {

            if (this.IsBottom(number))
            {
                return default(Tuple<ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>>);
            }
            
            ReadOnlyCollection<UInt16> floor = this.Environment.SecondNumber.Segments;
            ReadOnlyCollection<UInt16> ceiling = this.Divide(number, this.Environment.SecondNumber.Segments).Item1;

            ReadOnlyCollection<UInt16> lastNumberTried = this.GetWholeNumberSomewhereBetween(ceiling, floor);
            ReadOnlyCollection<UInt16> squareTestResult = this.Square(lastNumberTried);

            ReadOnlyCollection<UInt16> maxDifference = this.Subtract(lastNumberTried, new ReadOnlyCollection<UInt16>(new UInt16[] { 1 }));
            ReadOnlyCollection<UInt16> minimumTestResult = this.Subtract(number, maxDifference);
            ReadOnlyCollection<UInt16> maximumTestResult = this.Add(number, maxDifference);

            while (this.IsNotEqual(floor, ceiling) && (this.IsLessThan(squareTestResult, minimumTestResult) || this.IsGreaterThan(squareTestResult, maximumTestResult)))
            {
                if (this.IsLessThan(squareTestResult, minimumTestResult))
                {
                    floor = lastNumberTried;
                    lastNumberTried = this.GetWholeNumberSomewhereBetween(ceiling, floor, 1);
                }
                else if (this.IsGreaterThan(squareTestResult, maximumTestResult))
                {
                    ceiling = lastNumberTried;
                    lastNumberTried = this.GetWholeNumberSomewhereBetween(ceiling, floor, -1);
                }
                squareTestResult = this.Square(lastNumberTried);

                maxDifference = this.Subtract(lastNumberTried, new ReadOnlyCollection<UInt16>(new UInt16[] { 1 }));
                minimumTestResult = this.Subtract(number, maxDifference);
                maximumTestResult = this.Add(number, maxDifference);
            }

            Tuple<ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>> result;

            if (this.IsGreaterThan(number, squareTestResult))
            {
                var leftOver = this.Subtract(number, squareTestResult);
                result = new Tuple<ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>>(lastNumberTried, leftOver, number);

            }
            else if (this.IsGreaterThan(squareTestResult, number))
            {
                var wholeNumber = this.Subtract(lastNumberTried, this.Environment.KeyNumber[0].Segments);
                var leftOver = this.Subtract(squareTestResult, number);
                result = new Tuple<ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>>(wholeNumber, leftOver, number);
            }
            else
            {
                result = new Tuple<ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>>(lastNumberTried, null, null);
            }
            return result;
        }

        public Boolean IsEqual(ReadOnlyCollection<UInt16> a, ReadOnlyCollection<UInt16> b)
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

        public Boolean IsNotEqual(ReadOnlyCollection<UInt16> a, ReadOnlyCollection<UInt16> b)
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

        public Boolean IsGreaterThan(ReadOnlyCollection<UInt16> a, ReadOnlyCollection<UInt16> b)
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

        public Boolean IsLessThan(ReadOnlyCollection<UInt16> a, ReadOnlyCollection<UInt16> b)
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

        public Boolean IsGreaterThanOrEqualTo(ReadOnlyCollection<UInt16> a, ReadOnlyCollection<UInt16> b)
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

        public Boolean IsLessThanOrEqualTo(ReadOnlyCollection<UInt16> a, ReadOnlyCollection<UInt16> b)
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

        public int CompareTo(ReadOnlyCollection<UInt16> a, ReadOnlyCollection<UInt16> b)
        {
            Int32 result = 0;
            if (Object.ReferenceEquals(a, default(ReadOnlyCollection<UInt16>)) || a.Count == 0)
            {
                a = new ReadOnlyCollection<UInt16>(new UInt16[] { 0 });
            }

            if (Object.ReferenceEquals(b, default(ReadOnlyCollection<UInt16>)) || b.Count == 0)
            {
                b = new ReadOnlyCollection<UInt16>(new UInt16[] { 0 });
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

        public ReadOnlyCollection<UInt16> Multiply(ReadOnlyCollection<UInt16> a, UInt16 b)
        {
            if (b == 0)
            {
                return new ReadOnlyCollection<UInt16>(new UInt16[] { 0 });
            }
            var resultRaw = new List<UInt16>();

            UInt64 numberIndex = b;

            UInt64 carryOver = 0;
            for (var i = 0; i < a.Count; i++)
            {
                UInt64 segmentIndex = a[i];

                UInt64 columnTotal = (numberIndex * segmentIndex) + carryOver;

                UInt16 columnPositionResult;
                if (columnTotal >= (UInt64)this.Environment.Key.Count)
                {
                    UInt64 remainder = (columnTotal % (UInt64)this.Environment.Key.Count);
                    columnPositionResult = (UInt16)remainder;
                    carryOver = (columnTotal - remainder) / (UInt64)this.Environment.Key.Count;
                }
                else
                {
                    columnPositionResult = (UInt16)columnTotal;
                    carryOver = 0;
                }

                resultRaw.Add(columnPositionResult);
            }

            while (carryOver > 0)
            {
                UInt16 carryOverResult;
                if (carryOver > (UInt64)this.Environment.Key.Count)
                {
                    UInt16 remainder = (UInt16)(carryOver % (UInt64)this.Environment.Key.Count);
                    carryOverResult = remainder;
                    carryOver = (carryOver - remainder) / (UInt64)this.Environment.Key.Count;
                }
                else
                {
                    carryOverResult = (UInt16)carryOver;
                    carryOver = 0;
                }
                resultRaw.Add(carryOverResult);
            }
            return new ReadOnlyCollection<UInt16>(resultRaw);
        }

        public ReadOnlyCollection<UInt16> Multiply(ReadOnlyCollection<UInt16> a, ReadOnlyCollection<UInt16> b)
        {
            ReadOnlyCollection<UInt16> result = new ReadOnlyCollection<UInt16>(new UInt16[] { 0 });
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

            for (UInt16 i = 0; i < (UInt16)a.Count; i++)
            {
                UInt16 numberSegment = a[i];
                ReadOnlyCollection<UInt16> currentResult = this.Multiply(b, numberSegment);

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
#endif

            return result;
        }

        public UInt16[] Multiply(UInt16 a, UInt16 b)
        {

            UInt32 resultIndex = (UInt32)a * (UInt32)b;

            if (resultIndex >= this.Environment.Key.Count)
            {
                UInt32 firstNumber = (resultIndex % (UInt32)this.Environment.Key.Count);
                UInt32 secondNumber = (resultIndex - firstNumber) / (UInt32)this.Environment.Key.Count;

                return new UInt16[] { (UInt16)secondNumber, (UInt16)firstNumber };
            }
            else
            {
                return new UInt16[] { (UInt16)resultIndex };
            }
        }

        #endregion

        #region Divide

        public Tuple<ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>> Divide(ReadOnlyCollection<UInt16> numerator, ReadOnlyCollection<UInt16> denominator, ReadOnlyCollection<UInt16> hint = default(ReadOnlyCollection<UInt16>))
        {
            if (this.IsBottom(denominator))
            {
                return new Tuple<ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>>(numerator, null, null);
            }
            else if (this.IsLessThan(numerator, denominator))
            {
                return new Tuple<ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>>(new ReadOnlyCollection<UInt16>(new UInt16[] { 0 }), numerator, denominator);
            }
            
            ReadOnlyCollection<UInt16> floor = (hint == default(ReadOnlyCollection<UInt16>) || this.IsGreaterThan(denominator, hint)) ? this.Environment.KeyNumber[1].Segments : this.GetAboutHalf(hint, this.Environment.KeyNumber[1].Segments, -1);
            ReadOnlyCollection<UInt16> ceiling = (hint == default(ReadOnlyCollection<UInt16>) || this.IsGreaterThan(denominator, hint)) ? numerator : this.GetAboutHalf(hint, numerator, 1);

            ReadOnlyCollection<UInt16> lastNumberTried = this.GetWholeNumberSomewhereBetween(ceiling, floor);
            ReadOnlyCollection<UInt16> numeratorTestResult = this.Multiply(lastNumberTried, denominator);

            ReadOnlyCollection<UInt16> maxDifference = this.Subtract(denominator, new ReadOnlyCollection<UInt16>(new UInt16[] { 1 }));
            ReadOnlyCollection<UInt16> minimumTestResult = this.Subtract(numerator, maxDifference);

            while (this.IsLessThan(numeratorTestResult, minimumTestResult) || this.IsGreaterThan(numeratorTestResult, numerator))
            {
                if (this.IsLessThan(numeratorTestResult, numerator))
                {
                    floor = lastNumberTried;
                    lastNumberTried = this.GetWholeNumberSomewhereBetween(ceiling, floor, 1);
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

            Tuple<ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>> result;

            if (this.IsEqual(numeratorTestResult, numerator))
            {
                result = new Tuple<ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>>(lastNumberTried, null, null);
            }
            else
            {
                var leftOver = this.Subtract(numerator, numeratorTestResult);
                result = new Tuple<ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>>(lastNumberTried, leftOver, denominator);
            }

#if DEBUG
            if (this.IsGreaterThan(result.Item1, numerator) && this.IsGreaterThan(result.Item1, denominator))
            {
                throw new Exception("MathAlgorithm Division error");
            }
            else if (result.Item1 != default(ReadOnlyCollection<UInt16>) && result.Item1.Count > 1 && result.Item1[result.Item1.Count - 1] == 0)
            {
                throw new Exception("MathAlgorithm Division leading zero error whole number");
            }
            else if (result.Item2 != default(ReadOnlyCollection<UInt16>) && result.Item2.Count > 1 && result.Item2[result.Item2.Count - 1] == 0)
            {
                throw new Exception("MathAlgorithm Division leading zero error numerator");
            }
            else if (result.Item3 != default(ReadOnlyCollection<UInt16>) && result.Item3.Count > 1 && result.Item3[result.Item3.Count - 1] == 0)
            {
                throw new Exception("MathAlgorithm Division leading zero error denominator");
            }
#endif


            return result;
        }


        public Number Divide(UInt16 dividend, UInt16 divisor)
        {
            Number result;
            
            UInt16 remainder;

            if (dividend > divisor)
            {
                remainder = (UInt16)(dividend % divisor);

                UInt16 resultRaw = (UInt16)System.Math.Floor((decimal)dividend / (decimal)divisor);

                result = this.Environment.ConvertToFraction(resultRaw, remainder, divisor);
            }
            else
            {
                result = this.Environment.ConvertToFraction(0, dividend, divisor);
            }

            return result;
        }
        #endregion


        #region Subtract

        public ReadOnlyCollection<UInt16> Subtract(ReadOnlyCollection<UInt16> a, ReadOnlyCollection<UInt16> b)
        {
            if (this.IsLessThan(a, b))
            {
                throw new Exception("Negetive numbers not supported in MathAlgorithm subtract");
            }
            
            Int64 maxPosition = a.Count;
            if (b.Count > maxPosition)
            {
                maxPosition = b.Count;
            }

            // 60 - 90
            var resultSegments = new List<UInt16>();
            Int32 borrow = 0;
            Int32 position = 0;
            while (position < maxPosition)
            {
                Int32 columnValue = borrow;
                borrow = 0;


                if (position < a.Count)
                {
                    columnValue += a[position];
                }

                if (position < b.Count)
                {
                    columnValue -= b[position];
                }

                if (columnValue < 0)
                {
                    borrow -= 1;
                    columnValue += this.Environment.Key.Count;
                }

                resultSegments.Add((UInt16)columnValue);
                position++;
            }

            while (resultSegments.Count > 1 && resultSegments[resultSegments.Count - 1] == 0)
            {
                resultSegments.RemoveAt(resultSegments.Count - 1);
            }
            
            var result = new ReadOnlyCollection<UInt16>(resultSegments);

#if DEBUG
            if (this.IsGreaterThan(result, a) && this.IsGreaterThan(result, b))
            {
                throw new Exception("MathAlgorithm Subtraction error");
            }
            else if (result.Count > 1 && result[result.Count - 1] == 0)
            {
                throw new Exception("MathAlgorithm Subtraction leading zero error");
            }
#endif
            return result;

        }

        #endregion

        public Boolean IsFirst(ReadOnlyCollection<UInt16> number)
        {
            if (number != default(ReadOnlyCollection<UInt16>) && number.Count == 1 && number[0] == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean IsBottom(ReadOnlyCollection<UInt16> number)
        {
            if (number == default(ReadOnlyCollection<UInt16>) || number.Count == 0 || (number.Count == 1 && number[0] == 0))
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


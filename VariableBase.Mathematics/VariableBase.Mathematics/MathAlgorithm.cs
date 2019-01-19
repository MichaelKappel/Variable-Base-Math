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

        public Tuple<ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>> Divide(ReadOnlyCollection<UInt16> numerator, ReadOnlyCollection<UInt16> denominator)
        {
            if (this.IsBottom(denominator))
            {
                return new Tuple<ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>>(numerator, null, null);
            }
            else if (this.IsLessThan(numerator, denominator))
            {
                return new Tuple<ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>>(new ReadOnlyCollection<UInt16>(new UInt16[]{ 0 }), numerator, denominator);
            }


            ReadOnlyCollection<UInt16> floor = this.Environment.KeyNumber[1].Segments;
            ReadOnlyCollection<UInt16> ceiling = numerator;

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

        public bool IsOdd(ReadOnlyCollection<UInt16> a)
        {
            return !this.IsEven(a);
        }

        public bool IsEven(ReadOnlyCollection<UInt16> a)
        {
            if (a[0] == 0)
            {
                return true;
            }
            else if (this.Environment.Key.Count == 2)
            {
                return false;
            }

            UInt16 charIndex = a[0];
            if (charIndex % 2 == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal bool IsPrime(ReadOnlyCollection<UInt16> a)
        {
            return this.GetSmallestDivisor(a) == default(ReadOnlyCollection<UInt16>);
        }


        internal ReadOnlyCollection<UInt16> GetSmallestDivisor(ReadOnlyCollection<UInt16> a)
        {

            if (this.IsBottom(a))
            {
                return a;
            }

            if (this.IsEven(a))
            {
                return this.Environment.SecondNumber.Segments;
            }

            ReadOnlyCollection<UInt16> four = this.Square(this.Environment.SecondNumber.Segments);

            Tuple<ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>> maxNumber = this.Divide(a, four);

            ReadOnlyCollection<UInt16> testNumber = this.Environment.SecondNumber.Segments;
            while (this.IsLessThanOrEqualTo(testNumber, maxNumber.Item1))
            {
                Tuple<ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>, ReadOnlyCollection<UInt16>> currentNumber = this.Divide(a, testNumber);
                if (currentNumber.Item2 == default(ReadOnlyCollection<UInt16>))
                {
                    return testNumber;
                }

                testNumber = this.Add(testNumber, new ReadOnlyCollection<UInt16>(new UInt16[] { 1 }));
                Debug.WriteLine(String.Format("testNumber length {0} in GetSmallestDivisor ", testNumber.Count));
            }

            return default(ReadOnlyCollection<UInt16>);
        }

        public ReadOnlyCollection<UInt16> Square(ReadOnlyCollection<UInt16> a)
        {
            return this.Multiply(a, a);
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


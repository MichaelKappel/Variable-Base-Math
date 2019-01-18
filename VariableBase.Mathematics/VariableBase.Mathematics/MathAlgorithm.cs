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
        public ReadOnlyCollection<Char> Add(ReadOnlyCollection<Char> a, ReadOnlyCollection<Char> b)
        {

            Int32 maxPosition = a.Count;
            if (b.Count > maxPosition)
            {
                maxPosition = b.Count;
            }

            var resultNumber = new List<Char>();
            UInt64 carryOver = 0;
            Int32 position = 0;
            while (position < maxPosition)
            {
                UInt64 columnValue = carryOver;

                if (position < a.Count)
                {
                    columnValue += this.Environment.GetIndex(a[position]);
                }

                if (position < b.Count)
                {
                    columnValue += this.Environment.GetIndex(b[position]);
                }

                Char columnResult;
                if (columnValue >= this.Environment.Base)
                {
                    UInt16 columnResultRaw = (UInt16)(columnValue % this.Environment.Base);
                    columnResult = this.Environment.Key[columnResultRaw];

                    carryOver = (UInt64)(((Decimal)columnValue - (Decimal)columnResultRaw) / (Decimal)this.Environment.Base);
                }
                else
                {
                    columnResult = this.Environment.Key[(UInt16)columnValue];
                    carryOver = 0;
                }

                resultNumber.Add(columnResult);
                position++;
            }

            if (carryOver != 0)
            {
                Char columnResult;
                while (carryOver >= this.Environment.Base)
                {
                    UInt16 columnResultRaw = (UInt16)(carryOver % this.Environment.Base);
                    columnResult = this.Environment.Key[columnResultRaw];
                    resultNumber.Add(columnResult);

                    carryOver = (UInt16)((Decimal)columnResultRaw / (Decimal)this.Environment.Base);
                }

                if (carryOver > 0)
                {
                    columnResult = this.Environment.Key[(UInt16)carryOver];
                    resultNumber.Add(columnResult);
                }
            }

            var result = new ReadOnlyCollection<Char>(resultNumber);

#if DEBUG
            if (this.IsLessThan(result, a) || this.IsLessThan(result, b))
            {
                throw new Exception("MathAlgorithm addtion error");
            }
#endif
            return result;
        }

        public ReadOnlyCollection<Char> GetWholeNumberSomewhereBetween(ReadOnlyCollection<Char> a, ReadOnlyCollection<Char> b, Int64 variance = 0)
        {
            ReadOnlyCollection<Char> result;

            if (a == b)
            {
                result = a;
            }
            else
            {

                ReadOnlyCollection<Char> largerNumber;
                ReadOnlyCollection<Char> smallerNumber;

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

                Decimal firstIndexOfLargerNumber = this.Environment.GetIndex(largerNumber[largerNumber.Count - 1]);
                Decimal firstIndexOfSmallerNumber = this.Environment.GetIndex(smallerNumber[smallerNumber.Count - 1]);

                Decimal firstIndexOfResultRaw = (firstIndexOfLargerNumber + firstIndexOfSmallerNumber) / 2M;

                UInt16 firstIndexOfResult;
                Decimal halfBase;

                if (variance > 0)
                {
                    halfBase = ((Decimal)this.Environment.Base) / 2;
                    firstIndexOfResult = (UInt16)System.Math.Ceiling(firstIndexOfResultRaw);
                }
                else
                {
                    halfBase = ((Decimal)this.Environment.Base) / 2;
                    firstIndexOfResult = (UInt16)System.Math.Floor(firstIndexOfResultRaw);
                }



                if ((largerNumber.Count - smallerNumber.Count <= 1)
                    || (largerNumber.Count - smallerNumber.Count == 2 && firstIndexOfResult <= 1))
                {
                    ReadOnlyCollection<Char> combinedValue = this.Add(largerNumber, smallerNumber);
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
                        result = this.PowerOfBase(this.Environment.Key[firstIndexOfResult], (UInt16)(power + variance));
                    }
                    else
                    {
                        result = this.PowerOfBase(this.Environment.Key[firstIndexOfResult], (UInt16)(power));
                    }

                }
            }
            return result;
        }

        public ReadOnlyCollection<Char> GetAboutHalf(ReadOnlyCollection<Char> number, Int64 variance)
        {
            Decimal halfFirstCharIndexDetail = ((Decimal)this.Environment.GetIndex(number[number.Count - 1])) / 2M;

            Decimal halfBaseIndexDetailed = ((Decimal)this.Environment.Base) / 2M;

            Char[] resultSegments;

            Decimal remainder = 0M;


            if (halfFirstCharIndexDetail >= 1M)
            {
                resultSegments = new Char[number.Count];
            }
            else
            {
                resultSegments = new Char[number.Count - 1];
                remainder = halfBaseIndexDetailed;
            }

            for (var i = resultSegments.Length - 1; i >= 0; i--)
            {
                Decimal charIndex = this.Environment.GetIndex(number[i]);
                Decimal halfCharIndexWithRemainder = (charIndex / 2M) + remainder;
                
                if (i == 0)
                {
                    if (variance > 0 && (UInt16)System.Math.Ceiling(halfCharIndexWithRemainder) < this.Environment.Base)
                    {
                        resultSegments[0] = this.Environment.Key[(Int32)System.Math.Ceiling(halfCharIndexWithRemainder)];
                    }
                    else
                    {
                        resultSegments[0] = this.Environment.Key[(Int32)System.Math.Floor(halfCharIndexWithRemainder)];
                    }
                }
                else
                {
                    UInt16 halfCharIndexWithRemainderIndex = (UInt16)System.Math.Floor(halfCharIndexWithRemainder);
                    if (halfCharIndexWithRemainderIndex >= this.Environment.Base)
                    {
                        Int32 currentSegmentIndex = (Int32)System.Math.Floor(halfBaseIndexDetailed);
                        resultSegments[i] = this.Environment.Key[currentSegmentIndex];
                        remainder = 0M;
                    }
                    else
                    {
                        resultSegments[i] = this.Environment.Key[halfCharIndexWithRemainderIndex];
                        remainder = (halfCharIndexWithRemainder - ((Decimal)halfCharIndexWithRemainderIndex)) * this.Environment.Base;
                    }
                }
            }

            while (resultSegments[resultSegments.Length-1] == this.Environment.Bottom)
            {
                resultSegments = resultSegments.Take(resultSegments.Length - 1).ToArray();
            }

            return new ReadOnlyCollection<Char>(resultSegments);
        }

        public ReadOnlyCollection<Char> PowerOfBase(Char a, UInt16 times)
        {
            return this.PowerOfBase(new ReadOnlyCollection<Char>(new Char[] { a }), times);
        }

        public ReadOnlyCollection<Char> PowerOfBase(ReadOnlyCollection<Char> a, UInt16 times)
        {
            if (a.Count == 1 && a[0] == this.Environment.Bottom)
            {
                return a;
            }

            var segments = new Char[(a.Count + times)];
            for (Int32 i = segments.Length - 1; i >= 0; i--)
            {
                segments[i] = this.Environment.Bottom;
            }
            a.CopyTo(segments, times);
            return new ReadOnlyCollection<Char>(segments);
        }

        public Tuple<ReadOnlyCollection<Char>, ReadOnlyCollection<Char>, ReadOnlyCollection<Char>> Divide(ReadOnlyCollection<Char> numerator, ReadOnlyCollection<Char> denominator)
        {
            if (this.IsBottom(denominator))
            {
                return new Tuple<ReadOnlyCollection<Char>, ReadOnlyCollection<Char>, ReadOnlyCollection<Char>>(numerator, null, null);
            }
            else if (this.IsLessThan(numerator, denominator))
            {
                return new Tuple<ReadOnlyCollection<Char>, ReadOnlyCollection<Char>, ReadOnlyCollection<Char>>(new ReadOnlyCollection<char>(new Char[] { this.Environment.Bottom }), numerator, denominator);
            }


            var floor = new ReadOnlyCollection<Char>(new Char[] { this.Environment.First });
            ReadOnlyCollection<Char> ceiling = numerator;

            ReadOnlyCollection<Char> lastNumberTried = this.GetWholeNumberSomewhereBetween(ceiling, floor);
            ReadOnlyCollection<Char> numeratorTestResult = this.Multiply(lastNumberTried, denominator);

            ReadOnlyCollection<Char> maxDifference = this.Subtract(denominator, new ReadOnlyCollection<Char>(new Char[] { this.Environment.First }));
            ReadOnlyCollection<Char> minimumTestResult = this.Subtract(numerator, maxDifference);

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

            Tuple<ReadOnlyCollection<Char>, ReadOnlyCollection<Char>, ReadOnlyCollection<Char>> result;

            if (this.IsEqual(numeratorTestResult, numerator))
            {
                result = new Tuple<ReadOnlyCollection<Char>, ReadOnlyCollection<Char>, ReadOnlyCollection<Char>>(lastNumberTried, null, null);
            }
            else
            {
                var leftOver = this.Subtract(numerator, numeratorTestResult);
                result = new Tuple<ReadOnlyCollection<Char>, ReadOnlyCollection<Char>, ReadOnlyCollection<Char>>(lastNumberTried, leftOver, denominator);
            }

            return result;
        }

        public bool IsOdd(ReadOnlyCollection<Char> a)
        {
            return !this.IsEven(a);
        }

        public bool IsEven(ReadOnlyCollection<Char> a)
        {
            if (a[0] == this.Environment.Bottom)
            {
                return true;
            }
            else if (this.Environment.Base == 2)
            {
                return false;
            }

            UInt16 charIndex = this.Environment.GetIndex(a[0]);
            if (charIndex % 2 == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal bool IsPrime(ReadOnlyCollection<Char> a)
        {
            return this.GetSmallestDivisor(a) == default(ReadOnlyCollection<Char>);
        }


        internal ReadOnlyCollection<Char> GetSmallestDivisor(ReadOnlyCollection<Char> a)
        {

            if (this.IsBottom(a))
            {
                return a;
            }

            if (this.IsEven(a))
            {
                return this.Environment.SecondNumber.Segments;
            }

            ReadOnlyCollection<Char> four = this.Square(this.Environment.SecondNumber.Segments);

            Tuple<ReadOnlyCollection<Char>, ReadOnlyCollection<Char>, ReadOnlyCollection<Char>> maxNumber = this.Divide(a, four);

            ReadOnlyCollection<Char> testNumber = this.Environment.SecondNumber.Segments;
            while (this.IsLessThanOrEqualTo(testNumber, maxNumber.Item1))
            {
                Tuple<ReadOnlyCollection<Char>, ReadOnlyCollection<Char>, ReadOnlyCollection<Char>> currentNumber = this.Divide(a, testNumber);
                if (currentNumber.Item2 == default(ReadOnlyCollection<Char>))
                {
                    return testNumber;
                }

                testNumber = this.Add(testNumber, new ReadOnlyCollection<Char>(new Char[] { this.Environment.First }));
                Debug.WriteLine(String.Format("testNumber length {0} in GetSmallestDivisor ", testNumber.Count));
            }

            return default(ReadOnlyCollection<Char>);
        }

        public ReadOnlyCollection<Char> Square(ReadOnlyCollection<Char> a)
        {
            return this.Multiply(a, a);
        }

        public Boolean IsEqual(ReadOnlyCollection<Char> a, ReadOnlyCollection<Char> b)
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

        public Boolean IsGreaterThan(ReadOnlyCollection<Char> a, ReadOnlyCollection<Char> b)
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

        public Boolean IsLessThan(ReadOnlyCollection<Char> a, ReadOnlyCollection<Char> b)
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

        public Boolean IsGreaterThanOrEqualTo(ReadOnlyCollection<Char> a, ReadOnlyCollection<Char> b)
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

        public Boolean IsLessThanOrEqualTo(ReadOnlyCollection<Char> a, ReadOnlyCollection<Char> b)
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

        public int CompareTo(ReadOnlyCollection<Char> a, ReadOnlyCollection<Char> b)
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




        #region Add
        //public Number Add(MathEnvironment environment, Fraction a, Fraction b)
        //{
        //    Number denominator = a.Denominator * b.Denominator;
        //    Number numerator = (a.Numerator * b.Denominator) + (b.Numerator * a.Denominator);

        //    return environment.ConvertToFraction(numerator.Segments, denominator.Segments);
        //}

        #endregion

        #region Multiply

        //public Number Multiply(MathEnvironment environment, Fraction a, Fraction b)
        //{
        //    Number denominator = a.Denominator * b.Denominator;
        //    Number numerator = a.Numerator * b.Numerator;

        //    return environment.ConvertToFraction(numerator.Segments, denominator.Segments);
        //}

        public ReadOnlyCollection<Char> Multiply(ReadOnlyCollection<Char> a, Char b)
        {
            if (b == 0 || b == this.Environment.Bottom)
            {
                return new ReadOnlyCollection<char>(new Char[] { this.Environment.Bottom });
            }
            var resultRaw = new List<Char>();

            UInt64 numberIndex = this.Environment.GetIndex(b);

            UInt64 carryOver = 0;
            for (var i = 0; i < a.Count; i++)
            {
                UInt64 segmentIndex = this.Environment.GetIndex(a[i]);

                UInt64 columnTotal = (numberIndex * segmentIndex) + carryOver;

                Char columnPositionResult;
                if (columnTotal >= this.Environment.Base)
                {
                    UInt64 remainder = (columnTotal % this.Environment.Base);
                    columnPositionResult = this.Environment.Key[(UInt16)remainder];
                    carryOver = (columnTotal - remainder) / this.Environment.Base;
                }
                else
                {
                    columnPositionResult = this.Environment.Key[(UInt16)columnTotal];
                    carryOver = 0;
                }

                resultRaw.Add(columnPositionResult);
            }

            while (carryOver > 0)
            {
                Char carryOverResult;
                if (carryOver > this.Environment.Base)
                {
                    UInt16 remainder = (UInt16)(carryOver % (UInt64)this.Environment.Base);
                    carryOverResult = this.Environment.Key[remainder];
                    carryOver = (carryOver - remainder) / this.Environment.Base;
                }
                else
                {
                    carryOverResult = this.Environment.Key[(UInt16)carryOver];
                    carryOver = 0;
                }
                resultRaw.Add(carryOverResult);
            }
            return new ReadOnlyCollection<Char>(resultRaw);
        }

        public ReadOnlyCollection<Char> Multiply(ReadOnlyCollection<Char> a, ReadOnlyCollection<Char> b)
        {
            ReadOnlyCollection<Char> result = new ReadOnlyCollection<Char>(new Char[] { this.Environment.Bottom });

            for (UInt16 i = 0; i < (UInt16)a.Count; i++)
            {
                Char numberSegment = a[i];
                ReadOnlyCollection<Char> currentResult = this.Multiply(b, numberSegment);

                if (i > 0)
                {
                    currentResult = this.PowerOfBase(currentResult, i);
                }

                result = this.Add(currentResult, result);
            }

            return result;
        }

        public Char[] Multiply(Char number1, Char number2)
        {
            UInt16 number1Index = this.Environment.GetIndex(number1);
            UInt16 number2Index = this.Environment.GetIndex(number2);

            Int32 resultIndex = number1Index * number2Index;

            if (resultIndex >= this.Environment.Base)
            {
                Int32 firstNumber = (resultIndex % this.Environment.Base);
                Int32 secondNumber = (resultIndex - firstNumber) / this.Environment.Base;

                return new Char[] { this.Environment.Key[secondNumber], this.Environment.Key[firstNumber] };
            }
            else
            {
                return new Char[] { this.Environment.Key[resultIndex] };
            }
        }

        #endregion

        #region Divide

        public Number Divide(Char dividend, Char divisor)
        {
            Number result;

            UInt16 indexToDivide = this.Environment.GetIndex(dividend);
            UInt16 indexToDivideBy = this.Environment.GetIndex(divisor);

            UInt16 remainder;

            if (indexToDivide > indexToDivideBy)
            {
                remainder = (UInt16)(indexToDivide % indexToDivideBy);

                UInt16 resultRaw = (UInt16)System.Math.Floor((decimal)indexToDivide / (decimal)indexToDivideBy);

                result = this.Environment.ConvertToFraction(resultRaw, remainder, indexToDivideBy);
            }
            else
            {
                result = this.Environment.ConvertToFraction(0, dividend, divisor);
            }

            return result;
        }
        #endregion


        #region Subtract

        public ReadOnlyCollection<Char> Subtract(ReadOnlyCollection<Char> a, ReadOnlyCollection<Char> b)
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
            var resultSegments = new List<Char>();
            Int32 borrow = 0;
            Int32 position = 0;
            while (position < maxPosition)
            {
                Int32 columnValue = borrow;
                borrow = 0;


                if (position < a.Count)
                {
                    columnValue += this.Environment.GetIndex(a[position]);
                }

                if (position < b.Count)
                {
                    columnValue -= this.Environment.GetIndex(b[position]);
                }

                if (columnValue < 0)
                {
                    borrow -= 1;
                    columnValue += this.Environment.Base;
                }

                resultSegments.Add(this.Environment.Key[columnValue]);
                position++;
            }

            this.Environment.ValidateWholeNumber(resultSegments);

            return new ReadOnlyCollection<Char>(resultSegments);

        }

        #endregion

        public Boolean IsBottom(ReadOnlyCollection<Char> number)
        {
            if (number == default(ReadOnlyCollection<Char>) || number.Count == 0 || (number.Count == 1 && number[0] == this.Environment.Bottom))
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


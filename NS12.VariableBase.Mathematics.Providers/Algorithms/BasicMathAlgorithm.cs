using System;

using System.Linq;
using NS12.VariableBase.Mathematics.Common.Models;
using NS12.VariableBase.Mathematics.Providers;
using NS12.VariableBase.Mathematics.Common.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace NS12.VariableBase.Mathematics.Providers.Algorithms
{
    public class BasicMathAlgorithm : IBasicMathAlgorithm<Number>
    {

        public Number AsFraction(IMathEnvironment<Number> environment, ulong numberRaw, ulong numeratorNumber, ulong denominatorRaw)
        {
            Number result;
            NumberSegments number = AsSegments(environment, numberRaw);
            if (numeratorNumber > 0)
            {
                NumberSegments numerator = AsSegments(environment, numeratorNumber);
                NumberSegments denominator = AsSegments(environment, denominatorRaw);

                result = new Number(environment, number, numerator, denominator, false);
            }
            else
            {
                result = new Number(environment, number, default, false);
            }
            return result;
        }


        #region Add
        public NumberSegments Add(IMathEnvironment<Number> environment, decimal a, decimal b)
        {
            decimal resultRaw = a + b;

            return AsSegments(environment, resultRaw);
        }

        public NumberSegments Add(IMathEnvironment<Number> environment, NumberSegments a, NumberSegments b)
        {
            NumberSegments result = default;

            if (IsLessThan(environment, a, b))
            {
                result = Add(environment, b, a);
            }

            if (a.Size == 1 && b.Size == 1)
            {
                result = this.Add(environment, a[0], b[0]);
            }

            if (result == default(NumberSegments))
            {
                var resultNumber = new List<decimal>();
                decimal carryOver = 0;
                int position = 0;
                while (position < a.Size)
                {
                    decimal columnValue = carryOver;

                    if (position < a.Size)
                    {
                        columnValue += a[position];
                    }

                    if (position < b.Size)
                    {
                        columnValue += b[position];
                    }

                    decimal columnResult;
                    if (columnValue >= environment.Base)
                    {
                        decimal columnResultRaw = columnValue % environment.Base;
                        columnResult = columnResultRaw;

                        carryOver = (columnValue - columnResultRaw) / environment.Base;
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
                    decimal columnResult;
                    while (carryOver >= environment.Base)
                    {
                        decimal columnResultRaw = carryOver % environment.Base;
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
            if (IsLessThan(environment, result, a) || IsLessThan(environment, result, b))
            {
                throw new Exception("MathAlgorithm addtion error");
            }

            foreach (decimal segment in result)
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

            NumberSegments reversResult = Subtract(environment, result, a);
            if (IsNotEqual(environment, reversResult, b))
            {
                throw new Exception("Bad addition result could not reverse");
            }
#endif
            return result;
        }

        #endregion

        public NumberSegments ConvertToBase10(IMathEnvironment<Number> base10Environment, IMathEnvironment<Number> currentEnvironment, NumberSegments segments)
        {
            NumberSegments result = base10Environment.GetNumber(0).Whole;
            for (int iSegments = 0; iSegments < segments.Length; iSegments++)
            {
                NumberSegments currentNumber = base10Environment.GetNumber(1).Whole;
                for (int i2 = 0; i2 < iSegments; i2++)
                {
                    currentNumber = Multiply(base10Environment, currentNumber, currentEnvironment.Base);
                }

                currentNumber = this.Multiply(base10Environment, currentNumber, segments[iSegments]);
                result = Add(base10Environment, result, currentNumber);
            }
            return result;
        }

        public NumberSegments GetWholeNumberSomewhereBetween(IMathEnvironment<Number> environment, NumberSegments a, NumberSegments b, decimal variance = 0)
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

                if (IsGreaterThan(environment, a, b))
                {
                    largerNumber = a;
                    smallerNumber = b;
                }
                else
                {
                    largerNumber = b;
                    smallerNumber = a;
                }

                decimal firstIndexOfLargerNumber = largerNumber[largerNumber.Size - 1];
                decimal firstIndexOfSmallerNumber = smallerNumber[smallerNumber.Size - 1];

                decimal firstIndexOfResultRaw = (firstIndexOfLargerNumber + firstIndexOfSmallerNumber) / 2M;

                decimal firstIndexOfResult;
                decimal halfBase;

                if (variance > 0)
                {
                    halfBase = environment.Base / 2;
                    firstIndexOfResult = Math.Ceiling(firstIndexOfResultRaw);
                }
                else
                {
                    halfBase = environment.Base / 2;
                    firstIndexOfResult = Math.Floor(firstIndexOfResultRaw);
                }



                if (largerNumber.Size - smallerNumber.Size <= 1
                    || largerNumber.Size - smallerNumber.Size == 2 && firstIndexOfResult <= 1)
                {
                    NumberSegments combinedValue = Add(environment, largerNumber, smallerNumber);
                    result = GetAboutHalf(environment, combinedValue, variance);
                }
                else
                {
                    decimal somewhereBetweenPower = (largerNumber.Size - smallerNumber.Size) / 2M + smallerNumber.Size;

                    decimal power;
                    if (variance > 0)
                    {
                        power = Math.Ceiling(somewhereBetweenPower);
                    }
                    else
                    {
                        power = Math.Floor(somewhereBetweenPower);
                    }
                    decimal powerWithVariance = power + variance;
                    result = PowerOfBase(environment, AsSegments(environment, firstIndexOfResult), powerWithVariance);
                    while (IsGreaterThan(environment, result, a))
                    {
                        powerWithVariance -= 1;
                        result = PowerOfBase(environment, AsSegments(environment, firstIndexOfResult), powerWithVariance);
                    }
                }
            }
#if DEBUG
            if (IsGreaterThan(environment, a, b) && (IsGreaterThan(environment, result, a) || IsLessThan(environment, result, b)))
            {
                throw new Exception("MathAlgorithm GetWholeNumberSomewhereBetween error 1");
            }
            else if (IsLessThan(environment, a, b) && (IsGreaterThan(environment, result, b) || IsLessThan(environment, result, a)))
            {
                throw new Exception("MathAlgorithm GetWholeNumberSomewhereBetween Error 2");
            }

            foreach (decimal segment in result)
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

        public NumberSegments GetAboutHalf(IMathEnvironment<Number> environment, NumberSegments number, decimal variance)
        {
            decimal halfFirstCharIndexDetail = number[number.Size - 1] / 2M;

            decimal halfBaseIndexDetailed = environment.Base / 2M;

            decimal[] resultSegments;

            decimal remainder = 0M;


            if (halfFirstCharIndexDetail >= 1)
            {
                resultSegments = new decimal[number.Length];
            }
            else
            {
                resultSegments = new decimal[number.Length - 1];
                remainder = halfBaseIndexDetailed;
            }

            for (int i = resultSegments.Length - 1; i >= 0; i--)
            {
                decimal charIndex = number[i];
                decimal halfCharIndexWithRemainder = charIndex / 2M + remainder;

                if (i == 0)
                {
                    if (variance > 0 && Math.Ceiling(halfCharIndexWithRemainder) < environment.Base)
                    {
                        resultSegments[0] = Math.Ceiling(halfCharIndexWithRemainder);
                    }
                    else
                    {
                        resultSegments[0] = Math.Floor(halfCharIndexWithRemainder);
                    }
                }
                else
                {
                    decimal halfCharIndexWithRemainderIndex = Math.Floor(halfCharIndexWithRemainder);
                    if (halfCharIndexWithRemainderIndex >= environment.Base)
                    {
                        decimal currentSegmentIndex = Math.Floor(halfBaseIndexDetailed);
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

            while (resultSegments[resultSegments.Length - 1] == 0)
            {
                resultSegments = resultSegments.Take(resultSegments.Length - 1).ToArray();
            }
            var result = new NumberSegments(resultSegments);

#if DEBUG
            if (IsGreaterThan(environment, result, number))
            {
                throw new Exception("MathAlgorithm GetAboutHalf error");
            }
            foreach (decimal segment in result)
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

        public NumberSegments GetAboutHalf(IMathEnvironment<Number> environment, NumberSegments a, NumberSegments b, decimal variance)
        {
            NumberSegments x = Add(environment, a, b);
            (NumberSegments Whole, NumberSegments Numerator, NumberSegments Denominator) rawResult = Divide(environment, x, environment.GetNumber(2).Whole);

            NumberSegments result;
            if (variance == 1 && rawResult.Numerator != default(NumberSegments))
            {
                result = Add(environment, rawResult.Whole, environment.GetNumber(1).Whole);
            }
            else if (variance == -1 || rawResult.Numerator == default(NumberSegments))
            {
                result = rawResult.Whole;
            }
            else
            {
                NumberSegments doubleNumerator = Multiply(environment, rawResult.Numerator, 2);
                if (IsGreaterThanOrEqualTo(environment, doubleNumerator, rawResult.Numerator))
                {
                    result = Add(environment, rawResult.Whole, environment.GetNumber(1).Whole);
                }
                else
                {
                    result = rawResult.Whole;
                }
            }

#if DEBUG
            if (IsGreaterThan(environment, a, b) && (IsGreaterThan(environment, result, a) || IsLessThan(environment, result, b)))
            {
                throw new Exception("MathAlgorithm GetWholeNumberSomewhereBetween error 1");
            }
            else if (IsLessThan(environment, a, b) && (IsLessThan(environment, result, a) || IsGreaterThan(environment, result, b)))
            {
                throw new Exception("MathAlgorithm GetWholeNumberSomewhereBetween Error 2");
            }

            foreach (decimal segment in result)
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

        public NumberSegments AsSegments(IMathEnvironment<Number> environment, decimal rawDouble)
        {
            var resultRaw = new List<decimal>();
            if (rawDouble == 0)
            {
                resultRaw.Add(0);
            }
            else
            {

                decimal carryOver = rawDouble;
                while (carryOver > 0)
                {
                    if (carryOver >= environment.Base)
                    {
                        decimal columnResultRaw = 0;
                        columnResultRaw = carryOver % environment.Base;
                        resultRaw.Add(columnResultRaw);
                        carryOver = (carryOver - columnResultRaw) / environment.Base;
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

        //public NumberSegments PowerOfBase(IMathEnvironment<Number> environment, Decimal a, Decimal times)
        //{
        //    return this.PowerOfBase(environment,  a , times);
        //}

        public NumberSegments PowerOfBase(IMathEnvironment<Number> environment, NumberSegments a, decimal times)
        {
            if (a.Size == 1 && a[0] == 0)
            {
                return a;
            }

            var segments = new decimal[(int)(a.Size + times)];
            for (int i = segments.Length - 1; i >= 0; i--)
            {
                segments[i] = 0;
            }
            a.CopyTo(segments);
#if DEBUG
            foreach (decimal segment in segments)
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

            if (segments.Length > 0 && segments[segments.Length - 1] == 0)
            {
                throw new Exception("Bad PowerOfBase leading zero");
            }

#endif
            return new NumberSegments(segments);
        }

        public bool IsOdd(IMathEnvironment<Number> environment, NumberSegments a)
        {
            return !IsEven(environment, a);
        }

        public bool IsEven(IMathEnvironment<Number> environment, NumberSegments a)
        {
            bool isEven = false;
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
                (NumberSegments Whole, NumberSegments Numerator, NumberSegments Denominator) half = Divide(environment, a, environment.GetNumber(2).Whole);
                if (half.Numerator == default(NumberSegments))
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
            //            Tuple<NumberSegments, NumberSegments, NumberSegments> half = this.Divide(environment,a, environment.GetNumber(2).Segments);
            //            if (half.Numerator == default(NumberSegments) && !isEven)
            //            {
            //                throw new Exception("IsEven should be even but is not");
            //            }
            //            else if(half.Numerator != default(NumberSegments) && isEven)
            //            {
            //                throw new Exception("IsEven should NOT be even but is");
            //            }
            //#endif
            return isEven;
        }

        public NumberSegments Square(IMathEnvironment<Number> environment, NumberSegments a)
        {
            return Multiply(environment, a, a);
        }

        public (NumberSegments Whole, NumberSegments Numerator, NumberSegments Denominator) SquareRoot(IMathEnvironment<Number> environment, decimal number)
        {
            decimal x = (decimal)Math.Sqrt((double)number);
            decimal remainder = x % 1;
            if (remainder == 0)
            {
                return (new NumberSegments(new decimal[] { x }), null, null);
            }
            else
            {

                NumberSegments wholeNumber = AsSegments(environment, Math.Floor(x));
                NumberSegments numerator = AsSegments(environment, Math.Floor(remainder * 100000));
                NumberSegments denominator = AsSegments(environment, Math.Floor(number * 100000));
                return (wholeNumber, numerator, denominator);
            }
        }

        public (NumberSegments Whole, NumberSegments Numerator, NumberSegments Denominator) SquareRoot(IMathEnvironment<Number> environment, NumberSegments number)
        {

            if (IsBottom(number))
            {
                return default;
            }
            else if (number.Size == 1)
            {
                return this.SquareRoot(environment, number[0]);
            }
            else if (IsLessThanOrEqualTo(environment, number, AsSegments(environment, 3)))
            {
                return (environment.GetNumber(1).Whole, default(NumberSegments), default(NumberSegments));
            }

            NumberSegments floor = environment.GetNumber(2).Whole;
            NumberSegments ceiling = Divide(environment, number, environment.GetNumber(2).Whole).Whole;

            NumberSegments lastNumberTried = GetWholeNumberSomewhereBetween(environment, ceiling, floor);
            NumberSegments squareTestResult = Square(environment, lastNumberTried);

            NumberSegments maxDifference = Subtract(environment, lastNumberTried, new NumberSegments(new decimal[] { 1 }));

            while (IsNotEqual(environment, squareTestResult, number) && IsGreaterThan(environment, Subtract(environment, ceiling, floor), environment.GetNumber(1).Whole))
            {
                NumberSegments numberBeforLast = lastNumberTried;
                if (IsLessThan(environment, squareTestResult, number))
                {
                    floor = lastNumberTried;
                    lastNumberTried = GetWholeNumberSomewhereBetween(environment, ceiling, floor);
                    if (IsEqual(environment, numberBeforLast, lastNumberTried))
                    {
                        lastNumberTried = Add(environment, lastNumberTried, environment.GetNumber(1).Whole);
                    }
                }
                else if (IsGreaterThan(environment, squareTestResult, number))
                {
                    ceiling = lastNumberTried;
                    lastNumberTried = GetWholeNumberSomewhereBetween(environment, ceiling, floor, -1);
                    if (IsEqual(environment, numberBeforLast, lastNumberTried))
                    {
                        lastNumberTried = Subtract(environment, lastNumberTried, environment.GetNumber(1).Whole);
                    }
                }
                squareTestResult = Square(environment, lastNumberTried);

            }

            (NumberSegments Whole, NumberSegments Numerator, NumberSegments Denominator) result;

            if (IsGreaterThan(environment, number, squareTestResult))
            {
                NumberSegments leftOver = Subtract(environment, number, squareTestResult);
                result = (lastNumberTried, leftOver, number);

            }
            else if (IsGreaterThan(environment, squareTestResult, number))
            {
                NumberSegments wholeNumber = Subtract(environment, lastNumberTried, environment.GetNumber(0).Whole);
                NumberSegments leftOver = Subtract(environment, squareTestResult, number);
                result = (wholeNumber, leftOver, number);
            }
            else
            {
                result = (lastNumberTried, null, null);
            }
#if DEBUG
            foreach (decimal segment in result.Whole)
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

            if (result.Numerator != default(NumberSegments))
            {
                foreach (decimal segment in result.Numerator)
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

                foreach (decimal segment in result.Denominator)
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

        public bool IsEqual(IMathEnvironment<Number> environment, NumberSegments a, NumberSegments b)
        {
            if (CompareTo(environment, a, b) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsNotEqual(IMathEnvironment<Number> environment, NumberSegments a, NumberSegments b)
        {
            if (CompareTo(environment, a, b) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsGreaterThan(IMathEnvironment<Number> environment, NumberSegments a, NumberSegments b)
        {
            if (CompareTo(environment, a, b) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsLessThan(IMathEnvironment<Number> environment, NumberSegments a, NumberSegments b)
        {
            if (CompareTo(environment, a, b) < 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsGreaterThanOrEqualTo(IMathEnvironment<Number> environment, NumberSegments a, NumberSegments b)
        {
            if (CompareTo(environment, a, b) >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsLessThanOrEqualTo(IMathEnvironment<Number> environment, NumberSegments a, NumberSegments b)
        {
            if (CompareTo(environment, a, b) <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int CompareTo(IMathEnvironment<Number> environment, NumberSegments a, NumberSegments b)
        {
            int result = 0;
            if (object.ReferenceEquals(a, default(NumberSegments)) || a.Size == 0)
            {
                a = new NumberSegments(new decimal[] { 0 });
            }

            if (object.ReferenceEquals(b, default(NumberSegments)) || b.Size == 0)
            {
                b = new NumberSegments(new decimal[] { 0 });
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
                for (int i = (int)a.Size - 1; i >= 0; i--)
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

        public NumberSegments Multiply(IMathEnvironment<Number> environment, NumberSegments a, decimal b)
        {
            if (b == 0)
            {
                return new NumberSegments(new decimal[] { 0 });
            }
            var resultRaw = new List<decimal>();

            decimal numberIndex = b;

            decimal carryOver = 0;
            for (int i = 0; i < a.Size; i++)
            {
                decimal segmentIndex = a[i];

                decimal columnTotal = numberIndex * segmentIndex + carryOver;

                decimal columnPositionResult;
                if (columnTotal >= environment.Base)
                {
                    decimal remainder = columnTotal % environment.Base;
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
                decimal carryOverResult;
                if (carryOver >= environment.Base)
                {
                    decimal remainder = carryOver % environment.Base;
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
            foreach (decimal segment in resultRaw)
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

        public NumberSegments Multiply(IMathEnvironment<Number> environment, NumberSegments a, NumberSegments b)
        {
            NumberSegments result = new NumberSegments(new decimal[] { 0 });
            if (IsBottom(a) || IsBottom(b))
            {
                return a;
            }
            else if (IsFirst(a))
            {
                return b;
            }
            else if (IsFirst(b))
            {
                return a;
            }
            else if (a.Size == 1 && b.Size == 1)
            {
                return this.Multiply(environment, a[0], b[0]);
            }

            for (int i = 0; i < a.Size; i++)
            {
                decimal numberSegment = a[i];
                NumberSegments currentResult = Multiply(environment, b, numberSegment);

                if (i > 0)
                {
                    currentResult = PowerOfBase(environment, currentResult, i);
                }

                result = Add(environment, currentResult, result);
            }

#if DEBUG
            if (IsLessThan(environment, result, a) || IsLessThan(environment, result, b))
            {
                throw new Exception("MathAlgorithm Multiplication error");
            }
            else if (result.Size > 1 && result[result.Size - 1] == 0)
            {
                throw new Exception("MathAlgorithm Multiplication leading zero error");
            }

            foreach (decimal segment in result)
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

        public NumberSegments Multiply(IMathEnvironment<Number> environment, decimal a, decimal b)
        {
            decimal[] result;

            decimal resultIndex = a * b;

            if (resultIndex >= environment.Base)
            {
                decimal firstNumber = resultIndex % environment.Base;
                decimal secondNumber = (resultIndex - firstNumber) / environment.Base;

                result = new decimal[] { firstNumber, secondNumber };
            }
            else
            {
                result = new decimal[] { resultIndex };
            }

#if DEBUG
            foreach (decimal segment in result)
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

        public (NumberSegments Whole, NumberSegments Numerator, NumberSegments Denominator) Divide(IMathEnvironment<Number> environment, NumberSegments numerator, NumberSegments denominator, NumberSegments hint = default)
        {

#if DEBUG
            Debug.WriteLine("Division start {0} / {1}", string.Join(',', numerator.Reverse()), string.Join(',', denominator.Reverse()));
            foreach (decimal segment in numerator)
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

            foreach (decimal segment in denominator)
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
            if (IsBottom(denominator))
            {
                return (numerator, null, null);
            }
            else if (IsEqual(environment, numerator, denominator))
            {
                return (new NumberSegments(new decimal[] { 1 }), null, null);
            }
            else if (IsLessThan(environment, numerator, denominator))
            {
                return (new NumberSegments(new decimal[] { 0 }), numerator, denominator);
            }
            else if (numerator.Size == 1 && denominator.Size == 1)
            {
                return this.Divide(environment, numerator[0], denominator[0]);
            }
            else if (denominator.Size == 1)
            {
                return this.Divide(environment, numerator, denominator[0]);
            }

            NumberSegments floor = hint == default(NumberSegments) || IsGreaterThan(environment, denominator, hint) ? environment.GetNumber(1).Whole : GetAboutHalf(environment, hint, environment.GetNumber(1).Whole, -1);
            NumberSegments ceiling = hint == default(NumberSegments) || IsGreaterThan(environment, denominator, hint) ? numerator : GetAboutHalf(environment, hint, numerator, 1);

            NumberSegments lastNumberTried = GetWholeNumberSomewhereBetween(environment, ceiling, floor);
            NumberSegments numeratorTestResult = Multiply(environment, lastNumberTried, denominator);

            NumberSegments maxDifference = Subtract(environment, denominator, new NumberSegments(new decimal[] { 1 }));
            NumberSegments minimumTestResult = Subtract(environment, numerator, maxDifference);

            NumberSegments lastNumeratorTestResult = environment.GetNumber(0).Whole;

            while (IsLessThan(environment, numeratorTestResult, minimumTestResult) || IsGreaterThan(environment, numeratorTestResult, numerator))
            {
                lastNumeratorTestResult = numeratorTestResult;
                if (IsLessThan(environment, numeratorTestResult, numerator))
                {
                    floor = lastNumberTried;
                    lastNumberTried = GetWholeNumberSomewhereBetween(environment, ceiling, floor);
                }
                else if (IsGreaterThan(environment, numeratorTestResult, numerator))
                {
                    ceiling = lastNumberTried;
                    lastNumberTried = GetWholeNumberSomewhereBetween(environment, ceiling, floor, -1);
                }

                if (IsGreaterThanOrEqualTo(environment, floor, ceiling))
                {
                    floor = GetWholeNumberSomewhereBetween(environment, ceiling, environment.GetNumber(1).Whole);
                }
                else if (IsLessThanOrEqualTo(environment, ceiling, floor))
                {
                    ceiling = GetWholeNumberSomewhereBetween(environment, floor, numerator);
                }

                numeratorTestResult = Multiply(environment, lastNumberTried, denominator);
            }

            (NumberSegments Whole, NumberSegments Numerator, NumberSegments Denominator) result;

            if (IsEqual(environment, numeratorTestResult, numerator))
            {
                result = (lastNumberTried, default(NumberSegments), default(NumberSegments));
            }
            else
            {
                NumberSegments leftOver = Subtract(environment, numerator, numeratorTestResult);
                result = (lastNumberTried, leftOver, denominator);
            }

#if DEBUG
            if (IsGreaterThan(environment, result.Whole, numerator) && IsGreaterThan(environment, result.Whole, denominator))
            {
                throw new Exception("MathAlgorithm Division error");
            }
            else if (result.Whole != default(NumberSegments) && result.Whole.Size > 1 && result.Whole[result.Whole.Size - 1] == 0)
            {
                throw new Exception("MathAlgorithm Division leading zero error whole number");
            }
            else if (result.Numerator != default(NumberSegments) && result.Numerator.Size > 1 && result.Numerator[result.Numerator.Size - 1] == 0)
            {
                throw new Exception("MathAlgorithm Division leading zero error numerator");
            }
            else if (result.Denominator != default(NumberSegments) && result.Denominator.Size > 1 && result.Denominator[result.Denominator.Size - 1] == 0)
            {
                throw new Exception("MathAlgorithm Division leading zero error denominator");
            }

            foreach (decimal segment in result.Whole)
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

            if (result.Numerator != default(NumberSegments))
            {
                foreach (decimal segment in result.Numerator)
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

                foreach (decimal segment in result.Denominator)
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

            Debug.WriteLine("Division result {0}", string.Join(',', result.Whole.Reverse()));

            if (result.Numerator != default(NumberSegments))
            {
                Debug.WriteLine("Division remainder {0} / {1}", string.Join(',', result.Numerator.Reverse()), string.Join(',', result.Denominator.Reverse()));
            }
#endif
            return result;
        }


        public (NumberSegments Whole, NumberSegments Numerator, NumberSegments Denominator) Divide(IMathEnvironment<Number> environment, decimal dividend, decimal divisor)
        {
            (NumberSegments Whole, NumberSegments Numerator, NumberSegments Denominator) result;

            decimal remainder;

            if (dividend > divisor)
            {
                remainder = dividend % divisor;

                decimal resultRaw = Math.Floor(dividend / divisor);

                if (remainder == 0)
                {
                    result = (new NumberSegments(new decimal[] { resultRaw }), default(NumberSegments), default(NumberSegments));
                }
                else
                {
                    result = (new NumberSegments(new decimal[] { resultRaw }), new NumberSegments(new decimal[] { remainder }), new NumberSegments(new decimal[] { divisor }));
                }

            }
            else
            {
                result = (new NumberSegments(new decimal[] { 0 }), new NumberSegments(new decimal[] { dividend }), new NumberSegments(new decimal[] { divisor }));
            }

#if DEBUG
            foreach (decimal segment in result.Whole)
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

            if (result.Numerator != default(NumberSegments))
            {
                foreach (decimal segment in result.Numerator)
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

                foreach (decimal segment in result.Denominator)
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

        public (NumberSegments Whole, NumberSegments Numerator, NumberSegments Denominator) Divide(IMathEnvironment<Number> environment, NumberSegments dividend, decimal divisor)
        {
            if (dividend.Size == 1)
            {
                return this.Divide(environment, dividend[0], divisor);
            }

            (NumberSegments Whole, NumberSegments Numerator, NumberSegments Denominator) result;

            decimal remainder = 0M;

            decimal[] workingTotal = new decimal[dividend.Length];

            for (int i = dividend.Length - 1; i >= 0; i--)
            {
                decimal currentDividend = dividend[i] + remainder * environment.Base;
                decimal currentRawTotalWhole = Math.Floor(currentDividend / divisor);
                decimal currentRemaninder = currentDividend - currentRawTotalWhole * divisor;

                workingTotal[i] = currentRawTotalWhole;

                remainder = currentRemaninder;
            }

            var resultRaw = new List<decimal>();

            bool skip = true;
            for (int i = workingTotal.Count() - 1; i >= 0; i--)
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
                result = (new NumberSegments(resultRaw), default(NumberSegments), default(NumberSegments));
            }
            else
            {
                result = (new NumberSegments(resultRaw), new NumberSegments(new decimal[] { remainder }), new NumberSegments(new decimal[] { divisor }));
            }

#if DEBUG
            foreach (decimal segment in result.Whole)
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

            if (result.Numerator == default(NumberSegments))
            {
                NumberSegments reverseCheck = Multiply(environment, result.Whole, divisor);
                if (!IsEqual(environment, reverseCheck, dividend))
                {
                    throw new Exception(string.Format("Division Error {0} != {1} ", string.Join(',', reverseCheck.Reverse()), string.Join(',', dividend.Reverse())));
                }
            }
            else
            {
                foreach (decimal segment in result.Numerator)
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

                foreach (decimal segment in result.Denominator)
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

                NumberSegments reverseCheck = Add(environment, Multiply(environment, result.Whole, divisor), result.Numerator);
                if (IsNotEqual(environment, reverseCheck, dividend))
                {
                    throw new Exception(string.Format("Division Error {0} != {1} ", string.Join(',', reverseCheck.Reverse()), string.Join(',', dividend.Reverse())));
                }
            }

#endif


            return result;
        }
        #endregion


        #region Subtract
        public decimal Subtract(IMathEnvironment<Number> environment, decimal a, decimal b)
        {
            decimal resultRaw = a - b;

            return resultRaw;
        }

        public NumberSegments Subtract(IMathEnvironment<Number> environment, NumberSegments a, NumberSegments b)
        {
            if (IsLessThan(environment, a, b))
            {
                throw new Exception("Negetive numbers not supported in MathAlgorithm subtract");
            }
            else if (a.Size == 1 && b.Size == 1)
            {
                return new NumberSegments(new decimal[] { this.Subtract(environment, a[0], b[0]) });
            }

            decimal maxPosition = a.Size;
            if (b.Size > maxPosition)
            {
                maxPosition = b.Size;
            }

            // 60 - 90
            var resultSegments = new List<decimal>();
            decimal borrow = 0;
            decimal position = 0;
            while (position < maxPosition)
            {
                decimal columnValue = borrow;
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
            if (IsGreaterThan(environment, result, a) && IsGreaterThan(environment, result, b))
            {
                throw new Exception("MathAlgorithm Subtraction error");
            }
            else if (result.Size > 1 && result[result.Size - 1] == 0)
            {
                throw new Exception("MathAlgorithm Subtraction leading zero error");
            }

            foreach (decimal segment in result)
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

        public bool IsFirst(NumberSegments number)
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

        public bool IsBottom(NumberSegments number)
        {
            if (number == default(NumberSegments) || number.Size == 0 || number.Size == 1 && number[0] == 0)
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


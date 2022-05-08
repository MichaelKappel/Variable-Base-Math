using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Common.Models;
using Common.Interfaces;

namespace VariableBase.Mathematics.Algorithms
{
    public class BasicMathAlgorithm : IBasicMathAlgorithm<Number>
    {

        public Number AsFraction(IMathEnvironment<Number> environment, UInt64 numberRaw, UInt64 numeratorNumber, UInt64 denominatorRaw)
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
        public NumberSegments Add(IMathEnvironment<Number> environment, Decimal a, Decimal b)
        {
            Decimal resultRaw = a + b;

            return this.AsSegments(environment, resultRaw);
        }

        public NumberSegments Add(IMathEnvironment<Number> environment, NumberSegments a, NumberSegments b)
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
       
        public NumberSegments ConvertToBase10(IMathEnvironment<Number> base10Environment, IMathEnvironment<Number> currentEnvironment, NumberSegments segments)
        {
            NumberSegments result = base10Environment.GetNumber(0).Segments;
            for (Int32 iSegments = 0; iSegments < segments.Length; iSegments++)
            {
                NumberSegments currentNumber = base10Environment.GetNumber(1).Segments;
                for (Int32 i2 = 0; i2 < iSegments; i2++)
                {
                    currentNumber = this.Multiply(base10Environment, currentNumber, currentEnvironment.Base);
                }

                currentNumber = this.Multiply(base10Environment, currentNumber, segments[iSegments]);
                result = this.Add(base10Environment, result, currentNumber);
            }
            return result;
        }

        public NumberSegments GetWholeNumberSomewhereBetween(IMathEnvironment<Number> environment, NumberSegments a, NumberSegments b, Decimal variance = 0)
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
                    result = this.PowerOfBase(environment, this.AsSegments(environment, firstIndexOfResult), powerWithVariance);
                    while (this.IsGreaterThan(environment,result, a))
                    {
                        powerWithVariance -= 1;
                        result = this.PowerOfBase(environment, this.AsSegments(environment, firstIndexOfResult), powerWithVariance);
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

        public NumberSegments GetAboutHalf(IMathEnvironment<Number> environment, NumberSegments number, Decimal variance)
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

            for (Int32 i = resultSegments.Length - 1; i >= 0; i--)
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

        public NumberSegments GetAboutHalf(IMathEnvironment<Number> environment, NumberSegments a, NumberSegments b, Decimal variance)
        {
            NumberSegments x = this.Add(environment,a, b);
            (NumberSegments Whole, NumberSegments Numerator, NumberSegments Denominator) rawResult = this.Divide(environment, x, environment.GetNumber(2).Segments);

            NumberSegments result;
            if (variance == 1 && rawResult.Numerator != default(NumberSegments))
            {
                result = this.Add(environment,rawResult.Whole, environment.GetNumber(1).Segments);
            }
            else if (variance == -1 || rawResult.Numerator == default(NumberSegments))
            {
                result = rawResult.Whole;
            }
            else
            {
                NumberSegments doubleNumerator = this.Multiply(environment,rawResult.Numerator, 2);
                if (this.IsGreaterThanOrEqualTo(environment, doubleNumerator, rawResult.Numerator))
                {
                    result = this.Add(environment,rawResult.Whole, environment.GetNumber(1).Segments);
                }
                else
                {
                    result = rawResult.Whole;
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

        public NumberSegments AsSegments(IMathEnvironment<Number> environment, Decimal rawDouble)
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

        //public NumberSegments PowerOfBase(IMathEnvironment<Number> environment, Decimal a, Decimal times)
        //{
        //    return this.PowerOfBase(environment,  a , times);
        //}

        public NumberSegments PowerOfBase(IMathEnvironment<Number> environment, NumberSegments a, Decimal times)
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
            a.CopyTo(segments);
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

            if (segments.Length > 0 && segments[segments.Length-1] == 0)
            {
                throw new Exception("Bad PowerOfBase leading zero");
            }

#endif
            return new NumberSegments(segments);
        }

        public bool IsOdd(IMathEnvironment<Number> environment, NumberSegments a)
        {
            return !this.IsEven(environment, a);
        }

        public bool IsEven(IMathEnvironment<Number> environment, NumberSegments a)
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
                (NumberSegments Whole, NumberSegments Numerator, NumberSegments Denominator) half = this.Divide(environment, a, environment.GetNumber(2).Segments);
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
            return this.Multiply(environment,a, a);
        }

        public (NumberSegments Whole, NumberSegments Numerator, NumberSegments Denominator) SquareRoot(IMathEnvironment<Number> environment, Decimal number)
        {
            Decimal x = (Decimal)System.Math.Sqrt((Double)number);
            Decimal remainder = (x % 1);
            if (remainder == 0)
            {
                return (new NumberSegments(new Decimal[] { x }), null, null);
            }
            else
            {

                NumberSegments wholeNumber = this.AsSegments(environment, Math.Floor(x));
                NumberSegments numerator = this.AsSegments(environment, Math.Floor(remainder * 100000));
                NumberSegments denominator = this.AsSegments(environment, Math.Floor(number * 100000));
                return (wholeNumber, numerator, denominator);
            }
        }

        public (NumberSegments Whole, NumberSegments Numerator, NumberSegments Denominator) SquareRoot(IMathEnvironment<Number> environment, NumberSegments number)
        {

            if (this.IsBottom(number))
            {
                return default((NumberSegments Whole, NumberSegments Numerator, NumberSegments Denominator));
            }
            else if (number.Size == 1)
            {
                return this.SquareRoot(environment, number[0]);
            }
            else if (this.IsLessThanOrEqualTo(environment, number, this.AsSegments(environment, 3)))
            {
                return (environment.GetNumber(1).Segments, default(NumberSegments), default(NumberSegments));
            }
            
            NumberSegments floor = environment.GetNumber(2).Segments;
            NumberSegments ceiling = this.Divide(environment,number, environment.GetNumber(2).Segments).Whole;

            NumberSegments lastNumberTried = this.GetWholeNumberSomewhereBetween(environment, ceiling, floor);
            NumberSegments squareTestResult = this.Square(environment, lastNumberTried);

            NumberSegments maxDifference = this.Subtract(environment,lastNumberTried, new NumberSegments(new Decimal[] { 1 }));

            while (this.IsNotEqual(environment, squareTestResult, number) && this.IsGreaterThan(environment,this.Subtract(environment,ceiling, floor), environment.GetNumber(1).Segments))
            {
                NumberSegments numberBeforLast = lastNumberTried;
                if (this.IsLessThan(environment,squareTestResult, number))
                {
                    floor = lastNumberTried;
                    lastNumberTried = this.GetWholeNumberSomewhereBetween(environment, ceiling, floor);
                    if(this.IsEqual(environment, numberBeforLast, lastNumberTried))
                    {
                        lastNumberTried = this.Add(environment,lastNumberTried, environment.GetNumber(1).Segments);
                    }
                }
                else if (this.IsGreaterThan(environment,squareTestResult, number))
                {
                    ceiling = lastNumberTried;
                    lastNumberTried = this.GetWholeNumberSomewhereBetween(environment, ceiling, floor, -1);
                    if (this.IsEqual(environment, numberBeforLast, lastNumberTried))
                    {
                        lastNumberTried = this.Subtract(environment,lastNumberTried, environment.GetNumber(1).Segments);
                    }
                }
                squareTestResult = this.Square(environment, lastNumberTried);
                
            }

            (NumberSegments Whole, NumberSegments Numerator, NumberSegments Denominator) result;

            if (this.IsGreaterThan(environment,number, squareTestResult))
            {
                NumberSegments leftOver = this.Subtract(environment,number, squareTestResult);
                result = (lastNumberTried, leftOver, number);

            }
            else if (this.IsGreaterThan(environment,squareTestResult, number))
            {
                NumberSegments wholeNumber = this.Subtract(environment,lastNumberTried, environment.GetNumber(0).Segments);
                NumberSegments leftOver = this.Subtract(environment,squareTestResult, number);
                result = (wholeNumber, leftOver, number);
            }
            else
            {
                result = (lastNumberTried, null, null);
            }
#if DEBUG
            foreach (Decimal segment in result.Whole)
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
                foreach (Decimal segment in result.Numerator)
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

                foreach (Decimal segment in result.Denominator)
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

        public Boolean IsEqual(IMathEnvironment<Number> environment, NumberSegments a, NumberSegments b)
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

        public Boolean IsNotEqual(IMathEnvironment<Number> environment, NumberSegments a, NumberSegments b)
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

        public Boolean IsGreaterThan(IMathEnvironment<Number> environment, NumberSegments a, NumberSegments b)
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

        public Boolean IsLessThan(IMathEnvironment<Number> environment, NumberSegments a, NumberSegments b)
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

        public Boolean IsGreaterThanOrEqualTo(IMathEnvironment<Number> environment, NumberSegments a, NumberSegments b)
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

        public Boolean IsLessThanOrEqualTo(IMathEnvironment<Number> environment, NumberSegments a, NumberSegments b)
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

        public int CompareTo(IMathEnvironment<Number> environment, NumberSegments a, NumberSegments b)
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
                for (Int32 i = ((Int32)a.Size) - 1; i >= 0; i--)
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

        public NumberSegments Multiply(IMathEnvironment<Number> environment, NumberSegments a, Decimal b)
        {
            if (b == 0)
            {
                return new NumberSegments(new Decimal[] { 0 });
            }
            var resultRaw = new List<Decimal>();

            Decimal numberIndex = b;

            Decimal carryOver = 0;
            for (Int32 i = 0; i < a.Size; i++)
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
                if (carryOver >= environment.Base)
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

        public NumberSegments Multiply(IMathEnvironment<Number> environment, NumberSegments a, NumberSegments b)
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

        public NumberSegments Multiply(IMathEnvironment<Number> environment, Decimal a, Decimal b)
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

        public (NumberSegments Whole, NumberSegments Numerator, NumberSegments Denominator) Divide(IMathEnvironment<Number> environment, NumberSegments numerator, NumberSegments denominator, NumberSegments hint = default(NumberSegments))
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
                return (numerator, null, null);
            }
            else if (this.IsEqual(environment, numerator, denominator))
            {
                return (new NumberSegments(new Decimal[] { 1 }), null, null);
            }
            else if (this.IsLessThan(environment,numerator, denominator))
            {
                return (new NumberSegments(new Decimal[] { 0 }), numerator, denominator);
            }
            else if (numerator.Size == 1 && denominator.Size == 1)
            {
                return this.Divide(environment,numerator[0], denominator[0]);
            }
            else if (denominator.Size == 1)
            {
                return this.Divide(environment,numerator, denominator[0]);
            }

            NumberSegments floor = (hint == default(NumberSegments) || this.IsGreaterThan(environment,denominator, hint)) ? environment.GetNumber(1).Segments : this.GetAboutHalf(environment, hint, environment.GetNumber(1).Segments, -1);
            NumberSegments ceiling = (hint == default(NumberSegments) || this.IsGreaterThan(environment,denominator, hint)) ? numerator : this.GetAboutHalf(environment, hint, numerator, 1);

            NumberSegments lastNumberTried = this.GetWholeNumberSomewhereBetween(environment,  ceiling, floor);
            NumberSegments numeratorTestResult = this.Multiply(environment,lastNumberTried, denominator);

            NumberSegments maxDifference = this.Subtract(environment,denominator, new NumberSegments(new Decimal[] { 1 }));
            NumberSegments minimumTestResult = this.Subtract(environment,numerator, maxDifference);

            NumberSegments lastNumeratorTestResult = environment.GetNumber(0).Segments;

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
                    floor = this.GetWholeNumberSomewhereBetween(environment, ceiling, environment.GetNumber(1).Segments);
                }
                else if (this.IsLessThanOrEqualTo(environment, ceiling, floor))
                {
                    ceiling = this.GetWholeNumberSomewhereBetween(environment, floor, numerator);
                }

                numeratorTestResult = this.Multiply(environment,lastNumberTried, denominator);
            }

            (NumberSegments Whole, NumberSegments Numerator, NumberSegments Denominator) result;

            if (this.IsEqual(environment, numeratorTestResult, numerator))
            {
                result = (lastNumberTried, default(NumberSegments), default(NumberSegments));
            }
            else
            {
                NumberSegments leftOver = this.Subtract(environment,numerator, numeratorTestResult);
                result = (lastNumberTried, leftOver, denominator);
            }

#if DEBUG
            if (this.IsGreaterThan(environment,result.Whole, numerator) && this.IsGreaterThan(environment,result.Whole, denominator))
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

            foreach (Decimal segment in result.Whole)
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
                foreach (Decimal segment in result.Numerator)
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

                foreach (Decimal segment in result.Denominator)
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

            Debug.WriteLine("Division result {0}", String.Join(',', result.Whole.Reverse()));

            if (result.Numerator != default(NumberSegments))
            {
                Debug.WriteLine("Division remainder {0} / {1}", String.Join(',', result.Numerator.Reverse()), String.Join(',', result.Denominator.Reverse()));
            }
#endif
            return result;
        }


        public (NumberSegments Whole, NumberSegments Numerator, NumberSegments Denominator) Divide(IMathEnvironment<Number> environment, Decimal dividend, Decimal divisor)
        {
            (NumberSegments Whole, NumberSegments Numerator, NumberSegments Denominator) result;

            Decimal remainder;

            if (dividend > divisor)
            {
                remainder = (dividend % divisor);

                Decimal resultRaw = System.Math.Floor(dividend / divisor);

                if (remainder == 0)
                {
                    result = (new NumberSegments(new Decimal[] { resultRaw }), default(NumberSegments), default(NumberSegments));
                }
                else
                {
                    result = (new NumberSegments(new Decimal[] { resultRaw }), new NumberSegments(new Decimal[] { remainder }), new NumberSegments(new Decimal[] { divisor }));
                }

            }
            else
            {
                result = (new NumberSegments(new Decimal[] { 0 }), new NumberSegments(new Decimal[] { dividend }), new NumberSegments(new Decimal[] { divisor }));
            }

#if DEBUG
            foreach (Decimal segment in result.Whole)
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
                foreach (Decimal segment in result.Numerator)
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

                foreach (Decimal segment in result.Denominator)
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

        public (NumberSegments Whole, NumberSegments Numerator, NumberSegments Denominator) Divide(IMathEnvironment<Number> environment, NumberSegments dividend, Decimal divisor)
        {
            if (dividend.Size == 1)
            {
                return this.Divide(environment, dividend[0], divisor);
            }

            (NumberSegments Whole, NumberSegments Numerator, NumberSegments Denominator) result;

            Decimal remainder = 0M;

            Decimal[] workingTotal = new Decimal[dividend.Length];

            for (Int32 i = dividend.Length - 1; i >= 0; i--)
            {
                Decimal currentDividend = dividend[i] + (remainder * environment.Base);
                Decimal currentRawTotalWhole = (System.Math.Floor(currentDividend / divisor));
                Decimal currentRemaninder = currentDividend - (currentRawTotalWhole * divisor);
            
                workingTotal[i] = currentRawTotalWhole;
                
                remainder = currentRemaninder;
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
                result = (new NumberSegments(resultRaw), default(NumberSegments), default(NumberSegments));
            }
            else
            {
                result = (new NumberSegments(resultRaw), new NumberSegments(new Decimal[] { remainder }), new NumberSegments(new Decimal[] { divisor }));
            }

#if DEBUG
            foreach (Decimal segment in result.Whole)
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
                NumberSegments reverseCheck = this.Multiply(environment,result.Whole, divisor);
                if (!this.IsEqual(environment, reverseCheck, dividend))
                {
                    throw new Exception(String.Format("Division Error {0} != {1} ", String.Join(',', reverseCheck.Reverse()), String.Join(',', dividend.Reverse())));
                }
            }
            else
            { 
                foreach (Decimal segment in result.Numerator)
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

                foreach (Decimal segment in result.Denominator)
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
            
                NumberSegments reverseCheck = this.Add(environment,this.Multiply(environment,result.Whole, divisor), result.Numerator);
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
        public Decimal Subtract(IMathEnvironment<Number> environment, Decimal a, Decimal b)
        {
            Decimal resultRaw = a - b;

            return resultRaw;
        }

        public NumberSegments Subtract(IMathEnvironment<Number> environment, NumberSegments a, NumberSegments b)
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


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Math
{
    public class Operator : IOperator
    {

        #region Add

        public Number Add(Number a, Char b)
        {
            return this.Add(a, new Char[] { b });
        }

        public Number Add(Number a, Char[] b)
        {
            return this.Add(a, new Number(a.Environment, b, false));
        }

        public WholeNumber Add(WholeNumber a, WholeNumber b)
        {
            if (a.Environment != b.Environment)
            {
                throw new Exception("Adding different math environments not supported yet");
            }

            MathEnvironment environment = a.Environment;

            Int64 maxPosition = a.Segments.Count;
            if (b.Segments.Count > maxPosition)
            {
                maxPosition = b.Segments.Count;
            }

            var resultNumber = new List<Char>();
            UInt64 carryOver = 0;
            Int64 position = 0;
            while (position < maxPosition)
            {
                UInt64 columnValue = carryOver;

                if (position < a.Segments.Count)
                {
                    columnValue += environment.GetIndex(a.Segments[(Int32)position]);
                }

                if (position < b.Segments.Count)
                {
                    columnValue += environment.GetIndex(b.Segments[(Int32)position]);
                }

                Char columnResult;
                if (columnValue >= environment.Base)
                {
                    UInt64 columnResultRaw = (columnValue % environment.Base);
                    columnResult = environment.Key[(Int32)columnResultRaw];

                    carryOver = (UInt64)(((Decimal)columnValue - (Decimal)columnResultRaw) / (Decimal)environment.Base);
                }
                else
                {
                    columnResult = environment.Key[(Int32)columnValue];
                    carryOver = 0;
                }

                resultNumber.Add(columnResult);
                position++;
            }

            if (carryOver != environment.Bottom)
            {
                Char columnResult;
                while (carryOver >= environment.Base)
                {
                    UInt64 columnResultRaw = (carryOver % environment.Base);
                    columnResult = environment.Key[(Int32)columnResultRaw];
                    resultNumber.Add(columnResult);

                    carryOver = (UInt64)((Decimal)columnResultRaw / (Decimal)environment.Base);
                }

                if (carryOver > 0)
                {
                    columnResult = environment.Key[(Int32)carryOver];
                    resultNumber.Add(columnResult);
                }
            }

            return new WholeNumber(environment, resultNumber, false); ;
        }

        public Number Add(Number a, Number b)
        {
            if (a.Environment != b.Environment)
            {
                throw new Exception("Adding different math environments not supported yet");
            }

            MathEnvironment environment = a.Environment;
            
            if (a.Fragment != default(Fraction) && b.Fragment == default(Fraction))
            {
                var resultWhole = this.Add(a.Floor(), b.Floor());

                return new Number(resultWhole, a.Fragment);
            }
            else if (a.Fragment == default(Fraction) && b.Fragment != default(Fraction))
            {
                var resultWhole = this.Add(a.Floor(), b.Floor());

                return new Number(resultWhole, b.Fragment);
            }
            else if (a.Fragment != default(Fraction) && b.Fragment != default(Fraction))
            {
                Number aFloor = a.Floor().AsNumber();
                Number bFloor = b.Floor().AsNumber();
                
                var fragmentA = new Fraction(((aFloor * a.Fragment.Denominator) + a.Fragment.Numerator), a.Fragment.Denominator);
                var fragmentB = new Fraction(((bFloor * b.Fragment.Denominator) + b.Fragment.Numerator), b.Fragment.Denominator);

                return this.Add(environment, fragmentA, fragmentB);
            }
            else
            {
                return this.Add(a.Floor(), b.Floor()).AsNumber();
            }
        }

        public Number Add(MathEnvironment environment, Fraction a, Fraction b)
        {
            Number denominator = a.Denominator * b.Denominator;
            Number numerator = (a.Numerator * b.Denominator) + (b.Numerator * a.Denominator);

            return this.Convert(new Fraction(numerator, denominator));
        }

        #endregion

        #region Subtract

        public WholeNumber Subtract(WholeNumber a, WholeNumber b)
        {

            if (a.Environment != b.Environment)
            {
                throw new Exception("Subtracting different math environments not supported yet");
            }

            MathEnvironment environment = a.Environment;

            Boolean isNegative = false;

            if (a.IsNegative && b.IsNegative && a > b)
            {
                WholeNumber valueHolderAtoB = a.AsNegative();
                WholeNumber valueHolderBtoA = b.AsNegative();

                b = valueHolderBtoA;
                a = valueHolderAtoB;
            }
            else if (!a.IsNegative && !b.IsNegative && b > a)
            {
                isNegative = true;

                WholeNumber valueHolderAtoB = a.AsPositive();
                WholeNumber valueHolderBtoA = b.AsPositive();

                b = valueHolderBtoA;
                a = valueHolderAtoB;
            }
            else if (a.IsNegative)
            {
                WholeNumber positive = a.AsPositive() + b;

                return positive.AsNegative();
            }
            else if (b.IsNegative)
            {
                WholeNumber positive = a + b.AsPositive();

                return a + b;
            }


            Int64 maxPosition = a.Segments.Count;
            if (b.Segments.Count > maxPosition)
            {
                maxPosition = b.Segments.Count;
            }

            // 60 - 90
            var resultSegments = new List<Char>();
            Int64 borrow = 0;
            Int32 position = 0;
            while (position < maxPosition)
            {
                Int64 columnValue = borrow;
                borrow = 0;


                if (position < a.Segments.Count)
                {
                    columnValue += (Int64)environment.GetIndex(a.Segments[position]);
                }

                if (position < b.Segments.Count)
                {
                    columnValue -= (Int64)environment.GetIndex(b.Segments[position]);
                }

                if (columnValue < 0)
                {
                    borrow -= 1;
                    columnValue += (Int64)environment.Base;
                }

                resultSegments.Add(environment.Key[(Int32)columnValue]);
                position++;
            }

            environment.ValidateWholeNumber(resultSegments);

            return new WholeNumber(environment, resultSegments.ToArray(), isNegative);

        }

        public  Number Subtract(Number a, Number b)
        {
            return this.Subtract(a.Floor(), b.Floor()).AsNumber();
        }

        #endregion

        #region Multiply

        public Number Multiply(MathEnvironment environment, Fraction a, Fraction b)
        {
            Number denominator = a.Denominator * b.Denominator;
            Number numerator = a.Numerator * b.Numerator;

            return this.Convert(new Fraction(numerator, denominator));
        }

        public WholeNumber Multiply(WholeNumber a, WholeNumber b)
        {
            if (a.Environment != b.Environment)
            {
                throw new Exception("Subtracting different math environments not supported yet");
            }

            MathEnvironment environment = a.Environment;

            return this.Multiply(environment, a.Segments, b.Segments);
        }

        public  Number Multiply(Number a, Number b)
        {
            if (a.Environment != b.Environment)
            {
                throw new Exception("Multipling different math environments not supported yet");
            }

            MathEnvironment environment = a.Environment;

            if (a.Fragment != default(Fraction) || b.Fragment != default(Fraction))
            {
                Fraction fragmentA;
                if (a.Fragment == default(Fraction))
                {
                    fragmentA = new Fraction(a, environment.FirstNumber);
                }
                else
                {
                    Number aX1 = a.Floor().AsNumber() * a.Fragment.Denominator;
                    Number aX2 = aX1 + a.Fragment.Numerator;

                    fragmentA = new Fraction(aX2, a.Fragment.Denominator);
                }

                Fraction fragmentB; 
                if (b.Fragment == default(Fraction))
                {
                    fragmentB = new Fraction(b, environment.FirstNumber);
                }
                else
                {
                    Number bX1 = b.Floor().AsNumber() * b.Fragment.Denominator;
                    Number bX2 = bX1 + b.Fragment.Numerator;

                    fragmentB = new Fraction(bX2, b.Fragment.Denominator);
                }
                
                return this.Multiply(environment, fragmentA, fragmentB);
            }
            else
            {
                return this.Multiply(a.Floor(), b.Floor()).AsNumber();
            }
        }

        public WholeNumber Multiply(MathEnvironment environment, ReadOnlyCollection<Char> a, Char b)
        {
            if (b == 0 || b == environment.Bottom)
            {
                return environment.BottomWholeNumber;
            }
            var resultRaw = new List<Char>();

            UInt64 numberIndex = environment.GetIndex(b);

            UInt64 carryOver = 0;
            for (var i = 0; i < a.Count; i++)
            {
                UInt64 segmentIndex = environment.GetIndex(a[i]);

                UInt64 columnTotal = (numberIndex * segmentIndex) + carryOver;

                Char columnPositionResult;
                if (columnTotal >= environment.Base)
                {
                    UInt64 remainder = (columnTotal % environment.Base);
                    columnPositionResult = environment.Key[(Int32)remainder];
                    carryOver = (columnTotal - remainder) / environment.Base;
                }
                else
                {
                    columnPositionResult = environment.Key[(Int32)columnTotal];
                    carryOver = 0;
                }

                resultRaw.Add(columnPositionResult);
            }

            while (carryOver > 0)
            {
                Char carryOverResult;
                if (carryOver > environment.Base)
                {
                    UInt64 remainder = (carryOver % environment.Base);
                    carryOverResult = environment.Key[(Int32)remainder];
                    carryOver = (carryOver - remainder) / environment.Base;
                }
                else
                {
                    carryOverResult = environment.Key[(Int32)carryOver];
                    carryOver = 0;
                }
                resultRaw.Add(carryOverResult);
            }

            return new WholeNumber(environment, resultRaw, false);
        }

        public WholeNumber Multiply(MathEnvironment environment, ReadOnlyCollection<Char> a, ReadOnlyCollection<Char> b)
        {
            WholeNumber result = environment.BottomWholeNumber;

            for (UInt64 i = 0; i < (UInt64)a.Count; i++)
            {
                Char numberSegment = a[(Int32)i];
                WholeNumber currentResult = this.Multiply(environment, b, numberSegment);

                if (i > 0)
                {
                    currentResult = this.PowerOf(currentResult, i);
                }

                result += currentResult;
            }

            return result;
        }

        public WholeNumber Multiply(WholeNumber a, Char number)
        {
            return Multiply(a.Environment, a.Segments, number);
        }

        public  Number Multiply(MathEnvironment environment, Char number1, Char number2)
        {
            UInt64 number1Index = environment.GetIndex(number1);
            UInt64 number2Index = environment.GetIndex(number2);

            UInt64 resultIndex = number1Index * number2Index;

            if (resultIndex >= environment.Base)
            {
                UInt64 remainderIndex = (resultIndex % environment.Base);
                UInt64 carryOver = (resultIndex - remainderIndex) / environment.Base;

                return new Number(environment.BottomWholeNumber, environment.KeyNumber[(Int32)carryOver], environment.KeyNumber[(Int32)remainderIndex]);
            }
            else
            {
                return  environment.KeyNumber[(Int32)resultIndex];
            }
        }

        #endregion

        #region Divide

        public  Number Divide(Number dividend, Number divisor)
        {
            if (dividend.Environment != divisor.Environment)
            {
                throw new Exception("Dividing different math environments not supported yet");
            }
            
            MathEnvironment environment = dividend.Environment;

            if (divisor > dividend)
            {
                return new Number(environment.BottomWholeNumber, dividend, divisor);
            }
            else
            {
                return this.Divide(dividend.Floor(), divisor.Floor());
            }
        }
        public Number Divide(WholeNumber dividend, WholeNumber divisor)
        {
            if (dividend.Environment != divisor.Environment)
            {
                throw new Exception("Dividing different math environments not supported yet");
            }

            MathEnvironment environment = dividend.Environment;

            return this.Convert(new Fraction(environment, dividend.Segments, divisor.Segments));
        }

        public  Number Divide(MathEnvironment environment, Char dividend, Char divisor)
        {
            Number result = null;
            UInt64 indexToDivide = environment.GetIndex(dividend);
            UInt64 indexToDivideBy = environment.GetIndex(divisor);

            UInt64 remainder;

            if (indexToDivide > indexToDivideBy)
            {
                remainder = indexToDivide % indexToDivideBy;

                UInt64 resultRaw = (UInt64)System.Math.Floor((decimal)indexToDivide / (decimal)indexToDivideBy);

                result = this.Convert(environment, resultRaw, remainder, indexToDivideBy);
            }
            else
            {
                result = this.Convert(environment, 0, dividend, divisor);
            }

            return result;
        }

        #endregion
        

        public  Number Convert(MathEnvironment environment, UInt64 number)
        {
            return this.Convert(environment, number, 0, 0);
        }
        public  Number Convert(MathEnvironment environment, UInt64 numberRaw, UInt64 numeratorNumber, UInt64 denominatorRaw)
        {
            List<Char> number = this.Convert(environment.Base, environment.Key, numberRaw);
            List<Char> numerator = this.Convert(environment.Base, environment.Key, numeratorNumber);
            List<Char> denominator = this.Convert(environment.Base, environment.Key, denominatorRaw);

            var result = new Number(environment, number, false, new Fraction(environment, numerator, denominator));

            return result;
        }

        public WholeNumber GetWholeNumberSomewhereBetween(WholeNumber a, WholeNumber b, Int64 variance = 0)
        {
            WholeNumber result;

            if (a == b)
            {
                result = a;
            }
            else
            {

                MathEnvironment environment = a.Environment;

                WholeNumber largerNumber;
                WholeNumber smallerNumber;

                if (a > b)
                {
                    largerNumber = a;
                    smallerNumber = b;
                }
                else
                {
                    largerNumber = b;
                    smallerNumber = a;
                }

                Decimal smallerNumberDecimalPlaces = smallerNumber.DecimalPlaces;
                Decimal largerNumberDecimalPlaces = largerNumber.DecimalPlaces;

                Decimal wholeNumberSomewhereBetweenDecimalPlaces = largerNumberDecimalPlaces - smallerNumberDecimalPlaces;
                if (wholeNumberSomewhereBetweenDecimalPlaces < 2)
                {
                    result = this.GetAboutHalf(largerNumber + smallerNumber, variance);
                }
                else
                {
                    Decimal smallerNumberFirstCharIndex = smallerNumber.DecimalPlaces;
                    Decimal largerNumberFirstCharIndex = largerNumber.DecimalPlaces;

                    Decimal somewhereBetweenFirstCharIndexish = ((largerNumberFirstCharIndex - smallerNumberFirstCharIndex) / 2M) + smallerNumberFirstCharIndex;

                    Int32 somewhereBetweenFirstCharindex;
                    if (variance > 0)
                    {
                        somewhereBetweenFirstCharindex = (Int32)System.Math.Ceiling(somewhereBetweenFirstCharIndexish);
                    }
                    else
                    {
                        somewhereBetweenFirstCharindex = (Int32)System.Math.Floor(somewhereBetweenFirstCharIndexish);
                    }

                    if ((Decimal)wholeNumberSomewhereBetweenDecimalPlaces + variance > 1 && (Decimal)wholeNumberSomewhereBetweenDecimalPlaces + variance > 0)
                    {
                        result = this.PowerOf(environment.KeyWholeNumber[(Int32)somewhereBetweenFirstCharindex], (UInt32)(wholeNumberSomewhereBetweenDecimalPlaces + variance));
                    }
                    else
                    {
                        result = this.PowerOf(environment.KeyWholeNumber[(Int32)somewhereBetweenFirstCharindex], (UInt32)(wholeNumberSomewhereBetweenDecimalPlaces));
                    }

                }
            }
            return result;
        }

        public WholeNumber GetAboutHalf(WholeNumber number, Int64 variance)
        {
            Decimal firstCharIndex = number.Environment.GetIndex(number.FirstChar);
            
            Decimal halfBase = ((Decimal)number.Environment.Base) / 2M;

            Char[] resultSegments;

            Decimal remainder = 0M;

            
            if (firstCharIndex > 1)
            {
                resultSegments = new Char[number.Segments.Count];
            }
            else
            {
                resultSegments = new Char[number.Segments.Count - 1];
                Decimal halfCharIndex = (Int32)System.Math.Floor(halfBase);
                Decimal charIndex;
                if (halfCharIndex == 1)
                {
                    charIndex = 0M;
                    remainder = halfBase;
                }
                else
                {
                    charIndex = halfCharIndex;
                    remainder = charIndex - halfCharIndex;
                }
                resultSegments[resultSegments.Length - 1] = number.Environment.Key[(Int32)charIndex];
            }
            
            for (var i = resultSegments.Length - ((firstCharIndex > 1) ? 1 : 2); i >= 0; i--)
            {
                Decimal charIndex = number.Environment.GetIndex(number.Segments[i]);
        
                if (charIndex == 1)
                {
                    if (i == 0 && variance > 0)
                    {
                        charIndex = 1M;
                    }
                    else
                    {
                        charIndex = 0M;
                    }
                    remainder = halfBase;
                    resultSegments[i] = number.Environment.Key[(Int32)charIndex];
                }
                else
                {
                    Decimal halfCharIndexWithRemainder = (charIndex / 2M) + remainder;
                    Int32 halfCharIndexWithRemainderIndex;
                    if (i == 0 && variance > 0)
                    {
                        halfCharIndexWithRemainderIndex = (Int32)System.Math.Ceiling(halfCharIndexWithRemainder);
                    }
                    else
                    {
                        halfCharIndexWithRemainderIndex = (Int32)System.Math.Floor(halfCharIndexWithRemainder);
                    }

                    resultSegments[i] = number.Environment.Key[(Int32)System.Math.Floor(halfCharIndexWithRemainder)];
                    remainder = (halfCharIndexWithRemainder - System.Math.Floor(halfCharIndexWithRemainder)) * 10;
                }
            }

            return new WholeNumber(number.Environment, resultSegments, number.IsNegative);
        }

        public Number Convert(Fraction fraction)
        {

            if (fraction.Numerator.Environment != fraction.Denominator.Environment)
            {
                throw new Exception("Convert a Fraction to number of different math environments is not supported yet");
            }

            MathEnvironment environment = fraction.Numerator.Environment;
            if (fraction.Numerator < fraction.Denominator)
            {
                return new Number(environment.BottomWholeNumber, fraction);
            }
            

            WholeNumber numerator = fraction.Numerator.Floor();
            WholeNumber denominator = fraction.Denominator.Floor();

            UInt64 numeratorLength = (UInt64)numerator.Segments.Count;
            UInt64 denominatorLength = (UInt64)denominator.Segments.Count;

            WholeNumber ceiling = numerator;
            WholeNumber floor = denominator;

            WholeNumber ceilingLast = environment.BottomWholeNumber;
            WholeNumber floorLast = environment.BottomWholeNumber;


            WholeNumber lastNumberTried = numerator;
            WholeNumber numeratorTestResult = lastNumberTried * denominator;

            while (ceiling != floor && (floorLast != floor || ceilingLast != ceiling))
            {
                ceilingLast = ceiling;
                floorLast = floor;

                numeratorTestResult = lastNumberTried * denominator;
                if (numeratorTestResult < numerator)
                {
                    floor = lastNumberTried;
                    lastNumberTried = this.GetWholeNumberSomewhereBetween(ceiling, floor, 1);
                }
                else if (numeratorTestResult > numerator)
                {
                    ceiling = lastNumberTried;
                    lastNumberTried = this.GetWholeNumberSomewhereBetween(ceiling, floor, -1);
                }
               
            }

            Number result;
            Number leftOver = fraction.Numerator - numeratorTestResult.AsNumber();
            if (leftOver > environment.BottomNumber)
            {
                result = new Number(lastNumberTried, new Fraction(leftOver, fraction.Denominator));
            }
            else
            {
                result = lastNumberTried.AsNumber();
            }

            return result;
        }

        public  List<Char> Convert(UInt64 mathBase, IList<Char> key, UInt64 number)
        {
            var resultRaw = new List<Char>();

            UInt64 carryOver = number;
            while (carryOver > 0)
            {
                if (carryOver >= mathBase)
                {
                    UInt64 columnResultRaw = 0;
                    columnResultRaw = (carryOver % mathBase);
                    resultRaw.Add(key[(Int32)columnResultRaw]);
                    carryOver = (UInt64)(((Decimal)carryOver - (Decimal)columnResultRaw) / (Decimal)mathBase);
                }
                else
                {
                    resultRaw.Add(key[(Int32)carryOver]);
                    carryOver = 0;
                }
            }
            return resultRaw;
        }

        public WholeNumber PowerOf(WholeNumber a, UInt64 power)
        {
            if (a.IsBottom() || power == 1)
            {
                return a;
            }

            var segments = new Char[(a.Segments.Count + (Int32)power)];
            for (Int32 i = segments.Length - 1; i >= 0; i--)
            {
                segments[i] = a.Environment.Bottom;
            }
            a.Segments.CopyTo(segments, (Int32)power);
            return new WholeNumber(a.Environment, segments, false);
        }
        
    }

}

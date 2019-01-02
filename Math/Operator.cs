using System;
using System.Collections.Generic;
using System.Linq;

namespace Math
{
    public class Operator: IOperator
    {

        #region Add

        public  String Add(String key, String arg1, String arg2)
        {
            var environment = new MathEnvironment(key);
            return (new Number(environment, arg1) + new Number(environment, arg2)).ToString();
        }

        public  Number Add(Number a, Char b)
        {
            return this.Add(a, new Char[] { b });
        }

        public  Number Add(Number a, Char[] b)
        {
            return this.Add(a, new Number(a.Environment, b));
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

            return new WholeNumber(environment, resultNumber.ToArray()); ;
        }

        public  Number Add(Number a, Number b)
        {
            return this.Add(a.Floor(), b.Floor()).AsNumber();
        }

        #endregion

        #region Subtract

        public  String Subtract(String key, String arg1, String arg2)
        {
            var environment = new MathEnvironment(key);
            return (new Number(environment, arg1) - new Number(environment, arg2)).ToString();
        }
        
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

            return new WholeNumber(environment, resultSegments.ToArray(), isNegative);

        }

        public  Number Subtract(Number a, Number b)
        {
            return this.Subtract(a.Floor(), b.Floor()).AsNumber();
        }

        #endregion

        #region Multiply

        public  String Multiply(String key, String arg1, String arg2)
        {
            var environment = new MathEnvironment(key);
            return (new Number(environment, arg1) * new Number(environment, arg2)).ToString();
        }

        public Number Multiply(MathEnvironment evironment, Fraction a, Fraction b)
        {
            Number numerator = this.Multiply(a.Numerator, a.Denominator);
            Number denominator = this.Multiply(b.Numerator, b.Denominator);

            Number resultWholeNumber = evironment.BottomNumber;
            while (numerator > denominator)
            {
                resultWholeNumber += evironment.FirstNumber;
                numerator -= evironment.FirstNumber;
            }

            Number result;
            if (numerator > evironment.BottomNumber)
            {
                var resultWholeNumberSegments = new Char[resultWholeNumber.Segments.Count];
                result = new Number(evironment, resultWholeNumberSegments, new Fraction(numerator, denominator));
            }
            else
            {
                result = resultWholeNumber;
            }

            return result;
        }

        public WholeNumber Multiply(WholeNumber a, WholeNumber b)
        {
            if (a.Environment != b.Environment)
            {
                throw new Exception("Subtracting different math environments not supported yet");
            }

            MathEnvironment environment = a.Environment;

            return this.Multiply(environment, a.Segments.ToArray(), b.Segments.ToArray());
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
                    fragmentA = a.Fragment;
                }

                Fraction fragmentB; 
                if (b.Fragment == default(Fraction))
                {
                    fragmentB = new Fraction(b, environment.FirstNumber);
                }
                else
                {
                    fragmentB = b.Fragment;
                }
                
                return this.Multiply(environment, fragmentA, fragmentB);
            }
            else
            {
                return this.Multiply(a.Floor(), b.Floor()).AsNumber();
            }
        }

        public WholeNumber Multiply(MathEnvironment environment, Char[] a, Char b)
        {
            var resultRaw = new List<Char>();

            UInt64 numberIndex = environment.GetIndex(b);

            UInt64 carryOver = 0;
            for (var i = 0; i < a.Length; i++)
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

            return new WholeNumber(environment, resultRaw.ToArray());
        }

        public WholeNumber Multiply(MathEnvironment environment, Char[] a, Char[] b)
        {
            WholeNumber result = environment.BottomWholeNumber;

            for (UInt64 i = 0; i < (UInt64)a.Length; i++)
            {
                Char numberSegment = a[i];
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
            return Multiply(a.Environment, a.Segments.ToArray(), number);
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

                return new Number(environment, new Char[] { environment.Key[(Int32)carryOver], environment.Key[(Int32)remainderIndex] });
            }
            else
            {
                return new Number(environment, new Char[] { environment.Key[(Int32)resultIndex] });
            }
        }

        #endregion

        #region Divide

        public  String Divide(String key, String dividend, String divisor)
        {
            var environment = new MathEnvironment(key);
            return (new Number(environment, dividend) / new Number(environment, divisor)).ToString();
        }

        public  Number Divide(Number dividend, Number divisor)
        {
            if (dividend.Environment != divisor.Environment)
            {
                throw new Exception("Dividing different math environments not supported yet");
            }
            
            MathEnvironment environment = dividend.Environment;

            if (divisor > dividend)
            {
                return new Number(environment, new Fraction(dividend, divisor));
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
            
            if (divisor > dividend)
            {
                return new Number(environment, new Fraction(dividend.AsNumber(), divisor.AsNumber()));
            }

            UInt64 divisorSize = (UInt64)divisor.Segments.Count;
            UInt64 dividendSize = (UInt64)dividend.Segments.Count;

            WholeNumber resultWhole = environment.BottomWholeNumber;

            if (divisorSize == dividendSize)
            {
                while (dividend - divisor >= divisor)
                {
                    dividend = dividend - divisor;
                    resultWhole += environment.FirstWholeNumber;
                }
            }
            else
            {
                while (dividend >= divisor)
                {
                    UInt64 currentPlace = (UInt64)dividend.Segments.Count - 1;

                    WholeNumber tempResult = environment.BottomWholeNumber;

                    var tempSegments = new List<Char>() { dividend.Segments[(Int32)currentPlace] };

                    while (new WholeNumber(environment, tempSegments.ToArray()) < divisor)
                    {
                        currentPlace--;
                        tempSegments.Insert(0, dividend.Segments[(Int32)currentPlace]); // 90
                    }


                    var tempNumber = new WholeNumber(environment, tempSegments.ToArray());
                    while (tempNumber - divisor >= environment.BottomWholeNumber)
                    {
                        tempNumber = tempNumber - divisor;
                        tempResult += environment.FirstWholeNumber; // 3
                    }

                    var remainder = new WholeNumber(environment, tempSegments.ToArray()) - tempNumber;
                    if (currentPlace > 0) {
                        dividend -= this.PowerOf(remainder, currentPlace);
                        resultWhole += this.PowerOf(tempResult, currentPlace);
                    }
                    else
                    {
                        dividend -= remainder;
                        resultWhole += tempResult;
                    }
                }
             
            }

            Number result;
            if (dividend > environment.BottomWholeNumber)
            {
                result = new Number(resultWhole, new Fraction(environment, dividend.Segments.ToArray(), divisor.Segments.ToArray()));
            }
            else
            {
                result = resultWhole.AsNumber();
            }

            //UInt64 placesMoved = 0;

            //while (a > b)
            //{
            //    placesMoved += 1;
            //    b.Segments.Insert(0, environment.Bottom);
            //}

            return result;
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

            var result = new Number(environment, number.ToArray(), new Fraction(environment, numerator.ToArray(), denominator.ToArray()));

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
            var segments = new Char[((UInt64)a.Segments.Count + power)];
            for (UInt64 i = 0; i < power; i++)
            {
                segments[i] = a.Environment.Bottom;
            }
            a.Segments.CopyTo(segments, (Int32)power);
            return new WholeNumber(a.Environment, segments);
        }

        public Number PowerOf(Number a, Int64 power)
        {
            var segments = new Char[a.Segments.Count + power];
            for (var i = 0; i < power; i++)
            {
                segments[i] = a.Environment.Bottom;
            }
            a.Segments.CopyTo(segments, (Int32)power);
            return new Number(a.Environment, segments);
        }
    }

}

using System;
using System.Collections.Generic;
using System.Text;

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

        public  Number Add(Number a, Number b)
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

            var result = new Number(environment, resultNumber.ToArray());

            return result;
        }

        #endregion

        #region Subtract

        public  String Subtract(String key, String arg1, String arg2)
        {
            var environment = new MathEnvironment(key);
            return (new Number(environment, arg1) - new Number(environment, arg2)).ToString();
        }

        public  Number Subtract(Number aArg, Number bArg)
        {
            Number a = aArg.Copy();
            Number b = bArg.Copy();

            if (a.Environment != b.Environment)
            {
                throw new Exception("Subtracting different math environments not supported yet");
            }

            MathEnvironment environment = a.Environment;

            Boolean isNegative = false;

            if (a.IsNegative && b.IsNegative && a > b)
            {
                Number valueHolderAtoB = a.AsNegative();
                Number valueHolderBtoA = b.AsNegative();

                b = valueHolderBtoA;
                a = valueHolderAtoB;
            }
            else if (!a.IsNegative && !b.IsNegative && b > a)
            {
                isNegative = true;

                Number valueHolderAtoB = a.AsPositive();
                Number valueHolderBtoA = b.AsPositive();

                b = valueHolderBtoA;
                a = valueHolderAtoB;
            }
            else if (a.IsNegative)
            {
                Number positive = a.AsPositive() + b;

                return positive.AsNegative();
            }
            else if (b.IsNegative)
            {
                Number positive = a + b.AsPositive();
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

            Number result = new Number(environment, resultSegments.ToArray(), isNegative);

            return result;
        }

        #endregion

        #region Multiply

        public  String Multiply(String key, String arg1, String arg2)
        {
            var environment = new MathEnvironment(key);
            return (new Number(environment, arg1) * new Number(environment, arg2)).ToString();
        }

        public  Number Multiply(Number a, Number b)
        {
            if (a.Environment != b.Environment)
            {
                throw new Exception("Multipling different math environments not supported yet");
            }

            MathEnvironment evironment = a.Environment;
           

            Char[] segmentsA = new Char[a.Segments.Count];
            Char[] segmentsB = new Char[b.Segments.Count];

            a.Segments.CopyTo(segmentsA,0);
            b.Segments.CopyTo(segmentsB,0);

            return this.Multiply(evironment, segmentsA, segmentsB);
        }

        public  Number Multiply(MathEnvironment environment, Char[] a, Char b)
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

            return new Number(environment, resultRaw.ToArray());
        }

        public  Number Multiply(MathEnvironment environment, Char[] a, Char[] b)
        {
            Number result = environment.BottomNumber;

            for (var i = 0; i < a.Length; i++)
            {
                Char numberSegment = a[i];
                Number currentResult = this.Multiply(environment, b, numberSegment);

                if (i > 0)
                {
                    currentResult = this.PowerOf(currentResult, i);
                }

                result += currentResult;
            }

            return result;
        }

        public  Number Multiply(Number arg, Char number)
        {
            Char[] segments = new char[arg.Segments.Count];
            arg.Segments.CopyTo(segments, 0);

            return Multiply(arg.Environment, segments, number);
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

        public  String Divide(String key, String a, String b)
        {
            var environment = new MathEnvironment(key);
            return (new Number(environment, a) / new Number(environment, b)).ToString();
        }

        public  Number Divide(Number a, Number b)
        {
            if (a.Environment != b.Environment)
            {
                throw new Exception("Dividing different math environments not supported yet");
            }

            Number dividend = a.Copy();
            Number divisor = b.Copy();

            MathEnvironment environment = divisor.Environment;
            Number result = environment.BottomNumber;

            if (dividend < divisor)
            {
                result.Fragment = new Fraction(dividend, divisor);
                return result;
            }

            UInt64 divisorSize = (UInt64)divisor.Segments.Count;
            UInt64 dividendSize = (UInt64)dividend.Segments.Count;

            if (divisorSize == dividendSize)
            {
                while (dividend - divisor >= divisor)
                {
                    dividend = dividend - divisor;
                    result += environment.FirstNumber;
                }
            }
            else
            {

                // 30 | 9000
                //aSize must be longer then bSize
                Int32 currentPlace = dividend.Segments.Count - 1;
                var tempSegments = new List<Char>() { dividend.Segments[currentPlace] };

                while (new Number(environment, tempSegments.ToArray()) <  divisor)
                {
                    currentPlace--;
                    tempSegments.Insert(0, dividend.Segments[currentPlace]); // 90
                }

                Number tempNumber = new Number(environment, tempSegments.ToArray());
                while (tempNumber - divisor >= environment.BottomNumber)
                {
                    tempNumber = tempNumber - divisor;
                    result += environment.FirstNumber; // 3
                }
                
                
                for (Int32 i = 0; i < currentPlace; i++)
                {

                }
            }

            if (dividend > environment.BottomNumber)
            {
                result.Fragment = new Fraction(dividend, divisor);
            }

            //UInt64 placesMoved = 0;

            //while (a > b)
            //{
            //    placesMoved += 1;
            //    b.Segments.Insert(0, environment.Bottom);
            //}

            return result;
        }

        public  Number Divide(MathEnvironment environment, Char numberToDivide, Char numberToDivideBy)
        {
            Number result = null;
            UInt64 indexToDivide = environment.GetIndex(numberToDivide);
            UInt64 indexToDivideBy = environment.GetIndex(numberToDivideBy);

            UInt64 remainder;

            if (indexToDivide > indexToDivideBy)
            {
                remainder = indexToDivide % indexToDivideBy;

                UInt64 resultRaw = (UInt64)System.Math.Floor((decimal)indexToDivide / (decimal)indexToDivideBy);

                result = this.Convert(environment, resultRaw, remainder, indexToDivideBy);
            }
            else
            {
                result = this.Convert(environment, 0, numberToDivide, numberToDivideBy);
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

        public Number PowerOf(Number a, Int32 power)
        {
            var segments = new Char[a.Segments.Count + power];
            for (var i = 0; i < power; i++)
            {
                segments[i] = a.Environment.Bottom;
            }
            a.Segments.CopyTo(segments, power);
            return new Number(a.Environment, segments);
        }
    }

}

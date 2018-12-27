using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Math
{
    public class Number: IEquatable<Number>, IComparable<Number>, IComparer<Number>
    {
        public MathEnvironmentInfo Environment
        {
            get;
            set;
        }

        public Number(MathEnvironmentInfo environment)
        {
            this.Environment = environment;
            this.Segments = new Char[] { this.Environment.Bottom };

        }

        public Number(MathEnvironmentInfo environment, Char[] number)
        {
            this.Environment = environment;
            this.Segments = number.ToArray();

        }

        public Number(MathEnvironmentInfo environment, String rawNumber)
        {
            this.Environment = environment;
            Char[] numberArray = rawNumber.ToCharArray().Reverse().ToArray();
            this.Validate(numberArray);
            this.Segments = numberArray;
        }

        public Boolean IsBottom()
        {
            foreach (Char segment in Segments)
            {
                if (segment != this.Environment.Bottom)
                {
                    return false;
                }
            }
            return true;
        }

        public void Validate(Char[] numberArray)
        {
            foreach (Char segment in numberArray)
            {
                if (!this.Environment.Key.Contains(segment))
                {
                    throw new Exception(String.Format("Invalid Number {0} not found in {1}", segment, this.Environment));
                }
            }
        }
        public Char[] Segments { get; set; }

        public override String ToString()
        {
            String result = null;
            foreach (Char segment in this.Segments.Reverse())
            {
                result += segment;
            }
            return (String.IsNullOrWhiteSpace(result))?"0": result;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = this.Segments.GetHashCode();
                return hashCode;
            }
        }

        public override Boolean Equals(Object other)
        {
            return this.Equals((Number)other);
        }

        public Boolean Equals(Number other)
        {
            if (!this.Environment.Equals(other.Environment)) {
                //FIX: should able to tell if number values match in seperate Environments
                return false; 
            }
            
            if (this.Segments.Length != other.Segments.Length)
            {
                return false;
            }

            for (var i = 0; i < this.Segments.Length; i++)
            {
                if (this.Segments[i] != other.Segments[i])
                {
                    return false;
                }
            }
            return true;
        }

        public int CompareTo(Number other)
        {
            if (this.Segments.Length > other.Segments.Length)
            {
                return 1;
            }
            else if (this.Segments.Length < other.Segments.Length)
            {
                return -1;
            }

            for (var i = this.Segments.Length - 1; i >= 0; i--)
            {
                if (this.Segments[i] != other.Segments[i])
                {
                    if (this.Segments[i] > other.Segments[i])
                    {
                        return 1;
                    }
                    else if (this.Segments[i] < other.Segments[i])
                    {
                        return -1;
                    }
                }
            }
            return 0;
        }

        public int Compare(Number x, Number y)
        {
            return x.CompareTo(y);
        }

        #region operator overrides
        public static bool operator <(Number e1, Number e2)
        {
            return e1.CompareTo(e2) < 0;
        }

        public static bool operator <=(Number e1, Number e2)
        {
            return e1.CompareTo(e2) <= 0;
        }
        public static bool operator >(Number e1, Number e2)
        {
            return e1.CompareTo(e2) > 0;
        }

        public static bool operator >=(Number e1, Number e2)
        {
            return e1.CompareTo(e2) >= 0;
        }
        
        public static bool operator ==(Number e1, Number e2)
        {
            return e1.Equals(e2);
        }

        public static bool operator !=(Number e1, Number e2)
        {
            return !e1.Equals(e2);
        }
        
        public static Number operator +(Number a, Number b)
        {
            return Number.Add(a, b);
        }
        
        public static Number operator -(Number a, Number b)
        {
            return Number.Subtract(a, b);
        }

        public static Number operator *(Number a, Number b)
        {
            return Number.Multiply(a, b);
        }


        public static Number operator /(Number a, Number b)
        {
            return Number.Divide(a, b);
        }

        
        public static Number operator %(Number a, Number b)
        {
            return Number.Divide(a, b);
        }



        #endregion

        #region Add

        public static String Add(String key, String arg1, String arg2)
        {
            var environment = new MathEnvironmentInfo(key);
            return (new Number(environment, arg1) + new Number(environment, arg2)).ToString();
        }

        public static Number Add(Number a, Char b)
        {
            return Number.Add(a, new Char[] { b });
        }

        public static Number Add(Number a, Char[] b)
        {
            return Number.Add(a, new Number(a.Environment, b));
        }

        public static Number Add(Number a, Number b)
        {
            return Number.Add(new Numbers(a, b));
        }
        
        public static Number Add(Numbers arg)
        {
            Int64 maxPosition = 0;
            foreach (var number in arg.NumbersSegments)
            {
                if (number.Length > maxPosition)
                {
                    maxPosition = number.Length;
                }
            }

            var resultNumber = new List<Char>();
            Int64 carryOver = 0;
            Int64 position = 0;
            while (position < maxPosition)
            {
                Int64 columnValue = carryOver;
                foreach (var number in arg.NumbersSegments)
                {
                    if (position < number.Length)
                    {
                        columnValue += arg.Environment.GetIndex(number[(Int32)position]);
                    }
                }

                Char columnResult;
                if (columnValue >= arg.Environment.Base)
                {
                    Int64 columnResultRaw = (columnValue % arg.Environment.Base);
                    columnResult = arg.Environment.Key[(Int32)columnResultRaw];

                    carryOver = (Int64)(((Decimal)columnValue - (Decimal)columnResultRaw) / (Decimal)arg.Environment.Base);
                }
                else
                {
                    columnResult = arg.Environment.Key[(Int32)columnValue];
                    carryOver = 0;
                }

                resultNumber.Add(columnResult);
                position++;
            }

            if (carryOver != arg.Environment.Bottom)
            {
                Char columnResult;
                while (carryOver >= arg.Environment.Base)
                {
                    Int64 columnResultRaw = (carryOver % arg.Environment.Base);
                    columnResult = arg.Environment.Key[(Int32)columnResultRaw];
                    resultNumber.Add(columnResult);

                    carryOver = (Int64)((Decimal)columnResultRaw / (Decimal)arg.Environment.Base);
                }
                if (carryOver > 0)
                {
                    columnResult = arg.Environment.Key[(Int32)carryOver];
                    resultNumber.Add(columnResult);
                }
            }

            var result = new Number(arg.Environment, resultNumber.ToArray());

            return result;
        }

        #endregion

        #region Subtract

        public static String Subtract(String key, String arg1, String arg2)
        {
            var environment = new MathEnvironmentInfo(key);
            return (new Number(environment, arg1) - new Number(environment, arg2)).ToString();
        }
        
        public static Number Subtract(Number a, Number b)
        {
            return Number.Subtract(new Numbers(a, b));
        }

        public static Number Subtract(Numbers arg)
        {
            Int64 maxPosition = 0;
            foreach (var number in arg.NumbersSegments)
            {
                if (number.Length > maxPosition)
                {
                    maxPosition = number.Length;
                }
            }

            var resultNumber = new List<Char>();
            Int64 carryOver = 0;
            Int64 position = 0;
            while (position < maxPosition)
            {
                Int64 columnValue = carryOver;
                foreach (var number in arg.NumbersSegments)
                {
                    if (position < number.Length)
                    {
                        columnValue += arg.Environment.GetIndex(number[(Int32)position]);
                    }
                }

                Char columnResult;
                if (columnValue >= arg.Environment.Base)
                {
                    Int64 columnResultRaw = (columnValue % arg.Environment.Base);
                    columnResult = arg.Environment.Key[(Int32)columnResultRaw];

                    carryOver = (Int64)(((Decimal)columnValue - (Decimal)columnResultRaw) / (Decimal)arg.Environment.Base);
                }
                else
                {
                    columnResult = arg.Environment.Key[(Int32)columnValue];
                    carryOver = 0;
                }

                resultNumber.Add(columnResult);
                position++;
            }

            if (carryOver != arg.Environment.Bottom)
            {
                Char columnResult;
                while (carryOver >= arg.Environment.Base)
                {
                    Int64 columnResultRaw = (carryOver % arg.Environment.Base);
                    columnResult = arg.Environment.Key[(Int32)columnResultRaw];
                    resultNumber.Add(columnResult);

                    carryOver = (Int64)((Decimal)columnResultRaw / (Decimal)arg.Environment.Base);
                }
                if (carryOver > 0)
                {
                    columnResult = arg.Environment.Key[(Int32)carryOver];
                    resultNumber.Add(columnResult);
                }
            }

            var result = new Number(arg.Environment, resultNumber.ToArray());

            return result;
        }

        #endregion

        #region Multiply

        public static String Multiply(String key, String arg1, String arg2)
        {
            var environment = new MathEnvironmentInfo(key);
            return (new Number(environment, arg1) * new Number(environment, arg2)).ToString();
        }

        public static Number Multiply(Number a, Number b)
        {
            return Number.Multiply(new Numbers(a, b));
        }

        public static Number Multiply(MathEnvironmentInfo environment, Char[] number1, Char number2)
        {
            var resultRaw = new List<Char>();

            Int64 numberIndex = environment.GetIndex(number2);

            Int64 carryOver = 0;
            for (var i = 0; i < number1.Length; i++)
            {
                Int64 segmentIndex = environment.GetIndex(number1[i]);

                Int64 columnTotal = (numberIndex * segmentIndex) + carryOver;

                Char columnPositionResult;
                if (columnTotal >= environment.Base)
                {
                    Int64 remainder = (columnTotal % environment.Base);
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
                    Int64 remainder = (carryOver % environment.Base);
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
        public static Number Multiply(MathEnvironmentInfo environment, Char[] number1, Char[] number2)
        {
            var result = new Number(environment);

            for (var i = 0; i < number1.Length; i++)
            {
                Char numberSegment = number1[i];
                Number currentResult = Number.Multiply(environment, number2, numberSegment);
                for (var i2 = 0; i2 < i; i2++)
                {
                    IList<Char> currentResultList = currentResult.Segments.ToList();
                    currentResultList.Insert(0, environment.Bottom);
                    currentResult = new Number(environment, currentResultList.ToArray());
                }
                result += currentResult;
            }


            return result;
        }

        public static Number Multiply(Number arg, Char number)
        {
            return Multiply(arg.Environment, arg.Segments, number);
        }

        public static Number Multiply(MathEnvironmentInfo environment, Char number1, Char number2)
        {
            Int64 number1Index = environment.GetIndex(number1);
            Int64 number2Index = environment.GetIndex(number2);

            Int64 resultIndex = number1Index * number2Index;

            if (resultIndex >= environment.Base)
            {
                Int64 remainderIndex = (resultIndex % environment.Base);
                Int64 carryOver = (resultIndex - remainderIndex) / environment.Base;

                return new Number(environment, new Char[] { environment.Key[(Int32)carryOver], environment.Key[(Int32)remainderIndex] });
            }
            else
            {
                return new Number(environment, new Char[] { environment.Key[(Int32)resultIndex] });
            }
        }

        public static Number Multiply(Numbers arg)
        {
            if (arg.NumbersSegments.Count == 0)
            {
                return arg.Environment.BottomNumber;
            }
            else if (arg.NumbersSegments.Count == 1)
            {
                return new Number(arg.Environment, arg.NumbersSegments[0]);
            }

            Char[] runningTotal = new Char[] { arg.Environment.Bottom  };
            for (var i = 1; i < arg.NumbersSegments.Count; i ++)
            {
                Char[] numbersSegment1 = runningTotal;
                if (i == 1) {
                    numbersSegment1 = arg.NumbersSegments[i - 1];
                }
                else
                {
                    numbersSegment1 = runningTotal;
                }
                Char[] numbersSegment2 = arg.NumbersSegments[i];

                Number numbersSegment1And2Result = Number.Multiply(arg.Environment, numbersSegment1, numbersSegment2);
                runningTotal = numbersSegment1And2Result.Segments;
            }

            var result = new Number(arg.Environment, runningTotal.ToArray());

            return result;
        }
        #endregion

        #region Divide

        public static String Divide(String key, String arg1, String arg2)
        {
            var environment = new MathEnvironmentInfo(key);
            return (new Number(environment, arg1) / new Number(environment, arg2)).ToString();
        }
        
        public static Number Divide(Number a, Number b)
        {
            return Number.Divide(new Numbers(a, b));
        }

        public static Number Divide(Numbers arg)
        {
            var result = new Number(arg.Environment);

            //Number numberToDivideBy = arg.GetNumberAtIndex(0);
            //Number numberToDivide = arg.GetNumberAtIndex(1);

            //Number countDown = new Number(arg.Environment);
            //while (numberToDivide > arg.Environment.BottomNumber)
            //{
            //    Number numbersToSubtract = result - numberToDivide;
            //    result = Number.Subtract(numbersToSubtract);

            //    var subtractOne = new Numbers(countDown, new Number(arg.Environment, new Char[] { arg.Environment.Key[1] }));
            //    countDown = Number.Subtract(subtractOne);
            //}

            return result;
        }

        #endregion

        public static Number Convert(MathEnvironmentInfo environment, Int64 arg)
        {
            var resultRaw = new List<Char>();

            Int64 carryOver = 0;

            while (arg > 0)
            {
                Int64 columnResultRaw = 0;
                if (arg >= environment.Base)
                {
                    columnResultRaw = (arg % environment.Base);
                    carryOver = (Int64)(((Decimal)columnResultRaw - (Decimal)columnResultRaw) / (Decimal)environment.Base);
                }
                else
                {
                    columnResultRaw = arg;
                    carryOver = 0;
                }
                resultRaw.Add(environment.Key[(Int32)columnResultRaw]);
            }

            var result = new Number(environment, resultRaw.ToArray());
            return result;
        }

    }
}
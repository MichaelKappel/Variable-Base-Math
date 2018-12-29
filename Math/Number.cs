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
            this.Segments = new List<Char>() { this.Environment.Bottom };
            this.Fragments = new Tuple<List<Char>, List<Char>>(new List<Char> { this.Environment.Bottom } , new List<Char> { this.Environment.Bottom });

        }

        public Number(MathEnvironmentInfo environment, List<Char> number)
        {
            this.Environment = environment;
            this.Segments = number.ToList();
            this.Fragments = new Tuple<List<Char>, List<Char>>(new List<Char> { this.Environment.Bottom }, new List<Char> { this.Environment.Bottom });

        }

        public Number(MathEnvironmentInfo environment, List<Char> number, Tuple<List<Char>, List<Char>> fragments)
        {
            this.Environment = environment;
            this.Segments = number.ToList();
            this.Fragments = fragments;

        }

        public Number(MathEnvironmentInfo environment, UInt64 number)
        {
            this.Environment = environment;
            this.Segments = new List<Char>() { environment.Key[(Int32)number] };
            this.Fragments = new Tuple<List<Char>, List<Char>>(new List<Char> { this.Environment.Bottom }, new List<Char> { this.Environment.Bottom });
        }


        public Number(MathEnvironmentInfo environment, String rawNumber, String rawTopNumber, String rawBottomNumber)
        {
            this.Environment = environment;
            List<Char> numberArray = rawNumber.ToCharArray().Reverse().ToList();
            this.Validate(numberArray);
            this.Segments = numberArray;
            this.Fragments = new Tuple<List<Char>, List<Char>>(rawTopNumber.ToCharArray().Reverse().ToList(), rawBottomNumber.ToCharArray().Reverse().ToList());
        }


        public Number(MathEnvironmentInfo environment, String rawNumber)
        {
            this.Environment = environment;
            List<Char> numberArray = rawNumber.ToCharArray().Reverse().ToList();
            this.Validate(numberArray);
            this.Segments = numberArray;
            this.Fragments = new Tuple<List<Char>, List<Char>>(new List<Char> { this.Environment.Bottom }, new List<Char> { this.Environment.Bottom });
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

        public void Validate(List<Char> numberArray)
        {
            foreach (Char segment in numberArray)
            {
                if (!this.Environment.Key.Contains(segment))
                {
                    throw new Exception(String.Format("Invalid Number {0} not found in {1}", segment, this.Environment));
                }
            }
        }
        public Tuple<List<Char>, List<Char>> Fragments { get; set; }

        public List<Char> Segments { get; set; }

        public override String ToString()
        {
            String result = null;
            foreach (Char segment in this.Segments.ToArray().Reverse())
            {
                result += segment;
            }
            result = (String.IsNullOrWhiteSpace(result))?"0": result;
            if (this.Fragments.Item1.Count > 0 && !(this.Fragments.Item1.Count == 1 && this.Fragments.Item1[0] == Environment.Bottom))
            {
                String resultTop = null;
                foreach (Char segment in this.Fragments.Item1.ToArray().Reverse())
                {
                    resultTop += segment;
                }

                String resultBottom = null;
                foreach (Char segment in this.Fragments.Item2.ToArray().Reverse())
                {
                    resultBottom += segment;
                }
                result = String.Format("{0} {1}/{2} ", result, resultTop, resultBottom);
            }
            return result;
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
            
            if (this.Segments.Count != other.Segments.Count)
            {
                return false;
            }

            for (var i = 0; i < this.Segments.Count; i++)
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
            if (this.Segments.Count > other.Segments.Count)
            {
                return 1;
            }
            else if (this.Segments.Count < other.Segments.Count)
            {
                return -1;
            }

            for (var i = this.Segments.Count - 1; i >= 0; i--)
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

            if (this.Fragments.Item1 == other.Fragments.Item1 && this.Fragments.Item2 == other.Fragments.Item2)
            {
                return 0;
            }
            else
            {
                //FIX: temp solution
                var thisFragments = this.Fragments.Item1.Count;
                var othefFragments = other.Fragments.Item1.Count;

                if (thisFragments > othefFragments)
                {
                    return 1;
                }
                else 
                {
                    return -1;
                }
            }
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
            throw new Exception("% not supported yet");
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
            return Number.Add(a, new List<Char> { b });
        }

        public static Number Add(Number a, List<Char> b)
        {
            return Number.Add(a, new Number(a.Environment, b));
        }

        public static Number Add(Number a, Number b)
        {
            if (a.Environment != b.Environment)
            {
                throw new Exception("Adding different math environments not supported yet");
            }

            return Number.Add(new Numbers(a, b));
        }
        
        public static Number Add(Numbers arg)
        {

            Int64 maxPosition = 0;
            foreach (var number in arg.NumbersSegments)
            {
                if (number.Count > maxPosition)
                {
                    maxPosition = number.Count;
                }
            }

            var resultNumber = new List<Char>();
            UInt64 carryOver = 0;
            Int64 position = 0;
            while (position < maxPosition)
            {
                UInt64 columnValue = carryOver;
                foreach (var number in arg.NumbersSegments)
                {
                    if (position < number.Count)
                    {
                        columnValue += arg.Environment.GetIndex(number[(Int32)position]);
                    }
                }

                Char columnResult;
                if (columnValue >= arg.Environment.Base)
                {
                    UInt64 columnResultRaw = (columnValue % arg.Environment.Base);
                    columnResult = arg.Environment.Key[(Int32)columnResultRaw];

                    carryOver = (UInt64)(((Decimal)columnValue - (Decimal)columnResultRaw) / (Decimal)arg.Environment.Base);
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
                    UInt64 columnResultRaw = (carryOver % arg.Environment.Base);
                    columnResult = arg.Environment.Key[(Int32)columnResultRaw];
                    resultNumber.Add(columnResult);

                    carryOver = (UInt64)((Decimal)columnResultRaw / (Decimal)arg.Environment.Base);
                }

                if (carryOver > 0)
                {
                    columnResult = arg.Environment.Key[(Int32)carryOver];
                    resultNumber.Add(columnResult);
                }
            }

            var result = new Number(arg.Environment, resultNumber);

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
            if (a.Environment != b.Environment)
            {
                throw new Exception("Subtracting different math environments not supported yet");
            }

            return Number.Subtract(new Numbers(a, b));
        }

        public static Number Subtract(Numbers arg)
        {
            Int64 maxPosition = 0;
            foreach (var number in arg.NumbersSegments)
            {
                if (number.Count > maxPosition)
                {
                    maxPosition = number.Count;
                }
            }

            var resultNumber = new List<Char>();
            UInt64 carryOver = 0;
            Int64 position = 0;
            while (position < maxPosition)
            {
                UInt64 columnValue = carryOver;
                foreach (var number in arg.NumbersSegments)
                {
                    if (position < number.Count)
                    {
                        columnValue += arg.Environment.GetIndex(number[(Int32)position]);
                    }
                }

                Char columnResult;
                if (columnValue >= arg.Environment.Base)
                {
                    UInt64 columnResultRaw = (columnValue % arg.Environment.Base);
                    columnResult = arg.Environment.Key[(Int32)columnResultRaw];

                    carryOver = (UInt64)(((Decimal)columnValue - (Decimal)columnResultRaw) / (Decimal)arg.Environment.Base);
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
                    UInt64 columnResultRaw = (carryOver % arg.Environment.Base);
                    columnResult = arg.Environment.Key[(Int32)columnResultRaw];
                    resultNumber.Add(columnResult);

                    carryOver = (UInt64)((Decimal)columnResultRaw / (Decimal)arg.Environment.Base);
                }
                if (carryOver > 0)
                {
                    columnResult = arg.Environment.Key[(Int32)carryOver];
                    resultNumber.Add(columnResult);
                }
            }

            var result = new Number(arg.Environment, resultNumber);

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
            if (a.Environment != b.Environment)
            {
                throw new Exception("Multipling different math environments not supported yet");
            }

            return Number.Multiply(new Numbers(a, b));
        }

        public static Number Multiply(MathEnvironmentInfo environment, List<Char> number1, Char number2)
        {
            var resultRaw = new List<Char>();

            UInt64 numberIndex = environment.GetIndex(number2);

            UInt64 carryOver = 0;
            for (var i = 0; i < number1.Count; i++)
            {
                UInt64 segmentIndex = environment.GetIndex(number1[i]);

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

            return new Number(environment, resultRaw);
        }
        public static Number Multiply(MathEnvironmentInfo environment, List<Char> number1, List<Char> number2)
        {
            var result = new Number(environment);

            for (var i = 0; i < number1.Count; i++)
            {
                Char numberSegment = number1[i];
                Number currentResult = Number.Multiply(environment, number2, numberSegment);
                for (var i2 = 0; i2 < i; i2++)
                {
                    List<Char> currentResultList = currentResult.Segments.ToList();
                    currentResultList.Insert(0, environment.Bottom);
                    currentResult = new Number(environment, currentResultList);
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
            UInt64 number1Index = environment.GetIndex(number1);
            UInt64 number2Index = environment.GetIndex(number2);

            UInt64 resultIndex = number1Index * number2Index;

            if (resultIndex >= environment.Base)
            {
                UInt64 remainderIndex = (resultIndex % environment.Base);
                UInt64 carryOver = (resultIndex - remainderIndex) / environment.Base;

                return new Number(environment, new List<Char> { environment.Key[(Int32)carryOver], environment.Key[(Int32)remainderIndex] });
            }
            else
            {
                return new Number(environment, new List<Char> { environment.Key[(Int32)resultIndex] });
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

            List<Char> runningTotal = new List<Char> { arg.Environment.Bottom  };
            for (var i = 1; i < arg.NumbersSegments.Count; i ++)
            {
                List<Char> numbersSegment1 = runningTotal;
                if (i == 1) {
                    numbersSegment1 = arg.NumbersSegments[i - 1];
                }
                else
                {
                    numbersSegment1 = runningTotal;
                }
                List<Char> numbersSegment2 = arg.NumbersSegments[i];

                Number numbersSegment1And2Result = Number.Multiply(arg.Environment, numbersSegment1, numbersSegment2);
                runningTotal = numbersSegment1And2Result.Segments;
            }

            var result = new Number(arg.Environment, runningTotal);

            return result;
        }
        #endregion

        #region Divide

        public static String Divide(String key, String a, String b)
        {
            var environment = new MathEnvironmentInfo(key);
            return (new Number(environment, a) / new Number(environment, b)).ToString();
        }
        
        public static Number Divide(Number a, Number b)
        {
            if (a.Environment != b.Environment)
            {
                throw new Exception("Dividing different math environments not supported yet");
            }

            var result = new Number(a.Environment);
            if (a < b)
            {
                result.Fragments = new Tuple<List<Char>, List<Char>>(a.Segments, b.Segments);
                return result;
            }

            UInt64 aSize = (UInt64)a.Segments.Count;
            UInt64 bSize = (UInt64)b.Segments.Count;

            //UInt64 placesMoved = 0;

            //while (a > b)
            //{
            //    placesMoved += 1;
            //    b.Segments.Insert(0, environment.Bottom);
            //}

            return result;
        }

        public static Number Divide(MathEnvironmentInfo environment, Char numberToDivide, Char numberToDivideBy)
        {
            Number result = null;
            UInt64 indexToDivide = environment.GetIndex(numberToDivide);
            UInt64 indexToDivideBy = environment.GetIndex(numberToDivideBy);

            UInt64 remainder;

            if (indexToDivide > indexToDivideBy)
            {
                remainder = indexToDivide % indexToDivideBy;

                UInt64 resultRaw = (UInt64)System.Math.Floor((decimal)indexToDivide / (decimal)indexToDivideBy);

                result = Number.Convert(environment, resultRaw, remainder, indexToDivideBy);
            }
            else
            {
                result = Number.Convert(environment, 0, numberToDivide, numberToDivideBy);
            }
            
            return result;
        }

        #endregion



        public static Number Convert(MathEnvironmentInfo environment, UInt64 number)
        {
            return Number.Convert(environment, number, 0, 0);
        }
        public static Number Convert(MathEnvironmentInfo environment, UInt64 number, UInt64 fractionTopNumber, UInt64 fractionBottomNumber)
        {
            List<Char> resultRaw = Number.Convert(environment.Base, environment.Key, number);
            List<Char> fractionTop =  Number.Convert(environment.Base, environment.Key, fractionTopNumber);
            List<Char> fractionBottom = Number.Convert(environment.Base, environment.Key, fractionBottomNumber);
            var fragments = new Tuple<List<Char>, List<Char>>(fractionTop, fractionBottom);


            var result = new Number(environment, resultRaw, fragments);
            return result;
        }

        public static List<Char> Convert(UInt64 mathBase, IList<Char> key, UInt64 number)
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

    }
}
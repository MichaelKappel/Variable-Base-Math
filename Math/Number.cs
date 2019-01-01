using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Math
{
    public class Number: NumberBase, IEquatable<Number>, IComparable<Number>, IComparer<Number>,ICloneable
    {
        internal static IOperator Operator = new Operator();

        public Number(MathEnvironment environment, Char[] number, Boolean isNegative = false)
             : base(environment, number, isNegative)
        {

        }

        public Number(MathEnvironment environment, Char[] number, Fraction fragments, Boolean isNegative = false)
             : base(environment, number, fragments, isNegative)
        {

        }

        public Number(MathEnvironment environment, String rawNumber, Boolean isNegative = false)
                        : base(environment, rawNumber, isNegative)
        {

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
            return Operator.Add(a, b);
        }
        
        public static Number operator -(Number a, Number b)
        {
            return Operator.Subtract(a, b);
        }

        public static Number operator *(Number a, Number b)
        {
            return Operator.Multiply(a, b);
        }


        public static Number operator /(Number a, Number b)
        {
            return Operator.Divide(a, b);
        }

        
        public static Number operator %(Number a, Number b)
        {
            throw new Exception("% not supported yet");
        }

        #endregion

        public Number Copy()
        {
            var copy = new Number(this.Environment, this.Segments.ToArray(), this.Fragment, this.IsNegative);

            return copy;
        }

        public Number AsNegative()
        {
            var copy = new Number(this.Environment, this.Segments.ToArray(), this.Fragment, true);

            return copy;
        }

        public Number AsPositive()
        {
            var copy = new Number(this.Environment, this.Segments.ToArray(), this.Fragment, false);

            return copy;
        }
        
        Object ICloneable.Clone()
        {
            var copy = new Number(this.Environment, this.Segments.ToArray(), this.Fragment.Copy(), this.IsNegative);


            return copy;
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
            if (!this.Environment.Equals(other.Environment))
            {
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

            if (this.Fragment == default(Fraction))
            {
                return true;
            }
            else
            {
                return this.Fragment.Equals(other.Fragment);
            }
        }

        public int CompareTo(Number other)
        {
            if (!this.Environment.Equals(other.Environment))
            {
                //FIX: should able to tell if number values match in seperate Environments
                throw new Exception("Currently unable to compare seperate Environments");
            }
            Boolean reverse = false;
            if (!this.IsNegative && other.IsNegative)
            {
                return 1;
            }
            else if (this.IsNegative && !other.IsNegative)
            {
                return -1;
            }
            else if (this.IsNegative && other.IsNegative)
            {
                reverse = true;
            }


            Int32 result = 0;
            if (this.Segments.Count > other.Segments.Count)
            {
                result = 1;
            }
            else if (this.Segments.Count < other.Segments.Count)
            {
                result = -1;
            }

            if (result == 0)
            {
                for (var i = this.Segments.Count - 1; i >= 0; i--)
                {
                    if (this.Segments[i] != other.Segments[i])
                    {
                        if (this.Segments[i] > other.Segments[i])
                        {
                            result = 1;
                            break;
                        }
                        else if (this.Segments[i] < other.Segments[i])
                        {
                            result = -1;
                            break;
                        }
                    }
                }
            }

            if (result == 0)
            {
                if (this.Fragment != default(Fraction))
                {
                    result = this.Fragment.CompareTo(other.Fragment);
                }
            }

            if (reverse && result == 1)
            {
                return -1;
            }
            else if (reverse && result == -1)
            {
                return 1;
            }
            else
            {
                return result;
            }

        }

        public int Compare(Number x, Number y)
        {
            return x.CompareTo(y);
        }

    }
}
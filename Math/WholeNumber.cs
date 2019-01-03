using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Math
{
    public class WholeNumber : NumberBase, IEquatable<WholeNumber>, IComparable<WholeNumber>, IComparer<WholeNumber>
    {
        internal static IOperator Operator = new Operator();

        public WholeNumber(MathEnvironment environment, Char number, Boolean isNegative)
             : base(environment, number, isNegative)
        {

        }

        public WholeNumber(MathEnvironment environment, Char[] number, Boolean isNegative)
             : base(environment, number, isNegative)
        {

        }

        public WholeNumber(MathEnvironment environment, List<Char> number, Boolean isNegative)
             : base(environment, number, isNegative)
        {

        }

        public WholeNumber(MathEnvironment environment, ReadOnlyCollection<Char> number, Boolean isNegative)
             : base(environment, number, isNegative)
        {

        }


        #region operator overrides
        public static bool operator <(WholeNumber e1, WholeNumber e2)
        {
            return e1.CompareTo(e2) < 0;
        }

        public static bool operator <=(WholeNumber e1, WholeNumber e2)
        {
            return e1.CompareTo(e2) <= 0;
        }
        public static bool operator >(WholeNumber e1, WholeNumber e2)
        {
            return e1.CompareTo(e2) > 0;
        }

        public static bool operator >=(WholeNumber e1, WholeNumber e2)
        {
            return e1.CompareTo(e2) >= 0;
        }
        
        public static bool operator ==(WholeNumber e1, WholeNumber e2)
        {
            return e1.Equals(e2);
        }

        public static bool operator !=(WholeNumber e1, WholeNumber e2)
        {
            return !e1.Equals(e2);
        }
        
        public static WholeNumber operator +(WholeNumber a, WholeNumber b)
        {
            return Operator.Add(a, b);
        }
        
        public static WholeNumber operator -(WholeNumber a, WholeNumber b)
        {
            return Operator.Subtract(a, b);
        }

        public static WholeNumber operator *(WholeNumber a, WholeNumber b)
        {
            return Operator.Multiply(a, b);
        }


        public static Number operator /(WholeNumber a, WholeNumber b)
        {
            return Operator.Divide(a, b);
        }

        
        public static WholeNumber operator %(WholeNumber a, WholeNumber b)
        {
            throw new Exception("% not supported yet");
        }

        #endregion

        public WholeNumber AsNegative()
        {
            var copy = new WholeNumber(this.Environment, this.Segments, true);

            return copy;
        }

        public WholeNumber AsPositive()
        {
            var copy = new WholeNumber(this.Environment, this.Segments, false);

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
            return this.Equals((WholeNumber)other);
        }

        public Boolean Equals(WholeNumber other)
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
            
            return true;
        }

        public int CompareTo(WholeNumber other)
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

        public int Compare(WholeNumber x, WholeNumber y)
        {
            return x.CompareTo(y);
        }

        public Number AsNumber()
        {
            return new Number(this.Environment, this.Segments, this.IsNegative);
        }
    }
}
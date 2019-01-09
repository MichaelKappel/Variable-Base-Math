using Math.Base;
using Math.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Math
{
    public class Number: WholeNumber, IEquatable<Number>, IComparable<Number>, IComparer<Number>
    {
        internal INumberOperator Operator = new NumberOperator();

        internal Number(WholeNumber number, Fraction fragment = null)
         : base(number.Environment, number.Segments, number.IsNegative)
        {
            this.Fragment = fragment;
        }

        internal Number(WholeNumber number, Number numerator, Number denominator)
            : base(number.Environment, number.Segments, number.IsNegative)
        {
            this.Fragment = new Fraction(numerator, denominator);
        }

        internal Number(MathEnvironment environment, Char number, Boolean isNegative, Fraction fragment = null)
             : base(environment, number,  isNegative)
        {
            this.Fragment = fragment;
        }

        internal Number(MathEnvironment environment, Char[] number, Boolean isNegative, Fraction fragment = null)
             : base(environment, number,  isNegative)
        {
            this.Fragment = fragment;
        }

        internal Number(MathEnvironment environment, List<Char> number, Boolean isNegative, Fraction fragment = null)
             : base(environment, number, isNegative)
        {
            this.Fragment = fragment;
        }
        internal Number(MathEnvironment environment, ReadOnlyCollection<Char> number, Boolean isNegative, Fraction fragment = null)
             : base(environment, number, isNegative)
        {
            this.Fragment = fragment;
        }

        internal Number(MathEnvironment environment, ReadOnlyCollection<Char> number, ReadOnlyCollection<Char> numerator, ReadOnlyCollection<Char> denominator, Boolean isNegative)
            : base(environment, number, isNegative)
        {
            this.Fragment = new Fraction(environment, numerator, denominator);
        }

        public Fraction Fragment { get; protected set; }


        #region operator overrides
        public static bool operator <(Number a, Number b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator <=(Number a, Number b)
        {
            return a.CompareTo(b) <= 0;
        }
        public static bool operator >(Number a, Number b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator >=(Number a, Number b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static bool operator ==(Number e1, Number e2)
        {
            if (e1 is null && e2 is null)
            {
                return true;
            }
            return e1.Equals(e2);
        }

        public static bool operator !=(Number e1, Number e2)
        {
            if (e1 is null && e2 is null)
            {
                return false;
            }
            else if (e1 is null)
            {
                return !e2.Equals(e1); ;
            }
            else
            {
                return !e1.Equals(e2);
            }
        }


        public static Number operator +(Number a, Number b)
        {
            return a.Operator.Add(a, b);
        }
        
        public static Number operator -(Number a, Number b)
        {
            return a.Operator.Subtract(a, b);
        }

        public static Number operator *(Number a, Number b)
        {
            return a.Operator.Multiply(a, b);
        }


        public static Number operator /(Number a, Number b)
        {
            return a.Operator.Divide(a, b);
        }

        
        public static Number operator %(Number a, Number b)
        {
            throw new Exception("% not supported yet");
        }

        #endregion

        internal Number Copy()
        {
            var copy = new Number(this.Environment, this.Segments, this.IsNegative, this.Fragment);

            return copy;
        }

        internal Number AsNegativeNumber()
        {
            var copy = new Number(this.Environment, this.Segments, true, this.Fragment);

            return copy;
        }

        internal Number AsPositiveNumber()
        {
            var copy = new Number(this.Environment, this.Segments, false, this.Fragment);

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
            if (other == null)
            {
                if (this.Environment.Algorithm.IsBottom(this.Segments))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            var otherWholeNumber = (other as WholeNumber);
            if (otherWholeNumber != default(WholeNumber))
            {
                if(this.Fragment != default(Fraction))
                {
                    return false;
                }
                else if(this.IsNegative != otherWholeNumber.IsNegative)
                {
                    return false;
                }
                else
                {
                    return this.Environment.Algorithm.IsEqual(this.Segments, otherWholeNumber.Segments);
                }
            }
            else
            {
                //Fix: Try to determine real equlity for equivalent numbers types like Int32, Decimal, ect… 
                return false;
            }
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

        public override String ToString()
        {
            String result = base.ToString();

            if (this.Fragment != default(Fraction))
            {
                result = String.Format("{0} {1}", result, this.Fragment);
            }

            return result;
        }

        public WholeNumber Floor()
        {
            return new WholeNumber(this.Environment, this.Segments, this.IsNegative);
        }

        
    }
}
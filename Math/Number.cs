using Math.Base;
using Math.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Math
{
    public class Number: NumberBase, IEquatable<Number>, IComparable<Number>
    {
        internal IOperator Operator = new Operator();

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
            return this.Operator.Equals(this, (Number)other);
        }

        public Boolean Equals(Number other)
        {
            return this.Operator.Equals(this, other);
        }

        public int CompareTo(Number other)
        {
            return this.Operator.Compare(this, other);
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
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace Math
{
    public class Fraction: IEquatable<Fraction>, IComparable<Fraction>, IComparer<Fraction>
    {
        public Fraction(MathEnvironment environment, Char numerator, Char denominator)
            : this(new Number(environment, new Char[]{ numerator }), new Number(environment, new Char[]{ denominator }))
        {
            
        }

        public Fraction(MathEnvironment environment, Char[] numerator, Char[] denominator)
            : this(new Number(environment, numerator), new Number(environment, denominator))
        {
            
        }

        public Fraction(Number numerator, Number denominator)
        {
            this.Numerator =  numerator;
            this.Denominator = denominator;

            if (this.Numerator == numerator.Environment.BottomNumber)
            {
                throw new Exception("Numerator larger then denominator not currently supported");
            }
            if (this.Numerator == numerator.Environment.BottomNumber)
            {
                throw new DivideByZeroException("Numerator of nothing not currently supported");
            }
            if (this.Denominator == numerator.Environment.BottomNumber)
            {
                throw new DivideByZeroException("Denominator of nothing not currently supported");
            }
        }

        public Number Numerator { get; set; }
        public Number Denominator { get; set; }

        #region overrides
        public override String ToString()
        {
            return String.Format("{0}/{1}", this.Numerator, this.Denominator);
        }

        public Fraction Copy()
        {
            var copy = new Fraction(this.Numerator.Copy(), this.Denominator.Copy());

            return copy;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                Int64 hashCode1 = this.Numerator.GetHashCode();
                Int64 hashCode2 = this.Denominator.GetHashCode();
                Int64 hashCode = hashCode1 + hashCode2;
                if (hashCode <= Int32.MaxValue)
                {
                    return (Int32)hashCode;
                }
                else
                {
                    return (Int32)(hashCode - Int32.MaxValue);
                }
            }
        }

        public override Boolean Equals(Object other)
        {
            return this.Equals((Fraction)other);
        }

        public Boolean Equals(Fraction other)
        {

            if (object.ReferenceEquals(this, default(Fraction)) && object.ReferenceEquals(other, default(Fraction)))
            {
                return true;
            }
            else if (object.ReferenceEquals(this, default(Fraction)) || object.ReferenceEquals(other, default(Fraction)))
            {
                return false;
            }

            if (!this.Denominator.Equals(other.Denominator))
            {
                Number commonDenominator = this.Denominator * other.Denominator;

                Number otherNumerator = other.Numerator * this.Denominator;
                Number thisNumerator = this.Numerator * other.Denominator;

                if (thisNumerator == otherNumerator)
                {
                    return true;
                }

                return false;
            }

            if (!this.Numerator.Equals(other.Numerator))
            {
                return false;
            }


            return true;
        }

        public int CompareTo(Fraction other)
        {
            if (!this.Denominator.Equals(other.Denominator))
            {
                Number commonDenominator = this.Denominator * other.Denominator;

                Number otherNumerator = other.Numerator * this.Denominator;
                Number thisNumerator = this.Numerator * other.Denominator;

                if (thisNumerator > otherNumerator)
                {
                    return 1;
                }
                else if (thisNumerator < otherNumerator)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }

            if (this.Numerator > other.Numerator)
            {
                return 1;
            }
            else if (this.Numerator < other.Numerator)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        public int Compare(Fraction x, Fraction y)
        {
            return x.CompareTo(y);
        }
        #endregion

        #region operator overrides
        public static bool operator <(Fraction e1, Fraction e2)
        {
            if (e1 == default(Fraction) || e2 == default(Fraction))
            {
                return false;
            }
            return e1.CompareTo(e2) < 0;
        }

        public static bool operator <=(Fraction e1, Fraction e2)
        {
            if (e1 == default(Fraction) && e2 == default(Fraction))
            {
                return true;
            }
            else if (e1 == default(Fraction) || e2 == default(Fraction))
            {
                return false;
            }
            return e1.CompareTo(e2) <= 0;
        }
        public static bool operator >(Fraction e1, Fraction e2)
        {
            if (e1 == default(Fraction) || e2 == default(Fraction))
            {
                return false;
            }
            return e1.CompareTo(e2) > 0;
        }

        public static bool operator >=(Fraction e1, Fraction e2)
        {
            if (e1 == default(Fraction) && e2 == default(Fraction))
            {
                return true;
            }
            else if (e1 == default(Fraction) || e2 == default(Fraction))
            {
                return false;
            }
            return e1.CompareTo(e2) >= 0;
        }

        public static bool operator ==(Fraction a, Fraction b)
        {
            if (object.ReferenceEquals(a, default(Fraction)) && object.ReferenceEquals(b, default(Fraction)))
            {
                return true;
            }
            else if (object.ReferenceEquals(a, default(Fraction)) || object.ReferenceEquals(b, default(Fraction)))
            {
                return false;
            }
            return !a.Equals(b);
        }

        public static bool operator !=(Fraction a, Fraction b)
        {
            if (object.ReferenceEquals(a, default(Fraction)) && object.ReferenceEquals(b, default(Fraction)))
            {
                return false;
            }
            else if (object.ReferenceEquals(a, default(Fraction)) || object.ReferenceEquals(b, default(Fraction)))
            {
                return true;
            }
            return !a.Equals(b);
        }

        #endregion
    }
}

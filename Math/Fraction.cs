using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Math
{
    public class Fraction: IEquatable<Fraction>, IComparable<Fraction>
    {
        internal Fraction(MathEnvironment environment, Char numerator, Char denominator)
            : this(new Number(environment, numerator, false), new Number(environment, denominator, false))
        {
            
        }

        internal Fraction(MathEnvironment environment, Char[] numerator, Char[] denominator)
            : this(new Number(environment, numerator, false), new Number(environment, denominator, false))
        {
            
        }

        internal Fraction(MathEnvironment environment, List<Char> numerator, List<Char> denominator)
            : this(new Number(environment, numerator, false), new Number(environment, denominator, false))
        {

        }

        internal Fraction(MathEnvironment environment, ReadOnlyCollection<Char> numerator, ReadOnlyCollection<Char> denominator)
            : this(new Number(environment, numerator, false), new Number(environment, denominator, false))
        {

        }

        internal Fraction(Number numerator, Number denominator)
        {
            this.Numerator =  numerator;
            this.Denominator = denominator;
        }

        public Number Numerator { get; protected set; }
        public Number Denominator { get; protected set; }

        #region overrides
        public override String ToString()
        {
            return String.Format("{0}/{1}", this.Numerator, this.Denominator);
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
            return this.Compare(other) == 0;
        }

        public int Compare(Fraction other)
        {
            if (object.ReferenceEquals(this, default(Fraction)) && object.ReferenceEquals(other, default(Fraction)))
            {
                return 0;
            }
            else if (object.ReferenceEquals(this, default(Fraction)))
            {
                return -1;
            }
            else if (object.ReferenceEquals(other, default(Fraction)))
            {
                return 1;
            }

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

        public Int32 CompareTo(Fraction other)
        {
            return this.Compare(other);
        }

        #endregion

        #region operator overrides
        public static bool operator <(Fraction e1, Fraction e2)
        {
            if (e1 == default(Fraction) || e2 == default(Fraction))
            {
                return false;
            }
            return e1.Compare(e2) < 0;
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
            return e1.Compare(e2) <= 0;
        }
        public static bool operator >(Fraction e1, Fraction e2)
        {
            if (e1 == default(Fraction) || e2 == default(Fraction))
            {
                return false;
            }
            return e1.Compare(e2) > 0;
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
            return e1.Compare(e2) >= 0;
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
            return a.Equals(b);
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

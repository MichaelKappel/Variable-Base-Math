using VariableBase.Mathematics.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleToAttribute("Math.Tests")]

namespace VariableBase.Mathematics
{
    public struct Number:IEquatable<Number>, IComparable<Number>, IDisposable
    {
        internal static INumberOperator Operator = new NumberOperator();

        public Fraction Fragment { get; set; }

        public Double First { get; set; }
        
        public Boolean IsNegative { get; set; }

        public IMathEnvironment Environment { get; set; }

        public ReadOnlyCollection<Double> Segments { get; set; }
        
        internal Number(IMathEnvironment environment, ReadOnlyCollection<Double> segments, ReadOnlyCollection<Double> numerator, ReadOnlyCollection<Double> denominator, Boolean isNegative)
        {
            if (segments == default(ReadOnlyCollection<Double>) || segments.Count == 0)
            {
                segments = new ReadOnlyCollection<Double>(new Double[]{ 0 });
            }

            this.Environment = environment;

            this.IsNegative = isNegative;

            this.Segments = segments;

            if (numerator != default(ReadOnlyCollection<Double>) && denominator != default(ReadOnlyCollection<Double>))
            { 
                this.Fragment = new Fraction(environment, numerator, denominator);
            }
            else
            {
                this.Fragment = default(Fraction);
            }

            this.First = this.Segments[this.Segments.Count - 1];
            if (this.First == 0 && this.Segments.Count > 1)
            {
                throw new Exception("Numbers longer then a power can not start with bottom number char");
            }
        }

        internal Number(IMathEnvironment environment, ReadOnlyCollection<Double> segments, Fraction fragment, Boolean isNegative)
        {
            this.Environment = environment;

#if DEBUG
            foreach (Double segment in segments)
            {
                if (segment > this.Environment.Base) {
                    throw new Exception("Bad number segment larger than base");
                }
                else if (segment < 0)
                {
                    throw new Exception("Bad number segment less than zero");
                }
            }
#endif

            this.IsNegative = isNegative;

            this.Segments = segments;

            this.Fragment = fragment;

            this.First = this.Segments[this.Segments.Count - 1];
            if (this.First == 0 && this.Segments.Count > 1)
            {
                throw new Exception("Numbers longer then a power can not start with bottom number char");
            }

        }


        #region operator overrides
        public static bool operator <(Number a, Number b)
        {
            return Operator.Compare(a, b) < 0;
        }

        public static bool operator <=(Number a, Number b)
        {
            return Operator.Compare(a, b) <= 0;
        }
        public static bool operator >(Number a, Number b)
        {
            return Operator.Compare(a, b) > 0;
        }

        public static bool operator >=(Number a, Number b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static bool operator ==(Number a, Number b)
        {
            return Operator.IsEqual(a, b);
        }

        public static bool operator !=(Number a, Number b)
        {
            return !Operator.IsEqual(a, b);
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
            Number totalResult = Operator.Divide(a, b);
            return new Number(a.Environment, new ReadOnlyCollection<Double>(new Double[] { 0 }), totalResult.Fragment, totalResult.IsNegative);
        }

        #endregion

        internal Number Copy()
        {
            return new Number(this.Environment, this.Segments, this.Fragment, this.IsNegative);
        }

        internal Number AsNegativeNumber()
        {
            return new Number(this.Environment, this.Segments, this.Fragment, true);
        }

        internal Number AsPositiveNumber()
        {
            return new Number(this.Environment, this.Segments, this.Fragment, false);
        }

        internal Number Floor()
        {
            return new Number(this.Environment, this.Segments, null, this.IsNegative);
        }


        public Tuple<Number, Number> GetComposite()
        {
            return Operator.GetComposite(this);
        }

        public Boolean IsPrime()
        {
            return Operator.IsPrime(this);
           
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
            return Operator.IsEqual(this, (Number)other);
        }

        public Boolean Equals(Number other)
        {
            return Operator.IsEqual(this, other);
        }

        public int CompareTo(Number other)
        {
            return Operator.Compare(this, other);
        }

        public Number Convert(IMathEnvironment environment)
        {
            Number result = Operator.Convert(environment, this);
#if DEBUG
            Number resultReverse = Operator.Convert(this.Environment, result);
            if (resultReverse != this)
            {
                throw new Exception(String.Format("Convert Reveerse Failed {0} {1}", result, resultReverse));
            }
#endif
            return result;
        }
        
        public Fraction AsFraction()
        {
            if (this.Fragment != default(Fraction) && (this.Environment != this.Fragment.Denominator.Environment || this.Fragment.Denominator.Environment != this.Fragment.Denominator.Environment))
            {
                throw new Exception("Fractions in differnt enviorments is not currently supported");
            }

            Number numerator;
            Number denominator;

            if (this.Fragment == default(Fraction))
            {
                numerator = this;
                denominator = this.Environment.KeyNumber[1];
            }
            else 
            {

                ReadOnlyCollection<Double> numeratorRaw =  this.Environment.BasicMath.Add(this.Environment.BasicMath.Multiply(this.Segments, this.Fragment.Denominator.Segments), this.Fragment.Numerator.Segments);
                numerator = new Number(this.Environment, numeratorRaw, null, false);

                ReadOnlyCollection<Double> denominatorRaw =  this.Environment.BasicMath.Add(this.Environment.BasicMath.Multiply(this.Segments, this.Fragment.Denominator.Segments), this.Fragment.Numerator.Segments);
                denominator = new Number(this.Environment, denominatorRaw, null, false);

                if (this.Fragment.Numerator.Fragment != default(Fraction) || this.Fragment.Denominator.Fragment != default(Fraction))
                {
                    Fraction numeratorFraction = numerator.AsFraction();
                    if (this.Fragment.Numerator.Fragment != default(Fraction))
                    {
                        numeratorFraction = numerator.AsFraction() + this.Fragment.Numerator.Fragment;
                    }

                    Fraction denominatorFraction = numerator.AsFraction();
                    if (this.Fragment.Numerator.Fragment != default(Fraction))
                    {
                        denominatorFraction = denominator.AsFraction() + this.Fragment.Denominator.Fragment;
                    }

                    Fraction fractionResult = numeratorFraction / denominatorFraction;
                    numerator = fractionResult.Numerator;
                    denominator = fractionResult.Denominator;
                }
            }

            return new Fraction(numerator, denominator);
        }

        public String GetActualValue(IMathEnvironment environment)
        {
            String result = String.Empty;
            if (this.IsNegative)
            {
                result = "-" + result;
            }

            ReadOnlyCollection<Double> resultSegments;
            if (environment != this.Environment)
            {
                resultSegments = this.Convert(environment).Segments;
            }
            else
            {
                resultSegments = this.Segments;
            }
            if (environment.Base < UInt16.MaxValue)
            {
                for (Double i = resultSegments.Count; i > 0; i--)
                {
                    result += environment.Key[(Int32)resultSegments[(Int32)i - 1]];
                }
            }
            else
            {
                result += String.Join(',', resultSegments);
            }

            if (this.Fragment != default(Fraction))
            {
                result = String.Format("{0} {1}", result, this.Fragment);
            }

            return result;
        }
        public String GetDisplayValue()
        {
            String result = String.Empty;
            if (this.IsNegative)
            {
                result = "-" + result;
            }
            if (this.Segments.Count > 200)
            {
                result += String.Format("{0}e{1}", this.Environment.Key[(Int32)this.First], this.Segments.Count);
                if (this.Fragment != default(Fraction))
                {
                    result += String.Format("{0} {1}", result, this.Fragment);
                }
            }
            else
            {
                result += String.Concat(this.Segments.Select(x => x.ToString("G17")).Reverse());
                if (this.Environment.Base == 10)
                {
                    result = result.Replace(" ", "");
                }
                if (this.Fragment != default(Fraction))
                {
                    result = String.Format("{0} {1}", result, this.Fragment);
                }
            }
            return result;
        }

        public String ToString(IMathEnvironment environment)
        {
            return this.GetActualValue(environment);
        }

        public override String ToString()
        {
            return this.GetDisplayValue();
        }

        public void Dispose()
        {
            this.Fragment = default(Fraction);

            this.First = 0;
            
            this.IsNegative = false;

            this.Environment = default(IMathEnvironment);

            this.Segments = default(ReadOnlyCollection<Double>);
        }
    }
}
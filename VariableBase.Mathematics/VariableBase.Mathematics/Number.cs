using VariableBase.Mathematics.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using VariableBase.Mathematics.Models;
using VariableBase.Mathematics.Algorithms;
using VariableBase.Mathematics.Operators;

[assembly: InternalsVisibleToAttribute("Math.Tests")]
namespace VariableBase.Mathematics
{
    public struct Number:IEquatable<Number>, IComparable<Number>, IEquatable<Decimal>, IComparable<Decimal>, IDisposable
    {
        public static INumberOperator Operator = new NumberOperator(new BasicMathAlgorithm());

        public static Boolean IsBottom(Number number)
        {
            return (number.Size == 1 && number.Segments[0] == 0);
        }

        public static Boolean IsFirst(Number number)
        {
            return (number.Size == 1 && number.Segments[0] == 1);
        }

        public static Boolean IsBottom(NumberSegments segments)
        {
            return (segments.Size == 1 && segments[0] == 0);
        }

        public static Boolean IsFirst(NumberSegments segments)
        {
            return (segments.Size == 1 && segments[0] == 1);
        }

        public Fraction Fragment { get; set; }

        public Decimal First { get; set; }
        
        public Boolean IsNegative { get; set; }

        public IMathEnvironment Environment { get; set; }

        public NumberSegments Segments { get; set; }
        public int Size { get; private set; }

        internal Number(IMathEnvironment environment, NumberSegments segments, NumberSegments numerator, NumberSegments denominator, Boolean isNegative)
        {
            if (segments == default(NumberSegments) || segments.Size == 0)
            {
                segments = new NumberSegments(new Decimal[]{ 0 });
            }

            this.Environment = environment;

            this.IsNegative = isNegative;

            this.Segments = segments;

            this.Size = segments.Length;

            if (numerator != default(NumberSegments) && denominator != default(NumberSegments))
            { 
                this.Fragment = new Fraction(environment, numerator, denominator);
            }
            else
            {
                this.Fragment = default(Fraction);
            }

            this.First = this.Segments[this.Segments.Size - 1];
            if (this.First == 0 && this.Segments.Size > 1)
            {
                throw new Exception("Numbers longer then a power can not start with bottom number char");
            }
        }


        internal Number(Fraction fraction)
        {
            if (fraction.Numerator.Environment != fraction.Numerator.Environment)
            {
                throw new Exception("Adding differnt enviorments is not currently supported");
            }
            IMathEnvironment environment = fraction.Numerator.Environment;

            this.Environment = environment;

            if (fraction.Denominator == 0)
            {
                this.Segments = environment.KeyNumber[0].Segments;
                this.Fragment = default(Fraction);
            }
            else if (Operator.IsGreaterThan(fraction.Denominator, fraction.Numerator))
            {
                this.Segments = new NumberSegments(new Decimal[] { 0 });
                this.Fragment = fraction; 
            }
            else
            {

                Number numerator = fraction.Numerator;
                Number denominator = fraction.Denominator;

                if (denominator.Fragment != default(Fraction) || denominator.Fragment != default(Fraction))
                {
                    var aFraction = default(Fraction);
                    if (numerator.Fragment != default(Fraction))
                    {
                        Number aDividend = Operator.Add(Operator.Multiply(numerator, numerator.Fragment.Denominator), numerator.Fragment.Numerator);
                        aFraction = new Fraction(aDividend, environment.KeyNumber[1]);
                    }
                    else
                    {
                        aFraction = new Fraction(numerator.Environment, numerator.Segments, environment.KeyNumber[1].Segments);
                    }

                    var bFraction = default(Fraction);
                    if (denominator.Fragment != default(Fraction))
                    {
                        Number bDividend = Operator.Add(Operator.Multiply(denominator, denominator.Fragment.Denominator), denominator.Fragment.Numerator);
                        bFraction = new Fraction(bDividend, environment.KeyNumber[1]);

                    }
                    else if (aFraction != default(Fraction))
                    {
                        bFraction = new Fraction(denominator.Environment, denominator.Segments, environment.KeyNumber[1].Segments);
                    }

                    Fraction fractionResult = aFraction / bFraction;
                    numerator = fractionResult.Numerator;
                    denominator = fractionResult.Denominator;
                }

                Number resultSegments = Operator.Divide(numerator, denominator);
                this.Segments = resultSegments.Segments;
                if (resultSegments.Fragment == default(Fraction))
                {
                    this.Fragment = default(Fraction);
                }
                else
                {
                    this.Fragment = new Fraction(numerator, denominator);
                }
            }
            this.Size = this.Segments.Length;
            this.IsNegative = false;
            this.First = this.Segments[this.Segments.Length-1];
        }


        internal Number(IMathEnvironment environment, NumberSegments segments, Fraction fragment, Boolean isNegative)
        {
            this.Environment = environment;

#if DEBUG
            foreach (Decimal segment in segments)
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

            this.Size = segments.Length;

            this.First = this.Segments[this.Segments.Size - 1];
            if (this.First == 0 && this.Segments.Size > 1)
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
            return Operator.Compare(a, b) == 0;
        }

        public static bool operator !=(Number a, Number b)
        {
            return Operator.Compare(a, b) != 0;
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
            return new Number(a.Environment, new NumberSegments(new Decimal[] { 0 }), totalResult.Fragment, totalResult.IsNegative);
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

        public static implicit operator Decimal(Number d) 
        {
            return d.Segments[0];
        }

        public static implicit operator Int64(Number d)
        {
            return (Int64)d.Segments[0];
        }


        public static implicit operator UInt64(Number d)
        {
            return (UInt64)d.Segments[0];
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
            return Operator.Compare(this, (Number)other) == 0;
        }

        public Boolean Equals(Number other)
        {
            return Operator.Compare(this, other) == 0;
        }

        public Int32 CompareTo(Number other)
        {
            return Operator.Compare(this, other);
        }

        public Boolean Equals(Decimal other)
        {
            return Operator.Compare(this, other) == 0;
        }

        public Int32 CompareTo(Decimal other)
        {
            return Operator.Compare(this, other);
        }

        public Number Convert(IMathEnvironment environment)
        {
            Number result = Operator.Convert(environment, this);
            return result;
        }
        
        public String GetCharArray(IMathEnvironment environment = default(IMathEnvironment))
        {
            String result = String.Empty;
            if (this.IsNegative)
            {
                result = "-" + result;
            }

            NumberSegments resultSegments;
            if (environment != default(IMathEnvironment) && environment != this.Environment)
            {
                resultSegments = this.Convert(environment).Segments;
            }
            else
            {
                resultSegments = this.Segments;
            }

            result += String.Concat(resultSegments.Select(x => (Char)x).Reverse());

            if (this.Fragment != default(Fraction))
            {
                result = String.Format("{0} {1}", result, this.Fragment);
            }

            return result;
        }

        public String GetDecimalArray(IMathEnvironment environment = default(IMathEnvironment))
        {
            String result = String.Empty;
            if (this.IsNegative)
            {
                result = "-" + result;
            }

            NumberSegments resultSegments;
            if (environment != default(IMathEnvironment) && environment != this.Environment)
            {
                resultSegments = this.Convert(environment).Segments;
            }
            else
            {
                resultSegments = this.Segments;
            }

             result += String.Join(',', resultSegments.Reverse());

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

            result += this.Segments.ToString();
         
            if (this.Fragment != default(Fraction))
            {
                result += String.Format("{0} {1}", result, this.Fragment);
            }

            return result;
        }

        public String ToString(IMathEnvironment environment)
        {
            return this.GetDecimalArray(environment);
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

            this.Segments = default(NumberSegments);
        }
    }
}
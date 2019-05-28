using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Common.Models;
using VariableBase.Mathematics.Algorithms;
using VariableBase.Mathematics.Operators;
using System.IO;
using System.Diagnostics;
using Common.Interfaces;

[assembly: InternalsVisibleToAttribute("Math.Tests")]
namespace VariableBase.Mathematics
{
    public struct Number:IEquatable<Number>, IComparable<Number>, IDisposable
    {
        public static IOperator<Number> Operator = new NumberOperator(new BasicMathAlgorithm());
        public static IStorageRepository StorageRepository = new AzureBlobStorageRepository();

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
        
        public Boolean? Even { get; set; }

        public Boolean IsOdd()
        {
            if (!this.Even.HasValue)
            {
                this.Even = Operator.IsEven(this);
            }
            return !Even.Value;
        }


        public Boolean IsEven()
        {
            if (!this.Even.HasValue)
            {
                this.Even = Operator.IsEven(this);
            }
            return Even.Value;
        }

        public IMathEnvironment<Number> Environment { get; set; }

        public NumberSegments Segments { get; set; }
        public int Size { get; private set; }

        public Number(IMathEnvironment<Number> environment, NumberSegments segments, NumberSegments numerator, NumberSegments denominator, Boolean isNegative)
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

            this.Even = null;
        }


        internal Number(Fraction fraction)
        {
            if (fraction.Numerator.Environment != fraction.Numerator.Environment)
            {
                throw new Exception("Adding differnt enviorments is not currently supported");
            }
            IMathEnvironment<Number> environment = fraction.Numerator.Environment;

            this.Environment = environment;

            if (fraction.Denominator == 0)
            {
                this.Segments = environment.GetNumber(0).Segments;
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
                        aFraction = new Fraction(aDividend, environment.GetNumber(1));
                    }
                    else
                    {
                        aFraction = new Fraction(numerator.Environment, numerator.Segments, environment.GetNumber(1).Segments);
                    }

                    var bFraction = default(Fraction);
                    if (denominator.Fragment != default(Fraction))
                    {
                        Number bDividend = Operator.Add(Operator.Multiply(denominator, denominator.Fragment.Denominator), denominator.Fragment.Numerator);
                        bFraction = new Fraction(bDividend, environment.GetNumber(1));

                    }
                    else if (aFraction != default(Fraction))
                    {
                        bFraction = new Fraction(denominator.Environment, denominator.Segments, environment.GetNumber(1).Segments);
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


            this.Even = null;
        }


        internal Number(IMathEnvironment<Number> environment, NumberSegments segments, Fraction fragment, Boolean isNegative)
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
            
            this.Even = null;
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
#if DEBUG
            Console.WriteLine("Add Number");
#endif
            return Operator.Add(a, b);
        }
        
        public static Number operator -(Number a, Number b)
        {
#if DEBUG
            Console.WriteLine("Subtract Number");
#endif
            return Operator.Subtract(a, b);
        }

        public static Number operator *(Number a, Number b)
        {
#if DEBUG
            Console.WriteLine("Multiply Number");
#endif
            return Operator.Multiply(a, b);
        }


        public static Number operator /(Number a, Number b)
        {
#if DEBUG
            Console.WriteLine("Divide Number");
#endif
            return Operator.Divide(a, b);
        }

        
        public static Number operator %(Number a, Number b)
        { 
            Number totalResult = Operator.Divide(a, b);
            return new Number(a.Environment, new NumberSegments(new Decimal[] { 0 }), totalResult.Fragment, totalResult.IsNegative);
        }

#endregion

        public Boolean SaveFile(String folderName, String fileName)
        {
            try
            {

                IMathEnvironment<Number> environment = this.Environment;
                String numberString = new String(this.Segments.Select(x => environment.Key[(Int32)x]).Reverse().ToArray());


                StorageRepository.Create(folderName, fileName, numberString);

                return true;
            }
            catch (Exception ex)
            {
                Debug.Write("SaveFile ERROR: " + ex.Message);
                return false;
            }
        }

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

        public Number Convert(IMathEnvironment<Number> environment)
        {
            return Operator.Convert(environment, this);
        }

        public Number Square()
        {
            return Operator.Square(this);
        }

        public Number SquareRoot()
        {
            return Operator.SquareRoot(this);
        }

        public String GetCharArray(IMathEnvironment<Number> environment = default(IMathEnvironment<Number>))
        {
            String result = String.Empty;
            if (this.IsNegative)
            {
                result = "-" + result;
            }

            NumberSegments resultSegments;
            if (environment != default(IMathEnvironment<Number>) && environment != this.Environment)
            {
                resultSegments = this.Convert(environment).Segments;
            }
            else
            {
                environment = this.Environment;
                resultSegments = this.Segments;
            }

            result += String.Concat(resultSegments.Select(x => environment.Key[(Int32)x]).Reverse());

            if (this.Fragment != default(Fraction))
            {
                result = String.Format("{0} {1}", result, this.Fragment);
            }

            return result;
        }

        public String GetDecimalArray(IMathEnvironment<Number> environment = default(IMathEnvironment<Number>))
        {
            String result = String.Empty;
            if (this.IsNegative)
            {
                result = "-" + result;
            }

            NumberSegments resultSegments;
            if (environment != default(IMathEnvironment<Number>) && environment != this.Environment)
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
                result = String.Format("{0} {1}", result, this.Fragment);
            }

            return result;
        }

        public String ToString(IMathEnvironment<Number> environment)
        {
            return this.GetDecimalArray(environment);
        }

        public override String ToString()
        {
            return this.GetDisplayValue();
        }

        public Number ConvertToBase10()
        {
            return Operator.ConvertToBase10(this);
        }

        public void Dispose()
        {
            this.Fragment = default(Fraction);

            this.First = 0;
            
            this.IsNegative = false;

            this.Environment = default(IMathEnvironment<Number>);

            this.Segments.Dispose();
        }
    }
}
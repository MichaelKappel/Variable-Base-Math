using System;
using System.Linq;
using System.Runtime.CompilerServices;
using NS12.VariableBase.Mathematics.Common.Models;
using NS12.VariableBase.Mathematics.Providers.Algorithms;
using NS12.VariableBase.Mathematics.Providers.Operators;
using System.IO;
using System.Diagnostics;
using NS12.VariableBase.Mathematics.Common.Interfaces;

[assembly: InternalsVisibleTo("Math.Tests")]
namespace NS12.VariableBase.Mathematics.Providers
{
    public struct Number : IEquatable<Number>, IComparable<Number>, IDisposable
    {
        public static IOperator<Number> Operator = new NumberOperator(new BasicMathAlgorithm());
        //public static IStorageRepository StorageRepository = new AzureBlobStorageRepository();

        public static bool IsBottom(Number number)
        {
            return number.Size == 1 && number.Whole[0] == 0;
        }

        public static bool IsFirst(Number number)
        {
            return number.Size == 1 && number.Whole[0] == 1;
        }

        public static bool IsBottom(NumberSegments segments)
        {
            return segments.Size == 1 && segments[0] == 0;
        }

        public static bool IsFirst(NumberSegments segments)
        {
            return segments.Size == 1 && segments[0] == 1;
        }

        /// <summary>
        /// Must be nullable to avoid infinite recursion
        /// </summary>
        public Fraction? Fragment { get; set; }

        public decimal First { get; set; }

        public bool IsNegative { get; set; }

        public bool? Even { get; set; }

        public bool IsOdd()
        {
            if (!Even.HasValue)
            {
                Even = Operator.IsEven(this);
            }
            return !Even.Value;
        }


        public bool IsEven()
        {
            if (!Even.HasValue)
            {
                Even = Operator.IsEven(this);
            }
            return Even.Value;
        }

        public IMathEnvironment<Number> Environment { get; set; }

        public NumberSegments Whole { get; set; }
        public int Size { get; private set; }

        public Number(IMathEnvironment<Number> environment, NumberSegments segments, bool isNegative = false)
            :this(environment, segments, null, isNegative)
        {
           
        }

        public Number(IMathEnvironment<Number> environment, NumberSegments wholeNumber, NumberSegments numerator, NumberSegments denominator, bool isNegative)
        {
            if (wholeNumber == default(NumberSegments) || wholeNumber.Size == 0)
            {
                wholeNumber = new NumberSegments(new decimal[] { 0 });
            }

            this.Environment = environment;
            this.IsNegative = isNegative;
            this.Whole = wholeNumber;
            this.Size = wholeNumber.Length;

            if (numerator != default(NumberSegments) && denominator != default(NumberSegments))
            {
                this.Fragment = new Fraction(environment, numerator, denominator);
            }
            else
            {
                this.Fragment = default;
            }

            this.First = this.Whole[this.Whole.Size - 1];
            if (this.First == 0 && this.Whole.Size > 1)
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

            Environment = environment;

            if (fraction.Denominator == 0)
            {
                this.Whole = environment.GetNumber(0).Whole;
                this.Fragment = default;
            }
            else if (Operator.IsGreaterThan(fraction.Denominator, fraction.Numerator))
            {
                this.Whole = new NumberSegments(new decimal[] { 0 });
                this.Fragment = fraction;
            }
            else
            {
                Number numerator = fraction.Numerator;
                Number denominator = fraction.Denominator;

                if (denominator.Fragment != default || denominator.Fragment != default)
                {
                    Fraction aFraction;
                    if (numerator.Fragment != default)
                    {
                        Number aDividend = Operator.Add(Operator.Multiply(numerator, numerator.Fragment.Denominator), numerator.Fragment.Numerator);
                        aFraction = new Fraction(aDividend, environment.GetNumber(1));
                    }
                    else
                    {
                        aFraction = new Fraction(numerator.Environment, numerator.Whole, environment.GetNumber(1).Whole);
                    }

                    var bFraction = default(Fraction);
                    if (denominator.Fragment != default)
                    {
                        Number bDividend = Operator.Add(Operator.Multiply(denominator, denominator.Fragment.Denominator), denominator.Fragment.Numerator);
                        bFraction = new Fraction(bDividend, environment.GetNumber(1));

                    }
                    else if (aFraction != default)
                    {
                        bFraction = new Fraction(denominator.Environment, denominator.Whole, environment.GetNumber(1).Whole);
                    }

                    Fraction fractionResult = aFraction / bFraction;
                    numerator = fractionResult.Numerator;
                    denominator = fractionResult.Denominator;
                }

                Number resultSegments = Operator.Divide(numerator, denominator);
                Whole = resultSegments.Whole;
                if (resultSegments.Fragment == default)
                {
                    Fragment = default;
                }
                else
                {
                    Fragment = new Fraction(resultSegments.Fragment.Numerator, resultSegments.Fragment.Denominator);
                }
            }
            this.Size = Whole.Length;
            this.IsNegative = false;
            this.First = Whole[^1];


            this.Even = null;
        }


        internal Number(IMathEnvironment<Number> environment, NumberSegments segments, Fraction? fragment, bool isNegative)
        {
            this.Environment = environment;

#if DEBUG
            foreach (decimal segment in segments)
            {
                if (segment > this.Environment.Base)
                {
                    throw new Exception("Bad number segment larger than base");
                }
                else if (segment < 0)
                {
                    throw new Exception("Bad number segment less than zero");
                }
            }
#endif

            this.IsNegative = isNegative;

            this.Whole = segments;

            this.Fragment = fragment;

            this.Size = segments.Length;

            this.First = this.Whole[Whole.Size - 1];
            if (First == 0 && Whole.Size > 1)
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
            return new Number(a.Environment, new NumberSegments(new decimal[] { 0 }), totalResult.Fragment, totalResult.IsNegative);
        }

        #endregion

        //public bool SaveFile(string folderName, string fileName)
        //{
        //    try
        //    {

        //        IMathEnvironment<Number> environment = Environment;
        //        string numberString = new string(Segments.Select(x => environment.Key[(int)x]).Reverse().ToArray());


        //        StorageRepository.Create(folderName, fileName, numberString);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.Write("SaveFile ERROR: " + ex.Message);
        //        return false;
        //    }
        //}

        internal Number Copy()
        {
            return new Number(this.Environment, this.Whole, this.Fragment, IsNegative);
        }

        internal Number AsNegativeNumber()
        {
            return new Number(this.Environment, this.Whole, this.Fragment, true);
        }

        internal Number AsPositiveNumber()
        {
            return new Number(this.Environment, this.Whole, this.Fragment, false);
        }

        internal Number Floor()
        {
            return new Number(this.Environment, this.Whole, IsNegative);
        }

        public static implicit operator decimal(Number d)
        {
            return d.Whole[0];
        }

        public static implicit operator long(Number d)
        {
            return (long)d.Whole[0];
        }


        public static implicit operator ulong(Number d)
        {
            return (ulong)d.Whole[0];
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Whole.GetHashCode();
                return hashCode;
            }
        }

        public override bool Equals(object other)
        {
            return Operator.Compare(this, (Number)other) == 0;
        }

        public bool Equals(Number other)
        {
            return Operator.Compare(this, other) == 0;
        }

        public int CompareTo(Number other)
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

        public string GetCharArray(IMathEnvironment<Number> environment = default)
        {
            string result = string.Empty;
            if (this.IsNegative)
            {
                result = "-" + result;
            }

            NumberSegments resultSegments;
            if (environment != default(IMathEnvironment<Number>) && environment != Environment)
            {
                resultSegments = Convert(environment).Whole;
            }
            else
            {
                environment = Environment;
                resultSegments = Whole;
            }

            result += string.Concat(resultSegments.Select(x => environment.Key[(int)x]).Reverse());

            if (this.Fragment != null)
            {
                result = string.Format("{0} {1}", result, Fragment);
            }

            return result;
        }

        public string GetDecimalArray(IMathEnvironment<Number> environment)
        {
            string result = string.Empty;
            if (this.IsNegative)
            {
                result = "-" + result;
            }

            NumberSegments resultSegments;
            if (environment != default(IMathEnvironment<Number>) && environment != Environment)
            {
                resultSegments = this.Convert(environment).Whole;
            }
            else
            {
                resultSegments = Whole;
            }

            result += string.Join(',', resultSegments.Reverse());

            if (this.Fragment != null)
            {
                result = string.Format("{0} {1}", result, Fragment);
            }

            return result;
        }
        public string GetDisplayValue()
        {
            string result = string.Join("", Environment.ConvertToString(Whole));

            if (this.IsNegative)
            {
                result = "-" + result;
            }

            if (this.Fragment != null)
            {
                result = string.Format("{0} {1}", result, Fragment);
            }

            return result;
        }

        public string ToString(IMathEnvironment<Number> environment)
        {
            return GetDecimalArray(environment);
        }

        public override string ToString()
        {
            return GetDisplayValue();
        }

        public Number ConvertToBase10()
        {
            return Operator.ConvertToBase10(this);
        }

        public void Dispose()
        {
            this.Fragment = default;

            this.First = 0;

            this.IsNegative = false;

            this.Environment = default;

            this.Whole.Dispose();
        }

        public static Number Reduce(Number number)
        { 
            if (number.Fragment == null)
            {
                return number;
            }
            else
            {
                Number numberWithFractionUnder1;
                if (number.Fragment.Numerator > number.Fragment.Denominator)
                {
                    Number fractionAsWholeNumber = number.Fragment.Numerator / number.Fragment.Denominator;
                    numberWithFractionUnder1 = fractionAsWholeNumber + number.Environment.GetNumber(number.Whole, number.IsNegative);
                }
                else
                {
                    numberWithFractionUnder1 = number;
                }

                Number fractionDivided = numberWithFractionUnder1.Fragment!.Denominator / numberWithFractionUnder1.Fragment!.Numerator;
                if (fractionDivided.Fragment == null)
                {
                    numberWithFractionUnder1.Fragment = new Fraction(number.Environment.First, fractionDivided);

                }
                //else
                //{
                //    while (numberWithFractionUnder1.Fragment!.Numerator.IsEven() && numberWithFractionUnder1.Fragment!.Denominator.IsEven())
                //    {
                //        numberWithFractionUnder1.Fragment = new Fraction((numberWithFractionUnder1.Fragment.Numerator / number.Environment.Secound), (numberWithFractionUnder1.Fragment.Denominator / number.Environment.Secound));
                //    }
                //}
                return numberWithFractionUnder1;
            }
        }
    }
}
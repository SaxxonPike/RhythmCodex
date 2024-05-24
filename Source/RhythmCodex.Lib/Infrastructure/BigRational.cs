//   Copyright (c) Microsoft Corporation.  All rights reserved.
/*============================================================
** Class: BigRational
**
** Purpose: 
** --------
** This class is used to represent an arbitrary precision
** BigRational number
**
** A rational number (commonly called a fraction) is a ratio
** between two integers.  For example (3/6) = (2/4) = (1/2)
**
** Arithmetic
** ----------
** a/b = c/d, iff ad = bc
** a/b + c/d  == (ad + bc)/bd
** a/b - c/d  == (ad - bc)/bd
** a/b % c/d  == (ad % bc)/bd
** a/b * c/d  == (ac)/(bd)
** a/b / c/d  == (ad)/(bc)
** -(a/b)     == (-a)/b
** (a/b)^(-1) == b/a, if a != 0
**
** Reduction Algorithm
** ------------------------
** Euclid's algorithm is used to simplify the fraction.
** Calculating the greatest common divisor of two n-digit
** numbers can be found in
**
** O(n(log n)^5 (log log n)) steps as n -> +infinity
============================================================*/

using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace RhythmCodex.Infrastructure;

/// <summary>
///     Represents an arbitrary precision rational number.
/// </summary>
[ComVisible(false)]
public struct BigRational : IComparable, IComparable<BigRational>, IEquatable<BigRational>
{
    // ---- SECTION:  members for internal support ---------*

    #region Members for Internal Support

    [StructLayout(LayoutKind.Explicit)]
    private struct DoubleUlong
    {
        [FieldOffset(0)] public double dbl;
        [FieldOffset(0)] public ulong uu;
    }

    private const int DoubleMaxScale = 308;
    private static readonly BigInteger DoublePrecision = BigInteger.Pow(10, DoubleMaxScale);
    private static readonly BigInteger DoubleMaxValue = (BigInteger) double.MaxValue;
    private static readonly BigInteger DoubleMinValue = (BigInteger) double.MinValue;

    [StructLayout(LayoutKind.Explicit)]
    private struct DecimalUInt32
    {
        [FieldOffset(0)] public decimal dec;
        [FieldOffset(0)] public int flags;
    }

    private const int DecimalScaleMask = 0x00FF0000;
    private const int DecimalSignMask = unchecked((int) 0x80000000);
    private const int DecimalMaxScale = 28;
    private static readonly BigInteger DecimalPrecision = BigInteger.Pow(10, DecimalMaxScale);
    private static readonly BigInteger DecimalMaxValue = (BigInteger) decimal.MaxValue;
    private static readonly BigInteger DecimalMinValue = (BigInteger) decimal.MinValue;

    private const string Solidus = @"/";

    #endregion Members for Internal Support

    // ---- SECTION: public properties --------------*

    #region Public Properties

    /// <summary>
    ///     A pre-initialized BigRational with the value of zero.
    /// </summary>
    public static BigRational Zero { get; } = new(BigInteger.Zero);

    /// <summary>
    ///     A pre-initialized BigRational with the value of one.
    /// </summary>
    public static BigRational One { get; } = new(BigInteger.One);

    /// <summary>
    ///     A pre-initialized BigRational with the value of negative one.
    /// </summary>
    public static BigRational MinusOne { get; } = new(BigInteger.MinusOne);
        
    /// <summary>
    ///     A pre-initialized BigRational with the value of one half.
    /// </summary>
    public static BigRational OneHalf { get; } = new(BigInteger.One, 2);

    /// <summary>
    ///     Gets a number that indicates the sign (negative, positive, or zero) of the current BigRational object.
    /// </summary>
    public int Sign => Numerator.Sign;

    /// <summary>
    ///     Gets the numerator of the BigRational.
    /// </summary>
    public BigInteger Numerator { get; }

    /// <summary>
    ///     Gets the denominator of the BigRational.
    /// </summary>
    public BigInteger Denominator { get; }

    #endregion Public Properties

    // ---- SECTION: public instance methods --------------*

    #region Public Instance Methods

    // GetWholePart() and GetFractionPart()
    // 
    // BigRational == Whole, Fraction
    //  0/2        ==     0,  0/2
    //  1/2        ==     0,  1/2
    // -1/2        ==     0, -1/2
    //  1/1        ==     1,  0/1
    // -1/1        ==    -1,  0/1
    // -3/2        ==    -1, -1/2
    //  3/2        ==     1,  1/2

    /// <summary>
    ///     Gets the integer value.
    /// </summary>
    public BigInteger GetWholePart()
    {
        return BigInteger.Divide(Numerator, Denominator);
    }

    /// <summary>
    ///     Gets the fractional value.
    /// </summary>
    public BigRational GetFractionPart()
    {
        return new BigRational(BigInteger.Remainder(Numerator, Denominator), Denominator);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is BigRational br && Equals(br);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return unchecked(Numerator.GetHashCode() * 17 + Denominator.GetHashCode());
    }

    /// <inheritdoc />
    int IComparable.CompareTo(object? obj)
    {
        if (obj == null)
            return 1;

        if (obj is not BigRational br)
            throw new ArgumentException("Argument must be of type BigRational", nameof(obj));

        return Compare(this, br);
    }

    /// <inheritdoc />
    public int CompareTo(BigRational other)
    {
        return Compare(this, other);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Numerator:R}{Solidus}{Denominator:R}";
    }

    /// <inheritdoc />
    public bool Equals(BigRational other)
    {
        // a/b = c/d, iff ad = bc
        return Denominator == other.Denominator
            ? Numerator == other.Numerator
            : Numerator * other.Denominator == Denominator * other.Numerator;
    }

    #endregion Public Instance Methods

    // -------- SECTION: constructors -----------------*

    #region Constructors

    /// <summary>
    ///     Create a BigRational with the specified whole value.
    /// </summary>
    public BigRational(BigInteger numerator)
    {
        Numerator = numerator;
        Denominator = BigInteger.One;
    }

    /// <summary>
    ///     Create a BigRational with the specified floating point value.
    /// </summary>
    public BigRational(double value)
    {
        Numerator = BigInteger.Zero;
        Denominator = BigInteger.One;
            
        if (double.IsPositiveInfinity(value))
        {
            Numerator = BigInteger.One;
            Denominator = BigInteger.Zero;
        }
        else if (double.IsNegativeInfinity(value))
        {
            Numerator = BigInteger.MinusOne;
            Denominator = BigInteger.Zero;
        }
        else
        {
            var ratio = Init((decimal) value);
            Numerator = ratio.Item1;
            Denominator = ratio.Item2;
        }
    }

    /// <summary>
    ///     Create a BigRational with the specified decimal value.
    /// </summary>
    public BigRational(decimal value)
    {
        Numerator = BigInteger.Zero;
        Denominator = BigInteger.One;
            
        var ratio = Init(value);
        Numerator = ratio.Item1;
        Denominator = ratio.Item2;
    }

    private (BigInteger, BigInteger) Init(decimal value)
    {
        var bits = decimal.GetBits(value);
        if (bits == null || bits.Length != 4 || (bits[3] & ~(DecimalSignMask | DecimalScaleMask)) != 0 ||
            (bits[3] & DecimalScaleMask) > 28 << 16)
            throw new ArgumentException("Invalid Decimal", nameof(value));

        if (value == decimal.Zero)
        {
            return (BigInteger.Zero, BigInteger.One);
        }

        // build up the numerator
        var ul = ((ulong) (uint) bits[2] << 32) | (uint) bits[1]; // (hi    << 32) | (mid)
        var numerator = (new BigInteger(ul) << 32) | (uint) bits[0]; // (hiMid << 32) | (low)

        var isNegative = (bits[3] & DecimalSignMask) != 0;
        if (isNegative)
            numerator = BigInteger.Negate(Numerator);

        // build up the denominator
        var scale = (bits[3] & DecimalScaleMask) >> 16; // 0-28, power of 10 to divide numerator by
        var denominator = BigInteger.Pow(10, scale);

        var simplified = Simplify(numerator, denominator);
        (numerator, denominator) = simplified;
        return (numerator, denominator);
    }

    /// <summary>
    ///     Create a BigRational with the specified numerator and denominator.
    /// </summary>
    /// <exception cref="DivideByZeroException">Thrown when the denominator is zero.</exception>
    public BigRational(BigInteger numerator, BigInteger denominator, bool allowZeroDenominator = false)
    {
        if (!allowZeroDenominator && denominator.Sign == 0)
            throw new DivideByZeroException();

        if (numerator.Sign == 0)
        {
            // 0/m -> 0/1
            Numerator = BigInteger.Zero;
            Denominator = BigInteger.One;
        }
        else if (denominator.Sign < 0)
        {
            Numerator = BigInteger.Negate(numerator);
            Denominator = BigInteger.Negate(denominator);
        }
        else
        {
            Numerator = numerator;
            Denominator = denominator;
        }

        if (denominator.Sign != 0)
        {
            var simplified = Simplify(Numerator, Denominator);
            (Numerator, Denominator) = simplified;
        }
    }

    /// <summary>
    ///     Create a BigRational with the specified whole and fractional parts.
    /// </summary>
    /// <exception cref="DivideByZeroException">Thrown when the denominator is zero.</exception>
    public BigRational(BigInteger whole, BigInteger numerator, BigInteger denominator)
    {
        if (denominator.Sign == 0)
            throw new DivideByZeroException();

        if (numerator.Sign == 0 && whole.Sign == 0)
        {
            Numerator = BigInteger.Zero;
            Denominator = BigInteger.One;
        }
        else if (denominator.Sign < 0)
        {
            Denominator = BigInteger.Negate(denominator);
            Numerator = BigInteger.Negate(whole) * Denominator + BigInteger.Negate(numerator);
        }
        else
        {
            Denominator = denominator;
            Numerator = whole * denominator + numerator;
        }

        var simplified = Simplify(Numerator, Denominator);
        (Numerator, Denominator) = simplified;
    }

    #endregion Constructors

    // -------- SECTION: public static methods -----------------*

    #region Public Static Methods

    /// <summary>
    ///     Gets the absolute value of a BigRational object.
    /// </summary>
    public static BigRational Abs(BigRational r)
    {
        return r.Numerator.Sign < 0
            ? new BigRational(BigInteger.Abs(r.Numerator), r.Denominator)
            : r;
    }

    /// <summary>
    ///     Negates a specified BigRational value.
    /// </summary>
    public static BigRational Negate(BigRational r)
    {
        return new BigRational(BigInteger.Negate(r.Numerator), r.Denominator);
    }

    /// <summary>
    ///     Reciprocates the BigRational value.
    /// </summary>
    public static BigRational Invert(BigRational r)
    {
        return new BigRational(r.Denominator, r.Numerator);
    }

    /// <summary>
    ///     Adds two BigRational values.
    /// </summary>
    public static BigRational Add(BigRational x, BigRational y)
    {
        return x + y;
    }

    /// <summary>
    ///     Subtracts two BigRational values.
    /// </summary>
    public static BigRational Subtract(BigRational x, BigRational y)
    {
        return x - y;
    }

    /// <summary>
    ///     Multiplies two BigRational values.
    /// </summary>
    public static BigRational Multiply(BigRational x, BigRational y)
    {
        return x * y;
    }

    /// <summary>
    ///     Divides two BigRational values.
    /// </summary>
    public static BigRational Divide(BigRational dividend, BigRational divisor)
    {
        return dividend / divisor;
    }

    /// <summary>
    ///     Divides two BigRational values and returns the remainder.
    /// </summary>
    public static BigRational Remainder(BigRational dividend, BigRational divisor)
    {
        return dividend % divisor;
    }

    /// <summary>
    ///     Returns the greater of two BigRational values.
    /// </summary>
    public static BigRational Max(BigRational left, BigRational right)
    {
        return left > right ? left : right;
    }

    /// <summary>
    ///     Returns the lesser of two BigRational values.
    /// </summary>
    public static BigRational Min(BigRational left, BigRational right)
    {
        return left < right ? left : right;
    }

    /// <summary>
    ///     Returns e raised to the specified power.
    /// </summary>
    public static BigRational Exp(BigRational val)
    {
        var e = (double) val;
        if (e > 700)
            e = 700;

        return Math.Exp(e);
    }

    /// <summary>
    ///     Returns the sine of the specified angle.
    /// </summary>
    public static BigRational Sin(BigRational val)
    {
        var e = (double) val;
        return Math.Sin(e);
    }

    /// <summary>
    ///     Returns the cosine of the specified angle.
    /// </summary>
    public static BigRational Cos(BigRational val)
    {
        var e = (double) val;
        return Math.Cos(e);
    }

    /// <summary>
    ///     Returns true if the BigRational is equal to NaN.
    /// </summary>
    public static bool IsNaN(BigRational val)
    {
        const float zero = 0;
        return val == zero / zero;
    }

    /// <summary>
    ///     Returns true if the BigRational is equal to infinity.
    /// </summary>
    public static bool IsInfinity(BigRational val)
    {
        const float zero = 0;
        return val == 1 / zero;
    }

    /// <summary>
    ///     Divide two BigRational objects, returning the result and remainder.
    /// </summary>
    public static BigRational DivRem(BigRational dividend, BigRational divisor, out BigRational remainder)
    {
        // a/b / c/d  == (ad)/(bc)
        // a/b % c/d  == (ad % bc)/bd

        // (ad) and (bc) need to be calculated for both the division and the remainder operations.
        var ad = dividend.Numerator * divisor.Denominator;
        var bc = dividend.Denominator * divisor.Numerator;
        var bd = dividend.Denominator * divisor.Denominator;

        remainder = new BigRational(ad % bc, bd);
        return new BigRational(ad, bc);
    }

    /// <summary>
    ///     Return the square root of a BigRational.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the input value is less than zero.</exception>
    public static BigRational Sqrt(BigRational baseValue)
    {
        if (baseValue < 0)
            throw new ArgumentException("Negative argument.");

        // square roots of 0 and 1 are trivial and
        // y == 0 will cause a divide-by-zero exception
        if (baseValue == 0 || baseValue == 1)
            return baseValue;

        // check for perfect squares
        if (baseValue.Denominator == 1)
        {
            var ps = Isqrt(baseValue.Numerator);
            if (ps * ps == baseValue.Numerator)
                return ps;
        }

        BigRational temp = 1;
        const int precision = 21;

        while (true)
        {
            if (temp.Numerator.ToString().Length >= precision)
                break;

            temp = (baseValue / temp + temp) / 2;
        }

        return temp;
    }

    private static BigInteger Isqrt(BigInteger x)
    {
        var b = 15; // this is the next bit we try 
        var r = BigInteger.Zero; // r will contain the result
        var r2 = BigInteger.Zero; // here we maintain r squared

        while (b >= 0)
        {
            var sr2 = r2;
            var sr = r;
            // compute (r+(1<<b))**2, we have r**2 already.
            r2 += (uint) ((r << (1 + b)) + (1 << (b + b)));
            r += (uint) (1 << b);
            if (r2 > x)
            {
                r = sr;
                r2 = sr2;
            }
            b--;
        }
        return r;
    }

    /// <summary>
    ///     Return the value of the BigRational raised to an exponent.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if the base value is zero and the exponent is negative.</exception>
    public static BigRational Pow(BigRational baseValue, BigInteger exponent)
    {
        if (exponent.Sign == 0)
            return One;

        if (exponent.Sign < 0)
        {
            if (baseValue == Zero)
                throw new ArgumentException("Cannot raise zero to a negative power.", nameof(baseValue));

            // n^(-e) -> (1/n)^e
            baseValue = Invert(baseValue);
            exponent = BigInteger.Negate(exponent);
        }

        var result = baseValue;
        while (exponent > BigInteger.One)
        {
            result *= baseValue;
            exponent--;
        }

        return result;
    }

    // Least Common Denominator (LCD)
    //
    // The LCD is the least common multiple of the two denominators.  For instance, the LCD of
    // {1/2, 1/4} is 4 because the least common multiple of 2 and 4 is 4.  Likewise, the LCD
    // of {1/2, 1/3} is 6.
    //       
    // To find the LCD:
    //
    // 1) Find the Greatest Common Divisor (GCD) of the denominators
    // 2) Multiply the denominators together
    // 3) Divide the product of the denominators by the GCD

    /// <summary>
    ///     Returns the least common denominator of two BigRational values.
    /// </summary>
    public static BigInteger LeastCommonDenominator(BigRational x, BigRational y)
    {
        // LCD( a/b, c/d ) == (bd) / gcd(b,d)
        return x.Denominator * y.Denominator / BigInteger.GreatestCommonDivisor(x.Denominator, y.Denominator);
    }

    /// <summary>
    ///     Compares two BigRational values.
    /// </summary>
    public static int Compare(BigRational r1, BigRational r2)
    {
        //     a/b = c/d, iff ad = bc
        return BigInteger.Compare(r1.Numerator * r2.Denominator, r2.Numerator * r1.Denominator);
    }

    #endregion Public Static Methods

    #region Operator Overloads

    public static bool operator ==(BigRational x, BigRational y)
    {
        return Compare(x, y) == 0;
    }

    public static bool operator !=(BigRational x, BigRational y)
    {
        return Compare(x, y) != 0;
    }

    public static bool operator <(BigRational x, BigRational y)
    {
        return Compare(x, y) < 0;
    }

    public static bool operator <=(BigRational x, BigRational y)
    {
        return Compare(x, y) <= 0;
    }

    public static bool operator >(BigRational x, BigRational y)
    {
        return Compare(x, y) > 0;
    }

    public static bool operator >=(BigRational x, BigRational y)
    {
        return Compare(x, y) >= 0;
    }

    public static BigRational operator +(BigRational r)
    {
        return r;
    }

    public static BigRational operator -(BigRational r)
    {
        return new BigRational(-r.Numerator, r.Denominator);
    }

    public static BigRational operator ++(BigRational r)
    {
        return r + One;
    }

    public static BigRational operator --(BigRational r)
    {
        return r - One;
    }

    // a/b + c/d  == (ad + bc)/bd
    public static BigRational operator +(BigRational r1, BigRational r2)
    {
        return new BigRational(r1.Numerator * r2.Denominator + r1.Denominator * r2.Numerator,
            r1.Denominator * r2.Denominator);
    }

    // a/b - c/d  == (ad - bc)/bd
    public static BigRational operator -(BigRational r1, BigRational r2)
    {
        return new BigRational(r1.Numerator * r2.Denominator - r1.Denominator * r2.Numerator,
            r1.Denominator * r2.Denominator);
    }

    // a/b * c/d  == (ac)/(bd)
    public static BigRational operator *(BigRational r1, BigRational r2)
    {
        return new BigRational(r1.Numerator * r2.Numerator, r1.Denominator * r2.Denominator);
    }

    // a/b / c/d  == (ad)/(bc)
    public static BigRational operator /(BigRational r1, BigRational r2)
    {
        return new BigRational(r1.Numerator * r2.Denominator, r1.Denominator * r2.Numerator);
    }

    // a/b % c/d  == (ad % bc)/bd
    public static BigRational operator %(BigRational r1, BigRational r2)
    {
        return new BigRational(r1.Numerator * r2.Denominator % (r1.Denominator * r2.Numerator),
            r1.Denominator * r2.Denominator);
    }
        
    public static BigRational PositiveInfinity => new(BigInteger.One, BigInteger.Zero, true);

    #endregion Operator Overloads

    // ----- SECTION: explicit conversions from BigRational to numeric base types  ----------------*

    #region explicit conversions from BigRational

    public static explicit operator sbyte(BigRational value)
    {
        return (sbyte) BigInteger.Divide(value.Numerator, value.Denominator);
    }

    public static explicit operator ushort(BigRational value)
    {
        return (ushort) BigInteger.Divide(value.Numerator, value.Denominator);
    }

    public static explicit operator uint(BigRational value)
    {
        return (uint) BigInteger.Divide(value.Numerator, value.Denominator);
    }

    public static explicit operator ulong(BigRational value)
    {
        return (ulong) BigInteger.Divide(value.Numerator, value.Denominator);
    }

    public static explicit operator byte(BigRational value)
    {
        return (byte) BigInteger.Divide(value.Numerator, value.Denominator);
    }

    public static explicit operator short(BigRational value)
    {
        return (short) BigInteger.Divide(value.Numerator, value.Denominator);
    }

    public static explicit operator int(BigRational value)
    {
        return (int) BigInteger.Divide(value.Numerator, value.Denominator);
    }

    public static explicit operator long(BigRational value)
    {
        return (long) BigInteger.Divide(value.Numerator, value.Denominator);
    }

    public static explicit operator BigInteger(BigRational value)
    {
        return BigInteger.Divide(value.Numerator, value.Denominator);
    }

    // The Single value type represents a single-precision 32-bit number with
    // values ranging from negative 3.402823e38 to positive 3.402823e38      
    // values that do not fit into this range are returned as Infinity
    public static explicit operator float(BigRational value)
    {
        return (float) (double) value;
    }

    // The Double value type represents a double-precision 64-bit number with
    // values ranging from -1.79769313486232e308 to +1.79769313486232e308
    // values that do not fit into this range are returned as +/-Infinity
    public static explicit operator double(BigRational value)
    {
        if (IsInfinity(value))
            return value.Sign == 1 ? double.PositiveInfinity : double.NegativeInfinity;
            
        if (SafeCastToDouble(value.Numerator) && SafeCastToDouble(value.Denominator))
            return (double) value.Numerator / (double) value.Denominator;

        // scale the numerator to preserve the fraction part through the integer division
        var denormalized = value.Numerator * DoublePrecision / value.Denominator;
        if (denormalized.IsZero)
            return value.Sign < 0
                ? BitConverter.Int64BitsToDouble(unchecked((long) 0x8000000000000000))
                : 0d; // underflow to -+0

        double result = 0;
        var isDouble = false;
        var scale = DoubleMaxScale;

        while (scale > 0)
        {
            if (!isDouble)
                if (SafeCastToDouble(denormalized))
                {
                    result = (double) denormalized;
                    isDouble = true;
                }
                else
                {
                    denormalized /= 10;
                }
            result /= 10;
            scale--;
        }

        return !isDouble
            ? (value.Sign < 0 ? double.NegativeInfinity : double.PositiveInfinity)
            : result;
    }

    // The Decimal value type represents decimal numbers ranging
    // from +79,228,162,514,264,337,593,543,950,335 to -79,228,162,514,264,337,593,543,950,335
    // the binary representation of a Decimal value is of the form, ((-2^96 to 2^96) / 10^(0 to 28))
    public static explicit operator decimal(BigRational value)
    {
        if (SafeCastToDecimal(value.Numerator) && SafeCastToDecimal(value.Denominator))
            return (decimal) value.Numerator / (decimal) value.Denominator;

        // scale the numerator to preserve the fraction part through the integer division
        var denormalized = value.Numerator * DecimalPrecision / value.Denominator;
        if (denormalized.IsZero)
            return decimal.Zero; // underflow - fraction is too small to fit in a decimal

        for (var scale = DecimalMaxScale; scale >= 0; scale--)
            if (!SafeCastToDecimal(denormalized))
            {
                denormalized /= 10;
            }
            else
            {
                var dec = new DecimalUInt32 {dec = (decimal) denormalized};
                dec.flags = (dec.flags & ~DecimalScaleMask) | (scale << 16);
                return dec.dec;
            }
        throw new OverflowException("Value was either too large or too small for a Decimal.");
    }

    #endregion explicit conversions from BigRational

    // ----- SECTION: implicit conversions from numeric base types to BigRational  ----------------*

    #region implicit conversions to BigRational

    public static implicit operator BigRational(sbyte value)
    {
        return new BigRational((BigInteger) value);
    }

    public static implicit operator BigRational(ushort value)
    {
        return new BigRational((BigInteger) value);
    }

    public static implicit operator BigRational(uint value)
    {
        return new BigRational((BigInteger) value);
    }

    public static implicit operator BigRational(ulong value)
    {
        return new BigRational((BigInteger) value);
    }

    public static implicit operator BigRational(byte value)
    {
        return new BigRational((BigInteger) value);
    }

    public static implicit operator BigRational(short value)
    {
        return new BigRational((BigInteger) value);
    }

    public static implicit operator BigRational(int value)
    {
        return new BigRational((BigInteger) value);
    }

    public static implicit operator BigRational(long value)
    {
        return new BigRational((BigInteger) value);
    }

    public static implicit operator BigRational(BigInteger value)
    {
        return new BigRational(value);
    }

    public static implicit operator BigRational(float value)
    {
        return new BigRational(value);
    }

    public static implicit operator BigRational(double value)
    {
        return new BigRational(value);
    }

    public static implicit operator BigRational(decimal value)
    {
        return new BigRational(value);
    }

    #endregion implicit conversions to BigRational

    // ----- SECTION: private instance utility methods ----------------*

    #region instance helper methods

    private static (BigInteger, BigInteger) Simplify(BigInteger num, BigInteger den)
    {
        // * if the numerator is {0, +1, -1} then the fraction is already reduced
        // * if the denominator is {+1} then the fraction is already reduced
        if (num == BigInteger.Zero)
            den = BigInteger.One;

        var gcd = BigInteger.GreatestCommonDivisor(num, den);
        if (gcd > BigInteger.One)
        {
            num /= gcd;
            den /= gcd;
        }

        return (num, den);
    }

    #endregion instance helper methods

    // ----- SECTION: private static utility methods -----------------*

    #region static helper methods

    private static bool SafeCastToDouble(BigInteger value)
    {
        return DoubleMinValue <= value && value <= DoubleMaxValue;
    }

    private static bool SafeCastToDecimal(BigInteger value)
    {
        return DecimalMinValue <= value && value <= DecimalMaxValue;
    }

    private static void SplitDoubleIntoParts(double dbl, out int sign, out int exp, out ulong man,
        out bool isFinite)
    {
        DoubleUlong du;
        du.uu = 0;
        du.dbl = dbl;

        sign = 1 - ((int) (du.uu >> 62) & 2);
        man = du.uu & 0x000FFFFFFFFFFFFF;
        exp = (int) (du.uu >> 52) & 0x7FF;

        switch (exp)
        {
            case 0:
                // Denormalized number.
                isFinite = true;
                if (man != 0)
                    exp = -1074;
                break;
            case 0x7FF:
                // NaN or Infinite.
                isFinite = false;
                exp = int.MaxValue;
                break;
            default:
                isFinite = true;
                man |= 0x0010000000000000; // mask in the implied leading 53rd significand bit
                exp -= 1075;
                break;
        }
    }

    #endregion static helper methods
}
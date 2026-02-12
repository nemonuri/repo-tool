# Modulo 연산의 여러가지 정의

- 정수의 몫과 나머지를 구하는 연산의 여러 정의들.

## [Modulo - Variants_of_the_definition](https://en.wikipedia.org/wiki/Modulo#Variants_of_the_definition)

### Truncated division

- 가장 일반적인 구현 방법
- [Rounding toward zero] 를 사용해 몫을 구함

### Floored division

- [Rounding down] 을 사용해 몫을 구함

### Ceiling division

- [Rounding up] 을 사용해 몫을 구함

### Euclidean division

- 나머지가 음수가 되지 않도록 몫을 구함
- [Euclidean division] 알고리즘 사용

## .NET 공식 구현 살펴보기

- 마침 .NET 11 공식 API에, [DivisionRounding] 기능이 추가되었다!
  - 어떻게 구현했나 볼까!

### [System/Numerics/DivisionRounding.cs](https://github.com/dotnet/runtime/blob/v11.0.0-preview.1.26104.118/src/libraries/System.Private.CoreLib/src/System/Numerics/DivisionRounding.cs)

```cs
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Numerics
{
    /// <summary>
    /// Specifies the rounding strategy to use when performing division.
    /// </summary>
    public enum DivisionRounding
    {
        /// <summary>
        /// Truncated division (rounding toward zero) — round the division result towards zero.
        /// </summary>
        Truncate = 0,
        // Graph for truncated division with positive divisor https://www.wolframalpha.com/input?i=Plot%5B%7BIntegerPart%5Bn%5D%2C+n+-+IntegerPart%5Bn%5D%7D%2C+%7Bn%2C+-3%2C+3%7D%5D
        // Graph for truncated division with negative divisor https://www.wolframalpha.com/input?i=Plot%5B%7BIntegerPart%5B-n%5D%2C+n+%2B+IntegerPart%5B-n%5D%7D%2C+%7Bn%2C+-3%2C+3%7D%5D

        /// <summary>
        /// Floor division (rounding down) — round the division result down to the next lower integer.
        /// </summary>
        Floor = 1,
        // Graph for floor division with positive divisor https://www.wolframalpha.com/input?i=Plot%5B%7BFloor%5Bn%5D%2C+n+-+Floor%5Bn%5D%7D%2C+%7Bn%2C+-3%2C+3%7D%5D
        // Graph for floor division with negative divisor https://www.wolframalpha.com/input?i=Plot%5B%7BFloor%5B-n%5D%2C+n+%2B+Floor%5B-n%5D%7D%2C+%7Bn%2C+-3%2C+3%7D%5D

        /// <summary>
        /// Ceiling division (rounding up) — round the division result up to the next higher integer.
        /// </summary>
        Ceiling = 2,
        // Graph for ceiling division with positive divisor https://www.wolframalpha.com/input?i=Plot%5B%7BCeiling%5Bn%5D%2C+n+-+Ceiling%5Bn%5D%7D%2C+%7Bn%2C+-3%2C+3%7D%5D
        // Graph for ceiling division with negative divisor https://www.wolframalpha.com/input?i=Plot%5B%7BCeiling%5B-n%5D%2C+n+%2B+Ceiling%5B-n%5D%7D%2C+%7Bn%2C+-3%2C+3%7D%5D

        /// <summary>
        /// AwayFromZero division (rounding away zero) — round the division result away from zero to the nearest integer.
        /// </summary>
        AwayFromZero = 3,
        // Graph for AwayFromZero division with positive divisor https://www.wolframalpha.com/input?i=Plot%5B%7BIntegerPart%5Bn%5D+%2B+Sign%5Bn%5D%2C+n+-+IntegerPart%5Bn%5D+-+Sign%5Bn%5D%7D%2C+%7Bn%2C+-3%2C+3%7D%5D
        // Graph for AwayFromZero division with negative divisor https://www.wolframalpha.com/input?i=Plot%5B%7BIntegerPart%5B-n%5D+%2B+Sign%5B-n%5D%2C+n+%2B+IntegerPart%5B-n%5D+%2B+Sign%5B-n%5D%7D%2C+%7Bn%2C+-3%2C+3%7D%5D

        /// <summary>
        /// Euclidean division ensures a non-negative remainder:
        ///   for positive divisor — round the division result down to the next lower integer (rounding down);
        ///   for negative divisor — round the division result up to the next higher integer  (rounding up);
        /// </summary>
        Euclidean = 4,
        // Graph for Euclidean division with positive divisor https://www.wolframalpha.com/input?i=Plot%5B%7BFloor%5Bn%5D%2C+n+-+Floor%5Bn%5D%7D%2C+%7Bn%2C+-3%2C+3%7D%5D
        // Graph for Euclidean division with negative divisor https://www.wolframalpha.com/input?i=Plot%5B%7BCeiling%5B-n%5D%2C+n+%2B+Ceiling%5B-n%5D%7D%2C+%7Bn%2C+-3%2C+3%7D%5D
    }
}
```

### [System/Numerics/IBinaryInteger.cs](https://github.com/dotnet/runtime/blob/v11.0.0-preview.1.26104.118/src/libraries/System.Private.CoreLib/src/System/Numerics/IBinaryInteger.cs)

#### Truncated division

```cs
        /// <summary>Computes the quotient and remainder of two values.</summary>
        /// <param name="left">The value which <paramref name="right" /> divides.</param>
        /// <param name="right">The value which divides <paramref name="left" />.</param>
        /// <returns>The quotient and remainder of <paramref name="left" /> divided-by <paramref name="right" />.</returns>
        static virtual (TSelf Quotient, TSelf Remainder) DivRem(TSelf left, TSelf right)
        {
            TSelf quotient = left / right;
            return (quotient, (left - (quotient * right)));
        }
```

#### DivisionRounding 에 따른 변형

```cs
        /// <summary>Computes the quotient and remainder of two values using the specified division rounding mode.</summary>
        /// <param name="left">The value which <paramref name="right" /> divides.</param>
        /// <param name="right">The value which divides <paramref name="left" />.</param>
        /// <param name="mode">The <see cref="DivisionRounding"/> mode.</param>
        /// <returns>The quotient and remainder of <paramref name="left" /> divided-by <paramref name="right" /> with the specified division rounding mode.</returns>
        static virtual (TSelf Quotient, TSelf Remainder) DivRem(TSelf left, TSelf right, DivisionRounding mode)
        {
            (TSelf quotient, TSelf remainder) = TSelf.DivRem(left, right);

            if (TSelf.IsZero(remainder))
            {
                return (quotient, remainder);
            }

            switch (mode)
            {
                case DivisionRounding.Truncate:
                {
                    break;
                }

                case DivisionRounding.Floor:
                {
                    if (TSelf.IsPositive(left) != TSelf.IsPositive(right))
                    {
                        quotient--;
                        remainder += right;
                    }
                    break;
                }

                case DivisionRounding.Ceiling:
                {
                    if (TSelf.IsPositive(left) == TSelf.IsPositive(right))
                    {
                        quotient++;
                        remainder -= right;
                    }
                    break;
                }

                case DivisionRounding.AwayFromZero:
                {
                    if (TSelf.IsPositive(left) != TSelf.IsPositive(right))
                    {
                        quotient--;
                        remainder += right;
                    }
                    else
                    {
                        quotient++;
                        remainder -= right;
                    }
                    break;
                }

                case DivisionRounding.Euclidean:
                {
                    if (TSelf.IsNegative(left))
                    {
                        if (TSelf.IsPositive(right))
                        {
                            quotient--;
                            remainder += right;
                        }
                        else
                        {
                            quotient++;
                            remainder -= right;
                        }
                    }
                    break;
                }

                default:
                {
                    ThrowHelper.ThrowArgumentException_InvalidEnumValue(mode);
                    break;
                }
            }

            return (quotient, remainder);
        }
```

역시, 엄청 깔끔하게 구현했네!

[Rounding toward zero]: https://en.wikipedia.org/wiki/Rounding#Rounding_toward_zero
[Rounding down]: https://en.wikipedia.org/wiki/Rounding#Rounding_down
[Rounding up]: https://en.wikipedia.org/wiki/Rounding#Rounding_up
[Euclidean division]: https://en.wikipedia.org/wiki/Euclidean_division
[DivisionRounding]: https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview1/libraries.md#divisionrounding-for-integer-division-modes
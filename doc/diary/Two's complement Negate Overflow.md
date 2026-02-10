# Two's complement Negate Overflow

## 들어가기

- [System.SByte] 는 2의 보수로 표현된 부호 있는 정수 타입이다.
  - 이 타입의 [MinValue](System.SByte.MinValue) 는 `-128`, [MaxValue](System.SByte.MaxValue) 는 `127` 이다.

- 잠깐, 그럼 `-System.SByte.MinValue` 의 값은 뭐야?
  - `+128`은 `System.SByte` 타입으로 표현할 수 없잖아?


## [Most negative number] 문제

- '부호 있는 2의 보수 관련' 각종 연산자들이, 이 **Most negative number** 에서 여러 오류를 일으킨다.

### 예시

- 대표적인 사례로, `System.SByte` 의 `MinValue` 인 `-128` 을 사용한다.
- **⟼** 은 '입력 ⟼ 출력' 관계를 나타낸다.

#### Negation

- -(-128) ⟼ -128
- [ecma-335](iii.3.50-neg.md) 에도 해당 동작이 명시되어 있다.
  - > Negation of integral values is standard twos-complement negation. \
      In particular, negating the most negative number (which does not have a positive counterpart) yields the most negative number.

#### Abs

- .NET 에서는 [Overflow](https://learn.microsoft.com/en-us/dotnet/api/system.math.abs?view=net-10.0#system-math-abs(system-sbyte)) 발생
- Java SE 7의 경우 MinValue 를 그대로 반환

#### Multiply -1

- (-128) * (-1) ⟼ -128

#### Divide by -1

- (-128) / (-1) ⟼ `System.ArithmeticException`  ([ecma-335](https://github.com/stakx/ecma-335/blob/master/docs/iii.3.31-div.md))
- (-128) % (-1) ⟼ `System.ArithmeticException`  ([ecma-335](https://github.com/stakx/ecma-335/blob/master/docs/iii.3.55-rem.md))

0으로 나누기만 조심하면 되는 줄 알았는데;;


[System.SByte]: https://learn.microsoft.com/en-us/dotnet/api/system.sbyte?view=net-10.0
[System.SByte.MinValue]: https://learn.microsoft.com/en-us/dotnet/api/system.sbyte.minvalue?view=net-10.0
[System.SByte.MaxValue]: https://learn.microsoft.com/en-us/dotnet/api/system.sbyte.maxvalue?view=net-10.0
[Most negative number]: https://en.wikipedia.org/wiki/Two%27s_complement#Most_negative_number
[iii.3.50-neg.md]: https://github.com/stakx/ecma-335/blob/master/docs/iii.3.50-neg.md
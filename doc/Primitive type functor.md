# Primitive type functor

## 무정의 용어

- **Bit field**
  - 참고: [Wikipedia - Bit field](https://en.wikipedia.org/wiki/Bit_field)

## 용어 정의

### Bit width
- 'Bit field' ⇀ (ℕ ∪ ⊥)
  - 주어진 'Bit field' 에 대해, 길이 또는 오류를 반환하는 [Deterministic](https://en.wikipedia.org/wiki/Deterministic_algorithm) [Partial Computable function](https://en.wikipedia.org/wiki/Computable_function)

### Bit array
- 𝒫('Bit field') × 'Bit width'
  - 'Bit field' 의 한 부분집합(**fs**)과, 한 'Bit width'(**w**)의 순서쌍
- ∀((fs, w) ∈ 'Bit array').∀(f ∈ fs).((w f) ∈ ℕ)
  - 'w'는 'fs'가 포함하는 모든 'Bit field'에 대해, 자연수 길이를 반환해야 한다.
    
### Bit vector
- 'Bit array' × ℕ
  - 한 'Bit array' 와, 한 자연수(**n**)의 순서쌍
- ∀(((fs, w), n) ∈ 'Bit vector').∀(f ∈ fs).((w f) = n)
  - 'w'는 'fs'가 포함하는 모든 'Bit field'에 대해, 자연수 길이 n을 반환해야 한다.
  - 대략적으로 설명하면, 'Bit array' 에 고정 길이가 주어진 타입이 'Bit vector' 이다.

## Essential primitive types for all programming language

모든 프로그래밍 언어에 필수적인 기초 타입들

### int

- (읽기, 쓰기, 덧셈, 뺄셈, 곱셈, 몫, 나머지, 같음비교, 크기비교) 연산이 평균적으로 가장 빠른 정수 타입
  - 그래서 반복문이나, 배열 인덱스, 정수 리터럴 등에서 가장 많이 사용되는 정수 타입
- 대략적으로 설명하면, 가장 기초적인 정수 타입

### bool

- (읽기, 쓰기, 같음비교) 연산이 평균적으로 가장 빠른 정수 타입
- 덧셈, 뺄셈, 곱셈, 몫, 나머지, 크기비교 연산이 필요 없다.

### char

- 다음 둘 중 하나
  1. ASCII 관련 연산이 가장 빠른 정수 타입
  2. 유니코드 관련 연산이 가장 빠른 정수 타입
- 프로그래밍 언어가 'ASCII' 또는 '유니코드' 둘 중 무엇을 기반으로 설계되었는지 잘 따져봐야 한다.

### string

- char 배열 관련 연산을, 프로그래머가 가장 사용하기 편리하고 안전한 형태로 변환한 타입
  - string 만큼은, 속도만큼 편의성 및 안정성이 중요하다.

## Primitive bit array types

- 문제는, 앞에서 소개한 기초 타입들이, 프로그래밍 언어마다 서로 다른 'w'을 가진 'Bit array' 라는 점이다.
- 따라서, 프로그래밍 언어 A와 API 및 ABI가 완전히 동일한 함수를 프로그래밍 언어 B에서 구현하는 것은 불가능하다!
- 결국, '선택'을 해야 한다.
  1. 동일한 ABI 함수만 구현하기
  2. 동일한 API 함수만 구현하기
  3. 동일한 ABI 함수 및 API 함수 모두 다 구현하기
- 그런데, 3은 잘못했다가는 'Combination Explosion'이 일어난다.
  - 따라서, 'Functor'를 잘 이용해야 한다.
    - Monad 기반이냐, 아니면 Applicator 기반이냐!

## FP abstractions
- https://fsprojects.github.io/FSharpPlus/abstractions.html
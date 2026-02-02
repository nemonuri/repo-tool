# Mocking canon fstarc

- 모든 빌드 시스템의 목적은, 컴파일러 및 링커에게 적절한 순서로 적절한 commandline 을 실행하는 것이다.
  - 내가 구현하고자 하는 것도, 일종의 빌드 시스템이다.
- 하지만, 현재 fstarc 의 commandline 에 해당하는 적절한 .NET API 가 없다.
  - 즉, 내가 직접 구현해야 한다.
- 가장 빠르고 간단하게 fstarc .NET API 를 만드는 방법은? 
  - fstarc 의 소스 코드의 일부를, .NET Assembly 로 컴파일하기!
  - 즉, **Mock FStarc** 만들기!
- [v2025.12.15](https://github.com/FStarLang/FStar/blob/v2025.12.15) 의 src/basic 을 컴파일하는 것이 목표!
# NativeAot Publish 로 생성된 실행 파일 경로 찾기

## 들어가기

- MSBuild 의 다음 타겟이, NativeBinary 를 최종 경로에 복사한다.

```xml
<!-- Microsoft.NETCore.Native.Publish.targets -->

  <Target Name="CopyNativeBinary" AfterTargets="Publish">
    <!-- replace apphost with binary we generated during native compilation -->
    <Delete Files="$(PublishDir)\$(TargetName)$(NativeBinaryExt)" />
    <Copy SourceFiles="$(NativeBinary)" DestinationFolder="$(PublishDir)" />
  </Target>
```

- 문제는, 'NativeBinary' 와 'PublishDir' 모두 MSBuild 공식 문서에 명세된 Property 가 아니라, Unstable 하다.
  - 그리고 Publish 타겟 실행 시점에서 알 수 있는 값도 아니다.
  - Stable 한 NativeAot 파일 경로 없나...?

## OutputPath

```
OutputPath

Specifies the path to the output directory, relative to the project directory, for example, bin\Debug or bin\Debug\$(Platform) in non-AnyCPU builds.
```

- 이거다! project directory 와 OutputPath 를 이용해, TFM 과 RID 가 명세된 폴더명을 구할 수 있어!
  - 다만, Configuration 이 Debug 인지, Relation 인지는 확실하게 해야겠네...
  - 수정: project directory 는 필요 없다...
- 그리고 결과물 경로는, 

```
$(OutputPath)/publish/$(AssemblyName)$(_NativeExecutableExtension)
```

- ...결국 완전히 stable 한 것은 무리!

## 참고자료

### MSBuild 공식 Property 명세 문서

- [Common MSBuild project properties](https://learn.microsoft.com/en-us/visualstudio/msbuild/common-msbuild-project-properties)
- [MSBuild reserved and well-known properties](https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild-reserved-and-well-known-properties)
- [MSBuild reference for .NET SDK projects](https://learn.microsoft.com/en-us/dotnet/core/project-sdk/msbuild-props)
- [NuGet pack and restore as MSBuild targets](https://learn.microsoft.com/en-us/nuget/reference/msbuild-targets)
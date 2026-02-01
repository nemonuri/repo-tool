(**
### Reference

- https://github.com/FStarLang/FStar/blob/master/src/basic/FStarC.Options.fsti
- https://github.com/FStarLang/FStar/blob/master/src/basic/FStarC.Options.fst
*)

namespace Nemonuri.RepoTools.FStar.BuildTasks.ProjectSnapShots

open Nemonuri.RepoTools.FStar.BuildTasks.FStarC
open System.IO

type CodeGen = Options.codegen_t

type ModuleSymbol = | ModuleSymbol of string

type NamespaceSymbol = | NamespaceSymbol of ModuleSymbol list

type SetActionKind =
| Add = 0
| Remove = 1

[<RequireQualifiedAccess>]
type ModuleQuery =
| All
| Namespace of NamespaceSymbol
| Module of ModuleSymbol

/// [+|-]( * | namespace | module)
type ModuleSymbolSetAction = {
    ActionKind: SetActionKind
    Query: ModuleQuery
}

type ModuleSelector = | ModuleSelector of ModuleSymbolSetAction list

[<RequireQualifiedAccess>]
type ExtractSettingTarget =
| Specific of target: CodeGen
| Default

type ExtractSetting = | ExtractSetting of Map<ExtractSettingTarget, ModuleSelector>

type WarnErrorSetActionKind =
| Warning = 0
| Silent = 1
| AlwaysError = 2

type ErrorCodeRange = {
    InclusiveMin: int
    InclusiveMax: int
}

type WarnErrorSetAction = {
    ActionKind: WarnErrorSetActionKind
    Range: ErrorCodeRange
}

type ExtensionKnobNameSymbol = | ExtensionKnobNameSymbol of string

type ExtensionKnobPathSymbol = | ExtensionKnobNamespaceSymbol of ExtensionKnobNameSymbol list

type ExtensionKnobSetAction = {
    ExtensionKnobPath: ExtensionKnobPathSymbol
    Value: string
}


type BuildSnapShot = {
    Files: FileInfo list
    CodeGen: CodeGen list
    CodeGenLib: NamespaceSymbol list
    ExtractSetting: ExtractSetting
    ExtractDir: DirectoryInfo option
    AlreadyCached: ModuleSelector
    CacheCheckedModules: bool

    //--- common ---
    FStarExe: FileInfo
    CacheDir: DirectoryInfo option
    WarnError: WarnErrorSetAction list
    CacheOff: bool
    Lax: bool
    Ext: ExtensionKnobSetAction list
    //---|
}

type DesignSnapShot = {
    Include: string list

    //--- common ---
    FStarExe: string
    CacheDir: string option
    WarnError: string list
    CacheOff: bool
    Lax: bool
    Ext: string list
    //---|
}

type ISnapShot =
    abstract member AbortOn: bool with get,set
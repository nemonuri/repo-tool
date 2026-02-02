// Reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/fsharp/base/FStar_Char.fs

module Nemonuri.FStarDotNet.FStar.Dyn

type dyn = obj

let mkdyn (x:'a) : dyn = box x

let undyn (d:dyn) : 'a = unbox<'a> d
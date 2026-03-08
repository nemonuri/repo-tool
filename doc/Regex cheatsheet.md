# Regex cheatsheet

## System.String literal → Prims.string literal

| From | To |
| --- | --- |
| `"[\x00-\x7F]*?"` | `(toString $&B)` |
| `\bstring\b` | `Prims.string` |

## Indent 2 → Indent 4

| From | To |
| --- | --- |
| `^(  )+` | `$&$&` |

## Format OCaml type API

| From | To |
| --- | --- |
| `\\|([\w\d_]+)[\r\n ]+?([\x00-\x7F]*?)[\r\n]+` | `\| $1  (* $2 *)\n` |

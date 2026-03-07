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

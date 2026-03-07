# Regex cheatsheet

## Convert System.String literal to Prims.string literal

| From | To |
| --- | --- |
| `"[\x00-\x7F]*?"` | `(toString $&B)` |
| `\bstring\b` | `Prims.string` |
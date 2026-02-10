/**
- Copy From: https://github.com/dotnet/runtime/blob/v10.0.2/src/libraries/System.Private.CoreLib/src/System/MemoryExtensions.Trim.cs
*/

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Nemonuri.NetStandards
{
    public static partial class MemoryTheory
    {
        /// <summary>
        /// Removes all leading and trailing occurrences of a specified element from the memory.
        /// </summary>
        /// <param name="memory">The source memory from which the element is removed.</param>
        /// <param name="trimElement">The specified element to look for and remove.</param>
        public static Memory<T> Trim<T>(Memory<T> memory, T trimElement) where T : IEquatable<T>?
        {
            ReadOnlySpan<T> span = memory.Span;
            int start = ClampStart(span, trimElement);
            int length = ClampEnd(span, start, trimElement);
            return memory.Slice(start, length);
        }

        /// <summary>
        /// Removes all leading occurrences of a specified element from the memory.
        /// </summary>
        /// <param name="memory">The source memory from which the element is removed.</param>
        /// <param name="trimElement">The specified element to look for and remove.</param>
        public static Memory<T> TrimStart<T>(Memory<T> memory, T trimElement) where T : IEquatable<T>?
            => memory.Slice(ClampStart(memory.Span, trimElement));

        /// <summary>
        /// Removes all trailing occurrences of a specified element from the memory.
        /// </summary>
        /// <param name="memory">The source memory from which the element is removed.</param>
        /// <param name="trimElement">The specified element to look for and remove.</param>
        public static Memory<T> TrimEnd<T>(Memory<T> memory, T trimElement) where T : IEquatable<T>?
            => memory.Slice(0, ClampEnd(memory.Span, 0, trimElement));

        /// <summary>
        /// Removes all leading and trailing occurrences of a specified element from the memory.
        /// </summary>
        /// <param name="memory">The source memory from which the element is removed.</param>
        /// <param name="trimElement">The specified element to look for and remove.</param>
        public static ReadOnlyMemory<T> Trim<T>(ReadOnlyMemory<T> memory, T trimElement) where T : IEquatable<T>?
        {
            ReadOnlySpan<T> span = memory.Span;
            int start = ClampStart(span, trimElement);
            int length = ClampEnd(span, start, trimElement);
            return memory.Slice(start, length);
        }

        /// <summary>
        /// Removes all leading occurrences of a specified element from the memory.
        /// </summary>
        /// <param name="memory">The source memory from which the element is removed.</param>
        /// <param name="trimElement">The specified element to look for and remove.</param>
        public static ReadOnlyMemory<T> TrimStart<T>(ReadOnlyMemory<T> memory, T trimElement) where T : IEquatable<T>?
            => memory.Slice(ClampStart(memory.Span, trimElement));

        /// <summary>
        /// Removes all trailing occurrences of a specified element from the memory.
        /// </summary>
        /// <param name="memory">The source memory from which the element is removed.</param>
        /// <param name="trimElement">The specified element to look for and remove.</param>
        public static ReadOnlyMemory<T> TrimEnd<T>(ReadOnlyMemory<T> memory, T trimElement) where T : IEquatable<T>?
            => memory.Slice(0, ClampEnd(memory.Span, 0, trimElement));

        /// <summary>
        /// Removes all leading and trailing occurrences of a specified element from the span.
        /// </summary>
        /// <param name="span">The source span from which the element is removed.</param>
        /// <param name="trimElement">The specified element to look for and remove.</param>
        public static Span<T> Trim<T>(Span<T> span, T trimElement) where T : IEquatable<T>?
        {
            int start = ClampStart(span, trimElement);
            int length = ClampEnd(span, start, trimElement);
            return span.Slice(start, length);
        }

        /// <summary>
        /// Removes all leading occurrences of a specified element from the span.
        /// </summary>
        /// <param name="span">The source span from which the element is removed.</param>
        /// <param name="trimElement">The specified element to look for and remove.</param>
        public static Span<T> TrimStart<T>(Span<T> span, T trimElement) where T : IEquatable<T>?
            => span.Slice(ClampStart(span, trimElement));

        /// <summary>
        /// Removes all trailing occurrences of a specified element from the span.
        /// </summary>
        /// <param name="span">The source span from which the element is removed.</param>
        /// <param name="trimElement">The specified element to look for and remove.</param>
        public static Span<T> TrimEnd<T>(Span<T> span, T trimElement) where T : IEquatable<T>?
            => span.Slice(0, ClampEnd(span, 0, trimElement));

        /// <summary>
        /// Removes all leading and trailing occurrences of a specified element from the span.
        /// </summary>
        /// <param name="span">The source span from which the element is removed.</param>
        /// <param name="trimElement">The specified element to look for and remove.</param>
        public static ReadOnlySpan<T> Trim<T>(ReadOnlySpan<T> span, T trimElement) where T : IEquatable<T>?
        {
            int start = ClampStart(span, trimElement);
            int length = ClampEnd(span, start, trimElement);
            return span.Slice(start, length);
        }

        /// <summary>
        /// Removes all leading occurrences of a specified element from the span.
        /// </summary>
        /// <param name="span">The source span from which the element is removed.</param>
        /// <param name="trimElement">The specified element to look for and remove.</param>
        public static ReadOnlySpan<T> TrimStart<T>(ReadOnlySpan<T> span, T trimElement) where T : IEquatable<T>?
            => span.Slice(ClampStart(span, trimElement));

        /// <summary>
        /// Removes all trailing occurrences of a specified element from the span.
        /// </summary>
        /// <param name="span">The source span from which the element is removed.</param>
        /// <param name="trimElement">The specified element to look for and remove.</param>
        public static ReadOnlySpan<T> TrimEnd<T>(ReadOnlySpan<T> span, T trimElement) where T : IEquatable<T>?
            => span.Slice(0, ClampEnd(span, 0, trimElement));

        /// <summary>
        /// Delimits all leading occurrences of a specified element from the span.
        /// </summary>
        /// <param name="span">The source span from which the element is removed.</param>
        /// <param name="trimElement">The specified element to look for and remove.</param>
        private static int ClampStart<T>(ReadOnlySpan<T> span, T trimElement) where T : IEquatable<T>?
        {
            int start = 0;

            if (trimElement != null)
            {
                for (; start < span.Length; start++)
                {
                    if (!trimElement.Equals(span[start]))
                    {
                        break;
                    }
                }
            }
            else
            {
                for (; start < span.Length; start++)
                {
                    if (span[start] != null)
                    {
                        break;
                    }
                }
            }

            return start;
        }

        /// <summary>
        /// Delimits all trailing occurrences of a specified element from the span.
        /// </summary>
        /// <param name="span">The source span from which the element is removed.</param>
        /// <param name="start">The start index from which to being searching.</param>
        /// <param name="trimElement">The specified element to look for and remove.</param>
        private static int ClampEnd<T>(ReadOnlySpan<T> span, int start, T trimElement) where T : IEquatable<T>?
        {
            // Initially, start==len==0. If ClampStart trims all, start==len
            Debug.Assert((uint)start <= span.Length);

            int end = span.Length - 1;

            if (trimElement != null)
            {
                for (; end >= start; end--)
                {
                    if (!trimElement.Equals(span[end]))
                    {
                        break;
                    }
                }
            }
            else
            {
                for (; end >= start; end--)
                {
                    if (span[end] != null)
                    {
                        break;
                    }
                }
            }

            return end - start + 1;
        }

        /// <summary>
        /// Removes all leading and trailing occurrences of a set of elements specified
        /// in a readonly span from the memory.
        /// </summary>
        /// <param name="memory">The source memory from which the elements are removed.</param>
        /// <param name="trimElements">The span which contains the set of elements to remove.</param>
        /// <remarks>If <paramref name="trimElements"/> is empty, the memory is returned unaltered.</remarks>
        public static Memory<T> Trim<T>(Memory<T> memory, ReadOnlySpan<T> trimElements) where T : IEquatable<T>?
        {
            if (trimElements.Length > 1)
            {
                ReadOnlySpan<T> span = memory.Span;
                int start = ClampStart(span, trimElements);
                int length = ClampEnd(span, start, trimElements);
                return memory.Slice(start, length);
            }

            if (trimElements.Length == 1)
            {
                return Trim(memory, trimElements[0]);
            }

            return memory;
        }

        /// <summary>
        /// Removes all leading occurrences of a set of elements specified
        /// in a readonly span from the memory.
        /// </summary>
        /// <param name="memory">The source memory from which the elements are removed.</param>
        /// <param name="trimElements">The span which contains the set of elements to remove.</param>
        /// <remarks>If <paramref name="trimElements"/> is empty, the memory is returned unaltered.</remarks>
        public static Memory<T> TrimStart<T>(Memory<T> memory, ReadOnlySpan<T> trimElements) where T : IEquatable<T>?
        {
            if (trimElements.Length > 1)
            {
                return memory.Slice(ClampStart(memory.Span, trimElements));
            }

            if (trimElements.Length == 1)
            {
                return TrimStart(memory, trimElements[0]);
            }

            return memory;
        }

        /// <summary>
        /// Removes all trailing occurrences of a set of elements specified
        /// in a readonly span from the memory.
        /// </summary>
        /// <param name="memory">The source memory from which the elements are removed.</param>
        /// <param name="trimElements">The span which contains the set of elements to remove.</param>
        /// <remarks>If <paramref name="trimElements"/> is empty, the memory is returned unaltered.</remarks>
        public static Memory<T> TrimEnd<T>(Memory<T> memory, ReadOnlySpan<T> trimElements) where T : IEquatable<T>?
        {
            if (trimElements.Length > 1)
            {
                return memory.Slice(0, ClampEnd(memory.Span, 0, trimElements));
            }

            if (trimElements.Length == 1)
            {
                return TrimEnd(memory, trimElements[0]);
            }

            return memory;
        }

        /// <summary>
        /// Removes all leading and trailing occurrences of a set of elements specified
        /// in a readonly span from the memory.
        /// </summary>
        /// <param name="memory">The source memory from which the elements are removed.</param>
        /// <param name="trimElements">The span which contains the set of elements to remove.</param>
        /// <remarks>If <paramref name="trimElements"/> is empty, the memory is returned unaltered.</remarks>
        public static ReadOnlyMemory<T> Trim<T>(ReadOnlyMemory<T> memory, ReadOnlySpan<T> trimElements) where T : IEquatable<T>?
        {
            if (trimElements.Length > 1)
            {
                ReadOnlySpan<T> span = memory.Span;
                int start = ClampStart(span, trimElements);
                int length = ClampEnd(span, start, trimElements);
                return memory.Slice(start, length);
            }

            if (trimElements.Length == 1)
            {
                return Trim(memory, trimElements[0]);
            }

            return memory;
        }

        /// <summary>
        /// Removes all leading occurrences of a set of elements specified
        /// in a readonly span from the memory.
        /// </summary>
        /// <param name="memory">The source memory from which the elements are removed.</param>
        /// <param name="trimElements">The span which contains the set of elements to remove.</param>
        /// <remarks>If <paramref name="trimElements"/> is empty, the memory is returned unaltered.</remarks>
        public static ReadOnlyMemory<T> TrimStart<T>(ReadOnlyMemory<T> memory, ReadOnlySpan<T> trimElements) where T : IEquatable<T>?
        {
            if (trimElements.Length > 1)
            {
                return memory.Slice(ClampStart(memory.Span, trimElements));
            }

            if (trimElements.Length == 1)
            {
                return TrimStart(memory, trimElements[0]);
            }

            return memory;
        }

        /// <summary>
        /// Removes all trailing occurrences of a set of elements specified
        /// in a readonly span from the memory.
        /// </summary>
        /// <param name="memory">The source memory from which the elements are removed.</param>
        /// <param name="trimElements">The span which contains the set of elements to remove.</param>
        /// <remarks>If <paramref name="trimElements"/> is empty, the memory is returned unaltered.</remarks>
        public static ReadOnlyMemory<T> TrimEnd<T>(ReadOnlyMemory<T> memory, ReadOnlySpan<T> trimElements) where T : IEquatable<T>?
        {
            if (trimElements.Length > 1)
            {
                return memory.Slice(0, ClampEnd(memory.Span, 0, trimElements));
            }

            if (trimElements.Length == 1)
            {
                return TrimEnd(memory, trimElements[0]);
            }

            return memory;
        }

        /// <summary>
        /// Removes all leading and trailing occurrences of a set of elements specified
        /// in a readonly span from the span.
        /// </summary>
        /// <param name="span">The source span from which the elements are removed.</param>
        /// <param name="trimElements">The span which contains the set of elements to remove.</param>
        /// <remarks>If <paramref name="trimElements"/> is empty, the span is returned unaltered.</remarks>
        public static Span<T> Trim<T>(Span<T> span, ReadOnlySpan<T> trimElements) where T : IEquatable<T>?
        {
            if (trimElements.Length > 1)
            {
                int start = ClampStart(span, trimElements);
                int length = ClampEnd(span, start, trimElements);
                return span.Slice(start, length);
            }

            if (trimElements.Length == 1)
            {
                return Trim(span, trimElements[0]);
            }

            return span;
        }

        /// <summary>
        /// Removes all leading occurrences of a set of elements specified
        /// in a readonly span from the span.
        /// </summary>
        /// <param name="span">The source span from which the elements are removed.</param>
        /// <param name="trimElements">The span which contains the set of elements to remove.</param>
        /// <remarks>If <paramref name="trimElements"/> is empty, the span is returned unaltered.</remarks>
        public static Span<T> TrimStart<T>(Span<T> span, ReadOnlySpan<T> trimElements) where T : IEquatable<T>?
        {
            if (trimElements.Length > 1)
            {
                return span.Slice(ClampStart(span, trimElements));
            }

            if (trimElements.Length == 1)
            {
                return TrimStart(span, trimElements[0]);
            }

            return span;
        }

        /// <summary>
        /// Removes all trailing occurrences of a set of elements specified
        /// in a readonly span from the span.
        /// </summary>
        /// <param name="span">The source span from which the elements are removed.</param>
        /// <param name="trimElements">The span which contains the set of elements to remove.</param>
        /// <remarks>If <paramref name="trimElements"/> is empty, the span is returned unaltered.</remarks>
        public static Span<T> TrimEnd<T>(Span<T> span, ReadOnlySpan<T> trimElements) where T : IEquatable<T>?
        {
            if (trimElements.Length > 1)
            {
                return span.Slice(0, ClampEnd(span, 0, trimElements));
            }

            if (trimElements.Length == 1)
            {
                return TrimEnd(span, trimElements[0]);
            }

            return span;
        }

        /// <summary>
        /// Removes all leading and trailing occurrences of a set of elements specified
        /// in a readonly span from the span.
        /// </summary>
        /// <param name="span">The source span from which the elements are removed.</param>
        /// <param name="trimElements">The span which contains the set of elements to remove.</param>
        /// <remarks>If <paramref name="trimElements"/> is empty, the span is returned unaltered.</remarks>
        public static ReadOnlySpan<T> Trim<T>(ReadOnlySpan<T> span, ReadOnlySpan<T> trimElements) where T : IEquatable<T>?
        {
            if (trimElements.Length > 1)
            {
                int start = ClampStart(span, trimElements);
                int length = ClampEnd(span, start, trimElements);
                return span.Slice(start, length);
            }

            if (trimElements.Length == 1)
            {
                return Trim(span, trimElements[0]);
            }

            return span;
        }

        /// <summary>
        /// Removes all leading occurrences of a set of elements specified
        /// in a readonly span from the span.
        /// </summary>
        /// <param name="span">The source span from which the elements are removed.</param>
        /// <param name="trimElements">The span which contains the set of elements to remove.</param>
        /// <remarks>If <paramref name="trimElements"/> is empty, the span is returned unaltered.</remarks>
        public static ReadOnlySpan<T> TrimStart<T>(ReadOnlySpan<T> span, ReadOnlySpan<T> trimElements) where T : IEquatable<T>?
        {
            if (trimElements.Length > 1)
            {
                return span.Slice(ClampStart(span, trimElements));
            }

            if (trimElements.Length == 1)
            {
                return TrimStart(span, trimElements[0]);
            }

            return span;
        }

        /// <summary>
        /// Removes all trailing occurrences of a set of elements specified
        /// in a readonly span from the span.
        /// </summary>
        /// <param name="span">The source span from which the elements are removed.</param>
        /// <param name="trimElements">The span which contains the set of elements to remove.</param>
        /// <remarks>If <paramref name="trimElements"/> is empty, the span is returned unaltered.</remarks>
        public static ReadOnlySpan<T> TrimEnd<T>(ReadOnlySpan<T> span, ReadOnlySpan<T> trimElements) where T : IEquatable<T>?
        {
            if (trimElements.Length > 1)
            {
                return span.Slice(0, ClampEnd(span, 0, trimElements));
            }

            if (trimElements.Length == 1)
            {
                return TrimEnd(span, trimElements[0]);
            }

            return span;
        }

        /// <summary>
        /// Delimits all leading occurrences of a set of elements specified
        /// in a readonly span from the span.
        /// </summary>
        /// <param name="span">The source span from which the elements are removed.</param>
        /// <param name="trimElements">The span which contains the set of elements to remove.</param>
        private static int ClampStart<T>(ReadOnlySpan<T> span, ReadOnlySpan<T> trimElements) where T : IEquatable<T>?
        {
            int start = 0;
            for (; start < span.Length; start++)
            {
                if (!Contains(trimElements, span[start]))
                {
                    break;
                }
            }

            return start;
        }

        /// <summary>
        /// Delimits all trailing occurrences of a set of elements specified
        /// in a readonly span from the span.
        /// </summary>
        /// <param name="span">The source span from which the elements are removed.</param>
        /// <param name="start">The start index from which to being searching.</param>
        /// <param name="trimElements">The span which contains the set of elements to remove.</param>
        private static int ClampEnd<T>(ReadOnlySpan<T> span, int start, ReadOnlySpan<T> trimElements) where T : IEquatable<T>?
        {
            // Initially, start==len==0. If ClampStart trims all, start==len
            Debug.Assert((uint)start <= span.Length);

            int end = span.Length - 1;
            for (; end >= start; end--)
            {
                if (!Contains(trimElements, span[end]))
                {
                    break;
                }
            }

            return end - start + 1;
        }

        /// <summary>
        /// Removes all leading and trailing white-space characters from the memory.
        /// </summary>
        /// <param name="memory">The source memory from which the characters are removed.</param>
        public static Memory<char> Trim(Memory<char> memory)
        {
            ReadOnlySpan<char> span = memory.Span;
            int start = ClampStart(span);
            int length = ClampEnd(span, start);
            return memory.Slice(start, length);
        }

        /// <summary>
        /// Removes all leading white-space characters from the memory.
        /// </summary>
        /// <param name="memory">The source memory from which the characters are removed.</param>
        public static Memory<char> TrimStart(Memory<char> memory)
            => memory.Slice(ClampStart(memory.Span));

        /// <summary>
        /// Removes all trailing white-space characters from the memory.
        /// </summary>
        /// <param name="memory">The source memory from which the characters are removed.</param>
        public static Memory<char> TrimEnd(Memory<char> memory)
            => memory.Slice(0, ClampEnd(memory.Span, 0));

        /// <summary>
        /// Removes all leading and trailing white-space characters from the memory.
        /// </summary>
        /// <param name="memory">The source memory from which the characters are removed.</param>
        public static ReadOnlyMemory<char> Trim(ReadOnlyMemory<char> memory)
        {
            ReadOnlySpan<char> span = memory.Span;
            int start = ClampStart(span);
            int length = ClampEnd(span, start);
            return memory.Slice(start, length);
        }

        /// <summary>
        /// Removes all leading white-space characters from the memory.
        /// </summary>
        /// <param name="memory">The source memory from which the characters are removed.</param>
        public static ReadOnlyMemory<char> TrimStart(ReadOnlyMemory<char> memory)
            => memory.Slice(ClampStart(memory.Span));

        /// <summary>
        /// Removes all trailing white-space characters from the memory.
        /// </summary>
        /// <param name="memory">The source memory from which the characters are removed.</param>
        public static ReadOnlyMemory<char> TrimEnd(ReadOnlyMemory<char> memory)
            => memory.Slice(0, ClampEnd(memory.Span, 0));

        /// <summary>
        /// Removes all leading and trailing white-space characters from the span.
        /// </summary>
        /// <param name="span">The source span from which the characters are removed.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> Trim(ReadOnlySpan<char> span)
        {
            // Assume that in most cases input doesn't need trimming
            if (span.Length == 0 ||
                (!char.IsWhiteSpace(span[0]) && !char.IsWhiteSpace(span[span.Length-1])))
            {
                return span;
            }
            return TrimFallback(span);

            [MethodImpl(MethodImplOptions.NoInlining)]
            static ReadOnlySpan<char> TrimFallback(ReadOnlySpan<char> span)
            {
                int start = 0;
                for (; start < span.Length; start++)
                {
                    if (!char.IsWhiteSpace(span[start]))
                    {
                        break;
                    }
                }

                int end = span.Length - 1;
                for (; end > start; end--)
                {
                    if (!char.IsWhiteSpace(span[end]))
                    {
                        break;
                    }
                }
                return span.Slice(start, end - start + 1);
            }
        }

        /// <summary>
        /// Removes all leading white-space characters from the span.
        /// </summary>
        /// <param name="span">The source span from which the characters are removed.</param>
        public static ReadOnlySpan<char> TrimStart(ReadOnlySpan<char> span)
        {
            int start = 0;
            for (; start < span.Length; start++)
            {
                if (!char.IsWhiteSpace(span[start]))
                {
                    break;
                }
            }

            return span.Slice(start);
        }

        /// <summary>
        /// Removes all trailing white-space characters from the span.
        /// </summary>
        /// <param name="span">The source span from which the characters are removed.</param>
        public static ReadOnlySpan<char> TrimEnd(ReadOnlySpan<char> span)
        {
            int end = span.Length - 1;
            for (; end >= 0; end--)
            {
                if (!char.IsWhiteSpace(span[end]))
                {
                    break;
                }
            }

            return span.Slice(0, end + 1);
        }

        /// <summary>
        /// Removes all leading and trailing occurrences of a specified character from the span.
        /// </summary>
        /// <param name="span">The source span from which the character is removed.</param>
        /// <param name="trimChar">The specified character to look for and remove.</param>
        public static ReadOnlySpan<char> Trim(ReadOnlySpan<char> span, char trimChar)
        {
            int start = 0;
            for (; start < span.Length; start++)
            {
                if (span[start] != trimChar)
                {
                    break;
                }
            }

            int end = span.Length - 1;
            for (; end > start; end--)
            {
                if (span[end] != trimChar)
                {
                    break;
                }
            }

            return span.Slice(start, end - start + 1);
        }

        /// <summary>
        /// Removes all leading occurrences of a specified character from the span.
        /// </summary>
        /// <param name="span">The source span from which the character is removed.</param>
        /// <param name="trimChar">The specified character to look for and remove.</param>
        public static ReadOnlySpan<char> TrimStart(ReadOnlySpan<char> span, char trimChar)
        {
            int start = 0;
            for (; start < span.Length; start++)
            {
                if (span[start] != trimChar)
                {
                    break;
                }
            }

            return span.Slice(start);
        }

        /// <summary>
        /// Removes all trailing occurrences of a specified character from the span.
        /// </summary>
        /// <param name="span">The source span from which the character is removed.</param>
        /// <param name="trimChar">The specified character to look for and remove.</param>
        public static ReadOnlySpan<char> TrimEnd(ReadOnlySpan<char> span, char trimChar)
        {
            int end = span.Length - 1;
            for (; end >= 0; end--)
            {
                if (span[end] != trimChar)
                {
                    break;
                }
            }

            return span.Slice(0, end + 1);
        }

        /// <summary>
        /// Removes all leading and trailing occurrences of a set of characters specified
        /// in a readonly span from the span.
        /// </summary>
        /// <param name="span">The source span from which the characters are removed.</param>
        /// <param name="trimChars">The span which contains the set of characters to remove.</param>
        /// <remarks>If <paramref name="trimChars"/> is empty, white-space characters are removed instead.</remarks>
        public static ReadOnlySpan<char> Trim(ReadOnlySpan<char> span, ReadOnlySpan<char> trimChars)
            => span.TrimStart(trimChars).TrimEnd(trimChars);

        /// <summary>
        /// Removes all leading occurrences of a set of characters specified
        /// in a readonly span from the span.
        /// </summary>
        /// <param name="span">The source span from which the characters are removed.</param>
        /// <param name="trimChars">The span which contains the set of characters to remove.</param>
        /// <remarks>If <paramref name="trimChars"/> is empty, white-space characters are removed instead.</remarks>
        public static ReadOnlySpan<char> TrimStart(ReadOnlySpan<char> span, ReadOnlySpan<char> trimChars)
        {
            if (trimChars.IsEmpty)
            {
                return span.TrimStart();
            }

            int start = 0;
            for (; start < span.Length; start++)
            {
                for (int i = 0; i < trimChars.Length; i++)
                {
                    if (span[start] == trimChars[i])
                    {
                        goto Next;
                    }
                }

                break;
            Next:
                ;
            }

            return span.Slice(start);
        }

        /// <summary>
        /// Removes all trailing occurrences of a set of characters specified
        /// in a readonly span from the span.
        /// </summary>
        /// <param name="span">The source span from which the characters are removed.</param>
        /// <param name="trimChars">The span which contains the set of characters to remove.</param>
        /// <remarks>If <paramref name="trimChars"/> is empty, white-space characters are removed instead.</remarks>
        public static ReadOnlySpan<char> TrimEnd(ReadOnlySpan<char> span, ReadOnlySpan<char> trimChars)
        {
            if (trimChars.IsEmpty)
            {
                return span.TrimEnd();
            }

            int end = span.Length - 1;
            for (; end >= 0; end--)
            {
                for (int i = 0; i < trimChars.Length; i++)
                {
                    if (span[end] == trimChars[i])
                    {
                        goto Next;
                    }
                }

                break;
            Next:
                ;
            }

            return span.Slice(0, end + 1);
        }

        /// <summary>
        /// Removes all leading and trailing white-space characters from the span.
        /// </summary>
        /// <param name="span">The source span from which the characters are removed.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<char> Trim(Span<char> span)
        {
            // Assume that in most cases input doesn't need trimming
            if (span.Length == 0 ||
                (!char.IsWhiteSpace(span[0]) && !char.IsWhiteSpace(span[span.Length-1])))
            {
                return span;
            }
            return TrimFallback(span);

            [MethodImpl(MethodImplOptions.NoInlining)]
            static Span<char> TrimFallback(Span<char> span)
            {
                int start = 0;
                for (; start < span.Length; start++)
                {
                    if (!char.IsWhiteSpace(span[start]))
                    {
                        break;
                    }
                }

                int end = span.Length - 1;
                for (; end > start; end--)
                {
                    if (!char.IsWhiteSpace(span[end]))
                    {
                        break;
                    }
                }
                return span.Slice(start, end - start + 1);
            }
        }

        /// <summary>
        /// Removes all leading white-space characters from the span.
        /// </summary>
        /// <param name="span">The source span from which the characters are removed.</param>
        public static Span<char> TrimStart(Span<char> span)
            => span.Slice(ClampStart(span));

        /// <summary>
        /// Removes all trailing white-space characters from the span.
        /// </summary>
        /// <param name="span">The source span from which the characters are removed.</param>
        public static Span<char> TrimEnd(Span<char> span)
            => span.Slice(0, ClampEnd(span, 0));

        /// <summary>
        /// Delimits all leading occurrences of whitespace characters from the span.
        /// </summary>
        /// <param name="span">The source span from which the characters are removed.</param>
        private static int ClampStart(ReadOnlySpan<char> span)
        {
            int start = 0;

            for (; start < span.Length; start++)
            {
                if (!char.IsWhiteSpace(span[start]))
                {
                    break;
                }
            }

            return start;
        }

        /// <summary>
        /// Delimits all trailing occurrences of whitespace characters from the span.
        /// </summary>
        /// <param name="span">The source span from which the characters are removed.</param>
        /// <param name="start">The start index from which to being searching.</param>
        private static int ClampEnd(ReadOnlySpan<char> span, int start)
        {
            // Initially, start==len==0. If ClampStart trims all, start==len
            Debug.Assert((uint)start <= span.Length);

            int end = span.Length - 1;

            for (; end >= start; end--)
            {
                if (!char.IsWhiteSpace(span[end]))
                {
                    break;
                }
            }

            return end - start + 1;
        }
    }
}
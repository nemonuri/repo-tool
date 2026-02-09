using System.Numerics;
using static Nemonuri.ByteChars.Extensions.GuardExtensions;
using Vs = Nemonuri.ByteChars.Internal.ByteCharSpanTheory.ByteVectorSizePremise;

namespace Nemonuri.ByteChars.Internal;

static partial class ByteCharSpanTheory
{
#if !NET8_0_OR_GREATER
    internal static unsafe void UnsafeByteSpanUpdate
    (
        Span<byte> left, 
        ReadOnlySpan<byte> right, 
        delegate*<byte, byte, byte> byteOp,
        delegate*<Vector<byte>, Vector<byte>, Vector<byte>> vectorOp
    )
    {
        if (TryGetConstant(right, out var rConstant))
        {
            static void ByteFallback(Span<byte> spanL, byte constR, delegate*<byte, byte, byte> byteOp0)
            {
                for (int i = 0; i < spanL.Length; i++)
                {
                    spanL[i] = byteOp0(spanL[i], constR);
                }
            }

            if (IsProperToUseVector(left))
            {
                var sr = Vs.SplitSpan(left);
                Vector<byte> vbR = GetVectorConstant(rConstant);
                foreach (Span<byte> chunk in sr.Chunks)
                {
                    Vector<byte> vbL = LoadVector(chunk);
                    Vector<byte> toStore = vectorOp(vbL, vbR);
                    StoreVector(chunk, toStore);
                }
                ByteFallback(sr.Remainder, rConstant, byteOp);
            }
            else
            {
                ByteFallback(left, rConstant, byteOp);
            }
        }
        else
        {
            static void ByteFallback(Span<byte> spanL, ReadOnlySpan<byte> spanR, delegate*<byte, byte, byte> byteOp0)
            {
                for (int i = 0; i < spanL.Length; i++)
                {
                    spanL[i] = byteOp0(spanL[i], spanR[i]);
                }
            }

            Guard.HasSizeEqualTo(left, right);

            if (IsProperToUseVector(left))
            {
                var srL = Vs.SplitSpan(left);
                var srR = Vs.SplitSpan(right);
                var chunksL = srL.Chunks;
                var chunksR = srR.Chunks;

                for (int i = 0; i < chunksL.Length; i++)
                {
                    Span<byte> chunkL = chunksL[i];
                    var vecL = LoadVector(chunkL);
                    var vecR = LoadVector(chunksR[i]);
                    
                    Vector<byte> toStore = vectorOp(vecL, vecR);
                    StoreVector(chunkL, toStore);
                }
                ByteFallback(srL.Remainder, srR.Remainder, byteOp);
            }
            else
            {
                ByteFallback(left, right, byteOp);
            }
        }
    }
#endif
}
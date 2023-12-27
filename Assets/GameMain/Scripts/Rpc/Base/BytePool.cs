
using System;
using System.Buffers;
using System.IO;

namespace GameMain.Rpc
{
    public static class BytePool
    {
        public static byte[] Rent(int length)
        {
            return ArrayPool<byte>.Shared.Rent(length);
        }

        public static byte[] Rent(MemoryStream ms)
        {
            var bytes = ArrayPool<byte>.Shared.Rent((int)ms.Length);
            Buffer.BlockCopy(ms.GetBuffer(), 0, bytes, 0, (int)ms.Length);
            return bytes;
        }

        public static void Return(byte[] buffer)
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}
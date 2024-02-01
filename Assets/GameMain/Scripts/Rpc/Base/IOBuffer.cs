using System;

namespace GameMain.Rpc
{
    public class IOBuffer
    {
        private readonly int m_Capacity;

        public int Head { get; private set; }
        public int Tail { get; private set; }

        public int Length => Tail - Head;
        public int FreeLength => m_Capacity - Tail;
        public int AllFreeLength => m_Capacity - Length;
        public byte[] Data { get; }

        public byte this[int index] => Data[Head + index];

        public IOBuffer(int length)
        {
            Data = new byte[length];
            Head = 0;
            Tail = 0;
            m_Capacity = length;
        }

        public void Reset()
        {
            Head = 0;
            Tail = 0;
        }

        public void MoveHead(int length)
        {
            if (length > Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            Head += length;
            if (Head == Tail)
            {
                Head = Tail = 0;
            }
        }

        public void MoveTail(int length)
        {
            if (length > FreeLength)
            {
                throw new ArgumentOutOfRangeException();
            }

            Tail += length;
        }

        public void AlignToHead()
        {
            if (Head == 0)
            {
                return;
            }

            var len = Length;
            Buffer.BlockCopy(Data, Head, Data, 0, len);
            Head = 0;
            Tail = len;
        }
    }
}
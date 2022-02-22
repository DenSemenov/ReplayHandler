using System;
namespace ReplayHandler
{
    public class HQMMessageReader
    {
        public byte[] Buf { get; set; }
        public int Pos { get; set; }
        public int BitPos { get; set; }

        public HQMMessageReader(byte[] bytes)
        {
            Buf = bytes;
            Pos = 0;
            BitPos = 0;
        }

        public int ReadByteAligned()
        {
            Align();
            var res = SafeGetByte(Pos);
            Pos += 1;
            return res;
        }

        public void Align()
        {
            if (BitPos > 0)
            {
                BitPos = 0;
                Pos += 1;
            }
        }

        public int SafeGetByte(int pos)
        {
            if (Pos < Buf.Length)
            {
                return Buf[pos];
            }
            else
            {
                return 0;
            }
        }

        public int ReadBits(int b)
        {
            var bitsRemaining = b;
            var res = 0;
            var p = 0;

            while (bitsRemaining > 0)
            {
                var bitsPossibleToWrite = 8 - BitPos;
                var bits = Math.Min(bitsRemaining, bitsPossibleToWrite);
                var mask = bits == 8 ? 0xFF : ~(0xFF << bits);
                var a = (SafeGetByte(Pos) >> BitPos) & mask;

                res = (int)(res | (a << p));

                if (bitsRemaining >= bitsPossibleToWrite)
                {
                    bitsRemaining -= bitsPossibleToWrite;

                    BitPos = 0;
                    Pos += 1;
                    p += bits;
                }
                else
                {
                    BitPos += bitsRemaining;
                    bitsRemaining = 0;
                }
            }

            return res;
        }

        public int ReadU32Aligned()
        {
            Align();
            var b1 = SafeGetByte(Pos);
            var b2 = SafeGetByte(Pos + 1);
            var b3 = SafeGetByte(Pos + 2);
            var b4 = SafeGetByte(Pos + 3);
            Pos += 4;
            return b1 | b2 << 8 | b3 << 16 | b4 << 24;
        }

        public int ReadBitsSigned(int b)
        {
            var a = ReadBits(b);

            if (a>=1 << (b - 1))
            {
                return (-1 << b) | (a);
            }
            else
            {
                return a;
            }
        }

        public int ReadPos(int b)
        {
            var posType = ReadBits(2);

            switch (posType)
            {
                case 0:
                    return ReadBitsSigned(3);
                case 1:
                    return ReadBitsSigned(6);
                case 2:
                    return ReadBitsSigned(12);
                case 3:
                    return ReadBits(b);
                default:
                    return 0;
            }
        }

        public void Next()
        {
            Pos += 1;
            BitPos = 0;
        }
    }
}
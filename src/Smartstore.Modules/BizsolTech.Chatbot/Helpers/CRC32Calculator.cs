using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace BizsolTech.Chatbot.Helpers
{
    public class CRC32Calculator
    {
        public static uint CalculateCRC32FromFile(IFormFile file)
        {
            using (var stream = file.OpenReadStream())
            {
                var crc32 = new Crc32();
                var hash = crc32.ComputeHash(stream);
                return BitConverter.ToUInt32(hash, 0);
            }
        }
        public static uint CalculateCRC32FromContent(string content)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            using (var stream = new MemoryStream(bytes))
            {
                var crc32 = new Crc32();
                var hash = crc32.ComputeHash(stream);
                return BitConverter.ToUInt32(hash, 0);
            }
        }
    }

    public class Crc32 : HashAlgorithm
    {
        private const uint Polynomial = 0xedb88320;
        private uint[] _table;

        public Crc32()
        {
            _table = CreateTable();
            HashSizeValue = 32;
        }

        public override void Initialize()
        {
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            uint crc = 0xffffffff;
            for (var i = ibStart; i < cbSize; i++)
            {
                crc = (_table[(crc ^ array[i]) & 0xff] ^ (crc >> 8));
            }
            HashValue = BitConverter.GetBytes(~crc);
        }

        protected override byte[] HashFinal()
        {
            return HashValue;
        }

        private static uint[] CreateTable()
        {
            var table = new uint[256];

            for (uint i = 0; i < 256; i++)
            {
                var crc = i;
                for (int j = 8; j > 0; j--)
                {
                    if ((crc & 1) == 1)
                    {
                        crc = (crc >> 1) ^ Polynomial;
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
                table[i] = crc;
            }

            return table;
        }
    }
}

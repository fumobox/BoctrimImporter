using System;
using System.Text;

namespace Boctrim.Infrastructure
{

    public static class Base64Tools
    {

        public const int ErrorCode = -1;

        const int SixBitMask = 63;

        static readonly char[] EncodeTableA = 
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H',
            'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
            'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X',
            'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f',
            'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n',
            'o', 'p', 'q', 'r', 's', 't', 'u', 'v',
            'w', 'x', 'y', 'z', '0', '1', '2', '3',
            '4', '5', '6', '7', '8', '9', '+', '/' 
        };

        /// <summary>
        /// intをbase64文字列に変換する。下位2bitは無視。
        /// </summary>
        public static string ToBase64String30A(int i)
        {
            var buff = new StringBuilder();
            buff.Append(EncodeTableA[(i >> 26) & SixBitMask]);
            buff.Append(EncodeTableA[(i >> 20) & SixBitMask]);
            buff.Append(EncodeTableA[(i >> 14) & SixBitMask]);
            buff.Append(EncodeTableA[(i >> 8) & SixBitMask]);
            buff.Append(EncodeTableA[(i >> 2) & SixBitMask]);
            return buff.ToString();
        }

        public static string ToBase64String24A(int i)
        {
            var buff = new StringBuilder();
            buff.Append(EncodeTableA[(i >> 26) & SixBitMask]);
            buff.Append(EncodeTableA[(i >> 20) & SixBitMask]);
            buff.Append(EncodeTableA[(i >> 14) & SixBitMask]);
            buff.Append(EncodeTableA[(i >> 8) & SixBitMask]);
            return buff.ToString();
        }

        /// <summary>
        /// base64文字列をintに変換する。
        /// </summary>
        public static int ParseInt6(string str)
        {
            // 文字列のサイズを揃える。
            if (str.Length > 6)
            {
                str = str.Substring(0, 6);
            }
            else if (str.Length < 6)
            {
                while (str.Length < 6)
                {
                    str = str + EncodeTableA[0];
                }
            }

            int value = 0;
            int n = 0;
            for (int i = 0; i < 5; i++)
            {
                n = DecodeChar(str[i]);
                if (n == ErrorCode)
                {
                    return ErrorCode;
                }
                value |= n << (26 - (i * 6));
            }

            // 最後の数値
            n = DecodeChar(str[5]) >> 4;
            value |= n;

            return value;
        }

        /// <summary>
        /// base64文字列をintに変換する。高速化のためにエラー処理をしない。
        /// </summary>
        public static uint ParseInt30Fast(String str)
        {
            uint value = 0;
            for (int i = 0; i < 5; i++)
            {
                char c = str[i];
                for (uint j = 0; j < EncodeTableA.Length; j++)
                {
                    if (c == EncodeTableA[j])
                    {
                        value |= j << (26 - (i * 6));
                        break;
                    }
                }
            }
            return value;
        }

        public static uint ParseInt24Fast(String str)
        {
            uint value = 0;
            for (int i = 0; i < 4; i++)
            {
                char c = str[i];
                for (uint j = 0; j < EncodeTableA.Length; j++)
                {
                    if (c == EncodeTableA[j])
                    {
                        value |= j << (26 - (i * 6));
                        break;
                    }
                }
            }
            return value;
        }

        /// <summary>
        /// Convert the base64 character into the array index.
        /// </summary>
        public static int DecodeChar(char c)
        {
            for (int i = 0; i < EncodeTableA.Length; i++)
            {
                if (c == EncodeTableA[i])
                {
                    return i;
                }
            }
            return ErrorCode;
        }

    }

}
namespace Boctrim.Domain
{

    /// <summary>
    /// Boct packet tools for Fumotrim format.
    /// </summary>
    public class BoctPacketTools
    {
        // アドレス仕様
        // 上位3ビットはアドレス長
        // 次の21ビットは3ビット区切りのアドレス
        // 次の7ビットはマテリアルID
        // 最後の1ビットは拡張用フラグ
        
        // ルートアドレスを指すが、データパートが未入力、つまり0-15がルートアドレスを指す
        public const uint ADDRESS_ROOT = 0;

        public const uint MASK_ADDRESS_LENGTH = 3758096384;
        // 3bit
        public const uint MASK_ADDRESS = 536870656;
        // 24bit
        public const uint MASK_MATERIAL = 252;
        // 6bit
        public const uint MASK_EXTENSION = 3;
        // 2bit
        public const uint MASK_POSITION = 7;
        // 111
        public const uint MASK_NON_ADDRESS = 255;
        // non address part

        public const uint ADDRESS_LENGTH_MAX = 7;

        public const uint BIT_LENGTH_POSITION = 3;

        public const uint MASK_CLEAN_UP = 2097151;

        public static float[] _scales;

        static BoctPacketTools()
        {
            // レンダリング計算で使用するので少し多めに確保
            _scales = new float[ADDRESS_LENGTH_MAX + 2];
            _scales[0] = 1.0f;
            for (int i = 1; i < _scales.Length; i++)
            {
                _scales[i] = _scales[i - 1] * 0.5f;
            }
        }

        public static bool IsRootAddress(uint p)
        {
            if (GetAddressLengthPart(p) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsMaxLength(uint p)
        {
            if (GetAddressLengthPart(p) == ADDRESS_LENGTH_MAX)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static uint GetAddressLengthPart(uint p)
        {
            //return (p & MASK_ADDRESS_LENGTH) >>> 29;
            return p >> 29;
        }

        public static uint GetAddressPart(uint p)
        {
            return (p & MASK_ADDRESS) >> 8;
        }

        public static uint GetMaterialPart(uint p)
        {
            return (p & MASK_MATERIAL) >> 2;
        }

        public static uint SetMaterialPart(uint p, uint value)
        {
            return (p & ~MASK_MATERIAL) | (value << 2);
        }

        public static uint ClearMaterialPart(uint p)
        {
            return p & ~MASK_MATERIAL;
        }

        public static uint GetExtensionPart(uint p)
        {
            return p & MASK_EXTENSION;
        }

        public static uint SetExtensionPart(uint p, uint value)
        {
            return (p & ~MASK_EXTENSION) | (value);
        }

        public static uint ClearExtensionPart(uint p)
        {
            return p & ~MASK_EXTENSION;
        }

        public static uint ClearNonAddressPart(uint p)
        {
            return p & ~MASK_NON_ADDRESS;
        }

        /// <summary>
        /// Combine parts.
        /// </summary>
        public static uint Combine(uint length, uint address, uint material, uint ext)
        {
            return (length << 29) | (address << 8) | (material << 2) | ext;
        }

        /// <summary>
        /// Combine parts.
        /// </summary>
        public static uint Combine(uint length, uint address, uint material)
        {
            return (length << 29) | (address << 8) | (material << 2);
        }

        public static uint PushAddress(uint p, uint value)
        {
            // 境界処理は外部でやる 0 <= value <= 7
            uint lp = GetAddressLengthPart(p) + 1;
            uint ap = GetAddressPart(p);
            return Combine(lp, ap | PutPosition(lp, value), GetMaterialPart(p), GetExtensionPart(p));
        }

        private static uint PutPosition(uint index, uint value)
        {
            // 1 <= index <= 7 0 <= value <= 7
            // アドレス領域に挿入するためのポジションを生成する
            return value << (int)((ADDRESS_LENGTH_MAX - index) * BIT_LENGTH_POSITION);
        }

        /// <summary>
        /// 最後のアドレスを返す ルートの場合、0
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static uint GetLastPosition(uint p)
        {
            uint lp = GetAddressLengthPart(p);
            if (lp == 0)
            {
                return 0;
            }
            else
            {
                /*
             * 1 <= lp <= 7
             */
                uint shift = ((ADDRESS_LENGTH_MAX - lp) * BIT_LENGTH_POSITION);
                return (GetAddressPart(p) & (MASK_POSITION << (int)shift)) >> (int)shift;
            }
        }

        /// <summary>
        /// Get the position in the address.
        /// </summary>
        /// <returns>The position</returns>
        /// <param name="p">Packet</param>
        /// <param name="index">Index (0 <= index <= 7)</param>
        public static uint GetPosition(uint p, uint index)
        {
            if (index == 0)
            {
                return 0;
            }
            else
            {
                uint shift = ((ADDRESS_LENGTH_MAX - index) * BIT_LENGTH_POSITION);
                return (GetAddressPart(p) & (MASK_POSITION << (int)shift)) >> (int)shift;
            }
        }

        private static uint SetPosition(uint p, uint index, uint value)
        {
            // 0 <= index <= 7
            // lengthが変わる可能性があるので、内部使用限定
            
            if (index == 0)
            {
                return 0;
            }
            else
            {
                uint ap = GetAddressPart(p);
                uint mask_value = PutPosition(index, value);
                uint mask_clear = ~PutPosition(index, MASK_POSITION);
                ap = (ap & mask_clear) | mask_value;
                return Combine(GetAddressLengthPart(p), ap, GetMaterialPart(p), GetExtensionPart(p));
            }
        }

        public static string ToString(uint p)
        {
            uint lp = GetAddressLengthPart(p);
            // int ap = getAddressPart(address);
            uint mp = GetMaterialPart(p);
            uint ep = GetExtensionPart(p);

            string str = "[" + lp + ",";
            for (uint i = 1; i <= ADDRESS_LENGTH_MAX; i++)
            {
                str += GetPosition(p, i);
            }
            str += "," + mp + "," + ep;
            return str + "]";
        }

        /// <summary>
        /// アドレスパートの余分な箇所と拡張パートをクリアする
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static uint cleanup(uint p)
        {
            /*
         * (p  >>> 29)
         * アドレス長を取得
         * 
         * ((p  >>> 29) * BIT_LENGTH_POSITION)
         * シフト量を計算
         * 
         * (MASK_CLEAN_UP >>> ((p  >>> 29) * BIT_LENGTH_POSITION))
         * マスクを作成
         * 
         * ((MASK_CLEAN_UP >>> ((p  >>> 29) * BIT_LENGTH_POSITION)) << 8)
         * マスクを配置
         * 
         * ~(((MASK_CLEAN_UP >>> ((p  >>> 29) * BIT_LENGTH_POSITION)) << 8) | MASK_EXTENSION)
         * マスクを追加して全体を反転
         * 
         */
            return p & ~(((MASK_CLEAN_UP >> (int)((p >> 29) * BIT_LENGTH_POSITION)) << 8) | MASK_EXTENSION);
        }

        public static float getBoctScale(int p)
        {
            return _scales[(p & MASK_ADDRESS_LENGTH) >> 29];
        }

        /// <summary>
        /// パケットからアドレス部のみを抽出する。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static uint getKey(uint p)
        {
            /*
         * (p  >>> 29)
         * アドレス長を取得
         * 
         * ((p  >>> 29) * BIT_LENGTH_POSITION)
         * シフト量を計算
         * 
         * (MASK_CLEAN_UP >>> ((p  >>> 29) * BIT_LENGTH_POSITION))
         * マスクを作成
         * 
         * ((MASK_CLEAN_UP >>> ((p  >>> 29) * BIT_LENGTH_POSITION)) << 8)
         * マスクを配置
         * 
         * ~(((MASK_CLEAN_UP >>> ((p  >>> 29) * BIT_LENGTH_POSITION)) << 8) | MASK_NON_ADDRESS)
         * マスクを追加して全体を反転
         * 
         */
            return p & ~(((MASK_CLEAN_UP >> (int)((p >> 29) * BIT_LENGTH_POSITION)) << 8) | MASK_NON_ADDRESS);
        }

    }

}

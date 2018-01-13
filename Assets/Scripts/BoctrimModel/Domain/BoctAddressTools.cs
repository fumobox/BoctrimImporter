using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Boctrim.Domain
{
    public static class BoctAddressTools
    {
        /// <summary>
        /// Adjoining Boct Address Table
        /// Adjoining Position = ABA[position][face]
        /// </summary>
        static readonly int[,] AbaTable =
            {
                {1, 3, 11, 13, 14, 4},
                {10, 2, 0, 12, 15, 5},
                {13, 11, 3, 1, 16, 6},
                {2, 10, 12, 0, 17, 7},
                {5, 7, 15, 17, 0, 10},
                {14, 6, 4, 16, 1, 11},
                {17, 15, 7, 5, 2, 12},
                {6, 14, 16, 4, 3, 13}
            };

        static readonly Dictionary<int, float> Extents = new Dictionary<int, float>();

        /// <summary>
        /// </summary>
        /// <remarks>
        /// WIP
        /// </remarks>
        public static List<byte> CreateGlobalAddress(int length = 100)
        {
            var list = new List<byte>();
            list.Add(0);
            for (int i = 0; i < length; i++) 
            {
                list.Add(6);
            }
            return list;
        }

        public static string ToString(IEnumerable<byte> address, bool printHead = false)
        {
            if (address == null)
                return "";

            if (!address.Any())
            {
                return printHead ? "H" : "";
            }

            var builder = new StringBuilder();
            foreach(var b in address)
            {
                builder.Append(b);
            }
            return builder.ToString();
        }

        public static List<byte> FromString(string address)
        {
            var list = new List<byte>(address.Length);

            if (string.IsNullOrEmpty(address))
            {
                return list;
            }

            foreach (var c in address)
            {
                list.Add(byte.Parse(c.ToString()));
            }
            return list;
        }

        /// <summary>
        /// Get the length of one side of the block.
        /// </summary>
        /// <param name="address">Boct Address</param>
        /// <returns>Normalized length</returns>
        /// <remarks>The root block length is 1.0.</remarks>
        public static float GetBlockSize(List<byte> address)
        {
            int n = address.Count;

            if (n == 0)
                return 1;

            float l = 1;

            for (int i = 0; i < n; i++)
            {
                l *= 0.5f;
            }
            return l;
        }

        /// <summary>
        /// Get the coordinate of the block.
        /// </summary>
        /// <param name="address"></param>
        /// <returns>Normalized coordinate</returns>
        public static Vector3 GetCoordinate(List<byte> address)
        {
            var p = Vector3.zero;
            float l = 0.25f;
            for (int i = 0; i < address.Count; i++)
            {
                switch (address[i])
                {
                    case 0:
                        p.x -= l;
                        p.y += l;
                        p.z += l;
                        break;
                    case 1:
                        p.x -= l;
                        p.y += l;
                        p.z -= l;
                        break;
                    case 2:
                        p.x += l;
                        p.y += l;
                        p.z -= l;
                        break;
                    case 3:
                        p.x += l;
                        p.y += l;
                        p.z += l;
                        break;
                    case 4:
                        p.x -= l;
                        p.y -= l;
                        p.z += l;
                        break;
                    case 5:
                        p.x -= l;
                        p.y -= l;
                        p.z -= l;
                        break;
                    case 6:
                        p.x += l;
                        p.y -= l;
                        p.z -= l;
                        break;
                    case 7:
                        p.x += l;
                        p.y -= l;
                        p.z += l;
                        break;
                }
                l *= 0.5f;
            }

            //Debug.Log("C: " + BoctAddressTools.ToString(address) + " " + p.ToString("F4"));

            return p;
        }

        public static Vector3 GetCoordinateXZ(List<byte> address)
        {
            var p = Vector2.zero;
            float l = 0.25f;
            for (int i = 0; i < address.Count; i++)
            {
                switch (address[i])
                {
                    case 0:
                        p.x -= l;
                        p.y += l;
                        break;
                    case 1:
                        p.x -= l;
                        p.y -= l;
                        break;
                    case 2:
                        p.x += l;
                        p.y -= l;
                        break;
                    case 3:
                        p.x += l;
                        p.y += l;
                        break;
                    case 4:
                        p.x -= l;
                        p.y += l;
                        break;
                    case 5:
                        p.x -= l;
                        p.y -= l;
                        break;
                    case 6:
                        p.x += l;
                        p.y -= l;
                        break;
                    case 7:
                        p.x += l;
                        p.y += l;
                        break;
                }
                l *= 0.5f;
            }
            return p;
        }

        public static Vector2 GetCoordinateXY(List<byte> address)
        {
            var p = Vector2.zero;
            float l = 0.25f;
            for (int i = 0; i < address.Count; i++)
            {
                switch (address[i])
                {
                    case 0:
                        p.x -= l;
                        p.y += l;
                        break;
                    case 1:
                        p.x -= l;
                        p.y += l;
                        break;
                    case 2:
                        p.x += l;
                        p.y += l;
                        break;
                    case 3:
                        p.x += l;
                        p.y += l;
                        break;
                    case 4:
                        p.x -= l;
                        p.y -= l;
                        break;
                    case 5:
                        p.x -= l;
                        p.y -= l;
                        break;
                    case 6:
                        p.x += l;
                        p.y -= l;
                        break;
                    case 7:
                        p.x += l;
                        p.y -= l;
                        break;
                }
                l *= 0.5f;
            }

            //Debug.Log("C: " + BoctAddressTools.ToString(address) + " " + p.ToString("F4"));

            return p;
        }

        public static Vector4 GetBounds(List<byte> address)
        {
            var bounds = Vector4.zero;
            var c = GetCoordinate(address);
            bounds.x = c.x;
            bounds.y = c.y;
            bounds.z = c.z;

            int len = address.Count;
            if (Extents.ContainsKey(len))
            {
                // Load the extents from cache.
                bounds.w = Extents[len];
            }
            else
            {
                var ex = Mathf.Pow(0.5f, len) / 2f;
                bounds.w = ex;

                // Cache the extents.
                Extents.Add(len, ex);
            }

            return bounds;
        }

        public static int GetDepth(List<byte> address)
        {
            return address.Count;
        }

        public static List<byte> GetAdjoiningBoctAddress(List<byte> address, int face, bool loop = false) 
        {
            int len = address.Count;
            if (len == 0)
            {
                return null;
            }

            // Adjoining boct address
            var aba = new List<byte>(address.Count);

            bool copy = false;

            // Except root boct
            for (int i = len - 1; i >= 0; i--)
            {
                // Adjacent boct ID
                byte localAddress = address[i];

                if(copy)
                {
                    aba.Add ((byte)localAddress);
                    continue;
                }

                int s = AbaTable[localAddress,face];

                if (s < 10)
                {
                    aba.Add ((byte)s);
                    copy = true;
                }
                else 
                {
                    if (i == 0)
                    {
                        if(loop)
                        {
                            aba.Add((byte)(s-10));
                        }
                        else
                        {
                            return null;
                        }
                    } 
                    else
                    {
                        aba.Add((byte)(s-10));
                    }
                }
            }
            aba.Reverse();
            return aba;
        }

        public static List<byte> Shift(List<byte> address, Direction direction, int? depth = null)
        {
            // Debug.Log(((byte)direction).ToString());

            if (!depth.HasValue)
                depth = address.Count - 1;

            var res = new List<byte>(address);
            for (int i = depth.Value; i >= 0; i--)
            {
                int a = address[i];
                int s = AbaTable[a,(byte)direction];
                if (s < 10)
                {
                    // Bros
                    res[i] = (byte)s;
                    break;
                }
                else
                {
                    // Cousin
                    res[i] = (byte)(s - 10);
                }
            }
            return res;
        }

    }

}
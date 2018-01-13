using System.Collections;
using System;
using UnityEngine;

namespace Boctrim.Domain
{

    [Serializable]
    public static class ColorExtension
    {
        
        public static ColorData ToData(this Color col)
        {
            var data = new ColorData();
            data.R = col.r;
            data.G = col.g;
            data.B = col.b;
            data.A = col.a;
            return data;
        }

    }

}

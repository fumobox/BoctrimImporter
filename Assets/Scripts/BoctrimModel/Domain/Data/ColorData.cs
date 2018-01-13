using System.Collections;
using System;
using UnityEngine;

namespace Boctrim.Domain
{

    public class ColorData: BoctrimData
    {
        public float R { get; set;}
        public float G { get; set;}
        public float B { get; set;}
        public float A { get; set;}
        
        public ColorData()
        {
        }
        
        public ColorData(float r, float g, float b, float a = 1f)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
        
        public Color ToColor()
        {
            var color = new Color();
            color.r = R;
            color.g = G;
            color.b = B;
            color.a = A;
            return color;
        }

    }

}

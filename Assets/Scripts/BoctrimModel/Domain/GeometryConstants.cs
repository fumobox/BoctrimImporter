using UnityEngine;
using System.Collections;

namespace Boctrim.Domain
{
    
    public enum Direction
    {
        South = 0, East, North, West, Top, Bottom
    }

    /// <summary>
    /// Geometry constants.
    /// </summary>
    public static class GeometryConstants
    {

        public static readonly Vector3[] Normals;

        public static readonly Vector2[] UVs;

        public static readonly float[] R;

        public static readonly Plane[] Planes;

        public static Vector3 NormalSouth {get {return Normals[0];}}
        public static Vector3 NormalEast {get {return Normals[1];}}
        public static Vector3 NormalNorth {get {return Normals[2];}}
        public static Vector3 NormalWest {get {return Normals[3];}}
        public static Vector3 NormalTop {get {return Normals[4];}}
        public static Vector3 NormalBottom {get {return Normals[5];}}

        static GeometryConstants()
        {
            Normals = new Vector3[6];
            // South
            Normals[0] = new Vector3(0, 0, -1);
            // East
            Normals[1] = new Vector3(1, 0, 0);
            // North
            Normals[2] = new Vector3(0, 0, 1);
            // West
            Normals[3] = new Vector3(-1, 0, 0);
            // Top
            Normals[4] = new Vector3(0, 1, 0);
            // Bottom
            Normals[5] = new Vector3(0, -1, 0);

            UVs = new Vector2[6];
            UVs[0] = new Vector2(0, 0);
            UVs[1] = new Vector2(0, 1);
            UVs[2] = new Vector2(1, 0);
            UVs[3] = new Vector2(1, 1);

            R = new float[100];
            R[0] = 1f;
            for (int i = 1; i < R.Length; i++)
            {
                R[i] = R[i - 1] * 0.5f;
            }

            Planes = new Plane[6];
            // South
            Planes[0] = new Plane(new Vector3(0, 0, -1), 1);
            // East
            Planes[1] = new Plane(new Vector3(1, 0, 0), 1);
            // North
            Planes[2] = new Plane(new Vector3(0, 0, 1), 1);
            // West
            Planes[3] = new Plane(new Vector3(-1, 0, 0), 1);
            // Top
            Planes[4] = new Plane(new Vector3(0, 1, 0), 1);
            // Bottom
            Planes[5] = new Plane(new Vector3(0, -1, 0), 1);

        }

    }
}
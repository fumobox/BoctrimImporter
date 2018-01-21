using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Boctrim.Domain;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Boctrim.Presentation
{

    /// <summary>
    /// Mesh generator for the boct model.
    /// </summary>
    public class BoctMeshGenerator
    {
        public float BlockScale { get; set; }

        public Material BoctMaterial { get; set; }

        public BoctMaterialList MaterialList { get; set; }

        public bool GenerateCollider { get; set; }
     
        #if UNITY_EDITOR
        public bool SaveMesh { get; set; }
        #endif
        
        public string MeshPath { get; set; }
        
        readonly List<Vector3> _vertexList;
        readonly List<Vector3> _normalList;
        readonly List<Vector2> _uvList;
        readonly List<Color> _colorList;
        
        public BoctMeshGenerator()
        {
            _vertexList = new List<Vector3>();
            _normalList = new List<Vector3>();
            _uvList = new List<Vector2>();
            _colorList = new List<Color>();

            BlockScale = 0.9f;
        }
        
        public void GenerateMesh(Boct head, GameObject target, string meshName = "")
        {
            var address = head.Address;
            var pos = BoctAddressTools.GetCoordinate(address);

            #if UNITY_EDITOR
            if (string.IsNullOrEmpty(meshName))
            {
                var cnt = head.SolidCount;
                target.name = BoctAddressTools.ToString(address, true) + "_" + cnt;
            }
            else
            {
                target.name = meshName;
            }
            #endif
            
            target.transform.localRotation = Quaternion.identity;
            target.transform.localPosition = Vector3.zero;
            target.transform.localScale = new Vector3(1, 1, 1);

            var mf = target.GetComponent<MeshFilter>();
            if(mf == null)
                mf = target.gameObject.AddComponent<MeshFilter>();

            var mr = target.GetComponent<MeshRenderer>();
            if(mr == null)
                mr = target.gameObject.AddComponent<MeshRenderer>();

            mr.material = BoctMaterial;

            var mesh = new Mesh();
            mesh.name = meshName;
            
            GenerateMesh(head, address.Count, pos);

            // Convert to arrays.
            mesh.vertices = _vertexList.ToArray();
            mesh.normals = _normalList.ToArray();
            mesh.colors = _colorList.ToArray();
            mesh.uv = _uvList.ToArray();

            int[] indexArray = new int[_vertexList.Count];
            for (int i = 0; i < indexArray.Length; i++)
            {
                indexArray[i] = i;
            }

            mesh.triangles = indexArray;
            mf.mesh = mesh;

            #if UNITY_EDITOR
            if (SaveMesh)
            {
                var path = MeshPath + "/" + mesh.name + ".asset";
                if (File.Exists(path))
                {
                    AssetDatabase.DeleteAsset(path);
                }
                AssetDatabase.CreateAsset(mesh,  MeshPath + "/" + mesh.name + ".asset");
                AssetDatabase.SaveAssets();
            }
            #endif
            
            if (GenerateCollider)
            {
                var meshCollider = target.GetComponent<MeshCollider>();
                if (meshCollider == null)
                    meshCollider = target.gameObject.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = mesh;
            }

            // Clear lists.
            _vertexList.Clear();
            _normalList.Clear();
            _uvList.Clear();
            _colorList.Clear();
        }

        void GenerateMesh(Boct b, int depth, Vector3 c)
        {
            // Debug.Log("GenerateMesh: " + BoctAddressTools.ToString(b.Address));

            if (b.HasChild)
            {
                //Kenny.D("HasChildren: true");

                float r = GeometryConstants.R[depth + 2];
                float cx0 = c.x - r;
                float cx1 = c.x + r;
                float cy0 = c.y - r;
                float cy1 = c.y + r;
                float cz0 = c.z - r;
                float cz1 = c.z + r;

                depth++;

                Vector3[] vc = new Vector3[8];
                vc[0] = new Vector3(cx0, cy1, cz1);
                vc[1] = new Vector3(cx0, cy1, cz0);
                vc[2] = new Vector3(cx1, cy1, cz0);
                vc[3] = new Vector3(cx1, cy1, cz1);
                vc[4] = new Vector3(cx0, cy0, cz1);
                vc[5] = new Vector3(cx0, cy0, cz0);
                vc[6] = new Vector3(cx1, cy0, cz0);
                vc[7] = new Vector3(cx1, cy0, cz1);

                Boct[] bc = b.Children;
                for (int i = 0; i < 8; i++)
                {
                    if (bc[i] == null)
                        continue;
                    GenerateMesh(bc[i], depth, vc[i]);
                }
            }
            else
            {
                // Debug.Log("HasChildren: false");

                if (!b.HasMaterial)
                {
                    // Debug.Log("Empty boct.");
                    return;
                }
                var mid = b.MaterialId;
                var col = MaterialList.Materials[mid].Color;

                // Debug.LogWarning(col.ToString());

                // Add Vector
                // TODO Fix scale
                float r = GeometryConstants.R[depth + 1] * BlockScale;

                float cx0 = c.x - r;
                float cx1 = c.x + r;
                float cy0 = c.y - r;
                float cy1 = c.y + r;
                float cz0 = c.z - r;
                float cz1 = c.z + r;

                // South
                _vertexList.Add(new Vector3(cx0, cy1, cz0));
                _vertexList.Add(new Vector3(cx1, cy0, cz0));
                _vertexList.Add(new Vector3(cx0, cy0, cz0));

                _vertexList.Add(new Vector3(cx0, cy1, cz0));
                _vertexList.Add(new Vector3(cx1, cy1, cz0));
                _vertexList.Add(new Vector3(cx1, cy0, cz0));

                _normalList.Add(GeometryConstants.Normals[0]);
                _normalList.Add(GeometryConstants.Normals[0]);
                _normalList.Add(GeometryConstants.Normals[0]);
                _normalList.Add(GeometryConstants.Normals[0]);
                _normalList.Add(GeometryConstants.Normals[0]);
                _normalList.Add(GeometryConstants.Normals[0]);

                _uvList.Add(GeometryConstants.Normals[1]);
                _uvList.Add(GeometryConstants.Normals[2]);
                _uvList.Add(GeometryConstants.Normals[0]);
                _uvList.Add(GeometryConstants.Normals[1]);
                _uvList.Add(GeometryConstants.Normals[3]);
                _uvList.Add(GeometryConstants.Normals[2]);

                _colorList.Add(col);
                _colorList.Add(col);
                _colorList.Add(col);
                _colorList.Add(col);
                _colorList.Add(col);
                _colorList.Add(col);

                // East
                _vertexList.Add(new Vector3(cx1, cy1, cz0));
                _vertexList.Add(new Vector3(cx1, cy0, cz1));
                _vertexList.Add(new Vector3(cx1, cy0, cz0));

                _vertexList.Add(new Vector3(cx1, cy1, cz0));
                _vertexList.Add(new Vector3(cx1, cy1, cz1));
                _vertexList.Add(new Vector3(cx1, cy0, cz1));

                _normalList.Add(GeometryConstants.Normals[1]);
                _normalList.Add(GeometryConstants.Normals[1]);
                _normalList.Add(GeometryConstants.Normals[1]);
                _normalList.Add(GeometryConstants.Normals[1]);
                _normalList.Add(GeometryConstants.Normals[1]);
                _normalList.Add(GeometryConstants.Normals[1]);

                _uvList.Add(GeometryConstants.Normals[1]);
                _uvList.Add(GeometryConstants.Normals[2]);
                _uvList.Add(GeometryConstants.Normals[0]);
                _uvList.Add(GeometryConstants.Normals[1]);
                _uvList.Add(GeometryConstants.Normals[3]);
                _uvList.Add(GeometryConstants.Normals[2]);

                _colorList.Add(col);
                _colorList.Add(col);
                _colorList.Add(col);
                _colorList.Add(col);
                _colorList.Add(col);
                _colorList.Add(col);

                // North
                _vertexList.Add(new Vector3(cx1, cy1, cz1));
                _vertexList.Add(new Vector3(cx0, cy0, cz1));
                _vertexList.Add(new Vector3(cx1, cy0, cz1));

                _vertexList.Add(new Vector3(cx1, cy1, cz1));
                _vertexList.Add(new Vector3(cx0, cy1, cz1));
                _vertexList.Add(new Vector3(cx0, cy0, cz1));

                _normalList.Add(GeometryConstants.Normals[2]);
                _normalList.Add(GeometryConstants.Normals[2]);
                _normalList.Add(GeometryConstants.Normals[2]);
                _normalList.Add(GeometryConstants.Normals[2]);
                _normalList.Add(GeometryConstants.Normals[2]);
                _normalList.Add(GeometryConstants.Normals[2]);

                _uvList.Add(GeometryConstants.Normals[1]);
                _uvList.Add(GeometryConstants.Normals[2]);
                _uvList.Add(GeometryConstants.Normals[0]);
                _uvList.Add(GeometryConstants.Normals[1]);
                _uvList.Add(GeometryConstants.Normals[3]);
                _uvList.Add(GeometryConstants.Normals[2]);

                _colorList.Add(col);
                _colorList.Add(col);
                _colorList.Add(col);
                _colorList.Add(col);
                _colorList.Add(col);
                _colorList.Add(col);

                // West
                _vertexList.Add(new Vector3(cx0, cy1, cz1));
                _vertexList.Add(new Vector3(cx0, cy0, cz0));
                _vertexList.Add(new Vector3(cx0, cy0, cz1));

                _vertexList.Add(new Vector3(cx0, cy1, cz1));
                _vertexList.Add(new Vector3(cx0, cy1, cz0));
                _vertexList.Add(new Vector3(cx0, cy0, cz0));

                _normalList.Add(GeometryConstants.Normals[3]);
                _normalList.Add(GeometryConstants.Normals[3]);
                _normalList.Add(GeometryConstants.Normals[3]);
                _normalList.Add(GeometryConstants.Normals[3]);
                _normalList.Add(GeometryConstants.Normals[3]);
                _normalList.Add(GeometryConstants.Normals[3]);

                _uvList.Add(GeometryConstants.Normals[1]);
                _uvList.Add(GeometryConstants.Normals[2]);
                _uvList.Add(GeometryConstants.Normals[0]);
                _uvList.Add(GeometryConstants.Normals[1]);
                _uvList.Add(GeometryConstants.Normals[3]);
                _uvList.Add(GeometryConstants.Normals[2]);

                _colorList.Add(col);
                _colorList.Add(col);
                _colorList.Add(col);
                _colorList.Add(col);
                _colorList.Add(col);
                _colorList.Add(col);

                // Top
                _vertexList.Add(new Vector3(cx0, cy1, cz1));
                _vertexList.Add(new Vector3(cx1, cy1, cz0));
                _vertexList.Add(new Vector3(cx0, cy1, cz0));

                _vertexList.Add(new Vector3(cx0, cy1, cz1));
                _vertexList.Add(new Vector3(cx1, cy1, cz1));
                _vertexList.Add(new Vector3(cx1, cy1, cz0));

                _normalList.Add(GeometryConstants.Normals[4]);
                _normalList.Add(GeometryConstants.Normals[4]);
                _normalList.Add(GeometryConstants.Normals[4]);
                _normalList.Add(GeometryConstants.Normals[4]);
                _normalList.Add(GeometryConstants.Normals[4]);
                _normalList.Add(GeometryConstants.Normals[4]);

                _uvList.Add(GeometryConstants.Normals[1]);
                _uvList.Add(GeometryConstants.Normals[2]);
                _uvList.Add(GeometryConstants.Normals[0]);
                _uvList.Add(GeometryConstants.Normals[1]);
                _uvList.Add(GeometryConstants.Normals[3]);
                _uvList.Add(GeometryConstants.Normals[2]);

                _colorList.Add(col);
                _colorList.Add(col);
                _colorList.Add(col);
                _colorList.Add(col);
                _colorList.Add(col);
                _colorList.Add(col);

                // Bottom»»
                _vertexList.Add(new Vector3(cx1, cy0, cz1));
                _vertexList.Add(new Vector3(cx0, cy0, cz0));
                _vertexList.Add(new Vector3(cx1, cy0, cz0));

                _vertexList.Add(new Vector3(cx1, cy0, cz1));
                _vertexList.Add(new Vector3(cx0, cy0, cz1));
                _vertexList.Add(new Vector3(cx0, cy0, cz0));

                _normalList.Add(GeometryConstants.Normals[5]);
                _normalList.Add(GeometryConstants.Normals[5]);
                _normalList.Add(GeometryConstants.Normals[5]);
                _normalList.Add(GeometryConstants.Normals[5]);
                _normalList.Add(GeometryConstants.Normals[5]);
                _normalList.Add(GeometryConstants.Normals[5]);

                _uvList.Add(GeometryConstants.Normals[1]);
                _uvList.Add(GeometryConstants.Normals[2]);
                _uvList.Add(GeometryConstants.Normals[0]);
                _uvList.Add(GeometryConstants.Normals[1]);
                _uvList.Add(GeometryConstants.Normals[3]);
                _uvList.Add(GeometryConstants.Normals[2]);

                _colorList.Add(col);
                _colorList.Add(col);
                _colorList.Add(col);
                _colorList.Add(col);
                _colorList.Add(col);
                _colorList.Add(col);

            }
        }

    }

}
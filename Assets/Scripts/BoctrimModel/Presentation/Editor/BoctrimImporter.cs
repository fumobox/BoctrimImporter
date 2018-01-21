using System;
using System.IO;
using Boctrim.Domain;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace Boctrim.Presentation
{

    [ScriptedImporter(1, "boctrim")]
    public class BoctrimImporter : ScriptedImporter
    {
        [SerializeField] float _blockScale = 0.9f;

        [SerializeField] bool _useRegion = true;
        
        [SerializeField] bool _generateCollider = false;
        
        public override void OnImportAsset(AssetImportContext ctx)
        {
            BoctModel model = null;
            
            try
            {
                model = BoctModelImporter.Import(ctx.assetPath);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
            
            var go = new GameObject();
            
            var material = Resources.Load("BoctMaterial") as Material;

            var dir = Path.GetDirectoryName(ctx.assetPath);

            var modelName = string.IsNullOrEmpty(model.Info.Name) ? "BoctModel" : model.Info.Name;
            
            GenerateModel(model, modelName, Resources.Load("BoctMaterial") as Material, dir, go);

            ctx.AddObjectToAsset(modelName, go);
            ctx.AddObjectToAsset("Material", material);
            ctx.SetMainObject(go);
            
            Destroy(go);
        }

        void GenerateModel(BoctModel model, string modelName, Material mat, string assetDir, GameObject target)
        {
            var gen = new BoctMeshGenerator();
            gen.BoctMaterial = mat;
            gen.MaterialList = model.MaterialList;
            gen.SaveMesh = true;
            gen.MeshPath = assetDir;
            gen.BlockScale = _blockScale;
            gen.GenerateCollider = _generateCollider;

            if (_useRegion)
            {
                foreach (var region in model.Regions)
                {
                    var go = new GameObject();
                    go.transform.parent = target.transform;

                    gen.GenerateMesh(region.Value.Head, go, modelName + "_" + region.Value.LUID);
                }
            }
            else
            {
                gen.GenerateMesh(model.Head, target, modelName);
            }

        }

    }

}

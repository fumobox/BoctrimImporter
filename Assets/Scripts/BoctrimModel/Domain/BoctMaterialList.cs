using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using MPF.Infrastructure;
using System.Linq;
using System;
using MPF.Domain;

namespace Boctrim.Domain
{

    public class BoctMaterialList : DomainModel
    {

        Dictionary<int, BoctMaterial> _materials;

        public BoctMaterialList()
        {
            _materials = new Dictionary<int, BoctMaterial>();
        }

        /// <summary>
        /// Clone an instance.
        /// </summary>
        public BoctMaterialList(BoctMaterialList list) : this()
        {
            var materials = list.Materials;

            if (materials == null)
            {
                throw new BoctException("Material is null.");
            }

            foreach (var kv in materials)
            {
                _materials[kv.Key] = new BoctMaterial(kv.Value);
            }
        }

        public BoctMaterialList(MaterialListData data) : this()
        {
            var materialDataList = data.List;

            if (materialDataList == null)
            {
                throw new BoctException("Material data is null.");
            }

            foreach (var materialData in materialDataList)
            {
                _materials[materialData.LUID] = new BoctMaterial(materialData);
            }
        }

        public Dictionary<int, BoctMaterial> Materials
        {
            get
            {
                return _materials;
            }
        }

        public BoctMaterial DefaultMaterial
        {
            get
            {
                return _materials[0];
            }
        }

        public BoctMaterial AddMaterial()
        {
            var mat = new BoctMaterial();
            mat.GUID = Guid.NewGuid();
            mat.LUID = _materials.GetHighestKey<BoctMaterial>() + 1;
            mat.Color = BoctMaterial.DefaultColor;
            mat.ColorChanged = true;
            _materials.Add(mat.LUID, mat);
            return mat;
        }

        public BoctMaterial AddMaterial(BoctMaterial mat)
        {
            _materials.Add(mat.LUID, mat);
            return mat;
        }

        public BoctMaterial AddNewMaterial(BoctMaterial mat)
        {
            var newMaterial = new BoctMaterial(mat);

            newMaterial.GUID = Guid.NewGuid();
            newMaterial.LUID = _materials.GetHighestKey<BoctMaterial>() + 1;
            _materials.Add(newMaterial.LUID, newMaterial);
            return newMaterial;
        }

        public void DisposeMaterial(int id)
        {
            _materials[id].MoveId = BoctMaterial.EmptyId;
        }

        public bool Dirty
        {
            get
            {
                return _materials.Any(m => m.Value.Dirty);
            }
        }

        public override string ToString()
        {
            return _materials.Select(a => a.Value.ToString()).Aggregate((a, b) => a + ", " + b);
        }

        public void ApplyChange()
        {
            var dic = new Dictionary<int, BoctMaterial>();

            foreach (var kv in _materials)
            {
                var mat = kv.Value;
                if (!mat.Disposed)
                {
                    mat.LUID = mat.MoveId;
                    dic.Add(mat.LUID, mat);
                }
                else
                {
                    Debug.Log("Dispose material: " + mat.ToString());
                }
            }

            _materials = dic;
        }

        public MaterialListData ToData()
        {
            var data = new MaterialListData();

            data.List = _materials.Select(m => m.Value.ToData()).ToList();

            return data;
        }

        public BoctMaterial GetRandomMaterial()
        {
            int index = UnityEngine.Random.Range(0, _materials.Count);
            return _materials.Values.ElementAt(index);
        }

        public int GetRandomMaterialId()
        {
            int index = UnityEngine.Random.Range(0, _materials.Count);
            return _materials.Keys.ElementAt(index);
        }

        public BoctMaterial GetMaterial(int id)
        {
            return _materials[id];
        }

        public BoctMaterial GetSameColorMaterial(BoctMaterial mat)
        {
            foreach (var kv in _materials)
            {
                var m = kv.Value;
                if (m.Color.Equals(mat.Color))
                    return m;
            }
            return null;
        }

        public Dictionary<int, int> Merge(BoctMaterialList list)
        {
            var conversionTable = new Dictionary<int, int>();

            foreach (var kv0 in list.Materials)
            {
                var m = kv0.Value;
                var sameMat = GetSameColorMaterial(m);
                if (sameMat == null)
                {
                    var newMat = AddNewMaterial(m);
                    conversionTable.Add(m.LUID, newMat.LUID);
                }
                else
                {
                    conversionTable.Add(m.LUID, m.LUID);
                }
            }

            return conversionTable;
        }

    }

}

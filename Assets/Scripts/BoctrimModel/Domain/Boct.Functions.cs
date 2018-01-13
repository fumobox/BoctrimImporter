using System.Collections.Generic;
using UnityEngine;

namespace Boctrim.Domain
{

    /// <summary>
    /// Basic boct functions
    /// </summary>
    public partial class Boct
    {

        /// <summary>
        /// Initialize a boct.
        /// </summary>
        public void Init()
        {
            Parent = null;
            Children = null;
            MaterialId = BoctMaterial.DefaultId;
            ExtensionId = BoctExtension.EmptyId;
        }

        /// <summary>
        /// Clear the extension.
        /// </summary>
        public void ClearExtension()
        {
            ExtensionId = BoctExtension.EmptyId;
        }

        /// <summary>
        /// Clear the parent.
        /// </summary>
        public void ClearParent()
        {
            if (_parent == null)
                return;

            if (_parent._children == null)
                return;

            for (int i = 0; i < 8; i++)
            {
                var child = _parent._children[i];
                if (child == this)
                {
                    _parent._children[i] = null;
                    break;
                }
            }

            _children = null;
        }

        /// <summary>
        /// Clear children.
        /// </summary>
        public void ClearChildren()
        {
            if (_children == null)
                return;

            foreach (var child in _children)
            {
                if (child == null)
                    continue;

                child._parent = null;
            }

            _children = null;
        }

        /// <summary>
        /// Erace the boct.
        /// </summary>
        public bool Trim()
        {
            // Cannot trim the root boct.
            if (_parent == null) return false;

            Boct[] bc = _parent.Children;
            bool trimParent = true;
            for(int i = 0; i < 8; i++)
            {
                if(bc[i] == this)
                {
                    bc[i] = null;
                }
                else
                {
                    if(bc[i] != null)
                    {
                        trimParent = false;
                    }
                }
            }

            if(trimParent)
            {
                _parent.Trim();
            }

            _parent = null;
            _materialId = BoctMaterial.EmptyId;
            return true;
        }

        /// <summary>
        /// Erace block
        /// </summary>
        /// <remarks>
        /// Faster than Ttim().
        /// </remarks>
        public void FastTrim()
        {
            Boct[] bc = _parent._children;
            for (int i = 0; i < 8; i++)
            {
                if (bc[i] == this)
                {
                    bc[i] = null;
                    return;
                }
            }
        }

        /// <summary>
        /// Erace boct.
        /// </summary>
        /// <remarks>
        /// Faster than Ttim().
        /// </remarks>
        public void FastTrim(int index)
        {
            _parent._children[index] = null;
        }

        /// <summary>
        /// Return a value indicating whether the boct can trim.
        /// </summary>
        public bool CanTrim()
        {
            // Cannot trim the root boct.
            if (_parent == null) return false;

            return true;
        }

        /// <summary>
        /// Erace bocts.
        /// </summary>
        public bool TrimChild(int[] indexArray)
        {
            bool res = true;
            for (int i = 0; i < indexArray.Length; i++)
            {
                res &= TrimChild(indexArray[i]);
            }
            return res;
        }

        /// <summary>
        /// Erace boct.
        /// </summary>
        public bool TrimChild(int index)
        {
            if (_children == null)
                return false;
            if (_children[index] == null)
                return false;
            return _children[index].Trim();
        }

        /// <summary>
        /// Erace the area of boct.
        /// </summary>
        public bool TrimArea(Direction d)
        {
            if (_children == null)
                return false;

            int[] arr = null;

            switch (d)
            {
                case Direction.South:
                    arr = new int[] { 1, 2, 5, 6};
                    break;
                case Direction.East:
                    arr = new int[] { 2, 3, 6, 7 };
                    break;
                case Direction.North:
                    arr = new int[] { 0, 3, 4, 7 };
                    break;
                case Direction.West:
                    arr = new int[] { 0, 1, 4, 5 };
                    break;
                case Direction.Top:
                    arr = new int[] { 0, 1, 2, 3 };
                    break;
                case Direction.Bottom:
                    arr = new int[] { 4, 5, 6, 7 };
                    break;
            }

            TrimChild(arr);

            return true;
        }

        /// <summary>
        /// Divide the boct.
        /// </summary>
        public bool Divide(bool spawn = true) 
        {
            if(Children == null)
            {
                Children = new Boct[8];
                if(spawn)
                {
                    for(int i = 0; i < 8; i++)
                    {
                        Children[i] = new Boct(this);
                        Children[i].MaterialId = MaterialId;
                        Children[i].RegionId = RegionId;
                    }
                }
                MaterialId = BoctMaterial.EmptyId;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Divide the boct.
        /// </summary>
        public List<Boct> Divide(bool[] spawnList)
        {
            if (Children == null)
            {
                var list = new List<Boct>();
                Children = new Boct[8];
                for (int i = 0; i < 8; i++)
                {
                    if (spawnList[i])
                    {
                        Children[i] = new Boct(this);
                        Children[i].MaterialId = MaterialId;
                        Children[i].RegionId = RegionId;
                        list.Add(Children[i]);
                    }
                }
                MaterialId = BoctMaterial.EmptyId;
                return list;
            }
            else
            {
                Debug.LogWarning("Children is not null.");
                return new List<Boct>();
            }
        }

        /// <summary>
        /// Return a value indicating whether the boct can divide.
        /// </summary>
        public bool CanDivide()
        {
            return Children == null;
        }
        
    }
}

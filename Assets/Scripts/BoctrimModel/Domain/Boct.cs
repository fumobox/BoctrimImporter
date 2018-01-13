using System.Collections.Generic;

namespace Boctrim.Domain
{

    /// <summary>
    /// A block that can be divided into 8 blocks.
    /// </summary>
    public partial class Boct
    {
        Boct _parent = null;
    
        Boct[] _children = null;
    
        // If there has no material, the boct is invisible.
        int _materialId = BoctMaterial.EmptyId;
    
        int _extensionId = BoctExtension.EmptyId;

        int _regionId = BoctRegion.EmptyId;

        /// <summary>
        /// Create an empty boct.
        /// </summary>
        public Boct()
        {
        }
    
        public Boct(Boct parent)
        {
            _parent = parent;
        }
    
        /// <summary>
        /// Get a root boct.
        /// </summary>
        /// <value>The root</value>
        public Boct Root
        {
            get 
            {
                Boct b = this;
                while(b._parent != null)
                {
                    b  = b._parent;
                }
                return b;
            }
        }

        /// <summary>
        /// Return a value indicating whether the boct is root.
        /// </summary>
        public bool IsRoot
        {
            get 
            {
                return _parent != null;
            }
        }

        /// <summary>
        /// Get a parent boct.
        /// </summary>
        public Boct Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
            }
        }
        
        /// <summary>
        /// Return a value indicating whether the boct has a parent.
        /// </summary>
        public bool HasParent
        {
            get
            {
                return _parent != null;
            }
        }

        /// <summary>
        /// Return a value indicating whether the boct has a child.
        /// </summary>
        public bool HasChild 
        {
            get 
            {
                return _children != null;
            }
        }
    
        /// <summary>
        /// Get children bocts.
        /// </summary>
        public Boct[] Children
        {
            get
            {
                return _children;
            }
            set
            {
                _children = value;
            }
        }

        #region Address
        
        /// <summary>
        /// Get an address from root boct.
        /// </summary>
        public List<byte> Address
        {
            get
            {
                var list = new List<byte>();
                Boct b = this;
                int localAddress = b.LocalAddress;
                while(localAddress != -1)
                {
                    b = b._parent;
                    list.Add((byte)localAddress);
                    localAddress = b.LocalAddress;
                }
                list.Reverse();
                return list;
            }
        }
    
        /// <summary>
        /// Get a string expression of address.
        /// </summary>
        public string AddressString
        {
            get
            {
                return BoctAddressTools.ToString(Address);
            }
        }

        /// <summary>
        /// Gen a local address.
        /// </summary>
        public int LocalAddress
        {
            get 
            {
                if(_parent == null)
                {
                    return -1;
                }
                else
                {
                    Boct[] bc = _parent._children;
                    for(int i = 0; i < 8; i++)
                    {
                        if(bc[i] == this)
                        {
                            return i;
                        }
                    }
                    return -1;
                }
            }
        }

        #endregion
        
        #region Material
        
        /// <summary>
        /// Get a boct Material
        /// </summary>
        public int MaterialId
        {
            get
            {
                return _materialId;
            }
            set
            {
                if (value == BoctMaterial.EmptyId)
                {
                    _materialId = value;
                    return;
                }

                if (_children != null)
                    throw new BoctException();
                
                _materialId = value;
            }
        }

        /// <summary>
        /// Return a value indicating whether the boct has a material.
        /// </summary>
        public bool HasMaterial
        {
            get
            {
                return _materialId != BoctMaterial.EmptyId;
            }
        }
        
        #endregion
        
        #region Extension
        
        /// <summary>
        /// Get an extension ID.
        /// </summary>
        public int ExtensionId
        {
            get
            {
                return _extensionId;
            }
            set
            {
                _extensionId = value;
            }
        }

        #endregion
        
        #region Region

        /// <summary>
        /// Get a region ID.
        /// </summary>
        public int RegionId
        {
            get
            {
                return _regionId;
            }
            set
            {
                _regionId = value;
            }
        }

        /// <summary>
        /// Return a value indicating whether the boct has a region.
        /// </summary>
        public bool HasRegion
        {
            get
            {
                return _regionId != BoctRegion.EmptyId;
            }
        }
        
        /// <summary>
        /// Return a value indicating whether the boct is a head of region.
        /// </summary>
        public bool IsRegionHead
        {
            get
            {
                if (_regionId == BoctRegion.EmptyId)
                    return false;

                if (_parent == null)
                    return true;

                if (_parent._regionId == BoctRegion.EmptyId)
                    return true;
                else
                    return false;
            }
        }
        
        #endregion

        #region Other Properties
        
        /// <summary>
        /// Get a solid count.
        /// </summary>
        public int SolidCount 
        {
            get
            {
                Boct b = this;
                Boct[] bc = b._children;
                if(bc == null)
                {
                    return 1;
                }
                else
                {
                    int count = 0;
                    for(int i = 0; i < 8; i++)
                    {
                        if (bc[i] != null)
                        {
                            count += bc[i].SolidCount;
                        }
                    }
                    return count;
                }
            }
        }

        /// <summary>
        /// Get a depth of boct.
        /// </summary>
        /// <remarks>
        /// The depth of the root is 0.
        /// </remarks>
        public int Depth
        {
            get
            {
                int depth = 0;
                Boct b = this;
                while(b._parent != null)
                {
                    b = b._parent;
                    depth++;
                }
                return depth;
            }
        }
        
        public override string ToString()
        {
            return "[B] " + BoctAddressTools.ToString(Address, true);
        }
        
        #endregion
    }

}
using System;
using System.Collections.Generic;

namespace Boctrim.Domain
{

    public class BoctModelData: BoctrimData
    {
        public BoctModelInfo Info { get; set;}

        List<RegionData> _regions;

        public List<RegionData> Regions
        {
            get
            {
                if (_regions == null)
                    _regions = new List<RegionData>();
                return _regions;
            }
            set
            {
                _regions = value;
            }
        }

        MaterialListData _materialList;

        public MaterialListData MaterialList
        {
            get
            {
                if (_materialList == null)
                    _materialList = new MaterialListData();
                return _materialList;
            }
            set
            {
                _materialList = value;
            }
        }

        ExtensionListData _extensionList;

        public ExtensionListData ExtensionList
        {
            get
            {
                if (_extensionList == null)
                    _extensionList = new ExtensionListData();
                return _extensionList;
            }
            set
            {
                _extensionList = value;
            }
        }

    }

}
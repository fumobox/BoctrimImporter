using System.Collections.Generic;
using MPF.Domain;
using UniRx;
using UnityEngine;

namespace Boctrim.Domain
{

    /// <summary>
    /// Boct Model
    /// </summary>
    public partial class BoctModel: DomainModel
    {

        public const int InitialRegionIndex = 10000;

        // Max value is 1024.
        public const int DefaultRegionSize = 1024;

        Boct _head;

        int _regionSize = DefaultRegionSize;

        int _regionIndex = InitialRegionIndex;

        Dictionary<int, BoctRegion> _regions;

        BoctMaterialList _materialList;

        BoctExtensionList _extensionList;

        public List<int> NewRegionIdList { get; set;}

        public List<byte> CenterAddress { get; private set; }

        public Subject<BoctRegion> RegionStream { get; private set;}

        public Bounds SolidBounds { get; private set;}

        public BoctModel()
        {
            _head = new Boct();
            _regions = new Dictionary<int, BoctRegion>();
            _materialList = new BoctMaterialList();
            _extensionList = new BoctExtensionList();
            CenterAddress = new List<byte>();

            RegionStream = new Subject<BoctRegion>();

            SolidBounds = new Bounds(Vector3.zero, Vector3.one);
        }

        public BoctModel(BoctModelData data): this()
        {
            GUID = data.GUID;

            Info = data.Info;

            foreach (var region in data.Regions)
                AddRegion(region);

            _materialList = new BoctMaterialList(data.MaterialList);

            UpdateSolidBounds();
        }

        public BoctModelInfo Info { get; set;}

        public Boct Head
        {
            get
            {
                return _head;
            }
            set
            {
                _head = value;
            }
        }

        public Dictionary<int, BoctRegion> Regions
        {
            get
            {
                return _regions;
            }
        }

        public int RegionIndex
        {
            get
            {
                return _regionIndex;
            }
            set
            {
                _regionIndex = value;
            }
        }

        public int RegionSize
        {
            get
            {
                return _regionSize;
            }
            set
            {
                _regionSize = value;
            }
        }

        public BoctMaterialList MaterialList
        {
            get
            {
                return _materialList;
            }
            set
            {
                _materialList = value;
            }
        }

        public BoctExtensionList ExtensionList
        {
            get
            {
                return _extensionList;
            }
        }

    }

}
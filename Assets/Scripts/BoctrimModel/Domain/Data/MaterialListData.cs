using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Boctrim.Domain
{

    public class MaterialListData: BoctrimData
    {
        List<MaterialData> _materialList = null;

        public List<MaterialData> List
        {
            get
            {
                if (_materialList == null)
                    _materialList = new List<MaterialData>();
                return _materialList;
            }
            set
            {
                _materialList = value;
            }
        }

        [YamlIgnore]
        public int Count
        {
            get
            {
                if (_materialList.Count == 0)
                    return 0;

                return _materialList.Count;
            }
        }

    }

}

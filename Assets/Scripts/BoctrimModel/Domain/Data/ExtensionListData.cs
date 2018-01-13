using System.Collections;
using System;
using System.Collections.Generic;

namespace Boctrim.Domain
{

    public class ExtensionListData: BoctrimData
    {
        List<ExtensionData> _extensionList = null;

        public List<ExtensionData> List
        {
            get
            {
                if (_extensionList == null)
                    _extensionList = new List<ExtensionData>();
                return _extensionList;
            }
            set
            {
                _extensionList = value;
            }
        }
    }

}

using System.Collections.Generic;
using System.Linq;
using MPF.Domain;

namespace Boctrim.Domain
{

    public class BoctExtensionList : DomainModel
    {

        Dictionary<int, BoctExtension> _extensions;

        public BoctExtensionList()
        {
            _extensions = new Dictionary<int, BoctExtension>();
        }

        /// <summary>
        /// Clone an instance.
        /// </summary>
        public BoctExtensionList(BoctExtensionList list): this()
        {
            var extensions = list.Extensions;

            if (extensions == null)
            {
                throw new BoctException("Extenson is null.");
            }

            foreach (var kv in extensions)
            {
                _extensions[kv.Key] = new BoctExtension(kv.Value);
            }
        }

        public BoctExtensionList(ExtensionListData data): this()
        {
            var extensionDataList = data.List;
            
            if (extensionDataList == null)
            {
                throw new BoctException("Extension data is null.");
            }
            
            foreach (var extensionData in extensionDataList)
            {
                _extensions[extensionData.LUID] = new BoctExtension(extensionData);
            }
        }

        public Dictionary<int, BoctExtension> Extensions
        {
            get
            {
                return _extensions;
            }
        }
            
        public ExtensionListData ToData()
        {
            var data = new ExtensionListData();

            data.List = _extensions.Select(m => m.Value.ToData()).ToList();

            return data;
        }

    }

}

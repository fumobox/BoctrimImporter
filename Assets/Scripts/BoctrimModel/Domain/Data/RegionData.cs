using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Boctrim.Domain
{

    public class RegionData: BoctrimData
    {
        public int LUID { get; set;}

        public string HeadAddress { get; set;}

        public int HeadMaterialId { get; set;}

        public int HeadExtensionId { get; set;}

        public List<string> AddressList { get; set;}

        public List<int> MaterialIdList { get; set;}

        public List<int> ExtensionIdList { get; set;}

        public RegionData()
        {
            HeadMaterialId = BoctMaterial.EmptyId;
            HeadExtensionId = BoctExtension.EmptyId;
            AddressList = new List<string>();
            MaterialIdList = new List<int>();
            ExtensionIdList = new List<int>();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("[Region Data]\n");
            sb.Append("GUID: " + GUID + "\n");
            sb.Append("LUID: " + LUID + "\n");
            sb.Append("Head Address: " + (string.IsNullOrEmpty(HeadAddress) ? "Empty": HeadAddress) + "\n");
            sb.Append("Head Material ID: " + HeadMaterialId + "\n");
            sb.Append("Address: " + AddressList.DefaultIfEmpty("Empty").Aggregate((a, b) => a + ", " + b) + "\n");
            sb.Append("Material: " + MaterialIdList.Select(a => a.ToString()).DefaultIfEmpty("Empty").Aggregate((a, b) => a + ", " + b) + "\n");
            sb.Append("Extension: " + ExtensionIdList.Select(a => a.ToString()).DefaultIfEmpty("Empty").Aggregate((a, b) => a + ", " + b));
            return sb.ToString();
        }

    }

}
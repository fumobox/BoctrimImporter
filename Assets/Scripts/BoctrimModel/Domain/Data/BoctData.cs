using System.Collections.Generic;

namespace Boctrim.Domain
{

    // NOTE 

    // 6[100000]215632451
    // 6+
    // 

    public class BoctData
    {

        public string address; 

        public int mid;

        public BoctData(Boct b)
        {
            address = BoctAddressTools.ToString(b.Address);
            mid = b.MaterialId;
        }

        public string ToDebugString()
        {
            return address + " " + mid;
        }

    }
}
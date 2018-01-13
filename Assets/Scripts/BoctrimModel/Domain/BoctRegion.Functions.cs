namespace Boctrim.Domain
{

    public partial class BoctRegion
    {

        /// <summary>
        /// Dispose the region and clean up referenced boct data.
        /// </summary>
        public void Dispose(bool hard)
        {
            if (hard)
            {
                Head.ClearParent();
                Head.ClearChildren();
                Head.MaterialId = BoctMaterial.EmptyId;
                Head.ExtensionId = BoctExtension.EmptyId;
            }
            BoctTools.ClearRegionId(Head);
            Head = null;
            CurrentState.Value = State.Disposed;
        }

        public void SetDirty()
        {
            CurrentState.Value = State.Dirty;
        }

        /// <summary>
        /// Convert to data.
        /// </summary>
        public RegionData ToData()
        {
            var data = new RegionData ();

            data.GUID = GUID;

            data.LUID = LUID;

            var headAddress = Head.Address;
            var headAddressLength = headAddress.Count;

            data.HeadAddress = BoctAddressTools.ToString (headAddress);

            if (Head.HasMaterial)
            {
                // If the head has material, the region is a single block.
                data.HeadMaterialId = Head.MaterialId;
            }
            else
            {
                var list = BoctTools.GetSolidBoctArray(Head);

                for (int i = 0; i < list.Length; i++)
                {
                    var b = list[i];
                    var childAddress = b.Address;
                    // Trim headAddress from child address.
                    var localAddress = childAddress.GetRange(headAddressLength, childAddress.Count - headAddressLength);
                    data.AddressList.Add(BoctAddressTools.ToString(localAddress));
                    data.MaterialIdList.Add(b.MaterialId);
                    data.ExtensionIdList.Add(b.ExtensionId);
                }
            }

            data.HeadExtensionId = Head.ExtensionId;

            return data;
        }

        public bool Contains(Direction d)
        {
            var address = Head.Address;

            if (address.Count == 0)
            {
                return false;
            }

            switch (address[0])
            {
                case 0:
                    if (d == Direction.North || d == Direction.West || d == Direction.Top)
                        return true;
                    break;
                case 1:
                    if (d == Direction.South || d == Direction.West || d == Direction.Top)
                        return true;
                    break;
                case 2:
                    if (d == Direction.South || d == Direction.East || d == Direction.Top)
                        return true;
                    break;
                case 3:
                    if (d == Direction.North || d == Direction.East || d == Direction.Top)
                        return true;
                    break;
                case 4:
                    if (d == Direction.North || d == Direction.West || d == Direction.Bottom)
                        return true;
                    break;
                case 5:
                    if (d == Direction.South || d == Direction.West || d == Direction.Bottom)
                        return true;
                    break;
                case 6:
                    if (d == Direction.South || d == Direction.East || d == Direction.Bottom)
                        return true;
                    break;
                case 7:
                    if (d == Direction.North || d == Direction.East || d == Direction.Bottom)
                        return true;
                    break;
            }

            return false;
        }

    }

}

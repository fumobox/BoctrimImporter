using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using MPF.Infrastructure;
using UnityEngine;

namespace Boctrim.Domain
{

    /// <summary>
    /// Boct model functions
    /// </summary>
    public partial class BoctModel
    {

        /// <summary>
        /// Scales up the head boct.
        /// </summary>
        /// <returns>An old head address</returns>
        /// <param name="iteration">Scale up iteration</param>
        public List<byte> ScaleUp(int iteration = 1)
        {
            var newHead = new Boct();
            newHead.Divide(false);
            newHead.Children[0] = new Boct(newHead);
            Boct b = newHead.Children[0];
            for(int i = 0; i < iteration; i++)
            {
                b.Divide(false);
                b.Children[6] = new Boct(b);
                b = b.Children[6];
            }
            b.Parent.Children[6] = Head;
            Head.Parent = b.Parent;
            var address = Head.Address;
            Head = newHead;

            foreach (var region in Regions)
            {
                region.Value.SetDirty();
            }

            return address;
        }

        #region Expand

        public ExpandResult Expand(Boct boct)
        {
            if (!boct.HasMaterial)
            {
                return ExpandResult.Failed;
            }

            if (!boct.HasParent)
            {
                return ExpandResult.Failed;
            }

            var rid = boct.RegionId;

            bool outsideRegion = false;
            
            if(boct.Parent.RegionId != boct.RegionId)
            {
                var dic = new Dictionary<int, int>();
                var children = boct.Parent.Children;
                foreach (var child in children)
                {
                    if (child != null && child != boct)
                    {
                        BoctTools.GetRegionBoctCount(child, dic);
                    }
                }

                foreach (var d in dic)
                {
                    var id = d.Key;
                    if(id != BoctRegion.EmptyId)
                    {
                        _regions[id].Dispose(true);
                    }
                }

                _regions[rid].Head = boct.Parent;

                outsideRegion = true;
            }

            boct.ClearChildren();
            var parent = boct.Parent;
            boct.Parent.ClearChildren();
            parent.MaterialId = boct.MaterialId;
            parent.RegionId = boct.RegionId;
            parent.ExtensionId = boct.ExtensionId;
            _regions[rid].SetDirty();

            return outsideRegion ? ExpandResult.OutsideRegion: ExpandResult.InsideRegion;
        }

        public List<int> GetExpandInflictRegionList(Boct boct)
        {
            var list = new List<int>();
            var dic = new Dictionary<int, int>();
            var children = boct.Parent.Children;

            foreach (var child in children)
            {
                if (child != null && child != boct)
                {
                    BoctTools.GetRegionBoctCount(child, dic);
                }
            }

            foreach (var d in dic)
            {
                var rid = d.Key;
                // Ignore the empty ID and the self ID.
                if (rid != BoctRegion.EmptyId && rid != boct.RegionId)
                {
                    list.Add(rid);
                }
            }

            return list;
        }

        #endregion

        public void InitRegion()
        {
            Regions.Clear();
            RegionIndex = 10000;
            UpdateRegionR(Head);
        }

        public void InitRegion(Direction d)
        {
            if(!Head.HasChild)
            {
                throw new BoctException("Children are null.");
            }

            switch (d)
            {
                case Direction.South:
                    UpdateRegionR(Head.Children[1]);
                    UpdateRegionR(Head.Children[2]);
                    UpdateRegionR(Head.Children[5]);
                    UpdateRegionR(Head.Children[6]);
                    break;
                case Direction.East:
                    UpdateRegionR(Head.Children[2]);
                    UpdateRegionR(Head.Children[3]);
                    UpdateRegionR(Head.Children[6]);
                    UpdateRegionR(Head.Children[7]);
                    break;
                case Direction.North:
                    UpdateRegionR(Head.Children[0]);
                    UpdateRegionR(Head.Children[3]);
                    UpdateRegionR(Head.Children[4]);
                    UpdateRegionR(Head.Children[7]);
                    break;
                case Direction.West:
                    UpdateRegionR(Head.Children[0]);
                    UpdateRegionR(Head.Children[1]);
                    UpdateRegionR(Head.Children[4]);
                    UpdateRegionR(Head.Children[5]);
                    break;
                case Direction.Top:
                    UpdateRegionR(Head.Children[0]);
                    UpdateRegionR(Head.Children[1]);
                    UpdateRegionR(Head.Children[2]);
                    UpdateRegionR(Head.Children[3]);
                    break;
                case Direction.Bottom:
                    UpdateRegionR(Head.Children[4]);
                    UpdateRegionR(Head.Children[5]);
                    UpdateRegionR(Head.Children[6]);
                    UpdateRegionR(Head.Children[7]);
                    break;
            }
        }

        void AddRegion(Boct head, Guid? guid = null, int? regionId = null)
        {
            var region = new BoctRegion();

            region.GUID = guid ?? Guid.NewGuid();

            region.LUID = regionId ?? RegionIndex;

            BoctTools.FillRegionId(head, region.LUID);
            region.Head = head;

            Regions.Add(region.LUID, region);
            if(NewRegionIdList != null)
                NewRegionIdList.Add(RegionIndex);

            if(regionId == null)
                RegionIndex++;

            region.CurrentState.Subscribe(state =>
                {
                    switch(state)
                    {
                        case BoctRegion.State.Ready:
                            break;
                        case BoctRegion.State.Disposed:
                            Regions.Remove(region.LUID);
                            break;
                        case BoctRegion.State.Dirty:
                            region.MaterialCounts = BoctMaterialTools.CountMaterials(region.Head, MaterialList.Materials);
                            break;
                    }
                });

            // Call last time.
            RegionStream.OnNext(region);
        }

        public void AddRegion(RegionData data)
        {
            var headAddress = BoctAddressTools.FromString(data.HeadAddress);

            var head = Find(headAddress, true);
            if (head == null)
            {
                throw new BoctException("Could not found the boct.");
            }

            if (head.RegionId != BoctRegion.EmptyId)
            {
                throw new BoctException("RegionId is not empty: " + head.RegionId);
            }

            head.ClearChildren();

            if (data.HeadMaterialId == BoctMaterial.EmptyId)
            {
                for (int i = 0; i < data.AddressList.Count; i++)
                {
                    var byteAddress = BoctAddressTools.FromString(data.HeadAddress + data.AddressList[i]);
                    BoctTools.InsertBoct(byteAddress, _head, data.MaterialIdList[i]);
                }
            }
            else
            {
                head.MaterialId = data.HeadMaterialId;
            }

            head.ExtensionId = data.HeadExtensionId;

            AddRegion(head, data.GUID, data.LUID);
        }

        void UpdateRegionR(Boct b)
        {
            if (b == null)
            {
                Debug.LogWarning("Boct is null.");
                return;
            }

            if (BoctTools.LargerThan(b, RegionSize))
            {
                b.RegionId = BoctRegion.EmptyId;
                if (b.HasChild)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (b.Children[i] != null)
                            UpdateRegionR(b.Children[i]);
                    }
                }
            }
            else
            {
                AddRegion(b);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="regionId">Region ID</param>
        public void SplitRegion(int regionId)
        {
            var region = Regions[regionId];

            var regionHead = region.Head;

            // Dispose the region.
            region.Dispose(false);

            UpdateRegionR(regionHead);
        }

        public void SetDirtyAllRegions()
        {
            foreach(var kv in _regions)
            {
                kv.Value.SetDirty();
            }
        }

        public InsertResult Insert(List<byte> address, int mid, bool setDirty = true)
        {
            return Insert(address, Head, mid, setDirty);
        }

        public InsertResult Insert(List<byte> address, Boct head, int mid, bool setDirty = true)
        {
            if(address == null)
            {
                return InsertResult.Failed;
            }

            Boct target = head;

            int insideRegionId = BoctRegion.EmptyId;
            bool insideRegion = false;
            int insideRegionSize = -1;

            // Insert bocts
            for (int i = 0; i < address.Count; i++)
            {
                if(target.HasMaterial)
                {
                    return InsertResult.Failed;
                }
                else
                {
                    // If new region is detected, then 
                    if (!insideRegion && target.RegionId != BoctRegion.EmptyId)
                    {
                        insideRegion = true;
                        insideRegionId = target.RegionId;
                        insideRegionSize = target.SolidCount;
                    }

                    byte b = address[i];
                    if (target.Children == null)
                    {
                        target.Children = new Boct[8];
                        target.Children[b] = new Boct(target);
                        target = target.Children[b];
                        target.RegionId = insideRegionId;
                    }
                    else
                    {
                        if (target.Children[b] == null)
                        {
                            target.Children[b] = new Boct(target);
                            target = target.Children[b];
                        }
                        else
                        {
                            target = target.Children[b];
                        }
                    }
                }
            }

            // Update the region.
            if(target.Children == null && target.MaterialId == BoctMaterial.EmptyId)
            {
                target.MaterialId = mid;
                target.RegionId = insideRegionId;
                if (insideRegionId == BoctRegion.EmptyId)
                {
                    // Create new region.
                    AddRegion(target);

                    return InsertResult.OutsideRegion;
                }
                else
                {
                    // If the region is overflow, then split the region.
                    if (insideRegionSize > RegionSize)
                    {
                        var regionHead = Regions[insideRegionId].Head;

                        // Dispose the current region.
                        Regions[insideRegionId].Dispose(false);

                        // Create new regions.
                        UpdateRegionR(regionHead);

                        return InsertResult.OverflowRegion;
                    }
                    else
                    {
                        // Keep the current region.
                        if(setDirty)
                            Regions[insideRegionId].SetDirty();
                        return InsertResult.InsideRegion;
                    }
                }
            }
            else
            {
                return InsertResult.Failed;
            }
        }

        public Boct Find(List<byte> address, bool create = false)
        {
            var target = _head;
            foreach (var b in address)
            {
                if (target.HasMaterial)
                {
                    return null;
                }
                else
                {
                    if (target.HasChild)
                    {
                        if (target.Children[b] == null)
                        {
                            if (create)
                            {
                                var boct = new Boct(target);
                                boct.RegionId = target.RegionId;
                                target.Children[b] = boct;
                                target = boct;
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            target = target.Children[b];
                        }
                    }
                    else
                    {
                        if (create)
                        {
                            target.Divide(false);
                            var boct = new Boct(target);
                            boct.RegionId = target.RegionId;
                            target.Children[b] = boct;
                            target = boct;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            return target;
        }

        public BoctModelData ToData()
        {
            var data = new BoctModelData ();
            data.GUID = GUID;
            data.Info = Info;
            data.Regions = Regions.Select (r => r.Value.ToData ()).ToList ();
            //data.Scale = Scale;
            data.MaterialList = MaterialList.ToData();

            return data;
        }

        /// <summary>
        /// Update a bounds of solid bocts.
        /// </summary>
        /// <param name="scanBoct"></param>
        public void UpdateSolidBounds(bool scanBoct = true)
        {
            SolidBounds = BoctTools.GetBounds(this, scanBoct);
            //Kenny.D("Bounds: " + SolidBounds.ToString("F4"));
        }

    }

    /// <summary>
    /// Insert operation result
    /// </summary>
    public enum InsertResult
    {
        Failed, OverflowRegion, OutsideRegion, InsideRegion
    }

    /// <summary>
    /// Expand operation result
    /// </summary>
    public enum ExpandResult
    {
        Failed, OutsideRegion, InsideRegion
    }

}

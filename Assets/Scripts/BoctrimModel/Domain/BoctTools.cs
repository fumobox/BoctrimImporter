using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.Assertions;

namespace Boctrim.Domain
{

    public static class BoctTools
    {

        static int __param0, __param1;

        static bool __flag0;

        static readonly byte[] MirrorConvertTableEW = { 3, 2, 1, 0, 7, 6, 5, 4 };
        static readonly byte[] MirrorConvertTableTB = { 4, 5, 6, 7, 0, 1, 2, 3 };
        static readonly byte[] MirrorConvertTableSN = { 1, 0, 3, 2, 5, 4, 7, 6 };

        static readonly byte[,] RotateTableX = {
            { 4, 0, 3, 7, 5, 1, 2, 6 },
            { 5, 4, 7, 6, 1, 0, 3, 2 },
            { 1, 5, 6, 2, 0, 4, 7, 3 } };
        static readonly byte[,] RotateTableY = { 
            { 3, 0, 1, 2, 7, 4, 5, 6 },
            { 2, 3, 0, 1, 6, 7, 4, 5 },
            { 1, 2, 3, 0, 5, 6, 7, 4 } };
        static readonly byte[,] RotateTableZ = { 
            { 3, 2, 6, 7, 0, 1, 5, 4 }, 
            { 7, 6, 5, 4, 3, 2, 1, 0 }, 
            { 4, 5, 1, 0, 7, 6, 2, 3 } };

        static BoctTools()
        {
        }

        public static void Trace(Boct b)
        {
            if (b.HasChild)
            {
                // Debug.Log("Address: " + BoctAddressTools.ToString(b.Address, true) + " " + "E");
                Boct[] bc = b.Children;
                for (int i = 0; i < 8; i++)
                {
                    if (bc[i] != null)
                    {
                        Trace(bc[i]);
                    }
                }
            }
            else
            {
                // Debug.Log("Address: " + BoctAddressTools.ToString(b.Address, true) + " " + "S");	
            }
        }

        public static int CountSolid(Boct b)
        {
            if (b.HasMaterial)
            {
                return 1;
            }
            else
            {
                if (b.HasChild)
                {
                    int cnt = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        if (b.Children[i] != null)
                        {
                            cnt += CountSolid(b.Children[i]);
                        }
                    }
                    return cnt;
                }
                else
                {
                    return 0;
                }
            }
        }

        public static int CountSolid(Boct b, int depth)
        {
            if (b.HasMaterial)
            {
                return 1;
            }
            else
            {
                depth--;
                if (depth < 0)
                {
                    return 0;
                }

                if (b.HasChild)
                {
                    int cnt = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        if (b.Children[i] != null)
                        {
                            cnt += CountSolid(b.Children[i], depth);
                        }
                    }
                    return cnt;
                }
                else
                {
                    return 0;
                }
            }
        }

        static int[] _countList;
        static int _maxDepth = 0;

        public static int[] CountSolid2(Boct b)
        {
            const int size = 100;
            _countList = new int[size];
            ReportR(b, 0);
            _countList[_maxDepth + 1] = -1;
            var list = _countList;
            _countList = null;
            return list;
        }

        public static bool LargerThan(Boct b, int num)
        {
            __param0 = num;
            __param1 = 0;
            LargerThanR(b);
            return __param1 > __param0;
        }

        static void LargerThanR(Boct b)
        {
            if (b.HasMaterial)
            {
                __param1++;
            }
            else
            {
                if (__param1 > __param0)
                {
                    return;
                }

                if (b.HasChild)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (b.Children[i] != null)
                        {
                            LargerThanR(b.Children[i]);
                        }
                    }
                }
            }
        }

        static void ReportR(Boct b, int depth)
        {
            if (b.HasMaterial)
            {
                _countList[depth]++;
                _maxDepth = depth;
            }
            else
            {
                if (b.HasChild)
                {
                    depth++;
                    for (int i = 0; i < 8; i++)
                    {
                        if (b.Children[i] != null)
                            ReportR(b.Children[i], depth);
                    }
                }
            }
        }

        /// <summary>
        /// Optimize Boct
        /// </summary>
        public static void Optimize(Boct b)
        {
            var list = GetSolidBoctArray(b);
            foreach (var b1 in list)
            {
                OptimizeBoct(b1);
            }
        }

        static void OptimizeBoct(Boct b)
        {
            if (!b.HasParent)
                return;

            int mid = b.MaterialId;
            Boct[] bros = b.Parent.Children;
            int sameMaterialCount = 0;

            for (int j = 0; j < 8; j++)
            {
                if (bros[j] != null && bros[j].MaterialId == mid)
                {
                        sameMaterialCount++;
                }
            }

            // Merge Boct
            if (sameMaterialCount == 8)
            {
                Boct parent = b.Parent;
                parent.Children = null;
                parent.MaterialId = mid;
                for (int j = 0; j < 8; j++)
                {
                    if (bros[j] != null)
                        bros[j].Parent = null;
                }
                OptimizeBoct(parent);
            }
        }

        // TODO
        public static void Validate(Boct b)
        {
            if (b.HasMaterial && b.HasChild)
                Debug.LogWarning("C M");
        }

        public static List<BoctData> ToDataArray(Boct target)
        {
            var res = new List<BoctData>();
            var scanList = new List<Boct>();
            scanList.Add(target);

            while (true)
            {
                var nextList = new List<Boct>();
                foreach (var b in scanList)
                {
                    if (b.HasMaterial)
                    {
                        res.Add(new BoctData(b));
                    }
                    else
                    {
                        if (b.HasChild)
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                if (b.Children[i] != null)
                                    nextList.Add(b.Children[i]);
                            }
                        }
                    }
                }
                if (nextList.Count == 0)
                    break;
                scanList = nextList;
            }
            return res;
        }

        public static List<Boct> GetSolidBoctList(Boct target)
        {
            var res = new List<Boct>();
            var scanList = new List<Boct>();
            scanList.Add(target);

            while (true)
            {
                var nextList = new List<Boct>();
                foreach (var b in scanList)
                {
                    if (b.HasMaterial)
                    {
                        res.Add(b);
                    }
                    else
                    {
                        if (b.HasChild)
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                if (b.Children[i] != null)
                                    nextList.Add(b.Children[i]);
                            }
                        }
                    }
                }
                if (nextList.Count == 0)
                    break;
                scanList = nextList;
            }
            return res;
        }

        public static Boct[] GetSolidBoctArray(Boct target)
        {
            return GetSolidBoctList(target).ToArray();
        }

        public static List<Boct> GetSolidBoctList(Boct target, int depth)
        {
            var res = new List<Boct>();
            var scanList = new List<Boct>();
            scanList.Add(target);

            while (depth >= 0)
            {
                var nextList = new List<Boct>();
                foreach (var b in scanList)
                {
                    if (b.HasMaterial)
                    {
                        res.Add(b);
                    }
                    else
                    {
                        if (b.HasChild)
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                if (b.Children[i] != null)
                                    nextList.Add(b.Children[i]);
                            }
                        }
                    }
                }
                if (nextList.Count == 0)
                    break;
                scanList = nextList;
                depth--;
            }
            return res;
        }

        public static Boct[] GetSolidBoctArray(Boct target, int depth)
        {
            return GetSolidBoctList(target, depth).ToArray();
        }

        public static Dictionary<int, List<Boct>> GetHierarchizedSolidBoctList(Boct target, int depth)
        {
            // Debug.Log("GetSolidBoctList: depth=" + depth);

            var res = new Dictionary<int, List<Boct>>();
            var scanList = new List<Boct>();
            scanList.Add(target);

            for(int d = 0; d <= depth; d++)
            {
                res.Add(d, new List<Boct>());
                var nextList = new List<Boct>();
                for (int i = 0; i < scanList.Count; i++)
                {
                    var boct = scanList[i];
                    if (boct.HasMaterial)
                    {
                        res[d].Add(boct);
                    }
                    else
                    {
                        if (boct.HasChild)
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                if(boct.Children[j] != null)
                                    nextList.Add(boct.Children[j]);
                            }
                        }
                    }
                }
                scanList = nextList;
            }

            // Debug.Log("Result: " + res.Count);
            // foreach (var kv in res)
            // {
                // Debug.Log(kv.Key + ": " + kv.Value.Count);
            // }

            return res;
        }

        public static void ScanSolidBoct(Action<Boct> action, Boct b)
        {
            ScanSolidBoctR(action, b);
        }

        static void ScanSolidBoctR(Action<Boct> action, Boct b)
        {
            if (b.HasMaterial)
            {
                action(b);
            }
            else
            {
                if (b.HasChild)
                {
                    var children = b.Children;
                    for (int i = 0; i < 8; i++)
                    {
                        if (children[i] != null)
                            ScanSolidBoctR(action, b.Children[i]);
                    }
                }
            }
        }

        public static int GetMaxDepth(Boct target)
        {
            var scanList = new List<Boct>();
            scanList.Add(target);

            int maxDepth = 0;
            
            while (scanList.Any())
            {
                var nextList = new List<Boct>();
                foreach (var b in scanList)
                {
                    if (b.HasChild)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            if (b.Children[i] != null)
                                nextList.Add(b.Children[i]);
                        }
                    }
                }
                if (nextList.Count == 0)
                    break;
                scanList = nextList;
                maxDepth++;
            }
            return maxDepth;
        }
        
        public static void FillRegionId(Boct b, int id)
        {
            __param0 = id;
            FillRegionIdR(b);
        }

        static void FillRegionIdR(Boct b)
        {
            b.RegionId = __param0;
            if (b.HasChild)
            {
                if (b.Children != null)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (b.Children[i] != null)
                            FillRegionIdR(b.Children[i]);
                    }
                }
            }
        }

        public static void ClearRegionId(Boct b)
        {
            FillRegionId(b, BoctRegion.EmptyId);
        }

        #region RegionBoctCount

        public static void GetRegionBoctCount(List<Boct> list, Dictionary<int, int> output)
        {
            foreach (var boct in list)
            {
                if (output.ContainsKey(boct.RegionId))
                {
                    output[boct.RegionId] = output[boct.RegionId] + 1;
                }
                else
                {
                    output[boct.RegionId] = 1;
                }
            }
        }

        public static void GetRegionBoctCount(Boct boct, Dictionary<int, int> output)
        {
            GetRegionBoctCountR(boct, output);
        }

        static void GetRegionBoctCountR(Boct boct, Dictionary<int, int> output)
        {
            if (!output.ContainsKey(boct.RegionId))
                output.Add(boct.RegionId, 0);

            output[boct.RegionId] = output[boct.RegionId] + 1;

            if (boct.HasChild)
            {
                foreach (var child in boct.Children)
                {
                    if (child != null)
                    {
                        GetRegionBoctCountR(child, output);
                    }
                }
            }
        }

        public static Dictionary<int, int> MergeRegionBoctCount(List<Dictionary<int, int>> dics)
        {
            var res = new Dictionary<int, int>();
            foreach (var dic in dics)
            {
                foreach (var kv in dic)
                {
                    if (!res.ContainsKey(kv.Key))
                        res.Add(kv.Key, 0);

                    res[kv.Key] = res[kv.Key] + kv.Value;
                }
            }
            return res;
        }

        #endregion

        public static Boct GetAdjoiningBoct(Boct b, int face, BoctModel model)
        {
            var adjoiningAddress = BoctAddressTools.GetAdjoiningBoctAddress(b.Address, face);

            if (adjoiningAddress == null)
            {
                return null;
            }
            else
            {
                return model.Find(adjoiningAddress);
            }
        }

        public static Boct FilterEquals(Boct root, int depth)
        {
            var output = new Boct();
            return output;
        }

        /// <summary>
        /// Finds bocts of same depth.
        /// </summary>
        public static void FindSameDepthBoct(int targetDepth, Boct head, List<Boct> output)
        {
            // Debug.Log("[FindSameDepthBoct]");
            // Debug.Log(head.ToString());
            // Debug.Log("Depth: " + targetDepth);
            FindSameDepthBoctR(targetDepth, head, output, 0);
        }

        static void FindSameDepthBoctR(int targetDepth, Boct boct, List<Boct> output, int depth)
        {
            if (depth == targetDepth)
            {
                if (boct.HasMaterial)
                {
                    output.Add(boct);
                }
            }
            else
            {
                depth++;
                if (boct.HasChild)
                {
                    foreach (var child in boct.Children)
                    {
                        if (child == null)
                            continue;

                        FindSameDepthBoctR(targetDepth, child, output, depth);
                    }
                }
            }
        }

        public static bool InsertBoct(List<Boct> boctList, Boct head)
        {
            foreach (var b in boctList)
            {
                if (!InsertBoct(b, head))
                    return false;
            }
            return true;
        }

        public static bool InsertBoct(Boct boct, Boct head)
        {
            return InsertBoct(boct.Address, head, boct.MaterialId);
        }

        public static bool InsertBoct(List<byte> address, Boct head, int mid)
        {
            if (address == null)
            {
                // Debug.Log("Address is null.");
                return false;
            }

            Boct target = head;

            // Insert bocts
            for (int i = 0; i < address.Count; i++)
            {
                if (target.HasMaterial)
                {
                    // Debug.Log("Target has a material.");
                    // Debug.Log("Address: " + BoctAddressTools.ToString(target.Address));
                    // Debug.Log("RID: " + target.RegionId);
                    return false;
                }
                else
                {
                    byte b = address[i];
                    if (target.Children == null)
                    {
                        target.Children = new Boct[8];
                        target.Children[b] = new Boct(target);
                        target = target.Children[b];
                        target.RegionId = head.RegionId;
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

            target.MaterialId = mid;
            return true;
        }

        public static Dictionary<string, Boct> ToDictionary(List<Boct> list)
        {
            var dic = new Dictionary<string, Boct>();
            foreach (var boct in list)
            {
                dic.Add(boct.AddressString, boct);
            }
            return dic;
        }

        /// <summary>
        /// Get the flood boct list.
        /// </summary>
        public static List<Boct> GetFloodBoctList(Boct target, Boct head)
        {
            // Debug.Log("[GetFloodBoctList]");

            var sameDepthBoctList = new List<Boct>();

            BoctTools.FindSameDepthBoct(target.Depth, head, sameDepthBoctList);

            // Debug.Log("SameDepthBoct: " + sameDepthBoctList.Count);

            var sameMaterialBoctList = sameDepthBoctList.Where(b => b.MaterialId == target.MaterialId).ToList<Boct>();

            // Debug.Log("SameMaterialBoct: " + sameMaterialBoctList.Count);

            // Debug.Log("Dictionary");
            var dic = BoctTools.ToDictionary(sameMaterialBoctList);
            //foreach (var kv in dic)
            //{
                // Debug.Log(kv.Key + " " + kv.Value.ToString());
            //}

            // Remove target.
            dic.Remove(target.AddressString);

            var output = new List<Boct>();
            output.Add(target);

            var searchList = new List<Boct>();
            searchList.Add(target);

            while (searchList.Count > 0)
            {
                var b = searchList[0];
                searchList.RemoveAt(0);
                for (int i = 0; i < 6; i++)
                {
                    var address = BoctAddressTools.GetAdjoiningBoctAddress(b.Address, i);
                    if (address == null)
                        continue;

                    var addressString = BoctAddressTools.ToString(address);

                    // Debug.Log("Address" + i + ": " + addressString);

                    if (dic.ContainsKey(addressString))
                    {
                        output.Add(dic[addressString]);
                        searchList.Add(dic[addressString]);
                        dic.Remove(addressString);
                    }
                }
            }
            return output;
        }

        public static Vector3 GetCenter(BoctModel model)
        {
            if(model.Regions == null)
                return Vector3.zero;

            if (model.Regions.Count == 0)
                return Vector3.zero;

            var sum = Vector3.zero;
            int count = 0;

            foreach (var r in model.Regions)
            {
                var solidList = BoctTools.GetSolidBoctArray(r.Value.Head);
                for (int i = 0; i < solidList.Length; i++)
                {
                    var boct = solidList[i];
                    sum += BoctAddressTools.GetCoordinate(boct.Address);
                    count++;
                }
            }

            return sum / count;
        }

        public static Bounds GetBounds(BoctModel model, bool scanBoct = true)
        {
            var bounds = new Bounds();

            if(model.Regions == null)
                return bounds;

            if (model.Regions.Count == 0)
                return bounds;

            var xRange = new Vector2(float.MaxValue, float.MinValue);
            var yRange = new Vector2(float.MaxValue, float.MinValue);
            var zRange = new Vector2(float.MaxValue, float.MinValue);

            if (scanBoct)
            {
                foreach (var r in model.Regions)
                {
                    var solidList = BoctTools.GetSolidBoctArray(r.Value.Head);
                    for (int j = 0; j < solidList.Length; j++)
                    {
                        var boct = solidList[j];
                        var boctBounds = BoctAddressTools.GetBounds(boct.Address);
                        //Debug.Log("Bound: " + boctBounds.ToString("F4"));
                        if (xRange.x > boctBounds.x - boctBounds.w)
                            xRange.x = boctBounds.x - boctBounds.w;
                        if (xRange.y < boctBounds.x + boctBounds.w)
                            xRange.y = boctBounds.x + boctBounds.w;
                        if (yRange.x > boctBounds.y - boctBounds.w)
                            yRange.x = boctBounds.y - boctBounds.w;
                        if (yRange.y < boctBounds.y + boctBounds.w)
                            yRange.y = boctBounds.y + boctBounds.w;
                        if (zRange.x > boctBounds.z - boctBounds.w)
                            zRange.x = boctBounds.z - boctBounds.w;
                        if (zRange.y < boctBounds.z + boctBounds.w)
                            zRange.y = boctBounds.z + boctBounds.w;
                    }
                }
            }
            else
            {
                foreach (var r in model.Regions)
                {
                    var boct = r.Value.Head;
                    var boctBounds = BoctAddressTools.GetBounds(boct.Address);
                    //Debug.Log("Bound: " + boctBounds.ToString("F4"));
                    if (xRange.x > boctBounds.x - boctBounds.w)
                        xRange.x = boctBounds.x - boctBounds.w;
                    if (xRange.y < boctBounds.x + boctBounds.w)
                        xRange.y = boctBounds.x + boctBounds.w;
                    if (yRange.x > boctBounds.y - boctBounds.w)
                        yRange.x = boctBounds.y - boctBounds.w;
                    if (yRange.y < boctBounds.y + boctBounds.w)
                        yRange.y = boctBounds.y + boctBounds.w;
                    if (zRange.x > boctBounds.z - boctBounds.w)
                        zRange.x = boctBounds.z - boctBounds.w;
                    if (zRange.y < boctBounds.z + boctBounds.w)
                        zRange.y = boctBounds.z + boctBounds.w;
                }
            }

            var center = new Vector3(
                (xRange.x + xRange.y) * 0.5f,
                (yRange.x + yRange.y) * 0.5f,
                (zRange.x + zRange.y) * 0.5f
            );

            var extents = new Vector3(
                (xRange.y - xRange.x) * 0.5f,
                (yRange.y - yRange.x) * 0.5f,
                (zRange.y - zRange.x) * 0.5f
            );

            bounds.center = center;
            bounds.extents = extents;

            return bounds;
        }

        public static void DivideArea(Boct target, int depth)
        {
            // Debug.Log("DivideArea: depth=" + depth);
            var dic = BoctTools.GetHierarchizedSolidBoctList(target, depth);

            foreach (var kv in dic)
            {
                int d = depth - kv.Key;
                //Kenny.D(string.Format("Key: {0}, Count: {1}, d: {2}", kv.Key, kv.Value.Count, d));
                List<Boct> list = kv.Value;
                for (int i = 0; i < list.Count; i++)
                {
                    DivideDeeply(list[i], d);
                }
            }
        }

        /// <summary>
        /// Divide 
        /// </summary>
        public static bool DivideDeeply(Boct target, int depth, bool spawn = true) 
        {
            var scanList = new List<Boct>();
            scanList.Add(target);

            for(int d = 0; d <= depth; d++)
            {
                var nextList = new List<Boct>();
                foreach (var b in scanList)
                {
                    bool res = b.Divide(spawn);
                    if (res)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            nextList.Add(b.Children[i]);
                        }
                    }
                }
                scanList = nextList;
            }

            // TODO fix
            return true;
        }

        public static void MirrorCopy(Direction direction, Boct target)
        {
            Assert.IsTrue(target.HasChild);

            byte[] mirrorTable = null;

            switch (direction)
            {
                case Direction.South:
                    target.TrimChild(new int[] { 1, 2, 5, 6 });
                    mirrorTable = MirrorConvertTableSN;
                    break;
                case Direction.East:
                    target.TrimChild(new int[] { 2, 3, 6, 7 });
                    mirrorTable = MirrorConvertTableEW;
                    break;
                case Direction.North:
                    target.TrimChild(new int[] { 0, 3, 4, 7 });
                    mirrorTable = MirrorConvertTableSN;
                    break;
                case Direction.West:
                    target.TrimChild(new int[] { 0, 1, 4, 5 });
                    mirrorTable = MirrorConvertTableEW;
                    break;
                case Direction.Top:
                    target.TrimChild(new int[] { 0, 1, 2, 3 });
                    mirrorTable = MirrorConvertTableTB;
                    break;
                case Direction.Bottom:
                    target.TrimChild(new int[] { 4, 5, 6, 7 });
                    mirrorTable = MirrorConvertTableTB;
                    break;
            }

            var sourceAddressList = GetSolidBoctArray(target);
            var mirrorAddressList = new List<List<byte>>(sourceAddressList.Length);
            for (int i = 0; i < sourceAddressList.Length; i++)
            {
                var sourceAddress = sourceAddressList[i].Address;
                var mirrorAddress = new List<byte>(sourceAddress.Count);
                for (int j = 0; j < sourceAddress.Count; j++)
                {
                    mirrorAddress.Add(mirrorTable[sourceAddress[j]]);
                }
                mirrorAddressList.Add(mirrorAddress);
            }

            for(int i = 0; i < sourceAddressList.Length; i++)
            {
                BoctTools.InsertBoct(mirrorAddressList[i], target, sourceAddressList[i].MaterialId);
            }

        }

        public static void Flip(Direction direction, Boct target)
        {
            Assert.IsTrue(target.HasChild);

            byte[] mirrorTable = null;

            switch (direction)
            {
                case Direction.South:
                    mirrorTable = MirrorConvertTableSN;
                    break;
                case Direction.East:
                    mirrorTable = MirrorConvertTableEW;
                    break;
                case Direction.North:
                    mirrorTable = MirrorConvertTableSN;
                    break;
                case Direction.West:
                    mirrorTable = MirrorConvertTableEW;
                    break;
                case Direction.Top:
                    mirrorTable = MirrorConvertTableTB;
                    break;
                case Direction.Bottom:
                    mirrorTable = MirrorConvertTableTB;
                    break;
            }

            var sourceAddressList = GetSolidBoctArray(target);
            var mirrorAddressList = new List<List<byte>>(sourceAddressList.Length);
            for (int i = 0; i < sourceAddressList.Length; i++)
            {
                var sourceAddress = sourceAddressList[i].Address;
                var mirrorAddress = new List<byte>(sourceAddress.Count);
                for (int j = 0; j < sourceAddress.Count; j++)
                {
                    mirrorAddress.Add(mirrorTable[sourceAddress[j]]);
                }
                mirrorAddressList.Add(mirrorAddress);
            }

            target.Children = null;

            for (int i = 0; i < sourceAddressList.Length; i++)
            {
                BoctTools.InsertBoct(mirrorAddressList[i], target, sourceAddressList[i].MaterialId);
            }

        }

        public static void Rotate(Direction direction, Boct target)
        {
            Assert.IsTrue(target.HasChild);

            byte[,] rotateTable = null;

            int iteration = 0;

            switch (direction)
            {
                case Direction.South:
                    rotateTable = RotateTableY;
                    iteration = 0;
                    break;
                case Direction.East:
                    rotateTable = RotateTableZ;
                    iteration = 0;
                    break;
                case Direction.North:
                    rotateTable = RotateTableY;
                    iteration = 2;
                    break;
                case Direction.West:
                    rotateTable = RotateTableZ;
                    iteration = 2;
                    break;
                case Direction.Top:
                    rotateTable = RotateTableX;
                    iteration = 0;
                    break;
                case Direction.Bottom:
                    rotateTable = RotateTableX;
                    iteration = 2;
                    break;
            }

            var sourceAddressList = GetSolidBoctArray(target);
            var rotateAddressList = new List<List<byte>>(sourceAddressList.Length);
            for (int i = 0; i < sourceAddressList.Length; i++)
            {
                var sourceAddress = sourceAddressList[i].Address;
                var mirrorAddress = new List<byte>(sourceAddress.Count);
                for (int j = 0; j < sourceAddress.Count; j++)
                {
                    mirrorAddress.Add(rotateTable[iteration, sourceAddress[j]]);
                }
                rotateAddressList.Add(mirrorAddress);
            }

            target.Children = null;

            for (int i = 0; i < sourceAddressList.Length; i++)
            {
                BoctTools.InsertBoct(rotateAddressList[i], target, sourceAddressList[i].MaterialId);
            }

        }

    }

}

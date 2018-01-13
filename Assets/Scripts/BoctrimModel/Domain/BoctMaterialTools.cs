using System.Collections.Generic;

namespace Boctrim.Domain
{

    public static class BoctMaterialTools
    {

        public static Dictionary<int, int> CountMaterials(Boct head, Dictionary<int, BoctMaterial> materials)
        {
            var dic = CreateMaterialContainer(materials);

            var solidBoctList = BoctTools.GetSolidBoctArray(head);

            for (int i = 0; i < solidBoctList.Length; i++)
            {
                var boct = solidBoctList[i];
                dic[boct.MaterialId]++;
            }

            return dic;
        }

        static Dictionary<int, int> CreateMaterialContainer(Dictionary<int, BoctMaterial> materials)
        {
            var dic = new Dictionary<int, int>();

            foreach (var kv in materials)
            {
                dic.Add(kv.Key, 0);
            }
            return dic;
        }

        public static Dictionary<int, int> ConcatMaterialCounts(Dictionary<int, BoctRegion> regions)
        {
            var res = new Dictionary<int, int>();
            foreach (var regionKv in regions)
            {
                var mc = regionKv.Value.MaterialCounts;
                foreach (var mcKv in mc)
                {
                    if (!res.ContainsKey(mcKv.Key))
                        res[mcKv.Key] = mcKv.Value;
                    else
                        res[mcKv.Key] += mcKv.Value;
                }
            }
            return res;
        }

    }

}

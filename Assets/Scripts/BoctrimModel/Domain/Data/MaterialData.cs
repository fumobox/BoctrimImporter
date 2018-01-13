using System.Collections;
using System;

namespace Boctrim.Domain
{

    public class MaterialData: BoctrimData
    {

        public int LUID { get; set;}

        public ColorData Color { get; set;}

        public int SortOrder {get; set;}

    }

}

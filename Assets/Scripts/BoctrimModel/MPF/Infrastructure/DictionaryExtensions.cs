using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MPF.Infrastructure
{

    public static class DictionaryExtensions
    {

        public static int GetHighestKey<T>(this Dictionary<int, T> dictionary)
        {
            return dictionary.Aggregate((l, r) => l.Key > r.Key ? l : r).Key;
        }

        public static int GetHighestValue<T>(this Dictionary<int, T> dictionary)
        {
            return dictionary.Aggregate((l, r) => l.Key > r.Key ? l : r).Key;
        }

    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyLogic_Mozerov
{
    public static class Utils
    {
        public static void Swap<T>(this List<T> list, int i, int j)
        {
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
        public static void Shuffle<T>(this List<T> list, Random rnd)
        {
            for (var i = 0; i < list.Count; i++)
                list.Swap(i, rnd.Next(i, list.Count));
        }
    }
}

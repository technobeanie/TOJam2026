
using System;
using System.Collections.Generic;

namespace Utils
{
    public static class ListExtensions
    {
        // const

        // public

        // protected

        // private
        private static Random _random = new Random();

        // properties

        #region Methods
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = _random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        #endregion
    }
}

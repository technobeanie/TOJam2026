using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class MathfPlus
    {
        // const

        // public

        // protected

        // private

        // properties

        #region Unity Methods
        #endregion

        #region Public Methods
        public static int Modulo(int value, int modulo)
        {
            if (modulo != 0)
            {
                int returnModulo = value % modulo;
                return returnModulo < 0 ? returnModulo + modulo : returnModulo;
            }

            return value;
        }

        public static void Swap<T>(this IList<T> list, int indexA, int indexB)
        {
            T temp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = temp;
        }

        public static int IndexOf<T>(this IReadOnlyList<T> list, T itemToFind)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                var item = list[i];
                if (Equals(item, itemToFind))
                {
                    return i;
                }
            }

            return -1;
        }

        public static IList<T> GetRange<T>(this IReadOnlyList<T> list, int index, int count)
        {
            List<T> newList = new List<T>();

            for (int i = index; i >= 0 && i < list.Count && i < count; ++i)
            {
                var item = list[i];
                newList.Add(item);
            }

            return newList;
        }
        #endregion

        #region Protected Methods
        #endregion

        #region Private Methods
        #endregion
    }
}

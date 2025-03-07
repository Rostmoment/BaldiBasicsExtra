using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BBE.Extensions
{
    public static class CollectionExtensions
    {
        public static IEnumerable<T> CopyCollection<T>(this IEnumerable<T> values)
        {
            List<T> list = new List<T>();
            if (values == null)
                return list;
            for (int i = 0; i<values.Count(); i++)
                list.Add(values.ElementAt(i));
            return list;
        }
        public static void SetRow<T>(this T[,] matrix, int row, params T[] rowVector)
        {
            int rowLength = matrix.GetLength(1);
            for (int i = 0; i < rowLength; i++)
                matrix[row, i] = rowVector[i];
        }
        public static T[] GetRow<T>(this T[,] matrix, int row)
        {
            if (matrix.ArrayIsEmptyOrNull())
                return null;
            int rowLength = matrix.GetLength(1);
            T[] rowVector = new T[rowLength];
            for (int i = 0; i < rowLength; i++)
                rowVector[i] = matrix[row, i];

            return rowVector;
        }
        public static void SetColumn<T>(this T[,] matrix, int column, params T[] columnVector)
        {
            int columnLength = matrix.GetLength(0);
            for (int i = 0; i < columnLength; i++)
                matrix[i, column] = columnVector[i];
        }
        public static T[] GetColumn<T>(this T[,] matrix, int row)
        {
            if (matrix.ArrayIsEmptyOrNull())
                return null;
            int columnLength = matrix.GetLength(0);
            T[] columnVector = new T[columnLength];
            for (int i = 0; i < columnLength; i++)
                columnVector[i] = matrix[i, row];

            return columnVector;
        }
        public static void Add<K, V>(this List<KeyValuePair<K, V>> list, K key, V value)
        {
            list.Add(new KeyValuePair<K, V>(key, value));
        }
        public static bool AllAre<T>(this IEnumerable<T> list, Func<T, bool> func) => list.Count(func) == list.Count();

        public static void AddOrRemove<T>(this List<T> list, T value)
        {
            if (list == null) return;
            if (list.Contains(value)) list.Remove(value);
            else list.Add(value);
        }

        public static bool HasSame<T>(this IEnumerable<T> list, IEnumerable<T> values)
        {
            return values.ToList().Exists(x => list.Contains(x));
        }

        public static K FindKey<K, V>(this Dictionary<K, V> dictionary, V value)
        {
            return dictionary.Where(x => x.Value.Equals(value)).FirstOrDefault().Key;
        }

        public static float FindNearest(this IEnumerable<float> ints, float value)
        {
            float nearest = ints.ElementAt(0);
            float minDifference = Math.Abs(ints.ElementAt(0) - value);
            foreach (float x in ints)
            {
                float difference = Math.Abs(x - value);
                if (difference < minDifference)
                {
                    minDifference = difference;
                    nearest = x;
                }
            }
            return nearest;
        }
        public static int FindNearest(this IEnumerable<int> ints, int value)
        {
            int nearest = ints.ElementAt(0);
            int minDifference = Math.Abs(ints.ElementAt(0) - value);

            foreach (int x in ints)
            {
                int difference = Math.Abs(x - value);
                if (difference < minDifference)
                {
                    minDifference = difference;
                    nearest = x;
                }
            }
            return nearest;
        }

        public static List<T> Shuffle<T>(this List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
            return list;
        }

        public static bool IfExists<T>(this IEnumerable<T> values, Func<T, bool> func, out T res)
        {
            if (values.Where(func).Count() > 0)
            {
                res = values.Where(func).First();
                return true;
            }
            res = default;
            return false;
        }


        public static List<T> RemoveIfContains<T>(this List<T> list, T value)
        {
            if (list.Contains(value)) list.Remove(value);
            return list;
        }

        public static bool ArrayIsEmptyOrNull(this Array array)
        {
            if (array == null) return true;
            return array.Length == 0;
        }
        public static bool EmptyOrNull(this string str)
        {
            if (str == null) return true;
            return str == "";
        }
        public static bool EmptyOrNull<T>(this IEnumerable<T> value)
        {
            if (value == null) return true;
            return value.Count() == 0;
        }

        public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<KeyValuePair<K, V>> pairs, bool exceptionIfSameKey = false)
        {
            Dictionary<K, V> res = new Dictionary<K, V>();
            foreach (var data in pairs)
            {
                if (!res.ContainsKey(data.Key) || exceptionIfSameKey)
                {
                    res.Add(data.Key, data.Value);
                }
            }
            return res;
        }

        public static List<T> ReplaceWhere<T>(this IEnumerable<T> list, Func<T, bool> conditional, T replaceWith)
        {
            List<T> res = new List<T>(list);
            for (int i = 0; i < res.Count; i++) if (conditional(res[i])) res[i] = replaceWith;
            return res;
        }

        public static T[] RemoveLastN<T>(this T[] array, int n)
        {
            return array.ToList().RemoveLastN(n).ToArray();
        }
        public static List<T> RemoveLastN<T>(this List<T> list, int n)
        {
            if (n <= list.Count)
            {
                list.RemoveRange(list.Count - n, n);
                return list;
            }
            else
            {
                return list;
            }
        }
        public static List<List<T>> SplitList<T>(this List<T> values, int chunkSize)
        {
            List<List<T>> res = new List<List<T>>();
            for (int i = 0; i < values.Count; i += chunkSize)
            {
                res.Add(values.GetRange(i, Math.Min(chunkSize, values.Count - i)));
            }
            return res;
        }

        public static T[] ChooseRandomCount<T>(this IEnumerable<T> value) => value.ChooseRandom(UnityEngine.Random.Range(0, value.Count()));
        public static T[] ChooseRandom<T>(this IEnumerable<T> value, int count)
        {
            List<T> tmp = new List<T>(value);
            List<T> res = new List<T>();
            for (int i = 0; i < count; i++)
            {
                T val = tmp.ChooseRandom();
                res.Add(val);
                tmp.Remove(val);
            }
            return res.ToArray();
        }
        public static T[] ChooseRandomCount<T>(this IEnumerable<T> value, System.Random rng) => value.ChooseRandom(UnityEngine.Random.Range(0, value.Count()), rng);
        public static T ChooseRandom<T>(this IEnumerable<T> list)
        {
            if (list.EmptyOrNull())
            {
                return default;
            }
            return list.ElementAt(UnityEngine.Random.Range(0, list.Count()));
        }
        public static T[] ChooseRandom<T>(this IEnumerable<T> value, int count, System.Random rng)
        {
            List<T> tmp = new List<T>(value);
            List<T> res = new List<T>();
            for (int i = 0; i < count; i++)
            {
                T val = tmp.ChooseRandom(rng);
                res.Add(val);
                tmp.Remove(val);
            }
            return res.ToArray();
        }
        public static T ChooseRandom<T>(this IEnumerable<T> list, System.Random rng)
        {
            if (list.EmptyOrNull())
            {
                return default;
            }
            return list.ElementAt(rng.Next(0, list.Count()));
        }
    }
}

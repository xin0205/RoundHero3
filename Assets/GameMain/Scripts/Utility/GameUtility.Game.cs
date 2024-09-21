using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public static partial class GameUtility
    {
        public static Tweener DelayExcute(float delayTime, Action action)
        {

            int a = 0;
            return DOTween.To(()=> a, x => a = x, 1, delayTime).OnComplete(() =>
            {
                action.Invoke();
            });
        }
        
        public static string Format(this float value)
        {
            if (value % 1 * 1 % 1 == 0)
            {
                return value.ToString("0");
            }
            else if (value % 1 * 10 % 1 == 0)
            {
                return value.ToString("0.0");
            }
            else if (value % 1 * 100 % 1 == 0)
            {
                return value.ToString("0.00");
            }
            
            return value.ToString("0.000");
        }
        

        public static List<int[]> GetPermutation(int listCount, int resCount)
        {
            resCount = resCount > listCount ? listCount : resCount;
            var arr = new int[listCount];
            for (int i = 0; i < listCount; i++)
            {
                arr[i] = i;
            }

            return PermutationAndCombination<int>.GetPermutation(arr, resCount);
        }

  
        public static int listIdx = 0;
        
        
        public static List<List<int>> listSort(int index, List<List<int>> matrix, List<int> str, ref List<List<int>> listMask)
        {
            listIdx = 0;
            return ListRecursively(index, matrix, str, ref listMask);
        }
        
        public static List<List<int>> ListRecursively(int index, List<List<int>> matrix, List<int> str, ref List<List<int>> listMask)
        {
            var m = matrix[index];
            var mCount = m.Count;
            for (int k = 0; k < mCount; ++k)
            {
                var ch = m[k];

                var l = listMask[listIdx];
                var strCount = str.Count;
                for (int i = 0; i < strCount; i++)
                {
                    l[i] = str[i];
                }
                
                l[index] = ch;

                if (index == matrix.Count - 1)
                {
                    listIdx++;
                }
                else
                {
                    ListRecursively(index + 1, matrix, l, ref listMask); // 向下传递的逻辑
                }
            }
            return listMask;
        }

        public static T GetEnum<T>(string enumStr)
        {
            return (T)Enum.Parse(typeof(T), enumStr);
        }

        public static List<T> GetEnums<T>(List<string> enumStrs)
        {
            var enums = new List<T>();

            foreach (var enumStr in enumStrs)
            {
                enums.Add((T)Enum.Parse(typeof(T), enumStr));
            }
            
            return enums;
        }

        public static Vector2Int GetDirect(Vector2Int coord)
        {
            coord.x = coord.x != 0 ? coord.x / Mathf.Abs(coord.x) : coord.x;
            coord.y = coord.y != 0 ? coord.y / Mathf.Abs(coord.y) : coord.y;

            return coord;
        }
        
        public static void InsertionSort<T>(IList<T> list, Comparison <T>  comparison)
        {
            if (list == null)
                throw new ArgumentNullException( "list" );
            if (comparison == null)
                throw new ArgumentNullException( "comparison" );

            int count = list.Count;
            for (int j = 1; j < count; j++)
            {
                T key = list[j];

                int i = j - 1;
                for (; i >= 0 && comparison( list[i], key ) > 0; i--)
                {
                    list[i + 1] = list[i];
                }
                list[i + 1] = key;
            }
        }
    }
}
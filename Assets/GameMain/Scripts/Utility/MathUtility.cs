using System;
using System.Collections.Generic;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public class MathUtility
    {
        private static List<int> randomNumList = new List<int>(100);
        public static List<int> GetRandomNum(int count, int start, int length, Random random = null, bool isRepeat = false)
        {
            if (count == 0)
                return new List<int>();

            if (count > length)
            {
                Log.Warning("GetRandomNum count > length!");
            }
            
            randomNumList.Clear();
            for (int i = 0; i < length; i++)
            {
                randomNumList.Add(start + i);
            }

            var randNumList = new List<int>(count);
            while (count > 0 && randomNumList.Count > 0)
            {
                var randIdx = 0;
                if (random != null)
                {
                    randIdx = random.Next(0, randomNumList.Count);
                }
                else
                {
                    randIdx = UnityEngine.Random.Range(0, randomNumList.Count);
                }


                randNumList.Add(randomNumList[randIdx]);
                if (!isRepeat)
                {
                    randomNumList.RemoveAt(randIdx);
                }
                
                count--;
            }
            
            return randNumList;
        }
    }
}
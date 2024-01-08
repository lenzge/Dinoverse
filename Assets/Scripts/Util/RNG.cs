using System;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Util
{
    public class RNG
    {
        private static int count1 = 0;
        private static int count2 = 0;
        private static int count3 = 0;
        public void Start()
        {
            for (int i = 0; i < 5; i++)
            {
                Debug.Log(RandomDonut(30, 10, i));
            }

        }

        public static Vector2 RandomDonut(int radius, int innerRadius, int randomOffset)
        {
            int seed = (int) DateTime.Now.Ticks * randomOffset * ((int) DateTime.Now.Ticks / 2);
            Random.InitState(seed);
            int x;
            int z;
            int i = Random.Range(0, 2);
            switch (i)
            {
               case 0:
                   x = Random.Range(innerRadius, radius);
                   count1 += 1;
                   break;
               case 1:
                   x = Random.Range(-innerRadius, -radius);
                   count2 += 1;
                   break;
               default:
                   x = 0;
                   count3 += 1;
                   break;
            }
            int j = Random.Range(0, 2);
            switch (j)
            {
                case 0:
                    z = Random.Range(innerRadius, radius);
                    count1 += 1;
                    break;
                case 1:
                    z = Random.Range(-innerRadius, -radius);
                    count2 += 1;
                    break;
                default:
                    z = 0;
                    count3 += 1;
                    break;
            }

            Debug.LogError($"{x}, {z}");
            return new Vector2(x, z);
        }

        public static void DebugRNG()
        {
            Debug.LogError($"{count1}, {count2}, {count3}");
        }
    }
}
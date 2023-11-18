using System;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Util
{
    public class RNG
    {
        public void Start()
        {
            for (int i = 0; i < 5; i++)
            {
                Debug.Log(RandomDonut(30, 10, i));
            }

        }

        public static Vector2 RandomDonut(int radius, int innerRadius, int randomOffset)
        {
            Random.InitState((int)DateTime.Now.Ticks * randomOffset * ((int)DateTime.Now.Ticks / 2));
            int x;
            int z;
            int i = Random.Range(0, 3);
            switch (i)
            {
               case 1:
                   x = Random.Range(innerRadius, radius);
                   break;
               case 2:
                   x = Random.Range(-innerRadius, -radius);
                   break;
               default:
                   x = Random.Range(-innerRadius, -radius);
                   break;
            }
            int j = Random.Range(0, 3);
            switch (j)
            {
                case 1:
                    z = Random.Range(innerRadius, radius);
                    break;
                case 2:
                    z = Random.Range(-innerRadius, -radius);
                    break;
                default:
                    z = Random.Range(-innerRadius, -radius);
                    break;
            }

            return new Vector2(x, z);
        }
    }
}
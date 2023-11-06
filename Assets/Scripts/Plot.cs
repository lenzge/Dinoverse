using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Data/Statistics")]
    public class Plot : ScriptableObject
    {
        public List<Statistic> statistics = new List<Statistic>();

        public void SaveData(int survivedTime, int reproduced, int timeOfDeath)
        {
            statistics.Add(new Statistic(survivedTime, reproduced, timeOfDeath));

        }

        public void ResetData()
        {
            statistics.Clear();
        }

        public List<float> LifeSpan()
        {
            int timeOfDeath = 0;
            List<int> buffer = new List<int>();
            List<float> lifeSpanList = new List<float>();
            //Debug.Log(statistics.Count);

            for (int i = 0; i < statistics.Count; i++)
            {
                if (statistics[i].TimeOfDeath == timeOfDeath)
                {
                    //Debug.Log("survivedTime: " + statistics[i].SurvivedTime);
                    buffer.Add(statistics[i].SurvivedTime);
                    
                }
                else
                { 
                    float average = 0;
                    if (buffer.Count != 0)
                    {
                        float sum = buffer.Sum();
                        //Debug.Log("sum: " + sum);
                        //Debug.Log("count: " + buffer.Count);
                        average = sum / buffer.Count;
                    }

                    lifeSpanList.Add(average);
                    Debug.LogWarning($"Average Fitness in Generation {timeOfDeath}: " + average );
                    buffer.Clear();
                    timeOfDeath++;
                }
            }

            return lifeSpanList;
        }
    }

    public struct Statistic
    {
        public int SurvivedTime;
        public int Reproduced;
        public int TimeOfDeath;

        public Statistic(int survivedTime, int reproduced, int timeOfDeath)
        {
            SurvivedTime = survivedTime;
            Reproduced = reproduced;
            TimeOfDeath = timeOfDeath;
        }

    }
}
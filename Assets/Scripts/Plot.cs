using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace DefaultNamespace
{
    public class Plot : MonoBehaviour
    {
        private string FileName;
        public List<Statistic> statistics = new List<Statistic>();

        private string filePath = "";
        private Queue<Statistic> writeQueue = new Queue<Statistic>();
        private object lockObject = new object();
        private bool isWriting = false;

        public void SaveData(int key, int generation, int grandChild, int survivedTime, int eatenTrees,
            int reproduced, int reproducedMutual, int timeOfDeath, int causeOfDeath, int fitness, int newLevel)
        {
            Statistic statistic = new Statistic(key, generation, grandChild, survivedTime, eatenTrees, reproduced, 
                reproducedMutual,timeOfDeath, causeOfDeath, fitness, newLevel);
            statistics.Add(statistic);
            WriteToCSV(statistic);

        }

        public void StartGame()
        {
            statistics.Clear();
            FileName = DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + ".csv";
#if UNITY_EDITOR
            filePath = Path.Combine(Application.dataPath,"Plots", FileName);
#else
            filePath = Path.Combine(Application.persistentDataPath, FileName);
#endif

            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("key,generation,grandChild,survivedTime,eatenTrees,reproduced,reproducedMutual," +
                                     "timeOfDeath,causeOfDeath,fitness,newLevel");
                }

            }
            catch (Exception e)
            {
                Debug.LogError("Can't write into CSV file because of " + e);
            }

        }

        public List<float> LifeSpan()
        {
            int timeOfDeath = 1;
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
                    Debug.LogWarning($"Average Fitness in Generation {timeOfDeath}: " + average);
                    buffer.Clear();
                    timeOfDeath++;
                }
            }

            return lifeSpanList;
        }


        private void WriteToCSV(Statistic data)
        {
            lock (lockObject)
            {
                writeQueue.Enqueue(data);
                if (!isWriting)
                {
                    isWriting = true;
                    Thread writingThread = new Thread(WriteQueueToCSV);
                    writingThread.Start();
                }
            }
        }

        private void WriteQueueToCSV()
        {
            while (true)
            {
                Statistic data;

                lock (lockObject)
                {
                    if (writeQueue.Count > 0)
                    {
                        data = writeQueue.Dequeue();
                    }
                    else
                    {
                        isWriting = false;
                        return;
                    }
                }

                try
                {
                    using (StreamWriter writer = new StreamWriter(filePath, true))
                    {
                        writer.WriteLine($"{data.Key}, {data.Generation}, {data.GrandChild}, {data.SurvivedTime}, " +
                                         $"{data.EatenTrees}, {data.Reproduced}, {data.ReproducedMutual}, {data.TimeOfDeath}, " +
                                         $"{data.CauseOfDeath}, {data.Fitness}, {data.NewLevel}");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Can't write into CSV file because of " + e);
                }
            }
        }

        public struct Statistic
        {
            public int Key;
            public int Generation;
            public int GrandChild;
            public int SurvivedTime;
            public int EatenTrees;
            public int Reproduced;
            public int ReproducedMutual;
            public int TimeOfDeath;
            public int CauseOfDeath;
            public int Fitness;
            public int NewLevel;

            public Statistic(int key, int generation, int grandChild, int survivedTime, int eatenTrees,
                int reproduced, int reproducedMutual, int timeOfDeath, int causeOfDeath, int fitness, int newLevel)
            {
                Key = key;
                Generation = generation;
                GrandChild = grandChild;
                SurvivedTime = survivedTime;
                EatenTrees = eatenTrees;
                Reproduced = reproduced;
                ReproducedMutual = reproducedMutual;
                TimeOfDeath = timeOfDeath;
                CauseOfDeath = causeOfDeath;
                Fitness = fitness;
                NewLevel = newLevel;
            }

        }
    }
}



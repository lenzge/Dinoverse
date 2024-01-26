using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Animal;
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
        private NumberFormatInfo formatInfo;

        public void SaveData(int key,int population, int generation, float color, int survivedTime, int eatenTrees, int eatenAnimals,
            int reproducedSolo, int reproducedMutual, int timeOfDeath, int causeOfDeath, int fitness, int newLevel, DNA dna)
        {
            Statistic statistic = new Statistic(key,population, generation, color, survivedTime, eatenTrees, eatenAnimals, reproducedSolo, 
                reproducedMutual,timeOfDeath, causeOfDeath, fitness, newLevel, dna);
            statistics.Add(statistic);
            WriteToCSV(statistic);

        }

        public void StartGame()
        {
            statistics.Clear();
            formatInfo = new NumberFormatInfo();
            formatInfo.NumberDecimalSeparator = ".";
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
                    writer.WriteLine("key,population,generation,color,survivedTime,eatenTrees,eatenAnimals,reproducedSolo,reproducedMutual," +
                                     "timeOfDeath,causeOfDeath,fitness,newLevel,lifeExpectation,weight,mutationAmount,mutationChance," +
                                     "carnivore,visualRadius,angleBetweenRaycasts,movementSpeed,sexualMaturity,litterSize");
                }

            }
            catch (Exception e)
            {
                Debug.LogError("Can't write into CSV file because of " + e);
            }

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
                        writer.WriteLine($"{data.Key}, {data.Population}, {data.Generation}, {data.Color.ToString("0.000", formatInfo)}, {data.SurvivedTime}, " +
                                         $"{data.EatenTrees}, {data.EatenAnimals}, {data.ReproducedSolo}, " +
                                         $"{data.ReproducedMutual}, {data.TimeOfDeath}, {data.CauseOfDeath}, {data.Fitness}, " +
                                         $"{data.NewLevel}, {data.LifeExpectation}, {data.Weight}, {data.MutationAmount.ToString("0.000", formatInfo)}, " +
                                         $"{data.MutationChance.ToString("0.000", formatInfo)}, {data.Carnivore.ToString("0.000", formatInfo)}, {data.VisualRadius}, {data.AngleBetweenRaycasts}, " +
                                         $"{data.MovementSpeed}, {data.SexualMaturity}, {data.LitterSize}");
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
            public int Population;
            public int Generation;
            public float Color;
            public int SurvivedTime;
            public int EatenTrees;
            public int EatenAnimals;
            public int ReproducedSolo;
            public int ReproducedMutual;
            public int TimeOfDeath;
            public int CauseOfDeath;
            public int Fitness;
            public int NewLevel;
            public int LifeExpectation;
            public int Weight;
            public float MutationAmount;
            public float MutationChance;
            public float Carnivore;
            public int VisualRadius;
            public int AngleBetweenRaycasts;
            public int MovementSpeed;
            public int SexualMaturity;
            public int LitterSize;

            public Statistic(int key, int population, int generation, float color, int survivedTime, int eatenTrees,
                int eatenAnimals, int reproducedSolo, int reproducedMutual, int timeOfDeath, int causeOfDeath, int fitness, 
                int newLevel, DNA dna)
            {
                Key = key;
                Population = population;
                Generation = generation;
                Color = color;
                SurvivedTime = survivedTime;
                EatenTrees = eatenTrees;
                EatenAnimals = eatenAnimals;
                ReproducedSolo = reproducedSolo;
                ReproducedMutual = reproducedMutual;
                TimeOfDeath = timeOfDeath;
                CauseOfDeath = causeOfDeath;
                Fitness = fitness;
                NewLevel = newLevel;
                LifeExpectation = dna.LifeExpectation[0]; 
                Weight = dna.Weight[0];
                MutationAmount = dna.MutationAmount[0]; 
                MutationChance = dna.MutationChance[0];
                Carnivore = dna.Carnivore[0];
                VisualRadius = dna.VisualRadius[0]; 
                AngleBetweenRaycasts = dna.AngleBetweenRaycasts[0];
                MovementSpeed = dna.MovementSpeed[0];
                SexualMaturity = dna.SexualMaturity[0];
                LitterSize = dna.LitterSize[0];
            }

        }
    }
}



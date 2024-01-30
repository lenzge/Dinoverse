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
        public EnvironmentData EnvironmentData;
        public AnimalController animal;
        public List<Statistic> statistics = new List<Statistic>();

        private string plotFilePath = "";
        private string envFilePath = "";
        private string aniFilePath = "";
        private string buildNumber = "v13-";
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
            string dateTime = DateTime.Now.ToString("dd-MM-yy-HH-mm-ss");
            string plotFileName = "PLT"+ buildNumber + dateTime + ".csv";
            string envFileName = "ENV"+ buildNumber + dateTime + ".json";
            string aniFileName = "ANI"+ buildNumber + dateTime + ".json";
            string envJson = CreateEnvJson();
            string aniJson = CreateAniJson();
            
#if UNITY_EDITOR
            plotFilePath = Path.Combine(Application.dataPath,"Plots", plotFileName);
            envFilePath = Path.Combine(Application.dataPath,"Plots", envFileName);
            aniFilePath = Path.Combine(Application.dataPath,"Plots", aniFileName);
#else
            plotFilePath = Path.Combine(Application.persistentDataPath, plotFileName);
            envFilePath = Path.Combine(Application.persistentDataPath, envFileName);
            aniFilePath = Path.Combine(Application.persistentDataPath, aniFileName);
#endif

            try
            {
                using (StreamWriter writer = new StreamWriter(plotFilePath))
                {
                    writer.WriteLine("key,population,generation,color,survivedTime,eatenTrees,eatenAnimals,reproducedSolo,reproducedMutual," +
                                     "timeOfDeath,causeOfDeath,fitness,newLevel,lifeExpectation,weight,mutationAmount,mutationChance," +
                                     "carnivore,visualRadius,angleBetweenRaycasts,movementSpeed,sexualMaturity,litterSize");
                }
                using (StreamWriter writer = new StreamWriter(envFilePath))
                {
                    writer.Write(envJson);
                }
                using (StreamWriter writer = new StreamWriter(aniFilePath))
                {
                    writer.Write(aniJson);
                }

            }
            catch (Exception e)
            {
                Debug.LogError("Can't write into CSV file because of " + e);
            }

        }

        private string CreateEnvJson()
        {
            string json = JsonUtility.ToJson(EnvironmentData, true);
            string[] lines = json.Split('\n');
            json = string.Join("\n", lines.Take(1).Concat(lines.Skip(24)));
            return json;
        }
        
        private string CreateAniJson()
        {
            string json = JsonUtility.ToJson(animal.DNA, true);
            string[] lines = json.Split('\n');
            json = string.Join("\n", lines.Take(1).Concat(lines.Skip(4)));
            return json;
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
                    using (StreamWriter writer = new StreamWriter(plotFilePath, true))
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



using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Animal
{
    [CreateAssetMenu(menuName = "Data/Genome Parser")]
    public class GenomeParser : ScriptableObject
    {
        [Header("Save new Genome in")]
        public string FileName = "placeholder.json";

        public List<TextAsset> GenomesToLoad = new List<TextAsset>();
        
        public void SaveToJson(Brain brain, DNA dna)
        {
            Genome genome = new Genome(brain, dna);

            string json = JsonUtility.ToJson(genome, true);
            string filePath = Path.Combine(Application.dataPath,"Genomes", FileName);

            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.Write(json);
                }

            }
            catch (Exception e)
            {
                Debug.LogError("Can't write into CSV file because of " + e);
            }
            //File.WriteAllText(Application.dataPath + "/Genomes/"+ filename, json);
        }

        public Genome LoadFromJson(TextAsset genomeFile)
        {
            string json = genomeFile.text;
            Genome genome = JsonUtility.FromJson<Genome>(json);

            return genome;
        }

        public List<Genome> LoadAllGenomes()
        {
            List<Genome> genomes = new List<Genome>();
            foreach (var genomeFile in GenomesToLoad)
            {
                genomes.Add(LoadFromJson(genomeFile));
            }

            return genomes;
        }
    }
}
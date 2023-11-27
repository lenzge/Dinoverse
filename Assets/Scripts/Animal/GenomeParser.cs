using System;
using System.IO;
using UnityEngine;

namespace Animal
{
    public class GenomeParser : MonoBehaviour
    {
        public string FileName = "placeholder.json";
        
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

        public Genome LoadFromJson()
        {
            string json = File.ReadAllText(Application.dataPath + "/Genomes/" + FileName);
            Genome genome = JsonUtility.FromJson<Genome>(json);

            return genome;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Animal
{
    [Serializable]
    public class Genome
    {
        [Header("Brain")] 
        public int[] NetworkShape;
        public float[] Weights;
        public float[] Biases;
        public int[] Inputs;
        public int[] Neurons;
        
        [Header("DNA")] 
        public int[] LifeExpectation;
        public int[] Weight;
        public float[] MutationAmount;
        public float[] MutationChance;
        public float[] EatingSpeed;
        public int[] VisualRadius;
        public int[] NumRaycasts;
        public int[] AngleBetweenRaycasts;
        public int[] HiddenLayer;
        public int[] MaxNeurons;
        public int[] MinNeurons;
        public int[] MovementSpeed;
        public int[] SexualMaturity;
        public int[] Menopause;
        public int[] LitterSize;

        public Genome(Brain brain, DNA dna)
        {
            NetworkShape = brain.NetworkShape;
            
            int layerCount = brain.Layers.Length;
            Inputs = new int[layerCount];
            Neurons = new int[layerCount];
            List<float> WeightList = new List<float>();
            List<float> BiasesList = new List<float>();

            for (int l = 0; l < layerCount; l++)
            {
                Inputs[l] = brain.Layers[l].Inputs;
                Neurons[l] = brain.Layers[l].Neurons;
                
                for(int i = 0; i < Neurons[l]; i++)
                {
                    for(int j = 0; j < Inputs[l]; j++)
                    {
                        WeightList.Add(brain.Layers[l].Weights[i, j]);
                    }

                    BiasesList.Add(brain.Layers[l].Biases[i]);
                }
            }

            Weights = WeightList.ToArray();
            Biases = BiasesList.ToArray();
            
            LifeExpectation = dna.LifeExpectation;
            Weight = dna.Weight;
            MutationAmount = dna.MutationAmount;
            MutationChance = dna.MutationChance;
            EatingSpeed = dna.EatingSpeed;
            VisualRadius = dna.VisualRadius;
            NumRaycasts = dna.NumRaycasts;
            AngleBetweenRaycasts = dna.AngleBetweenRaycasts;
            HiddenLayer = dna.HiddenLayer;
            MaxNeurons = dna.MaxNeurons;
            MinNeurons = dna.MinNeurons;
            MovementSpeed = dna.MovementSpeed;
            SexualMaturity = dna.SexualMaturity;
            Menopause = dna.Menopause;
            LitterSize = dna.LitterSize;
        }

        public void LoadGenome(Brain brain, DNA dna)
        {
            int iterWeights = 0;
            int iterBiases = 0;
            brain.NetworkShape = NetworkShape;
            int layerCount = Inputs.Length;
            brain.Layers = new Brain.Layer[layerCount];
            for (int l = 0; l < layerCount; l++)
            {
                brain.Layers[l] = new Brain.Layer(Inputs[l], Neurons[l]);
                
                for (int i = 0; i < Neurons[l]; i++)
                {
                    for (int j = 0; j < Inputs[l]; j++)
                    {
                        brain.Layers[l].Weights[i, j] = Weights[iterWeights];
                        iterWeights += 1;
                    }

                    brain.Layers[l].Biases[i] = Biases[iterBiases];
                    iterBiases += 1;

                }
            }
            
            dna.LifeExpectation = LifeExpectation;
            dna.Weight = Weight;
            dna.MutationAmount = MutationAmount;
            dna.MutationChance = MutationChance;
            dna.EatingSpeed = EatingSpeed;
            dna.VisualRadius = VisualRadius;
            dna.NumRaycasts = NumRaycasts;
            dna.AngleBetweenRaycasts = AngleBetweenRaycasts;
            dna.HiddenLayer = HiddenLayer;
            dna.MaxNeurons = MaxNeurons;
            dna.MinNeurons = MinNeurons;
            dna.MovementSpeed = MovementSpeed;
            dna.SexualMaturity = SexualMaturity;
            dna.Menopause = Menopause;
            dna.LitterSize = LitterSize;
        }

    }
}
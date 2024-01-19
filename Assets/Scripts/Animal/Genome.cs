using System;
using System.Collections.Generic;
using System.Linq;
using Classification;
using UnityEngine;

namespace Animal
{
    [Serializable]
    public class Genome
    {
        public string Subspecies = "No name";
        
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
        public float[] Carnivore;
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


        public Point CreatePoint(string name, float color)
        {
            return new Point(new []
            {
                LifeExpectation[0], Weight[0], MutationAmount[0], MutationChance[0], EatingSpeed[0], Carnivore[0],
                VisualRadius[0], AngleBetweenRaycasts[0], MovementSpeed[0], SexualMaturity[0], Menopause[0], LitterSize[0]
            }, name, color);
        }

        public float[] CreateFeatureWeights()
        {
            return new [] { Var(LifeExpectation), Var(Weight), Var(MutationAmount), Var(MutationChance), 
                Var(EatingSpeed), Var(Carnivore), Var(VisualRadius), Var(AngleBetweenRaycasts), Var(MovementSpeed), 
                Var(SexualMaturity), Var(Menopause), Var(LitterSize) };
        }

        public void CompareGenomes(Genome other)
        {
            float weightsDiff = ArrayDiff(Weights, other.Weights) * 3;
            float biasesDiff = ArrayDiff(Biases, other.Biases) * 3;
            
            float lifeExpectationDiff = Diff(LifeExpectation, other.LifeExpectation);
            float mutationAmountDiff = Diff(MutationAmount, other.MutationAmount);
            float mutationChanceDiff = Diff(MutationChance, other.MutationChance);
            float eatingSpeedDiff = Diff(EatingSpeed, other.EatingSpeed);
            float carnivoreDiff = Diff(Carnivore, other.Carnivore);
            float visualRadiusDiff = Diff(VisualRadius, other.VisualRadius);
            float angleBetweenRaycastsDiff = Diff(AngleBetweenRaycasts, other.AngleBetweenRaycasts);
            float movementSpeedDiff = Diff(MovementSpeed, other.MovementSpeed);
            float sexualMaturityDiff = Diff(SexualMaturity, other.SexualMaturity);
            float menopauseDiff = Diff(Menopause, other.Menopause);
            float litterSizeDiff = Diff(LitterSize, other.LitterSize);

            float diff = (weightsDiff + biasesDiff + lifeExpectationDiff + mutationAmountDiff + mutationChanceDiff
                          + eatingSpeedDiff + carnivoreDiff + visualRadiusDiff + angleBetweenRaycastsDiff + movementSpeedDiff +
                          sexualMaturityDiff + menopauseDiff + litterSizeDiff) / 13;
            
            Debug.LogWarning(diff);

            /*Debug.LogWarning($"weightsDiff: {weightsDiff}, biasesDiff: {biasesDiff}, lifeExpectationDiff: {lifeExpectationDiff}," +
                             $" mutationAmountDiff: {mutationAmountDiff}, mutationChanceDiff: {mutationChanceDiff}," +
                             $" eatingSpeedDiff: {eatingSpeedDiff}, visualRadiusDiff: {visualRadiusDiff}, " +
                             $"angleBetweenRaycastsDiff: {angleBetweenRaycastsDiff}, movementSpeedDiff: {movementSpeedDiff}," +
                             $" sexualMaturityDiff: {sexualMaturityDiff}, menopauseDiff: {menopauseDiff}, litterSizeDiff: {litterSizeDiff},");*/
        }

        public float Diff(int[] own, int[] other)
        {
            float variance = own[2] - own[1];
            if (variance == 0) variance = 1;
            return Mathf.Abs(own[0] - other[0])/ variance;
        }
        public float Diff(float[] own, float[] other)
        {
            float variance = own[2] - own[1];
            if (variance == 0) variance = 1;
            return Mathf.Abs(own[0] - other[0])/ variance;
        }
        
        public float Var(int[] own)
        {
            float variance = own[2] - own[1];
            if (variance == 0) variance = 1;
            return variance;
        }
        
        public float Var(float[] own)
        {
            float variance = own[2] - own[1];
            if (variance == 0) variance = 1;
            return variance;
        }
        
        public float ArrayDiff(float[] own, float[] other)
        {
            float diff = 0;
            for (int i = 0; i < own.Length; i++)
            {
                diff += Mathf.Abs(own[i] - other[i]);
            }

            return diff / own.Length;
        }
        
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
            Carnivore = dna.Carnivore;
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
            dna.Carnivore = Carnivore;
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
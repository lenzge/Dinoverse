﻿using UnityEngine;
using Random = UnityEngine.Random;

namespace Animal
{
    /// <summary>
    /// Every parameter as array
    /// param[0] currentValue
    /// param[1] minValue
    /// param[2] maxValue
    /// </summary>
    public class DNA : MonoBehaviour
    {
        [Header("General")] 
        public int[] LifeExpectation;
        public int[] Weight;
        
        [Space]
        [Header("Mutation")]
        public float[] MutationAmount;
        public float[] MutationChance;

        [Space] 
        [Header("Food")]
        public float[] EatingSpeed;
        public float[] Carnivore;
        
        [Space] 
        [Header("Eyes")] 
        public int[] VisualRadius;
        public int[] NumRaycasts;
        public int[] AngleBetweenRaycasts;
        
        [Space] 
        [Header("Brain")] 
        public int[] HiddenLayer;
        public int[] MaxNeurons;
        public int[] MinNeurons;

        [Space] 
        [Header("Movement")] 
        public int[] MovementSpeed;
        
        [Space] 
        [Header("Reproduction")] 
        public int[] SexualMaturity;
        public int[] Menopause;
        public int[] LitterSize;
        

        public void Mutate()
        {
            //This ensures that the random numbers we generate aren't the same pattern each time. 
            Random.InitState((int)System.DateTime.Now.Ticks);
            
            MutateFloatParam(MutationChance);
            MutateFloatParam(MutationAmount);
            
            MutateIntParam(LifeExpectation);
            MutateIntParam(Weight);
            MutateFloatParam(EatingSpeed);
            MutateFloatParam(Carnivore);
            MutateIntParam(VisualRadius);
            //MutateIntParam(NumRaycasts); Will stay constant
            MutateIntParam(AngleBetweenRaycasts);
            MutateIntParam(MovementSpeed);
            //MutateIntParam(SexualMaturity);
            if (SexualMaturity[0] < SexualMaturity[2]) SexualMaturity[0] += 1; //Random.value < 0.5 && 
            MutateIntParam(Menopause);
            MutateIntParam(LitterSize);
        }

        public void CreateNewDNA()
        {
            Random.InitState((int)System.DateTime.Now.Ticks);
            
            CreateRandomFloatParam(MutationChance);
            CreateRandomFloatParam(MutationAmount);
            
            CreateRandomIntParam(LifeExpectation);
            CreateRandomIntParam(Weight);
            CreateRandomFloatParam(EatingSpeed);
            CreateRandomFloatParam(Carnivore);
            CreateRandomIntParam(VisualRadius);
            CreateRandomIntParam(NumRaycasts);
            CreateRandomIntParam(AngleBetweenRaycasts);
            CreateRandomIntParam(HiddenLayer);
            CreateRandomIntParam(MaxNeurons);
            CreateRandomIntParam(MinNeurons);
            CreateRandomIntParam(MovementSpeed);
            //CreateRandomIntParam(SexualMaturity);
            SexualMaturity[0] = SexualMaturity[1]; 
            CreateRandomIntParam(Menopause);
            CreateRandomIntParam(LitterSize);
        }

        public void CopyValuesFrom(DNA otherDNA)
        {
            if (otherDNA != null)
            {
                LifeExpectation = CopyIntArray(otherDNA.LifeExpectation);
                Weight = CopyIntArray(otherDNA.Weight);
                MutationAmount = CopyFloatArray(otherDNA.MutationAmount);
                MutationChance = CopyFloatArray(otherDNA.MutationChance);
                EatingSpeed = CopyFloatArray(otherDNA.EatingSpeed);
                Carnivore = CopyFloatArray(otherDNA.Carnivore);
                VisualRadius = CopyIntArray(otherDNA.VisualRadius);
                NumRaycasts = CopyIntArray(otherDNA.NumRaycasts);
                AngleBetweenRaycasts = CopyIntArray(otherDNA.AngleBetweenRaycasts);
                HiddenLayer = CopyIntArray(otherDNA.HiddenLayer);
                MaxNeurons = CopyIntArray(otherDNA.MaxNeurons);
                MinNeurons = CopyIntArray(otherDNA.MinNeurons);
                MovementSpeed = CopyIntArray(otherDNA.MovementSpeed);
                SexualMaturity = CopyIntArray(otherDNA.SexualMaturity);
                Menopause = CopyIntArray(otherDNA.Menopause);
                LitterSize = CopyIntArray(otherDNA.LitterSize);
            }
            else
            {
                Debug.LogWarning("Cannot copy values from null DNA.");
            }
        }
        
        public void CrossoverDNA(DNA otherDNA)
        {
            Random.InitState((int)System.DateTime.Now.Ticks);
            if (otherDNA != null)
            {
                if(Random.value <= 0.5) LifeExpectation = CopyIntArray(otherDNA.LifeExpectation);
                if(Random.value <= 0.5) Weight = CopyIntArray(otherDNA.Weight);
                if(Random.value <= 0.5) MutationAmount = CopyFloatArray(otherDNA.MutationAmount);
                if(Random.value <= 0.5) MutationChance = CopyFloatArray(otherDNA.MutationChance);
                if(Random.value <= 0.5) EatingSpeed = CopyFloatArray(otherDNA.EatingSpeed);
                if(Random.value <= 0.5) Carnivore = CopyFloatArray(otherDNA.Carnivore);
                if(Random.value <= 0.5) VisualRadius = CopyIntArray(otherDNA.VisualRadius);
                if(Random.value <= 0.5) AngleBetweenRaycasts = CopyIntArray(otherDNA.AngleBetweenRaycasts);
                if(Random.value <= 0.5) MovementSpeed = CopyIntArray(otherDNA.MovementSpeed);
                if(Random.value <= 0.5) SexualMaturity = CopyIntArray(otherDNA.SexualMaturity);
                if(Random.value <= 0.5) Menopause = CopyIntArray(otherDNA.Menopause);
                if(Random.value <= 0.5) LitterSize = CopyIntArray(otherDNA.LitterSize);
            }
            else
            {
                Debug.LogWarning("Cannot crossover values from null DNA.");
            }
        }

        private int[] CopyIntArray(int[] array)
        {
            int[] newArray = new int[array.Length];
            System.Array.Copy(array, newArray, array.Length);
            return newArray;
        }
        
        private float[] CopyFloatArray(float[] array)
        {
            float[] newArray = new float[array.Length];
            System.Array.Copy(array, newArray, array.Length);
            return newArray;
        }
        
        private void MutateIntParam(int[] param)
        {
            float rando = Random.value;
            //Debug.Log($"Random value: {rando}");
            if (rando < MutationChance[0])
            {
                int change = (int) (param[0] * MutationAmount[0] * Random.Range(-1, 1));
                //Debug.Log($"Old Value: {param[0]}. Change of {change}");
                param[0] += change;
                if (param[0] < param[1])
                {
                    param[0] = param[1];
                }
                else if (param[0] > param[2])
                {
                    param[0] = param[2];
                }
            }
        }
        
        private void MutateFloatParam(float[] param)
        {
            if (Random.value < MutationChance[0])
            {
                param[0] += param[2] * MutationAmount[0] * Random.Range(-1, 1);
                if (param[0] < param[1])
                {
                    param[0] = param[1];
                }
                else if (param[0] > param[2])
                {
                    param[0] = param[2];
                }
            }
        }

        private void CreateRandomIntParam(int[] param)
        {
            param[0] = Random.Range(param[1], param[2]);
        }
        
        private void CreateRandomFloatParam(float[] param)
        {
            param[0] = Random.Range(param[1], param[2]);
        }
    }
}
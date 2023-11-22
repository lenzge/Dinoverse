using System;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Animal
{
    public class Uterus : Organ
    {
        public DNA DNA;
        private AnimalCreator animalCreator;

        private int childCount;
        public int ReproductionEnergy;

        public override void Init(bool isChild = false)
        {
            // TODO make animal creator static
            childCount = 0;
            ReproductionEnergy = 0;
            animalCreator = GameObject.Find("AnimalCreator").GetComponent<AnimalCreator>();
        }

        public bool TryToReproduce(AnimalController parent)
        {
            if (parent.Age >= DNA.SexualMaturity[0] 
                && childCount <= DNA.Menopause[0]
                && ReproductionEnergy >= 3)
            {
                childCount += 1;
                int litterSize = LitterSize(parent.Generation, parent.Age);
                Debug.LogWarning($"[{parent.name}] Reproduced {litterSize} times");
                for (int i = 0; i < litterSize; i++)
                {
                    //create a new agent, and set its position to the parent's position + a random offset in the x and z directions (so they don't all spawn on top of each other)
                    //animalCreator.SpawnChildAnimal(parent.Key, parent.Generation + 1, parent, i*147);
                }
                ReproductionEnergy = 0;
                return true;
            }

            return false;
        }

        public int LitterSize(int generation, int age)
        {
            if (animalCreator.GetAnimalCount() >= 300)
            {
                return 1;
            }
            if (animalCreator.GetAnimalCount() >= 200)
            {
                if (childCount > 3) return 3;
                else return childCount;
            }
            else
            {
                int litterSize = 0;
                
                if (animalCreator.pastTimeSteps < animalCreator.stopRespawn) litterSize += 3;
                
                if (childCount > 5) litterSize += 5;
                else litterSize += childCount;
                
                if (generation > 5) litterSize += 5;
                else litterSize += generation;
                
                if (age > 500) litterSize += 5;
                else litterSize += age / 100;

                if (litterSize > 10) return 10;
                else return  litterSize;
            }
        }

        public int NeededReproductionEnergy(int generation)
        {
            if (generation <= 5)
            {
                return 2;
            }
            if (generation <= 10)
            {
                return 3;
            }
            if (generation <= 15)
            {
                return 4;
            }
            else
            {
                return 5;
            }
        }

        public float SexualMaturityLevel(AnimalController controller)
        {
            return (float) controller.Age / DNA.SexualMaturity[0];
        }
        
        public float ReproductionEnergyLevel(AnimalController controller)
        {
            return (float) ReproductionEnergy / NeededReproductionEnergy(controller.Generation);
        }

        public bool IsInMenopause()
        {
            if (childCount == DNA.Menopause[0])
            {
                return true;
            }

            return false;
        }

        public int GetChildCount()
        {
            return childCount;
        }
        
    }
}
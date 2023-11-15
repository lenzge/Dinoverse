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
        private int reproductionEnergy;

        public override void Init(bool isChild = false)
        {
            // TODO make animal creator static
            childCount = 0;
            reproductionEnergy = 0;
            animalCreator = GameObject.Find("AnimalCreator").GetComponent<AnimalCreator>();
        }

        public bool TryToReproduce(AnimalController parent)
        {
            reproductionEnergy += 1;

            //if (parent.Age >= DNA.SexualMaturity[0] && reproductionEnergy >= animalCreator.GetReproductionEnergy())
            {
                childCount += 1;
                for (int i = 0; i < DNA.LitterSize[0]; i++)
                {
                    //create a new agent, and set its position to the parent's position + a random offset in the x and z directions (so they don't all spawn on top of each other)
                    animalCreator.SpawnChildAnimal(parent.Key, parent.Generation + 1, parent, i*147);
                }
                reproductionEnergy = 0;
                return true;
            }

            return false;
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
using System;
using AI;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Animals
{
    public class ReproductionManager : MonoBehaviour
    {
        [SerializeField] public GameObject AnimalPrefab;
        [SerializeField] private int MaxChildCount;

        private int childCount = 0;
        private int reproductionEnergy = 0;
        private AnimalCreator animalCreator;

        private void Start()
        {
            childCount = 0;
            reproductionEnergy = 0;
            animalCreator = GameObject.Find("AnimalCreator").GetComponent<AnimalCreator>();
        }

        public bool TryToReproduce(AnimalController parent)
        {
            reproductionEnergy += 1;

            if (reproductionEnergy == animalCreator.GetReproductionEnergy())
            {
                childCount += 1;
                for (int i = 0; i < childCount*2; i++)
                {
                    //create a new agent, and set its position to the parent's position + a random offset in the x and z directions (so they don't all spawn on top of each other)
                    AnimalController child = animalCreator.SpawnAnimal(parent.Key, parent.GrandChild + 1);
                    child.MutationAmount = parent.MutationAmount;
                    child.MutationChance = parent.MutationChance;
                    child.Eyes.NumRaycasts = parent.Eyes.NumRaycasts;
                    child.Eyes.Radius = parent.Eyes.Radius;
                    child.Eyes.AngleBetweenRaycasts = parent.Eyes.AngleBetweenRaycasts;
                    //copy the parent's neural network to the child
                    child.Brain.layers = parent.Brain.copyLayers();
                    child.Brain.NetworkShape = parent.Brain.NetworkShape;
                    Debug.LogWarning($"Reproduced!");
                }
                reproductionEnergy = 0;
                return true;
            }

            return false;
        }

        public bool IsInMenopause()
        {
            if (childCount == MaxChildCount)
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
using System;
using DefaultNamespace;
using Enums;
using UnityEngine;
using Action = Enums.Action;
using Random = UnityEngine.Random;

namespace Animal
{
    public class Uterus : Organ
    {
        public int ReproductionEnergy;
        
        // Plot Data
        public int SoloChildCount { get; private set; }
        public int MutualChildCount {get; private set; }
        
        private Collider[] colliderBuffer;
        private SpawnType spawnType;

        public override void Init(bool isChild = false)
        {
            SoloChildCount = 0;
            MutualChildCount = 0;
            ReproductionEnergy = 0;
            colliderBuffer = new Collider[4];

            if (EnvironmentData.RandomSpawnPoint)
            {
                spawnType = SpawnType.Random;
            }
            else
            {
                spawnType = SpawnType.NearParent;
            }
            
        }

        public bool TryToReproduce(Layer species)
        {
            if (!CanReproduce()) return false;
            
            if (EnvironmentData.SexualReproduction && Physics.OverlapSphereNonAlloc(animalController.transform.position, 40, colliderBuffer,
                1 << (int) species) >= 4) // min 2, because of self interaction
            {
                foreach (var collider in colliderBuffer)
                {
                    if (collider.gameObject != animalController.gameObject && !collider.isTrigger)
                    {
                        AnimalController mate = collider.gameObject.GetComponentInParent<AnimalController>();
                        if (mate.Uterus.CanReproduce())  // && mate.CurrentAction == Action.Reproduce) -- to difficult! 
                        {
                            mate.EvaluateFitness();
                            animalController.EvaluateFitness();
                            Debug.LogWarning($"[{animalController.name}] Reproduced {animalController.DNA.LitterSize[0]} times. Parents: {mate.name} with Fitness {mate.Fitness} and {animalController.name} with Fitness {animalController.Fitness}");
                            Debug.LogWarning($"[{mate.name}] Reproduced {mate.DNA.LitterSize[0]} times. Parents: {mate.name} with Fitness {mate.Fitness} and {animalController.name} with Fitness {animalController.Fitness}");
                            MutualChildCount += 1;
                            mate.Uterus.MutualChildCount += 1;
                            int bonuskids = animalController.AnimalCreator.BonusKids();
                            bool prio = false;
                            for (int i = 0; i < animalController.DNA.LitterSize[0]* bonuskids; i++)
                            {
                                animalController.AnimalCreator.CreateChildObject(prio,animalController.Key, animalController.Generation + 1, GenomeType.Crossover,spawnType, animalController, mate);
                                mate.Stomach.BurnCaloriesOnBirthGiving();
                            }
                            for (int i = 0; i < mate.DNA.LitterSize[0]* bonuskids; i++)
                            {
                                animalController.AnimalCreator.CreateChildObject(prio,mate.Key, mate.Generation + 1, GenomeType.Crossover,spawnType, mate, animalController);
                                mate.Stomach.BurnCaloriesOnBirthGiving();
                            }
                            ReproductionEnergy = 0;
                            mate.Uterus.ReproductionEnergy = 0;
                            
                            mate.StartCoroutine(mate.AnimationFreeze(Action.Reproduce));

                            return true;
                        }
                    }
                }
            }
            else if (!EnvironmentData.SexualReproduction)
            {
                SoloChildCount += 1; 
                int litterSize =  animalController.DNA.LitterSize[0];
                Debug.LogWarning($"[{animalController.name}] Reproduced {litterSize} times. Alone");
                for (int i = 0; i < litterSize; i++)
                {
                    animalController.AnimalCreator.CreateChildObject(false,animalController.Key, animalController.Generation + 1, GenomeType.Parent,spawnType, animalController);
                    animalController.Stomach.BurnCaloriesOnBirthGiving();
                }
                ReproductionEnergy = 0;
                return true;
            }

            return false;
        }

        private int LitterSize(int parentFitness, int mateFitness)
        {
            int meanFitness = (parentFitness + mateFitness) / 2;

            if (meanFitness <= 0)
            {
                return 6;
            }
            else if (meanFitness < animalController.AnimalCreator.FitnessToScore / 2)
            {
                return 7;
            }
            else if (meanFitness < animalController.AnimalCreator.FitnessToScore)
            {
                return 8;
            }
            else if (meanFitness == animalController.AnimalCreator.FitnessToScore)
            {
                return 9;
            }
            else if (meanFitness > animalController.AnimalCreator.FitnessToScore)
            {
                return 10;
            }
            else
            {
                return 1;
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

        public float SexualMaturityLevel()
        {
            return (float) animalController.Age / animalController.DNA.SexualMaturity[0];
        }
        
        public float ReproductionEnergyLevel()
        {
            if (animalController.EnvironmentData.ReproductionEnergy != 0)
            {
                return (float) ReproductionEnergy / animalController.EnvironmentData.ReproductionEnergy;
            }
            else
            {
                return 1;
            }
        }

        public bool IsInMenopause()
        {
            if (SoloChildCount == animalController.DNA.Menopause[0])
            {
                return true;
            }

            return false;
        }

        public int GetChildCountSolo()
        {
            return SoloChildCount;
        }
        public int GetChildCountMutual()
        {
            return MutualChildCount;
        }

        public bool CanReproduce()
        {
            if (animalController.Age >= animalController.DNA.SexualMaturity[0]
                && ReproductionEnergy >= animalController.EnvironmentData.ReproductionEnergy)
            {
                return true;
            }

            return false;
        }
    }
}
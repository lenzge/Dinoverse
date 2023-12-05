using System;
using DefaultNamespace;
using Enums;
using Unity.VisualScripting;
using UnityEngine;
using Action = Enums.Action;
using Random = UnityEngine.Random;

namespace Animal
{
    public class Uterus : Organ
    {
        public DNA DNA;
        private AnimalCreator animalCreator;

        private int soloChildCount;
        private int mutualChildCount;
        private int trySoHard;
        private Collider[] colliderBuffer;
        public int ReproductionEnergy;

        public override void Init(bool isChild = false)
        {
            // TODO make animal creator static
            soloChildCount = 0;
            mutualChildCount = 0;
            ReproductionEnergy = 0;
            trySoHard = 0;
            colliderBuffer = new Collider[4];
            animalCreator = GameObject.Find("AnimalCreator").GetComponent<AnimalCreator>();
        }

        public bool TryToReproduce(AnimalController parent, Layer species)
        {
            if (CanReproduce(parent))
            {
                trySoHard += 1;
                if (Physics.OverlapSphereNonAlloc(parent.transform.position, parent.AnimalCreator.ReproductionRadius, colliderBuffer,
                    1 << (int) species) >= 4) // min 2, because of self interaction
                {
                    foreach (var collider in colliderBuffer)
                    {
                        if (collider.gameObject != parent.gameObject && !collider.isTrigger)
                        {
                            AnimalController mate = collider.gameObject.GetComponentInParent<AnimalController>();
                            if (mate.Uterus.CanReproduce(mate) && (mate.CurrentAction == Action.Reproduce || !parent.AnimalCreator.MutualReproduction)) //  -- to difficult!
                            {
                                mutualChildCount += 1;
                                int litterSize = LitterSize(parent.Generation, parent.Age);
                                litterSize = 3;
                                Debug.LogWarning($"[{parent.name}] Reproduced {litterSize} times. Parents: {mate} and {parent}");
                                for (int i = 0; i < litterSize; i++)
                                {
                                    //create a new agent, and set its position to the parent's position + a random offset in the x and z directions (so they don't all spawn on top of each other)
                                    //animalCreator.CreateChildObject(parent.Key, parent.Generation + 1, SpawnType.Random, parent);
                                    animalCreator.CreateChildObject(parent.Key, parent.Generation + 1, GenomeType.Parent,SpawnType.Random, parent);
                                    animalCreator.CreateChildObject(parent.Key, parent.Generation + 1, GenomeType.Parent,SpawnType.Random, mate);
                                    animalCreator.CreateChildObject(parent.Key, parent.Generation + 1, GenomeType.Crossover,SpawnType.Random, parent, mate);
                                    animalCreator.CreateChildObject(parent.Key, parent.Generation + 1, GenomeType.Crossover,SpawnType.Random, parent, mate);
                                    animalCreator.CreateChildObject(parent.Key, parent.Generation + 1, GenomeType.Crossover,SpawnType.Random, parent, mate);
                                }
                                ReproductionEnergy = 0;
                                trySoHard = 0;
                                return true;
                            }
                        }
                    }
                }
                if (trySoHard >= 50 && parent.AnimalCreator.SelfReproduction)
                {
                    soloChildCount += 1; 
                    int litterSize = 10;
                    Debug.LogWarning($"[{parent.name}] Reproduced {litterSize} times. ALONE");
                    for (int i = 0; i < litterSize; i++)
                    {
                        //create a new agent, and set its position to the parent's position + a random offset in the x and z directions (so they don't all spawn on top of each other)
                        //animalCreator.CreateChildObject(parent.Key, parent.Generation + 1, SpawnType.Random, parent);
                        animalCreator.CreateChildObject(parent.Key, parent.Generation + 1, GenomeType.Parent,SpawnType.Random, parent);
                    }
                    ReproductionEnergy = 0;
                    trySoHard = 0;
                    return true;
                }

            }

            return false;
        }

        private int LitterSize(int generation, int age)
        {
            if (animalCreator.GetAnimalCount() >= 300)
            {
                return 1;
            }
            if (animalCreator.GetAnimalCount() >= 200)
            {
                if (soloChildCount > 3) return 3;
                else return soloChildCount;
            }
            else
            {
                int litterSize = 0;
                
                if (animalCreator.pastTimeSteps < animalCreator.stopRespawn) litterSize += 3;
                
                if (soloChildCount > 5) litterSize += 5;
                else litterSize += soloChildCount;
                
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
            if (soloChildCount == DNA.Menopause[0])
            {
                return true;
            }

            return false;
        }

        public int GetChildCountSolo()
        {
            return soloChildCount;
        }
        public int GetChildCountMutual()
        {
            return mutualChildCount;
        }

        public bool CanReproduce(AnimalController animal)
        {
            if (animal.Age >= DNA.SexualMaturity[0]
                && soloChildCount <= DNA.Menopause[0]
                && ReproductionEnergy >= animal.AnimalCreator.ReproductionEnergy)
            {
                return true;
            }

            return false;
        }
    }
}
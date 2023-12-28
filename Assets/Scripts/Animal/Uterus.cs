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
                //trySoHard += 1;
                if (Physics.OverlapSphereNonAlloc(parent.transform.position, 40, colliderBuffer,
                    1 << (int) species) >= 4) // min 2, because of self interaction
                {
                    foreach (var collider in colliderBuffer)
                    {
                        if (collider.gameObject != parent.gameObject && !collider.isTrigger)
                        {
                            AnimalController mate = collider.gameObject.GetComponentInParent<AnimalController>();
                            if (mate.Uterus.CanReproduce(mate) && (mate.CurrentAction == Action.Reproduce || !parent.AnimalCreator.MutualReproduction)) //  -- to difficult!
                            {
                                mate.EvaluateFitness();
                                parent.EvaluateFitness();
                                int litterSize = LitterSize(parent.Fitness, mate.Fitness);
                                Debug.LogWarning($"[{parent.name}] Reproduced {litterSize} times. Parents: {mate.name} with Fitness {mate.Fitness} and {parent.name} with Fitness {parent.Fitness}");
                                mutualChildCount += 1;
                                bool prio = litterSize >= 5;
                                for (int i = 0; i < litterSize; i++)
                                {
                                    //create a new agent, and set its position to the parent's position + a random offset in the x and z directions (so they don't all spawn on top of each other)
                                    //animalCreator.CreateChildObject(parent.Key, parent.Generation + 1, SpawnType.Random, parent);
                                    animalCreator.CreateChildObject(prio,parent.Key, parent.Generation + 1, GenomeType.Parent,SpawnType.Random, parent);
                                    animalCreator.CreateChildObject(prio,parent.Key, parent.Generation + 1, GenomeType.Parent,SpawnType.Random, mate);
                                    animalCreator.CreateChildObject(prio,parent.Key, parent.Generation + 1, GenomeType.Crossover,SpawnType.Random, parent, mate);
                                    animalCreator.CreateChildObject(prio,parent.Key, parent.Generation + 1, GenomeType.Crossover,SpawnType.Random, parent, mate);
                                    animalCreator.CreateChildObject(prio,parent.Key, parent.Generation + 1, GenomeType.Crossover,SpawnType.Random, parent, mate);
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
                        animalCreator.CreateChildObject(false,parent.Key, parent.Generation + 1, GenomeType.Parent,SpawnType.Random, parent);
                    }
                    ReproductionEnergy = 0;
                    trySoHard = 0;
                    return true;
                }

            }

            return false;
        }

        private int LitterSize(int parentFitness, int mateFitness)
        {
            int meanFitness = (parentFitness + mateFitness) / 2;

            if (meanFitness <= 0)
            {
                return 2;
            }
            else if (meanFitness < animalCreator.FitnessToScore / 2)
            {
                return 3;
            }
            else if (meanFitness < animalCreator.FitnessToScore)
            {
                return 4;
            }
            else if (meanFitness == animalCreator.FitnessToScore)
            {
                return 5;
            }
            else if (meanFitness > animalCreator.FitnessToScore)
            {
                return 6;
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

        public float SexualMaturityLevel(AnimalController controller)
        {
            return (float) controller.Age / DNA.SexualMaturity[0];
        }
        
        public float ReproductionEnergyLevel(AnimalController controller)
        {
            return (float) ReproductionEnergy / animalCreator.ReproductionEnergy;
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
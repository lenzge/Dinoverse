using Enums;
using UnityEngine;

namespace Animal
{
    public class Weapon : Organ
    {
        private Collider[] colliderBuffer;

        public override void Init(bool isChild = false)
        {
            colliderBuffer = new Collider[4];
        }

        public bool TryToFight(Layer species)
        {
            if (animalController.IsKilled) return false;
            if (Physics.OverlapSphereNonAlloc(animalController.transform.position, 20, colliderBuffer,
                1 << (int) species) >= 4) // min 2, because of self interaction
            {
                foreach (var collider in colliderBuffer)
                {
                    if (collider.gameObject != animalController.gameObject && !collider.isTrigger)
                    {
                        AnimalController prey = collider.gameObject.GetComponentInParent<AnimalController>();
                        if (!prey.CanBeAttacked()) return false; // No double attack
                        //Debug.LogError($"{name} calories before fighting: {animalController.Stomach.GetCurrentCalories()}");
                        Attack(prey);
                        return true;
                    }

                }
            }

            return false;
        }

        public void TryToKill(AnimalController prey)
        {
            //Debug.LogError($"{name} calories after fighting: {animalController.Stomach.GetCurrentCalories()}");
            //Debug.LogError($"{prey.name} calories after fighting: {prey.Stomach.GetCurrentCalories()}");
            if (animalController.GetStrength() >= prey.GetStrength())
            {
                //Debug.LogWarning($"Attacker [{name}] killed prey {prey.name}, {prey.Stomach.GetCurrentCalories()}");
                animalController.Stomach.AddCalories(prey.Stomach.GetCurrentCalories(), FoodSource.meat);
                animalController.Uterus.ReproductionEnergy += 1;
                animalController.EatenAnimals += 1;
                prey.IsKilled = true;
                prey.KillIfDead();
            }
            else
            {
                //Debug.LogWarning($"Prey [{prey.name}] killed predator {name}. , {animalController.Stomach.GetCurrentCalories()}");
                prey.Stomach.AddCalories(animalController.Stomach.GetCurrentCalories(), FoodSource.meat);
                prey.Uterus.ReproductionEnergy += 1;
                prey.EatenAnimals += 1;
                animalController.IsKilled = true;
                animalController.KillIfDead();
            }
        }

        private void Attack(AnimalController prey)
        {
            if (prey.CurrentAction != Action.Fight)
            {
                prey.CurrentAction = Action.Fight;
                prey.StartCoroutine(prey.AnimationFreeze(Action.Fight));
                //Debug.LogError($"{prey.name} calories before fighting: {prey.Stomach.GetCurrentCalories()}");
            }
            
            animalController.StartCoroutine(animalController.AnimationFreeze(Action.Fight, prey));
        }
    }
}
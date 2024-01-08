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
            if (Physics.OverlapSphereNonAlloc(animalController.transform.position, 30, colliderBuffer,
                1 << (int) species) >= 4) // min 2, because of self interaction
            {
                foreach (var collider in colliderBuffer)
                {
                    if (collider.gameObject != animalController.gameObject && !collider.isTrigger)
                    {
                        AnimalController prey = collider.gameObject.GetComponentInParent<AnimalController>();
                        if (prey.IsKilled) return false; // No double attack
                        Attack(prey);
                        return true;
                    }

                }
            }

            return false;
        }

        public void Attack(AnimalController prey)
        {
            if (prey.CurrentAction != Action.Fight) StartCoroutine(prey.FightFreeze());
            if (animalController.GetStrength() >= prey.GetStrength())
            {
                prey.IsKilled = true;
                animalController.Stomach.AddCalories(prey.Stomach.GetCurrentCalories(), FoodSource.meat);
                Debug.LogWarning($"Attacker [{name}] got {prey.Stomach.GetCurrentCalories()} calories from prey {prey.name}.");
            }
            else if (prey.CurrentAction != Action.Fight)
            {
                animalController.IsKilled = true;
                prey.Stomach.AddCalories(animalController.Stomach.GetCurrentCalories(), FoodSource.meat);
                Debug.LogWarning($"Prey [{prey.name}] got {animalController.Stomach.GetCurrentCalories()} calories from predator {name}.");
            }

        }
    }
}
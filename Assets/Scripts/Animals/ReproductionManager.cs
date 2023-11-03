using UnityEngine;

namespace Animals
{
    public class ReproductionManager : MonoBehaviour
    {
        [SerializeField] public GameObject AnimalPrefab;
        [SerializeField] private int MaxChildCount;
        
        private int childCount = 0;
        private int reproductionEnergy = 0;

        public bool TryToReproduce(Transform characterTransform, dynamic brain)
        {
            reproductionEnergy += 1;

            if (reproductionEnergy == 2)
            {
                childCount += 1;
                for (int i = 0; i < childCount; i++)
                {
                    BearAnimal(brain.reproduce(brain), characterTransform.position);
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
        
        private void BearAnimal(dynamic animalBrain, Vector3 position)
        {
            Vector3 spawnPosition = new Vector3(position.x + Random.Range(2,10), 0,
                position.z +Random.Range(2,10));
            GameObject newAnimal = Instantiate(AnimalPrefab, spawnPosition, Quaternion.identity);
            newAnimal.GetComponent<AnimalController>().Brain = animalBrain;
        }
    }
}
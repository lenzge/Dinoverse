using System;
using System.Collections.Generic;
using DefaultNamespace;
using Enums;
using UnityEngine;

namespace Animal
{
    
    public class Eyes : Organ
    {
        public DNA DNA;
        
        private float[] distances;
        //private Collider[] animalBuffer;
        //private AnimalController[] nearestAnimals;
        //private float[] mateInfos;

        private int mateAmount = 3;
        private int mostEatenTreesSeen;

        public override void Init(bool isChild = false)
        {
            distances = new float[DNA.NumRaycasts[0] * 2];
            //animalBuffer = new Collider[3];
            //nearestAnimals = new AnimalController[3];
            //mateInfos = new float[mateAmount * 4];
            mostEatenTreesSeen = 1; // no zero devision
        }

        public float[] LookAround(Transform characterTransform, Layer food)
        {
            // Look for Food
            RaycastHit hit;
            for (int i = 0; i < DNA.NumRaycasts[0]; i++)
            {
                float angle = ((2 * i + 1 - DNA.NumRaycasts[0]) * (float) DNA.AngleBetweenRaycasts[0] / 2);
                // Rotate the direction of the raycast by the specified angle around the y-axis of the agent
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
                Vector3 rayDirection = rotation * characterTransform.forward;
                // Increase the starting point of the raycast by 0.1 units
                Vector3 rayStart = characterTransform.position + Vector3.up * 0.1f;
                if (Physics.Raycast(rayStart, rayDirection, out hit, DNA.VisualRadius[0], 1 << (int) food))
                {
                    // Draw a line representing the raycast in the scene view for debugging purposes
                    Debug.DrawRay(rayStart, rayDirection * hit.distance, Color.red);
                    distances[i] = hit.distance / DNA.VisualRadius[0];
                }
                else
                {
                    // Draw a line representing the raycast in the scene view for debugging purposes
                    Debug.DrawRay(rayStart, rayDirection * DNA.VisualRadius[0], Color.yellow);
                    // If no food object is detected, set the distance to the maximum length of the raycast
                    distances[i] = 1;
                }
            }
            
            // Look for Water
            for (int i = 0; i < DNA.NumRaycasts[0]; i++)
            {
                float angle = ((2 * i + 1 - DNA.NumRaycasts[0]) * (float) DNA.AngleBetweenRaycasts[0] / 2);
                // Rotate the direction of the raycast by the specified angle around the y-axis of the agent
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
                Vector3 rayDirection = rotation * characterTransform.forward;
                // Increase the starting point of the raycast by 0.1 units
                Vector3 rayStart = characterTransform.position + Vector3.up * 0.1f;
                if (Physics.Raycast(rayStart, rayDirection, out hit, DNA.VisualRadius[0], 1 << (int) Layer.Water))
                {
                    // Draw a line representing the raycast in the scene view for debugging purposes
                    Debug.DrawRay(rayStart, rayDirection * hit.distance, Color.blue);
                    distances[DNA.NumRaycasts[0] + i] = hit.distance / DNA.VisualRadius[0];
                }
                else
                {
                    // Draw a line representing the raycast in the scene view for debugging purposes
                    Debug.DrawRay(rayStart, rayDirection * DNA.VisualRadius[0], Color.yellow);
                    // If no food object is detected, set the distance to the maximum length of the raycast
                    distances[DNA.NumRaycasts[0] + i] = 1;
                }
            }
            

            return distances;
        }

        public float[] LookForMate(Transform characterTransform, Layer species)
        {
            Collider[] animalBuffer = new Collider[mateAmount * 4]; // more collisions for all that are not ready
            AnimalController[] nearestAnimals = new AnimalController[mateAmount];
            float[] mateInfos = new float[mateAmount * 4];
            
            int searchIterations = 10;
            int baseRadius = 40;
            //Look for mates
            
            int nearestAnimalIter = 0;
            for (int a = mateAmount+1; a > 1; a--)
            {
                for (int i = 1; i <= searchIterations; i++)
                {
                    if (Physics.OverlapSphereNonAlloc(characterTransform.position, baseRadius*i, animalBuffer,
                        1 << (int) species) >= a)
                    {
                        for (int j = 0; j < a; j++)
                        {
                            if (animalBuffer[j].gameObject != gameObject &&
                                nearestAnimalIter < mateAmount &&
                                animalBuffer[j].gameObject.GetComponent<AnimalController>().Uterus.CanReproduce(animalBuffer[j].gameObject.GetComponent<AnimalController>()))
                            {
                                //Debug.LogWarning($"[{characterTransform.gameObject}] Found Partner {animalBuffer[j].gameObject} in {i} iterations");
                                nearestAnimals[nearestAnimalIter++] = animalBuffer[j].gameObject.GetComponent<AnimalController>();
                            }
                        }

                        a = 0;
                        break;
                    }
                }
            }
            

            foreach (var animal in nearestAnimals)
            {
                if (animal == null) break;
                if (animal.EatenTrees > mostEatenTreesSeen) mostEatenTreesSeen = animal.EatenTrees;
            }
            
            
            int mateIter = 0;
            
            for (int i = 0; i < mateAmount; i++)
            {
                if (nearestAnimals[i] == null)
                {
                    mateInfos[mateIter++] = 0;
                    mateInfos[mateIter++] = 0;
                    mateInfos[mateIter++] = 0;
                    mateInfos[mateIter++] = 0;
                }
                else
                {
                    Vector3 direction = (nearestAnimals[i].transform.position - characterTransform.position).normalized;
                    float distance =  Vector3.Distance(nearestAnimals[i].transform.position, characterTransform.position);
                    mateInfos[mateIter++] = direction.x;
                    mateInfos[mateIter++] = direction.z;
                    mateInfos[mateIter++] = distance / (baseRadius * searchIterations);
                    mateInfos[mateIter++] = (float) nearestAnimals[i].EatenTrees / mostEatenTreesSeen;
                }
            }

            //Debug.LogWarning(ArrayToString(mateInfos));
            return mateInfos;

        }
        
        private string ArrayToString(float[] array)
        {
            // Convert the array elements to strings and join them with commas
            return "[" + string.Join(" , ", array) + "]";
        }
    }
}
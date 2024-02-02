using System;
using System.Collections.Generic;
using DefaultNamespace;
using Enums;
using UnityEngine;
using Action = Enums.Action;

namespace Animal
{
    
    public class Eyes : Organ
    {
        private float[] distances;
        private Collider[] colliderBuffer;

        public override void Init(bool isChild = false)
        {
            distances = new float[animalController.DNA.NumRaycasts[0] * 7];
            colliderBuffer = new Collider[40];
        }

        public float[] LookAround(Transform characterTransform, Layer food, Layer species)
        {
            // Look for Food
            RaycastHit hit;
            int j = 0;
            for (int i = 0; i < animalController.DNA.NumRaycasts[0]; i++)
            {
                float angle = ((2 * i + 1 - animalController.DNA.NumRaycasts[0]) * (float) animalController.DNA.AngleBetweenRaycasts[0] / 2);
                // Rotate the direction of the raycast by the specified angle around the y-axis of the agent
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
                Vector3 rayDirection = rotation * characterTransform.forward;
                // Increase the starting point of the raycast by 0.1 units
                Vector3 rayStart = characterTransform.position + Vector3.up * 0.1f;
                if (Physics.Raycast(rayStart, rayDirection, out hit, animalController.DNA.VisualRadius[0], 1 << (int) food))
                {
                    // Draw a line representing the raycast in the scene view for debugging purposes
                    //Debug.DrawRay(rayStart, rayDirection * hit.distance, Color.red, 0.5f);
                    distances[j++] = hit.distance / animalController.DNA.VisualRadius[0];
                    distances[j++] = hit.collider.GetComponent<Nurture>().CurrentCalories / 1000f;
                }
                else
                {
                    // Draw a line representing the raycast in the scene view for debugging purposes
                    //Debug.DrawRay(rayStart, rayDirection * animalController.DNA.VisualRadius[0], Color.yellow, 0.5f);
                    // If no food object is detected, set the distance to the maximum length of the raycast
                    distances[j] = 1;
                    distances[j++] = 0;
                }
            }
            
            // Look for Water
            for (int i = 0; i < animalController.DNA.NumRaycasts[0]; i++)
            {
                float angle = ((2 * i + 1 - animalController.DNA.NumRaycasts[0]) * (float) animalController.DNA.AngleBetweenRaycasts[0] / 2);
                // Rotate the direction of the raycast by the specified angle around the y-axis of the agent
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
                Vector3 rayDirection = rotation * characterTransform.forward;
                // Increase the starting point of the raycast by 0.1 units
                Vector3 rayStart = characterTransform.position + Vector3.up * 0.1f;
                if (Physics.Raycast(rayStart, rayDirection, out hit, animalController.DNA.VisualRadius[0], 1 << (int) Layer.Water))
                {
                    // Draw a line representing the raycast in the scene view for debugging purposes
                    //Debug.DrawRay(rayStart, rayDirection * hit.distance, Color.blue);
                    distances[animalController.DNA.NumRaycasts[0]*2 + i] = hit.distance / animalController.DNA.VisualRadius[0];
                }
                else
                {
                    // Draw a line representing the raycast in the scene view for debugging purposes
                    //Debug.DrawRay(rayStart, rayDirection * DNA.VisualRadius[0], Color.yellow);
                    // If no food object is detected, set the distance to the maximum length of the raycast
                    distances[animalController.DNA.NumRaycasts[0]*2 + i] = 1;
                }
            }

            j = 0;
            // Look for Friends
            for (int i = 0; i < animalController.DNA.NumRaycasts[0]; i++)
            {
                float angle = ((2 * i + 1 - animalController.DNA.NumRaycasts[0]) * (float) animalController.DNA.AngleBetweenRaycasts[0] / 2);
                // Rotate the direction of the raycast by the specified angle around the y-axis of the agent
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
                Vector3 rayDirection = rotation * characterTransform.forward;
                // Increase the starting point of the raycast by 0.1 units
                Vector3 rayStart = characterTransform.position + Vector3.up * 0.1f;
                if (Physics.Raycast(rayStart, rayDirection, out hit, animalController.DNA.VisualRadius[0], 1 << (int) species)) 
                    //&& hit.collider.GetComponentInParent<AnimalController>().Uterus.CanReproduce())
                {
                    // Draw a line representing the raycast in the scene view for debugging purposes
                    //Debug.DrawRay(rayStart, rayDirection * hit.distance, Color.blue,1);
                    distances[animalController.DNA.NumRaycasts[0]*3 + j++] = hit.distance / animalController.DNA.VisualRadius[0];
                    distances[animalController.DNA.NumRaycasts[0] * 3 + j++] = GetActionValue(hit.collider.GetComponentInParent<AnimalController>().CurrentAction);
                    distances[animalController.DNA.NumRaycasts[0] * 3 + j++] =
                        GetStrengthValue(hit.collider.GetComponentInParent<AnimalController>().GetStrength());
                    distances[animalController.DNA.NumRaycasts[0] * 3 + j++] =
                        GetSpeciesValue(hit.collider.GetComponentInParent<AnimalController>().Color);
                }
                else
                {
                    // Draw a line representing the raycast in the scene view for debugging purposes
                    //Debug.DrawRay(rayStart, rayDirection * animalController.DNA.VisualRadius[0], Color.yellow,1);
                    // If no food object is detected, set the distance to the maximum length of the raycast
                    distances[animalController.DNA.NumRaycasts[0]*3 + j++] = 1;
                    distances[animalController.DNA.NumRaycasts[0]*3 + j++] = 0;
                    distances[animalController.DNA.NumRaycasts[0]*3 + j++] = 0;
                    distances[animalController.DNA.NumRaycasts[0]*3 + j++] = 0;
                }
            }
            
            return distances;
        }

        private float GetSpeciesValue(float color)
        {
            return animalController.Color - color;
        }

        public float FoodDensity(Layer food)
        {
            float foodAmount = Physics.OverlapSphereNonAlloc(animalController.transform.position,
                animalController.DNA.VisualRadius[0], colliderBuffer, 1 << (int) food);
            foodAmount /= 20f;

            return Mathf.Clamp(foodAmount, 0, 1);
        }
        
        public float AnimalDensity(Layer species)
        {
            float animalAmount = Physics.OverlapSphereNonAlloc(animalController.transform.position,
                animalController.DNA.VisualRadius[0], colliderBuffer, 1 << (int) species);
            animalAmount /= 2; // characterController and extra collider
            animalAmount -= 1; // self
            animalAmount /= 19f;
            return Mathf.Clamp(animalAmount, 0, 1);
        }

        private string ArrayToString(float[] array)
        {
            // Convert the array elements to strings and join them with commas
            return "[" + string.Join(" , ", array) + "]";
        }

        private int GetActionValue(Action action)
        {
            switch (action)
            {
               case Action.Chill:
                   return 0;
               case Action.Eat:
                   return 0;
               case Action.Fight:
                   return -1;
               case Action.Reproduce:
                   return 1;
               default:
                   return 0;
            }
        }
        
        private float GetStrengthValue(float otherStrength)
        {
            float strengthValue = otherStrength / (float) animalController.GetStrength();
            strengthValue /= 2;
            Mathf.Clamp(strengthValue, -1, 1);
            return strengthValue;
        }
    }
}
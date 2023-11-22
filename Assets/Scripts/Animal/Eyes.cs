using System;
using Enums;
using UnityEngine;

namespace Animal
{
    
    public class Eyes : Organ
    {
        public DNA DNA;
        
        private float[] distances;

        public override void Init(bool isChild = false)
        {
            distances = new float[DNA.NumRaycasts[0] * 2];
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
            /*Vector3 thisRayStart = characterTransform.position + Vector3.up * 0.1f;
            if (Physics.Raycast(thisRayStart, characterTransform.forward, out hit, DNA.VisualRadius[0], 1 << (int) Layer.Water))
            {
                Debug.DrawRay(thisRayStart, characterTransform.forward * hit.distance, Color.blue);
                distances[DNA.NumRaycasts[0]] = hit.distance / DNA.VisualRadius[0];
            }
            else
            {
                // Draw a line representing the raycast in the scene view for debugging purposes
                Debug.DrawRay(thisRayStart, characterTransform.forward * DNA.VisualRadius[0], Color.yellow);
                // If no food object is detected, set the distance to the maximum length of the raycast
                distances[DNA.NumRaycasts[0]] = 1;
            }*/
            
            return distances;
        }
    }
}
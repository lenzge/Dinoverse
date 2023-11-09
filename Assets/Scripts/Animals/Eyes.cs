using System;
using Enums;
using UnityEngine;

namespace Animals
{
    
    public class Eyes : MonoBehaviour
    {
        public int Radius;
        public int NumRaycasts = 5;
        public int AngleBetweenRaycasts = 30;
        public float[] distances;

        public void SetEyeParams(int numRaycasts, int angleBetweenRaycasts, int radius)
        {
            Radius = radius;
            NumRaycasts = numRaycasts;
            AngleBetweenRaycasts = angleBetweenRaycasts;
            distances = new float[numRaycasts];
        }

        public Vector3 FindFood(Transform characterTransform, Layer food)
        {
            Vector3 nearestFoodPosition = Vector3.zero;
            Transform nearestFoodObject = null;
            RaycastHit hit;
            for (int i = 0; i < NumRaycasts; i++)
            {
                float angle = ((2 * i + 1 - NumRaycasts) * (float) AngleBetweenRaycasts / 2);
                // Rotate the direction of the raycast by the specified angle around the y-axis of the agent
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
                Vector3 rayDirection = rotation * characterTransform.forward;
                // Increase the starting point of the raycast by 0.1 units
                Vector3 rayStart = characterTransform.position + Vector3.up * 0.1f;
                
                if (Physics.Raycast(rayStart, rayDirection, out hit, Radius, 1 << (int) food))
                {
                    //Debug.DrawRay(rayStart, rayDirection * hit.distance, Color.red, 1);
                    
                    // Safe the nearest food position
                    if (nearestFoodPosition == Vector3.zero || 
                        Vector3.Distance(nearestFoodPosition, characterTransform.position) >
                        hit.distance)
                    {
                        nearestFoodPosition = hit.point;
                        nearestFoodObject = hit.collider.transform;
                    }
                }
                else
                {
                    //Debug.DrawRay(rayStart, rayDirection * Radius, Color.yellow, 1);
                }
            }

            if (nearestFoodObject != null)
            {
                return nearestFoodObject.position;
            }

            return nearestFoodPosition;
        }

        public float[] LookAround(Transform characterTransform, Layer food)
        {
            RaycastHit hit;
            for (int i = 0; i < NumRaycasts; i++)
            {
                float angle = ((2 * i + 1 - NumRaycasts) * (float) AngleBetweenRaycasts / 2);
                // Rotate the direction of the raycast by the specified angle around the y-axis of the agent
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
                Vector3 rayDirection = rotation * characterTransform.forward;
                // Increase the starting point of the raycast by 0.1 units
                Vector3 rayStart = characterTransform.position + Vector3.up * 0.1f;
                if (Physics.Raycast(rayStart, rayDirection, out hit, Radius, 1 << (int) food))
                {
                    // Draw a line representing the raycast in the scene view for debugging purposes
                    Debug.DrawRay(rayStart, rayDirection * hit.distance, Color.red);
                    distances[i] = hit.distance / Radius;
                }
                else
                {
                    // Draw a line representing the raycast in the scene view for debugging purposes
                    Debug.DrawRay(rayStart, rayDirection * Radius, Color.yellow);
                    // If no food object is detected, set the distance to the maximum length of the raycast
                    distances[i] = 1;
                }
            }
            /*Vector3 thisRayStart = characterTransform.position + Vector3.up * 0.1f;
            if (Physics.Raycast(thisRayStart, characterTransform.forward, out hit, Radius, 1 << (int) Layer.Water))
            {
                Debug.DrawRay(thisRayStart, characterTransform.forward * hit.distance, Color.blue);
                distances[5] = hit.distance / Radius;
            }
            else
            {
                // Draw a line representing the raycast in the scene view for debugging purposes
                Debug.DrawRay(thisRayStart, characterTransform.forward * Radius, Color.yellow);
                // If no food object is detected, set the distance to the maximum length of the raycast
                distances[5] = 1;
            }*/
            
            
            return distances;
        }

        /*public AnimalController LookForPartner(Transform characterTransform, Layer friend)
        {
            FriendCount = Physics.OverlapSphereNonAlloc(characterTransform.position, Radius, colliderBuffer,
                1 << (int) friend);

            for (int i = 0; i < FriendCount; i++)
            {
                if (BestPartner == null || 
                    BestPartner.ChildCount < colliderBuffer[i].gameObject.GetComponent<AnimalController>().ChildCount)
                {
                    BestPartner = colliderBuffer[i].gameObject.GetComponent<AnimalController>();
                }
            }

            Debug.Log($"best partner has {BestPartner.ChildCount} childs");
            return BestPartner;
        }*/

    }
}
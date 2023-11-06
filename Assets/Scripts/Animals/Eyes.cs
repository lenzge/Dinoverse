using Enums;
using UnityEngine;

namespace Animals
{
    
    public class Eyes : MonoBehaviour
    {
        public int Radius;
        public int NumRaycasts = 15;
        public float AngleBetweenRaycasts = 24;


        public Vector3 FindFood(Transform characterTransform, Layer food)
        {
            Vector3 nearestFoodPosition = Vector3.zero;
            Transform nearestFoodObject = null;
            RaycastHit hit;
            for (int i = 0; i < NumRaycasts; i++)
            {
                float angle = ((2 * i + 1 - NumRaycasts) * AngleBetweenRaycasts / 2);
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
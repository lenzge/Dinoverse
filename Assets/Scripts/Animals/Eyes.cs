using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

namespace Animals
{
    
    public class Eyes : MonoBehaviour
    {
        public int Radius;
        public int NumRaycasts = 10;
        public float AngleBetweenRaycasts = 20;

        [HideInInspector] public List<Vector3> FoodPositions = new List<Vector3>();
        [HideInInspector] public Vector3 NearestFoodPosition = Vector3.zero;
        [HideInInspector] public AnimalController BestPartner;
        [HideInInspector] public int FriendCount;
        [HideInInspector] public int EnemyCount;

        public float FoodDistance = 0;
        public float FoodAngle = 0;
        public bool isWall;

        private Collider[] colliderBuffer = new Collider[10];
        private float[] distances;
        
        private void Start()
        {
            distances = new float[NumRaycasts];
        }

        public void RaycastLookAround(Transform characterTransform, LAYER food, LAYER friend, LAYER enemy)
        {
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
                    // Draw a line representing the raycast in the scene view for debugging purposes
                    //Debug.DrawRay(rayStart, rayDirection * hit.distance, Color.red, 1);
                    distances[i] = hit.distance/Radius;
                    if (NearestFoodPosition == Vector3.zero || 
                        Vector3.Distance(NearestFoodPosition, characterTransform.position) >
                        hit.distance)
                    {
                        NearestFoodPosition = hit.point;
                        FoodDistance = hit.distance;
                        FoodAngle = Vector3.Angle(rayDirection, characterTransform.forward);
                    }
                }
                else
                {
                    // Draw a line representing the raycast in the scene view for debugging purposes
                    //Debug.DrawRay(rayStart, rayDirection * Radius, Color.yellow, 1);
                    // If no food object is detected, set the distance to the maximum length of the raycast
                    distances[i] = 1;
                }
            }

            if (Physics.Raycast(characterTransform.position, characterTransform.forward, 
                out hit, Radius/10, 1 << (int) LAYER.Water))
            {
                isWall = true;
            }
            

        }

        public AnimalController LookForPartner(Transform characterTransform, LAYER friend)
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
        }
        
        
        // 360degrees
        public void LookAround(Vector3 characterPosition, LAYER food, LAYER friend, LAYER enemy)
        {
            FoodPositions.Clear();
            int overlapCount = Physics.OverlapSphereNonAlloc(characterPosition, Radius, colliderBuffer,
                1 << (int) food);

            // for now
            if (overlapCount == 0)
            {
                FoodPositions.Add(Vector3.zero);
            }
            else
            {
                for (int i = 0; i < overlapCount; i++ )
                {
                    FoodPositions.Add(colliderBuffer[i].transform.position);
                    colliderBuffer[i].transform.position = new Vector3(colliderBuffer[i].transform.position.x, 20,
                        colliderBuffer[i].transform.position.z);
                }

                NearestFoodPosition = FoodPositions[0];

                for (int i = 1; i < overlapCount; i++)
                {
                    if (Vector3.Distance(NearestFoodPosition, characterPosition) >
                        Vector3.Distance(FoodPositions[i], characterPosition))
                    {
                        NearestFoodPosition = FoodPositions[i];
                    }
                }
            }

            
            
            FriendCount = Physics.OverlapSphereNonAlloc(characterPosition, Radius, colliderBuffer,
                1 << (int) friend);
            
            EnemyCount = Physics.OverlapSphereNonAlloc(characterPosition, Radius, colliderBuffer,
                1 << (int) enemy);
        }
        
    }
}
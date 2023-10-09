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

        [HideInInspector] public List<Vector3> FoodPositions = new List<Vector3>();
        [HideInInspector] public int FriendCount;
        [HideInInspector] public int EnemyCount;

        private Collider[] colliderBuffer = new Collider[10];


        // 360degrees
        public void LookAround(Vector3 characterPosition, LAYER food, LAYER friend, LAYER enemy)
        {
            FoodPositions.Clear();
            int overlapCount = Physics.OverlapSphereNonAlloc(characterPosition, Radius, colliderBuffer,
                1 << (int) food);

            for (int i = 0; i < overlapCount; i++ )
            {
                FoodPositions.Add(colliderBuffer[i].transform.position);
            }
            
            FriendCount = Physics.OverlapSphereNonAlloc(characterPosition, Radius, colliderBuffer,
                1 << (int) friend);
            
            EnemyCount = Physics.OverlapSphereNonAlloc(characterPosition, Radius, colliderBuffer,
                1 << (int) enemy);
        }
        
    }
}
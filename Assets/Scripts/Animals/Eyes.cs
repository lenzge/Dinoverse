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

        private Collider[] colliderBuffer = new Collider[10];


        // 360degrees
        public void LookAround(Transform characterTransform)
        {
            FoodPositions.Clear();
            int overlapCount = Physics.OverlapSphereNonAlloc(characterTransform.position, Radius, colliderBuffer,
                1 << (int) LAYER.Tree);

            for (int i = 0; i < overlapCount; i++ )
            {
                FoodPositions.Add(colliderBuffer[i].transform.position);
            }
        }
        
    }
}
using System;
using UnityEngine;

namespace Animals
{
    public class AnimalController: MonoBehaviour

    {
        public MovementController MovementController;
        
        private Transform characterTransform;

        private void Start()
        {
            characterTransform = transform;
        }

        private void Update()
        {
            MovementController.Move(characterTransform);
        }
    }
}
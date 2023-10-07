using UnityEngine;

namespace Animals
{
    public class MovementController : MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;
        [SerializeField] private float movementSpeed;
        [SerializeField] private Vector3 movementDirection;

        public void Move(Transform characterTransform)
        {
            // Rotate in the right direction
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            characterTransform.rotation = Quaternion.RotateTowards(characterTransform.rotation, toRotation, 700 * Time.deltaTime);
            
            // Move 
            characterController.SimpleMove(movementDirection.normalized * movementSpeed);
        }
    }
}
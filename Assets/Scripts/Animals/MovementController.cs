using UnityEngine;

namespace Animals
{
    public class MovementController : MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;
        [SerializeField] private float movementSpeed;
        
        private Vector3 movementDirection;

        public void Move(AnimalController animalController,Transform characterTransform)
        {
            // Rotate in the right direction
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            characterTransform.rotation = Quaternion.RotateTowards(characterTransform.rotation, toRotation, 700 * Time.deltaTime);
            
            // Move 
            characterController.SimpleMove(movementDirection.normalized * (movementSpeed * animalController.EnvironmentData.timeSpeed));
        }

        public void SetMoveDirection(Vector2 moveDirection)
        {
            movementDirection = new Vector3(moveDirection.normalized.x, 0, moveDirection.normalized.y);
            Debug.Log($"new direction {movementDirection}");
        }
    }
}
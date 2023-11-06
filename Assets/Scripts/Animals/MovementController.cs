using UnityEngine;

namespace Animals
{
    public class MovementController : MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Animator animator;
        [SerializeField] private float movementSpeed;
        
        private Vector3 movementDirection = new Vector3(0,0,0);

        public void Move(AnimalController animalController,Transform characterTransform)
        {
            // Rotate in the right direction
            if (movementDirection != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
                characterTransform.rotation = Quaternion.RotateTowards(characterTransform.rotation, toRotation, 700 * Time.deltaTime);
            }
            
            // Move 
            characterController.SimpleMove(movementDirection.normalized * (movementSpeed * animalController.EnvironmentData.TimeSpeed));
            // TODO only check after a change
            animator.speed = animalController.EnvironmentData.TimeSpeed;
        }

        public void SetMoveDirection(Vector2 moveDirection)
        {
            movementDirection = new Vector3(moveDirection.normalized.x, 0, moveDirection.normalized.y);
        }
    }
}
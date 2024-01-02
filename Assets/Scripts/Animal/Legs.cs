using UnityEngine;

namespace Animal
{
    public class Legs : Organ
    {
        public Animator Animator;

        private Vector3 movementDirection = Vector3.zero;
        private int currentMovementSpeed;

        public void Move(Transform characterTransform)
        {
            // Rotate in the right direction
            if (movementDirection != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
                characterTransform.rotation = Quaternion.RotateTowards(characterTransform.rotation, toRotation, 700 * Time.deltaTime);
                
                
            }
            
            // Move 
            animalController.CharacterController.SimpleMove(movementDirection.normalized * (currentMovementSpeed * animalController.EnvironmentData.TimeSpeed));
            // TODO only check after a change
            Animator.speed = animalController.EnvironmentData.TimeSpeed * currentMovementSpeed / 10f;
        }

        public void SetMoveDirection(Vector2 moveDirection, float speed)
        {
            if (moveDirection == Vector2.zero)
            {
                movementDirection = Vector3.zero;
            }
            else
            {
                movementDirection = new Vector3(moveDirection.normalized.x, 0, moveDirection.normalized.y);
            }
            if (speed < 0)
            {
                currentMovementSpeed = animalController.DNA.MovementSpeed[1];
            }
            else
            {
                currentMovementSpeed = animalController.DNA.MovementSpeed[0];
            }
            //currentMovementSpeed = (int) (DNA.MovementSpeed[0] * Mathf.Abs(speed));
        }
    }
}
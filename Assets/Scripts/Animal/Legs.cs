using UnityEngine;

namespace Animal
{
    public class Legs : Organ
    {
        public DNA DNA;
        public CharacterController CharacterController;
        public Animator Animator;

        private Vector3 movementDirection = Vector3.zero;

        public void Move(AnimalController animalController, Transform characterTransform)
        {
            // Rotate in the right direction
            if (movementDirection != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
                characterTransform.rotation = Quaternion.RotateTowards(characterTransform.rotation, toRotation, 700 * Time.deltaTime);
            }
            
            // Move 
            CharacterController.SimpleMove(movementDirection.normalized * (DNA.MovementSpeed[0] * animalController.EnvironmentData.TimeSpeed));
            // TODO only check after a change
            Animator.speed = animalController.EnvironmentData.TimeSpeed / 2f;
        }

        public void SetMoveDirection(Vector2 moveDirection)
        {
            movementDirection = new Vector3(moveDirection.normalized.x, 0, moveDirection.normalized.y);
        }
    }
}
using UnityEngine;

namespace Animal
{
    public class Legs : Organ
    {
        public Animator Animator;

        private Vector3 movementDirection = Vector3.zero;
        private int currentMovementSpeed;

        public override void Init(bool isChild = false)
        {
            EnvironmentData.TimeSpeedChangedEvent.AddListener(OnTimeScaleChanged);
        }

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
        }

        private void OnTimeScaleChanged(int speed)
        {
            switch (speed)
            {
                case <= 5:
                    Animator.speed = speed / 3f ;
                    break;
                case <= 20:
                    Animator.speed = speed / 5f ;
                    break;
                default:
                    Animator.speed = speed / 9f ;
                    break;
            }
        }

        public void SetMoveDirection(Vector2 moveDirection, int speed)
        {
            if (moveDirection == Vector2.zero || speed == 0)
            {
                movementDirection = Vector3.zero;
            }
            else
            {
                movementDirection = new Vector3(moveDirection.normalized.x, 0, moveDirection.normalized.y);
            }
            currentMovementSpeed = speed;
            Animator.SetInteger("MovementSpeed", currentMovementSpeed);
        }
    }
}
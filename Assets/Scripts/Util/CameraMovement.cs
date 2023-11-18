using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public class CameraMovement : MonoBehaviour
    {
        public int Speed = 3;

        void Update()
        {
            float xAxisValue = Input.GetAxis("Horizontal") * Speed;
            float zAxisValue = Input.GetAxis("Vertical") * Speed;
            float yValue = 0.0f;

            if (Input.GetKey(KeyCode.Q))
            {
                if (transform.position.y > 5)
                {
                    yValue = -Speed/2f;
                }
                else
                {
                    yValue = 0;
                }
            }

            if (Input.GetKey(KeyCode.E))
            {
                yValue = Speed/2f;
            }

            transform.position = new Vector3(transform.position.x + xAxisValue, transform.position.y + yValue,
                transform.position.z + zAxisValue);
        }

    }
}

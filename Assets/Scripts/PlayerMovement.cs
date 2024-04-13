using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public float depthSpeed;
    
    void Update()
    {
        Vector2 movement = Vector2.ClampMagnitude(
            new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")), 
            1.0f
        );
            
        float horizontalMovement = movement.x * speed * Time.deltaTime;
        float depthMovement = movement.y * depthSpeed * Time.deltaTime;

        transform.Translate(horizontalMovement, 0, depthMovement);
    }
}

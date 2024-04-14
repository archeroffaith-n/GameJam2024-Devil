using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveUpdater : MonoBehaviour
{
    public bool useStartPosition = true;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = gameObject.transform.position;
    }

    void Update()
    {
        Vector3 newPosition;
        if (useStartPosition) {
            float shift = gameObject.transform.position.z - startPosition.z;

            newPosition = new Vector3(
                gameObject.transform.position.x, 
                startPosition.y + shift,
                gameObject.transform.position.z
            );
        } else {
            newPosition = new Vector3(
                gameObject.transform.position.x, 
                gameObject.transform.position.z, 
                gameObject.transform.position.z
            );
        }
        
        gameObject.transform.position = newPosition;
    }
}

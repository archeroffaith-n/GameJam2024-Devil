using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderLine : MonoBehaviour
{
    void OnDrawGizmosSelected()
    {
        Mesh mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
        Gizmos.color = new Color(245 / 255.0f, 208 / 255.0f, 86 / 255.0f, 0.35f);
        Gizmos.DrawMesh(mesh, transform.position, transform.rotation, transform.localScale);
    }
}

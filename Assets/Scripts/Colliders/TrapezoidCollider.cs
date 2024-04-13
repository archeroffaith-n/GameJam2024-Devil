using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TrapezoidCollider : MonoBehaviour
{
    private Mesh colliderMesh = null;
    private GameObject frontLine;
    private GameObject backLine;
    private Vector3 frontLinePosition;
    private Vector3 backLinePosition;

    void CreateMesh()
    {
        if (colliderMesh == null) {
            frontLine = gameObject.transform.Find("FrontLine").gameObject;
            backLine = gameObject.transform.Find("BackLine").gameObject;

            if ((frontLine == null) || (backLine == null)) {
                throw new Exception("Lines of collider are not found");
            }
        } else if (
            (frontLinePosition == frontLine.transform.position - transform.position) 
            && (backLinePosition == backLine.transform.position - transform.position)) {
            return;
        }

        frontLinePosition = frontLine.transform.position - transform.position;
        backLinePosition = backLine.transform.position - transform.position;
        
        float frontLineShift = frontLine.transform.localScale.x * 0.5f;
        float backLineShift = backLine.transform.localScale.x * 0.5f;

        // 5 -- 6
        // |\   |\
        // | 1 -- 2
        // 4 |  7 |
        //  \|   \|
        //   0 -- 3

        Vector3[] vertices = new Vector3[8];
        int[] triangles = new int[36];

        vertices[0] = new Vector3(frontLinePosition.x - frontLineShift, frontLinePosition.y, frontLinePosition.z);
        vertices[1] = new Vector3(frontLinePosition.x - frontLineShift,  backLinePosition.y, frontLinePosition.z);
        vertices[2] = new Vector3(frontLinePosition.x + frontLineShift,  backLinePosition.y, frontLinePosition.z);
        vertices[3] = new Vector3(frontLinePosition.x + frontLineShift, frontLinePosition.y, frontLinePosition.z);
        vertices[4] = new Vector3( backLinePosition.x -  backLineShift, frontLinePosition.y,  backLinePosition.z);
        vertices[5] = new Vector3( backLinePosition.x -  backLineShift,  backLinePosition.y,  backLinePosition.z);
        vertices[6] = new Vector3( backLinePosition.x +  backLineShift,  backLinePosition.y,  backLinePosition.z);
        vertices[7] = new Vector3( backLinePosition.x +  backLineShift, frontLinePosition.y,  backLinePosition.z);

        triangles[0] =  0; triangles[1] =  1; triangles[2] =  2;
        triangles[3] =  2; triangles[4] =  3; triangles[5] =  0;
        triangles[6] =  4; triangles[7] =  5; triangles[8] =  1;
        triangles[9] =  1; triangles[10] = 0; triangles[11] = 4;
        triangles[12] = 1; triangles[13] = 5; triangles[14] = 6;
        triangles[15] = 6; triangles[16] = 2; triangles[17] = 1;
        triangles[18] = 7; triangles[19] = 6; triangles[20] = 5;
        triangles[21] = 5; triangles[22] = 4; triangles[23] = 7;
        triangles[24] = 3; triangles[25] = 7; triangles[26] = 4;
        triangles[27] = 4; triangles[28] = 0; triangles[29] = 3;
        triangles[30] = 3; triangles[31] = 2; triangles[32] = 6;
        triangles[33] = 6; triangles[34] = 7; triangles[35] = 3;

        colliderMesh = new Mesh {
            vertices = vertices,
            triangles = triangles
        };

        colliderMesh.RecalculateNormals();
    }

    void Start()
    {
        CreateMesh();
        gameObject.GetComponent<MeshCollider>().sharedMesh = colliderMesh;
    }

    void OnDrawGizmosSelected()
    {
        CreateMesh();
        Gizmos.color = new Color(86 / 255.0f, 245 / 255.0f, 83 / 255.0f, 0.25f);
        Gizmos.DrawWireMesh(colliderMesh, transform.position, transform.rotation, transform.localScale);
    }

    void OnDrawGizmos()
    {
        // Is required only because of bugs
    }
}

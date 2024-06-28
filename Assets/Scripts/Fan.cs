using System.Collections.Generic;
using UnityEngine;

public class Fan : MonoBehaviour
{
    public List<Vector3> GetVertices()
    {
        List<Vector3> vertices = new List<Vector3>();
        // Get all MeshFilters in this GameObject and its children
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();

        foreach (MeshFilter meshFilter in meshFilters)
        {
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                Mesh mesh = meshFilter.sharedMesh;

                // Transform the vertices to world space and add to the list
                foreach (Vector3 vertex in mesh.vertices)
                {
                    Vector3 worldVertex = meshFilter.transform.TransformPoint(vertex);
                    vertices.Add(worldVertex);
                }
            }
        }

        return vertices ;
    }

    // void OnDrawGizmos()
    // {
    //     List<Vector3> vertices = GetVertices();
    //     Gizmos.color = Color.red;
    //     foreach (Vector3 vertex in vertices)
    //     {
    //         Gizmos.DrawSphere(vertex, 0.1f); // Draw a small sphere at each vertex
    //     }
    // }
}
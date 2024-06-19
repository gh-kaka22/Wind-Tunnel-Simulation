using System.Collections.Generic;
using UnityEngine;

public class Fan : MonoBehaviour
{
    public List<Vector3> getVertices()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        List<Vector3> lst = new List<Vector3>();

        if (meshFilter != null)
        {
            Mesh mesh = meshFilter.sharedMesh;
        
            // Get the vertices of the mesh and transform to world space
            foreach (Vector3 vertex in mesh.vertices)
            {
                Vector3 trnsvertex = transform.TransformPoint(vertex);
                lst.Add(trnsvertex);
            }
            
            // // Now add points within the volume of the balloon
            // Bounds bounds = mesh.bounds;
            // Vector3 center = bounds.center;
            // Vector3 extents = bounds.extents;
            // int pointsInside = 1; // Adjust this to change the number of internal points
            //
            // // Scaling factors to map unit sphere points to the balloon ellipsoid
            // Vector3 scale = extents;
            //
            // for (int i = 0; i < pointsInside; i++)
            // {
            //     // Generate a random point inside a unit sphere
            //     Vector3 randomPointInSphere = Random.insideUnitSphere;
            //
            //     // Scale to the balloon's ellipsoid dimensions
            //     Vector3 randomPointInEllipsoid = new Vector3(
            //         randomPointInSphere.x * scale.x,
            //         randomPointInSphere.y * scale.y,
            //         randomPointInSphere.z * scale.z
            //     );
            //
            //     // Transform to world space
            //     Vector3 worldPoint = transform.TransformPoint(randomPointInEllipsoid + center);
            //     lst.Add(worldPoint);
            // }
        }

        return lst;
    }

    
    // public int pointsInside = 1000; // Number of points to generate inside the balloon
    // public List<Vector3> getVertices()
    // {
    //     MeshFilter meshFilter = GetComponent<MeshFilter>();
    //     List<Vector3> lst = new List<Vector3>();
    //
    //     if (meshFilter != null)
    //     {
    //         Mesh mesh = meshFilter.mesh;
    //         foreach (Vector3 vertex in mesh.vertices)
    //         {
    //             Vector3 trnsvertex = transform.TransformPoint(vertex);
    //             lst.Add(trnsvertex);
    //         }
    //
    //         // Get the bounds of the balloon
    //         Bounds bounds = mesh.bounds;
    //         Vector3 center = bounds.center;
    //         Vector3 extents = bounds.extents;
    //
    //         // Generate points inside the balloon's bounds
    //         for (int i = 0; i < pointsInside; i++)
    //         {
    //             Vector3 randomPoint = new Vector3(
    //                 Random.Range(center.x - extents.x, center.x + extents.x),
    //                 Random.Range(center.y - extents.y, center.y + extents.y),
    //                 Random.Range(center.z - extents.z, center.z + extents.z)
    //             );
    //
    //             // Transform to world space
    //             Vector3 worldPoint = transform.TransformPoint(randomPoint);
    //
    //             // Optional: Check if the point is within a certain distance from the center (balloon's volume)
    //             // Assuming an ellipsoidal balloon, use a distance check
    //             if (Vector3.Distance(worldPoint, transform.position) < extents.magnitude)
    //             {
    //                 lst.Add(worldPoint);
    //             }
    //         }
    //     }
    //
    //     return lst;
    // }

    // void OnDrawGizmos()
    // {
    //     List<Vector3> vertices = getVertices();
    //     Gizmos.color = Color.red;
    //     foreach (Vector3 vertex in vertices)
    //     {
    //         Gizmos.DrawSphere(vertex, 0.1f); // Draw a small sphere at each vertex
    //     }
    // }
 
    // void Start() 
    // { 
    //     // Get the MeshFilter component attached to this GameObject 
    //     // MeshFilter meshFilter = GetComponent<MeshFilter>(); 
    //
    //     // if (meshFilter != null) 
    //     // { 
    //     //     // Get the Mesh from the MeshFilter 
    //     //     Mesh mesh = meshFilter.mesh; 
    //
    //     //     // Print out some information about the mesh 
    //     //     Debug.Log("Mesh name: " + mesh.name); 
    //     //     Debug.Log("Vertex count: " + mesh.vertexCount); 
    //
    //     //     // Optionally, print out all vertices 
    //     //     foreach (Vector3 vertex in mesh.vertices) 
    //     //     { 
    //     //         Debug.Log("Vertex position: " + vertex); 
    //     //     } 
    //     // } 
    //     // else 
    //     // { 
    //     //     Debug.LogError("No MeshFilter component found on this GameObject."); 
    //     // } 
    // } 
}

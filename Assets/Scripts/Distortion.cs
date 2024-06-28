using System;
using UnityEngine;

public class WindTunnelDistortion : MonoBehaviour
{
    public float bounceSpeed = 1000000f;
    public float fallForce = 10f;
    public float stiffness = 0.1f;
    public float velocityScale = 0.5f; // Scale factor for maxDistortionStrength

    private MeshFilter meshFilter;
    private Mesh mesh;
    private Vector3[] initialVertices;
    private Vector3[] currentVertices;
    private Vector3[] vertexVelocities;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            return;
        }

        mesh = meshFilter.mesh;
        if (mesh == null)
        {
            return;
        }

        initialVertices = mesh.vertices;
        if (initialVertices.Length == 0)
        {
            return;
        }

        currentVertices = new Vector3[initialVertices.Length];
        vertexVelocities = new Vector3[initialVertices.Length];

        for (int i = 0; i < initialVertices.Length; i++)
        {
            currentVertices[i] = initialVertices[i];
        }

    }

    private void Update()
    {
        if (mesh == null || initialVertices.Length == 0)
        {
            return;
        }

        UpdateVertices();
        DrawVerticesInGameScene();
    }

    private void UpdateVertices()
    {
        for (int i = 0; i < currentVertices.Length; i++)
        {
            Vector3 currentDisplacement = currentVertices[i] - initialVertices[i];
            vertexVelocities[i] -= currentDisplacement * bounceSpeed * Time.deltaTime;
            vertexVelocities[i] *= 1f - stiffness * Time.deltaTime;
            currentVertices[i] += vertexVelocities[i] * Time.deltaTime;
        }



        mesh.vertices = currentVertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

    }


    public void ApplyVelocity(Vector3[] velocities)
    {
        if (mesh == null || initialVertices.Length == 0)
        {
            return;
        }

        for (int i = 0; i < currentVertices.Length; i++)
        {
            //Debug.Log($"Vertex {i} before: {currentVertices[i]}");
            vertexVelocities[i] += velocities[i] * Time.deltaTime;
            // Debug.Log($"Vertex {i} velocity: {velocities[i]}");
        }
    }

    private void OnDrawGizmos()
    {
        if (initialVertices != null && currentVertices != null && initialVertices.Length > 0 && currentVertices.Length > 0)
        {
            Vector3 offset = new Vector3(0, 0, 0);

            Gizmos.color = Color.green;
            for (int i = 0; i < initialVertices.Length; i++)
            {
                Gizmos.DrawSphere(transform.TransformPoint(initialVertices[i]) + offset, 0.05f);
            }

            Gizmos.color = Color.black;
            for (int i = 0; i < currentVertices.Length; i++)
            {
                Gizmos.DrawSphere(transform.TransformPoint(currentVertices[i]) + offset, 0.06f);
            }
        }
    }

    void DrawVerticesInGameScene()
    {
        if (initialVertices != null && currentVertices != null && initialVertices.Length > 0 && currentVertices.Length > 0)
        {
            Vector3 offset = new Vector3(0, 20, 0);

            for (int i = 0; i < initialVertices.Length; i++)
            {
                Debug.DrawLine(transform.TransformPoint(initialVertices[i]) + offset, transform.TransformPoint(initialVertices[i]) + offset + Vector3.up * 0.05f, Color.green);
            }

            for (int i = 0; i < currentVertices.Length; i++)
            {
                Debug.DrawLine(transform.TransformPoint(currentVertices[i]) + offset, transform.TransformPoint(currentVertices[i]) + offset + Vector3.up * 0.06f, Color.black);
            }
        }
    }




}
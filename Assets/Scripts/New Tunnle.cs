using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewTunnle : MonoBehaviour
{
    public int width = 5;
    public int height = 3;
    public int length = 7;
    public GameObject wallPrefab;

    void Start()
    {
        //BuildRoom();
    }

    void BuildRoom()
    {
        // Instantiate walls along the width
        for (int i = 0; i < width; i++)
        {
            InstantiateWall(new Vector3(i, 0, 0));
            InstantiateWall(new Vector3(i, 0, length - 1));
        }

        // Instantiate walls along the length
        for (int i = 0; i < length; i++)
        {
            InstantiateWall(new Vector3(0, 0, i));
            InstantiateWall(new Vector3(width - 1, 0, i));
        }

        // Instantiate ceiling
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                InstantiateWall(new Vector3(i, height - 1, j));
            }
        }
    }

    void InstantiateWall(Vector3 position)
    {
        GameObject wall = Instantiate(wallPrefab, position, Quaternion.identity);
        wall.transform.parent = transform; // Set the room as the parent of the wall
    }
}

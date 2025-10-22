using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorSpawnpoint : MonoBehaviour
{
    enum Direction
    {
        North,
        East,
        West
    }

    [Header("Direction")]
    [SerializeField] Direction direction = Direction.North;

    [SerializeField] List<GameObject> corridorPrefabs = new List<GameObject>();

    void Start()
    {
        GameManager manager = FindObjectOfType<GameManager>();

        if (manager.corridorCount >= manager.maxCorridors)
        {
            Destroy(gameObject);
            return;
        }
        manager.corridorCount++;

        GameObject corridorToSpawn = corridorPrefabs[Random.Range(0, corridorPrefabs.Count)];

        switch (direction)
        {
            case Direction.North:
                Instantiate(corridorToSpawn, transform.position, Quaternion.Euler(0f, 0f + transform.parent.eulerAngles.y, 0f), manager.corridorContainer);
                break;
            case Direction.East:
                Instantiate(corridorToSpawn, transform.position, Quaternion.Euler(0f, 90f + transform.parent.eulerAngles.y, 0f), manager.corridorContainer);
                break;
            case Direction.West:
                Instantiate(corridorToSpawn, transform.position, Quaternion.Euler(0f, -90f + transform.parent.eulerAngles.y, 0f), manager.corridorContainer);
                break;
        }
    }
}

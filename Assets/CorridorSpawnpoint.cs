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
    [SerializeField] GameObject deadEndPrefab;

    void Start()
    {
        //compute rotation
        Quaternion spawnRotation = new Quaternion();
        switch (direction)
        {
            case Direction.North:
                spawnRotation = Quaternion.Euler(0f, 0f + transform.parent.eulerAngles.y, 0f);
                break;
            case Direction.East:
                spawnRotation = Quaternion.Euler(0f, 90f + transform.parent.eulerAngles.y, 0f);
                break;
            case Direction.West:
                spawnRotation = Quaternion.Euler(0f, -90f + transform.parent.eulerAngles.y, 0f);
                break;
        }

        //check if we can spawn more corridors
        MapGenerator generator = FindObjectOfType<MapGenerator>();

        if (generator.corridorCount >= generator.maxCorridors)
        {
            //max corridors reached, become dead end
            Debug.Log("Max corridors reached, becoming dead end.");
            BecomeDeadEnd();
            return;
        }

        //spawn another corridor, increase active spawnpoint count
        generator.activeSpawnPointCount++;

        //choose what corridor to spawn
        GameObject corridorToSpawn = corridorPrefabs[Random.Range(0, corridorPrefabs.Count)];

        while(CanSpawn(corridorToSpawn, transform.position, spawnRotation, generator.corridorLayerMask) == false)
        {
            Debug.Log("Can not spawn: " + corridorToSpawn.name);

            corridorPrefabs.Remove(corridorToSpawn);
            if (corridorPrefabs.Count > 0)
            {
                //try another corridor variant
                corridorToSpawn = corridorPrefabs[Random.Range(0, corridorPrefabs.Count)];
            }
            else
            {
                //check if this is the last spawnpoint
                if(generator.activeSpawnPointCount <= 1)
                {
                    //restart generation
                    generator.StartMapGeneration();
                }
                else
                {
                    Debug.Log("Not the last active spawnpoint, becoming dead end.");
                    BecomeDeadEnd();
                    generator.activeSpawnPointCount--;
                    return;
                }
            }
                
        }

        //spawning corridor
        Debug.Log("Can spawn: " + corridorToSpawn.name);
        SpawnCorridor(corridorToSpawn);

        generator.corridorCount++;
        generator.activeSpawnPointCount--;
    }

    void BecomeDeadEnd()
    {
        transform.parent.GetComponentInParent<CorridorSpawnpoint>().SpawnCorridor(deadEndPrefab);
        transform.parent.GetChild(0).gameObject.SetActive(false);
    }

    void SpawnCorridor(GameObject corridorToSpawn)
    {
        switch (direction)
        {
            case Direction.North:
                Instantiate(corridorToSpawn, transform.position, Quaternion.Euler(0f, 0f + transform.parent.eulerAngles.y, 0f), transform);
                break;
            case Direction.East:
                Instantiate(corridorToSpawn, transform.position, Quaternion.Euler(0f, 90f + transform.parent.eulerAngles.y, 0f), transform);
                break;
            case Direction.West:
                Instantiate(corridorToSpawn, transform.position, Quaternion.Euler(0f, -90f + transform.parent.eulerAngles.y, 0f), transform);
                break;
        }
    }

    bool CanSpawn(GameObject obj, Vector3 pos, Quaternion rot, LayerMask hitLayerMask)
    {
        MeshCollider meshCollider = obj.transform.GetChild(0).GetComponent<MeshCollider>();

        Vector3 localOffset = meshCollider.transform.localPosition;
        Vector3 worldCenter = pos + rot * localOffset;

        Bounds meshBounds = meshCollider.sharedMesh.bounds;
        Vector3 scaleSize = Vector3.Scale(meshBounds.size, meshCollider.transform.lossyScale);

        Collider[] hits = Physics.OverlapBox(worldCenter + rot * meshBounds.center, scaleSize / 2f, rot, hitLayerMask);

        foreach (Collider hit in hits)
        {
            if (hit.transform.IsChildOf(transform.parent))
                continue;

            Debug.Log("Hit: " + hit.name);
            return false;
        }

        return true;
    }
}

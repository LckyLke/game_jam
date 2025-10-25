using NUnit.Framework;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;
using static UnityEngine.Rendering.DebugUI.Table;

public class CorridorSpawnpoint : MonoBehaviour
{
    //public GameObject debugCube;
    //public GameObject debugCubeSpawnpoint;
    public bool hasSpawned = false;

    [SerializeField] GameObject wallPrefab;
    [SerializeField] GameObject deadEndPrefab;
    [SerializeField] GameObject debugCube;


    Quaternion spawnRotation = new Quaternion();
    Vector3 checkPosition;
    Quaternion checkRotation;
    MapGenerator generator;
    Vector3 gizmoCenter, gizmoSize;

    enum Direction
    {
        North,
        East,
        West
    }

    [Header("Direction")]
    [SerializeField] Direction direction = Direction.North;

    [SerializeField] List<GameObject> corridorPrefabs = new List<GameObject>();
    [SerializeField] List<GameObject> crossings = new List<GameObject>();

    private void Start()
    {
        Debug.Log(transform.parent.name + " is crossing: " + isCrossing().ToString());
        generator = FindObjectOfType<MapGenerator>();
        generator.spawnQueue.Enqueue(this);
    }

    public void Place()
    {
        //compute rotation
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
        if (generator.corridorCount >= generator.maxCorridors)
        {
            //max corridors reached, become dead end
            Debug.Log("Max corridors reached, becoming dead end.");
            BecomeDeadEnd();
            return;
        }

        //spawn another corridor, increase active spawnpoint count
        generator.activeSpawnPointCount++;
        Debug.Log("active Spawnpoint count: " + generator.activeSpawnPointCount);

        if(generator.activeSpawnPointCount > generator.maxCorridors / 2)
        {
            Debug.Log("Too many active spawnpoints, becoming dead end.");
            BecomeDeadEnd();
            return;
        }

        //choose what corridor to spawn
        GameObject corridorToSpawn = corridorPrefabs[Random.Range(0, corridorPrefabs.Count)];

        while(CanSpawn(corridorToSpawn, transform.position, spawnRotation, generator.corridorLayerMask) == false)
        {
            Debug.Log("Can not spawn: " + corridorToSpawn.name + " --> remove it from list");
            corridorPrefabs.Remove(corridorToSpawn);
            if (corridorPrefabs.Count > 0)
            {
                //try another corridor variant
                corridorToSpawn = corridorPrefabs[Random.Range(0, corridorPrefabs.Count)];
            }
            else
            {
                //cant spawn any corridor here
                Debug.Log("Can not spawn any corridor here. Change previous corridor type");
                ChangePreviousCorridor();
                return;
            }
                
        }

        //spawning corridor
        Debug.Log("Can spawn: " + corridorToSpawn.name);
        SpawnCorridor(corridorToSpawn);
        if (generator.activeSpawnPointCount > 0)
            generator.activeSpawnPointCount--;
    }

    void BecomeDeadEnd()
    {
        if(CanSpawn(deadEndPrefab, transform.position, spawnRotation, generator.corridorLayerMask))
        {
            Instantiate(deadEndPrefab, transform.position, spawnRotation, generator.deadEndContainer);
        }
        else
        {
            Instantiate(wallPrefab, transform.position, spawnRotation, generator.deadEndContainer);        
        }
        hasSpawned = true;
        if(generator.activeSpawnPointCount > 0)
            generator.activeSpawnPointCount--;
        Physics.SyncTransforms();
    }

    int GetMaxChildDepth(Transform root)
    {
        if (root.childCount == 0)
            return 0;

        int maxDepth = 0;
        foreach (Transform child in root)
        {
            int childDepth = 1 + GetMaxChildDepth(child);
            if (childDepth > maxDepth)
                maxDepth = childDepth;
        }
        return maxDepth;
    }

    public void ChangePreviousCorridor()
    {
        hasSpawned = false;
        generator.backtrack++;
        Debug.Log("Backtrack count: " + generator.backtrack);
        if (generator.backtrack > 50)
        {
            Debug.Log("Backtrack limit reached, restarting generation.");
            //generator.StartMapGeneration();
            return;
        }

        generator.corridorCount -= GetMaxChildDepth(transform.parent) / 3;
        transform.parent.GetComponentInParent<CorridorSpawnpoint>().hasSpawned = false;
        transform.parent.GetComponentInParent<CorridorSpawnpoint>().corridorPrefabs.Remove(transform.parent.GetChild(0).gameObject);
        if (transform.parent.GetComponentInParent<CorridorSpawnpoint>().corridorPrefabs.Count < 1)
        {
            transform.parent.GetComponentInParent<CorridorSpawnpoint>().ChangePreviousCorridor();
        }
        else
        {
            generator.spawnQueue.Enqueue(transform.parent.GetComponentInParent<CorridorSpawnpoint>());
            //transform.parent.GetComponentInParent<CorridorSpawnpoint>().Place();
        }

        StartCoroutine(DestroyParentNextFrame(transform.parent.gameObject));
    }

    void SpawnCorridor(GameObject corridorToSpawn)
    {
        //StartCoroutine(SpawnDelayed(corridorToSpawn));
        if(hasSpawned)
        {
            Debug.Log("List Lenght: " + corridorPrefabs.Count.ToString());
            Debug.Log("This spawnpoint (" + transform.name + ") has already spawned.");
            Instantiate(debugCube, transform.position, Quaternion.identity, transform);
            return;
        }

        Instantiate(corridorToSpawn, transform.position, spawnRotation, transform);
        hasSpawned = true;
        generator.corridorCount++;

        if (generator.backtrack > 0)
            generator.backtrack -= 1;

        Physics.SyncTransforms();
    }

    bool CanSpawn(GameObject corridorToSpawn, Vector3 pos, Quaternion rot, LayerMask corridorLayer)
    {
        if(isCrossing() && crossings.Contains(corridorToSpawn))
            return false;

        BoxCollider box = corridorToSpawn.GetComponent<BoxCollider>();
        gizmoSize = box.size;

        checkPosition = transform.position + spawnRotation * box.center;
        //Instantiate(debugCube, checkPosition, Quaternion.identity);

        Collider[] hits = Physics.OverlapBox(checkPosition, gizmoSize / 2f, spawnRotation, corridorLayer, QueryTriggerInteraction.Collide);

        foreach (Collider hit in hits)
        {
            if(hit.transform.IsChildOf(transform.parent) || hit.transform == transform.parent.parent || hit.transform == transform.parent)
            {
                //Debug.Log("Hit own corridor part, ignoring." + " --> " + hit.transform.name);
                continue;
            }

            Debug.Log("Hit: " + hit.name + " --> false");
            return false;
        }

        return true;
    }

    IEnumerator DestroyParentNextFrame(GameObject parent)
    {
        yield return new WaitForEndOfFrame();
        Destroy(parent);
    }

    bool isCrossing()
    {
        if (crossings.Contains(transform.parent.gameObject))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

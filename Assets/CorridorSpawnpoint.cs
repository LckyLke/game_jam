using NUnit.Framework;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;
using static UnityEngine.Rendering.DebugUI.Table;

public class CorridorSpawnpoint : MonoBehaviour
{
    public GameObject debugCube;
    public GameObject debugCubeSpawnpoint;




    Quaternion spawnRotation = new Quaternion();
    Vector3 checkPosition;
    Quaternion checkRotation;

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
    [SerializeField] GameObject deadEndPrefab;

    void Start()
    {
        Instantiate(debugCubeSpawnpoint, transform.position, Quaternion.identity);


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
        Destroy(transform.parent.gameObject);
    }

    void SpawnCorridor(GameObject corridorToSpawn)
    {
        switch (direction)
        {
            case Direction.North:
                Instantiate(corridorToSpawn, transform.position, spawnRotation, transform);
                break;
            case Direction.East:
                Instantiate(corridorToSpawn, transform.position, spawnRotation, transform);
                break;
            case Direction.West:
                Instantiate(corridorToSpawn, transform.position, spawnRotation, transform);
                break;
        }
    }

    bool CanSpawn(GameObject corridorToSpawn, Vector3 pos, Quaternion rot, LayerMask corridorLayer)
    {
        BoxCollider box = corridorToSpawn.GetComponent<BoxCollider>();
        gizmoSize = box.size;

        checkPosition = transform.position + spawnRotation * box.center;
        Instantiate(debugCube, checkPosition, Quaternion.identity);

        Collider[] hits = Physics.OverlapBox(checkPosition, gizmoSize / 2f, spawnRotation, corridorLayer);

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        //Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.matrix = Matrix4x4.TRS(checkPosition, spawnRotation, Vector3.one);
        Gizmos.DrawWireCube(checkPosition, gizmoSize);
    }
}

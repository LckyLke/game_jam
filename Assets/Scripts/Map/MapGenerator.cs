using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    bool corridorsFinished = false;
    public Queue<CorridorSpawnpoint> spawnQueue = new Queue<CorridorSpawnpoint>();

    public int corridorCount = 0;
    public int maxCorridors = 20;
    public int activeSpawnPointCount = 0;
    public LayerMask corridorLayerMask;
    public int backtrack = 0;

    [SerializeField] Transform corridorContainer;
    public Transform deadEndContainer;
    [SerializeField] GameObject spawnpointPrefab;
    [SerializeField] float spawnDelay = 0.5f;

    void Start()
    {
        StartMapGeneration();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            StartMapGeneration();
        }

        if(spawnQueue.Count > 0)
        {
            StartCoroutine(ProcessNextSpawnpoint());
        }

        if(corridorCount >= maxCorridors && !corridorsFinished && spawnQueue.Count < 1)
        {
            Debug.Log("Corridor generation finished.");
            corridorsFinished = true;

            if(deadEndContainer.childCount < 5)
            {
                Debug.Log("Not enough dead ends generated, restarting map generation.");
                //StartMapGeneration();
            }
            else
            {
                Debug.Log("Corridors generation successful. --> Start generation rooms...");
            }
        }
    }

    public void StartMapGeneration()
    {
        Debug.Log("--------------------------------------Starting new map generation.--------------------------------------");

        // Clear existing corridors
        foreach (Transform corridor in corridorContainer.transform)
        {
            Destroy(corridor.gameObject);
        }
        foreach (Transform deadEnd in deadEndContainer.transform)
        {
            Destroy(deadEnd.gameObject);
        }

        corridorsFinished = false;
        spawnQueue.Clear();
        corridorCount = 0;
        activeSpawnPointCount = 0;
        backtrack = 0;

        //start new generation
        Instantiate(spawnpointPrefab, Vector3.zero, Quaternion.identity, corridorContainer);
    }

    IEnumerator ProcessNextSpawnpoint()
    {
        // warte bis Ende des Frames, damit Collider aktiv sind
        yield return new WaitForEndOfFrame();

        if (spawnQueue.Count > 0)
            spawnQueue.Dequeue().Place();

        yield return new WaitForSeconds(spawnDelay);
    }
}

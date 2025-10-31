using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Metadata;
using spawnUsed;
using System;
public enum Tags
{
    ItemSpawnpoint
}

public class MapGenerator : MonoBehaviour
{
    public bool corridorsFinished = false;
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
    [SerializeField] GameObject debugCube;

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
            

            if(deadEndContainer.childCount < 5)
            {
                Debug.Log("Not enough dead ends generated, restarting map generation.");
                //StartMapGeneration();
            }
            else
            {
                Debug.Log("Corridors generation successful. --> Start generation rooms...");
            }

            Debug.Log("Corridor generation finished.");
            corridorsFinished = true;
            AddScripttoSpawns();

            
            LinkedList<Transform> children = GetCorridorList(corridorContainer);
            Debug.Log("Children Count" + children.Count);
            //test item spawn with debugCube
            foreach(Transform f in children)
            {
                spawnObject("ItemSpawnpoint", debugCube, f);
            }



        }
    }

    public void AddScripttoSpawns()
    {
        
        LinkedList<Transform> children = GetCorridorList(corridorContainer);
        foreach(Transform f in children)
        {
            foreach (Tags t in Enum.GetValues(typeof(Tags)))
            {
              for (int i = 0; i < f.childCount; i++)
                {
                    var s = f.GetChild(i);
                    if (s.CompareTag(t.ToString()))
                    {
                        s.AddComponent<SpawnpointUsed>();     
                    }
                }

            }
        }

    }



    public GameObject spawnObject(string tag, GameObject objectToSpawn, Transform corridor)
    {
        LinkedList<Transform> itemspawns = new LinkedList<Transform>();
        for (int i = 0; i < corridor.childCount; i++)
        {
            var s = corridor.GetChild(i);
            if (s.CompareTag(tag))
            {
                itemspawns.AddLast(s);
            }
        }

        var ispawn = itemspawns.ElementAt(UnityEngine.Random.Range(0, itemspawns.Count));
        return Instantiate(objectToSpawn, ispawn.position, ispawn.rotation, ispawn.parent);
    }

    public GameObject spawnObject(string tag, GameObject objectToSpawn, Transform corridor, Quaternion rotation)
    {
        LinkedList<Transform> itemspawns = new LinkedList<Transform>();
        for (int i = 0; i < corridor.childCount; i++)
        {
            var s = corridor.GetChild(i);
            if (s.CompareTag(tag))
            {
                itemspawns.AddLast(s);
            }
        }

        var ispawn = itemspawns.ElementAt(UnityEngine.Random.Range(0, itemspawns.Count));
        return Instantiate(objectToSpawn, ispawn.position, rotation, ispawn.parent);
    }


    public LinkedList<Transform> GetCorridorList(Transform firstSpawn)
    {
        LinkedList<Transform> corridorObjets = new LinkedList<Transform>();
        foreach (Transform child in firstSpawn)
        {
            if(child.gameObject.layer == 3 && child.gameObject.tag == "Corridor" ) { 
            Debug.Log("Child: " + child);
                corridorObjets.AddLast(child);
            
            }
            foreach(var c in GetCorridorList(child))
            corridorObjets.AddLast(c);
        }
        return corridorObjets;
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
        {

           
            CorridorSpawnpoint spawnpointObject = spawnQueue.Dequeue();
            if(spawnpointObject != null) { 
            spawnpointObject.tag = "SpawnPoint";
            spawnpointObject.Place();
            }
        }

        yield return new WaitForSeconds(spawnDelay);
    }
}

using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int corridorCount = 0;
    public int maxCorridors = 20;
    public int activeSpawnPointCount = 0;
    public LayerMask corridorLayerMask;

    [SerializeField] Transform corridorContainer;
    [SerializeField] GameObject spawnpointPrefab;

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
    }

    public void StartMapGeneration()
    {
        Debug.Log("--------------------------------------Starting new map generation.--------------------------------------");

        // Clear existing corridors
        foreach (Transform corridor in corridorContainer.transform)
        {
            Destroy(corridor.gameObject);
        }

        corridorCount = 0;
        activeSpawnPointCount = 0;

        //start new generation
        Instantiate(spawnpointPrefab, Vector3.zero, Quaternion.identity, corridorContainer);
    }
}

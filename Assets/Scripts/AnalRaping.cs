using System.Reflection;
using UnityEngine;

public class AnalRaping : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private GameObject analVictim;
    [SerializeField] private float radius = 0f;
    Rigidbody m_Rigidbody;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        //spawn it at random position inside the dungeon

    }

    // Update is called once per frame
    void Update()
    {
        UnityEngine.Vector3 playerPos = analVictim.transform.position;
        //Camera.main.transformation.position;


        //Debug.Log(cameraPos + "Camera");
        UnityEngine.Vector3 raperPos = GameObject.Find("AnalRaper").transform.position;

        // check if raper is in radius of player
        float distance = UnityEngine.Vector3.Distance(playerPos, raperPos);
        UnityEngine.Vector3 newVector = playerPos - raperPos;
        newVector.y = 0;
        //Debug.Log("newV" + newVector);
        if (distance < radius)
        {

            return;

        }
    }
}

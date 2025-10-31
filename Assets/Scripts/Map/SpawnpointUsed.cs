using System;
using UnityEngine;
namespace spawnUsed { 
public class SpawnpointUsed :MonoBehaviour
{
    private Boolean used = false;

    public void Start()
    {
        
    }

    public void Update()
    {
        
    }

    public Boolean IsUsed()
    {
        if (used)
        {
            return true;
        }
        else { return false; }
    }

    public void SetUsed()
    {
        used = true;
    }

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame

}

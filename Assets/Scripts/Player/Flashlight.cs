using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [SerializeField] GameObject flashlight;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        { 
            flashlight.SetActive(!flashlight.activeSelf);
        }
    }
}

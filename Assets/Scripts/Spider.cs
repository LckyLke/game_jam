using System.Numerics;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private Camera playerCam;
    [SerializeField] private float radius;
    Rigidbody m_Rigidbody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UnityEngine.Vector3 cameraPos = playerCam.transform.position;
        //Camera.main.transformation.position;


        //Debug.Log(cameraPos + "Camera");
        UnityEngine.Vector3 spiderPos = transform.position;

        float distance = UnityEngine.Vector3.Distance(cameraPos, spiderPos);
        UnityEngine.Vector3 newVector = cameraPos - spiderPos;
        newVector.y = 0;
        //Debug.Log("newV" + newVector);
        //transform.Translate(newVector * Time.deltaTime);
        UnityEngine.Vector3 playerXY = new UnityEngine.Vector3(cameraPos.x, 1, cameraPos.z);
        




        Debug.Log(distance > radius);
        if (distance > radius){
        transform.Translate(newVector * Time.deltaTime);
            var qTo = UnityEngine.Quaternion.LookRotation(playerXY - transform.position);
            qTo = UnityEngine.Quaternion.Slerp(transform.rotation, qTo, 10 * Time.deltaTime);
            GetComponent<Rigidbody>().MoveRotation(qTo);
        }
       
        //transform.Translate(UnityEngine.Vector3.down * verticalVelocity);
    }
    

    /**private float VerticalForceCalculation()
        if (controller.isGrounded)
    {
        {
            float v = -1f;
        }

        verticalVelocity -= gravity * Time.deltaTime;
        return verticalVelocity;
    }*/




}

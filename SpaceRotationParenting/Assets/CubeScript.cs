using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeScript : MonoBehaviour
{
    public Transform sphereTransform;
    // Start is called before the first frame update
    void Start()
    {
        sphereTransform.parent = transform;
        sphereTransform.localScale = Vector3.one * 2;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.eulerAngles += new Vector3(0, 1, 0)*180*Time.deltaTime;
        transform.Rotate(Vector3.up * 180 * Time.deltaTime, Space.World); //Global Space
        transform.Translate(Vector3.forward * 7 * Time.deltaTime, Space.World); //Global Space
        
        //transform.Translate(Vector3.forward * 7 * Time.deltaTime); //Space.Self Object Space
        //transform.Rotate(Vector3.up * 180 * Time.deltaTime); // Space.Self Object Space

        if(Input.GetKeyDown(KeyCode.Space))
        {
            sphereTransform.position = Vector3.zero;
            //sphereTransform.localPosition *= Vector3.zero;
        }
    }
}

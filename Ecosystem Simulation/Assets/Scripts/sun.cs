using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sun : MonoBehaviour
{
    public float speed;
    public Transform SunOrbit;
    public GameController GameController;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.RotateAround(SunOrbit.transform.position, Vector3.right, speed * Time.deltaTime);
        //transform.localRotation = Quaternion.Euler(new Vector3((GameController.TimeOfDay / 24 * 360f)-90, 170,0));
        //transform.RotateAround(Vector3.zero, Vector3.right,(GameController.TimeOfDay / 24 * 360f)-90);
    }
}

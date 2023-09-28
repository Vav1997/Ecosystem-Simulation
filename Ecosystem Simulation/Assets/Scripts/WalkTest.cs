using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkTest : MonoBehaviour
{
    public float MoveSpeed;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0, 0, -MoveSpeed * Time.deltaTime);
    }
}

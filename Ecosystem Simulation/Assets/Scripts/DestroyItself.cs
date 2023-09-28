using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyItself : MonoBehaviour
{
    public float delay;
    void Start()
    {
        Destroy(this, delay);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

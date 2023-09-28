using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocialLinks : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenLink(string Link)
    {
        Application.OpenURL(Link);
    }
}

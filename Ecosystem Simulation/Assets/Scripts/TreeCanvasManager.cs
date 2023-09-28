using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TreeCanvasManager : MonoBehaviour
{
    
    public TextMeshProUGUI HeightText;

    public PanelFader PanelFader;
    void Start()
    {

        PanelFader = GetComponent<PanelFader>();
    }

    // Update is called once per frame
    void Update()
    {
        if(PanelFader.CanvasIsVisible)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - PanelFader.Camera.transform.position);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelFader : MonoBehaviour
{
    public CanvasManager CanvasManager;
    public bool isActiveFade;
    private bool mFaded = true;
    public float Duration = 0.4f;
    public CanvasGroup CanvasGroup;

    public float fadeInDist;
    
    public bool FadeDone;
    public float dist;

    public bool CanvasIsVisible;
    public GameObject Camera;
    
    

    void Start()
    {
        CanvasManager = GetComponent<CanvasManager>();
        CanvasGroup = GetComponent<CanvasGroup>();
        Camera = GameObject.FindGameObjectWithTag("MainCamera");
        

        if(isActiveFade)
        {
            StartCoroutine(CheckFade());
        }
        else
        {
            CanvasGroup.alpha = 1;
        }
    }

    // Update is called once per frame
 

    public void Fade(bool FadeIn)
    {
        //Debug.Log("arec fade");
        
        if(FadeIn)
        {
            StartCoroutine(DoFade(CanvasGroup, 0, 1));
        }
        else
        {
            StartCoroutine(DoFade(CanvasGroup, 1, 0));
        }
        
    }

    public IEnumerator DoFade(CanvasGroup Group, float start, float end)
    {
        float counter = 0;
       
        while(counter < Duration)
        {
            counter += Time.deltaTime;
            CanvasGroup.alpha = Mathf.Lerp(start, end, counter / Duration);
            yield return null;
        }
    }

    public IEnumerator CheckFade()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.2f);

            dist = Vector3.Distance(transform.position, Camera.transform.position);
            if (dist <= fadeInDist && !FadeDone)
            {
                CanvasIsVisible = true;
                FadeDone = true;
                Fade(true);
                
            }
            else if(dist > fadeInDist && FadeDone)
            {
                CanvasIsVisible = false;
                FadeDone = false;
                Fade(false);
                
            }

        }
        
    }
}

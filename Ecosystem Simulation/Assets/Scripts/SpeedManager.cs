using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedManager : MonoBehaviour
{
    public Image NormalSpeedImage;
    public Image DoubleSpeedImage;
    public Image FourSpeedImage;
    public Sprite NormalSpeedActive;
    public Sprite NormalSpeedInactive;
    public Sprite DoubleSpeedActive;
    public Sprite DoubleSpeedInactive;
    public Sprite FourSpeedActive;
    public Sprite FourSpeedInactive;
    void Start()
    {
        NormalSpeedImage.sprite = NormalSpeedActive;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSpeed(float speed)
    {
        Time.timeScale = speed;

        if(speed == 1)
        {
           NormalSpeedImage.sprite = NormalSpeedActive;
           DoubleSpeedImage.sprite = DoubleSpeedInactive;
           FourSpeedImage.sprite = FourSpeedInactive;
        }
        else if(speed == 2)
        {
            NormalSpeedImage.sprite = NormalSpeedInactive;
            DoubleSpeedImage.sprite = DoubleSpeedActive;
            FourSpeedImage.sprite = FourSpeedInactive;
        }
        else if(speed == 4)
        {
            NormalSpeedImage.sprite = NormalSpeedInactive;
            DoubleSpeedImage.sprite = DoubleSpeedInactive;
            FourSpeedImage.sprite = FourSpeedActive;
        }
       
    }
}

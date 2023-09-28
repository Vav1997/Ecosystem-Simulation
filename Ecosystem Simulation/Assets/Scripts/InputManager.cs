using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public SpeedManager SpeedManager;
    public PauseMenu PauseMenu;
    public Animator CameraAnim;
    

    void Start()
    {
        SpeedManager = GetComponent<SpeedManager>();
        PauseMenu = GetComponent<PauseMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpeedManager.SetSpeed(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SpeedManager.SetSpeed(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SpeedManager.SetSpeed(4);
        }

        if(Input.GetKeyDown(KeyCode.F1))
        {
            PauseMenu.PauseGame();
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(CameraAnim.enabled)
            {
                CameraAnim.enabled = false;
            }
            else
            {
                CameraAnim.enabled = true;
            }
        }
    }
}

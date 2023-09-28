using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public bool isPaused;
    public GameObject PausePanel;
    public GameObject SettingsPanel;
    public FreeFlyCamera FreeFlyCamera;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PauseGame()
    {
            if(isPaused)
            {
                Time.timeScale = 1;
                isPaused = false;
                PausePanel.SetActive(false);
                SettingsPanel.SetActive(false);
                FreeFlyCamera.enabled = true;
            }
            else
            {
                Time.timeScale = 0;
                isPaused = true;
                PausePanel.SetActive(true);
                FreeFlyCamera.enabled = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
    }



}

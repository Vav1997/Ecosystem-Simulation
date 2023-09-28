using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChangeManager : MonoBehaviour
{
    public GameObject Camera;
    public float ChangeSpeed;
    public MainCameraController MainCameraController;
    public FreeFlyCamera FreeFlyCamera;
    public Transform FixCameraTransform;

    public Vector3 startMainCameraPosition;
    public Quaternion startMainCameraRotation;

    
    
    public bool moveCamera;
    void Start()
    {
        startMainCameraPosition = new Vector3(Camera.transform.position.x, Camera.transform.position.y, Camera.transform.position.z);
        startMainCameraRotation = Quaternion.Euler(Camera.transform.rotation.x,0,0);
        MainCameraController = GetComponent<MainCameraController>();
        FreeFlyCamera = GetComponentInChildren<FreeFlyCamera>();
    }

    
    void Update()
    {
        

        if(Input.GetKeyUp(KeyCode.Escape))
        {
            ChangeToFixedCamera();
            MoveCameraPos();
        }

        if(!MainCameraController.isActive)
        {
            transform.position = new Vector3(Camera.transform.position.x, 0,Camera.transform.position.z);
        }
        // if(moveCamera)
        // {
        //     MoveCameraPos();
        // }
    }

    public void ChangeToFixedCamera()
    {
        Camera.transform.parent = transform;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //FixCameraTransform = null;
        MainCameraController.isActive = true;
        FreeFlyCamera.enabled = false;
        moveCamera = true;

    }

    public void ChangeToFreeCamera()
    {
        Camera.transform.parent = null;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        FixCameraTransform = transform;
        MainCameraController.ResetAllData();
        MainCameraController.isActive = false;
        FreeFlyCamera.enabled = true;
    }

    public void MoveCameraPos()
    {
        Camera.transform.position = startMainCameraPosition;
        Camera.transform.localRotation = Quaternion.Euler(45,0,0);

        //Camera.transform.position = Vector3.Lerp(Camera.transform.position, startMainCameraPosition, ChangeSpeed * Time.deltaTime);
        //Camera.transform.localRotation = Quaternion.Lerp(Camera.transform.localRotation, startMainCameraRotation, ChangeSpeed * Time.deltaTime);
        //Camera.transform.localRotation = Quaternion.Lerp(Camera.transform.localRotation, Quaternion.Euler(45,0,0), ChangeSpeed * Time.deltaTime);
    }
}

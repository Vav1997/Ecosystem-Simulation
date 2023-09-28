using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class InputFields
{
    public string ObjectName;
    public TMP_InputField InputField;
}

public class StartInputManager : MonoBehaviour
{
    public InputFields[] InputFields;
    
    // public TMP_InputField CoyoteFemaleInput;
    // public TMP_InputField DeerMaleInput;
    // public TMP_InputField DeerFemaleInput;
    // public TMP_InputField TreeInput;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void StartSimulation()
    {
        // for (int i = 0; i < InputFields.Length; i++)
        // {
        //     if(InputFields[i].InputField == null && int.Parse(InputFields[i].InputField.text) > 0)
        //     {
        //         return;
        //     }
        // }

        PlayerPrefs.SetInt("CoyoteMale", int.Parse(InputFields[0].InputField.text));
        PlayerPrefs.SetInt("CoyoteFemale", int.Parse(InputFields[1].InputField.text));
        PlayerPrefs.SetInt("DeerMale", int.Parse(InputFields[2].InputField.text));
        PlayerPrefs.SetInt("DeerFemale", int.Parse(InputFields[3].InputField.text));
        PlayerPrefs.SetInt("Tree", int.Parse(InputFields[4].InputField.text));

        
        SceneManager.LoadScene("Simulation");


        
    }
}

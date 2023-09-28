using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class CSVExport : MonoBehaviour
{
    string fileName;
    public TextMeshProUGUI SavedText;
    
    
    void Start()
    {
        Directory.CreateDirectory(Application.streamingAssetsPath + "/CSV Files/");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WriteCSV(List<CoyotesDailyInfo> CoyotesInfo, List<TreesDailyInfo> TreesInfo, List<DeersDailyInfo> DeersInfo)
    {   
        int FileNumber = PlayerPrefs.GetInt("FileNumber");
       
        //fileName = Application.dataPath + "/CSV Files/Evolution Info " + FileNumber +".csv";
        fileName = Application.streamingAssetsPath + "/CSV Files/Evolution Info " + FileNumber +".csv";

        //Coyotes
        TextWriter tw = new StreamWriter(fileName, false);
        tw.WriteLine("Day, Coyotes count, Coyotes Born, Coyotes Died, AverageRunSpeed, AverageRadius, AverageStamina");
        tw.Close();

        tw = new StreamWriter(fileName, true);
        
        for (int i = 0; i < CoyotesInfo.Count; i++)
        {
            tw.WriteLine(CoyotesInfo[i].Day + "," + CoyotesInfo[i].OverallCount + "," + CoyotesInfo[i].CoyotesBorn + "," + CoyotesInfo[i].CoyotesDied
            + "," + CoyotesInfo[i].AverageRunSpeed+ "," + CoyotesInfo[i].AverageRadius + "," + CoyotesInfo[i].AverageStamina);
        }

        //Trees
        tw.WriteLine("Day, Trees count, Trees Born, Trees Died, AverageHeight");
        tw.Close();

        tw = new StreamWriter(fileName, true);
        
        for (int i = 0; i < TreesInfo.Count; i++)
        {
            tw.WriteLine(TreesInfo[i].Day + "," + TreesInfo[i].OverallCount + "," + TreesInfo[i].TreesBorn + "," + TreesInfo[i].TreesDied
            + "," + TreesInfo[i].AverageHeight);
        }


        //Deers
        tw.WriteLine("Day, Deers count, Deers Born, Deers Died, AverageRunSpeed, AverageRadius, AverageStamina, AverageNeckHeight, AverageStartFleeDistance");
        tw.Close();

        tw = new StreamWriter(fileName, true);
        
        for (int i = 0; i < DeersInfo.Count; i++)
        {
            tw.WriteLine(DeersInfo[i].Day + "," + DeersInfo[i].OverallCount + "," + DeersInfo[i].DeersBorn + "," + DeersInfo[i].DeersDied
            + "," + DeersInfo[i].AverageRunSpeed + "," + DeersInfo[i].AverageRadius + "," + DeersInfo[i].AverageStamina + "," + DeersInfo[i].AverageNeckHeight + "," + DeersInfo[i].AverageStartFleeDistance);
        }


        tw.Close();

        StartCoroutine(ShowSavedText());

        PlayerPrefs.SetInt("FileNumber", FileNumber + 1);
    }

    public IEnumerator ShowSavedText()
    {
        SavedText.text = "Excel file created in " + fileName;
        yield return new WaitForSeconds(7f);
        SavedText.text = "";
    }
    
    public void WriteCSVTrees(List<TreesDailyInfo> TreesInfo)
    {
        TextWriter tw = new StreamWriter(fileName, false);
        tw.WriteLine("Day, Trees count, Trees Born, Trees Died, AverageHeight");
        tw.Close();

        tw = new StreamWriter(fileName, true);
        
        for (int i = 0; i < TreesInfo.Count; i++)
        {
            tw.WriteLine(TreesInfo[i].Day + "," + TreesInfo[i].OverallCount + "," + TreesInfo[i].TreesBorn + "," + TreesInfo[i].TreesDied
            + "," + TreesInfo[i].AverageHeight);
        }

        tw.Close();

        Debug.Log("arec csv");


    }
}

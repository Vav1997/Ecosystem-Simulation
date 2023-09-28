using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CoyotesDailyInfo
{
    public int Day;
    public int OverallCount;
    public int CoyotesBorn;
    public int CoyotesDied;
    public float AverageRunSpeed;
    public float AverageRadius;
    public float AverageStamina;
}
[System.Serializable]
public class CoyotesIndividualDailyInfo
{
    public GameObject Coyote;
    public float Runspeed;
    public float SearchRadius;
    public float StaminaAmount;
}

[System.Serializable]
public class TreesDailyInfo
{
    public int Day;
    public int OverallCount;
    public int TreesBorn;
    public int TreesDied;
    public float AverageHeight;
}

[System.Serializable]
public class TreeSIndividualDailyInfo
{
    public GameObject Tree;
    public float TreeHeight;
}

[System.Serializable]
public class DeersDailyInfo
{
    public int Day;
    public int OverallCount;
    public int DeersBorn;
    public int DeersDied;
    public float AverageRunSpeed;
    public float AverageRadius;
    public float AverageStamina;
    public float AverageNeckHeight;
    public float AverageStartFleeDistance;
}
[System.Serializable]
public class DeersIndividualDailyInfo
{
    public GameObject Deer;
    public float Runspeed;
    public float SearchRadius;
    public float StaminaAmount;
    public float NeckHeight;
    public float StartFleeDistance;
}

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public CSVExport CSVExport;

    [Header("Coyotes excel info")]
    public float CoyoterunSpeedSum;
    public float CoyoteRadiusSum;
    public float CoyoteStaminaSum;
    public List<CoyotesDailyInfo> CoyotesDailyInfo = new List<CoyotesDailyInfo>();
     
    public int OverallCoyotes;
    public int DailyBornCoyotes;
    public int DailyDiedCoyotes;
    public List<CoyotesIndividualDailyInfo> CoyotesIndividualDailyInfo = new List<CoyotesIndividualDailyInfo>();


    [Header("Trees excel info")]
    public List<TreesDailyInfo> TreesDailyInfo = new List<TreesDailyInfo>();
    public List<TreeSIndividualDailyInfo> TreeSIndividualDailyInfo = new List<TreeSIndividualDailyInfo>();
    public float TreesHeightSum;
    public int OverallTrees;
    public int DailyBornTrees;
    public int DailyDiedTrees;



    [Header("Deers excel info")]

    public List<DeersDailyInfo> DeersDailyInfo = new List<DeersDailyInfo>();
    public List<DeersIndividualDailyInfo> DeersIndividualDailyInfo = new List<DeersIndividualDailyInfo>();
    public float DeersSpeedSum;
    public float DeersRadiusSum;
    public float DeersStaminaSum;
    public float DeersNeckHeightSum;
    public float DeerStartFleeDistanceSum;
    
     
    public int OverallDeers;
    public int DailyBornDeers;
    public int DailyDiedDeers;



    [Header("Light settings")]
    public LightingManager LightingManager;

    public bool ChangeLighting;
    private bool isDefaultLighting;

    private bool skyboxChanged = false;

    

    [Range(0,24)]
    public float TimeOfDay;
    
    public int DayCount;



    public event CoyoteCountChange OnCoyoteCountChange;
    public delegate void CoyoteCountChange(int CoyotesCount);

    public event TreeCountChange OnTreeCountChange;
    public delegate void TreeCountChange(int TreesCount);

    public event DeerCountChange OnDeerCountChange;
    public delegate void DeerCountChange(int DeersCount);

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        DayCount = 0;
        CSVExport = GetComponent<CSVExport>();
        LightingManager = GetComponent<LightingManager>();
       
    }

    // Update is called once per frame
    void Update()
    {   
        //het berel
        // if(Input.GetKeyDown(KeyCode.H))
        // {
        //     CSVExport.WriteCSV(CoyotesDailyInfo, TreesDailyInfo, DeersDailyInfo);
        // }
    }


    //is triggered from time controller
    public void InsertDailyCoyoteData(int day)
    {
        CoyotesDailyInfo CoyotesDayInfo = new CoyotesDailyInfo();
        CoyotesDayInfo.OverallCount = OverallCoyotes;

        
        for (int i = 0; i < CoyotesIndividualDailyInfo.Count; i++)
        {
            CoyoterunSpeedSum += CoyotesIndividualDailyInfo[i].Runspeed;
            CoyoteRadiusSum += CoyotesIndividualDailyInfo[i].SearchRadius;
            CoyoteStaminaSum += CoyotesIndividualDailyInfo[i].StaminaAmount;
        }

        CoyotesDayInfo.AverageRunSpeed = CoyoterunSpeedSum / CoyotesIndividualDailyInfo.Count;
        CoyotesDayInfo.AverageRadius = CoyoteRadiusSum / CoyotesIndividualDailyInfo.Count;
        CoyotesDayInfo.AverageStamina = CoyoteStaminaSum / CoyotesIndividualDailyInfo.Count;

        CoyotesDayInfo.Day = day;

        CoyotesDayInfo.CoyotesBorn = DailyBornCoyotes;
        CoyotesDayInfo.CoyotesDied = DailyDiedCoyotes;

        CoyoterunSpeedSum = 0;
        CoyoteRadiusSum = 0;
        CoyoteStaminaSum = 0;
        DailyBornCoyotes = 0;
        DailyDiedCoyotes = 0;

        CoyotesDailyInfo.Add(CoyotesDayInfo);
        
    }

    public void InsertIndividuaCoyoteData(float _runSpeed, float _radius, float _stamina, GameObject Coyote)
    {
        CoyotesIndividualDailyInfo coyotesIndividualDayInfo = new CoyotesIndividualDailyInfo();
        coyotesIndividualDayInfo.Runspeed = _runSpeed;
        coyotesIndividualDayInfo.SearchRadius = _radius;
        coyotesIndividualDayInfo.StaminaAmount = _stamina;
        coyotesIndividualDayInfo.Coyote = Coyote;

        CoyotesIndividualDailyInfo.Add(coyotesIndividualDayInfo);
        //Debug.Log("ID is " + (CoyotesIndividualInfo.Count -1));
        //return CoyotesIndividualInfo.Count -1; 
    }

    public void RemoveIndividulData(GameObject Coyote)
    {

        for (int i = 0; i < CoyotesIndividualDailyInfo.Count; i++)
        {
            if(CoyotesIndividualDailyInfo[i].Coyote == Coyote)
            {
                 CoyotesIndividualDailyInfo.RemoveAt(i);
            }
        }
    }


    public void InsertDailyTreeData(int day)
    {
        TreesDailyInfo TreesDayInfo = new TreesDailyInfo();
        TreesDayInfo.OverallCount = OverallTrees;

        
        for (int i = 0; i < TreeSIndividualDailyInfo.Count; i++)
        {
            TreesHeightSum += TreeSIndividualDailyInfo[i].TreeHeight;
        }

        TreesDayInfo.AverageHeight = TreesHeightSum / TreeSIndividualDailyInfo.Count;
        

        TreesDayInfo.Day = day;

        TreesDayInfo.TreesBorn = DailyBornTrees;
        TreesDayInfo.TreesDied = DailyDiedTrees;

        TreesHeightSum = 0;
        DailyBornTrees = 0;
        DailyDiedTrees = 0;

        TreesDailyInfo.Add(TreesDayInfo);
        
    }

    public void InsertIndividualTreeData(float _height, GameObject _tree)
    {
        TreeSIndividualDailyInfo treeSIndividualDailyInfo = new TreeSIndividualDailyInfo();
        treeSIndividualDailyInfo.TreeHeight = _height;
        treeSIndividualDailyInfo.Tree = _tree;
        TreeSIndividualDailyInfo.Add(treeSIndividualDailyInfo);     
    }

    public void RemoveIndividualTreeData(GameObject Tree)
    {
        for (int i = 0; i < TreeSIndividualDailyInfo.Count; i++)
        {
            if(TreeSIndividualDailyInfo[i].Tree == Tree)
            {
                 TreeSIndividualDailyInfo.RemoveAt(i);
            }
        }
    }

    public void InsertDailyDeerData(int day)
    {
        DeersDailyInfo DeersDayInfo = new DeersDailyInfo();
        DeersDayInfo.OverallCount = OverallDeers;

        
        for (int i = 0; i < DeersIndividualDailyInfo.Count; i++)
        {
            DeersNeckHeightSum += DeersIndividualDailyInfo[i].NeckHeight;
            DeersSpeedSum += DeersIndividualDailyInfo[i].Runspeed;
            DeersRadiusSum += DeersIndividualDailyInfo[i].SearchRadius;
            DeersStaminaSum += DeersIndividualDailyInfo[i].StaminaAmount;
            DeerStartFleeDistanceSum += DeersIndividualDailyInfo[i].StartFleeDistance;
        }

        DeersDayInfo.AverageNeckHeight = DeersNeckHeightSum / DeersIndividualDailyInfo.Count;
        DeersDayInfo.AverageRunSpeed = DeersSpeedSum / DeersIndividualDailyInfo.Count;
        DeersDayInfo.AverageRadius = DeersRadiusSum / DeersIndividualDailyInfo.Count;
        DeersDayInfo.AverageStamina = DeersStaminaSum / DeersIndividualDailyInfo.Count;
        DeersDayInfo.AverageStartFleeDistance = DeerStartFleeDistanceSum / DeersIndividualDailyInfo.Count;
        

        DeersDayInfo.Day = day;

        DeersDayInfo.DeersBorn = DailyBornDeers;
        DeersDayInfo.DeersDied = DailyDiedDeers;

        DeersNeckHeightSum = 0;
        DeersSpeedSum = 0;
        DeersRadiusSum = 0;
        DeersStaminaSum = 0;
        DeerStartFleeDistanceSum = 0;
        

        DeersDailyInfo.Add(DeersDayInfo);
        
    }

    public void InsertIndividualDeerData(float _runSpeed, float _radius, float _stamina, float _neckHeight, float _startFleeDistance, GameObject _deer)
    {
        DeersIndividualDailyInfo deersIndividualDailyInfo = new DeersIndividualDailyInfo();
        deersIndividualDailyInfo.NeckHeight = _neckHeight;
        deersIndividualDailyInfo.Runspeed = _runSpeed;
        deersIndividualDailyInfo.SearchRadius = _radius;
        deersIndividualDailyInfo.StaminaAmount = _stamina;
        deersIndividualDailyInfo.StartFleeDistance = _startFleeDistance;
        deersIndividualDailyInfo.Deer = _deer;
        
        DeersIndividualDailyInfo.Add(deersIndividualDailyInfo);  
           
    }

    public void RemoveIndividualDeerData(GameObject Deer)
    {
        for (int i = 0; i < DeersIndividualDailyInfo.Count; i++)
        {
            if(DeersIndividualDailyInfo[i].Deer == Deer)
            {
                 DeersIndividualDailyInfo.RemoveAt(i);
            }
        }
    }

    public void AddCoyoteToOverall()
    {
        OverallCoyotes++;
        OnCoyoteCountChange?.Invoke(OverallCoyotes);
    }

    public void RemoveCoyoteFromOverall()
    {
        OverallCoyotes--;
        OnCoyoteCountChange?.Invoke(OverallCoyotes);
    }

    public void AddDeerToOverall()
    {
        OverallDeers++;
        OnDeerCountChange?.Invoke(OverallDeers);
    }

    public void RemoveDeerFromOverall()
    {
        OverallDeers--;
        OnDeerCountChange?.Invoke(OverallDeers);
    }

    public void AddTreeToOverall()
    {
        OverallTrees++;
        OnTreeCountChange?.Invoke(OverallTrees);
    }

    public void RemoveTreeFromOverall()
    {
        OverallTrees--;
        OnTreeCountChange?.Invoke(OverallTrees);
    }


    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SimulationInfoUI : MonoBehaviour
{
    public TextMeshProUGUI TimeText;
    public TextMeshProUGUI DayText;

    public TimeController TimeController;

    public TextMeshProUGUI CoyotesCountText;
    public TextMeshProUGUI DeersCountText;
    public TextMeshProUGUI TreesCountText;

    public GameController GameController;

    void Awake()
    {
        GameController.OnCoyoteCountChange += GameController_OnCoyoteCountChange;
        GameController.OnTreeCountChange += GameController_OnTreeCountChange;
        GameController.OnDeerCountChange += GameController_OnDeerCountChange;
    }
    void Start()
    {
       


        CoyotesCountText.text = "Coyotes - " + 0;
        DeersCountText.text = "Deers - " + 0;
        TreesCountText.text = "Trees - " + 0;
        

    }

    // Update is called once per frame
    void Update()
    {
        TimeText.text = TimeController.currentTime.ToString("HH:mm");
    }

    public void GameController_OnCoyoteCountChange(int OverallCoyotes)
    {
        CoyotesCountText.text = "Coyotes - " + OverallCoyotes.ToString();
    }

    public void GameController_OnTreeCountChange(int OverallTrees)
    {
        
        TreesCountText.text = "Trees - " + OverallTrees.ToString();
    }

    public void GameController_OnDeerCountChange(int OverallDeers)
    {
        
        DeersCountText.text = "Deers - " + OverallDeers.ToString();
    }
}

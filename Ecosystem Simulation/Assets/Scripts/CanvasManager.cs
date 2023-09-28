using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasManager : MonoBehaviour
{
    public SurvivalManager SurvivalManager;
    public ReproductionManager ReproductionManager;
    public Image hungerImg, thurstImg, staminaImg, healthImg, reproductionImg;
    
    public TextMeshProUGUI ObjectText;
    public TextMeshProUGUI NeckHeightText;
    public FieldOfView FieldOfView;
    public PanelFader PanelFader;

    public GameObject reproductionIcon;

    

    public Animal Animal;

    void Start()
    {
        PanelFader = GetComponent<PanelFader>();
        SurvivalManager = GetComponentInParent<SurvivalManager>();
        ReproductionManager = GetComponentInParent<ReproductionManager>();
        FieldOfView = GetComponentInParent<FieldOfView>();
        Animal = GetComponentInParent<Animal>();

        if(ReproductionManager.sex == Sex.Male)
        {
            reproductionIcon.SetActive(true);
        }
        

        if(GetComponentInParent<DeerController>())
        {
            NeckHeightText.text = GetComponentInParent<DeerController>().NeckHeight.ToString("F2");
        }

        StartCoroutine(ShowImageFills());
    }

    public void Update()
    {
        if(PanelFader.CanvasIsVisible)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - PanelFader.Camera.transform.position);
        }
    }



    public IEnumerator ShowImageFills()
    {
        while(true)
        {
            Debug.Log("ashxatuma cor");
            yield return new WaitForSeconds(0.1f);
            if(PanelFader.CanvasIsVisible)
            {
                


                hungerImg.fillAmount = SurvivalManager.hungerPercent;
                thurstImg.fillAmount = SurvivalManager.thurstPercent;
                staminaImg.fillAmount = SurvivalManager.staminaPercent;
                healthImg.fillAmount = SurvivalManager.healthPercent;
                reproductionImg.fillAmount = SurvivalManager.ReproductionPercent;
                Debug.Log("Mtnuma stex");

                if(Animal.state != State.None)
                {
                    ObjectText.text = Animal.state.ToString();
                    
                }
                else if(Animal.state == State.Searching)
                {
                    Animal.searchState.ToString();
                }
            }
              
        }
    }

   
}

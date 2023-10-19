using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectObjectBtnController : MonoBehaviour
{
    public TMP_Text SelectAI_MoojiText;
    public GameObject Indecator3D, IndecatorAI,Select3dPanel,SelectAIPanel;
    public TMP_Text mediaFileCount;
    public Transform Moji3DCountParent, MojiAICountParent;
    public bool isActive3D = false;
    public bool isActiveAI = false;

    private void Update()
    {
        if(isActive3D)
        {
            SelectAI_MoojiText.text = "Select 3d Mooji";
            mediaFileCount.text = Moji3DCountParent.childCount + " Media File";
            Select3dPanel.SetActive(true);
            SelectAIPanel.SetActive(false);
            Indecator3D.SetActive(true);
            IndecatorAI.SetActive(false);
        }
        else if(isActiveAI)
        {
            SelectAI_MoojiText.text = "Select AI";
            mediaFileCount.text = MojiAICountParent.childCount + " Media File";
            Select3dPanel.SetActive(false);
            SelectAIPanel.SetActive(true);
            Indecator3D.SetActive(false);
            IndecatorAI.SetActive(true);
        }
    }

    public void SelectActive3DController(SelectObjectBtnController selectObjectBtnController)
    {
        isActive3D = true;
        selectObjectBtnController.isActive3D = false;
        selectObjectBtnController.isActiveAI = false;
        FindAnyObjectByType<GameManager>().MakeSelectedNull();
    }

    public void SelectActiveAIController(SelectObjectBtnController selectObjectBtnController)
    {
        isActiveAI = true;
        selectObjectBtnController.isActive3D = false;
        selectObjectBtnController.isActiveAI = false;
        FindAnyObjectByType<GameManager>().MakeSelectedNull();
    }
}

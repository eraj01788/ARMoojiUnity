using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DropPrefabManager : MonoBehaviour
{
    public TMP_Text DropMessage;
    public GameObject AddFriendBtn;
    public Canvas canvas;
    bool isSent=false;
    void Start()
    {
        canvas.worldCamera = Camera.main;
    }
    public void ProfileClick()
    {
        if(!isSent)
        {
            AddFriendBtn.SetActive(true);
            isSent = true;

        }
    }

    public void AddFrind()
    {
        AddFriendBtn.SetActive(false);
    }
}

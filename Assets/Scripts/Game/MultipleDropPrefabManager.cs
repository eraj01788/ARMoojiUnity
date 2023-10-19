using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MultipleDropPrefabManager : MonoBehaviour
{
    public string location;
    public GameObject UserPanel;
    public Transform content;
    public TMP_Text MojiCountText;
    public GameObject MessageListPanel, MessageSmallView;
    public Canvas canvas;
    GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        canvas.worldCamera = Camera.main;
    }

    public void OpenMessageListPanel()
    {
        MessageListPanel.SetActive(true);
        MessageSmallView.SetActive(false);
        foreach (var item in gameManager.DropPrefabs)
        {
            item.SetActive(false);
        }
    }

    public void CloseMessageListPanel()
    {
        MessageListPanel.SetActive(false);
        MessageSmallView.SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendListUIManager : MonoBehaviour
{
    UserDataManager dataManager;
    public GameObject FriendRequestPanelPrefab, FriendListPanelPrefab;
    public RectTransform frpParent, flpParent;
    public List<GameObject> allFriendSt = new List<GameObject>();
    void Start()
    {
        dataManager = FindObjectOfType<UserDataManager>();
        CreateUpdateFriendUI();
    }

    public void CreateUpdateFriendUI()
    {

        foreach (var flist in dataManager.friendLists)
        {
            if(flist.isAccepted=="Wait For Resp.")
            {
                GameObject FriendRequestPanel = Instantiate(FriendRequestPanelPrefab, frpParent);
                FriendRequestManager friendRequestManager = FriendRequestPanel.GetComponent<FriendRequestManager>();
                friendRequestManager.fid = flist.friendId;
                friendRequestManager.confirmFriendBtn.SetActive(true);
                allFriendSt.Add(FriendRequestPanel);
            }
            else if (flist.isAccepted == "Request Sent")
            {
                GameObject FriendRequestPanel = Instantiate(FriendRequestPanelPrefab, frpParent);
                FriendRequestManager friendRequestManager = FriendRequestPanel.GetComponent<FriendRequestManager>();
                friendRequestManager.fid = flist.friendId;
                friendRequestManager.confirmFriendBtn.SetActive(false);
                allFriendSt.Add(FriendRequestPanel);
            }
            else if(flist.isAccepted == "Friend")
            {
                GameObject FriendListPanel = Instantiate(FriendListPanelPrefab, flpParent);
                FriendChatManager friendListManager = FriendListPanel.GetComponent<FriendChatManager>();
                friendListManager.fid = flist.friendId;
                allFriendSt.Add(FriendListPanel);
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(frpParent);
        LayoutRebuilder.ForceRebuildLayoutImmediate(flpParent);
    }

    public void AcceptFriendRequest(string fid,GameObject toDestroy)
    {
        dataManager.AcceptFriendRequest(fid, isSuccess =>
        {
            if (isSuccess)
            {
                Destroy(toDestroy);
            }
        });
    }
}

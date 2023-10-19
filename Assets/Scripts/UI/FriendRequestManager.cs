using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendRequestManager : MonoBehaviour
{
    public string fid;
    public FriendListUIManager friendListUIManager;
    public GameObject confirmFriendBtn;

    private void Start()
    {
        friendListUIManager = FindObjectOfType<FriendListUIManager>();
    }

    public void AcceptFriend()
    {
        friendListUIManager.AcceptFriendRequest(fid,this.gameObject);
    }
   
}

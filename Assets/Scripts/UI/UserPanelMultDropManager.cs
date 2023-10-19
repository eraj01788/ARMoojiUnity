using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserPanelMultDropManager : MonoBehaviour
{
    public TMP_Text UserNameText, UserMessageText;
    public Image ProfileImage;
    public GameObject ObjectToView;
    public MultipleDropPrefabManager mdpm;
    public string friendId;
    public string friendName;
    UserDataManager dataManager;
    public string friendStatus;
    public Button AddFriendBtn;
    public TMP_Text FriendStatusText;

    private void Start()
    {
        dataManager = FindObjectOfType<UserDataManager>();
        if (friendId == dataManager.loginManager.user.UserId)
        {
            AddFriendBtn.interactable = false;
            FriendStatusText.text = "Me";
        }
        else
        {
            dataManager.StartListeningForFriendStatusChanges(friendId, friendStatus => {
                if (friendStatus == "Friend")
                {
                    AddFriendBtn.interactable = true;
                    FriendStatusText.text = "Chat";
                }
                else if (friendStatus == "Request Sent")
                {
                    AddFriendBtn.interactable = false;
                    FriendStatusText.text = "Request Sent";
                }
                else if (friendStatus == "Wait For Resp.")
                {
                    AddFriendBtn.interactable = false;
                    FriendStatusText.text = "Wait For Resp.";
                }
                else if (friendStatus == "Friend Data not found")
                {
                    AddFriendBtn.interactable = true;
                    FriendStatusText.text = "Add Friend";
                }
            });
        }
    }

    private void Update()
    {
        
    }

    void HandleFriendStatus(string status)
    {
        friendStatus = status;
    }

    public void ViewFullMessage()
    {
        ObjectToView.SetActive(true);
        mdpm.CloseMessageListPanel();
    }

    public void AddFriend()
    {
        dataManager.AddFriend(friendId, friendName, false);
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Placed3DObjectData
{
    public string userId;
    public string userName;
    public string objectId;
    public string ObjectName;
    public string ObjectLocation;
    public string message;
    public bool isInsideMult;
    public GameObject RefObject;

    public Placed3DObjectData(string id,string objectName, string objectLocation,string msg,bool insideStatus,string oid,string name)
    {
        userId = id;
        ObjectName = objectName;
        ObjectLocation = objectLocation;
        message = msg;
        isInsideMult = insideStatus;
        objectId = oid;
        userName = name;
    }
}

[System.Serializable]
public class FriendList
{
    public string friendId, friendName;
    public string isAccepted;

    public FriendList(string id,string name,string isAcc)
    {
        friendId = id;
        friendName = name;
        isAccepted = isAcc;
    }
}
public class User
{
    public string userID;
    public string Name;
    public bool IsActive;

    public User(string id, string name, bool acc)
    {
        userID = id;
        Name = name;
        IsActive = acc;
    }
}

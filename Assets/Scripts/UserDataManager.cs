using Ai.Inworld.Studio.V1Alpha;
using DG.Tweening.Core.Easing;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

public class UserDataManager : MonoBehaviour
{
    // Start is called before the first frame update
    public string Name;
    public string ID;
    public bool isActive;
    public List<FriendList> friendLists = new List<FriendList>();
    public List<Placed3DObjectData> placed3DObjects = new List<Placed3DObjectData>();
    private bool isFirstUpdate = true;
    public float detectionDistance = 10f; // Set your desired detection distance in meters.
    GameManager gameManager;
    public FriendListUIManager friendListUIManager;

    public LoginManager loginManager;

    void Start()
    {
        loginManager = FindObjectOfType<LoginManager>();
        gameManager = FindObjectOfType<GameManager>();

    }

    public void StartGameAndPlaceObject()
    {
        StartListeningForUserDataChanges();
        StartListeningForPlacedObjectData();
        StartListeningForFriendListChanges();
        UpdateIsActiveStatus(true);
    }

    void StartListeningForPlacedObjectData()
    {
        // Specify the path to the placed object data
        string placedObjectPath = "PlacedObject";

        // Set up a listener for data changes at the "PlacedObject" path
        loginManager.reference.Child(placedObjectPath).ValueChanged += HandlePlacedObjectValueChanged;
    }

    private void HandlePlacedObjectValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError("Database error: " + args.DatabaseError.Message);
            return;
        }

        if (args.Snapshot.Exists)
        {
            if (isFirstUpdate)
            {
                foreach (var childSnapshot in args.Snapshot.Children)
                {
                    string objectName = childSnapshot.Child("ObjectName").Value.ToString();
                    string objectLocation = childSnapshot.Child("ObjectLocation").Value.ToString();
                    string message = childSnapshot.Child("message").Value.ToString();
                    bool isInsideMult = (bool)childSnapshot.Child("isInsideMult").Value;
                    string objectId = childSnapshot.Child("objectId").Value.ToString();
                    // Create a Placed3DObject instance with the retrieved data
                    string displayName = childSnapshot.Child("userName").Value.ToString();
                    string userId = childSnapshot.Child("userId").Value.ToString();

                    Placed3DObjectData placedObject = new Placed3DObjectData(userId, objectName, objectLocation, message, false, objectId, displayName);

                    // Add the placed object to your list or process it as needed
                    placed3DObjects.Add(placedObject);
                    PlaceObject(placedObject);
                }
                isFirstUpdate = false; // Set the flag to false to indicate subsequent updates
            }
            else
            {
                // Subsequent updates, retrieve and process only the latest data
                var latestChild = args.Snapshot.Children.LastOrDefault();

                if (latestChild != null)
                {
                    string objectName = latestChild.Child("ObjectName").Value.ToString();
                    string objectLocation = latestChild.Child("ObjectLocation").Value.ToString();
                    string message = latestChild.Child("message").Value.ToString();
                    bool isInsideMult = (bool)latestChild.Child("isInsideMult").Value;
                    string objectId = latestChild.Child("objectId").Value.ToString();
                    string displayName = latestChild.Child("userName").Value.ToString();
                    string userId = latestChild.Child("userId").Value.ToString();
                    // Create a Placed3DObject instance with the retrieved data

                    Placed3DObjectData placedObject = new Placed3DObjectData(userId, objectName, objectLocation, message, false, objectId, displayName);
                    placed3DObjects.Add(placedObject);
                    PlaceObject(placedObject);
                }
            }
        }
    }

    public void UpdatePlacedObjectStatus(string objectKey, bool newStatus)
    {
        // Specify the path to the placed object
        string placedObjectPath = "PlacedObject";

        // Get a reference to the Firebase Database for the specific object using its key
        DatabaseReference databaseReference = loginManager.reference.Child(placedObjectPath).Child(objectKey);

        // Create a dictionary to update only the 'status' field
        Dictionary<string, object> updates = new Dictionary<string, object>
    {
        { "isInsideMult", newStatus }
    };

        // Update the 'status' field in Firebase
        databaseReference.UpdateChildrenAsync(updates)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("Placed object status updated successfully.");
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError("Failed to update placed object status: " + task.Exception.Message);
                }
            });
    }
    void PlaceObject(Placed3DObjectData placedObject)
    {
        gameManager.DropMessageFromFirebase(placedObject);
    }


    void StartListeningForUserDataChanges()
    {
        // Specify the path to the data you want to listen to
        string dataPath = "users/"+loginManager.user.UserId; // Replace with the actual path to your data
        // Set up a listener for data changes at the specified path
        loginManager.reference.Child(dataPath).ValueChanged += HandleValueChanged;
    }

    private void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError("Database error: " + args.DatabaseError.Message);
            return;
        }

        if (args.Snapshot.Exists)
        {
            string updatedName = args.Snapshot.Child("Name").Value.ToString();
            string updatedID = args.Snapshot.Child("ID").Value.ToString();
            bool activestatus = (bool)args.Snapshot.Child("isActive").Value;
            Name = updatedName;
            ID = updatedID;
            isActive = activestatus;
        }
    }

    public void AddFriend(string friendUserId, string friendName, bool isAccepted)
    {
        // Specify the path to the user's friend list
        string friendListPathmine = "users/" + loginManager.user.UserId + "/FriendList/" + friendUserId;
        string friendListPathfriend = "users/" + friendUserId + "/FriendList/" + loginManager.user.UserId;
        string displayName;
        if (string.IsNullOrEmpty( loginManager.user.DisplayName))
        {
            displayName = "Anonymously";
        }
        else
        {
            displayName = loginManager.user.DisplayName;
        }
        // Create a new FriendList object with the friend's information
        FriendList friendM = new FriendList(friendUserId, friendName, "Request Sent");
        FriendList friendF = new FriendList(loginManager.user.UserId, displayName, "Wait For Resp.");

        // Convert the FriendList object to a JSON string
        string friendMJson = JsonUtility.ToJson(friendM);
        string friendFJson = JsonUtility.ToJson(friendF);

        // Get a reference to the Firebase Database at the specified friend list path and set the new friend data
        DatabaseReference friendMReference = loginManager.reference.Child(friendListPathmine);
        DatabaseReference friendFReference = loginManager.reference.Child(friendListPathfriend);
        friendMReference.SetRawJsonValueAsync(friendMJson).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Friend added successfully.");
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("Failed to add friend: " + task.Exception.Message);
            }
        });
        friendFReference.SetRawJsonValueAsync(friendFJson).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Friend added successfully.");
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("Failed to add friend: " + task.Exception.Message);
            }
        });
    }

    public void StartListeningForFriendStatusChanges(string friendUserId, Action<string> callback)
    {
        // Specify the path to the friend's status data
        string friendStatusPath = "users/" + loginManager.user.UserId + "/FriendList/" + friendUserId;

        // Set up a listener for changes in friend's status data
        loginManager.reference.Child(friendStatusPath).ValueChanged += (sender, args) =>
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError("Database error: " + args.DatabaseError.Message);
                return;
            }

            if (args.Snapshot.Exists)
            {
                // Deserialize the friend data
                string friendJson = args.Snapshot.GetRawJsonValue();
                FriendList friend = JsonUtility.FromJson<FriendList>(friendJson);

                if (friend.isAccepted=="Friend")
                {
                    // Friend is accepted, call the callback with the status
                    callback("Friend");
                }
                else if(friend.isAccepted == "Request Sent")
                {
                    // Friend is not accepted, call the callback with the status
                    callback("Request Sent");
                }
                else if (friend.isAccepted == "Wait For Resp.")
                {
                    // Friend is not accepted, call the callback with the status
                    callback("Wait For Resp.");
                }
            }
            else
            {
                // Friend data not found, call the callback with an appropriate status
                callback("Friend Data not found");
            }
        };
    }

    void StartListeningForFriendListChanges()
    {
        // Specify the path to the user's friend list
        string friendListPath = "users/" + loginManager.user.UserId + "/FriendList";

        // Get a reference to the Firebase Database at the specified friend list path
        DatabaseReference friendListReference = loginManager.reference.Child(friendListPath);

        // Set up a listener for changes in the entire friend list
        friendListReference.ValueChanged += HandleFriendListChanged;
    }

    private void HandleFriendListChanged(object sender, ValueChangedEventArgs args)
    {
        friendLists.Clear();
        if (args.DatabaseError != null)
        {
            Debug.LogError("Database error: " + args.DatabaseError.Message);
            return;
        }

        if (args.Snapshot.Exists)
        {
            foreach (var child in args.Snapshot.Children)
            {
                string friendJson = child.GetRawJsonValue();
                FriendList friend = JsonUtility.FromJson<FriendList>(friendJson);
                friendLists.Add(friend);
            }
            if(friendListUIManager.gameObject.activeSelf)
            {
                friendListUIManager.CreateUpdateFriendUI();
            }
        }
        else
        {
            // Handle the case where the friend list is empty or not found
            Debug.Log("Friend List is empty or not found.");
        }
    }

    public void AddPlacedObject(string objectName, string objectLocation,string message)
    {
        string placedObjectPath = "PlacedObject";

        // Convert the PlacedObject instance to a JSON string

        // Get a reference to the Firebase Database at the placed object path and set the data
        DatabaseReference databaseReference = loginManager.reference.Child(placedObjectPath).Push();
        string uniqueId = databaseReference.Key;

        string displayName = loginManager.user.DisplayName;
        if (string.IsNullOrEmpty(displayName))
        {
            displayName = "Anonymous";
        }

        Placed3DObjectData placedObject = new Placed3DObjectData(loginManager.user.UserId, objectName, objectLocation, message, false, uniqueId, displayName);

        string placedObjectJson = JsonUtility.ToJson(placedObject);
        databaseReference.SetRawJsonValueAsync(placedObjectJson)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("Placed object added successfully.");
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError("Failed to add placed object: " + task.Exception.Message);
                }
            });
    }

    public void UpdateIsActiveStatus(bool isActive)
    {

        // Replace "userId" with the actual user ID or a key to identify the user.
        string userId = loginManager.user.UserId;

        // Update the isActive status for the user.
        loginManager.reference.Child("users").Child(userId).Child("IsActive").SetValueAsync(isActive);
    }

    public void AcceptFriendRequest(string friendId, Action<bool> callback)
    {
        // Specify the path to the user's friend list
        string friendListPathM = "users/" + loginManager.user.UserId + "/FriendList";
        string friendListPathF = "users/" + friendId + "/FriendList";

        // Get a reference to the Firebase Database at the friend list path
        DatabaseReference friendListMReference = loginManager.reference.Child(friendListPathM);
        DatabaseReference friendListFReference = loginManager.reference.Child(friendListPathF);

        // Use the SetValueAsync method to update the friend's "isAccepted" status
        friendListMReference.Child(friendId).Child("isAccepted").SetValueAsync("Friend")
        .ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                friendListFReference.Child(loginManager.user.UserId).Child("isAccepted").SetValueAsync("Friend")
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompleted)
                    {
                        // Friend request accepted
                        callback(true);
                    }
                    else if (task.IsFaulted)
                    {
                        Debug.LogError("Failed to accept friend request: " + task.Exception.Message);
                        callback(false);
                    }
                });
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("Failed to accept friend request: " + task.Exception.Message);
                callback(false);
            }
        });
    }


    private void OnApplicationQuit()
    {
        UpdateIsActiveStatus(false);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            UpdateIsActiveStatus(false);
            print("pause");
        }
        else
        {
            UpdateIsActiveStatus(true);
            print("not pause");
        }
    }
}

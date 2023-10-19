using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapChildLayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Transform child in gameObject.transform)
        {
            int LayerMap = LayerMask.NameToLayer("Map");
            child.gameObject.layer = LayerMap;
            if (child.childCount > 0)
            {
                foreach (Transform child_child in child)
                {
                    child_child.gameObject.layer = LayerMap;
                }
            }
        }
    }
}

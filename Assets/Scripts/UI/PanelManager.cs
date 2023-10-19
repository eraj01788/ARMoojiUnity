using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PanelManager : MonoBehaviour
{
    public Dictionary<string, GameObject> panels = new Dictionary<string, GameObject>();
    public List<GameObject> UIPanels = new List<GameObject>();

    private void Start()
    {
        RegisterPanel();
    }

    public void RegisterPanel()
    {
        foreach (var panelObj in UIPanels)
        {
            panels.Add(panelObj.name, panelObj);
        }
    }

    public void ShowPanel(string panelName)
    {
        foreach (var panel in panels.Values)
        {
            panel.SetActive(panel.name == panelName);
        }
    }
}

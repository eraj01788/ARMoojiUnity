using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System;
using Unity.VisualScripting;
using UnityEngine.UI;
using static Google.Rpc.Help.Types;

public class ClickableDropMessage : MonoBehaviour
{
    TMP_Text DropMessageText;
    string checkText;
    Uri uriResult;
    bool isUrl;
    private void Start()
    {
        DropMessageText = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        checkText = DropMessageText.text;
        isUrl = Uri.IsWellFormedUriString(checkText, UriKind.Absolute);
        if(isUrl)
        {
            GetComponent<Button>().enabled = true;
        }
        else
        {
            GetComponent<Button>().enabled = false;
        }
    }

    public void OpenUrl()
    {
        Application.OpenURL(GetLinkFromText(checkText));
    }

    public string GetLinkFromText(string input)
    {
        string link="";
        // Find the first occurrence of 'http://' or 'https://'
        int startIndex = input.IndexOf("http://");
        if (startIndex == -1)
            startIndex = input.IndexOf("https://");

        if (startIndex != -1)
        {
            // Find the end of the URL (e.g., when space or newline is encountered)
            int endIndex = input.IndexOfAny(new char[] { ' ', '\n' }, startIndex);
            if (endIndex == -1)
                endIndex = input.Length;

            // Extract the link and text
            link = input.Substring(startIndex, endIndex - startIndex);
            string text = input.Remove(startIndex, endIndex - startIndex).Trim();
        }

        return link;
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StringUtility : MonoBehaviour
{
    public const char BACKSLASH = '\\';

    public static StringUtility Instance;

    public bool IsTyping = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void TypeTextEffect(TMP_Text displayText, string contentText, float speedMultiplier)
    {
        Instance.StartCoroutine(Instance.TypeText(displayText, contentText, speedMultiplier));
    }

    private IEnumerator TypeText(TMP_Text displayText, string contentText, float speedMultiplier)
    {
        for (int i = 0; i < contentText.Length; i++)
        {
            IsTyping = true;
            char letter = contentText[i];
            if (letter.Equals(BACKSLASH))
            {
                if (i == contentText.Length - 1)
                {
                    Debug.Log("ERROR: You have a backslash as the last character of your input string.");
                    break;
                }

                char nextLetter = contentText[i + 1];
                i += 1;
                switch (nextLetter)
                {
                    case 'p': // '\p' means pause for this character
                        yield return new WaitForSecondsRealtime(0.5f / speedMultiplier);
                        break;
                    case 's': // '\p' means small pause for this character
                        yield return new WaitForSecondsRealtime(0.1f / speedMultiplier);
                        break;
                    case 'n': // '\n' means add a new line
                        displayText.text += '\n';
                        yield return new WaitForSecondsRealtime(0.05f / speedMultiplier);
                        break;
                }
            } 
            else
            {
                displayText.text += letter;
                yield return new WaitForSecondsRealtime(0.05f / speedMultiplier);
            }
        }
        IsTyping = false;
    }
}
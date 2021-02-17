﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Holding : Building
{
    // How much does a player entering the hitbox change the scale?
    public const float EntryScaleDelta = 0.05f;
    public const float MinYScale = 0.1f;

    public int ThresholdNumHeldPlayers = 4;

    public List<GameObject> HeldPlayers = new List<GameObject>();

    public TMP_Text ThresholdText;

    public bool stopped = false;

    public void IncrementThreshold()
    {
        ThresholdNumHeldPlayers += 1;

        Vector3 currScale = transform.localScale;
        transform.localScale = new Vector3(currScale.x, EntryScaleDelta * ThresholdNumHeldPlayers + MinYScale, currScale.z);
        Vector3 currPosition = transform.localPosition;
        transform.localPosition = new Vector3(currPosition.x, currPosition.y - currScale.y / 2 + transform.localScale.y / 2, currPosition.z);

        ThresholdText.text = HeldPlayers.Count.ToString() + "/" + ThresholdNumHeldPlayers.ToString();
    }

    public void DecrementThreshold()
    {
        if (ThresholdNumHeldPlayers == 1)
        {
            return;
        }
        ThresholdNumHeldPlayers -= 1;

        Vector3 currScale = transform.localScale;
        transform.localScale = new Vector3(currScale.x, EntryScaleDelta * ThresholdNumHeldPlayers + MinYScale, currScale.z);
        Vector3 currPosition = transform.localPosition;
        transform.localPosition = new Vector3(currPosition.x, currPosition.y - currScale.y / 2 + transform.localScale.y / 2, currPosition.z);

        ThresholdText.text = HeldPlayers.Count.ToString() + "/" + ThresholdNumHeldPlayers.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!stopped)
        {
            // Debug.Log("enter");
            if (other.CompareTag("player"))
            {
                other.gameObject.GetComponent<UnitMovement>().UnitSpeed = 0.2f;
                HeldPlayers.Add(other.gameObject);

                Vector3 currScale = transform.localScale;
                transform.localScale = Vector3.Max(currScale - new Vector3(0, EntryScaleDelta, 0), Vector3.zero);
                Vector3 currPosition = transform.localPosition;
                transform.localPosition = new Vector3(currPosition.x, currPosition.y - currScale.y / 2 + transform.localScale.y / 2, currPosition.z);

                ThresholdText.text = HeldPlayers.Count.ToString() + "/" + ThresholdNumHeldPlayers.ToString();
            }
            if (HeldPlayers.Count == ThresholdNumHeldPlayers)
            {
                stopped = true;
                // Debug.Log("stopped");
                foreach (GameObject player in HeldPlayers)
                {
                    player.GetComponent<UnitMovement>().UnitSpeed = 2f;
                }
                HeldPlayers.Clear();
                gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!stopped)
        {
            // Debug.Log("exit");
            if (other.CompareTag("player"))
            {
                other.gameObject.GetComponent<UnitMovement>().UnitSpeed = 2f;
                HeldPlayers.Remove(other.gameObject);

                Vector3 currScale = transform.localScale;
                transform.localScale = currScale + new Vector3(0, EntryScaleDelta, 0);
                Vector3 currPosition = transform.localPosition;
                transform.localPosition = new Vector3(currPosition.x, currPosition.y - currScale.y / 2 + transform.localScale.y / 2, currPosition.z);

                ThresholdText.text = HeldPlayers.Count.ToString() + "/" + ThresholdNumHeldPlayers.ToString();
            }
        }
    }
}

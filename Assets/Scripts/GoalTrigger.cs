﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalTrigger : MonoBehaviour
{
    public static GoalTrigger Instance;

    [Header("Level Specifics")]
    public int NumPackages = 4;
    public int packagesDelivered = 0;
    public int packagesLost = 0;
    public bool levelCanEnd = true;

    private int optimalRemainingBudget;
    private int targetRemainingBudget;

    [Header("UI")]
    public GameObject ResultsPanel;
    public GameObject MenuPanel;
    public GameObject ActionsPanel;
    public GameObject ControlsPanel;
    public GameObject GoalEffectPrefab;
    public ResultsPanelTypeEffect ResultsPanelTypeEffect;

    bool levelDoneAlready = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.EnrollSFXSource(GetComponent<AudioSource>());
        optimalRemainingBudget = MoneyManager.Instance.OptimalRemaining;
        ControlsPanel = GameObject.Find("Controls");
    }


    // Update is called once per frame
    void Update()
    {
        if (levelDoneAlready)
        {
            return;
        }

        // Finish level and bring up results panel
        if (IsLevelDone() && levelCanEnd)
        {
            FinishLevel();
            levelDoneAlready = true;
        }
    }

    private void FinishLevel()
    {
        // Hide UI
        // TODO: Make this an animation
        MenuPanel.SetActive(false);
        ActionsPanel.SetActive(false);
        ControlsPanel.SetActive(false);

        int remainingBudget = MoneyManager.Instance.GetRemainingMoney();
        float percentPackagesDelivered = (float) packagesDelivered / NumPackages * 100f;
        
        // Determining performance string
        int perfInt = DeterminePerformance(
            percentPackagesDelivered, 
            remainingBudget,
            optimalRemainingBudget
            );

        // Unlock next level
        if (percentPackagesDelivered >= 50 && remainingBudget >= 0)
        {
            int highestLevelUnlocked = PlayerPrefs.GetInt(LevelSelectUI.PLAYER_PREFS_HIGHEST_LEVEL_UNLOCKED, 0);
            int nextLevel = GetCurrentLevel() + 1;
            if (highestLevelUnlocked < nextLevel)
            {
                PlayerPrefs.SetInt(LevelSelectUI.PLAYER_PREFS_HIGHEST_LEVEL_UNLOCKED, nextLevel);
            }

            // Set Score
            int currentHighScore = PlayerPrefs.GetInt(LevelSelectUI.PLAYER_PREFS_HIGH_SCORE_BASE + GetCurrentLevel(), 0);
            if (perfInt > currentHighScore)
            {
                PlayerPrefs.SetInt(LevelSelectUI.PLAYER_PREFS_HIGH_SCORE_BASE + GetCurrentLevel(), perfInt);
            }
        }


        ResultsPanelTypeEffect.SetIntroText(packagesDelivered, NumPackages, optimalRemainingBudget - remainingBudget, perfInt);

        // Hide next button if the level is failed
        if (perfInt == 0)
        {
            UIManager.Instance.NextButton.SetActive(false);
        } else
        {
            UIManager.Instance.NextButton.SetActive(true);
        }
        ResultsPanel.GetComponent<Animator>().SetBool("open", true);

        levelDoneAlready = true;
    }

    private int DeterminePerformance(
        float percentPackagesDelivered, 
        int remainingBudget,
        int optimalRemainingBudget
        )
    {
        if (percentPackagesDelivered < 50 || remainingBudget < 0)
        {
            return 0;
        } else if (percentPackagesDelivered < 100 && remainingBudget < optimalRemainingBudget)
        {
            return 1;
        } else if (percentPackagesDelivered < 100 && remainingBudget >= optimalRemainingBudget)
        {
            return 2;
        } else if (percentPackagesDelivered == 100 && remainingBudget < optimalRemainingBudget)
        {
            return 2;
        } else if (percentPackagesDelivered == 100 && remainingBudget >= optimalRemainingBudget)
        {
            return 3;
        }
        return -1;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
        {
            GameObject cameraParent = GameObject.FindGameObjectWithTag("cameraParent");
            if (cameraParent != null)
            {
                CameraShake cameraShake = cameraParent.GetComponent<CameraShake>();
                if (cameraShake != null)
                {
                    cameraShake.StartCameraShake(0.5f, 0.1f);
                }
            }
            GetComponent<AudioSource>().pitch = Random.Range(0.99f, 1.01f);
            GetComponent<AudioSource>().Play();
            packagesDelivered += other.GetComponent<Dog>().NumPackages;
            other.gameObject.GetComponent<Dog>().DeleteDog();
            Instantiate(GoalEffectPrefab, transform.position, Quaternion.Euler(270.0f, 0.0f, 0.0f));
        }
    }

    public void ResetPlayerResults()
    {
        packagesDelivered = 0;
        packagesLost = 0;
        levelDoneAlready = false;
    }

    public bool IsLevelDone()
    {
        return packagesDelivered + packagesLost == NumPackages;
    }

    private int GetCurrentLevel()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        int currentLevel = -1;
        if (int.TryParse(currentSceneName.Substring(5), out currentLevel))
        {
            return currentLevel;
        }
        return -1;
    }

}

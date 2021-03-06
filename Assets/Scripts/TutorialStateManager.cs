﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialStateManager : MonoBehaviour
{
  
    public enum State{zero, one_text, one_wait, two_text, two_wait, three_text, three_wait, four_text, four_wait, five_text, five_wait, 
                        six_text, six_wait, seven_text, seven_wait, eight_text, eight_wait, nine_text, nine_wait, ten_text, ten_wait,
                        eleven_text, eleven_wait, twelve_text, twelve_wait, thirteen_text, thirteen_wait, fourteen_text, fourteen_wait, fifteen_text, fifteen_wait,
                        sixteen_text, sixteen_wait, seventeen_text, seventeen_wait, eightteen_text, eightteen_wait, nineteen_text, nineteen_wait,
                        tenpointfive_text, tenpointfive_wait}

    public State currState;

    [Header("UI_Elements")]
    public TextMeshProUGUI tutorialNarration;
    public GameObject NarrationPanel;
    public Button PlayButton;
    public Button ResetButton;
    public GameObject BuildingPanel;
    public GameObject MenuPanel;
    public GameObject Controls;
    public Button SignButton;
    public Button ControlsButton;
    public Animator BuildingFadeIn;

    [Header("Type Speed")]
    public float TypeSpeed;

    [Header("Objects")]
    public GameObject Tile1;
    public GameObject Tile2;
    public GameObject Tile2_pcube1;
    public GameObject mainCamera;

    [Header("Abstract")]
    public GameObject buildManager;

    private bool textFlag = true;
    private bool eighteen_wait_flag = true;
    public Button optimalButton;

    private void Start() {
        tutorialNarration.text = "";
        optimalButton.enabled = false;
    }

    private void Update() {
        switch(currState){
        case State.zero:
            if (NarrationPanel.GetComponent<NarrationPanelScript>().canUse){
                currState = State.one_text;
            }
            break;
        case State.one_text:
            if (textFlag){
                StringUtility.TypeTextEffect(tutorialNarration, 
                "Hey!\\p Laura here.\\p\\p\\n\\n" +
                "Let's walk through how \\n" +
                "Loss Prevention works.\\p\\p\\n\\n" +
                "Send in our associates\\n"+
                "to pick up those lost\\n"+
                "packages by pressing\\n"+
                "the play button.\\n",
                TypeSpeed);
                PlayButton.interactable = false;
                ResetButton.interactable = false;
                SignButton.interactable = false;
                textFlag = false;
            }
            // PlayButton.GetComponent<Animator>().SetTrigger("Highlighted");
            if (!StringUtility.Instance.IsTyping){
                PlayButton.interactable = true;
                PlayButton.transform.parent.GetComponent<Animator>().SetBool("highlightedPlay", true);
                currState = State.one_wait;
                textFlag = true;
            }
            break;
        case State.one_wait:
            if (GameStateManager.Instance.CurrState == GameStateManager.State.Play){
                PlayButton.transform.parent.GetComponent<Animator>().SetBool("highlightedPlay", false);
                PlayButton.interactable = false;
                ResetButton.interactable = false;
                // PlayButton.GetComponent<Animator>().SetTrigger("Normal");
            }
            if (GoalTrigger.Instance.IsLevelDone()){
                currState = State.two_text;
                textFlag = true;
            }
            break;
        case State.two_text:
            if (textFlag){
                tutorialNarration.text = "";
                StringUtility.TypeTextEffect(tutorialNarration, 
                "Oops!\\p\\p\\p Don't worry we'll \\n"+
                "pick him up later.\\p\\p\\n\\n"+
                "Did I mention Deep Space\\n"+ 
                "Delivery is number 1 in\\n"+
                "human employee safety? \\p\\p\\n\\n"+
                "Press the retry button\\n"+
                "to try again.",
                TypeSpeed);
                textFlag = false;
            }
            // ResetButton.GetComponent<Animator>().SetTrigger("Highlighted");
            if (!StringUtility.Instance.IsTyping){
                ResetButton.transform.parent.GetComponent<Animator>().SetBool("highlightedReset", true);
                textFlag = true;
                ResetButton.interactable = true;
                currState = State.two_wait;
            }
            break;
        case State.two_wait:
            if (GameStateManager.Instance.CurrState == GameStateManager.State.Plan){
                ResetButton.transform.parent.GetComponent<Animator>().SetBool("highlightedReset", false);
                currState = State.three_text;
                ResetButton.interactable = false;
                // ResetButton.GetComponent<Animator>().SetTrigger("Normal");
            }
            break;
        case State.three_text:
            if (textFlag){
                tutorialNarration.text = "";
                StringUtility.TypeTextEffect(tutorialNarration, 
                "While our associates are\\n"+
                "diligent,\\p they're not\\n"+
                "always the brightest.\\p\\p\\n\\n"+
                "It's our job to point 'em\\n"+
                "in the right direction.\\n\\n"+
                "\\p\\pTry building a sign.",
                TypeSpeed);
                BuildingPanel.SetActive(true);
                SignButton.interactable = false;
                Tile2.layer = 0;
                textFlag = false;
            }
            if (!StringUtility.Instance.IsTyping){
                SignButton.interactable = true;
                BuildManager.Instance.allowBuildingNewBuildings = true;
                textFlag = true;
                SignButton.transform.parent.GetComponent<Animator>().SetBool("highlighted", true);
                BuildingFadeIn.SetBool("visible", true);
                currState = State.three_wait;
            }
            break;
        case State.three_wait:
            if (BuildManager.Instance.CurrBuilding != null){
                currState = State.four_text;
                BuildManager.Instance.allowBuildingNewBuildings = false;
                SignButton.transform.parent.GetComponent<Animator>().SetBool("highlighted", false);
                BuildingFadeIn.SetBool("visible", false);
                // BuildingPanel.SetActive(false);
                SignButton.interactable = false;
                TileManager.Instance.AddUnoccupiedTile(Tile2.GetComponent<Tile>());
            }
            break;
        case State.four_text:
            if (textFlag){
                tutorialNarration.text = "";
                StringUtility.TypeTextEffect(tutorialNarration, 
                "The meter on the left\\n"+
                "tracks your expenditure.\\n\\p"+
                "We've also marked it with\\n"+
                "how much we expect you to\\n"+
                "spend per mission.\\p\\p\\n\\n" + 
                "Click to place the sign.",
                TypeSpeed);
                textFlag = false;
            }
            if (!StringUtility.Instance.IsTyping){
                textFlag = true;
                currState = State.four_wait;
            }
            break;
        case State.four_wait:
            if (Tile1.GetComponent<Tile>().OccupyingBuilding != null){
                currState = State.five_text;
            }
            break;
        case State.five_text:
            if (textFlag){
                tutorialNarration.text = "";
                StringUtility.TypeTextEffect(tutorialNarration, 
                "Right now,\\p it won’t point\\n"+
                "them in a new direction,\\p\\n"+
                "but we can fix that.\\p\\p\\n\\n"+
                "Click the sign to pick\\n"+
                "it up again.",
                TypeSpeed);
                textFlag = false;
            }
            if (!StringUtility.Instance.IsTyping){
                BuildManager.Instance.allowPickingUpBuildings = true;
                textFlag = true;
                currState = State.five_wait;
            }
            break;
        case State.five_wait:
            if (BuildManager.Instance.CurrBuilding != null){
                BuildManager.Instance.allowRotatingBuildings = true;
                currState = State.six_text;
            }
            break;
        case State.six_text:
            if (textFlag){
                tutorialNarration.text = "";
                StringUtility.TypeTextEffect(tutorialNarration, 
                "Rotate the sign with Q/E.\\n\\n\\p\\p"+
                "Turn it to point entirely\\n"+
                "towards the right and\\n"+
                "click to place it.",
                TypeSpeed);
                textFlag = false;
            }
            if (!StringUtility.Instance.IsTyping){
                textFlag = true;
                currState = State.six_wait;
            }
            break;
        case State.six_wait:
            if (Tile1.GetComponent<Tile>().OccupyingBuilding != null){
                if (Tile1.GetComponent<Tile>().OccupyingBuilding.transform.eulerAngles.y > 89 && Tile1.GetComponent<Tile>().OccupyingBuilding.transform.eulerAngles.y < 91){
                    currState = State.eight_text;
                } else {
                    currState = State.seven_text;
                }
            }
            break;
        case State.seven_text:
            if (textFlag){
                tutorialNarration.text = "";
                StringUtility.TypeTextEffect(tutorialNarration, 
                "That is not quite the\\n"+
                "right direction,\\p pick it\\n"+
                "up again and turn with\\n"+
                "Q/E so it points entirely\\n"+
                "towards the right.",
                TypeSpeed);
                textFlag = false;
            }
            if (!StringUtility.Instance.IsTyping){
                textFlag = true;
                currState = State.seven_wait;
            }
            break;
        case State.seven_wait:
            if (BuildManager.Instance.CurrBuilding != null){
                currState = State.six_wait;
                // tutorialNarration.text = "";
            }
            break;
        case State.eight_text:
            if (textFlag){
                tutorialNarration.text = "";
                StringUtility.TypeTextEffect(tutorialNarration, 
                "Great, now put down one\\n"+
                "more sign to redirect our\\n"+
                "associates to the exit.\\n\\n\\p\\p" +
                "Don't forget to click it\\n" +
                "to place it!",
                TypeSpeed);
                BuildManager.Instance.allowPickingUpBuildings = false;
                textFlag = false;
                BuildingPanel.SetActive(true);
                SignButton.interactable = false;
                Tile2.layer = 8;
                TileManager.Instance.AddUnoccupiedTile(Tile2.GetComponent<Tile>());
            }
            if (!StringUtility.Instance.IsTyping){
                SignButton.interactable = true;
                SignButton.transform.parent.GetComponent<Animator>().SetBool("highlighted", true);
                BuildManager.Instance.allowBuildingNewBuildings = true;
                BuildManager.Instance.allowRotatingBuildings = false;
                textFlag = true;
                currState = State.eight_wait;
            }
            break;
        case State.eight_wait:
            if (Tile2.GetComponent<Tile>().OccupyingBuilding != null){
                BuildManager.Instance.allowPickingUpBuildings = false;
                currState = State.nine_text;
                PlayButton.interactable = true;
                SignButton.transform.parent.GetComponent<Animator>().SetBool("highlighted", false);
                // PlayButton.GetComponent<Animator>().SetTrigger("Normal");
                }
            break;
        case State.nine_text:
            // PlayButton.GetComponent<Animator>().SetTrigger("Highlighted");
            if (textFlag){
                tutorialNarration.text = "";
                StringUtility.TypeTextEffect(tutorialNarration, 
                "This should work!\\p\\p\\n\\n"+
                "Let’s test it.",
                TypeSpeed);
                textFlag = false;
            }
            if (!StringUtility.Instance.IsTyping){
                PlayButton.transform.parent.GetComponent<Animator>().SetBool("highlightedPlay", true);
                textFlag = true;
                currState = State.nine_wait;
            }
            break;
        case State.nine_wait:
            if (GameStateManager.Instance.CurrState == GameStateManager.State.Play) PlayButton.transform.parent.GetComponent<Animator>().SetBool("highlightedPlay", false);
            BuildingPanel.SetActive(true);
            if (GoalTrigger.Instance.IsLevelDone()){
                currState = State.ten_text;
                ResetButton.interactable = true;
            }
            break;
        case State.ten_text:
            if (textFlag){
                tutorialNarration.text = "";
                StringUtility.TypeTextEffect(tutorialNarration, 
                "That works,\\p but there's\\n"+
                "a cheaper solution!\\p\\p\\n\\n"+
                "Every penny counts.\\p\\p\\p\\n\\n"+
                "Let's reset again.",
                TypeSpeed);
                textFlag = false;
            }
            if (!StringUtility.Instance.IsTyping){
                ResetButton.transform.parent.GetComponent<Animator>().SetBool("highlightedReset", true);
                textFlag = true;
                currState = State.ten_wait;
            }
            break;
        case State.ten_wait:
            if (GameStateManager.Instance.CurrState == GameStateManager.State.Plan){
                ResetButton.transform.parent.GetComponent<Animator>().SetBool("highlightedReset", false);
                currState = State.tenpointfive_text;
                ResetButton.interactable = false;
                PlayButton.interactable = false;
            }
            break;
        case State.tenpointfive_text:
            if (textFlag){
                optimalButton.enabled = true;
                tutorialNarration.text = "";
                StringUtility.TypeTextEffect(tutorialNarration, 
                "The expenditure meter\\n"+
                "on your left has a\\n"+
                "glowing segment, this\\n"+
                "displays the optimal cost\\n"+
                "of the solution. Click\\n"+
                "it to acknowledge.",
                TypeSpeed);
                textFlag = false;
            }
            if (!StringUtility.Instance.IsTyping){
                // ResetButton.transform.parent.GetComponent<Animator>().SetBool("highlightedReset", true);
                textFlag = true;
                currState = State.tenpointfive_wait;
            }
            break;
        case State.tenpointfive_wait:
            break;
        case State.eleven_text:
            if (textFlag){
                optimalButton.enabled = false;
                tutorialNarration.text = "";
                StringUtility.TypeTextEffect(tutorialNarration, 
                "Click to select a sign\\n"+
                "and then right click\\n"+
                "to remove it.\\p\\p\\n\\n"+
                "Remove all of the signs.",
                TypeSpeed);
                textFlag = false;
            }
            if (!StringUtility.Instance.IsTyping){
                BuildManager.Instance.allowPickingUpBuildings = true;
                BuildManager.Instance.allowDeletingBuildings = true;
                textFlag = true;
                currState = State.eleven_wait;
            }
            break;
        case State.eleven_wait:
            // int childcount = 0;
            // foreach (Transform building in buildManager.transform){
            //     childcount++;
            // }
            // if (childcount == 0){
            //     currState = State.twelve_text;
            // }
            if (Tile1.GetComponent<Tile>().OccupyingBuilding == null && Tile2.GetComponent<Tile>().OccupyingBuilding == null && BuildManager.Instance.CurrBuilding == null){
                currState = State.twelve_text;
            }
            break;
        case State.twelve_text:
            if (textFlag){
                tutorialNarration.text = "";
                StringUtility.TypeTextEffect(tutorialNarration, 
                "Nice work.\\p\\n\\n" +
                "Now put a sign down on the\\n"+
                "same tile, \\pbut have it\\n"+
                "point at the goal.\\p\\p\\n\\n" +
                "Remember you can rotate\\n" +
                "the sign with Q/E.",
                TypeSpeed);
                textFlag = false;
                BuildingPanel.SetActive(true);
                SignButton.interactable = false;
                Tile2.layer = 0;
                TileManager.Instance.RemoveTile(Tile2.GetComponent<Tile>());
                Tile2.GetComponent<Tile>().enabled = false;
                // MoneyManager.Instance.StartingMoney -= 200;
                MoneyManager.Instance.Items[0].quantity = 1;
                SignButton.transform.parent.GetComponent<Animator>().SetBool("highlighted", true);
                }
            if (!StringUtility.Instance.IsTyping){
                SignButton.interactable = true;
                BuildManager.Instance.allowBuildingNewBuildings = true;
                BuildManager.Instance.allowRotatingBuildings = true;
                textFlag = true;
                currState = State.twelve_wait;
            }
            break;
        case State.twelve_wait:
            if (BuildManager.Instance.CurrBuilding != null){
                BuildManager.Instance.CurrBuilding.transform.position = Tile1.transform.position + BuildManager.Instance.CurrBuilding.GetComponent<Building>().SpawnHeight * BuildManager.Instance.CurrBuilding.transform.up;
                Tile2.GetComponent<Tile>().SetBaseColor();
                Tile1.GetComponent<Tile>().SetHoverColor();
            }
            if (Tile1.GetComponent<Tile>().OccupyingBuilding != null){
                SignButton.transform.parent.GetComponent<Animator>().SetBool("highlighted", false);
                BuildManager.Instance.allowDeletingBuildings = false;
                if (Tile1.GetComponent<Tile>().OccupyingBuilding.transform.eulerAngles.y > 44 && Tile1.GetComponent<Tile>().OccupyingBuilding.transform.eulerAngles.y < 46){
                    currState = State.fourteen_text;
                } else {
                    currState = State.thirteen_text;
                }
            }
            break;
        case State.thirteen_text:
            if (textFlag){
                tutorialNarration.text = "";
                StringUtility.TypeTextEffect(tutorialNarration, 
                "That is not quite the\\n"+
                "right direction.\\p\\p Pick it\\n"+
                "up again, and turn it to\\n"+
                "point directly to the goal.",
                TypeSpeed);
                BuildManager.Instance.allowPickingUpBuildings = false;
                textFlag = false;
            }
            if (!StringUtility.Instance.IsTyping){
                textFlag = true;
                BuildManager.Instance.allowPickingUpBuildings = true;
                currState = State.thirteen_wait;
            }
            break;
        case State.thirteen_wait:
             if (BuildManager.Instance.CurrBuilding != null){
                currState = State.twelve_wait;
                // tutorialNarration.text = "";
            }
            break;
        case State.fourteen_text:
            if (textFlag){
                tutorialNarration.text = "";
                StringUtility.TypeTextEffect(tutorialNarration, 
                "That’s how it’s done.\\p\\p\\n\\n"+
                "Press play and let's see\\n"+
                "if this works!",
                TypeSpeed);
                BuildManager.Instance.allowPickingUpBuildings = false;
                BuildManager.Instance.allowBuildingNewBuildings = false;
                textFlag = false;
            }
            if (!StringUtility.Instance.IsTyping){
                PlayButton.transform.parent.GetComponent<Animator>().SetBool("highlightedPlay", true);
                currState = State.fourteen_wait;
                textFlag = true;
                PlayButton.interactable = true;
            }
            break;
        case State.fourteen_wait:
            if (GameStateManager.Instance.CurrState == GameStateManager.State.Play) PlayButton.transform.parent.GetComponent<Animator>().SetBool("highlightedPlay", false);
            BuildingPanel.SetActive(true);
            if (GoalTrigger.Instance.IsLevelDone()){
                currState = State.fifteen_text;
                StringUtility.Instance.SkipEnabled = false;
            }
            break;
        case State.fifteen_text: // camera rotation
             if (textFlag){
                tutorialNarration.text = "";
                StringUtility.TypeTextEffect(tutorialNarration, 
                "Great job!\\p\\n\\n" +
                "Before I send you off,\\p\\n"+
                "you should know you can\\n"+
                "control the camera.\\n\\n"+
                "\\p\\pTry rotating the\\n"+
                "camera around the scene\\n"+
                "with A or D.",
                TypeSpeed);
                textFlag = false;
            }
            if (!StringUtility.Instance.IsTyping){
                PlayButton.interactable = false;
                currState = State.fifteen_wait;
                mainCamera.GetComponent<CameraMovement>().allowRotation = true;
                textFlag = true;
            }
            break;
        case State.fifteen_wait:
            if (mainCamera.transform.rotation.eulerAngles.y < 40 || mainCamera.transform.rotation.eulerAngles.y > 46){
                currState = State.sixteen_text;
            }
            break;
        case State.sixteen_text: // camera zoom
            if (textFlag){
                tutorialNarration.text = "";
                StringUtility.TypeTextEffect(tutorialNarration, 
                "For a closer view,\\p\\n"+
                "try zooming the camera\\n"+
                "with the scroll wheel.",
                TypeSpeed);
                textFlag = false;
            }
            if (!StringUtility.Instance.IsTyping){
                currState = State.sixteen_wait;
                mainCamera.GetComponent<CameraMovement>().allowZoom = true;
                textFlag = true;
            }
            break;
        case State.sixteen_wait:
            if (mainCamera.GetComponent<Camera>().orthographicSize < 5 || mainCamera.GetComponent<Camera>().orthographicSize > 7){
                currState = State.seventeen_text;
            }
            break;
        case State.seventeen_text:
            if (textFlag){
                tutorialNarration.text = "";
                mainCamera.GetComponent<CameraMovement>().allowPan = true;
                StringUtility.TypeTextEffect(tutorialNarration, 
                "Lastly you can pan the \\n"+
                "camera by left clicking\\n"+
                "and dragging the mouse\\n" +
                "on the screen.\\p\\p\\n\\n",
                TypeSpeed);
                textFlag = false;
            }
            if (!StringUtility.Instance.IsTyping){
                textFlag = true;
                currState = State.seventeen_wait;
            }
            break;
        case State.seventeen_wait:
            if (mainCamera.transform.parent.position.x != 0 || mainCamera.transform.parent.position.x != 0){
                currState = State.eightteen_text;
            }
            break;
        case State.eightteen_text:
            if (textFlag){
                tutorialNarration.text = "";
                StringUtility.TypeTextEffect(tutorialNarration, 
                "You can always find the\\n"+
                "controls to the interface\\n"+
                "up here,\\p make sure you\\n"+
                "read them!",
                TypeSpeed);
                textFlag = false;
                MenuPanel.SetActive(true);
                ControlsButton.transform.parent.GetComponent<Animator>().SetBool("highlighted2", false);
                ControlsButton.transform.parent.GetComponent<Animator>().SetBool("highlighted", true);
            }
            if (!StringUtility.Instance.IsTyping){
                textFlag = true;
                currState = State.eightteen_wait;
            }
            break;
        case State.eightteen_wait:
            if (Controls.GetComponent<Animator>().GetBool("show")){
                NarrationPanel.SetActive(false); 
                eighteen_wait_flag = false;
            }
            if (!(Controls.GetComponent<Animator>().GetBool("show")) && !eighteen_wait_flag){
                ControlsButton.transform.parent.GetComponent<Animator>().SetBool("highlighted", false);
                currState = State.nineteen_text;
                NarrationPanel.SetActive(true);
                ControlsButton.interactable = false;
            }
            break;
        case State.nineteen_text:
            if (textFlag){
                tutorialNarration.text = "";
                StringUtility.TypeTextEffect(tutorialNarration, 
                "That's all.\\n\\n\\p\\p" +
                "Press the X button in the\\n"+
                "top right to get to work!",
                TypeSpeed);
                textFlag = false;            }
            if (!StringUtility.Instance.IsTyping){
                textFlag = true;
                currState = State.nineteen_wait;
            }
            break;
        case State.nineteen_wait:
            break;
        }
    }

    public void tenpointfive_wait_continue(){
        currState = State.eleven_text;
    }

}

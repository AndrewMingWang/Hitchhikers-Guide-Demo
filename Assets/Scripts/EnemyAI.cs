﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    public enum State{waiting, chargingAnimationStart, charging, chargingAnimationEnd, returning, collisionAnimation, stunned, reset_wait}

    public State currState = State.waiting;
    int playerLayerMask = 1 << 10; // Player layer
    int buildingLayerMask = 1 << 9; // Building layer
    int envBuildingLayerMask = 1 << 13; // Environment Buildings

    int tileLayerMask = 1 << 8;
    int layerMask;
    RaycastHit hit;
    RaycastHit hit2;

    [HideInInspector]
    public Collider Collider;
    [HideInInspector]
    public Animator Animator;

    
    private Vector3 target_pos;
    private Vector3 starting_parent_pos;
    private Quaternion starting_parent_rotation;
      private Vector3 starting_local_pos;
    private Quaternion starting_local_rotation;
    private int lerpFrameTotal; // Number of frames to completely interpolate between the 2 positions
    private int elapsedFrames = 0;
    private float lerpRatio;
    private Vector3 lerpPosition;
    private Vector3 lastTile;
    private Vector3 currTile;

    // Start is called before the first frame update
    void Start()
    {
        currState = State.waiting;
        layerMask = playerLayerMask | buildingLayerMask | envBuildingLayerMask;
        Collider = GetComponent<Collider>();
        Animator = transform.parent.GetComponent<Animator>();
        starting_parent_pos = transform.parent.position;
        starting_parent_rotation = transform.parent.rotation;
        starting_local_pos = transform.localPosition;
        starting_local_rotation = transform.localRotation;
        lerpRatio = 0;
        Physics.Raycast(Collider.bounds.center, -transform.up, out hit, Mathf.Infinity, tileLayerMask);
        lastTile = hit.collider.gameObject.transform.position;
        currTile = hit.collider.gameObject.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch(currState){
            case State.waiting:
                float xnoise = Random.Range(-0.2f, 0.2f);
                xnoise = 0.0f;
                float ynoise = Random.Range(0.1f, 0.7f);
                ynoise = 0.3f;
                // Debug.Log("xnoise");
                transform.parent.position = starting_parent_pos + EnemyManager.Instance.SceneObjects.transform.position;
                transform.parent.rotation = starting_parent_rotation;
                transform.localPosition = starting_local_pos; 
                transform.localRotation = starting_local_rotation;
                lerpPosition = starting_parent_pos;
                    
                Debug.DrawRay(transform.parent.position + new Vector3(xnoise, ynoise,0.0f), transform.forward*10.0f, Color.blue, 0.5f);
                if (Physics.Raycast(transform.parent.position + new Vector3(xnoise, ynoise,0.0f), transform.forward, out hit, Mathf.Infinity, layerMask)){
                    // Debug.Log("thats a dog");
                    if (hit.transform.CompareTag("player"))
                    {
                        // currState = State.chargingAnimationStart;
                        // Debug.Log("HIT PLAYER");
                        currState = State.chargingAnimationStart;
                        target_pos = hit.transform.position;
                        lerpFrameTotal = (int)(12*hit.distance);
                        Animator.SetBool("ChargeUp", true);
                    }
                }

                break;
            case State.chargingAnimationStart:
                if (Animator.GetCurrentAnimatorStateInfo(0).IsTag("ChargeUpFinished")){
                    Animator.SetBool("ChargeUp", false);
                    currState = State.charging;
                }
                break;
            case State.charging:
                lerpRatio = (float)elapsedFrames / lerpFrameTotal;
                lerpPosition = Vector3.Lerp(starting_parent_pos, target_pos, lerpRatio);
                transform.parent.position = lerpPosition;
                if (elapsedFrames != lerpFrameTotal){
                    elapsedFrames++;
                } else {
                    elapsedFrames = 0;
                    // currState = State.chargingAnimationEnd;
                    currState = State.chargingAnimationEnd;
                    lerpFrameTotal = (int)(25*hit.distance);
                    Animator.SetBool("CoolDown", true);   
                }
                break;
            case State.chargingAnimationEnd:
                if (Animator.GetCurrentAnimatorStateInfo(0).IsTag("CoolDownFinished")){
                    Animator.SetBool("CoolDown", false);
                    currState = State.returning;
                }
                break;
            case State.returning:
                lerpRatio = (float)elapsedFrames / lerpFrameTotal;
                lerpPosition = Vector3.Lerp(target_pos, starting_parent_pos, lerpRatio);
                transform.parent.position = lerpPosition;
                if (elapsedFrames != lerpFrameTotal){
                    elapsedFrames++;
                } else {
                    elapsedFrames = 0;
                    // currState = State.chargingAnimationEnd;
                    currState = State.waiting;
                }
                break;
            case State.collisionAnimation:
                lerpRatio = (float)elapsedFrames / lerpFrameTotal;
                lerpPosition = Vector3.Lerp(transform.parent.position, lastTile, lerpRatio);
                transform.parent.position = lerpPosition;
                if (elapsedFrames != lerpFrameTotal){
                    elapsedFrames++;
                } else {
                    elapsedFrames = 0;
                    // currState = State.chargingAnimationEnd;
                    currState = State.stunned;
                }
                break;
            case State.stunned:
                // Debug.Log("Stunned");
                break;
            case State.reset_wait:
                if (Animator.GetBool("PreWaitingState")){
                    currState = State.waiting;
                    Animator.SetBool("PreWaitingState", false);
                }
                break;
        }

        if (Physics.Raycast(Collider.bounds.center, -transform.up, out hit2, Mathf.Infinity, tileLayerMask) && currState != State.collisionAnimation){
            if (hit2.collider.gameObject.transform.position != currTile){
                lastTile = currTile;
                currTile = hit2.collider.gameObject.transform.position;
            }
            if (currState == State.waiting){
                currTile = hit2.collider.gameObject.transform.position;
                lastTile = currTile;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "player"){
            // Debug.Log("yea");
            other.gameObject.GetComponent<Dog>().LosePlayer();
        }
        if (other.gameObject.tag == "enemy"){
            Debug.Log(other.gameObject.name);
            currState = State.collisionAnimation;
            Animator.SetBool("EnemyCollision", true);
        }
    }

    public void resetState(){
        Animator.SetTrigger("LevelReset");
        Animator.SetBool("ChargeUp", false);
        Animator.SetBool("CoolDown", false);
        Animator.SetBool("EnemyCollision", false);
        elapsedFrames = 0;
        
        currState = State.waiting;
    }
}

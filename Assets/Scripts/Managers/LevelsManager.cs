using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;
using System;
using UnityEngine.UI;

public class LevelsManager : MonoBehaviour
{
    public enum Mode
    {
        Stop,
        Change,
        Split,
        Unsplit
    }

    public Mode mode;
    private bool changeAfterUnsplit;
    public GameObject activeLevel;
    private GameObject activeMask;
    private GameObject targetLevel;
    private GameObject targetMask;

    public GameObject blackout;
    public float changeTime;
    public float splitTime;

    private float currentTime;
    private float targetTime;

    public float splitPosition;
    public float splitScale;

    private float startPlayerPosition;

    public void SetFade(float value)
    {
        var image = blackout.GetComponent<UnityEngine.UI.Image>();
        var color = image.color;
        color.a = value;
        image.color = color;
    }

    public void ChangeLevel(GameObject level)
    {
        if (mode != Mode.Stop) {
            throw new Exception("Can't change while in change!");
        }
        mode = Mode.Change;
        targetTime = changeTime;
        currentTime = 0.0f;

        targetLevel = level;
        targetMask = targetLevel.transform.Find("Sprite Mask").gameObject;

        targetLevel.SetActive(false);
    }

    public void SplitLevel(GameObject level)
    {
        if (mode != Mode.Stop) {
            throw new Exception("Can't change while in change!");
        }
        mode = Mode.Split;
        targetTime = splitTime;
        currentTime = 0.0f;
        startPlayerPosition = transform.position.x;

        targetLevel = level;
        targetMask = targetLevel.transform.Find("Sprite Mask").gameObject;
        targetLevel.transform.position = new Vector3(19.0f, targetLevel.transform.position.y, targetLevel.transform.position.z);
        targetMask.transform.localScale = new Vector3(19.0f, targetMask.transform.localScale.y, targetMask.transform.localScale.z);
        
        targetLevel.SetActive(true);
    }

    public void UnsplitLevel(bool changeAfterUnsplit)
    {
        if (mode != Mode.Stop) {
            throw new Exception("Can't change while in change!");
        }
        mode = Mode.Unsplit;
        targetTime = splitTime;
        currentTime = 0.0f;
        this.changeAfterUnsplit = changeAfterUnsplit;
    }

    void Start()
    {
        activeMask = activeLevel.transform.Find("Sprite Mask").gameObject;
    }

    void Update()
    {
        if (mode != Mode.Stop){
            currentTime += Time.deltaTime;
            currentTime = Mathf.Clamp(currentTime, 0, targetTime);
        }

        if (mode == Mode.Split) {
            activeLevel.transform.position = new Vector3(-currentTime / targetTime * splitPosition, activeLevel.transform.position.y, activeLevel.transform.position.z);
            activeMask.transform.localScale = new Vector3(currentTime / targetTime * (splitScale - 19.0f) + 19.0f, activeMask.transform.localScale.y, activeMask.transform.localScale.z);

            targetLevel.transform.position = new Vector3(currentTime / targetTime * (splitPosition - 19.0f) + 19.0f, activeLevel.transform.position.y, activeLevel.transform.position.z);
            targetMask.transform.localScale = new Vector3(currentTime / targetTime * (splitScale - 19.0f) + 19.0f, targetMask.transform.localScale.y, targetMask.transform.localScale.z);

            transform.position = new Vector3(-currentTime / targetTime * splitPosition + startPlayerPosition, transform.position.y, transform.position.z);

            if (currentTime == targetTime) {
                mode = Mode.Stop;
            }
        }
        if (mode == Mode.Unsplit) {
            activeLevel.transform.position = new Vector3(-(1.0f - currentTime / targetTime) * splitPosition, activeLevel.transform.position.y, activeLevel.transform.position.z);
            activeMask.transform.localScale = new Vector3(currentTime / targetTime * (19.0f - splitScale) + splitScale, activeMask.transform.localScale.y, activeMask.transform.localScale.z);

            targetLevel.transform.position = new Vector3(currentTime / targetTime * (19.0f - splitPosition) + splitPosition, activeLevel.transform.position.y, activeLevel.transform.position.z);
            targetMask.transform.localScale = new Vector3(currentTime / targetTime * (19.0f - splitScale) + splitScale, targetMask.transform.localScale.y, targetMask.transform.localScale.z);

            transform.position = new Vector3(-(1.0f - currentTime / targetTime) * splitPosition + startPlayerPosition, transform.position.y, transform.position.z);

            if (currentTime == targetTime) {
                targetLevel.SetActive(false);
                mode = Mode.Stop;
                if (changeAfterUnsplit) {
                    targetLevel.transform.position = new Vector3(0.0f, targetLevel.transform.position.y, targetLevel.transform.position.z);
                    ChangeLevel(targetLevel);
                }
                
            }
        }
        if (mode == Mode.Change) {
            if (currentTime < targetTime / 2.0f){
                SetFade(currentTime / (targetTime / 2.0f));
            } else {
                SetFade((targetTime - currentTime) / (targetTime / 2.0f));
            }
            if ((currentTime >= targetTime / 2.0f) && activeLevel.activeSelf) {
                activeLevel.SetActive(false);
                targetLevel.SetActive(true);
            }
            if (currentTime == targetTime) {
                activeLevel = targetLevel;
                activeMask = targetMask;
                targetLevel = null;
                targetMask = null;
                mode = Mode.Stop;
            }
        }
    }
}

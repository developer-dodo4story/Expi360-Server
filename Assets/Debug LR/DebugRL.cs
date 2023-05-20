using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugRL : MonoBehaviour
{
    public enum CameraRL { LeftCameras, RightCameras}

    public CameraRL cameraRL;
    [Range(0,5)]
    public int cameraNumber;

    public Camera myCamera;

    private GameObject gameObjectRL;
    private bool isActive = false;
    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        gameObjectRL = transform.GetChild(0).gameObject;
        SetActive();
    }

    private void FindCamera()
    {
        var name = cameraRL.ToString();
        myCamera = GameObject.Find(name).gameObject.transform.GetChild(cameraNumber).GetComponent<Camera>();
        canvas.worldCamera = myCamera;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            SetActive();
        }
    }

    private void SetActive()
    {
        if (gameObjectRL != null)
        {
            FindCamera();
            gameObjectRL.SetActive(isActive);
            isActive = !isActive;
        }
    }
}

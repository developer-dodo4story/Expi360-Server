using GameRoomServer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WindowedGame : MonoBehaviour
{
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
    public string windowGameName;
    public bool applyQualityLevel = false;
    public QualityLevel qualityLevel;

    private static WindowedGame Instance;
    private static GameConfig config;

    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    private static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
    [DllImport("user32.dll", EntryPoint = "FindWindow")]
    public static extern IntPtr FindWindow(System.String className, System.String windowName);

    public static void SetPosition(int x, int y, int resX = 0, int resY = 0)
    {
        var windowTitle = Instance.windowGameName;
        SetWindowPos(FindWindow(null, windowTitle), 0, x, y, resX, resY, resX * resY == 0 ? 1 : 0);
    }

    [DllImport("user32.dll", EntryPoint = "SetWindowText")]
    public static extern bool SetWindowText(System.IntPtr hwnd, System.String lpString);

    public static void SetWindowName(string name)
    {
        var windowTitle = Instance.windowGameName;
        //Get the window handle.
        var windowPtr = FindWindow(null, windowTitle);
        //Set the title text using the window handle.
        Instance.windowGameName = name;
        SetWindowText(windowPtr, Instance.windowGameName);
    }

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

        DontDestroyOnLoad(gameObject);
        SetupWindowSize(0f);
    }

    private void Start()
    {
        SetCurrentWindowSize();
        SetQualityLevel((int)qualityLevel);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += RefreshSetupWhenSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= RefreshSetupWhenSceneLoaded;
    }

    private void RefreshSetupWhenSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        SetCurrentWindowSize();
    }

    /// <summary>
    /// Ustawia obecna wielkosc okna w tytule okna
    /// </summary>
    public void SetCurrentWindowSize()
    {
        var h = Screen.height;
        var w = Screen.width;
        var newName = " current width: " + w + " height: " + h;
        SetWindowName(newName);

        if (config != null && (h != config.height || w != config.width))
        {
            SetupWindowSize(0f);
        }
    }

    private void SetupWindowSize(float time)
    {
        config = new GameConfig();
        Screen.fullScreenMode = FullScreenMode.Windowed;
        if (SerializeHelper.LoadFromJSON("gameConfig", SerializeHelper.pathToProject, ref config))
        {
            StartCoroutine(SetUpWindow(config, time));
        }
        else
        {
            config.SetParams(11520, 1080, 0, 0);
            StartCoroutine(SetUpWindow(config, time));
            SerializeHelper.SaveToJSON("gameConfig", SerializeHelper.pathToProject, config);
        }
    }

    private IEnumerator SetUpWindow(GameConfig config, float time = 0.5f)
    {
        yield return new WaitForSeconds(time);
        SetPosition(config.screenX + config.screenXOffset, config.screenY + config.screenYOffset, config.width + config.widthOffset, config.height + config.heightOffset);
    }


    public enum QualityLevel { VeryLow, Low, Medium, High, VeryHight, Ultra }
    /// <summary>
    // 0 - Very low
    // 1 - Low
    // 2 - Medium
    // 3 - High
    // 4 - Very High
    // 5 - Ultra
    /// </summary>
    private void SetQualityLevel(int level)
    {
        if (applyQualityLevel)
        {
            GameConfig config = null;
            if (SerializeHelper.LoadFromJSON("gameConfig", SerializeHelper.pathToProject, ref config))
            {
                if (config.quality != -1)
                {
                    level = config.quality;
                }
            }
            level = Mathf.Clamp(level, 0, 5);
            QualitySettings.SetQualityLevel(level, true);
        }
    }

#endif
}

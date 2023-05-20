using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedDodoServer;
using System;
using System.IO;

/// <summary>
/// Class that decides which type of server input should be used. Creates and reads a config.txt file with input information
/// </summary>
public class ServerInput : Singleton<ServerInput>
{
    Consts.InputType inputType;
    Consts.Server server;
    public GameObject gamePadServer, xboxPadServer;
    string path;
    string fileName;
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        //editor - one level above Assets
        //build - same folder where .exe resides
        path = Application.dataPath;
        //path = Directory.GetParent(Application.dataPath).ToString();        
        fileName = "config.txt";
        
        ReadConfig();        

        if (inputType == Consts.InputType.PhonePad)
        {
            gamePadServer.SetActive(true);
            xboxPadServer.SetActive(false);
            gamePadServer.GetComponent<Server>().server = server;
        }
        else if (inputType == Consts.InputType.XboxPad)
        {
            gamePadServer.SetActive(false);
            xboxPadServer.SetActive(true);
            xboxPadServer.GetComponent<Server>().server = server;
        }                
    }
    //private void SaveConfig(Consts.InputType _inputType)
    //{
    //    Config config = new Config();
    //    config.inputType = _inputType;
    //    string json = JsonUtility.ToJson(config);        
    //    StreamWriter streamWriter = new StreamWriter(Path.Combine(path, fileName));        
    //    streamWriter.WriteLine(json);
    //    streamWriter.Flush();
    //    streamWriter.Close();
    //}
    private void ReadConfig()
    {
        string fullPath = Path.Combine(path, fileName);
        Config config;
        if (!File.Exists(fullPath))
        {
            var stream = File.Create(fullPath);
            config = new Config();            
            string json = JsonUtility.ToJson(config);
            json += "\n//inputType: 0-PhonePad, 1-XboxPad, server: 0-Room1, 1-Room2, ...";
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
            stream.Write(bytes, 0, bytes.Length);
            stream.Close();
        }
        else
        {
            StreamReader streamReader = new StreamReader(fullPath);
            string json = streamReader.ReadLine();
            streamReader.Close();
            config = JsonUtility.FromJson<Config>(json);
        }
        inputType = config.inputType;
        server = config.server;
    }
}

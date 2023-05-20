using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedDodoServer;

public class XboxPadServer : Server
{
    int padCount = 0;
    List<PlayerInput> playerInputs = new List<PlayerInput>();
    int firstJoystickKeyCode = (int)KeyCode.Joystick1Button0;
    int numOfJoystickButtons = 20;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        //InitializeUDP();
        Server.instance.currentInputType = Consts.InputType.XboxPad;
        padCount = Input.GetJoystickNames().Length;        
        for (int i = 0; i < padCount; i++)
        {
            AddNewClient();
        }
    }

    private void HandleNewClients()
    {
        int newPadCount = Input.GetJoystickNames().Length;
        if (newPadCount > padCount)
        {
            padCount = newPadCount;
            AddNewClient();
        }
    }
    private void HandleInput(int i)
    {        
        float x = Input.GetAxis("Horizontal" + (i + 1).ToString());
        float y = -Input.GetAxis("Vertical" + (i + 1).ToString()); // it's inverted
        bool AButton = Input.GetKey((KeyCode)(firstJoystickKeyCode + i * numOfJoystickButtons));
        bool BButton = Input.GetKey((KeyCode)(firstJoystickKeyCode + i * numOfJoystickButtons + 1));
        bool XButton = Input.GetKey((KeyCode)(firstJoystickKeyCode + i * numOfJoystickButtons + 2));
        bool YButton = Input.GetKey((KeyCode)(firstJoystickKeyCode + i * numOfJoystickButtons + 3));
        bool startButton = Input.GetKey((KeyCode)(firstJoystickKeyCode + i * numOfJoystickButtons + 7));

        PlayerInput playerInput = connectedControllers[i].playerInput;
        playerInput.leftStick.Horizontal = x;
        playerInput.leftStick.Vertical = y;
        playerInput.aButton = AButton;
        playerInput.bButton = BButton;
        playerInput.xButton = XButton;
        playerInput.yButton = YButton;
        playerInput.start = startButton;

        SetPlayerInput(i, playerInput);       
    }
    private void HandleInput()
    {
        for (int i = 0; i < playerInputs.Count; i++)
        {
            HandleInput(i);
        }
    }
    protected override void Update()
    {
        base.Update();
        HandleNewClients();
        HandleInput();
    }
    void AddNewClient()
    {
        ConnectedController cc = AddController();
        playerInputs.Add(cc.playerInput);
    }
    public void InitializeUDP()
    {
        base.InitUDP();
    }
}

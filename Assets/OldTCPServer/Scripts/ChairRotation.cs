using GameRoomServer;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ChairRotation : MonoBehaviour {
    
    [SerializeField]
    private float accelerationSpeed = 10f;

    [SerializeField]
    [Range(1, 2000f)]
    private float maxRotationSpeed = 1100f;

    [SerializeField]
    [Range(1, 1000f)]
    private float minRotationSpeed = 50f;

    private List<RotatingChair> chairs = new List<RotatingChair>();
    private List<TCPConnectedChair> existingChairs = new List<TCPConnectedChair>();

    private void Start()
    {
        DontDestroyOnLoad(this);
    }
    public class RotatingChair
    {
        public TCPConnectedChair chair;
        public float speed;
        public RotatingChair(TCPConnectedChair chair, float speed)
        {
            this.chair = chair;
            this.speed = speed;
        }
    }
    void Update ()
    {
        SeekNewChairs();
        Rotate();
	}
    private void OnApplicationQuit()
    {
        foreach (RotatingChair rotatingChair in chairs)
        {
            rotatingChair.chair.SetRotationSpeed(0);
        }
    }
    void SeekNewChairs()
    {
        if(TCPServer.instance.clientList.Count > existingChairs.Count)
        {
            for(int i = existingChairs.Count; i<TCPServer.instance.clientList.Count; i++)
            {
                if(existingChairs.Contains(TCPServer.instance.clientList[i]))
                {
                    continue;
                }
                chairs.Add(new RotatingChair(TCPServer.instance.clientList[i], 0));
                existingChairs.Add(TCPServer.instance.clientList[i]);
            }
        }
    }
    void Rotate()
    {
        if (chairs.Count == 0 )
        {
            return;
        }
        foreach (RotatingChair rotatingChair in chairs)
        {
            int rotationFactor = GetRotationFactor(rotatingChair.chair);

            
            if (rotationFactor == rotatingChair.speed)
            {
                continue;
            }
            float sign = Mathf.Sign(rotationFactor - rotatingChair.speed);
            
            if (sign > 0)
            {
                rotatingChair.speed = rotatingChair.speed + accelerationSpeed * Time.deltaTime * 60;
                if (rotatingChair.speed > rotationFactor)
                {
                    rotatingChair.speed = rotationFactor;
                }
                rotatingChair.chair.SetRotationSpeed((int)rotatingChair.speed);
            }
            else
            {
                rotatingChair.speed = rotatingChair.speed - accelerationSpeed * Time.deltaTime * 60;
                if (rotatingChair.speed < rotationFactor)
                {
                    rotatingChair.speed = rotationFactor;
                }
                rotatingChair.chair.SetRotationSpeed((int)rotatingChair.speed);
            }
        }
    }
    private int GetRotationFactor(TCPConnectedChair chair)
    {
        float leftTrigger = chair.chairInput.leftTrigger;
        float rightTrigger = chair.chairInput.rightTrigger;
        int rotationFactor = (int)((leftTrigger - rightTrigger) * maxRotationSpeed);
        if (Mathf.Abs(rotationFactor) < 50)
        {
            rotationFactor = 0;
        }
        return rotationFactor;
    }
}

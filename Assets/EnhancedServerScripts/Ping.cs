using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ping 
{
    public Ping()
    {        
        pings = new List<float>();
        avg = 0f;
        max = 0f;
        min = float.MaxValue;
        prevTime = 0.0f;
    }
    public Ping(int id) : this()
    {
        this.id = id;
    }
    private int maxPings = 10;
    private List<float> pings;
    private float prevTime;
    public float avg, min, max, current;
    public int id;
    public void AddTime(float time)
    {
        //Debug.Log(time);
        if (prevTime == 0.0f)
        {
            prevTime = time;
            return;
        }
        else
        {
            float ping = time - prevTime;
            prevTime = time;
            AddPing(ping);
        }
    }
    public void AddPing(float ping)
    {
        pings.Add(ping);
        if (pings.Count > maxPings)
        {
            pings.RemoveAt(0);
        }
        current = ping;        
        avg = pings.Sum(p => p) / pings.Count;
        if (avg > max) max = avg;
        if (avg < min) min = avg;
               
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class GameConfig
{
    public int width;
    public int height;
    public int screenX = 0;
    public int screenY = 0;
    public int widthOffset = 16;
    public int heightOffset = 39;
    public int screenXOffset = -8;
    public int screenYOffset = -31;
    public float heightStretch = 1f;
    public float widthStretch = 2f;
    public int quality = -1;

    public void SetParams(int width, int height, int screenX, int screenY)
    {
        this.width = width;
        this.height = height;
        this.screenX = screenX;
        this.screenY = screenY;
    }
}

﻿using System;

[Serializable]
public class MyRect
{
    public int x, y, w, h;
}


[Serializable]
public class DroneVehicleDataNew
{
    public string client_id;
    public MyRect[] rects;
}
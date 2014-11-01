using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class TowerPassEvent:EventBase
{
    public const string TOWER_PASS = "TOWER_PASS";
    public int mfloor;
    public TowerPassEvent(string eventName) : base(eventName)
    {
    }
}


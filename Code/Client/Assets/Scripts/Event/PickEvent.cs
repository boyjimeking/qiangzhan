using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// Ë©È¡Ò»¸ö±¦Ïä(pick)
public class FindPickEvent : EventBase
{
    public static string FIND_PICK_BOX = "FIND_PICK_BOX";

    public int OwnerId = -1;
    public int PickResId = -1;
    public Vector3 Position;
    
    public FindPickEvent()
        : base(FIND_PICK_BOX)
    {
    }
}
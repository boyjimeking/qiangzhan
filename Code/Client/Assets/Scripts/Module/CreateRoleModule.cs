using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class CreateRoleModule : ModuleBase
{
    private uint Roleid = 0; //1男角，2女角，0未选择；
    public CreateRoleModule()
    {

    }
    public void SetRoleID(uint id)
    {
        Roleid = id;

        CreateRoleManager.Instance.ShowPlayer((int)Roleid);
    }
    public uint GetRoleID()
    {
        return Roleid;
    }
}
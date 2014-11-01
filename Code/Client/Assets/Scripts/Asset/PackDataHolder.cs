using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// 打包资源
/// </summary>
class PackDataHolder : ScriptableObject
{
    public List<string> namelist = new List<string>();
    public List<Object> objectlist = new List<Object>();

    public void Clear()
    {
        namelist.Clear();
        objectlist.Clear();
    }
    public Object GetObjectByName(string name)
    {
       int nIndex = namelist.IndexOf(name);
       if (nIndex < 0)
           return null;
       return objectlist[nIndex];
    }
    public void SetObject(string name,Object obj)
    {
        if (namelist.IndexOf(name) >= 0)
            return;
        namelist.Add(name);
        objectlist.Add(obj);
    }
}

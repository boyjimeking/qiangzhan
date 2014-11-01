using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class FilterTree
{
    private Dictionary<char, FilterTree> Childs = new Dictionary<char, FilterTree>();

    public void AddString(string str , int idx = 0)
    {
        char c = '|';
        if (idx >= str.Length)
        {
            if( !Childs.ContainsKey( c ) )
            {
                Childs.Add(c, null);
            }
            return;
        }
        c = str[idx];
        FilterTree child = null;
        if( !Childs.ContainsKey( c ) )
        {
            child = new FilterTree();
            Childs.Add(c, child);
        }else
        {
            child = Childs[c];
        }
        child.AddString(str, ++idx);
    }

    public bool HasChar(char c)
    {
        return Childs.ContainsKey(c);
    }

    public int ChildSize()
    {
        return Childs.Count;
    }

    public bool HasEnd()
    {
        char c = '|';
        return Childs.ContainsKey(c);
    }

    public int CheckBlack(string str , int idx = 0)
    {
        if (idx >= str.Length)
            return -1;
        int blackIdx = -1;
        int temp = -1;
        char c = str[idx];
        if ( Childs.ContainsKey(c) )
        {
            if( Childs[c].HasEnd() )
            {
                blackIdx = idx;
            }
            temp = Childs[c].CheckBlack( str , ++idx);

            if( temp >= 0 )
            {
                blackIdx = temp;
            }
        }
        return blackIdx;
    }
}
class StrFilterManager
{
    private static StrFilterManager instance = null;

    private FilterTree mFilterTree = null;

    public StrFilterManager()
    {
        instance = this;
    }

    public static StrFilterManager Instance
    {
        get
        {
            return instance;
        }
    }
    public bool Init(DataTable table)
    {
        if (mFilterTree == null)
            mFilterTree = new FilterTree();
        IDictionaryEnumerator itr = table.GetEnumerator();
        while (itr.MoveNext())
        {
            StrFilterTableItem item = itr.Value as StrFilterTableItem;
            mFilterTree.AddString(item.str);
        }

//         foreach( DictionaryEntry v in table )
//         {
//             StrFilterTableItem item = v.Value as StrFilterTableItem;
//             mFilterTree.AddString(item.str);
//         }
        return true;
    }

    public bool CheckBlacklist(string chkStr)
    {
        if (mFilterTree == null)
            return false;
        int len = chkStr.Length;

        int lastIdx = 0;
        while (lastIdx < len)
        {
            if ( mFilterTree.CheckBlack(chkStr, lastIdx) >= 0 )
            {
                return true;
            }
            else
            {
                lastIdx++;
            }
        }
        return false;
    }

    public string CheckAndReplace(string chkStr)
    {
        string rst = chkStr;
        if (mFilterTree == null)
            return rst;
        int len = chkStr.Length;

        int idx = -1;
        int lastIdx = 0;
        while (lastIdx < len)
        {
            idx = mFilterTree.CheckBlack(chkStr, lastIdx);
            if( idx >= 0 )
            {
                int num = idx + 1 - lastIdx;
                rst = rst.Remove(lastIdx, num);
                rst = rst.Insert(lastIdx, new string('*', num));

                lastIdx = idx + 1;
            }else
            {
                lastIdx++;
            }
        }
        return rst;
    }
}


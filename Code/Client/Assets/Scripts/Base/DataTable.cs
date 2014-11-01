using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public enum KEY_TYPE:int
{
    KEY_TYPE_ERROR = -1,
    KEY_TYPE_INT = 0,
    KEY_TYPE_STRING = 1,
}

//封装DataManager 使用的 Table
public class DataTable
{
    static Type intType = typeof(int);
    static Type uintType = typeof(uint);
    static Type stringType = typeof(string);
    static Type shortType = typeof(short);
    static Type longType = typeof(long);

    private Dictionary<Int64, object> IntDic = null;
    private Dictionary<string, object> StringDic = null;

    private KEY_TYPE mKeyType = KEY_TYPE.KEY_TYPE_ERROR;
    public DataTable()
    {

    }

    public int Count
    {
        get
        {
            if (mKeyType == KEY_TYPE.KEY_TYPE_INT)
            {
                return IntDic.Count;
            }else
            {
                return StringDic.Count;
            }
        }
    }

    public bool Add(object key , object obj)
    {
        if (mKeyType == KEY_TYPE.KEY_TYPE_ERROR)
        {
            Type type = key.GetType();
            if( type == intType ||
                type == longType ||
                type == uintType ||
                type == shortType )
            {
                mKeyType = KEY_TYPE.KEY_TYPE_INT;
                IntDic = new Dictionary<long, object>();
            }else if( type == stringType )
            {
                mKeyType = KEY_TYPE.KEY_TYPE_STRING;
                StringDic = new Dictionary<string, object>();
            }else
            {
                GameDebug.LogError("DataTable 不支持的Key类型");
                return false;
            }
        }
        if( mKeyType == KEY_TYPE.KEY_TYPE_INT )
        {
            return Add(Convert.ToInt64(key), obj);
        }else if( mKeyType == KEY_TYPE.KEY_TYPE_STRING)
        {
            return Add((string)key, obj);
        }
        return false;
    }

    private bool Add(Int64 key , object obj)
    {
        if (IntDic.ContainsKey(key))
            return false;
        IntDic.Add(key, obj);
        return true;
    }
    private bool Add(string key, object obj)
    {
        if (StringDic.ContainsKey(key))
            return false;
        StringDic.Add(key, obj);
        return true;
    }

    public bool ContainsKey(object key)
    {
        if (mKeyType == KEY_TYPE.KEY_TYPE_INT)
        {
            return ContainsKey(Convert.ToInt64(key));
        }else if(mKeyType == KEY_TYPE.KEY_TYPE_STRING)
        {
            return ContainsKey((string)key);
        }

        return ContainsKey((int)key);
    }

    private bool ContainsKey(Int64 key)
    {
        return IntDic.ContainsKey(key);  
    }

    private bool ContainsKey(string key)
    {
        return StringDic.ContainsKey(key);
    }

    public IDictionaryEnumerator GetEnumerator()
    {
        if (mKeyType == KEY_TYPE.KEY_TYPE_INT)
        {
            return IntDic.GetEnumerator();
        }
        if (mKeyType == KEY_TYPE.KEY_TYPE_STRING)
        {
            return StringDic.GetEnumerator();
        }
        return null;
    }

    public ArrayList GetKeys()
    {
        ArrayList list = new ArrayList();
        if (mKeyType == KEY_TYPE.KEY_TYPE_INT)
        {
            list.AddRange(IntDic.Keys);
        }
        if (mKeyType == KEY_TYPE.KEY_TYPE_STRING)
        {
            list.AddRange(StringDic.Keys);
        }
        return list;
    }

    public ArrayList GetValues()
    {
        ArrayList list = new ArrayList();
        if (mKeyType == KEY_TYPE.KEY_TYPE_INT)
        {
            list.AddRange(IntDic.Values);
        }
        if (mKeyType == KEY_TYPE.KEY_TYPE_STRING)
        {
            list.AddRange(StringDic.Values);
        }
        return list;
    }

    //DataTable 不允许写入
    public object this[object key]
    {
        get
        {
            if (mKeyType == KEY_TYPE.KEY_TYPE_INT)
            {
                Int64 intkey = Convert.ToInt64(key);
                if (!ContainsKey( intkey ))
                {
                    return null;
                }
                return IntDic[intkey];
            }
            if (mKeyType == KEY_TYPE.KEY_TYPE_STRING)
            {
                string strkey = (string)key;
                if (!ContainsKey(strkey))
                {
                    return null;
                }
                return StringDic[strkey];
            }
            return null;
        }
    }
}
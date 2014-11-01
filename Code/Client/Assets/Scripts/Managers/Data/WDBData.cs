using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable]
public class WDBData
{
    public WDBSheetLine[] m_Data;
	public List<TextAnalyze.TYPE> m_Type;
    [NonSerialized]
    private Dictionary<int, int> m_Index;

    public static T createInstance<T>()
    {
        if (typeof(T) == typeof(string))
        {
			return (T)Convert.ChangeType(string.Empty,typeof(T));
//            return (T)string.Empty;
        }
        return Activator.CreateInstance<T>();
    }

    public WDBSheetLine GetData(int index)
    {
        if (this.m_Index != null)
        {
            int num = 0;
            if (this.m_Index.TryGetValue(index, out num))
            {
                return this.GetDataByNumber(num);
            }
        }
        else
        {
            return this.GetDataByNumber(index);
        }
        return null;
    }

    public object GetData(int index, int field)
    {
        WDBSheetLine data = this.GetData(index);
        if (data != null)
        {
            return data.GetData(field);
        }
        return null;
    }

    public T GetData<T>(int index, int field)
    {
        object data = this.GetData(index, field);
        if (data != null)
        {
            return (T) data;
        }
        Debug.LogWarning(string.Concat(new object[] { "index:", index, "field:", field, " is null" }));
        return createInstance<T>();
    }

//    public T GetData<T>(int index, string fieldName)
//    {
//        int field = 0;
//        if (this.GetFieldByName(fieldName, out field))
//        {
//            return this.GetData<T>(index, field);
//        }
//        Debug.LogWarning(string.Concat(new object[] { "index:", index, "fieldName:", fieldName, " is null" }));
//        return Activator.CreateInstance<T>();
//    }

//    public object GetData(int index, string fieldName)
//    {
//        int field = 0;
//        if (this.GetFieldByName(fieldName, out field))
//        {
//            return this.GetData(index, field);
//        }
//        return null;
//    }
	
	/// <summary>
	/// 获取每一行的数据.
	/// </summary>
	/// <returns>
	/// The data by number.
	/// </returns>
	/// <param name='row'>
	/// Row.
	/// </param>
    public WDBSheetLine GetDataByNumber(int row)
    {
        if ((row >= 0) && (row < this.GetRecordCount()))
        {
            return this.m_Data[row + 1];
        }
        return null;
    }

    public object GetDataByNumber(int row, int field)
    {
        WDBSheetLine dataByNumber = this.GetDataByNumber(row);
        if (dataByNumber != null)
        {
            return dataByNumber.GetData(field);
        }
        return null;
    }

    public T GetDataByNumber<T>(int row, int field)
    {
        WDBSheetLine dataByNumber = this.GetDataByNumber(row);
        if (dataByNumber != null)
        {
            return dataByNumber.GetData<T>(field);
        }
        return createInstance<T>();
    }

//    public bool GetFieldByName(string fieldName, out int field)
//    {
//        return this.m_FieldName.TryGetValue(fieldName, out field);
//    }

    public int GetFieldCount()
    {
		return this.m_Type.Count;
    }

    public int GetFieldType(int field)
    {
        return (int) this.m_Data[0].GetData(field);
    }

//    public string GetFiledName(int field)
//    {
//        foreach (KeyValuePair<string, int> pair in this.m_FieldName)
//        {
//            if (pair.Value == field)
//            {
//                return pair.Key;
//            }
//        }
//        return null;
//    }

    public int GetRecordCount()
    {
        return (this.m_Data.Length - 1);
    }

    public bool HasIndex(int index)
    {
        return this.m_Index.ContainsKey(index);
    }

    public void Init()
    {
        if (this.m_Index == null)
        {
            int field = -1;
//            for (int i = 0; i < this.GetFieldCount(); i++)
//            {
//                if (this.GetFieldType(i) == 118)
//                {
//                    field = i;
//                }
//            }
			field = 0;
            if (field >= 0)
            {
                this.m_Index = new Dictionary<int, int>();
                for (int j = 0; j < this.GetRecordCount(); j++)
                {
                    int dataByNumber = (int) this.GetDataByNumber(j, field);
					if(!m_Index.ContainsKey(dataByNumber))
                    	m_Index.Add(dataByNumber, j);
                }
            }
        }
    }
	
	///////////////////////////////////////////////////////////////////
	//初始化操作
	
}


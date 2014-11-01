using System;
using UnityEngine;

[Serializable]
public class WDBSheetLine
{
    public object[] m_Line;

	public object GetData(object field)
	{
		int nfiled = Convert.ToInt32(field);
		if ((nfiled >= 0) && (nfiled < this.m_Line.Length))
        {

			if(m_Line[nfiled] == null)
				return null;
			return this.m_Line[nfiled];
		}
		return null;
    }



	public T GetData<T>(object field)
    {
		int nfiled = Convert.ToInt32(field);
		if ((nfiled >= 0) && (nfiled < this.m_Line.Length))
        {
			if(m_Line[nfiled] == null)
				return WDBData.createInstance<T>();
			return (T) this.m_Line[nfiled];
        }
        Debug.LogWarning("lineData.GetData:" + field + " error!");
        return WDBData.createInstance<T>();
    }
	public T GetData<T>(object field,T defaultValue)
	{
		int nfiled = Convert.ToInt32(field);
		if ((nfiled >= 0) && (nfiled < this.m_Line.Length))
		{
			if(m_Line[nfiled] == null)
				return defaultValue;
			return (T) this.m_Line[nfiled];
		}
		return defaultValue;
	}

	public int GetInt(object field,int defaultVal = -1)
	{
		return GetData<int>(field,defaultVal);
	}
	public bool GetBool(object field)
	{
		return GetData<bool>(field);
	}
	public string GetString(object field)
	{
		return GetData<string>(field);
	}

	public int? IntPtr(object field)
	{
		int nfiled = Convert.ToInt32(field);
		if ((nfiled >= 0) && (nfiled < m_Line.Length))
		{
			if(m_Line[nfiled] == null)
				return null;
			return new int?((int)m_Line[nfiled]);
		}
		return null;
	}
	public bool? BoolPtr(object field)
	{
		int nfiled = Convert.ToInt32(field);
		if ((nfiled >= 0) && (nfiled < m_Line.Length))
		{
			if(m_Line[nfiled] == null)
				return null;
			return new bool?((bool)m_Line[nfiled]);
		}
		return null;
	}
}


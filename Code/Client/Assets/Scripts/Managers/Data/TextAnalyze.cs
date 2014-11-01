using System;
using System.Collections.Generic;

public class TextAnalyze
{
    public enum TYPE : int
    {
        TYPE_INVALID = -1,
        TYPE_STRING = 0,
        TYPE_U64 = 1,
        TYPE_I64 = 2,
        TYPE_U32 = 3,
        TYPE_I32 = 4,
        TYPE_FLOAT = 5,
        TYPE_DOUBLE = 6,
        TYPE_BOOL = 7,
    }
    public TextAnalyze()
    {
    }

    public static WDBData Analyze(string text)
    {
        if (string.IsNullOrEmpty(text))
            return null;
        WDBData data = new WDBData();
        List<WDBSheetLine> sheetList = new List<WDBSheetLine>();
        data.m_Type = new List<TYPE>();
        Dictionary<int, TYPE> _type = new Dictionary<int, TYPE>();
        string[] lineData = text.Split('\n');

        int keyLine = -1;
        int typeLine = -1;
        for (int nLine = 0; nLine < lineData.Length; ++nLine)
        {
            string[] datas = lineData[nLine].Split('\r');
            datas = datas[0].Split('\t');

            if (datas.Length == 0 || datas[0].IndexOf("#") == 0)
                continue;//判断有错误,如果未找到该字符串，则为 -1。 如果 value 为 String.Empty，则返回值为 0

            if (datas.Length == 1 && datas[0] == "")
                continue;
            if (keyLine < 0)
            {
                keyLine = nLine;
            }
            else
            {
                if (nLine == keyLine + 1)
                    typeLine = nLine;
            }


            WDBSheetLine sheetLine = new WDBSheetLine();
            sheetLine.m_Line = new object[datas.Length];

            for (int col = 0; col < datas.Length; ++col)
            {

                //								if (nLine == keyLine)
                //										data.m_FieldName.Add (datas [col], col);
                if (nLine == typeLine)
                {
                    TYPE strType = nameToType(datas[col]);
                    if (strType == TYPE.TYPE_INVALID)
                    {
                        throw new Exception("failed to parse type: " + datas[col]);
                    }
                    _type.Add(col, strType);
                    data.m_Type.Add(strType);
                    continue;
                }
                if (nLine == keyLine)
                {
                    sheetLine.m_Line[col] = datas[col];
                }
                else
                {
                    sheetLine.m_Line[col] = ConvertValue(datas[col], _type[col]);
                }

            }
            if (nLine != typeLine)
                sheetList.Add(sheetLine);

        }

        if (sheetList.Count > 0)
        {
            data.m_Data = new WDBSheetLine[sheetList.Count];
            for (int n = 0; n < sheetList.Count; ++n)
            {
                data.m_Data[n] = sheetList[n];
            }
            sheetList.Clear();
            data.Init();
        }

        return data;
    }

    private static TYPE nameToType(string name)
    {
        switch (name)
        {
            case "STRING":

                return TYPE.TYPE_STRING;

            case "U64":

                return TYPE.TYPE_U64;

            case "I64":

                return TYPE.TYPE_I64;

            case "U32":

                return TYPE.TYPE_U32;

            case "I32":
            case "INT":

                return TYPE.TYPE_I32;

            case "FLOAT":

                return TYPE.TYPE_FLOAT;

            case "DOUBLE":

                return TYPE.TYPE_DOUBLE;

            case "BOOL":

                return TYPE.TYPE_BOOL;

            default:
                return TYPE.TYPE_INVALID;
        }
    }

    private static object ConvertValue(string source, TYPE type)
    {
		if (source == "")
		{
			if (type != TYPE.TYPE_STRING)
				throw new Exception("非STRING类型的列不能留空");

			return null;
		}

        try
        {
            switch (type)
            {
                case TYPE.TYPE_STRING:
                    {
						if (source == "-1")
							throw new Exception("无效的字符串参数需要留空, 而不是-1!");
                        return source.Clone();
                    }
                case TYPE.TYPE_I32:
                    {
                        return int.Parse(source);
                    }
                case TYPE.TYPE_BOOL:
                    {
//                         if (int.Parse(source) < 0)
//                             return false;
						if (source != "0" && source != "1")
							throw new Exception("BOOL只可以填入0和1");

                        return Convert.ToBoolean(int.Parse(source));
                    }
                case TYPE.TYPE_I64:
                    {
                        return Int64.Parse(source);
                    }
                case TYPE.TYPE_DOUBLE:
                    {
                        return double.Parse(source);
                    }
                case TYPE.TYPE_FLOAT:
                    {
                        return float.Parse(source);
                    }

                case TYPE.TYPE_U32:
                    {
						if (source == "-1")
							return uint.MaxValue;

                        return UInt32.Parse(source);
                    }
                case TYPE.TYPE_U64:
                    {
                        return UInt64.Parse(source);
                    }
                default:
					break;
            }
        }
        catch
        {
            throw new Exception("无法将" + source + "转换为数据类型" + type);
        }

        return null;
    }
}




using System;

public class AssisterTableItem
{
    public int id;
    public string desc;
    public string func;
    //public string beizhu;
    //private int[] funcList;


    public int[] GetFunction()
    {   
        string[] strArray = func.Split(new char[]{'|'},StringSplitOptions.RemoveEmptyEntries);
        int[] funcList = new int[strArray.Length];
        for (int i = 0; i < strArray.Length; ++i)
        {
            funcList[i] = Convert.ToInt32(strArray[i]);
        }

        return funcList;
    }

}


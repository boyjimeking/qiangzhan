//Ó¶±ø±í¸ñ
public class CropsProRatioTableItem
{
	public int id;
    public string onestarsratio;
    public string twostarsratio;
    public string threestarsratio;
    public string fourstarsratio;
    public string fivestarsratio;

    private string this[int s]
    {
        get
        {
            switch (s)
            {
                case 1:
                    return onestarsratio;

                case 2:
                    return twostarsratio;

                case 3:
                    return threestarsratio;

                case 4:
                    return fourstarsratio;

                case 5:
                    return fivestarsratio;

                default:
                    GameDebug.LogError("Ó¶±øÐÇ¼¶Âß¼­´íÎó");
                    return null;
            }
        }
    }

    public float GetRatioPlayerByProId(int resid, int starslv)
    {
        float mRatio = 0.0f;
        string[] arr = this[starslv].Split('|');
        if (CropsDefine.PRORATIONNUM > arr.Length)
        {
            GameDebug.LogError("Ó¶±øÏµÊý±í-cropsproratio.txtÌîÐ´´íÎó£¡");
            return mRatio;
        }
        switch (resid)
        {
            case (int)PropertyTypeEnum.PropertyTypeMaxHP:
                mRatio = float.Parse(arr[0]);
                break;
            case (int)PropertyTypeEnum.PropertyTypeDamage:
                mRatio = float.Parse(arr[1]);
                break;
            case (int)PropertyTypeEnum.PropertyTypeCrticalLV:
                mRatio = float.Parse(arr[2]);
                break;
            case (int)PropertyTypeEnum.PropertyTypeDefance:
                mRatio = float.Parse(arr[3]);
                break;
            case (int)PropertyTypeEnum.PropertyTypeMaxMana:
                mRatio = float.Parse(arr[4]);
                break;
            default:
                break;
        }

        return mRatio;
    }

    public float GetRatioCropsByProId(int resid, int starslv)
    {
        float mRatio = 0.0f;
        string[] arr = this[starslv].Split('|');
        if (CropsDefine.PRORATIONNUM > arr.Length)
        {
            GameDebug.LogError("Ó¶±øÏµÊý±í-cropsproratio.txtÌîÐ´´íÎó£¡");
            return mRatio;
        }
        switch (resid)
        {
            case (int)PropertyTypeEnum.PropertyTypeMaxHP:
                mRatio = float.Parse(arr[5]);
                break;
            case (int)PropertyTypeEnum.PropertyTypeDamage:
                mRatio = float.Parse(arr[6]);
                break;
            case (int)PropertyTypeEnum.PropertyTypeCrticalLV:
                mRatio = float.Parse(arr[7]);
                break;
            case (int)PropertyTypeEnum.PropertyTypeDefance:
                mRatio = float.Parse(arr[8]);
                break;
            case (int)PropertyTypeEnum.PropertyTypeMaxMana:
                mRatio = float.Parse(arr[9]);
                break;
            default:
                break;
        }

        return mRatio;
    }
}

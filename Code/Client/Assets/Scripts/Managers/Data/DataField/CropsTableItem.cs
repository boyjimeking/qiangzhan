//Ó¶±ø±í¸ñ
public class CropsTableItem : ItemTableItem
{
    public int itemid;
    public int itemnum;
    public int itemnum2;
    public int itemnum3;
    public int itemnum4;
    public int itemnum5;
    public int skillid1;
    public int skillid2;
    public int skillid3;
    public string cropsheadpic;

    public int this[int index]
    { 
        get
        {
            switch (index)
            {
                case 1:
                    return itemnum;
                case 2:
                    return itemnum2;
                case 3:
                    return itemnum3;
                case 4:
                    return itemnum4;
                case 5:
                    return itemnum5;
                default:
                    GameDebug.LogError("Ó¶±ø±íÂß¼­´íÎó");
                    return 0;
            }
        }
        
    }
}

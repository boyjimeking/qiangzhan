using UnityEngine;
using System.Collections;

public class TitleItemTableItem 
{
    public int id;
    //称号名;
    public string name;
    //所属分组;
    public int groupId;
    //称号类型;(0=限时，1=永久)
    public int type;
    //激活条件类型;
    public int contentId;
    //条件参数1;
    public int condition1;
    //条件参数2;
    public int condition2;
    //条件参数3;
    public int conditionVal;
    //对应的BuffID;
    public int buffId;
    //属性说明1;
    public string detail1;
    //属性说明2;
    public string detail2;
    //属性说明3;
    public string detail3;
    //属性说明4;
    public string detail4;
    //战力值;
    public int grade;
    //称号图;
    public string picName;
    //激活条件说明;
    public string contentDetail;
}

using UnityEngine;
using System.Collections;

public class AITableItem 
{
    public int      resID;
    public string   desc;
    public int      type;                   //AI类型
    public int      checkway;           //是否可以看见不能到达的敌人
    public int      searchEnemyInterval;    //索敌间隔
    public int      searchEnemyRadius;      //索敌距离
    public int      chaseDist;              //追敌距离
    public int      changeTarOdds;          //换目标的几率
    public int      changeTarMinInterval;   //改变目标的最小间隔
    public int      changeTarMaxInterval;   //改变目标的最大间隔   

    public int      skillslot1;
    public int      skillslot2;
    public int      skillslot3;
    public int      skillslot4;
    public int      skillslot5;
    public int      skillslot6;
    public int      skillslot7;
    public int      skillslot8;
    public int      skillslot9;
    public int      skillslot10;
    public int      skillslot11;
    public int      skillslot12;
    public int      skillslot13;
    public int      skillslot14;
    public int      skillslot15;

    // 由对应的AI类型进行解析
    public string commonIdle;

    public string param1;
    public string param2;
    public string param3;
    public string param4;
    public string param5;
    public string param6;
    public string param7;
    public string param8;
    public string param9;
    public string param10;
    public string param11;
    public string param12;
    public string param13;
    public string param14;
    public string param15;
    public string param16;
    public string param17;
    public string param18;
    public string param19;
    public string param20;
};

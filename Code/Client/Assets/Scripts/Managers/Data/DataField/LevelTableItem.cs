using UnityEngine;
using System.Collections;

public class LevelTableItem
{
    public int level;
    public int exp;
    public int maxhp;
    public int damage;
    public int crticalLV;
    public int damageReduce;
    public int energy;
	public int sp;

	// 血/蓝的每秒恢复值.
	public float hpRegRate;
	public float manaRegRate;

    public int grade;
    //推荐战斗力
    public int recom_grade;
}

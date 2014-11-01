using UnityEngine;
using System.Collections;

public class BuildTableItem
{
	// Id
	public int resID;

	// 备注
	public string desc;

    //阵营
    public LeagueDef league;

    // 模型Id
    public int modelId;

    //材质ID
    public uint materialID;

	// 血量
	public int hp;

	// 出生特效Id
	public int born_effect;

	// 出生特效持续时间
	public int born_effect_time;

	// 特效X缩放
	public float born_effect_scale_x;

	// 特效Y缩放
	public float born_effect_scale_y;

	// 特效Z缩放
	public float born_effect_scale_z;

	// 死亡动作
    public string die_ani;

    //死亡消失时间
    public float die_time;

    //BuffID
    public uint buffID;
    
    //死亡添加特效
    public int die_effect;
    public string die_bone;

	// 图形类型
	public ShapeType shapeType;

	// 图形参数1
	public float shapeParam1;

	// 图形参数2
	public float shapeParam2;
}

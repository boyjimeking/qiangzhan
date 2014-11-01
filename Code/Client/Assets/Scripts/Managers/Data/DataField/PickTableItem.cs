using UnityEngine;
using System.Collections;

public class PickTableItem
{
	// Id
	public int resID;

	// 备注
	public string desc;

	// 模型Id
	public int modelId;

	// 半径
	public int radius;

	// 掉落特效
	public int dropParticleId;

	// 掉落特效绑点
	public string dropParticleBone;

	// 被拾取特效
	public int pickParticleId;

	// 被拾取特效绑点
	public string pickParticleBone;

	// 拾取者特效Id
	public int playerParticleId;

	// 拾取者特效绑点
	public string playerParticleBone;

	// 拾取者skilleffect id(skilleffect.txt).
	public uint skillEffect2Picker;

	// 拾取者加buff
	public string skillBuff2Picker;

	// 浮动距离
	public float moveDist;

	// 浮动速度
	public float moveSpeed;

	// 旋转速度
	public float rotateAngle;

	// 保留时间
	public int keepTime;

	// 是否抛物
	public int flyOut;

	// 拾取音效Id
	public int pickSoundId;
}

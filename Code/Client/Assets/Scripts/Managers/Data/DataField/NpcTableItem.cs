using UnityEngine;
using System.Collections;

public class NPCTableItem {
	
	public int resID;
	public string name;
    public string information;
	public string headicon;
	public float radius;

    public int ai;
	public int model;
	public int weaponid;
	
	public LeagueDef league;

    public uint WaitDisappearTime;      //尸体存在时间
    public uint DisappearTime;          //尸体消失时间(半透过程的时间)

	public uint materialID;

    public float movespeed;
	public int defaultHP;			//默认血量上限
	public int defaultDamage;		//默认伤害
	public int defaultCrticalLV;	//默认暴击等级
	public int defaultDamageReduce;	//默认伤害减免
	public int defaultEnergy;		//默认能量值

    public uint bornEffect;         // 出生前特效
    public uint bornEffectTime;     //出生前特效时间

    public string bornAni;            //出生动作
    public uint bornAniTime;        //出生动作持续时间

	public uint bornBuff_0;			// 出生BUFF.
	public uint bornBuff_1;

    public int movetype;         //移动方式 MovingType

	// 等级
	public uint level;

	// Boss血条每管血量 -1不显示
	public int bossHpUnit;

    //是否显示血槽
    public bool showHp;

	// 金币数
	public int dropMoney;

	// 金币掉落概率
	public int dropMoneyWeight;

	// 金币PickId -1表示不掉落到场景
	public int dropMoneyPickId;

	// 死亡物品掉落包
	public int itemDropBoxId;

	// 是否掉落到场景 物品的PickId在item表中
	public int isDropOnGround;

	// 死亡Buff掉落包
	public int buffDropBoxId;

    //死亡播放的声音
    public int DeadSound;

    //播放概率
    public int deadSoundProb;

	// 死亡动作.
	public string deathAnimation;

    public int walkSound;

    //叫声
    public int crySound;
    //叫声间隔
    public int cryInternal;
    //叫声概率
    public int crySoundProp;

    //喊话
    public int talkID;

	// 枪林弹雨分数
    public int qiangLinDanYuScore;
	// 压制邪恶分数
	public uint yaZhiXieEScore;
}

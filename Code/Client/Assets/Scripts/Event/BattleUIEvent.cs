using UnityEngine;
using System.Collections;

public class BattleUIEvent : EventBase
{

	public static string BATTLE_UI_KILL_OTHER = "BATTLE_UI_KILL_OTHER";
	public static string BATTLE_UI_PICK_GEAR  = "BATTLE_UI_PICK_GEAR";//打僵尸活动捡齿轮;
    public static string BATTLE_UI_PICK_GOLD = "BATTLE_UI_PICK_GOLD";//打僵尸活动捡金币;
	public static string BATTLE_UI_SHOW_TIMER = "BATTLE_UI_SHOW_TIMER";//关卡计时器，不要直接发，界面没Load出来时候，收不到事件，调用BattleUIModule中的ShowTimer;
    public static string BATTLE_UI_ZOMBIE_CRAZY = "BATTLE_UI_ZOMBIE_CRAZY";//打僵尸僵尸疯狂了动画;
    public static string BATTLE_UI_SAY_SOMETHING = "BATTLE_UI_SAY_SOMETHING";//打僵尸，还剩30秒提示;
    public static string BATTLE_UI_ZOMBIE_PICK1 = "BATTLE_UI_ZOMBIE_PICK1";//打僵尸捡东西增加进度条;
    public static string BATTLE_UI_TEN_SECOND = "BATTLE_UI_TEN_SECOND";//打僵尸，还剩十秒;
    public static string BATTLE_UI_ZOMBIE_ENERMY_NUM = "BATTLE_UI_ZOMBIE_ENERMY_NUM";//僵尸本儿，显示怪物数量;
    public static string BATTLE_UI_PICK_TEMPMONEY = "BATTLE_UI_PICK_TEMPMONEY";//打挑战本时捡代币
    public static string BATTLE_UI_DAMAGE = "BATTLE_UI_DAMAGE";
	public static string BATTLE_UI_BUFF_ADDED = "BATTLE_UI_BUFF_ADDED";
    public static string BATTLE_UI_PLAYER_DAMAGE = "BATTLE_UI_PLAYER_DAMAGE";
	public static string BATTLE_UI_GHOST_DAMAGE = "BATTLE_UI_GHOST_DAMAGE";
	public static string BATTLE_UI_PLAYER_MANA_CHANGED = "BATTLE_UI_PLAYER_MANA_CHANGED";
    public static string BATTLE_UI_UPDATE_MONSTER_FlOOD = "BATTLE_UI_UPDATE_MONSTER_FlOOD";//刷新挑战本波数
    public static string BATTLE_UI_PICK_BUFF = "BATTLE_UI_PICK_BUFF";//拾取挑战本buff
	public static string BATTLE_UI_PROGRESS_SHOW = "BATTLE_UI_PROGRESS_SHOW"; // 显示进度条
	public static string BATTLE_UI_PROGRESS_UPDATE = "BATTLE_UI_PROGRESS_UPDATE"; // 更新进度条
	public static string BATTLE_UI_PROGRESS_OVER = "BATTLE_UI_PROGRESS_OVER"; // 进度碎裂 指引方向
    public static string BATTLE_UI_BOSSBLOOD_RESET = "BATTLE_UI_BOSSBLOOD_RESET"; //boss血条重置

    public static string BATTLE_UI_SHOOT_TYPE_CHANGE = "BATTLE_UI_SHOOT_TYPE_CHANGE";
	public object msg;
    public object msg1;

    public Vector3 pos = Vector3.zero;
	public int deltaMana = 0;
    public DamageInfo damage = null;
	public bool dead = false;

	public string bmpPath = null;
    public BattleUIEvent(string eventName)
        : base(eventName)
	{
		
	}
}

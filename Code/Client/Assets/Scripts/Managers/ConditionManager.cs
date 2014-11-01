using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ConditionManager
{
    private static ConditionManager instance;

    public ConditionManager()
	{
		instance = this;
	}

    public static ConditionManager Instance
	{
		get
		{
			return instance;
		}
	}

	/// <summary>
	/// 得到某条件的当前值，返回是否满足条件
	/// </summary>
	/// <param name="type">条件类型</param>
	/// <param name="param1">条件参数1</param>
	/// <param name="param2">条件参数2</param>
	/// <param name="value">传出当前值</param>
	/// <returns></returns>
	public bool GetCurrentConditionValue(ConditionType type, int param1, int param2, ref int value)
	{
		PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
		if (module == null)
		{
			return false;
		}

		switch(type)
		{
				// 等级
			case ConditionType.LEVEL:
				{
					value = module.GetLevel();
				}
				break;
				// 货币
			case ConditionType.MONEY:
				{
                    value = (int)module.GetProceeds((ProceedsType)param1);
				}
				break;
				// 经验
			case ConditionType.EXP:
				{
					value = module.GetExp();
				}
				break;
				// 道具
			case ConditionType.ITEM:
				{
					//TODO
					value = 0;
				}
				break;
				// 关卡解锁
			case ConditionType.STAGE_UNLOCK:
				{
					value = module.IsStageUnlock(param1) ? 1 : 0;
				}
				break;
				// 关卡评价
			case ConditionType.STAGE_GRADE:
				{
					value = (int)module.GetStageGrade(param1);
				}
				break;
				// 接受任务
			case ConditionType.QUEST_ACCEPT:
				{
					if(module.IsQuestAccepted(param1))
					{
						value = 1;
					}
					else
					{
						value = 0;
					}
				}
				break;
			default:
				return false;
		}

		return true;
	}

	/// <summary>
	/// 得到要求的条件值
	/// </summary>
	/// <param name="conditionId"></param>
	/// <returns></returns>
	public int GetConditionRequiredValue(int conditionId)
	{
		if (!DataManager.ConditionTable.ContainsKey(conditionId))
		{
			return -1;
		}

		ConditionTableItem res = DataManager.ConditionTable[conditionId] as ConditionTableItem;
		if (res == null)
		{
			return -1;
		}

		return res.mValue;
	}

	/// <summary>
	/// 检查是否满足条件
	/// </summary>
	/// <param name="type">条件类型</param>
	/// <param name="param1">条件参数1</param>
	/// <param name="param2">条件参数2</param>
	/// <param name="value">条件值</param>
	/// <returns>是否满足</returns>
	public bool CheckCondition(ConditionType type, int param1, int param2, int value)
	{
		int curValue = 0;
		if (!GetCurrentConditionValue(type, param1, param2, ref curValue))
		{
			return false;
		}

		return curValue >= value;
	}

	/// <summary>
	/// 检查是否满足条件
	/// </summary>
	/// <param name="conditionId">条件表Id</param>
	/// <returns>是否满足</returns>
	public bool CheckCondition(int conditionId)
	{
		if(!DataManager.ConditionTable.ContainsKey(conditionId))
		{
			return false;
		}

		ConditionTableItem res = DataManager.ConditionTable[conditionId] as ConditionTableItem;
		if(res == null)
		{
			return false;
		}

		return CheckCondition(res.mType, res.mParam1, res.mParam2, res.mValue);
	}

	/// <summary>
	/// 条件描述文本
	/// </summary>
	/// <param name="conditionId">条件表Id</param>
	/// <returns></returns>
	public string GetConditionText(int conditionId)
	{
		if (!DataManager.ConditionTable.ContainsKey(conditionId))
		{
			return null;
		}

		ConditionTableItem res = DataManager.ConditionTable[conditionId] as ConditionTableItem;
		if (res == null)
		{
			return null;
		}

		return GetConditionText(res.mType, res.mParam1, res.mParam2, res.mValue);
	}

	/// <summary>
	/// 条件描述文本
	/// </summary>
	/// <param name="type">条件类型</param>
	/// <param name="param1">参数1</param>
	/// <param name="param2">参数2</param>
	/// <param name="value">值</param>
	/// <returns></returns>
	public string GetConditionText(ConditionType type, int param1, int param2, int value)
	{
		string ret = null;
		switch (type)
		{
			// 等级
			case ConditionType.LEVEL:
				{
					ret = "等级:" + value.ToString() + "级";
				}
				break;
			// 货币
			case ConditionType.MONEY:
				{
					switch(param1)
					{
						case (int)ProceedsType.Money_Game:
							{
								ret = "金币:" + value.ToString();
							}
							break;
						case (int)ProceedsType.Money_RMB:
							{
								ret = "钻石:" + value.ToString();
							}
							break;
						case (int)ProceedsType.Money_Prestige:
							{
								ret = "声望:" + value.ToString();
							}
							break;
						case (int)ProceedsType.Money_Stren:
							{
								ret = "强化点:" + value.ToString();
							}
							break;
						case (int)ProceedsType.Money_Arena:
							{
								ret = "竞技币:" + value.ToString();
							}
							break;
						default:
							{
								GameDebug.LogError("未知货币类型");
							}
							break;
					}
				}
				break;
			// 经验
			case ConditionType.EXP:
				{
					ret = "经验:" + value.ToString();
				}
				break;
			// 道具
			case ConditionType.ITEM:
				{
					string itemname = ItemManager.Instance.getItemName(param1);
					if(!string.IsNullOrEmpty(itemname))
					{
						ret = itemname + "X" + value.ToString();
					}
				}
				break;
			// 关卡解锁
			case ConditionType.STAGE_UNLOCK:
				{
					if (DataManager.Scene_StageSceneTable.ContainsKey(param1))
					{
						Scene_StageSceneTableItem stageres = DataManager.Scene_StageSceneTable[param1] as Scene_StageSceneTableItem;
						if (stageres != null)
						{
							ret = "解锁关卡:" + stageres.name;
						}
					}
				}
				break;
			// 关卡评价
			case ConditionType.STAGE_GRADE:
				{
					string[] grade = { "S", "A", "B", "C" };
					if (value >= (int)StageGrade.StageGrade_C && value <= (int)StageGrade.StageGrade_S)
					{
						ret = "关卡评价:" + grade[value];
					}
				}
				break;
			default:
				return null;
		}

		return ret;
	}

	/// <summary>
	/// 条件图标
	/// </summary>
	/// <param name="conditionId">条件表Id</param>
	/// <returns></returns>
	public string GetConditionIcon(int conditionId)
	{
		if (!DataManager.ConditionTable.ContainsKey(conditionId))
		{
			return null;
		}

		ConditionTableItem res = DataManager.ConditionTable[conditionId] as ConditionTableItem;
		if (res == null)
		{
			return null;
		}

		return GetConditionIcon(res.mType, res.mParam1, res.mParam2);
	}

	/// <summary>
	/// 条件图标
	/// </summary>
	/// <param name="type">类型</param>
	/// <param name="param1">参数1</param>
	/// <param name="param2">参数2</param>
	/// <returns></returns>
	public string GetConditionIcon(ConditionType type, int param1, int param2)
	{
		string ret = null;
		switch (type)
		{
			// 等级
			case ConditionType.LEVEL:
				{
					//ret = "icon2";
				}
				break;
			// 货币
			case ConditionType.MONEY:
				{
					if(param1 == (int)ProceedsType.Money_Game)
					{
						ret = "common:jinbi1";
					}
					else if(param1 == (int)ProceedsType.Money_RMB)
					{
						ret = "common:zhuanshi1";
					}
					else if(param1 == (int)ProceedsType.Money_Prestige)
					{
						ret = "common:shengwang1";
					}
					else if(param1 == (int)ProceedsType.Money_Arena)
					{
						ret = "common:hupbi3";
					}
				}
				break;
			// 经验
			case ConditionType.EXP:
				{
					ret = "stage_icon_exp";
				}
				break;
			// 道具
			case ConditionType.ITEM:
				{
					ret = ItemManager.Instance.getItemBmp(param1);
				}
				break;
			// 关卡解锁
			case ConditionType.STAGE_UNLOCK:
				{
					//ret = "icon2";
				}
				break;
			// 关卡评价
			case ConditionType.STAGE_GRADE:
				{
					//ret = "icon2";
				}
				break;
			default:
				return null;
		}

		return ret;
	}
}

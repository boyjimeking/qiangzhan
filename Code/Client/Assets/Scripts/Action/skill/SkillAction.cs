using System;
using System.Collections.Generic;
using Message;

public class SkillWeaponSkillParam
{

}

public class SkillEquipActionParam
{
    //技能索引
    public int SkillIdx
    {
        get;
        set;
    }
    
    public int[] EquipIdx = new int[SkillMaxCountDefine.MAX_EQUIP_SKILL_NUM];

    public SkillEquipActionParam()
    {
        for( int i = 0; i < SkillMaxCountDefine.MAX_EQUIP_SKILL_NUM ; ++i )
        {
            EquipIdx[i] = -1;
        }
    }
}
public class SkillLevelActionParam
{
    //技能索引
    public List<int> SkillIdxs = new List<int>();
    public List<int> DefaultEquips = new List<int>();
}
public class SkillAction : LogicAction<request_skill_op, respond_skill_op>
{
    public SkillAction()
        : base((int)MESSAGE_ID.ID_MSG_SKILL)
    {
    }

    protected override void OnRequest(request_skill_op request, object userdata)
    {
        if( userdata is SkillEquipActionParam )
        {
            SkillEquipActionParam param = userdata as SkillEquipActionParam;
            request.op_type = (int)SKILL_OP_TYPE.SKILL_OP_EQUIP;
   
            skill_equip_info info = new skill_equip_info();
            for( int i = 0; i < param.EquipIdx.Length ; ++i )
            {
                info.equip_id.Add(param.EquipIdx[i]);
            }
            request.equip_info = info;
        }
        if (userdata is SkillLevelActionParam)
        {
            SkillLevelActionParam param = userdata as SkillLevelActionParam;
            request.op_type = (int)SKILL_OP_TYPE.SKILL_OP_LEVEL;

            for (int i = 0; i < param.SkillIdxs.Count; ++i )
            {
                request.skill_idx.Add(param.SkillIdxs[i]);
            }

            for (int i = 0; i < param.DefaultEquips.Count; ++i)
            {
                request.default_equip_idx.Add(param.DefaultEquips[i]);
            }
        }
        if (userdata is SkillWeaponSkillParam)
        {
            request.op_type = (int)SKILL_OP_TYPE.SKILL_OP_UNLOCK_WEAPON_SKILL;
        }
    }
    protected override void OnRespond(respond_skill_op respond, object userdata)
    {
        SkillModule module = ModuleManager.Instance.FindModule<SkillModule>();
        if( module == null )
        {
            GameDebug.LogError("没有找到SkillModule");
            return;
        }
        if (respond.errorcode != (int)Message.ERROR_CODE.ERR_SKILL_OK)
        {
            // 进行错误提示
            return;
        }
        if (userdata is SkillLevelActionParam)
        {
            for (int i = 0; i < respond.skill_idx.Count; ++i )
            {
                module.SetSkillLevel(respond.skill_idx[i], respond.skill_level[i]);
            }
        }
    }
}

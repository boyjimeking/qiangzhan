using System;
using System.Collections.Generic;
using Message;

public class PassStageActionParam
{
	// 关卡id
	public int stageid = -1;

	// 获得最高评级
	public StageGrade maxgrade = StageGrade.StageGrade_Invalid;

	// 连续杀伤率
	public uint killrate = 0;

	// 最高连击
	public uint maxcombo = 0;

	// 最快通关时间
	public uint passtimerecord = uint.MaxValue;

	// 普通翻牌数
	public uint normalcount = 0;

	// 钻石翻牌数
	public uint extracount = 0;
}

public class PassStageAction : LogicAction<request_msg_passstage, respond_msg_passstage>
{
	public PassStageAction()
		: base((int)MESSAGE_ID.ID_MSG_SCENE_PASS)
    {

    }

	protected override void OnRequest(request_msg_passstage request, object userdata)
    {
		PassStageActionParam param = userdata as PassStageActionParam;
		if (param == null)
            return;

		request.stage = new role_stage();
		request.stage.stage_id = (uint)param.stageid;
		request.stage.max_grade = (uint)param.maxgrade;
		request.stage.kill_rate = param.killrate;
		request.stage.max_combo = param.maxcombo;
		request.stage.passtime_record = param.passtimerecord;

		request.normal_award_count = param.normalcount;
		request.extra_award_count = param.extracount;
	    request.stage.pass_times ++;
    }

	protected override void OnRespond(respond_msg_passstage respond, object userdata)
    {
		if (respond.result == (uint)ERROR_CODE.ERR_SCENE_PASS_OK)
		{
			StageDataManager.Instance.SyncStagePass(respond.stage);

		}
		else
		{
			StageDataManager.Instance.PrintErrorCode((ERROR_CODE)(respond.result));
		}
    }
}

using System;
using System.Collections.Generic;
using Message;

public class YaZhiXieEReportActionParam
{
    public uint score = 0;

	public uint time_cost = uint.MaxValue;
}

public class YaZhiXieEReportScoreAction : LogicAction<request_yazhixiee_report, respond_yazhixiee_report>
{
    public YaZhiXieEReportScoreAction()
        : base((int)MESSAGE_ID.ID_MSG_YAZHIXIEE_REPORT)
    {

    }

	protected override void OnRequest(request_yazhixiee_report request, object userdata)
    {
		YaZhiXieEReportActionParam param = userdata as YaZhiXieEReportActionParam;
        request.score = param.score;
		request.time_cost = param.time_cost;
    }

    protected override void OnRespond(respond_yazhixiee_report respond, object userdata)
    {
        YaZhiXieEScene scn = SceneManager.Instance.GetCurScene() as YaZhiXieEScene;
        if(scn != null)
        {
            scn.ResetReportTime();
        }

		if (!respond.succeed)
			return;

		YaZhiXieEUpdateRankListEvent e = new YaZhiXieEUpdateRankListEvent();
		e.sortInfo = respond.sortinfo;

        EventSystem.Instance.PushEvent(e);
    }

}

using System;
using System.Collections.Generic;
using Message;

public class ZhaoCaiMaoReportActionParam
{
    public int damage = 0;
}

public class ZhaoCaiMaoReportScoreAction : LogicAction<request_zhaocaimao_report, respond_zhaocaimao_report>
{
	public ZhaoCaiMaoReportScoreAction()
        : base((int)MESSAGE_ID.ID_MSG_ZHAOCAIMAO_REPORT)
    {

    }

	protected override void OnRequest(request_zhaocaimao_report request, object userdata)
    {
		ZhaoCaiMaoReportActionParam param = userdata as ZhaoCaiMaoReportActionParam;
		request.damage = param.damage;
    }

    protected override void OnRespond(respond_zhaocaimao_report respond, object userdata)
    {
        ZhaoCaiMaoScene scn = SceneManager.Instance.GetCurScene() as ZhaoCaiMaoScene;
        if(scn != null)
        {
            scn.ResetReportTime();
        }

		if (!respond.succeed)
			return;

		ZhaoCaiMaoUpdateRankListEvent e = new ZhaoCaiMaoUpdateRankListEvent();
		e.sortInfo = respond.sortinfo;

        EventSystem.Instance.PushEvent(e);
    }

}

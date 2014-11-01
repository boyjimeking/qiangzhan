using System;
using System.Collections.Generic;
using Message;

public class QiangLinDanYuReportScoreActionParam
{
    public uint Score
    {
        get;
        set;
    }
}

public class QiangLinDanYuReportScoreAction : LogicAction<request_qianglindanyu_report_score, respond_qianglindanyu_report_score>
{
    public QiangLinDanYuReportScoreAction()
        : base((int)MESSAGE_ID.ID_MSG_QIANGLINDANYU_REPORT_SCORE)
    {

    }

    protected override void OnRequest(request_qianglindanyu_report_score request, object userdata)
    {
        QiangLinDanYuReportScoreActionParam param = userdata as QiangLinDanYuReportScoreActionParam;
        request.score = param.Score;
    }

    protected override void OnRespond(respond_qianglindanyu_report_score respond, object userdata)
    {
        //if(!respond.succeed)
        //{
        //    SceneManager.Instance.RequestEnterLastCity();
        //}
        //else
        {
            QiangLinDanYuScene scn = SceneManager.Instance.GetCurScene() as QiangLinDanYuScene;
            if(scn != null)
            {
                scn.ResetReportTime();
            }

            QiangLinDanYuUpdateEvent e = new QiangLinDanYuUpdateEvent();
            e.score = respond.score;
            e.sortInfo = respond.sortinfo;
            e.playerCount = respond.playercount;

            EventSystem.Instance.PushEvent(e);
        }
    }

}

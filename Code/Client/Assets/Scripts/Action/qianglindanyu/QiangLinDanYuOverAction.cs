using System;
using System.Collections.Generic;
using Message;

public class QiangLinDanYuOverActionParam
{
    public uint Score
    {
        get;
        set;
    }
}

public class QiangLinDanYuOverAction : LogicAction<request_qianglindanyu_over, respond_qianglindanyu_over>
{
    public QiangLinDanYuOverAction()
        : base((int)MESSAGE_ID.ID_MSG_QIANGLINDANYU_OVER)
    {

    }

    protected override void OnRequest(request_qianglindanyu_over request, object userdata)
    {
        QiangLinDanYuOverActionParam param = userdata as QiangLinDanYuOverActionParam;
        request.score = param.Score;
    }

    protected override void OnRespond(respond_qianglindanyu_over respond, object userdata)
    {
        if(!respond.succeed)
        {
            SceneManager.Instance.RequestEnterLastCity();
        }
        else
        {
            UIQiangLinDanYuOverParam param = new UIQiangLinDanYuOverParam();
            param.score = (int)respond.score;
            param.maxScore = (int)respond.maxscore;

            if (respond.item_award != null)
            {
                for (int i = 0; i < respond.item_award.Count; i++)
                {
                    param.resid.Add(respond.item_award[i].resid);
                    param.num.Add(respond.item_award[i].num);
                }
            }

            //param.resid.Add(1000);
            //param.num.Add(1);
            WindowManager.Instance.OpenUI("qianglindanyuover", param);
        }
    }

}

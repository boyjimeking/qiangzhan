using System;
using System.Collections.Generic;
using Message;

public class YaZhiXieEOverActionParam
{
	public uint score = 0;

	public uint time_cost = uint.MaxValue;
}

public class YaZhiXieEOverAction : LogicAction<request_yazhixiee_over, respond_yazhixiee_over>
{
	public YaZhiXieEOverAction()
        : base((int)MESSAGE_ID.ID_MSG_YAZHIXIEE_OVER)
    {

    }

	protected override void OnRequest(request_yazhixiee_over request, object userdata)
    {
		YaZhiXieEOverActionParam param = userdata as YaZhiXieEOverActionParam;
        request.score = param.score;
		request.time_cost = param.time_cost;
    }

    protected override void OnRespond(respond_yazhixiee_over respond, object userdata)
    {
        if(!respond.succeed)
        {
            SceneManager.Instance.RequestEnterLastCity();
        }
		else
		{
			GameScene scene = SceneManager.Instance.GetCurScene() as GameScene;
			if(scene == null)
			{
				SceneManager.Instance.RequestEnterLastCity();
				return;
			}

			UIYZXEBalanceParam param = new UIYZXEBalanceParam(respond.score, respond.time_cost, scene.GetResult());
			WindowManager.Instance.OpenUI("yzxebalance", param);
		}
    }

}

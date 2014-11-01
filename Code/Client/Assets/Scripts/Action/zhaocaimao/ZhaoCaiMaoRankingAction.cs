using System;
using System.Collections.Generic;
using Message;

public class ZhaoCaiMaoRankingAction : LogicAction<request_zhaocaimao_ranking, respond_zhaocaimao_ranking>
{
	public ZhaoCaiMaoRankingAction()
        : base((int)MESSAGE_ID.ID_MSG_ZHAOCAIMAO_RANKING)
    {

    }

	protected override void OnRequest(request_zhaocaimao_ranking request, object userdata)
    {

    }

	protected override void OnRespond(respond_zhaocaimao_ranking respond, object userdata)
    {
        if(!respond.succeed)
        {
            SceneManager.Instance.RequestEnterLastCity();
        }
		else
		{
			WindowManager.Instance.CloseUI("maodamageaward");

			UIMaoRankAwardParam param = new UIMaoRankAwardParam(respond.ranking, respond.item_id);
			WindowManager.Instance.OpenUI("maorankaward", param);
		}
    }

}

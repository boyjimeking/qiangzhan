using System;
using System.Collections.Generic;
using Message;

public class ZhaoCaiMaoOverActionParam
{
	public int damage = 0;
}

public class ZhaoCaiMaoOverAction : LogicAction<request_zhaocaimao_over, respond_zhaocaimao_over>
{
	public ZhaoCaiMaoOverAction()
        : base((int)MESSAGE_ID.ID_MSG_ZHAOCAIMAO_OVER)
    {

    }

	protected override void OnRequest(request_zhaocaimao_over request, object userdata)
    {
		ZhaoCaiMaoOverActionParam param = userdata as ZhaoCaiMaoOverActionParam;
		request.damage = param.damage;
    }

    protected override void OnRespond(respond_zhaocaimao_over respond, object userdata)
    {
        if(!respond.succeed)
        {
            SceneManager.Instance.RequestEnterLastCity();
        }
		else
		{
			PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
			if (!DataManager.ZhaoCaiMaoAwardTable.ContainsKey(module.GetLevel()))
				return;

			ZhaoCaiMaoAwardTableItem res = DataManager.ZhaoCaiMaoAwardTable[module.GetLevel()] as ZhaoCaiMaoAwardTableItem;

			MoneyItemTableItem moneyres = ItemManager.GetItemRes(res.mItemId) as MoneyItemTableItem;
			if(moneyres != null && moneyres.value > 0)
			{
				WindowManager.Instance.OpenUI("maodamageaward", moneyres.value);
			}
		}
    }

}

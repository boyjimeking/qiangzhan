using UnityEngine;
using System.Collections;
using Message;

public class EggClickParam
{
    public EGG_OP_TYPE opType
    {
        get;
        set;
    }
    public EggType eggType    // 蛋索引;
    {
        get;
        set;
    }

    public EggClickParam()
    {
        
    }
}

public class EggAction : LogicAction<request_egg_op, respond_egg_op>
{
    public EggAction()
        : base((int)MESSAGE_ID.ID_MSG_EGG)
    {
 
    }

    protected override void OnRequest(request_egg_op param, object userdata)
    {
        if (userdata is EggClickParam)
        {
            EggClickParam clickParam = userdata as EggClickParam;

            param.op_type = (int)clickParam.opType;
            param.eggid = (int)clickParam.eggType;
        }
    }

    protected override void OnRespond(respond_egg_op respond, object userdata)
    {
        EggModule module = ModuleManager.Instance.FindModule<EggModule>();
        
        if (module == null)
        {
            GameDebug.LogError("没有找到shopmodule");
            return;
        }

        if (respond.result != (int)Message.ERROR_CODE.ERR_EGG_OK)
        {
            switch ((Message.ERROR_CODE)respond.result)
            {
                case ERROR_CODE.ERR_EGG_FAILED:
                    break;
                //case ERROR_CODE.ERR_MALL_BUY_NO_TIMES:
                //    break;
                //case ERROR_CODE.ERR_MALL_FAILED:
                //    break;
                default:
                    break;
            }
            return;
        }

        if (userdata is EggClickParam)
        {
            module.OpenEggSucess((EggType)(respond.eggid) , respond.items);
        }
    }
}

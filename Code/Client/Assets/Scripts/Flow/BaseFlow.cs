using UnityEngine;
using System.Collections;

public enum GAME_FLOW_ENUM
{
    GAME_FLOW_INVAILD = -1,
    GAME_FLOW_VERIFY = 0,
    GAME_FLOW_LOGIN = 1,
    GAME_FLOW_MAIN = 2,
}

public enum FLOW_EXIT_CODE
{
    FLOW_EXIT_CODE_NONE = 0,
    FLOW_EXIT_CODE_ERROR = 1,
    FLOW_EXIT_CODE_NEXT = 2,    //进入下一流程
    FLOW_EXIT_CODE_PRE = 3,     //返回上一流程
}

public interface BaseFlow {
    bool Init();
    bool Term();
    FLOW_EXIT_CODE Update(uint elapsed);
    GAME_FLOW_ENUM GetFlowEnum();

}

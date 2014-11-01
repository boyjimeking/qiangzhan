using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MonsterFloodModule:ModuleBase
{
    public static string Pick_Buff = "pick_buff";
    //总的波数
    private int mTotalNum;
    public int mCurNum;
    //掉落物是否可拾取
    public bool IsPickUsable = false;
    //当前代币值
    private int mTempMoney = 0;
    private bool isShowPrompt = false;

    public int TempMoney
    {
        get { return mTempMoney; }
        set
        {
            if (mTempMoney < 50 && value >= 50)
            {
                isShowPrompt = true;
            }
            else
            {
                isShowPrompt = false;
            }
            mTempMoney = value;
            CheckPicksable(Pick_Buff);
        }
    }

    public int TotalNum
    {
        get { return mTotalNum; }
        set
        {
            GameDebug.Log("设置总数"+value);
            mTotalNum = value;
        }
    }

    public void Reset()
    {
        GameDebug.Log("挑战本重置数据");
        TempMoney = 0;
        //mCurNum = 0;
    }

    public bool IsMonsterFlood
    {
        get
        {
            GameScene bs = SceneManager.Instance.GetCurScene() as GameScene;
            if (bs == null)
            {
                 
                return false;
            }
            //Debug.Log("StageSceneType:" + bs.GetStageRes().mSubType);
            return SceneManager.GetSceneType(bs.GetSceneRes()) == SceneType.SceneType_MonsterFlood;
        }
    }

    public bool CheckPicksable(string pickname)
    {
        bool usable = CheckTempMoney();
        StageScene bs = SceneManager.Instance.GetCurScene() as StageScene;
        if (bs == null)
        {
            return false;
        }

        List<Pick> picks = bs.SearchObjsByAlias<Pick>(pickname);
        if (picks == null || picks.Count == 0)
        {
            GameDebug.Log("当前没有pickbuff");
            return false;
        }
        GameDebug.Log("设置buf状态：" + usable + "pick_buff数目：" + picks.Count);
        if (usable && isShowPrompt)
        {           
            PromptUIManager.Instance.AddNewPrompt(StringHelper.GetString("buy_daibi"));
        }
       
        foreach (var pick in picks)
        {
            if (usable)
            {               
                pick.ChangeAlpha(1);
                pick.mIsPickable = true;
            }
            else
            {
                pick.ChangeAlpha(0.3f);
                pick.mIsPickable = false;
            }
           
        }
        return true;
    }

    public bool CheckTempMoney()
    {
        return (mTempMoney >=50);
    }



}


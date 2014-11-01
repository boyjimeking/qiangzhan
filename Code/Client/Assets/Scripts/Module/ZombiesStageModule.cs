using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

class ZombiesStageModule : ModuleBase
{
    private static uint mTotalEnermyNum = 0;
    private static uint mLastEnermyNum = 0;
    private uint mLastGoldMoney = 0;
    private int mLastPlayerLv = 0;
    private Scene_StageSceneTableItem mCurZombieScene = null;
    private Scene_StageSceneTableItem mNextZombieScene = null;

    private static int mPickId = -1;

    public static int PickId
    {
        get
        {
            if(mPickId < 0)
            {
                mPickId = (int)ConfigManager.GetVal<int>(ConfigItemKey.ZOMBIE_BUFF_ID);
            }

            return mPickId;
        }
    }

    public bool IsZombieGame 
    {
        get
        {
            StageScene bs = SceneManager.Instance.GetCurScene() as StageScene;
            if (bs == null)
            {
                //GameDebug.LogError("scene找不到");
                return false;
            }

            return SceneManager.GetSceneType(bs.GetStageRes()) == SceneType.SceneType_Zombies;
        }
    }

    public Scene_StageSceneTableItem CurZombieScene
    {
        get
        {
            return mCurZombieScene;
        }
    }

    public Scene_StageSceneTableItem NextZombieScene
    {
        get
        {
            return mNextZombieScene;
        }
    }

    public uint BeginGoldNum
    {
        get
        {
            return mLastGoldMoney;
        }
    }

    override protected void OnEnable()
    {
        base.OnEnable();
        //EventSystem.Instance.addEventListener(PlayerDataEvent.PLAYER_DATA_CHANGED, onPlayerDataChanged);
    }

    override protected void OnDisable()
    {
        base.OnDisable();
    }

    public void OpenZombieStageListUI()
    {
        
    }

    public void SetBeginGoldNum()
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        mLastGoldMoney = module.GetProceeds(ProceedsType.Money_Game);
    }

    public static bool ZombieCrazy()
    {
        BattleUIEvent bue = new BattleUIEvent(BattleUIEvent.BATTLE_UI_ZOMBIE_CRAZY);
        EventSystem.Instance.PushEvent(bue);
        return true;
    }

    //public static bool SaySomething(string str)
    //{
    //    //BattleUIEvent bue = new BattleUIEvent(BattleUIEvent.BATTLE_UI_SAY_SOMETHING);
    //    //bue.msg = str;
    //    //EventSystem.Instance.PushEvent(bue);

    //    PromptUIManager.Instance.AddNewPrompt(str);

    //    return true;
    //}

    public static bool ZombieTenSecond(int fontSize, int resId) 
    {
        BattleUIEvent bue = new BattleUIEvent(BattleUIEvent.BATTLE_UI_TEN_SECOND);
        bue.msg = fontSize;
        bue.msg1 = resId;
        EventSystem.Instance.PushEvent(bue);
        return true;
    }

    private void onPlayerDataChanged(EventBase ev)
    {
        if (!IsZombieGame) return;

        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        if (mLastPlayerLv != module.GetLevel())
        {
            mLastPlayerLv = module.GetLevel();

            Scene_StageSceneTableItem cur = null, next = null;
            if (GetCurAndNextStage(out cur, out next))
            {
                if ((cur != mCurZombieScene) || (next != mNextZombieScene))
                {
                    UpdateZombieStageList();
                }
            }
            else
            {
                Debug.LogError("数据错误");
            }
        }
    }

    void UpdateZombieStageList()
    {
        //StageUnlockEvent ev = new StageUnlockEvent(StageUnlockEvent.ZOMBIE_STAGE_UNLOCK);
        //EventSystem.Instance.PushEvent(ev);
    }

    /// <summary>
    /// 根据StageSceneTableItem表中的解锁条件ID1来作为可进入等级判定;
    /// </summary>
    /// <param name="item1">当前等级可以进入的</param>
    /// <param name="item2">下一个等级可以进入的</param>
    /// <returns></returns>
    bool GetCurAndNextStage(out Scene_StageSceneTableItem item1, out Scene_StageSceneTableItem item2)
    {
        int cur = 0, next = int.MaxValue;

        item1 = null;
        item2 = null;
        IDictionaryEnumerator itr = DataManager.Scene_StageSceneTable.GetEnumerator();
        while (itr.MoveNext())
        {
            Scene_StageSceneTableItem scene = itr.Value as Scene_StageSceneTableItem;
            if (SceneManager.GetSceneType(scene) != SceneType.SceneType_Zombies)
                continue;

            int limitLv = scene.mUnlockCondId0;

            if ((limitLv < mLastPlayerLv) && (limitLv > cur))
            {
                cur = limitLv;
                item1 = scene;
            }

            if ((limitLv > mLastPlayerLv) && (limitLv < next))
            {
                next = limitLv;
                item2 = scene;
            }
        }
//         foreach (Scene_StageSceneTableItem scene in DataManager.Scene_StageSceneTable)
//         {
//             
//         }

        if (item1 == null)
            return false;

        return true;
    }

    public static bool SetTotalEnermyNum(uint num)
    {
        mLastEnermyNum = mTotalEnermyNum = num;
       
        BattleUIEvent ev = new BattleUIEvent(BattleUIEvent.BATTLE_UI_ZOMBIE_ENERMY_NUM);

        ev.msg = mLastEnermyNum;
        ev.msg1 = mTotalEnermyNum;

        EventSystem.Instance.PushEvent(ev);
        
        return true;
    }

    public static uint GetTotalEnermyNum()
    {
        return mTotalEnermyNum;
    }

    public static uint GetLastEnermyNum()
    {
        return mLastEnermyNum;
    }

    public bool KillEnermy()
    {
        mLastEnermyNum--;

        if (mLastEnermyNum < 0)
            return false;

        BattleUIEvent ev = new BattleUIEvent(BattleUIEvent.BATTLE_UI_ZOMBIE_ENERMY_NUM);
        
        ev.msg = mLastEnermyNum;
        ev.msg1 = mTotalEnermyNum;

        EventSystem.Instance.PushEvent(ev);

        if (mLastEnermyNum == 0)
        {
            return true;
        }

        return false;
    }
}

using FantasyEngine;


/// <summary>
/// 承载上层逻辑,依据人物状态和指令驱动的
/// 动作状态控制器
/// </summary>
public class MecanimStateController
{
    public enum Statelayer : uint
    {
        Normal,
        Move,
        Hurt,
        Count,
    }
    //动画控制器
    private MecanimManager mAnimManager;
    private VisualObject mOwner;

    private AnimFSM[] mAnimStates;


    private bool mHasSetup = false;

    public MecanimStateController()
    {

    }
    public void Setup(MecanimManager manager, VisualObject owner)
    {
        if (manager == null || manager.Anim == null || manager.Anim.runtimeAnimatorController == null)
            return;
        //初始化
        mAnimManager = manager;
        mOwner = owner;
        VisualAnimFSM vfsm = new VisualAnimFSM(mAnimManager, mOwner);
        vfsm.Initialize();
        vfsm.Activate();

        MoveAnimFSM mfsm = new MoveAnimFSM(mAnimManager, mOwner);
        mfsm.Initialize();
        mfsm.Activate();


        HurtAnimFSM hfsm = new HurtAnimFSM(mAnimManager, mOwner);
        hfsm.Initialize();
        hfsm.Activate();

        mAnimStates = new AnimFSM[] { vfsm, mfsm, hfsm };

        mHasSetup = true;
    }

    public AnimatorProperty AnimSet
    {
        get
        {
            if (mAnimManager == null)
                return null;
            return mAnimManager.Property;
        }
    }

    public void DoAction(AnimAction action, Statelayer layer = MecanimStateController.Statelayer.Normal)
    {
        //if(mOwner is Npc)
        //    Debug.Log(action);
        if (!mHasSetup)
            return;
        if (layer >= Statelayer.Count || action == null)
            return;
        mAnimStates[(uint)layer].DoAction(action);
    }
    public void DoAction(AnimActionFactory.E_Type type,Statelayer layer = Statelayer.Normal)
    {
        if (!mHasSetup)
            return;
       AnimAction action = AnimActionFactory.Create(type);
       DoAction(action, layer);
    }

    /// <summary>
    /// 结束当前的状态
    /// </summary>
    /// <param name="nStateHash">如果当前处于 ifStateHash时</param>
    /// <param name="layer"></param>
    public void  FinishCurrentState(int ifStateHash,Statelayer layer = Statelayer.Normal)
    {

        if (!mHasSetup)
            return;
        if (layer >= Statelayer.Count || ifStateHash == 0)
            return;
     

        EventContext content = new EventContext();
        content.curstate = ifStateHash;
        MecanimEvent even = new MecanimEvent(MecanimEvent.MEC_ANIM_END, content);

        mAnimStates[(uint)layer].CurrentAnimState.HandlemAnimatorEvent(even);
    }

    public void FinishCurrentState(string statename,Statelayer layer = Statelayer.Normal)
    {

        FinishCurrentState(AnimSet.GetStateHash(statename), layer);
    }

    public void Update(uint elapsed)
    {

        if (!mHasSetup)
            return;

        if (mAnimManager != null)
        {
            mAnimManager.Update();
        }
        //更新状态

        foreach(AnimFSM fsm in mAnimStates)
        {
            fsm.UpdateAnimStates();
        }
    }
    /// <summary>
    /// 接受到动画事件
    /// </summary>
    /// <param name="mecevent"></param>
    private void RecieveAnimEvent(MecanimEvent mecevent)
    {
        if (mecevent == null)
            return;
    }


}


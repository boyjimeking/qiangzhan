using UnityEngine;
using System.Collections;

public class AIState  
{
    protected CommonAI mAI = null;

    private int mGoalQueuePri = 0;
    protected AIGoalCompositeGoal mGoalQueue;

    private uint mThinkInterval = uint.MaxValue;


    public AIState(CommonAI ai)
    {
        mAI = ai;
        mGoalQueue = new AIGoalCompositeGoal(mAI, false);
    }

    public virtual bool Update(uint elapsed)
    {
        mThinkInterval += elapsed;
        if (mThinkInterval > 100)
        {
            OnUpdate(mThinkInterval);
            mThinkInterval = 0;
        }

        if(!mGoalQueue.Process(elapsed))
        {
            mGoalQueuePri = 0;
        }

        return true;
    }

    public virtual void Enter()
    {
        mThinkInterval = uint.MaxValue;
        OnEnter();
    }

    public virtual void Exit()
    {
        OnExit();
    }

    protected virtual void OnEnter()
    {
    }

    protected virtual void OnExit()
    {

    }

    protected virtual void OnUpdate(uint elapsed)
    {
    }

    public bool BeginCommand(int priority)
    {
        if (priority <= mGoalQueuePri)
            return false;

        mGoalQueuePri = priority;
        mGoalQueue.RemoveAllSubGoals();
        return true;
    }

    public void AddCommand(AIGoal goal)
    {
        if (goal == null)
            return;

        mGoalQueue.AddSubGoal(goal);
        return;
    }

    public void RemoveCommand()
    {
        if (mGoalQueue == null)
            return;

        mGoalQueue.RemoveAllSubGoals();
    }

    public bool EmptyCommand()
    {
        if (mGoalQueue == null)
            return true;

        return mGoalQueue.IsEmpty();
    }

    public void SetGoalQueuePri(int pri)
    {
        mGoalQueuePri = pri;
    }
};

public class AIStateIdle : AIState
{
    public AIStateIdle(CommonAI ai) : base(ai)
    {
    }

    protected override void OnEnter()
    {
        mAI.OnEnterIdle();
    }

    protected override void OnExit()
    {
        mAI.OnExitIdle();
    }

    protected override void OnUpdate(uint elapsed)
    {
        mAI.OnUpdateIdle(elapsed);
    }
};

public class AIStateGoHome : AIState
{
    public AIStateGoHome(CommonAI ai) : base(ai)
    {
    }

    public override void Enter()
    {
        Vector3f combatStartPos = mAI.GetCombatStartPosition();

        BaseAI.SetMoveMode(mAI.GetID(), BaseAI.MoveMode.MOVE_SCURRY);
        BaseAI.MoveTo(mAI.GetID(), combatStartPos);

        base.Enter();
    }

    public override void Exit()
    {
        if (BaseAI.isMoving(mAI.GetID()))
        {
            BaseAI.StopMove(mAI.GetID());
        }
        base.Exit();
    }

    public override bool Update(uint elapsed)
    {
        Vector3f combatStartPos = mAI.GetCombatStartPosition();

        Vector3f pos = BaseAI.GetPosition(mAI.GetID());

        if(System.Math.Abs(pos.x - combatStartPos.x) < 0.1f && System.Math.Abs(pos.z - combatStartPos.z) < 0.1f)
        {
            BaseAI.StopMove(mAI.GetID());
            return false;
        }
        else if(!BaseAI.isMoving(mAI.GetID()))
        {
            BaseAI.SetMoveMode(mAI.GetID(), BaseAI.MoveMode.MOVE_SCURRY);
            return BaseAI.MoveTo(mAI.GetID(), combatStartPos);
        }

        return base.Update(elapsed);
    }
  
};

public class AIStateCombat : AIState
{
    public AIStateCombat(CommonAI ai) : base(ai)
    {
    }

    public override void Enter()
    {
        mGoalQueue.RemoveAllSubGoals();
        base.Enter();
    }

    public override void Exit()
    {
        mGoalQueue.RemoveAllSubGoals();
        base.Exit();
    }

    protected override void OnEnter()
    {
        mAI.OnEnterCombat();
    }

    protected override void OnExit()
    {
        mAI.OnExitCombat();
    }

    protected override void OnUpdate(uint elapsed)
    {
        mAI.OnUpdateCombat(elapsed);
    }
};

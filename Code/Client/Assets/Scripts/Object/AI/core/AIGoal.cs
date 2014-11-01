
using System;
using System.Collections;
using System.Collections.Generic;

public class AIGoal
{
    protected BaseAI mAI = null;

    public AIGoal(BaseAI ai)
    {
        mAI = ai;
    }

    public virtual bool Activate()
    {
        return true;
    }

    public virtual bool Process(uint elapsed)
    {
        return false;
    }

    public virtual void Terminate()
    {
    }
};


public class AIGoalCompositeGoal : AIGoal
{
    private bool mLoop = false;
    private List<AIGoal> mSubGoals = new List<AIGoal>();
    private int mIndex = 0;
    private int mLastIndex = -1;

    public AIGoalCompositeGoal(BaseAI ai, bool loop) : base(ai)
    {
        mAI = ai;
        mLoop = loop;
    }

    public override bool Activate()
    {
        mIndex = 0;
        mLastIndex = -1;
        return true;
    }

    public override bool Process(uint elapsed)
    {
        if (mIndex >= mSubGoals.Count)
        {
            mIndex = 0;
            mLastIndex = -1;

            if (!mLoop)
            {
                mSubGoals.Clear();
                return false;
            }
        }

        if (mIndex != mLastIndex)
        {
            mLastIndex = mIndex;
            if (!mSubGoals[mIndex].Activate())
            {
                mIndex++;
                return true;
            }
        }

        if (!mSubGoals[mIndex].Process(elapsed))
        {
            mSubGoals[mIndex].Terminate();
            mIndex++;
        }

        return true;
    }

    public override void Terminate()
    {
        for(int i = mIndex; i < mSubGoals.Count; i++)
        {
            mSubGoals[i].Terminate();
        }

        mIndex = 0;
        mLastIndex = -1;
        base.Terminate();
    }

    public void AddSubGoal(AIGoal goal)
    {
        if(goal == null)
            return;

        mSubGoals.Add(goal);
    }

    public void RemoveAllSubGoals()
    {
        mIndex = 0;
        mLastIndex = -1;

        mSubGoals.Clear();
    }

    public bool IsEmpty()
    {
        return mSubGoals.Count == 0;
    }

    public bool IsLoop()
    {
        return mLoop;
    }

    public void setLoop(bool loop)
    {
        mLoop = loop;
    }
};

public class AIGoalWalk : AIGoal
{
    public Vector3f mTargetPosition = new Vector3f();

    public AIGoalWalk(BaseAI ai, Vector3f tar) : base(ai)
    {
        mTargetPosition.x = tar.x;
        mTargetPosition.y = tar.y;
        mTargetPosition.z = tar.z;
    }

    public override bool Activate()
    {
        Vector3f movTar = BaseAI.GetMoveTargetPosition(mAI.GetID());
        if(movTar.x != mTargetPosition.x || movTar.z != mTargetPosition.z)
        {
            BaseAI.SetMoveMode(mAI.GetID(), BaseAI.MoveMode.MOVE_WALK);
            if(!BaseAI.MoveTo(mAI.GetID(), mTargetPosition))
                return false;
        }

        return true;
    }

    public override bool Process(uint elapsed)
    {
        Vector3f curPosition = BaseAI.GetPosition(mAI.GetID());

		if ((Math.Abs(curPosition.x - mTargetPosition.x) <= 0.001) && (Math.Abs(curPosition.z - mTargetPosition.z) <= 0.001))
		{
            BaseAI.StopMove(mAI.GetID());
			return false;
		}

		Vector3f movTar = BaseAI.GetMoveTargetPosition(mAI.GetID());
        if(movTar.x != mTargetPosition.x || movTar.z != mTargetPosition.z)
        {
            BaseAI.SetMoveMode(mAI.GetID(), BaseAI.MoveMode.MOVE_WALK);
            if(!BaseAI.MoveTo(mAI.GetID(), mTargetPosition))
                return false;
        }

		return true;
    }

    public override void Terminate()
    {
        //BaseAI.StopMove(mAI.GetID());
    }

};


public class AIGoalRun : AIGoal
{
    public Vector3f mTargetPosition = new Vector3f();

    public AIGoalRun(BaseAI ai, Vector3f tar) : base(ai)
    {
        mTargetPosition.x = tar.x;
        mTargetPosition.y = tar.y;
        mTargetPosition.z = tar.z;
    }

    public override bool Activate()
    {
        Vector3f movTar = BaseAI.GetMoveTargetPosition(mAI.GetID());
        if(movTar.x != mTargetPosition.x || movTar.z != mTargetPosition.z)
        {
            BaseAI.SetMoveMode(mAI.GetID(), BaseAI.MoveMode.MOVE_RUN);
            if(!BaseAI.MoveTo(mAI.GetID(), mTargetPosition))
                return false;
        }

        return true;
    }

    public override bool Process(uint elapsed)
    {
        Vector3f curPosition = BaseAI.GetPosition(mAI.GetID());

		if ((Math.Abs(curPosition.x - mTargetPosition.x) <= 0.001f) && (Math.Abs(curPosition.z - mTargetPosition.z) <= 0.001f))
		{
            BaseAI.StopMove(mAI.GetID());
			return false;
		}

		Vector3f movTar = BaseAI.GetMoveTargetPosition(mAI.GetID());
        if(movTar.x != mTargetPosition.x || movTar.z != mTargetPosition.z)
        {
            BaseAI.SetMoveMode(mAI.GetID(), BaseAI.MoveMode.MOVE_RUN);
            if(!BaseAI.MoveTo(mAI.GetID(), mTargetPosition))
                return false;
        }

		return true;
    }

    public override void Terminate()
    {
        //BaseAI.StopMove(mAI.GetID());
    }

};

public class AIGoalMoveTo : AIGoal
{
    public Vector3f mTargetPosition = new Vector3f();
    public BaseAI.MoveMode mMoveMode;

    public AIGoalMoveTo(BaseAI ai, Vector3f tar, BaseAI.MoveMode moveMode)
        : base(ai)
    {
        mTargetPosition.x = tar.x;
        mTargetPosition.y = tar.y;
        mTargetPosition.z = tar.z;

        mMoveMode = moveMode;
    }

    public override bool Activate()
    {
        Vector3f movTar = BaseAI.GetMoveTargetPosition(mAI.GetID());
        if (movTar.x != mTargetPosition.x || movTar.z != mTargetPosition.z)
        {
            BaseAI.SetMoveMode(mAI.GetID(), mMoveMode);
            if (!BaseAI.MoveTo(mAI.GetID(), mTargetPosition))
                return false;
        }

        return true;
    }

    public override bool Process(uint elapsed)
    {
        Vector3f curPosition = BaseAI.GetPosition(mAI.GetID());

        if ((Math.Abs(curPosition.x - mTargetPosition.x) <= 0.001) && (Math.Abs(curPosition.z - mTargetPosition.z) <= 0.001))
        {
            BaseAI.StopMove(mAI.GetID());
            return false;
        }

        Vector3f movTar = BaseAI.GetMoveTargetPosition(mAI.GetID());
        if (movTar.x != mTargetPosition.x || movTar.z != mTargetPosition.z)
        {
            BaseAI.SetMoveMode(mAI.GetID(), BaseAI.MoveMode.MOVE_RUN);
            if (!BaseAI.MoveTo(mAI.GetID(), mTargetPosition))
                return false;
        }

        return true;
    }

    public override void Terminate()
    {
        //BaseAI.StopMove(mAI.GetID());
    }

};

public class AIGoalMoveTlimit : AIGoal
{
    public uint mMoveTime = 0;
    public uint mTimer = 0;
    public BaseAI.MoveMode mMoveMode;
    public uint mTargetId = 0;	
    public Vector3f posm = new Vector3f();
    public Vector3f posn = new Vector3f();

    private float mSkillMinRangle = 2.0f;
    private float mSkillMaxRangle = 3.0f;

    public AIGoalMoveTlimit(BaseAI ai, uint targetId, uint tMoveTime, BaseAI.MoveMode mMoveMode)
        : base(ai)
    {
        this.mMoveTime = tMoveTime;
        this.mMoveMode = mMoveMode;
        this.mTargetId = targetId;
    }

    public override bool Activate()
    {
        mTimer = mMoveTime;

        if (!BaseAI.IsValid(mTargetId))
            return false;

        float curangle = BaseAI.get_direction(mAI.GetID(),mTargetId);
        float randRange = BaseAI.Randomf(mSkillMinRangle, mSkillMaxRangle);

        Vector3f finalPos = BaseAI.get_position_angle_and_distance_position(posm, curangle, randRange);
        BaseAI.SetMoveMode(mAI.GetID(), mMoveMode);
        BaseAI.MoveTo(mAI.GetID(), finalPos);
        BaseAI.SetMoveMode(mAI.GetID(), mMoveMode);

        return true;
    }

    public override bool Process(uint elapsed)
    {
        if (mTimer > elapsed)
            mTimer -= elapsed;
        else
            mTimer = 0;

        if (mTimer <= 0)
            return false;

        if (!BaseAI.IsValid(mTargetId))
            return false;

        posm = BaseAI.GetPosition(mTargetId);
        posn = BaseAI.GetPosition(mAI.GetID());

        float curangle = BaseAI.get_direction(mAI.GetID(),mTargetId);
        float randRange = BaseAI.Randomf(mSkillMinRangle, mSkillMaxRangle);

        Vector3f finalPos = BaseAI.get_position_angle_and_distance_position(posm, curangle, randRange);

        if (posm.Subtract(posn).Length() < 5.0f)
        {
            int ranAngle1 = BaseAI.Random(-60, -30);
            int ranAngle2 = BaseAI.Random(30, 60);
            int ranAngle = BaseAI.Random(1, 3) == 1 ? ranAngle1 : ranAngle2;
            float angleRadius = (float)(ranAngle * System.Math.PI / 180.0f);
            curangle = curangle + angleRadius;
            finalPos = BaseAI.get_position_angle_and_distance_position(posn, curangle, randRange);
        }

        if (!BaseAI.isMoving(mAI.GetID()))
        {
            if (!BaseAI.MoveTo(mAI.GetID(), finalPos))
                return false;
        }

        return true;	
    }

    public override void Terminate()
    {
        //BaseAI.StopMove(mAI.GetID());
    }
}

public class AIGoalWait : AIGoal
{
    public uint mWaitTime = 0;
    public uint mTimer = 0;
    public AIGoalWait(BaseAI ai, uint waitTime) : base(ai)
    {
        mWaitTime = waitTime;
    }

    public override bool Activate()
    {
        mTimer = mWaitTime;
        return true;
    }

    public override bool Process(uint elapsed)
    {
        if (mTimer > elapsed)
            mTimer -= elapsed;
        else
            mTimer = 0;

        return mTimer > 0 ? true : false;
    }

    public override void Terminate()
    {
    }
};


public class AIGoalUseSkillToTarget : AIGoal
{
    public uint mTargetId = uint.MaxValue;
    public int mSkillId = -1;

    public AIGoalUseSkillToTarget(BaseAI ai, uint targetId, int skillId)
        : base(ai)
    {
        mTargetId = targetId;
        mSkillId = skillId;
    }

    public override bool Activate()
    {
        Vector3f cur_position = BaseAI.GetPosition(mAI.GetID());
        Vector3f target_position = BaseAI.GetPosition(mTargetId);
        if (BaseAI.SceneMayStraightReach(cur_position, target_position))
            return BaseAI.UseSkillToTarget(mAI.GetID(), mSkillId, mTargetId);
        else
            return false;
    }

    public override bool Process(uint elapsed)
    {
      return BaseAI.IsUseSkill(mAI.GetID()) == true;
    }

    public override void Terminate()
    {
        BaseAI.StopAllSkill(mAI.GetID());
    }
};

public class AIGoalUseSkillToPosition : AIGoal
{
    public int mSkillId = -1;
    public Vector3f mTarPosition = new Vector3f();

    public AIGoalUseSkillToPosition(BaseAI ai, int skillId, Vector3f tarPos) : base(ai)
    {
        mTarPosition.x = tarPos.x;
        mTarPosition.y = tarPos.y;
        mTarPosition.z = tarPos.z;
        this.mSkillId = skillId;
    }

    public override bool Activate()
    {
        BaseAI.StopMove(mAI.GetID());
        Vector3f cur_position = BaseAI.GetPosition(mAI.GetID());
        if (BaseAI.SceneMayStraightReach(cur_position, mTarPosition))
            return BaseAI.UseSkillToPosition(mAI.GetID(), mSkillId, mTarPosition);
        else
            return false;
    }

    public override bool Process(uint elapsed)
    {
      return BaseAI.IsUseSkill(mAI.GetID()) == true;
    }

    public override void Terminate()
    {
        BaseAI.StopAllSkill(mAI.GetID());
    }
};

public class AIGoalUseSkillToDirection : AIGoal
{
    public int mSkillId = -1;
    public float mDirection = 0;

    public AIGoalUseSkillToDirection(BaseAI ai, int skillId, float dir) : base(ai)
    {
        mSkillId    = skillId;
        mDirection  = dir;
    }


    public override bool Activate()
    {
        BaseAI.StopMove(mAI.GetID());
        return BaseAI.UseSkillToDirection(mAI.GetID(), mSkillId, mDirection);
    }

    public override bool Process(uint elapsed)
    {
      return BaseAI.IsUseSkill(mAI.GetID()) == true;
    }

    public override void Terminate()
    {
        BaseAI.StopAllSkill(mAI.GetID());
    }
};

public class AIGoalSay : AIGoal
{
    public int mTalkId = -1;

    public AIGoalSay(BaseAI ai, int talkId) : base(ai)
    {
        mTalkId = talkId;
    }

    public override bool Activate()
    {
        return BaseAI.Say(mAI.GetID(), mTalkId);
    }

    public override bool Process(uint elapsed)
    {
      return false;
    }

    public override void Terminate()
    {
    }
};

public class AIGoalUseSkillToTargetRangeTime : AIGoal
{
    public uint mTargetId;
    public int mSkillId;
    public BaseAI.MoveMode mMoveMode;

    private float mSkillMinRangle;
    private float mSkillMaxRangle;
    private float mTimer = 0;
    private bool mIsSkill = false;

    public AIGoalUseSkillToTargetRangeTime(BaseAI ai, uint targetId, int skillId, BaseAI.MoveMode moveMode, uint limit_millisecond)
        : base(ai)
    {
        mTargetId = targetId;
        mSkillId = skillId;
        mMoveMode = moveMode;
        mTimer = limit_millisecond;

        mSkillMinRangle = BaseAI.GetSkillMinRangle(mSkillId);
        mSkillMaxRangle = BaseAI.GetSkillMaxRangle(mSkillId);
    }

    public override bool Activate()
    {
        if (!BaseAI.IsValid(mTargetId))
            return false;

        Vector3f cur_position = BaseAI.GetPosition(mAI.GetID());
		Vector3f target_position = BaseAI.GetPosition(mTargetId);
			
		if(BaseAI.IsUseSkill(mAI.GetID()))
			return false;
			
		float x = cur_position.x - target_position.x;
		float z = cur_position.z - target_position.z;
			
		float dist = (float)System.Math.Sqrt(x * x + z * z);
		if (dist <=0.5) 
        {
			float curangle = BaseAI.get_direction(mTargetId, mAI.GetID());	
			float randRange= BaseAI.Randomf(mSkillMinRangle, mSkillMaxRangle);		
				
			if(mSkillMaxRangle - mSkillMinRangle >=3) 
                randRange = mSkillMinRangle;
								
			Vector3f finalPos = BaseAI.get_position_angle_and_distance_position(target_position,curangle,randRange);

            BaseAI.SetMoveMode(mAI.GetID(), mMoveMode);
				
			BaseAI.MoveTo(mAI.GetID(), finalPos);
			
		}

		BaseAI.SetMoveMode(mAI.GetID(), mMoveMode);

        return true;
    }

    public override bool Process(uint elapsed)
    {
        if(mTimer > elapsed)
            mTimer -= elapsed;
        else
            return false;

        if (!BaseAI.IsValid(mTargetId))
            return false;

		Vector3f final_position = BaseAI.GetMoveTargetPosition(mAI.GetID());
			
		Vector3f cur_position = BaseAI.GetPosition(mAI.GetID());
			
		Vector3f target_position = BaseAI.GetPosition(mTargetId);
			
		float curangle = BaseAI.get_direction(mTargetId, mAI.GetID());	
		float randRange= BaseAI.Randomf(mSkillMinRangle, mSkillMaxRangle);		
			
		if (mSkillMaxRangle - mSkillMinRangle >=3) 
        {
			//不要用最大射程值取这个点，补个0.5，因为浮点精度原因，算出的点可能还在射程外，
			//目标不动则始终停留该点，也无法射击
			randRange = mSkillMaxRangle - 0.5f;
		}
											
		Vector3f finalPos=BaseAI.get_position_angle_and_distance_position(target_position,curangle,randRange);
			
		float x = cur_position.x - target_position.x;
		float z = cur_position.z - target_position.z;
		float dist = (float)System.Math.Sqrt(x * x + z * z);			
			
		if(mIsSkill)
			return BaseAI.IsUseSkill(mAI.GetID()) == true;
			
		if (dist <= mSkillMaxRangle)
        {
            if (BaseAI.SceneMayStraightReach(cur_position, target_position))
            {
                BaseAI.StopMove(mAI.GetID());
                BaseAI.LookAt(mAI.GetID(), target_position);
                BaseAI.UseSkillToTarget(mAI.GetID(), mSkillId, mTargetId);
                mIsSkill = true;
                return true;
            }
            else
            {
                if (!BaseAI.isMoving(mAI.GetID()))
                {
                    if (!BaseAI.MoveTo(mAI.GetID(), target_position))
                        return false;
                }
            }
		}
			
		x = final_position.x - target_position.x;
		z = final_position.z - target_position.z;			
		dist = (float)System.Math.Sqrt(x * x + z * z);
		if (dist > mSkillMaxRangle || !BaseAI.isMoving(mAI.GetID()))
        {
			if(!BaseAI.MoveTo(mAI.GetID(), finalPos))
				return false;
		}	
		return true;	
    }

    public override void Terminate()
    {
        //BaseAI.StopMove(mAI.GetID());
    }

};

public class AIGoalApproachTarget : AIGoal
{
    public uint mTargetId = 0xFFFFFFFF;
    public float mDistance = 0;
    public BaseAI.MoveMode mMoveMode = BaseAI.MoveMode.MOVE_RUN;

    public AIGoalApproachTarget(BaseAI ai, uint targetId, float distance) : base(ai)
    {
        mTargetId = targetId;
        mDistance = distance;
    }

    public void setMoveMode(BaseAI.MoveMode mMoveMode)
    {
        this.mMoveMode = mMoveMode;
    }

    public override bool Activate()
    {
        if (!BaseAI.IsValid(mTargetId))
            return false;

        Vector3f final_position = BaseAI.GetMoveTargetPosition(mAI.GetID());
			
		Vector3f target_position = BaseAI.GetPosition(mTargetId);
			
		float x = final_position.x - target_position.x;
		float z = final_position.z - target_position.z;
			
		float dist = (float)Math.Sqrt(x * x + z * z);
		if (dist > mDistance)
        {
			BaseAI.SetMoveMode(mAI.GetID(), mMoveMode);
			BaseAI.MoveTo(mAI.GetID(), target_position);	
        }

        return true;
    }

    public override bool Process(uint elapsed)
    {
        if (!BaseAI.IsValid(mTargetId))
            return false;

        Vector3f final_position = BaseAI.GetMoveTargetPosition(mAI.GetID());
			
		Vector3f cur_position = BaseAI.GetPosition(mAI.GetID());
			
		Vector3f target_position = BaseAI.GetPosition(mTargetId);

			
		float x = cur_position.x - target_position.x;
		float z = cur_position.z - target_position.z;
		float dist = (float)Math.Sqrt(x * x + z * z);
		if (dist < mDistance)
        {
            if(!BaseAI.SceneMayStraightReach(cur_position, target_position))
            {
                BaseAI.SetMoveMode(mAI.GetID(), mMoveMode);
                if(!BaseAI.MoveTo(mAI.GetID(), target_position))
                    return false;
            }
            else
            {
                BaseAI.StopMove(mAI.GetID());
			    return false;
            }
        }
						
		x = final_position.x - target_position.x;
		z = final_position.z - target_position.z;			
		dist = (float)Math.Sqrt(x * x + z * z);
		if (dist > mDistance || !BaseAI.isMoving(mAI.GetID()))	
        {
            BaseAI.SetMoveMode(mAI.GetID(), mMoveMode);
			if (!BaseAI.MoveTo(mAI.GetID(), target_position))
				return false;				
        }
			
		return true;
    }

    public override void Terminate()
    {
        //BaseAI.StopMove(mAI.GetID());
    }
};

public class AIGoalApproachTargetLimit : AIGoal
{
    public uint mTargetId = 0xFFFFFFFF;
    public float mDistance = 0;
    public BaseAI.MoveMode mMoveMode = BaseAI.MoveMode.MOVE_RUN;
    public uint mMaxTime = 0;
    public uint mTimer = 0;

    public AIGoalApproachTargetLimit(BaseAI ai, uint targetId, float distance, BaseAI.MoveMode moveMode, uint maxTime) : base(ai)
    {
        mTargetId = targetId;
        mDistance = distance;
        mMoveMode = moveMode;
        mMaxTime = maxTime;
    }

    public override bool Activate()
    {
        if (!BaseAI.IsValid(mTargetId))
            return false;

        mTimer = mMaxTime;

        Vector3f final_position = BaseAI.GetMoveTargetPosition(mAI.GetID());
			
		Vector3f target_position = BaseAI.GetPosition(mTargetId);
			
		float x = final_position.x - target_position.x;
		float z = final_position.z - target_position.z;
			
		float dist = (float)Math.Sqrt(x * x + z * z);
		if (dist > mDistance)
        {
			BaseAI.SetMoveMode(mAI.GetID(), mMoveMode);
            if (!BaseAI.MoveTo(mAI.GetID(), target_position))
                return false;
        }

        return true;
    }

    public override bool Process(uint elapsed)
    {
        if (mTimer > elapsed)
            mTimer -= elapsed;
        else
            return false;

        if (!BaseAI.IsValid(mTargetId))
            return false;

        Vector3f final_position = BaseAI.GetMoveTargetPosition(mAI.GetID());
			
		Vector3f cur_position = BaseAI.GetPosition(mAI.GetID());
			
		Vector3f target_position = BaseAI.GetPosition(mTargetId);
			
		float x = cur_position.x - target_position.x;
		float z = cur_position.z - target_position.z;
		float dist = (float)Math.Sqrt(x * x + z * z);
		if (dist < mDistance)
        {
            if(!BaseAI.SceneMayStraightReach(cur_position, target_position))
            {
                BaseAI.SetMoveMode(mAI.GetID(), mMoveMode);
                if(!BaseAI.MoveTo(mAI.GetID(), target_position))
                    return false;
            }
            else
            {
			    return false;
            }
        }
						
		x = final_position.x - target_position.x;
		z = final_position.z - target_position.z;			
		dist = (float)Math.Sqrt(x * x + z * z);
		if (dist > mDistance || !BaseAI.isMoving(mAI.GetID()))	
        {
            BaseAI.SetMoveMode(mAI.GetID(), mMoveMode);
			if (!BaseAI.MoveTo(mAI.GetID(), target_position))
				return false;				
        }
			
		return true;
    }

    public override void Terminate()
    {
        //BaseAI.StopMove(mAI.GetID());
    }
};

public class AIGoalApproachPosition : AIGoal
{
    public Vector3f mTarPos = new Vector3f();
    public BaseAI.MoveMode mMoveMode = BaseAI.MoveMode.MOVE_RUN;
    public float mDistance = 0;

    public AIGoalApproachPosition(BaseAI ai, Vector3f tarPos, BaseAI.MoveMode moveMode, float distance) : base(ai)
    {
        mTarPos = tarPos;
        mMoveMode = moveMode;
        mDistance = distance;
    }

    public override bool Activate()
    {
        Vector3f final_position = BaseAI.GetMoveTargetPosition(mAI.GetID());
		Vector3f target_position = mTarPos;
			
		float x = final_position.x - target_position.x;
		float z = final_position.z - target_position.z;
			
		float dist = (float)Math.Sqrt(x * x + z * z);
		if (dist > mDistance)
        {
			BaseAI.SetMoveMode(mAI.GetID(), mMoveMode);
			BaseAI.MoveTo(mAI.GetID(), target_position);				
        }
		
        return true;
    }

    public override bool Process(uint elapsed)
    {
        Vector3f final_position = BaseAI.GetMoveTargetPosition(mAI.GetID());
			
		Vector3f cur_position = BaseAI.GetPosition(mAI.GetID());
			
		Vector3f target_position = mTarPos;
			
		float x = cur_position.x - target_position.x;
		float z = cur_position.z - target_position.z;
		float dist = (float)Math.Sqrt(x * x + z * z);
				
		if (dist < mDistance)
			return false;
				
		x = final_position.x - target_position.x;
		z = final_position.z - target_position.z;			
		dist = (float)Math.Sqrt(x * x + z * z);
		if (dist > mDistance || !BaseAI.isMoving(mAI.GetID()))
        {		
			BaseAI.SetMoveMode(mAI.GetID(), mMoveMode);
			if (!BaseAI.MoveTo(mAI.GetID(), target_position))
				return false;
        }
									
		return true;				
    }

    public override void Terminate()
    {
        //BaseAI.StopMove(mAI.GetID());
    }

};

public class AIGoalRandMove : AIGoal
{
    public Vector3f mCenter = new Vector3f();
	public float    mMaxRadius = 0;
	public float    mMinRadius = 0;
	public float    mMaxAngle	= 0;
	public float    mMinAngle  = 0;
	public BaseAI.MoveMode mMovemode = BaseAI.MoveMode.MOVE_RUN;

    public AIGoalRandMove(BaseAI ai, Vector3f center, float minRadius, float maxRadius, float minAngle, float maxAngle)
        : base(ai)
    {
        mCenter.x = center.x;
        mCenter.y = center.y;
        mCenter.z = center.z;

        mMaxRadius = maxRadius;
        mMinRadius = minRadius;
        mMaxAngle = maxAngle;
        mMinAngle = minAngle;
    }

    public override bool Activate()
    {		
		Vector3f pos = BaseAI.GetPosition(mAI.GetID());

		float dirx = mCenter.x - pos.x;
		float dirz = mCenter.z - pos.z;
		if (dirx == 0.0 && dirz == 0.0) 
        {
			dirx = BaseAI.Randomf(0.0f, 1.0f);
            dirz = BaseAI.Randomf(0.0f, 1.0f);
		}
			
		float dirlength = (float)Math.Sqrt(dirx * dirx + dirz * dirz);	
		if (dirlength <= 0.0) 
			return false;
						
		dirx = dirx / dirlength;
		dirz = dirz / dirlength;
		dirlength = 1;

		float [] dir = new float[2]{0, 0};
		for(int i = 0; i < 10; i++)
        {
			float rad = (float)(BaseAI.Randomf(mMinAngle, mMaxAngle) * Math.PI / 180.0f);
			if (BaseAI.Random(0, 100) > 50)
            {
				rad = rad * -1;
            }
				
			dir[0] = (float)(dirx * Math.Cos(rad) - dirz * Math.Sin(rad));
			dir[1] = (float)(dirz * Math.Cos(rad) + dirx * Math.Sin(rad));

            float move_radius = (float)BaseAI.Randomf(mMinRadius, mMaxRadius);
				
			Vector3f move_pos = new Vector3f();
			move_pos.x = mCenter.x + dir[0] * move_radius;
			move_pos.z = mCenter.z + dir[1] * move_radius;
				
			BaseAI.SetMoveMode(mAI.GetID(), mMovemode);
            if (BaseAI.MoveTo(mAI.GetID(), move_pos))
                return true;
        }

        return false;

    }

    public override bool Process(uint elapsed)
    {
        return BaseAI.isMoving(mAI.GetID());
    }

    public override void Terminate()
    {
        //BaseAI.StopMove(mAI.GetID());
    }

};

public class AIGoalRandMoveTime : AIGoal
{
    public Vector3f mCenter = new Vector3f();
    public float mMaxRadius = 0;
    public float mMinRadius = 0;
    public float mMaxAngle = 0;
    public float mMinAngle = 0;
    public BaseAI.MoveMode mMovemode = BaseAI.MoveMode.MOVE_RUN;
    public uint mMaxTime = 0;
    public uint mTimer = 0;

    public AIGoalRandMoveTime(BaseAI ai, Vector3f center, float minRadius, float maxRadius, float minAngle, float maxAngle, uint maxTime)
        : base(ai)
    {
        mCenter.x = center.x;
        mCenter.y = center.y;
        mCenter.z = center.z;

        mMaxRadius = maxRadius;
        mMinRadius = minRadius;
        mMaxAngle = maxAngle;
        mMinAngle = minAngle;
        mMaxTime = maxTime;
    }

    public override bool Activate()
    {
        Vector3f pos = BaseAI.GetPosition(mAI.GetID());

        mTimer = mMaxTime;

        float dirx = pos.x - mCenter.x;
        float dirz = pos.z - mCenter.z;
        if (dirx == 0.0 && dirz == 0.0)
        {
            dirx = BaseAI.Randomf(0.0f, 1.0f);
            dirz = BaseAI.Randomf(0.0f, 1.0f);
        }

        float dirlength = (float)Math.Sqrt(dirx * dirx + dirz * dirz);
        if (dirlength <= 0.0)
            return false;

        dirx = dirx / dirlength;
        dirz = dirz / dirlength;
        dirlength = 1;

        float[] dir = new float[2] { 0, 0 };

        float rad = (float)(BaseAI.Randomf(mMinAngle, mMaxAngle) * Math.PI / 180.0f);
        if (BaseAI.Random(0, 100) > 50)
        {
            rad = rad * -1;
        }

        dir[0] = (float)(dirx * Math.Cos(rad) - dirz * Math.Sin(rad));
        dir[1] = (float)(dirz * Math.Cos(rad) + dirx * Math.Sin(rad));

        float move_radius = (float)BaseAI.Randomf(mMinRadius, mMaxRadius);

        Vector3f move_pos = new Vector3f();
        move_pos.x = mCenter.x + dir[0] * move_radius;
        move_pos.z = mCenter.z + dir[1] * move_radius;

        BaseAI.SetMoveMode(mAI.GetID(), mMovemode);
        if (BaseAI.MoveTo(mAI.GetID(), move_pos))
            return true;

        return false;

    }

    public override bool Process(uint elapsed)
    {
        if (mTimer > elapsed)
            mTimer -= elapsed;
        else
            return false;

        Vector3f pos = BaseAI.GetPosition(mAI.GetID());

        float dirx = pos.x - mCenter.x;
        float dirz = pos.z - mCenter.z;
        if (dirx == 0.0 && dirz == 0.0)
        {
            dirx = BaseAI.Randomf(0.0f, 1.0f);
            dirz = BaseAI.Randomf(0.0f, 1.0f);
        }

        float dirlength = (float)Math.Sqrt(dirx * dirx + dirz * dirz);
        if (dirlength <= 0.0)
            dirlength = 1;

        dirx = dirx / dirlength;
        dirz = dirz / dirlength;
        dirlength = 1;

        float[] dir = new float[2] { 0, 0 };

        float rad = (float)(BaseAI.Randomf(mMinAngle, mMaxAngle) * Math.PI / 180.0f);
        if (BaseAI.Random(0, 100) > 50)
        {
            rad = rad * -1;
        }

        dir[0] = (float)(dirx * Math.Cos(rad) - dirz * Math.Sin(rad));
        dir[1] = (float)(dirz * Math.Cos(rad) + dirx * Math.Sin(rad));

        float move_radius = (float)BaseAI.Randomf(mMinRadius, mMaxRadius);

        Vector3f move_pos = new Vector3f();
        move_pos.x = mCenter.x + dir[0] * move_radius;
        move_pos.z = mCenter.z + dir[1] * move_radius;


        if (!BaseAI.isMoving(mAI.GetID()))
        {
            BaseAI.SetMoveMode(mAI.GetID(), mMovemode);
            if (BaseAI.MoveTo(mAI.GetID(), move_pos))
                return true;
        }

        return true;
    }

    public override void Terminate()
    {
        //BaseAI.StopMove(mAI.GetID());
    }

};

public class AIGoalStand : AIGoal
{
    public uint mMinWaitMillisecond = 0;
    public uint mMaxWaitMillisecond = 0;
    private uint mTimer = 0;
    public AIGoalStand(BaseAI ai, uint standMinMillisecond, uint standMaxMillisecond)
        : base(ai)
    {
        mMinWaitMillisecond = standMinMillisecond;
        mMaxWaitMillisecond = standMaxMillisecond;
    }

    public override bool Activate()
    {
        System.Random rand = new System.Random(System.DateTime.Now.Millisecond);

        mTimer = (uint)rand.Next((int)mMinWaitMillisecond, (int)mMaxWaitMillisecond); 
        BaseAI.StopMove(mAI.GetID());
        return true;
    }

    public override bool Process(uint elapsed)
    {
        if (mTimer > elapsed)
       {
           mTimer -= elapsed;
           return true;
       }

       return false;
    }

    public override void Terminate()
    {

    }
};

public class AIGoalDestorySelf : AIGoal
{
    public AIGoalDestorySelf(BaseAI ai)
        : base(ai)
    {

    }

    public override bool Activate()
    {
        BaseAI.DestoryObject(mAI.GetID());
        return true;
    }

    public override bool Process(uint elapsed)
    {
       return false;
    }

    public override void Terminate()
    {

    }
};

public class AIGoalLookAtTarget : AIGoal
{
    public uint mWaitMillisecond = 0;
    public uint mTargetId = 0xFFFFFFFF;

    public AIGoalLookAtTarget(BaseAI ai, uint tarId, uint millisecond)
        : base(ai)
    {
        mTargetId = tarId;
        mWaitMillisecond = millisecond;
    }

    public override bool Activate()
    {
        if (!BaseAI.IsValid(mTargetId))
            return false;

        Vector3f pos = BaseAI.GetPosition(mTargetId);

		BaseAI.StopMove(mAI.GetID());	
		BaseAI.LookAt(mAI.GetID(), pos);

        return true;
    }

    public override bool Process(uint elapsed)
    {
        if(mWaitMillisecond < elapsed)
            return false;
        
        mWaitMillisecond -= elapsed;

        if (!BaseAI.IsValid(mTargetId))
            return false;

        Vector3f pos = BaseAI.GetPosition(mTargetId);

		BaseAI.LookAt(mAI.GetID(), pos);
        return true;
    }

    public override void Terminate()
    {

    }
};

public class AIGoalLookAtPosition : AIGoal
{
    public uint mWaitMillisecond = 0;
    public Vector3f mTarPosition = new Vector3f();

    public AIGoalLookAtPosition(BaseAI ai, Vector3f tarPos, uint millisecond)
        : base(ai)
    {
        mWaitMillisecond = millisecond;
        mTarPosition.x = tarPos.x;
        mTarPosition.y = tarPos.y;
        mTarPosition.z = tarPos.z;
    }

    public override bool Activate()
    {
		BaseAI.StopMove(mAI.GetID());	
		BaseAI.LookAt(mAI.GetID(), mTarPosition);

        return true;
    }

    public override bool Process(uint elapsed)
    {
        if(mWaitMillisecond < elapsed)
            return false;
        
        mWaitMillisecond -= elapsed;

		BaseAI.LookAt(mAI.GetID(), mTarPosition);
        return true;
    }

    public override void Terminate()
    {

    }
};

public class AIGoalFlee : AIGoal
{
    public AIGoalFlee(BaseAI ai) : base(ai)
    {
    }

    public override bool Activate()
    {
        return true;
    }

    public override bool Process(uint elapsed)
    {
        return false;
    }

    public override void Terminate()
    {
    }
};

public class AIGoalTurnToTarget : AIGoal
{
    public uint mTarId = uint.MaxValue;

    public AIGoalTurnToTarget(BaseAI ai, uint tarId)
        : base(ai)
    {
        mTarId = tarId;
    }

    public override bool Activate()
    {
        if (!BaseAI.IsValid(mTarId))
            return false;

        Vector3f pos = BaseAI.GetPosition(mTarId);
        BaseAI.TurnTo(mAI.GetID(), pos);
        return true;
    }

    public override bool Process(uint elapsed)
    {
        return false;
    }

    public override void Terminate()
    {
    }
};

public class AIGoalTurn : AIGoal
{
    public float mAngle = 0.0f;

    public AIGoalTurn(BaseAI ai, float angle)
        : base(ai)
    {
        mAngle = angle;
    }

    public override bool Activate()
    {
        BaseAI.Turn(mAI.GetID(), mAngle);
        return true;
    }

    public override bool Process(uint elapsed)
    {
        return false;
    }

    public override void Terminate()
    {
    }
};

public class AIGoalTurnToPosition : AIGoal
{
    public Vector3f mPosition = new Vector3f();

    public AIGoalTurnToPosition(BaseAI ai, Vector3f tarPos)
        : base(ai)
    {
        mPosition.x = tarPos.x;
        mPosition.y = tarPos.y;
        mPosition.z = tarPos.z;
    }

    public override bool Activate()
    {
        BaseAI.TurnTo(mAI.GetID(), mPosition);
        return true;
    }

    public override bool Process(uint elapsed)
    {
        return false;
    }

    public override void Terminate()
    {
    }
};

public class AIGoalRandMoveRadius : AIGoal
{
    public float mMinRadius = 0.0f;
    public float mMaxRadius = 0.0f;

    public AIGoalRandMoveRadius(BaseAI ai, float minRadius, float maxRadius)
        : base(ai)
    {
        mMinRadius = minRadius;
        mMaxRadius = maxRadius;
    }

    public override bool Activate()
    {
        Vector3f pos = BaseAI.GetHomePosition(mAI.GetID());

        Random rand = new Random(System.DateTime.Now.Millisecond);

        for(int i = 0; i < 10; i++)
        {
            float distance = rand.Next((int)(mMinRadius * 100), (int)(mMaxRadius * 100)) / 100.0f;
            float rad = rand.Next((int)(0.0f), (int)(Math.PI * 200)) / 100.0f;

            pos.x += (float)(Math.Sin(rad) * distance);
            pos.z += (float)(Math.Cos(rad) * distance);

            if(BaseAI.MoveTo(mAI.GetID(), pos))
               return true; 
        }

        return false;
    }

    public override bool Process(uint elapsed)
    {
        return BaseAI.isMoving(mAI.GetID());
    }

    public override void Terminate()
    {
        //BaseAI.StopMove(mAI.GetID());
    }

};

public class AIGoalSkillToTargetRange : AIGoal
{
    public uint mTargetId;
    public int mSkillId;
    
    public BaseAI.MoveMode mMoveMode;

    private bool mIsSkill = false;
    private float mMinRangle = 0.0f;
    private float mMaxRangle = 10.0f;

    public AIGoalSkillToTargetRange(BaseAI ai, uint targetid, int skillid, BaseAI.MoveMode movemode)
        :base(ai)
    {
        mTargetId = targetid;
        mMinRangle = BaseAI.GetSkillMinRangle(skillid);
        mMaxRangle = BaseAI.GetSkillMaxRangle(skillid);
        mSkillId   = skillid;
        mMoveMode  = movemode;
    }

    public override bool Activate()
    {
        if (!BaseAI.IsValid(mTargetId))
            return false;

        Vector3f cur_position = BaseAI.GetPosition(mAI.GetID());
			
		Vector3f target_position = BaseAI.GetPosition(mTargetId);
			
		if(BaseAI.IsUseSkill(mAI.GetID()))
			return false;
	
		float x = cur_position.x - target_position.x;
		float z = cur_position.z - target_position.z;
			
		float dist = (float)System.Math.Sqrt((float)(x * x) + (float)(z * z));
		if(dist <= 0.5f)			
        {
			float curangle = BaseAI.CalcDirection(target_position, cur_position);	
			float randRange = BaseAI.Randomf(mMinRangle * 10, mMaxRangle * 10);		
			randRange = randRange / 10;
				
			if(mMaxRangle - mMinRangle >=3)
            {
				randRange = mMaxRangle;					
			}	
											
			Vector3f finalPos = BaseAI.get_position_angle_and_distance_position(target_position, curangle, randRange);

			BaseAI.SetMoveMode(mAI.GetID(), mMoveMode);
				
			BaseAI.MoveTo(mAI.GetID(), finalPos);
			
		}

        BaseAI.SetMoveMode(mAI.GetID(), mMoveMode);
        return true;
    }

    public override bool Process(uint elapsed)
    {
        if (!BaseAI.IsValid(mTargetId))
            return false;

        Vector3f final_position = BaseAI.GetMoveTargetPosition(mAI.GetID());

		Vector3f cur_position = BaseAI.GetPosition(mAI.GetID());

		Vector3f target_position = BaseAI.GetPosition(mTargetId);

			
		float curangle = BaseAI.CalcDirection(target_position, cur_position);	
		float randRange = BaseAI.Randomf(mMinRangle * 10, mMaxRangle * 10);		
		randRange = randRange / 10;
			
		if(mMaxRangle - mMinRangle >= 3)
			randRange= mMaxRangle;					
								
		Vector3f finalPos = BaseAI.get_position_angle_and_distance_position(target_position, curangle, randRange);
						
		float x = cur_position.x - target_position.x;
		float z = cur_position.z - target_position.z;
		float dist = (float)System.Math.Sqrt((float)(x * x + z * z));			
			
		if(mIsSkill)
        {
			return BaseAI.IsUseSkill(mAI.GetID()) == true;
		}
			
		if(dist <= mMaxRangle)
        {
            BaseAI.StopMove(mAI.GetID());
			BaseAI.LookAt(mAI.GetID(), target_position);
			BaseAI.UseSkillToTarget(mAI.GetID(), mSkillId, mTargetId);
			mIsSkill = true;		
			return true;
		}
						
		x = final_position.x - target_position.x;
		z = final_position.z - target_position.z;			
		dist = (float)System.Math.Sqrt((float)(x * x + z * z));
		if(dist > mMaxRangle || !BaseAI.isMoving(mAI.GetID()))
        {						
			if(!BaseAI.MoveTo(mAI.GetID(), finalPos))
				return false;
		}	
			
		return true;	
    }

    public override void Terminate()
    {
        //BaseAI.StopMove(mAI.GetID());
    }
};

public class AIGoalPlayAni : AIGoal
{
    string mAniName;

    public AIGoalPlayAni(BaseAI ai, string aniName)
        : base(ai)
    {
        mAniName = aniName;
    }

    public override bool Activate()
    {
        BaseAI.PlayAni(mAI.GetID(), mAniName);
        return true;
    }

    public override bool Process(uint elapsed)
    {
        return false;
    }

    public override void Terminate()
    {
        
    }

};

public class AIGoalPlayAniByFun : AIGoal
{
    public delegate void PlyAni();
    public PlyAni ply;

    public AIGoalPlayAniByFun(BaseAI ai, PlyAni fun)
        : base(ai)
    {
        ply = fun;
    }

    public override bool Activate()
    {
        ply();
        return true;
    }

    public override bool Process(uint elapsed)
    {
        return false;
    }

    public override void Terminate()
    {

    }

};

public class AIGoalAddBuff : AIGoal
{
    uint mObjId;
    int mBuffResId;

    public AIGoalAddBuff(BaseAI ai, uint objId, int buffResId)
        : base(ai)
    {
        mObjId = objId;
        mBuffResId = buffResId;
    }

    public override bool Activate()
    {
        BaseAI.AddBuffByResId(mAI.GetID(), mObjId, mBuffResId);
        return true;
    }

    public override bool Process(uint elapsed)
    {
        return false;
    }

    public override void Terminate()
    {

    }

};

public class AIGoalRemoveBuff : AIGoal
{
    uint mObjId;
    int mBuffResId;

    public AIGoalRemoveBuff(BaseAI ai, uint objId, int buffResId)
        : base(ai)
    {
        mObjId = objId;
        mBuffResId = buffResId;
    }

    public override bool Activate()
    {
        BaseAI.RemoveBuffByResId(mObjId, mBuffResId);
        return true;
    }

    public override bool Process(uint elapsed)
    {
        return false;
    }

    public override void Terminate()
    {

    }

};

public class AIGoalPromptUI : AIGoal
{
    private string mPrompt;

    public AIGoalPromptUI(BaseAI ai, string prompt)
        : base(ai)
    {
        mPrompt = prompt;
    }

    public override bool Activate()
    {
        BaseAI.PromptUI(mPrompt);
        return true;
    }

    public override bool Process(uint elapsed)
    {
        return false;
    }

    public override void Terminate()
    {

    }

};
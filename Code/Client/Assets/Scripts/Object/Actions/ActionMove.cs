using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ActionMoveInitParam : ActionInitParam
{
    public bool wayOrDir = true;   //  路径移动 或者方向移动
    public List<Vector3f> wayPoint = new List<Vector3f>();
    public float dir = 0.0f;

	public override ActionTypeDef ActionType
	{
		get { return ActionTypeDef.ActionTypeMove; }
	}
}

public enum MOVE_MODE:int
{
    MOVE_MODE_DIRECTION = 0,
    MOVE_MODE_WAY = 1,
}
public class ActionMove : Action
{
	public class Allocator : ActionAllocator
	{
		public Action Allocate()
		{
			return new ActionMove();
		}
	}

    private int mCurrentIndex = 0;
    private List<Vector3f> mPathNodes = new List<Vector3f>();

    private float mMoveDirection = 0.0f;

    private MOVE_MODE mMoveMode = MOVE_MODE.MOVE_MODE_DIRECTION;

	public override ActionTypeDef ActionType
	{
		get { return ActionTypeDef.ActionTypeMove; }
	}

    public bool IsDirection()
    {
        return (mMoveMode == MOVE_MODE.MOVE_MODE_DIRECTION);
    }

    protected override ErrorCode doStart(ActionInitParam param)
    {
        if (param == null)
            return ErrorCode.InvalidParam;

        if (param.GetType() != typeof(ActionMoveInitParam))
            return ErrorCode.InvalidParam;

        return Restart(param);
    }

    protected override UpdateRetCode onUpdate(uint elapsed)
    {
        if(mOwner == null)
            return UpdateRetCode.Aborted;

        // 能不能移动
        if (!mOwner.IsCanMove())
        {
            return UpdateRetCode.Aborted;
        }


        if( mMoveMode == MOVE_MODE.MOVE_MODE_WAY )
        {
            return UpdateMovePos(elapsed);
        }
        else if( mMoveMode == MOVE_MODE.MOVE_MODE_DIRECTION )
        {
            return UpdateMoveDir(elapsed);
        }
        return UpdateRetCode.Finished;
    }

    private UpdateRetCode UpdateMovePos(uint elapsed)
    {
        if (mPathNodes == null)
            return UpdateRetCode.Aborted;

        float movDist = elapsed * mOwner.GetSpeed() / 1000f;
        if (movDist < 0.001f)
            return UpdateRetCode.Continue;

        Vector3f toPos = mOwner.GetPosition3f();
        Vector3f curPos = mOwner.GetPosition3f();
        while (movDist > 0 && mCurrentIndex < mPathNodes.Count)
        {
            Vector3f nodePos = mPathNodes[mCurrentIndex];
            Vector3f len3d = nodePos.Subtract(curPos);
            len3d.y = 0;

            float distToNode = len3d.Length();
            if (movDist >= distToNode)
            {
                movDist -= distToNode;
                toPos = nodePos;

                ++mCurrentIndex;

                if (mCurrentIndex < mPathNodes.Count)
                {

                    Vector3f nodePosNext = mPathNodes[mCurrentIndex];
                    Vector3f posOwner = mOwner.GetPosition3f();
                    if (Math.Abs(posOwner.x - nodePosNext.x) > 0.01f ||
                        Math.Abs(posOwner.z - nodePosNext.z) > 0.01f)
                    {
                        float dir = Utility.Angle2D(new UnityEngine.Vector3(nodePosNext.x, nodePosNext.y, nodePosNext.z),new UnityEngine.Vector3(posOwner.x, posOwner.y, posOwner.z)) * UnityEngine.Mathf.Deg2Rad;
                        mOwner.SetDirection(dir);
                        mOwner.SetMoveDirection(dir);
                    }

                }
                else
                {
                    movDist = 0;
                }
            }
            else
            {
                if (mCurrentIndex > 0)
                {
                    Vector3f dir = mPathNodes[mCurrentIndex].Subtract(mPathNodes[mCurrentIndex - 1]);
                    dir.y = 0;

                    dir.Normalize();
                    dir.MultiplyBy(movDist);
                    toPos = curPos.Add(dir);

                    LineSegf line = new LineSegf(new Vector2f(mPathNodes[mCurrentIndex - 1].x, mPathNodes[mCurrentIndex - 1].z),
                                                new Vector2f(mPathNodes[mCurrentIndex].x, mPathNodes[mCurrentIndex].z));
                    Vector2f tarPos = Geometryalgorithm2d.point_line_intersect(new Vector2f(toPos.x, toPos.z), line);
                    toPos.x = tarPos.x;
                    toPos.z = tarPos.y;
                    movDist = 0;
                }
                else
                {
                    len3d.y = 0;
                    len3d.Normalize();
                    toPos = curPos.Add(len3d.MultiplyBy(movDist));
                    movDist = 0;
                }
            }
        }

        if (!mOwner.Scene.IsInWalkableRegion(toPos.x, toPos.z))
            return UpdateRetCode.Finished;

        //if (mOwner.Scene.IsBarrierRegion(mOwner, toPos.x, toPos.z))
        //{
        //    return UpdateRetCode.Finished;
        //}

        toPos.y = mOwner.Scene.GetHeight(toPos.x, toPos.z);

        mOwner.SetPosition3f(toPos);

        return mCurrentIndex >= mPathNodes.Count ? UpdateRetCode.Finished : UpdateRetCode.Continue;
    }

    private UpdateRetCode UpdateMoveDir(uint elapsed)
    {
        float movDist = elapsed * mOwner.GetSpeed() / 1000f;
        if (movDist < 0.001f)
            return UpdateRetCode.Continue;

        float sx = Mathf.Sin(mMoveDirection) * movDist;
        float sy = Mathf.Cos(mMoveDirection) * movDist;

        Vector3 toPos = mOwner.GetPosition();

        toPos.x += sx;
        toPos.z += sy;

        if (CanMoveTo(toPos))
        {
            BaseScene scn = mOwner.Scene;

            toPos.y = scn.GetHeight(toPos.x, toPos.z);
            mOwner.SetPosition(toPos);
        }

        //Vector3 f0 = (transform.position + (r * Vector3.forward) * distance);
        return UpdateRetCode.Continue;
    }

    private bool CanMoveTo( Vector3 tarPos)
    {
        BaseScene scn = mOwner.Scene;
        float KEEP_SIZE = 0.1f;

        if( tarPos.x < KEEP_SIZE || tarPos.z < KEEP_SIZE 
            || tarPos.x > scn.GetXSize() - KEEP_SIZE
            || tarPos.z > scn.GetZSize() - KEEP_SIZE )
        {
            return false;
        }
        if (mOwner.IsGMFly())
        {
            return true;
        }

       Camera camera =  CameraController.Instance.CurCamera;
        //主角受此影响
       if (mOwner is Player && camera != null && CameraController.Instance.LockCamera)
       {
           Vector3 posx = tarPos + new Vector3(1f, 0, 0);
           Vector3 negx = tarPos + new Vector3(2, 0, 0);
           Vector3 posy = tarPos + new Vector3(0, 2, 0);
           Vector3 negz = tarPos + new Vector3(0, 0, -1);

           if (!Utility.InScreen(camera.WorldToScreenPoint(posx)))
               return false;
           if (!Utility.InScreen(camera.WorldToScreenPoint(negx)))
               return false;
           if (!Utility.InScreen(camera.WorldToScreenPoint(posy)))
               return false;
           if (!Utility.InScreen(camera.WorldToScreenPoint(negz)))
               return false;
       }

        Vector3 curPos = mOwner.GetPosition();

        if( mMoveMode != MOVE_MODE.MOVE_MODE_WAY )
        {
            if( !scn.IsInWalkableRegion( tarPos.x , tarPos.z ) )
            {
                if (scn.IsInWalkableRegion(curPos.x, curPos.z))
                    return false;
            }

			if(scn.IsBarrierRegion(mOwner, tarPos.x, tarPos.z))
			{
				return false;
			}
        }

        return true;
    }

    public Vector3f GetMoveTargetPos()
    {
        if (mPathNodes.Count <= 0)
            return new Vector3f();

        Vector3f lastPoint = mPathNodes[mPathNodes.Count - 1];
        return new Vector3f(lastPoint.x, lastPoint.y, lastPoint.z);
    }

    public ErrorCode Restart(ActionInitParam param)    // 不停止动画
    {
        if (param == null)
            return ErrorCode.InvalidParam;

        if (param.GetType() != typeof(ActionMoveInitParam))
            return ErrorCode.InvalidParam;

        ActionMoveInitParam thisParam = (ActionMoveInitParam)param;

        if( thisParam.wayOrDir )
        {
            mPathNodes = thisParam.wayPoint;

            mCurrentIndex = 0;

            mMoveMode = MOVE_MODE.MOVE_MODE_WAY;
        }
        else
        {
            mMoveMode = MOVE_MODE.MOVE_MODE_DIRECTION;
            mMoveDirection = thisParam.dir;

            mOwner.SetMoveDirection(mMoveDirection);
        }

        return ErrorCode.Succeeded;
    }

    private int mSoundInsId = -1;
    protected override void onStopped(bool finished) 
    {
        // 停止移动动画
        MecanimStateController statecontrol = mOwner.GetStateController();
        bool lowerMove = mOwner.GetMovingType() == MovingType.MoveType_Lowwer;

        //AnimActionMove action = AnimActionFactory.Create(AnimActionFactory.E_Type.Move) as AnimActionMove;
        //action.type = AnimActionMove.MoveType.Stop;
        //mOwner.GetStateController().DoAction(action, lowerMove ? MecanimStateController.Statelayer.Move : MecanimStateController.Statelayer.Normal);
      
        if (mSoundInsId != -1)
        {
            SoundManager.Instance.RemoveSoundByID(mSoundInsId);
        }
    }

    protected override void onStarted() 
    {
        // 播放移动动画

        if (mOwner == null || mOwner.GetStateController() == null)
            return;
        // 停止移动动画
        MecanimStateController statecontrol = mOwner.GetStateController();
        bool lowerMove = mOwner.GetMovingType() == MovingType.MoveType_Lowwer;

        mOwner.GetStateController().DoAction(AnimActionFactory.E_Type.Move,lowerMove ? MecanimStateController.Statelayer.Move : MecanimStateController.Statelayer.Normal);
        
        if (mOwner.GetWalkSound() != -1)
        {
            mSoundInsId = SoundManager.Instance.Play(mOwner.GetWalkSound(), null, true);
        }
       
       
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class PlayerController
{
	private static PlayerController instance = null;

	private uint mCurCtrlID = uint.MaxValue;
    private uint mCurMainCropsCtrlID = uint.MaxValue;
    private uint mCurSubCropsCtrlID = uint.MaxValue;

    public Queue<Vector3> mWayPoints;
    public int mTargetMapId;
    public bool IsAutoMoving = false;
//     private float mMoveInterval = 0.0f;
// 
//     private static float MAX_MOVE_INTERVAL = 0.1	

    public void MoveTargetMap(float x, float z, int mapId)
    {
        IsAutoMoving = true;
        WindowManager.Instance.OpenUI("autofindway");
        mWayPoints.Clear();
        if (SceneManager.Instance.GetCurScene().GetSceneResId() == mapId)
        {
            MovePos(new Vector3(x, 1.0f, z));
        }
        else
        {
            ModuleManager.Instance.FindModule<WorldMapModule>().GuideResId = mapId;
            MovePos(SceneManager.Instance.GetCurScene().GetTransport());
            mWayPoints.Enqueue(new Vector3(x, 1, z));
            mTargetMapId = mapId;
        }
    }

    public bool QuestMoveCheck()
    {
        if ( mWayPoints.Count == 0) return false;
       
        if (mTargetMapId == SceneManager.Instance.GetCurScene().GetSceneResId())
        {
            WindowManager.Instance.OpenUI("autofindway");
            MovePos(mWayPoints.Peek());
           
        }
        else
        {
            mWayPoints.Clear();
        }
      
        return true;
    }
    public void BreakQuestMove()
    {
        mWayPoints.Clear();
        mTargetMapId = -1;
        IsAutoMoving = false;
        WindowManager.Instance.CloseUI("autofindway");
    }
    public static PlayerController Instance
	{
		get
		{
			return instance;
		}
	}

    public PlayerController()
	{
		instance = this;
        mWayPoints = new Queue<Vector3>();
	}

    public void Term()
    {
        mCurCtrlID = uint.MaxValue;
    }

    public void SetControl(uint instID)
    {
        mCurCtrlID = instID;
    }

    public uint GetControl()
    {
        return mCurCtrlID;
    }

    public void SetMainCropsControl(uint instID)
    {
        mCurMainCropsCtrlID = instID;
    }

    public void SetSubCropsControl(uint instID)
    {
        mCurSubCropsCtrlID = instID;
    }

    public uint GetMainCropsControl()
    {
        return mCurMainCropsCtrlID;
    }

    public uint GetSubCropsControl()
    {
        return mCurSubCropsCtrlID;
    }

    public ObjectBase GetControlObj()
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();

        if (scn == null)
            return null;

        ObjectBase obj = scn.FindObject(mCurCtrlID) as ObjectBase;
        return obj;
    }

    public ObjectBase GetMainCropsControlObj()
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();

        if (scn == null)
            return null;

        ObjectBase obj = scn.FindObject(mCurMainCropsCtrlID) as ObjectBase;
        return obj;
    }

    public ObjectBase GetSubCropsControlObj()
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();

        if (scn == null)
            return null;

        ObjectBase obj = scn.FindObject(mCurSubCropsCtrlID) as ObjectBase;
        return obj;
    }

    public void MoveDir(float dir)
    {
        Player player = GetControlObj() as Player;
        if (player == null)
            return;

        if (player.IsCanMoveRotation() && player.IsCanRotation() )
        {
            player.SetDirection(dir);
        }

        if (!player.IsMoveing())
        {
            player.MoveDir(dir);
        }

        //下面需要优化 效率太低
//         mMoveInterval += Time.deltaTime;
// 
//         if (mMoveInterval < MAX_MOVE_INTERVAL)
//             return;
//         mMoveInterval = 0.0f;

        BaseScene scn = player.Scene;

        float sx = Mathf.Sin(dir) * 1.0f;
        float sy = Mathf.Cos(dir) * 1.0f;

        Vector3 pos = player.GetPosition();

        pos.x = pos.x + sx;
        pos.z = pos.z + sy;

        if (player.IsGMFly() || !scn.IsInWalkableRegion(player.GetPosition()) || scn.MayLineArrive(player.GetPosition() , pos))
        {
            player.MoveDir(dir);
        }
        else
        {
            bool bFind = false;
            for( int i = 1 ; i <= 89 ;++i )
            {
                bool bBreak = false;
                float offset = i * Mathf.PI / 180.0f;
                for( int j = -1 ; j <=1 ; j +=2 )
                {
                    float tardir = Utility.normalizeRadian(dir + offset * j);
                    sx = Mathf.Sin(tardir) * 2.0f;
                    sy = Mathf.Cos(tardir) * 2.0f;

                    pos = player.GetPosition();
                    pos.x = pos.x + sx;
                    pos.z = pos.z + sy;
                    if (!scn.MayLineArrive(player.GetPosition(), pos))
                        continue;
                    bFind = true;

                    player.MoveDir(tardir);

                    bBreak = true;
                }

                if (bBreak)
                    break;
            }

            if (!bFind)
            {
                player.MoveDir(dir);
            }
         }
    }

    public void MovePos(Vector3 pos)
    {
        Player player = GetControlObj() as Player;
        if (player == null)
            return;
        player.MovePos(pos);
    }

    public void StopMove()
    {
        Player player = GetControlObj() as Player;

        if (player == null || !player.IsMoveing())
            return;
        player.StopMove();
        BreakQuestMove();
    }

    public void StopDir()
    {
        Player player = GetControlObj() as Player;

        if (player == null || !player.IsMoveing())
            return;

        player.StopDirMove();
    }

    /// <summary>
    /// 重置为初始位置
    /// </summary>
    public void SetInitPos()
    {
        Player player = GetControlObj() as Player;
        if (player == null)
            return;
        BaseScene scn = SceneManager.Instance.GetCurScene();

        if (scn == null)
            return ;
       
        player.SetPosition( scn.GetInitPos());
        player.SetDirection(scn.GetInitDir());
    }

    /// <summary>
    /// 设置定身效果
    /// </summary>
    /// <param name="isFreeze"> true为定身，false为解除定身</param>
    public void SetFreeze(bool isFreeze)
    {
        BaseScene scene = SceneManager.Instance.GetCurScene();
        Player player = PlayerController.Instance.GetControlObj() as Player;
        if (player == null) return;
        if (player != null)
        {
            player.SetLeague(LeagueDef.Neutral);
        }

        if (isFreeze)
        {
            scene.RemoveAllActionFlag();
            player.SetLeague(LeagueDef.Neutral);
        }
        else
        {
            scene.AddAllActionFlag();
            player.SetLeague(LeagueDef.Red);
            if (player.IsDead())
            {
                player.Relive((int) player.GetMaxHP(), (int) player.GetMaxMana());

            }
            else
            {
                player.ModifyPropertyValue((int)PropertyTypeEnum.PropertyTypeHP, (int)(player.GetMaxHP()));
                player.ModifyPropertyValue((int)(int)PropertyTypeEnum.PropertyTypeMana, (int)(player.GetMaxMana()));
            }
            //重置子弹数
            player.AddWeaponBullet(player.GetWeaponMaxBullet());
            player.ResetAllSKillCd();
        }

        Crops maincrops = GetMainCropsControlObj() as Crops;
        Crops subcrops = GetSubCropsControlObj() as Crops;

        if (null != maincrops)
        {
            if (maincrops.IsDead())
            {
                maincrops.Relive((int)maincrops.GetMaxHP(), (int)maincrops.GetMaxMana());
            }
            else
            {
                maincrops.ModifyPropertyValue((int)PropertyTypeEnum.PropertyTypeHP, (int)(maincrops.GetMaxHP()));
                maincrops.ModifyPropertyValue((int)(int)PropertyTypeEnum.PropertyTypeMana, (int)(maincrops.GetMaxMana()));
            }
        }

        if (null != subcrops)
        {
            if (subcrops.IsDead())
            {
                subcrops.Relive((int)subcrops.GetMaxHP(), (int)subcrops.GetMaxMana());
            }
            else
            {
                subcrops.ModifyPropertyValue((int)PropertyTypeEnum.PropertyTypeHP, (int)(subcrops.GetMaxHP()));
                subcrops.ModifyPropertyValue((int)(int)PropertyTypeEnum.PropertyTypeMana, (int)(subcrops.GetMaxMana()));
            }
        }

        if (null != maincrops || null != subcrops)
        {
            (scene as GameScene).ResetCropsReliveTimes();
            EventSystem.Instance.PushEvent(new CropsEvent(CropsEvent.CROPS_RELIVE_NO_TIME_DOWN));
        }
    }

	public void SetWudi(bool wudi)
	{
		BattleUnit unit = GetControlObj() as BattleUnit;
		if (unit == null)
			return;

		unit.SetWudi(wudi);
	}
}

using System.Collections;
using System.Collections.Generic;


public class XunLuoNoLoop : CommonAI
{
    private List<Vector3f> posList = new List<Vector3f>();
    private List<int> waitList = new List<int>();
    private int movePos = 0;


    public override BattleUnitAI CreateAIType(BattleUnit battleUnit)
    {
        return new XunLuoNoLoop(battleUnit);
    }
    public XunLuoNoLoop(BattleUnit battleUnit)
        : base(battleUnit)
    {
    }

    public override bool Init(int aiId)
    {
        if (!base.Init(aiId))
            return false;

        do
        {
            if (null != mRes.param1)
            {
                string [] pos = mRes.param1.Split(',');
                posList.Add(new Vector3f(float.Parse(pos[0]),float.Parse(pos[1]),float.Parse(pos[2])));
                waitList.Add(int.Parse(pos[3]));
            }   
        } while (false);

        do
        {
            if (null != mRes.param2)
            {
                string [] pos = mRes.param2.Split(',');
                posList.Add(new Vector3f(float.Parse(pos[0]),float.Parse(pos[1]),float.Parse(pos[2])));
                waitList.Add(int.Parse(pos[3]));
            }   
        } while (false);

        do
        {
            if (null != mRes.param3)
            {
                string [] pos = mRes.param3.Split(',');
                posList.Add(new Vector3f(float.Parse(pos[0]),float.Parse(pos[1]),float.Parse(pos[2])));
                waitList.Add(int.Parse(pos[3]));
            }   
        } while (false);

        do
        {
            if (null != mRes.param4)
            {
                string [] pos = mRes.param4.Split(',');
                posList.Add(new Vector3f(float.Parse(pos[0]),float.Parse(pos[1]),float.Parse(pos[2])));
                waitList.Add(int.Parse(pos[3]));
            }   
        } while (false);

        do
        {
            if (null != mRes.param5)
            {
                string [] pos = mRes.param5.Split(',');
                posList.Add(new Vector3f(float.Parse(pos[0]),float.Parse(pos[1]),float.Parse(pos[2])));
                waitList.Add(int.Parse(pos[3]));
            }   
        } while (false);

        do
        {
            if (null != mRes.param6)
            {
                string [] pos = mRes.param6.Split(',');
                posList.Add(new Vector3f(float.Parse(pos[0]),float.Parse(pos[1]),float.Parse(pos[2])));
                waitList.Add(int.Parse(pos[3]));
            }   
        } while (false);

        do
        {
            if (null != mRes.param7)
            {
                string [] pos = mRes.param7.Split(',');
                posList.Add(new Vector3f(float.Parse(pos[0]),float.Parse(pos[1]),float.Parse(pos[2])));
                waitList.Add(int.Parse(pos[3]));
            }   
        } while (false);

        do
        {
            if (null != mRes.param8)
            {
                string [] pos = mRes.param8.Split(',');
                posList.Add(new Vector3f(float.Parse(pos[0]),float.Parse(pos[1]),float.Parse(pos[2])));
                waitList.Add(int.Parse(pos[3]));
            }   
        } while (false);

        do
        {
            if (null != mRes.param9)
            {
                string [] pos = mRes.param9.Split(',');
                posList.Add(new Vector3f(float.Parse(pos[0]),float.Parse(pos[1]),float.Parse(pos[2])));
                waitList.Add(int.Parse(pos[3]));
            }   
        } while (false);

        do
        {
            if (null != mRes.param10)
            {
                string [] pos = mRes.param10.Split(',');
                posList.Add(new Vector3f(float.Parse(pos[0]),float.Parse(pos[1]),float.Parse(pos[2])));
                waitList.Add(int.Parse(pos[3]));
            }   
        } while (false);

        // ½âÎö²ÎÊý
        return true;
    }

    public override void OnEnterIdle()
    {
    }

    public override void OnExitIdle()
    {
    }

    public override void OnUpdateIdle(uint elapsed)
    {
        Vector3f posn = BaseAI.GetPosition(GetID());

        if (movePos <= posList.Count - 1 && posn.Subtract(posList[movePos]).Length() > 1)
        {
            BaseAI.MoveTo(GetID(), posList[movePos]);
        }
        else
        {
            if (movePos <= posList.Count - 1)
            {
                movePos += 1;
            }
        }
    }

    public override void OnEnterCombat()
    {
    }

    public override void OnExitCombat()
    {
    }

    public override void OnUpdateCombat(uint elapsed)
    {
        uint mainTargetId = GetCurrentTargetId();

        if (BeginCommand(100))
        {
            
        }
    }

};
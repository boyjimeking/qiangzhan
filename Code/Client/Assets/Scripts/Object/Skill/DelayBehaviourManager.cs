using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DelayBehaviourNode
{
    public BattleUnit mOwner = null;
    public MaterialBehaviourDef name;
    public ImpactDamageType impactDamageType;
    public float dir;
    public float time = 0.0f;
}
//延迟表现管理器
public class DelayBehaviourManager
{
    private List<DelayBehaviourNode> nodes = new List<DelayBehaviourNode>();
    private List<DelayBehaviourNode> exList = new List<DelayBehaviourNode>();

    private BaseScene mScene = null;
    public DelayBehaviourManager(BaseScene scn)
    {
        mScene = scn;
    }
    public void AddDelayBehaviour(BattleUnit obj,MaterialBehaviourDef name, ImpactDamageType impactDamageType, float dir,float time = 0.0f)
    {
        DelayBehaviourNode node = new DelayBehaviourNode();
        node.mOwner = obj;
        node.name = name;
        node.impactDamageType = impactDamageType;
        node.dir = dir;
        node.time = time;

        nodes.Add(node);
    }
    public void Update(uint elapsed)
    {
        if (nodes.Count <= 0)
            return;

        for( int i = 0 ; i < nodes.Count ; ++i )
        {
            nodes[i].time -= elapsed;
            if( nodes[i].time <= 0 )
            {
                exList.Add(nodes[i]);
            }
        }

        for( int i = 0 ; i < exList.Count ; ++i )
        {
            BattleUnit unit = exList[i].mOwner;
            if (unit != null && !unit.IsDestory())
            {
                SkillDetails.AddMaterialBehaviour(unit, unit.GetHitMaterialEffectCdContainer(), exList[i].name, exList[i].impactDamageType, exList[i].dir);
            }

            nodes.Remove(exList[i]);
        }

        exList.Clear();
    }
}

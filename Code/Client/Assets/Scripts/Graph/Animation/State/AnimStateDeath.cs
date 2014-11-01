using FantasyEngine;
using System;
using UnityEngine;

public class AnimStateDeath : AnimState
{
    private AnimActionDeath Action;
    private float CurrentMoveTime;
    private Vector3 FinalPosition;
    private Quaternion FinalRotation;
    private float MoveTime;
    private float RotationProgress;
    private Vector3 StartPosition;
    private Quaternion StartRotation;
	string deathAnim;

    public AnimStateDeath(MecanimManager anims, VisualObject owner)
        : base(anims, owner)
    {
    }

    public override bool HandleNewAction(AnimAction action)
    {
        if (action is AnimActionDeath)
        {
            action.SetFailed();
            return true;
        }
        //屏蔽其他一切动作，这里相当于最高级
        return true;
    }

    protected override void Initialize(AnimAction action)
    {
        base.Initialize(action);
        Action = action as AnimActionDeath;
        deathAnim = Action.dieAnim;

        SetTransition(deathAnim);
   }

    public override void OnActivate(AnimAction action)
    {
        base.OnActivate(action);
    }

    public override void Reset()
    {
        this.Action.SetSuccess();
    }

    public override void Update()
    {
        SetTransition(deathAnim);
    }
    public override void HandlemAnimatorEvent(MecanimEvent animEvent)
    {
		if (animEvent != null)
		{
            if (animEvent.name == MecanimEvent.MEC_ANIM_END && mAnimator.Property.GetStateHash(deathAnim) == animEvent.context.curstate)
				Release();
		}
    }
}


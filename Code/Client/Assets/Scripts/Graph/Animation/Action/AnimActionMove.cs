using System;

public class AnimActionMove : AnimAction
{
    public enum MoveType
    {
        Start,
        Stop,
    }
    public MoveType type = MoveType.Start;
    public AnimActionMove() : base(AnimActionFactory.E_Type.Move)
    {
    }
}


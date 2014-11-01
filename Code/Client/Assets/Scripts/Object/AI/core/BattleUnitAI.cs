
public abstract class BattleUnitAI : BaseAI
{
    public BattleUnitAI(BattleUnit owner) : base(owner)
    {
    }

    public abstract BattleUnitAI CreateAIType(BattleUnit battleUnit);
};
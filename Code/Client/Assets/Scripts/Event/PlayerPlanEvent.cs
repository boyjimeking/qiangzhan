public class PlanyerPlan : EventBase
{
    public static string PLAN_UPDATE_EVENT = "PLAN_UPDATE_EVENT";

    public PlanyerPlan()
        : base(PLAN_UPDATE_EVENT)
    {

    }
}
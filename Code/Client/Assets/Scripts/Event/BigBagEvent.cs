public class BigBagEvent : EventBase
{
    public static string BIGBAG_UPDATE_EVENT = "BIGBAG_UPDATE_EVENT";

    public BigBagEvent()
        : base(BIGBAG_UPDATE_EVENT)
    {

    }
}
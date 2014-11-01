
public class WingUIEvent:EventBase
{
	public static string Wing_UI_UPDATE = "Wing_UI_UPDATE";
	public static string WING_UI_ACTIVE = "WING_UI_ACTIVE";
	public static string WING_UI_EQUIP = "WING_UI_EQUIP";
	public static string WING_UI_FORGE = "WING_UI_FORGE";
    public static string WING_GHOST_UPDATE = "WING_GHOST_UPDATE";

	public int wingid;
	public int result;
	public int action;
    
	public WingUIEvent(string eventname):
		base(eventname)
	{
	
	}
}

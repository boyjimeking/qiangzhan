
 public class FashionEvent:EventBase
 {
     public FashionEvent(string eventName) : base(eventName)
     {
     }

     public const string FASHION_ACTIVE = "FASHION_ACTIVE";
     public const string FASHION_EQUIP = "FASHION_EQUIP";
     public const string FASHION_ADDSTAR = "FASHION_ADDSTAR";
     public const string FASHION_UPDATE = "FASHION_UPDATE";

     public int mfashionid;
     public int actionid;

 }


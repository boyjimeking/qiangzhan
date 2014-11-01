using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

  public  class EnterGameEvent:EventBase
    {
      public static string ENTER_GAME = "ENTER_GAME";
      public EnterGameEvent(string eventName) : base(eventName)
      {
      }
    }


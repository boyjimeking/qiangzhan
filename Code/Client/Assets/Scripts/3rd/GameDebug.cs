using System;
using UnityEngine;

/// <summary>
/// 游戏debug处理类
/// </summary>
public class GameDebug
{
	/// <summary>
	/// log日志级别
	/// </summary>
	public enum LogLevel{
		DEBUG = 0,
		INFO = 10,
		WORNING = 20,
		EXCEPTION = 30,
		ERROR = 40,
		NULL = 50,
	}
#if UNITY_STANDALONE_WIN || UNITY_EDITER
	public static bool IsPrintDebugInfo = true;
#else
	public static bool IsPrintDebugInfo = false;
#endif
    private static LogLevel logLevel = LogLevel.DEBUG;

	public static void Log(object message){
		if(!IsPrintDebugInfo){
			return;
		}
		if(logLevel <= LogLevel.DEBUG){
    			Debug.Log(message);
		}
	}
	
	public static void Log(object message,UnityEngine.Object context){
		if(!IsPrintDebugInfo){
			return;
		}
		if(logLevel <= LogLevel.DEBUG){
			Debug.Log(message,context);
		}
	}
	
	public static void LogWarning(object message){
		if(!IsPrintDebugInfo){
			return;
		}
		if(logLevel <= LogLevel.WORNING){
			Debug.LogWarning(message);
		}
	}
	
	public static void LogWarning(object message,UnityEngine.Object context){
		if(!IsPrintDebugInfo){
			return;
		}
		if(logLevel <= LogLevel.WORNING){
			Debug.LogWarning(message,context);
		}
	}
	
	public static void LogException(Exception exception){
		if(!IsPrintDebugInfo){
			return;
		}
		if(logLevel <= LogLevel.EXCEPTION){
			Debug.LogException(exception);
		}
	}
	
	public static void LogException(Exception exception,UnityEngine.Object context){
		if(!IsPrintDebugInfo){
			return;
		}
		if(logLevel <= LogLevel.EXCEPTION){
			Debug.LogException(exception,context);
		}
	}
	
	public static void LogError(object message){
		if(!IsPrintDebugInfo){
			return;
		}
		if(logLevel <= LogLevel.ERROR){
            Debug.LogError(message);
		}
	}
	
	public static void LogError(object message,UnityEngine.Object context){
		if(!IsPrintDebugInfo){
			return;
		}
		if(logLevel <= LogLevel.ERROR){
			Debug.LogError(message,context);
		}
	}
	
	
}



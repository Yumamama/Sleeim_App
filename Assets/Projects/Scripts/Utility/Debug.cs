#if ! UNITY_EDITOR
#define DEBUG_LOG_OVERWRAP //リリース版ではコメント解除してログ出力しないようにする
#endif

using UnityEngine;

public static class Debug
{
	static public void Break()
	{
		if (IsEnable()) {
			UnityEngine.Debug.Break();
		}
	}

	static public void Log(object message)
	{
		if (IsEnable()) {
			UnityEngine.Debug.Log(message);
		}
	}

	static public void Log(object message, Object context)
	{
		if (IsEnable()) {
			UnityEngine.Debug.Log(message, context);
		}
	}

	static public void LogWarning(object message)
	{
		if (IsEnable()) {
			UnityEngine.Debug.LogWarning(message);
		}
	}

	static public void LogWarning(object message, Object context)
	{
		if (IsEnable()) {
			UnityEngine.Debug.LogWarning(message, context);
		}
	}

	static public void LogError(object message)
	{
		if (IsEnable()) {
			UnityEngine.Debug.LogError(message);
		}
	}

	static public void LogError(object message, Object context)
	{
		if (IsEnable()) {
			UnityEngine.Debug.LogError(message, context);
		}
	}

	static public void LogException(System.Exception e)
	{
		if (IsEnable()) {
			UnityEngine.Debug.LogException(e);
		}
	}

	static public void LogException(System.Exception e, Object context)
	{
		if (IsEnable()) {
			UnityEngine.Debug.LogException(e, context);
		}
	}

	static bool IsEnable()
    {
#if DEBUG_LOG_OVERWRAP
		return false;
#else
        return true;
#endif
    }
}

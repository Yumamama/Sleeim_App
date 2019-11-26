using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// アラームの設定を一時的に保持しておくためのデータクラス
/// </summary>
public class AlermTempData : MonoBehaviour {

	const string objectName = "ProfileData";
	static GameObject instanceObj = null;
	public bool alermIsEnable;
	public UserDataManager.Setting.Alerm.CallTime callTime;
	public UserDataManager.Setting.Alerm.DelayTime delayTime;
	public bool ibikiAlermIsEnable;
	public UserDataManager.Setting.Alerm.IbikiAlerm.DetectSense detectSense;
	public bool lowBreathAlermIsEnable;
	public bool stopMoveAlermIsEnable;


	public static AlermTempData Instance {
		get {
			if (instanceObj == null) {
				//シーン遷移で消えないオブジェクト生成
				var obj = new GameObject (objectName);
				obj.AddComponent<AlermTempData> ();
				obj.GetComponent<AlermTempData> ().InitValue ();
				DontDestroyOnLoad (obj);
				instanceObj = obj;
			}
			return instanceObj.GetComponent<AlermTempData> ();
		}
	}

	public void InitValue () {
		//デバイス設定を適用
		alermIsEnable = UserDataManager.Setting.Alerm.isEnable ();
		callTime = UserDataManager.Setting.Alerm.GetCallTime ();
		delayTime = UserDataManager.Setting.Alerm.GetDelayTime ();
		ibikiAlermIsEnable = UserDataManager.Setting.Alerm.IbikiAlerm.isEnable ();
		detectSense = UserDataManager.Setting.Alerm.IbikiAlerm.GetDetectSense ();
		lowBreathAlermIsEnable = UserDataManager.Setting.Alerm.LowBreathAlerm.isEnable ();
		stopMoveAlermIsEnable = UserDataManager.Setting.Alerm.StopMoveAlerm.isEnable ();
	}

	//保存失敗した際に呼ぶ
	public static void ClearData () {
		if (instanceObj != null)
			DestroyImmediate (instanceObj);
	}
}

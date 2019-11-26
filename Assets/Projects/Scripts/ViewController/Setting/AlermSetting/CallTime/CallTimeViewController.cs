using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CallTimeViewController : ViewControllerBase {

	public Text callTime;	//鳴動時間の設定値

	protected override void Start () {
		base.Start ();
		InitCallTime ();
	}

	public override SceneTransitionManager.LoadScene SceneTag {
		get {
			return SceneTransitionManager.LoadScene.CallTime;
		}
	}

	//鳴動時間のテキスト表示を設定します
	void InitCallTime () {
		string text = "";
		switch (AlermTempData.Instance.callTime) {
		case UserDataManager.Setting.Alerm.CallTime.Sec5:
			text = "5秒";
			break;
		case UserDataManager.Setting.Alerm.CallTime.Sec10:
			text = "10秒";
			break;
		case UserDataManager.Setting.Alerm.CallTime.Sec15:
			text = "15秒";
			break;
		case UserDataManager.Setting.Alerm.CallTime.Sec30:
			text = "30秒";
			break;
		default:
			text = "設定しない";
			break;
		}
		callTime.text = text;
	}

	//戻るボタンが押されると呼び出される
	public void OnBackButtonTap () {
		SceneTransitionManager.LoadLevel (SceneTransitionManager.LoadScene.AlermSetting);
	}

	//「鳴動時間」ボタンが押されると呼び出される
	public void OnCallTimeButtonTap () {
		SceneTransitionManager.LoadLevel (SceneTransitionManager.LoadScene.CallTimeSetting);
	}
}

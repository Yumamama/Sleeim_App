using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AlermDelayTimeViewController : ViewControllerBase {

	public Text delayTime;	//アラーム遅延時間の設定値

	protected override void Start () {
		base.Start ();
		InitDelayTime ();
	}

	public override SceneTransitionManager.LoadScene SceneTag {
		get {
			return SceneTransitionManager.LoadScene.DelayTime;
		}
	}

	//遅延時間のテキスト表示を設定します
	void InitDelayTime () {
		string text = "";
		switch (AlermTempData.Instance.delayTime) {
		case UserDataManager.Setting.Alerm.DelayTime.Sec10:
			text = "10秒";
			break;
		case UserDataManager.Setting.Alerm.DelayTime.Sec20:
			text = "20秒";
			break;
		case UserDataManager.Setting.Alerm.DelayTime.Sec30:
			text = "30秒";
			break;
		default:
			text = "設定しない";
			break;
		}
		delayTime.text = text;
	}

	//「アラーム設定」を押したときに呼び出される
	public void OnBackButtonTap () {
		SceneTransitionManager.LoadLevel (SceneTransitionManager.LoadScene.AlermSetting);
	}

	//「アラーム開始遅延時間」を押したときに呼び出される
	public void OnAlermDelayTimeTap () {
		SceneTransitionManager.LoadLevel (SceneTransitionManager.LoadScene.DelayTimeSetting);
	}
}

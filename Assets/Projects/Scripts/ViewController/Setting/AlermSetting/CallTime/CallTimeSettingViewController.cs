using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CallTimeSettingViewController : ViewControllerBase {

	public Toggle noneToggle;
	public Toggle sec5;
	public Toggle sec10;
	public Toggle sec15;
	public Toggle sec30;

	protected override void Start () {
		base.Start ();
		InitSelect ();
	}

	public override SceneTransitionManager.LoadScene SceneTag {
		get {
			return SceneTransitionManager.LoadScene.CallTimeSetting;
		}
	}

	//鳴動時間をすでに設定していれば、再現する
	void InitSelect () {
		switch (AlermTempData.Instance.callTime) {
		case UserDataManager.Setting.Alerm.CallTime.Sec5:
			sec5.isOn = true;
			break;
		case UserDataManager.Setting.Alerm.CallTime.Sec10:
			sec10.isOn = true;
			break;
		case UserDataManager.Setting.Alerm.CallTime.Sec15:
			sec15.isOn = true;
			break;
		case UserDataManager.Setting.Alerm.CallTime.Sec30:
			sec30.isOn = true;
			break;
		default:
			noneToggle.isOn = true;
			sec5.isOn = false;
			sec10.isOn = false;
			sec15.isOn = false;
			sec30.isOn = false;
			break;
		}
	}

	//戻るボタンが押されたときに呼び出される
	public void OnBackButtonTap () {
		SceneTransitionManager.LoadLevel (SceneTransitionManager.LoadScene.CallTime);
	}
	//「設定なし」が押されたときに呼び出される
	public void OnNoSetButtonTap (bool isOn) {
		AlermTempData.Instance.callTime = UserDataManager.Setting.Alerm.CallTime.None;
	}
	//「5秒」が押されたときに呼び出される
	public void On5sButtonTap (bool isOn) {
		AlermTempData.Instance.callTime = isOn ? UserDataManager.Setting.Alerm.CallTime.Sec5 : UserDataManager.Setting.Alerm.CallTime.None;
	}
	//「10秒」が押されたときに呼び出される
	public void On10sButtonTap (bool isOn) {
		AlermTempData.Instance.callTime = isOn ? UserDataManager.Setting.Alerm.CallTime.Sec10 : UserDataManager.Setting.Alerm.CallTime.None;
	}
	//「15秒」が押されたときに呼び出される
	public void On15sButtonTap (bool isOn) {
		AlermTempData.Instance.callTime = isOn ? UserDataManager.Setting.Alerm.CallTime.Sec15 : UserDataManager.Setting.Alerm.CallTime.None;
	}
	//「30秒」が押されたときに呼び出される
	public void On30sButtonTap (bool isOn) {
		AlermTempData.Instance.callTime = isOn ? UserDataManager.Setting.Alerm.CallTime.Sec30 : UserDataManager.Setting.Alerm.CallTime.None;
	}
}

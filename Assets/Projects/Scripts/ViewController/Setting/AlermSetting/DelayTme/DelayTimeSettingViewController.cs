using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DelayTimeSettingViewController : ViewControllerBase {

	public Toggle notSetToggle;
	public Toggle sec10Toggle;
	public Toggle sec20Toggle;
	public Toggle sec30Toggle;

	protected override void Start () {
		base.Start ();
		InitStart ();
	}

	public override SceneTransitionManager.LoadScene SceneTag {
		get {
			return SceneTransitionManager.LoadScene.DelayTimeSetting;
		}
	}

	//遅延時間をすでに選択していれば、再現する
	void InitStart () {
		switch (AlermTempData.Instance.delayTime) {
		case UserDataManager.Setting.Alerm.DelayTime.None:
			notSetToggle.isOn = true;
			break;
		case UserDataManager.Setting.Alerm.DelayTime.Sec10:
			sec10Toggle.isOn = true;
			break;
		case UserDataManager.Setting.Alerm.DelayTime.Sec20:
			sec20Toggle.isOn = true;
			break;
		case UserDataManager.Setting.Alerm.DelayTime.Sec30:
			sec30Toggle.isOn = true;
			break;
		default:
			notSetToggle.isOn = false;
			sec10Toggle.isOn = false;
			sec20Toggle.isOn = false;
			sec30Toggle.isOn = false;
			break;
		}
	}

	//「鳴動時間」が押されたときに呼び出される
	public void OnBackButtonTap () {
		SceneTransitionManager.LoadLevel (SceneTransitionManager.LoadScene.DelayTime);
	}

	//「なし」の選択トグルの値が変化したときに呼び出される
	public void OnSelectNoSet (bool isOn) {
		if (isOn)
			AlermTempData.Instance.delayTime = UserDataManager.Setting.Alerm.DelayTime.None;
	}
	//「10秒」の選択トグルの値が変化したときに呼び出される
	public void OnSelect10s (bool isOn) {
		if (isOn)
			AlermTempData.Instance.delayTime = UserDataManager.Setting.Alerm.DelayTime.Sec10;
	}
	//「20秒」の選択トグルの値が変化したときに呼び出される
	public void OnSelect20s (bool isOn) {
		if (isOn)
			AlermTempData.Instance.delayTime = UserDataManager.Setting.Alerm.DelayTime.Sec20;
	}
	//「30秒の選択トグルの値が変化したときに呼び出される
	public void OnSelect30s (bool isOn) {
		if (isOn)
			AlermTempData.Instance.delayTime = UserDataManager.Setting.Alerm.DelayTime.Sec30;
	}
}

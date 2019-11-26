using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VibrationViewController : ViewControllerBase {

	[SerializeField]Toggle IsEnableToggle = null;	//低呼吸アラームが有効かどうかのトグル

	protected override void Start () {
		base.Start ();
		InitiIsEnableToggle ();
	}

	public override SceneTransitionManager.LoadScene SceneTag {
		get {
			return SceneTransitionManager.LoadScene.Vibration;
		}
	}

	void InitiIsEnableToggle () {
		IsEnableToggle.isOn = UserDataManager.Setting.Alerm.Vibration.isEnable ();
		//値が変化したときに保存されるようにする
		IsEnableToggle.onValueChanged.AddListener (isEnable => {
			UserDataManager.Setting.Alerm.Vibration.SaveIsEnable (isEnable);
			if (isEnable) {
				//ONになったときにバイブレーションを一回鳴らすようにする
				PlayVibrationOnce ();
			}
		});
	}

	//「アラーム設定」ボタンが押されると呼び出される
	public void OnAlermSettingButtonTap () {
		SceneTransitionManager.LoadLevel (SceneTransitionManager.LoadScene.AlermSetting);
	}

	//バイブレーションを一回鳴らす
	void PlayVibrationOnce () {
		if (SystemInfo.supportsVibration)
			Handheld.Vibrate ();
	}
}

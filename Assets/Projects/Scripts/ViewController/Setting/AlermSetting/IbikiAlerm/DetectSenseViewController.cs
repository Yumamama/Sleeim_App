using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetectSenseViewController : ViewControllerBase {

	public Toggle normalToggle;
	public Toggle largeToggle;

	protected override void Start () {
		base.Start ();
		InitSelect ();
	}

	public override SceneTransitionManager.LoadScene SceneTag {
		get {
			return SceneTransitionManager.LoadScene.DetectSense;
		}
	}

	//感度をすでに選択していれば、再現する
	void InitSelect () {
		switch (AlermTempData.Instance.detectSense) {
		case UserDataManager.Setting.Alerm.IbikiAlerm.DetectSense.Normal:
			normalToggle.isOn = true;
			break;
		case UserDataManager.Setting.Alerm.IbikiAlerm.DetectSense.Large:
			largeToggle.isOn = true;
			break;
		default:
			normalToggle.isOn = false;
			largeToggle.isOn = false;
			break;
		}
	}

	//「いびきアラーム」ボタンが押されると呼び出される
	public void OnBackButtonTap () {
		SceneTransitionManager.LoadLevel (SceneTransitionManager.LoadScene.IbikiAlerm);
	}
	//「普通」の選択トグルの値が変化したときに呼び出される
	public void OnSelectNormal (bool isOn) {
		if (isOn)
			AlermTempData.Instance.detectSense = UserDataManager.Setting.Alerm.IbikiAlerm.DetectSense.Normal;
	}
	//「大きめ」の選択トグルの値が変化したときに呼び出される
	public void OnSelectLarge (bool isOn) {
		if (isOn)
			AlermTempData.Instance.detectSense = UserDataManager.Setting.Alerm.IbikiAlerm.DetectSense.Large;
	}
}

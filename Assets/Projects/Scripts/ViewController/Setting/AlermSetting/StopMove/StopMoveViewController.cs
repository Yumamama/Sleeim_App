using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StopMoveViewController : ViewControllerBase {

	[SerializeField]Toggle IsEnableToggle = null;	//いびきアラームが有効かどうかのトグル

	protected override void Start () {
		base.Start ();
		InitIsEnableToggle ();
	}

	public override SceneTransitionManager.LoadScene SceneTag {
		get {
			return SceneTransitionManager.LoadScene.StopMove;
		}
	}

	//アラームが有効かどうかのトグル初期化
	void InitIsEnableToggle () {
		//保存された設定を適用する
		IsEnableToggle.isOn = AlermTempData.Instance.stopMoveAlermIsEnable;
		//値が変化した際に保存されるようにする
		IsEnableToggle.onValueChanged.AddListener (isEnable => {
			AlermTempData.Instance.stopMoveAlermIsEnable = isEnable;
		});
	}

	//「アラーム設定」ボタンが押されると呼び出される
	public void OnAlermSettingButtonTap () {
		SceneTransitionManager.LoadLevel (SceneTransitionManager.LoadScene.AlermSetting);
	}
}

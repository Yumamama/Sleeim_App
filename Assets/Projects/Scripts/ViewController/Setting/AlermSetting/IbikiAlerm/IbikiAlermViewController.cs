using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class IbikiAlermViewController : ViewControllerBase {

	public Transform settingItemParent;		//各種設定項目の親となるオブジェクトのTransform
	public Toggle ibikiAlermToggle;			//いびきアラームが有効かどうかのトグル
	public Text detectSenseText;			//検知感度のテキスト

	protected override void Start () {
		base.Start ();
		InitIsEnableToggle ();
		InitDetectSense ();
	}

	public override SceneTransitionManager.LoadScene SceneTag {
		get {
			return SceneTransitionManager.LoadScene.IbikiAlerm;
		}
	}

	//いびきアラームが有効かどうかのトグル初期化
	void InitIsEnableToggle () {
		//保存された設定を適用する
		bool isIbikiAlermOn = AlermTempData.Instance.ibikiAlermIsEnable;
		ibikiAlermToggle.isOn = isIbikiAlermOn;			//アラームトグルのON/OFF設定
		ChangeDispAlermSettingItem (isIbikiAlermOn);	//アラームトグル以外の項目の表示/非表示設定
		//値が変化した際に保存されるようにする
		ibikiAlermToggle.onValueChanged.AddListener (isEnable => {
			AlermTempData.Instance.ibikiAlermIsEnable = isEnable;
			ChangeDispAlermSettingItem (isEnable);	//トグルのON/OFF切り替えで、各種設定項目が表示/非表示されるように
		});
	}

	//「検知感度」の選択された項目を設定します
	void InitDetectSense () {
		string text = "";
		switch (AlermTempData.Instance.detectSense) {
		case UserDataManager.Setting.Alerm.IbikiAlerm.DetectSense.Normal:
			text = "普通";
			break;
		case UserDataManager.Setting.Alerm.IbikiAlerm.DetectSense.Large:
			text = "大きめ";
			break;
		}
		detectSenseText.text = text;
	}

	//いびきアラームの各種設定項目の表示・非表示を切り替え
	void ChangeDispAlermSettingItem (bool isDisp) {
		//アラームトグル以外の項目を取得
		//すべての項目のTransformを取得
		var allSettingItem = new List<Transform> ();
		foreach (Transform child in settingItemParent) {
			allSettingItem.Add (child);
		}
		//アラームトグル以外の項目の表示・非表示を設定する
		var alermToggleItemName = ibikiAlermToggle.transform.parent.name;
		allSettingItem = allSettingItem.Where (item => item.name != alermToggleItemName).ToList ();
		foreach (var settingItem in allSettingItem) {
			settingItem.gameObject.SetActive (isDisp);
		}
	}

	//戻るボタンが押されると呼び出される
	public void OnBackButtonTap () {
		SceneTransitionManager.LoadLevel (SceneTransitionManager.LoadScene.AlermSetting);
	}

	//「検知感度」ボタンを押したときに呼び出される
	public void OnDetectSenseTap () {
		SceneTransitionManager.LoadLevel (SceneTransitionManager.LoadScene.DetectSense);
	}
}

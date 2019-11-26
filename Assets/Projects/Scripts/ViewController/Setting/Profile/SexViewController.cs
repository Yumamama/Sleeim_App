using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SexViewController : ViewControllerBase {

	public Toggle maleToggle;
	public Toggle femaleToggle;

	protected override void Start () {
		base.Start ();
		InitSelect ();
	}

	public override SceneTransitionManager.LoadScene SceneTag {
		get {
			return SceneTransitionManager.LoadScene.Sex;
		}
	}

	//男女をすでに選択していれば、再現する
	void InitSelect () {
		switch (UserDataManager.Setting.Profile.GetSex ()) {
		case UserDataManager.Setting.Profile.Sex.Male:
			maleToggle.isOn = true;
			break;
		case UserDataManager.Setting.Profile.Sex.Female:
			femaleToggle.isOn = true;
			break;
		default:
			maleToggle.isOn = false;
			femaleToggle.isOn = false;
			break;
		}
	}

	//「プロフィール」ボタンが押されると呼び出される
	public void OnProfileButtonTap () {
		SceneTransitionManager.LoadLevel (SceneTransitionManager.LoadScene.Profile);
	}

	//「男」の選択トグルの値が変化したときに呼び出される
	public void OnSelectMale (bool isOn) {
		UserDataManager.Setting.Profile.SaveSex (isOn ? UserDataManager.Setting.Profile.Sex.Male : UserDataManager.Setting.Profile.Sex.Unknown);
	}
	//「女」の選択トグルの値が変化したときに呼び出される
	public void OnSelectFemale (bool isOn) {
		UserDataManager.Setting.Profile.SaveSex (isOn ? UserDataManager.Setting.Profile.Sex.Female : UserDataManager.Setting.Profile.Sex.Unknown);
	}
}

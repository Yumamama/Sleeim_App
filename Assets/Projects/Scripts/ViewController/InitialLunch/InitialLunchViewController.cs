using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialLunchViewController : ViewControllerBase {

	protected override void Start () {
		base.Start ();
	}

	public override SceneTransitionManager.LoadScene SceneTag {
		get {
			return SceneTransitionManager.LoadScene.InitialLunch;
		}
	}

	//「利用規約」ボタンが押されると呼び出される
	public void OnTeamsOfUseButtonTap () {
		SceneManager.LoadScene ("TermsOfUse");
	}

	//「プライバシーポリシー」ボタンが押されると呼び出される
	public void OnPrivacyPolicyButtonTap () {
		SceneManager.LoadScene ("PrivacyPolicy");
	}

	//「同意する」ボタンが押されると呼び出される
	public void OnAcceptButtonTap () {
		//規約に同意した事を記録
		UserDataManager.State.SaveAcceptTermOfUse ();
		//利用規約画面は初期起動時以外では表示されないため、常にプロフィール画面に遷移する
		SceneTransitionManager.LoadLevel (SceneTransitionManager.LoadScene.Profile);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TermsOfUse : ViewControllerBase {

	[SerializeField]GameObject prehab;
	[SerializeField]string URL;

	protected override void Start () {
		base.Start ();
		prehab.GetComponent<PopUpWebView> ().Url = URL;
	}

	public override SceneTransitionManager.LoadScene SceneTag {
		get {
			return SceneTransitionManager.LoadScene.TermsOfUse;
		}
	}

	//戻るボタンを押した際に実行される
	public void OnBackButtonTap () {
		SceneTransitionManager.LoadLevel (SceneTransitionManager.LoadScene.InitialLunch);
	}
}

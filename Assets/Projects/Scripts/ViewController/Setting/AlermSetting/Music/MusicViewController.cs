using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kaimin.Managers;

public class MusicViewController : ViewControllerBase {

	public AudioSource audioSource;
	public List<Toggle> selectToggleList;
	public List<MusicPlayElement> musicPlayList;

	[System.Serializable]
	public class MusicPlayElement {
		public string fileName;
		public Toggle toggle;
	}

	protected override void Start () {
		base.Start ();
		//登録したMusicPlayElementについて再生ボタンが押させたら、音楽を再生するように設定する
		foreach (MusicPlayElement element in musicPlayList) {
			element.toggle.onValueChanged.AddListener (isOn => {
				if (isOn) {
					#if UNITY_ANDROID
					var audioClipPath = Kaimin.Common.Utility.MusicTemplatePath () + "/" + element.fileName + ".ogg";
					Debug.Log (audioClipPath);
					NativeManager.Instance.MusicPlay (audioClipPath);
					#elif UNITY_IOS
					var audioClipPath = Kaimin.Common.Utility.MusicTemplatePath () + "/" + element.fileName + ".mp3";
					Debug.Log (audioClipPath);
					var www = new WWW ("file:///" + audioClipPath);
					while (!www.isDone) {
					//読み込み待ち
					}
					audioSource.clip = www.GetAudioClip ();
					audioSource.Play ();
					#endif
				} else {
					#if UNITY_ANDROID
					NativeManager.Instance.MusicStop ();
					#elif UNITY_IOS
					audioSource.Stop ();
					#endif
				}
			});
		}

		//選択中のアラームを選択する
		int selectIndex = UserDataManager.Setting.Alerm.GetSelectAlermIndex ();
		selectToggleList [selectIndex].isOn = true;
	}

	public override SceneTransitionManager.LoadScene SceneTag {
		get {
			return SceneTransitionManager.LoadScene.Music;
		}
	}

	void OnDisable () {
		//他シーンへ遷移する前にアラームを停止する
		#if UNITY_ANDROID
		NativeManager.Instance.MusicStop ();
		#elif UNITY_IOS
		Debug.Log ("TodoMusicStopInIOS");
		#endif
	}

	//アラームのトグルを選択したときに呼び出される
	public void SelectAlerm (int index) {
		UserDataManager.Setting.Alerm.SaveSelectAlermIndex (index);
	}

	//「キャンセル」ボタンが押されると呼び出される
	public void OnBackButtonTap () {
		//どういった動作をするのか未確認。とりあえず前シーンに戻る
		SceneTransitionManager.LoadLevel (SceneTransitionManager.LoadScene.AlermSetting);
	}
}

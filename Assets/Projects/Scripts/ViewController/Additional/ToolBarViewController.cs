using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using Kaimin.Managers;

public class ToolBarViewController : MonoBehaviour {

	public List<BarElement> BarElementList;
	[SerializeField]Image noticeIcon;		//お知らせアイコン
	[SerializeField]Color isOnColor;		//ツールバーのアイコン・文字のアクティブ時のカラー
	[SerializeField]Color isOffColor;		//ツールバーのアイコン・文字の非アクティブ時のカラー

	[System.Serializable]
	public class BarElement {
		public SceneTransitionManager.LoadScene scene;					//遷移先のシーン
		public Toggle toggle;
		public Text iconText;											//ツールバーのアイコンの下に表示される文字(ホーム・グラフなど)
		public List<SceneTransitionManager.LoadScene> allSceneElement;	//指定したタブとつながりのある全てのシーン
	}

	void Start () {
		//現在表示中のシーンを判断し、タブバーに反映させる
		BarElement currentSceneElement = GetCurrentSceneElement ();
		if (currentSceneElement != null) {
			ClearAllToggleCheck ();
			currentSceneElement.toggle.isOn = true;
			currentSceneElement.iconText.color = isOnColor;
		}
		//バー要素を押したときに実行されるイベントを登録する
		SetBarElementsEvent ();
		//設定項目のお知らせアイコンを制御する
		ControllNoticeIconOnSetting ();
		//デバイスとのペアリングが切れた際にアイコンの表示を更新できるように設定する
		DeviceStateManager.Instance.OnDevicePareringDisConnectEvent += ControllNoticeIconOnSetting;
		//ファームウェアの更新が必要になった場合にアイコンの表示を更新できるように設定する
		DeviceStateManager.Instance.OnFarmwareUpdateNecessaryEvent += ControllNoticeIconOnSetting;
		//ファームウェアの更新が必要なくなった場合にアイコンの表示を更新できるように設定する
		DeviceStateManager.Instance.OnFarmwareUpdateNonNecessaryEvent += ControllNoticeIconOnSetting;
	}

	void OnDisable () {
		//デバイスとのペアリング状態購読解除
		DeviceStateManager.Instance.OnDevicePareringDisConnectEvent -= ControllNoticeIconOnSetting;
		//ファームウェアの更新が必要になった場合のイベント購読解除
		DeviceStateManager.Instance.OnFarmwareUpdateNecessaryEvent -= ControllNoticeIconOnSetting;
		//ファームウェアの更新が必要なくなった場合のイベント購読解除
		DeviceStateManager.Instance.OnFarmwareUpdateNonNecessaryEvent -= ControllNoticeIconOnSetting;
	}

	//バー要素を押したときに実行されるイベントを登録する
	void SetBarElementsEvent () {
		foreach (BarElement element in BarElementList) {
			element.toggle.onValueChanged.AddListener (isOn =>  {
				if (isCurrentScene (element.scene))
					return;		//目的のシーンにいる場合は遷移しないようにする
				SceneTransitionManager.LoadLevel (element.scene);
			});
		}
	}

	void ClearAllToggleCheck () {
		foreach (BarElement element in BarElementList) {
			element.toggle.isOn = false;
			element.iconText.color = isOffColor;
		}
	}

	//現在表示中のシーンから、当てはまるタブ要素を取得する
	BarElement GetCurrentSceneElement () {
		//現在表示中のシーンのリスト取得
		List<Scene> currentScenes = Enumerable.Range (0, SceneManager.sceneCount - 1).Select (count => SceneManager.GetSceneAt (count)).ToList ();
		foreach (BarElement element in BarElementList) {
			foreach (string currentSceneName in currentScenes.Select (scene => scene.name)) {
				if (element.allSceneElement.Select (scene => SceneTransitionManager.GetSceneName (scene)).Contains (currentSceneName))
					return element;
			}
		}
		return null;
	}

	//指定したシーンが現在表示されているかどうか
	bool isCurrentScene (SceneTransitionManager.LoadScene scene) {
		return GetCurrentSceneElement ().scene.Equals (scene);
	}

	//設定項目にお知らせアイコンを表示するかどうか
	bool isDispSettingNoticeIcon () {
		//機器と未ペアリング状態、またはファームウェア更新ありの場合アイコンを表示する
		bool isPareringDevice = UserDataManager.State.isDoneDevicePareing ();
		bool isExistFarmwareUpdate = UserDataManager.Device.IsExistFirmwareVersionDiff ();
		return !isPareringDevice || isExistFarmwareUpdate;
	}

	//設定項目のお知らせアイコンを制御する
	void ControllNoticeIconOnSetting () {
		noticeIcon.enabled = isDispSettingNoticeIcon ();
	}
}

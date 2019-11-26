using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Kaimin.Managers;
using MiniJSON;
using System;
using System.Linq;

public class AlermSettingViewController : ViewControllerBase {

	[SerializeField]Transform settingItemParent = null;	//各種設定項目の親となるオブジェクトのTransform
	[SerializeField]Toggle alermIsEnableToggle = null;	//アラームが有効かどうかのトグル
	bool isAppForeground;		//iOS専用。アプリがフォアグラウンドにいるかどうか

	protected override void Start () {
		base.Start ();
		InitIsEnableToggle ();
	}

	public override SceneTransitionManager.LoadScene SceneTag {
		get {
			return SceneTransitionManager.LoadScene.AlermSetting;
		}
	}

	public override bool IsAllowSceneBack () {
		if (DialogBase.IsDisp ()) {
			//ダイアログ表示中であれば、戻れないように
			return false;
		} else {
			//一時保存データを削除する
			AlermTempData.ClearData ();
			return true;
		}
	}

	//アラームが有効かどうかのトグル初期化
	void InitIsEnableToggle () {
		//アラームトグルの値が変更された際の処理を登録する
		alermIsEnableToggle.onValueChanged.AddListener (isEnable => {
			OnAlermToggleValueChanged (isEnable);
		});
		//保存された設定を適用する
		alermIsEnableToggle.isOn = AlermTempData.Instance.alermIsEnable;
		//アラームトグルのON・OFFで項目を表示・非表示する
		ChangeDispAlermSettingItem (alermIsEnableToggle.isOn);
	}

	//アラームトグルの値が変化した際の処理
	//コルーチンを使いたい、且つトグルのコールバックに設定するために処理をまとめる
	void OnAlermToggleValueChanged (bool isEnable) {
		//値が変化した際に保存されるようにする
		AlermTempData.Instance.alermIsEnable = isEnable;
		//値が変化した際に設定項目の表示・非表示が切り替わるようにする
		ChangeDispAlermSettingItem (isEnable);
		if (isEnable)
			StartCoroutine (AlermNotificationCheck ());
	}

	//アラーム通知の設定を確認する
	//通知の許可がなければ許可を求める
	IEnumerator AlermNotificationCheck () {
		//アラームがONにされた際に通知許可があるかどうか確認する
		#if UNITY_IOS
		//iOSのみ通知許可の確認前に準備が必要
		bool isCompleteCheckPrepare = false;
		BluetoothManager.Instance.NotificationCheckPrepare (() => isCompleteCheckPrepare = true);
		yield return new WaitUntil (() => isCompleteCheckPrepare);	//通知許可の確認準備ができるまで待機
		#endif
		if (NativeManager.Instance.NotificationCheck ()) {
			//通知の許可があれば
			yield break;
		}
		//通知の許可がなければ、ユーザーに設定するか尋ねるダイアログを表示する
		bool isSetting = false;
		yield return StartCoroutine (AskSettingAlermNotification ((bool _isSetting) => isSetting = _isSetting));
		if (!isSetting) {
			//なにもしない
			yield break;
		}
		//設定するなら設定画面を開く
		NativeManager.Instance.NotificationRequest ();
		bool isAllowNotification = false;
		#if UNITY_ANDROID
		yield return new WaitUntil (() => NativeManager.Instance.NotificationRequestResultCode != -1);
		isAllowNotification = NativeManager.Instance.NotificationRequestResultCode == 1;
		#elif UNITY_IOS
		isAllowNotification = true;		//コールバックが受け取れないため許可されたと仮定して進める
		#endif
		if (isAllowNotification) {
			yield break;
		}
		//通知が許可されなければ、アラームの通知許可が設定されなかった事をユーザーに伝える
		yield return StartCoroutine (TellAlermNotificationIsNotSetting ());
	}

	//通知の許可がなかった際に、ユーザーに通知の設定をするかどうか尋ねる
	IEnumerator AskSettingAlermNotification (Action<bool> onResponse) {
		bool isOK = false;
		bool isCancel = false;
		MessageDialog.Show ("アラームを使用するには、通知を許可に設定してください。", true, true, () => isOK = true, () => isCancel = true, "設定", "キャンセル");
		yield return new WaitUntil (() => isOK || isCancel);
		onResponse (isOK);
	}

	//アラームの通知許可が設定されなかった事をユーザーに伝える
	IEnumerator TellAlermNotificationIsNotSetting () {
		bool isOK = false;
		MessageDialog.Show ("アラームの通知許可が設定されませんでした。", true, false, () => isOK = true);
		yield return new WaitUntil (() => isOK);
	}

	//アラームの各種設定項目を表示・非表示を切り替え
	void ChangeDispAlermSettingItem (bool isDisp) {
		//アラームトグル以外の全ての項目を取得
		//すべての項目のTransformを取得
		var allSettingItem = new List<Transform> ();
		foreach (Transform child in settingItemParent) {
			allSettingItem.Add (child);
		}
		//アラームトグル以外の項目の表示・非表示を設定する
		var alermToggleItemName = alermIsEnableToggle.transform.parent.name;
		allSettingItem = allSettingItem.Where (item => item.name != alermToggleItemName).ToList ();
		foreach (var settingItem in allSettingItem) {
			settingItem.gameObject.SetActive (isDisp);
		}
	}

	//戻るボタンを押したときに呼び出される
	public void OnToSettingButtonTap () {
		//一時保存データを削除する
		AlermTempData.ClearData ();
		SceneTransitionManager.LoadLevel (SceneTransitionManager.LoadScene.Setting);
	}

	//「保存」ボタンを押したときに呼び出される
	public void OnSaveButtonTap () {
		//設定した値でデータを保存する
		Debug.Log ("Start SendCommandAlerm");
		StartCoroutine (SendAlermSetting ());
	}

	//ペアリング出来てない事をユーザーに伝える
	IEnumerator TellNotParering () {
		bool isOK = false;
		MessageDialog.Show (
			"本体機器とのペアリングが完了していないため、処理を行えません。\n本体機器とのペアリングを行ってください。",
			true,
			false,
			() => isOK = true,
			null,
			"OK",
			"キャンセル");
		yield return new WaitUntil (() => isOK);
	}

	//アラーム設定をデバイスに転送する
	IEnumerator SendAlermSetting () {
		//通知が許可されているかのチェックを行う
		//アラームがONにされた際に通知許可があるかどうか確認する
		#if UNITY_IOS
		//iOSのみ通知許可の確認前に準備が必要
		bool isCompleteCheckPrepare = false;
		BluetoothManager.Instance.NotificationCheckPrepare (() => isCompleteCheckPrepare = true);
		yield return new WaitUntil (() => isCompleteCheckPrepare);	//通知許可の確認準備ができるまで待機
		#endif
		if (NativeManager.Instance.NotificationCheck ()) {
			//通知の許可があれば以降の処理を行う
		} else {
			//通知の許可がなければ、ユーザーに設定するか尋ねるダイアログを表示する
			bool isSetting = false;
			yield return StartCoroutine (AskSettingAlermNotification ((bool _isSetting) => isSetting = _isSetting));
			if (!isSetting) {
				//設定しないなら、以降の処理を実行しないようにする
				yield break;
			}
			//設定するなら設定画面を開く
			NativeManager.Instance.NotificationRequest ();
			bool isAllowNotification = false;
#if UNITY_ANDROID
            yield break;

            //yield return new WaitUntil (() => NativeManager.Instance.NotificationRequestResultCode != -1);
            //isAllowNotification = NativeManager.Instance.NotificationRequestResultCode == 1;
            //if (isAllowNotification) {
            //	//通知が許可されれば、以降の処理を続けて行う
            //} else {
            //	//通知が許可されなければ、アラームの通知許可が設定されなかった事をユーザーに伝えて以降の処理を行わない
            //	yield return StartCoroutine (TellAlermNotificationIsNotSetting ());
            //	yield break;
            //}
#elif UNITY_IOS
			//通知が許可されたかどうかわからないため、今回は以降の処理を行わない
			yield break;
#endif
        }
        //Bluetoothが有効かのチェックを行う
        bool isBluetoothActive = false;
		yield return StartCoroutine (BluetoothActiveCheck ((bool isActive) => isBluetoothActive = isActive));
		if (!isBluetoothActive) {
			yield break;	//接続エラー時に以降のBle処理を飛ばす
		}
		//ペアリング済みか確認
		if (UserDataManager.State.isDoneDevicePareing ()) {
			//デバイスと接続
			if (!UserDataManager.State.isConnectingDevice ()) {
				string deviceName = UserDataManager.Device.GetPareringDeviceName ();
				string deviceAdress = UserDataManager.Device.GetPareringBLEAdress ();
				bool isDeviceConnectSuccess = false;
				yield return StartCoroutine (DeviceConnect (deviceName, deviceAdress, (bool isSuccess) => isDeviceConnectSuccess = isSuccess));
				Debug.Log ("Connecting_Result:" + isDeviceConnectSuccess);
				if (!isDeviceConnectSuccess) {
					//デバイス接続に失敗すれば
					yield break;
				}
			}
		} else {
			yield return StartCoroutine (TellNotParering ());
			yield break;
		}
		UpdateDialog.Show ("同期中");
		int alermSet = AlermTempData.Instance.alermIsEnable ? 1 : 0;					//アラーム有効/無効
		int ibikiAlermSet = AlermTempData.Instance.ibikiAlermIsEnable ? 1 : 0;			//いびきアラーム有効/無効
		int ibikiAlermSense = AlermTempData.Instance.detectSense == UserDataManager.Setting.Alerm.IbikiAlerm.DetectSense.Large ? 1 : 0;	//アラーム感度
		int lowBreathAlermSet = AlermTempData.Instance.lowBreathAlermIsEnable ? 1 : 0;	//低呼吸アラーム有効/無効
		int alermDelay = (int)AlermTempData.Instance.delayTime;							//アラーム遅延
		int stopMove = AlermTempData.Instance.stopMoveAlermIsEnable ? 1 : 0;			//体動停止有効/無効
		int callTime = (int)AlermTempData.Instance.callTime;							//鳴動時間

		bool isSendCommandAlermSuccess = false;
		bool isSendCommandAlermFailed = false;
		BluetoothManager.Instance.SendCommandAlarm (
			alermSet, 
			ibikiAlermSet, 
			ibikiAlermSense, 
			lowBreathAlermSet, 
			alermDelay, 
			stopMove, 
			callTime, 
			(string data) => {
				//エラー時
				Debug.Log ("SendCommandAlerm_OnError:" + data);
				isSendCommandAlermFailed = true;
			}, 
			(bool success) => {
				//コマンド書き込み結果	
				Debug.Log ("SendCommandAlerm_Success:" + success);
				if (!success) 
					isSendCommandAlermFailed = true;
			}, 
			(string data) => {
				//応答結果
				Debug.Log ("SendCommandAlerm_OnResponse:" + data);
				var json = Json.Deserialize (data) as Dictionary<string, object>;
				bool response = Convert.ToBoolean (json["KEY2"]);
				if (response)
					isSendCommandAlermSuccess = true;
				else
					isSendCommandAlermFailed = true;
			});
		yield return new WaitUntil (() => isSendCommandAlermSuccess || isSendCommandAlermFailed);
		UpdateDialog.Dismiss ();
		if (isSendCommandAlermSuccess) {
			//アラーム設定が成功なら
			Debug.Log ("SendCommandAlerm Success!!");
			//デバイスの設定値を更新する
			UserDataManager.Setting.Alerm.SaveIsEnable (AlermTempData.Instance.alermIsEnable);
			UserDataManager.Setting.Alerm.IbikiAlerm.SaveIsEnable (AlermTempData.Instance.ibikiAlermIsEnable);
			UserDataManager.Setting.Alerm.IbikiAlerm.SaveDetectSense (AlermTempData.Instance.detectSense);
			UserDataManager.Setting.Alerm.LowBreathAlerm.SaveIsEnable (AlermTempData.Instance.lowBreathAlermIsEnable);
			UserDataManager.Setting.Alerm.SaveDelayTime (AlermTempData.Instance.delayTime);
			UserDataManager.Setting.Alerm.StopMoveAlerm.SaveIsEnable (AlermTempData.Instance.stopMoveAlermIsEnable);
			UserDataManager.Setting.Alerm.SaveCallTime (AlermTempData.Instance.callTime);
		} else {
			//アラーム設定が失敗なら
			Debug.Log ("SendCommandAlerm Failed...");
			//失敗した事を伝えるダイアログを表示する
			yield return StartCoroutine (TellFailedChangeAlermSetting ());
		}
	}

	//デバイスと接続できなかった事をユーザーに伝える
	IEnumerator TellFailedConnect (string deviceName) {
		bool isOk = false;
		MessageDialog.Show ("<size=32>" + deviceName + "と接続できませんでした。</size>", true, false, () => isOk = true);
		yield return new WaitUntil (() => isOk);
	}

	//デバイス接続の流れ
	IEnumerator DeviceConnect (string deviceName, string deviceAdress, Action<bool> onResponse) {
		UpdateDialogAddButton.Show (deviceName + "に接続しています。",
			false,
			true,
			null,
			() => {
				//キャンセルボタン押下時
				//デバイスとの接続を切る
				BluetoothManager.Instance.Disconnect ();
			},
			"OK",
			"キャンセル");
		bool isSuccess = false;	//接続成功
		bool isFailed = false;	//接続失敗
		string receiveData = "";		//デバイス接続で成功・失敗時に受け取るデータ（JSONにパースして使用）
		string uuid = "";		//ペアリング中のデバイスのUUID(iOSでのみ必要)
		#if UNITY_IOS
		uuid = UserDataManager.Device.GetPareringDeviceUUID ();
		#endif

		BluetoothManager.Instance.Connect (
			deviceAdress, 
			(string data) => {
				//エラー時
				receiveData = data;
				isFailed = true;
			},
			(string data) => {
				//接続完了時
				receiveData = data;
				isSuccess = true;
			},
			uuid);
		yield return new WaitUntil (() => isSuccess || isFailed);	//応答待ち
		if (isSuccess) {
			//接続成功時
			//接続したデバイス情報読み出し
			var json = Json.Deserialize (receiveData) as Dictionary<string, object>;
			string name = (string)json["KEY1"];
			string adress = (string)json["KEY2"];
			//接続したデバイスを記憶しておく
			UserDataManager.Device.SavePareringBLEAdress (adress);
			UserDataManager.Device.SavePareringDeviceName (name);
			UpdateDialogAddButton.Dismiss ();
		} else {
			//接続失敗時
			var json = Json.Deserialize (receiveData) as Dictionary<string, object>;
			int error1 = Convert.ToInt32 (json["KEY1"]);
			int error2 = Convert.ToInt32 (json["KEY2"]);
			UpdateDialogAddButton.Dismiss ();
			#if UNITY_IOS
			if (error2 == -8) {
			//iOSの_reconnectionPeripheralコマンドでのみ返ってくる、これ以降接続できる見込みがないときのエラー
			Debug.Log ("Connect_OcuurSeriousError");
			//接続を解除
			UserDataManager.State.SaveDeviceConnectState (false);
			//接続が解除された事を伝える
			DeviceStateManager.Instance.OnDeviceDisConnect ();
			//ペアリングを解除
			UserDataManager.State.ClearDeviceParering ();
			//ペアリングが解除されたことを伝える
			DeviceStateManager.Instance.OnDevicePrearingDisConnect ();
			//接続に失敗した旨のダイアログを表示
			yield return StartCoroutine (TellFailedConnect (deviceName));
			//再度ペアリングを要求するダイアログを表示
			yield return StartCoroutine (TellNeccesaryParering ());
            onResponse(false);
			yield break;
			}
			#endif
			if (error2 == -3)	//何らかの原因で接続できなかった場合(タイムアウト含む)
				Debug.Log ("OccurAnyError");
			else if (error2 == -4)	//接続が切れた場合(GATTサーバには接続できたが、サービスまで全て接続できないと接続完了にはならない。)
				Debug.Log ("DisConnectedError");
			//接続に失敗した旨のダイアログを表示
			yield return StartCoroutine (TellFailedConnect (deviceName));
		}
		onResponse (isSuccess);
	}

	//再ペアリングが必要な事をユーザーに伝える
	IEnumerator TellNeccesaryParering () {
		bool isOK = false;
		MessageDialog.Show ("再度ペアリング設定を行ってください。", true, false, () => isOK = true);
		yield return new WaitUntil (() => isOK);
	}

	//bluetoothが有効になっているかどうか確認する
	IEnumerator BluetoothActiveCheck (Action<bool> onResponse) {
		NativeManager.Instance.Initialize ();
		bool isActive = NativeManager.Instance.BluetoothValidCheck ();
		if (!isActive) {
			//無効になっているため、設定画面を開くかどうかのダイアログを表示する
			bool isSetting = false;
			yield return StartCoroutine (AskOpenSetting ((bool _isSetting) => isSetting = _isSetting));
			if (isSetting) {
				//Bluetoothを有効にするなら
				NativeManager.Instance.BluetoothRequest ();
				#if UNITY_ANDROID
				yield return new WaitUntil (() => NativeManager.Instance.PermissionCode > 0);
				isActive = NativeManager.Instance.PermissionCode == 1;
				#elif UNITY_IOS
				isActive = false;	//iOSの場合、ユーザーの選択が受け取れなかったため、拒否された前提で進める
				#endif
				if (isActive) {
					//Bluetoothが有効になったら
				} else {
					//Bluetoothが有効にされなかったら
					//ダイアログを閉じるだけ
				}
			} else {
				//Bluetoothが有効にされなかったなら
				isActive = false;
			}
		}
		onResponse (isActive);
		yield return null;
	}

	//端末の設定画面を開くかどうかユーザーに尋ねる
	IEnumerator AskOpenSetting (Action<bool> onResponse) {
		bool isSetting = false;
		bool isCancel = false;
		MessageDialog.Show ("<size=30>Bluetoothがオフになっています。\nSleeimと接続できるようにするには、\nBluetoothをオンにしてください。</size>", 
			true, 
			true, 
			() => isSetting = true, 
			() => isCancel = true,
			"設定",
			"キャンセル");
		yield return new WaitUntil (() => isSetting || isCancel);
		onResponse (isSetting);
	}

	//デバイスのアラーム設定変更に失敗した事をユーザーに伝えるダイアログを表示する
	IEnumerator TellFailedChangeAlermSetting () {
		bool isOk = false;
		MessageDialog.Show ("設定変更に失敗しました。", true, false, () => isOk = true);
		yield return new WaitUntil (() => isOk);
	}

	//「いびきアラーム」ボタンを押したときに呼び出される
	public void OnIbikiAlermButtonTap () {
		SceneTransitionManager.LoadLevel (SceneTransitionManager.LoadScene.IbikiAlerm);
	}

	//「低呼吸アラーム」ボタンを押したときに呼び出される
	public void OnLowBreathAlermButtonTap () {
		SceneTransitionManager.LoadLevel (SceneTransitionManager.LoadScene.LowBreathAlerm);
	}

	//「バイブレーション」ボタンを押したときに呼び出される
	public void OnVibrationButtonTap () {
		SceneTransitionManager.LoadLevel (SceneTransitionManager.LoadScene.Vibration);
	}

	//「音楽」ボタンを押したときに呼び出される
	public void OnMusicButtonTap () {
		SceneTransitionManager.LoadLevel (SceneTransitionManager.LoadScene.Music);
	}

	//「フェードイン」ボタンを押したときに呼び出される
	public void OnFeedInButtonTap () {
		SceneTransitionManager.LoadLevel (SceneTransitionManager.LoadScene.FeedIn);
	}

	//「体動停止」ボタンを押したときに呼び出される
	public void OnStopMoveButtonTap () {
		SceneTransitionManager.LoadLevel (SceneTransitionManager.LoadScene.StopMove);
	}

	//「鳴動時間」ボタンを押したときに呼び出される
	public void OnCallTimeButtonTap () {
		SceneTransitionManager.LoadLevel (SceneTransitionManager.LoadScene.CallTime);
	}

	//「アラーム開始遅延時間」ボタンを押したときに呼び出される
	public void OnDelayTimeButtonTap () {
		SceneTransitionManager.LoadLevel (SceneTransitionManager.LoadScene.DelayTime);
	}
}

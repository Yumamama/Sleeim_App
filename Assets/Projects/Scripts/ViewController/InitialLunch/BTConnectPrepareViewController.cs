using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kaimin.Managers;
using naichilab.InputEvents;
using System.Linq;

public class BTConnectPrepareViewController : ViewControllerBase {

	//説明画像を切り替えるトグルのリスト
	//indexが大きくなるごとに右にある画像という想定
	[SerializeField] List<Toggle> explainImageToggleList;		

	protected override void Start () {
		base.Start ();
		StartCoroutine (InitialCheck ());
	}

	public override SceneTransitionManager.LoadScene SceneTag {
		get {
			return SceneTransitionManager.LoadScene.BTConnectPrepare;
		}
	}

	public override bool IsAllowSceneBack () {
		//初回起動時に表示される接続準備画面ではバックボタンを使えないようにする
		if (DialogBase.IsDisp ()) {
			//ダイアログ表示中であれば、戻れない
			return false;
		} else {
			//スプラッシュから遷移してきた場合はバックボタン無効
			if (UserDataManager.State.isInitialLunch ()) {
				return false;
			}
			//設定画面から遷移してきた場合はバックボタンで戻れるように
			else {
				return true;
			}
		}
	}

	void OnEnable () {
		//タッチマネージャーのイベントリスナーを設定
		TouchManager.Instance.FlickComplete += OnFlickComplete;
	}

	void OnDisable () {
		//後処理
		TouchManager.Instance.FlickComplete -= OnFlickComplete;
	}

	//コマンドを使用する前提条件が整っているか確認・要求
	IEnumerator InitialCheck () {
		//BLE対応しているか確認
		yield return StartCoroutine (CheckBleSupport ());
		//Bluetoothが有効か確認
		yield return StartCoroutine (CheckBluetoothIsActive ());
	}

	//端末がBluetoothLEに対応しているか確認
	IEnumerator CheckBleSupport () {
		NativeManager.Instance.Initialize ();
		bool isSupport = NativeManager.Instance.BlesupportCheck ();
		if (!isSupport) {
			//端末がBluetoothに対応していなければ、それを伝えるダイアログを表示
			bool isOK = false;
			MessageDialog.Show ("お使いの端末はBluetooth LEをサポートしておりません。", true, false, () => isOK = true);
			yield return new WaitUntil (() => isOK);
			//ホームに遷移
			SceneTransitionManager.LoadLevel (SceneTransitionManager.LoadScene.Home);
		}
		yield return null;
	}

	//Bluetoothが有効になっているかどうか確認
	IEnumerator CheckBluetoothIsActive () {
		NativeManager.Instance.Initialize ();
		bool isActive = NativeManager.Instance.BluetoothValidCheck ();
		if (isActive) {
			//Bluetoothが有効なら
			yield return null;
		} else {
			//無効の場合は無効の旨を表示し、システム設定の変更を促す
			yield return StartCoroutine (BluetoothActivateCheck ());
		}
	}

	//Bluetoothが非アクティブの際にアクティブ化するためのダイアログを表示
	IEnumerator BluetoothActivateCheck () {
		bool isUpdate = false;
		bool isSkip = false;
		MessageDialog.Show (
			"<size=30>Bluetoothがオフになっています。\nSleeimと接続できるようにするには、\nBluetoothをオンにしてください。</size>", 
			true, 
			true, 
			() => isUpdate = true, 
			() => isSkip = true,
			"更新",
			"スキップ");
		yield return new WaitUntil (() => isUpdate || isSkip);
		Debug.Log ("isUpdate:" + isUpdate + "," + "isSkip:" + isSkip);
		if (isUpdate) {
			//Bluetooth有効化リクエストを発行する。有効にするかのダイアログが表示される
			NativeManager.Instance.BluetoothRequest ();
			bool isActivate = false;
			//ユーザの入力待ち
			#if UNITY_ANDROID
			yield return new WaitUntil (() => NativeManager.Instance.PermissionCode != -1);
			isActivate = NativeManager.Instance.PermissionCode == 1;	//Bluetoohが有効にされたか
			#elif UNITY_IOS
			isActivate = false;	//iOSの場合、ユーザーの選択が受け取れなかったため、拒否された前提で進める
			#endif
			if (isActivate) {
				//Bluetoothが有効になったら
			} else {
				//Bluetoothが有効にされなかったら
				//ダイアログを閉じるだけ
			}
			yield return null;
		} else {
			//ペアリングをスキップする
			yield return StartCoroutine (SkipParering ());
		}
	}
		
	//ペアリングをスキップする
	IEnumerator SkipParering () {
		//本当にスキップするか確認
		bool isOK = false;
		bool isCancel = false;
		MessageDialog.Show (
			"本体機器とのペアリングが完了していません。接続をスキップしますか？", 
			true, 
			true, 
			() => {
				isOK = true;
			}, 
			() => {
				isCancel = true;
			});
		yield return new WaitUntil (() => isOK || isCancel);
		if (isOK) {
			SceneTransitionManager.LoadLevel (SceneTransitionManager.LoadScene.Home);
		} else {
			//スキップしないなら再度はじめから再開？自分のシーンを再読み込み
			SceneTransitionManager.LoadLevel (SceneTransitionManager.LoadScene.BTConnectPrepare);
		}
		yield return null;
	}

	//「次へ」ボタンが押されると呼び出される
	public void OnNextButtonTap () {
		//Bluetooth接続画面へ遷移
		SceneTransitionManager.LoadLevel (SceneTransitionManager.LoadScene.BTConnect);
	}

	//「スキップ」ボタンが押されると呼び出される
	public void OnSkipButtonTap () {
		StartCoroutine (SkipParering ());
	}

	void OnFlickComplete (object sender, FlickEventArgs e) {
		string text = string.Format ("OnFlickComplete [{0}] Speed[{1}] Accel[{2}] ElapseTime[{3}]", new object[] {
			e.Direction.ToString (),
			e.Speed.ToString ("0.000"),
			e.Acceleration.ToString ("0.000"),
			e.ElapsedTime.ToString ("0.000")
		});
		Debug.Log (text);

		if (e.Direction == FlickEventArgs.Direction4.Left)
			ToRightExplainImage ();
		else if (e.Direction == FlickEventArgs.Direction4.Right)
			ToLeftExplainImage ();
	}

	//説明画面の画像を右に送る
	void ToRightExplainImage () {
		//現在表示中の画像の、トグルリストでのインデックスを取得
		int currentIndex = explainImageToggleList
			.Select ((toggle, index) => new {IsOn = toggle.isOn, Index = index})
			.Where (data => data.IsOn)
			.Select (data => data.Index)
			.First ();
		Debug.Log ("ToRight_Index:" + currentIndex);
		//もし右にまだ画像があれば
		if (explainImageToggleList.Count > currentIndex + 1) {
			//右のトグルを選択状態にする
			explainImageToggleList.ElementAt (currentIndex + 1).isOn = true;
		}
	}

	//説明画面の画像を左に送る
	void ToLeftExplainImage () {
		//現在表示中の画像の、トグルリストでのインデックスを取得
		int currentIndex = explainImageToggleList
			.Select ((toggle, index) => new {IsOn = toggle.isOn, Index = index})
			.Where (data => data.IsOn)
			.Select (data => data.Index)
			.First ();
		Debug.Log ("ToLeft_Index:" + currentIndex);
		//もし左にまだ画像があれば
		if (currentIndex > 0) {
			//左のトグルを選択状態にする
			explainImageToggleList.ElementAt (currentIndex - 1).isOn = true;
		}
	}
}

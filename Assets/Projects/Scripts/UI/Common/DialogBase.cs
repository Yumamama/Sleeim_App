using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// ダイアログ共通の処理をまとめたベースクラス
/// </summary>
public abstract class DialogBase : MonoBehaviour {

	//現在表示しているダイアログのオブジェクト
	protected static GameObject dialogObj;

	/// <summary>
	/// 現在ダイアログを表示中かどうか
	/// </summary>
	public static bool IsDisp () {
		return dialogObj != null;
	}

	/// <summary>
	/// ダイアログのインスタンスを作成します。
	/// インスタンスは表示されてるシーンの中で最上位のCanvasの中に作成されます。
	/// </summary>
	/// <returns>The dialog.</returns>
	protected static GameObject CreateDialog (string prehabPath) {
		//前に表示していたダイアログが残っており、重複するなら
		if (dialogObj != null) {
			//前のダイアログを閉じる
			Dismiss ();
		}
		GameObject prehab = GameObject.Instantiate (Resources.Load (prehabPath) as GameObject);
		Canvas canvas = GetUpperLayerCanvas ();
		//初期化
		//Transform設定
		prehab.transform.parent = canvas.transform;
		prehab.transform.localPosition = Vector3.zero;
		prehab.transform.localScale = Vector3.one;
		//ダイアログの背景を画面全体に広げる(ダイアログより背面のボタンなどをタップできなくするために)
		var rectTransform = prehab.GetComponent<RectTransform> ();
		rectTransform.offsetMin = new Vector2 (0, 0);
		rectTransform.offsetMax = new Vector2 (0, 0);
		dialogObj = prehab;
		return prehab;
	}

	/// <summary>
	/// ダイアログを閉じます
	/// </summary>
	public static void Dismiss () {
		if (dialogObj != null)
			DestroyImmediate (dialogObj);
	}

	//すべてのシーンの中で最前面に表示されているCanvasを取得する
	static Canvas GetUpperLayerCanvas () {
		var upperLayerScene = GetUpperLayerScene ();
		//シーンの中からCanvasを取得する
		var canvasObj = upperLayerScene.GetRootGameObjects ().Where (obj => obj.name == "Canvas").First ();
		return canvasObj.GetComponent<Canvas> ();
	}

	//すべてのシーンの中で最前面に表示されてるCanvasを持つシーンを取得する
	static Scene GetUpperLayerScene () {
		var uiSceneSortOrders = new List<Tuple<int, int>> ();	//Canvasが存在するシーンの表示レイヤーの情報を持たせたリスト (SceneIndex, SortOrder)
		//すべてのシーンからUI表示しているシーンをすべて取得する
		for (int i = 0; i < SceneManager.sceneCount; i++) {
			var scene = SceneManager.GetSceneAt (i);
			var rootGameObjects = scene.GetRootGameObjects ();
			//ルートのオブジェクトのなかにCanvasがあるか調べる
			bool isExistCanvas = rootGameObjects.Where (obj => obj.name == "Canvas").Count () > 0;
			if (isExistCanvas) {
				var canvasObj = rootGameObjects.Where (obj => obj.name == "Canvas").First ();
				var sortOrder = canvasObj.GetComponent<Canvas> ().sortingOrder;
				uiSceneSortOrders.Add (new Tuple<int, int> (i, sortOrder));
			}
		}
		//UI表示しているシーンの中で最も最前面に表示されてるシーンを探す
		int maxSortOrder = -1;
		int maxSortOrderSceneIndex = -1;
		foreach (var uiSceneSortOrder in uiSceneSortOrders) {
			if (uiSceneSortOrder.Item2 > maxSortOrder) {
				maxSortOrderSceneIndex = uiSceneSortOrder.Item1;
				maxSortOrder = uiSceneSortOrder.Item2;
			}
		}
		if (maxSortOrder == -1) {
			//最前面のシーンが見つからなければ
			return SceneManager.GetActiveScene ();
		}
		//最前面のシーンが見つかれば
		return SceneManager.GetSceneAt (maxSortOrderSceneIndex);
	}
}

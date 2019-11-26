using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

/// <summary>
/// どれか一つのボタンのみがアクティブになるUIを設定するためのコンポーネント
/// </summary>
public class Switcher : MonoBehaviour {

	public List<SwitcherElement> ElementList;

	[System.Serializable]
	public class SwitcherElement {
		public Button ButtonElement;
		public Graphic HeadElement;	//テキストやアイコンなど、見出しとなる要素
		public Image BackElement;	//背景となる要素
		public Color HeadElementActiveColor;
		public Color headElementDisActiveColor;
		public Color BackElementActiveColor;
		public Color backElementDisActiveColor;
		public Button.ButtonClickedEvent OnSelectedMe;		//自分が選択された際の処理を登録
		public Button.ButtonClickedEvent OnSelectedOther;	//自分以外が選択された際の処理を登録

		/// <summary>
		/// 要素がタップされた際に実行されるコールバックメソッドを設定します
		/// </summary>
		/// <param name="OnElementTourch">On element tourch.</param>
		public void SetCallbackOnElementTourch (Action<SwitcherElement> OnElementTourch) {
            if (ButtonElement != null)
            {
                ButtonElement.onClick.AddListener(() => OnElementTourch(this));
            }
		}

        /// <summary>
        ///	要素をアクティブ・非アクティブに設定します
        /// </summary>
        /// <param name="isActive">If set to <c>true</c> is active.</param>
        public void SetElementActive(bool isActive) {
            if (HeadElement != null) {
                HeadElement.color = isActive ? HeadElementActiveColor : headElementDisActiveColor;
            }
            
            if (BackElement != null) {
				BackElement.color = isActive ? BackElementActiveColor : backElementDisActiveColor;
			}
		}
	}

	void Start () {
		//すべての要素に対して、タップされた際のコールバックを設定します
		foreach (var element in ElementList) {
			element.SetCallbackOnElementTourch (OnElementTourch);
		}
	}

	//ある要素がタップされた際に実行される
	void OnElementTourch (SwitcherElement tourchedElement) {
		foreach (var element in ElementList) {
			if (element == tourchedElement) {
				element.SetElementActive (true);
				element.OnSelectedMe.Invoke ();
			} else {
				element.SetElementActive (false);
				element.OnSelectedOther.Invoke ();
			}
		}
	}

	/// <summary>
	/// アイテムの番号を指定して選択する
	/// </summary>
	/// <param name="index">アイテム番号(0はじまり)</param>
	public void Select (int index) {
		if (index >= 0 && index < ElementList.Count) {
			OnElementTourch (ElementList [index]);
		} else {
			Debug.Log ("SelectSwitchOutOfIndex...");
		}
	}
}

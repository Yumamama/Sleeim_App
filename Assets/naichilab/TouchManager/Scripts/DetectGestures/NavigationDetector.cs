using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace naichilab.InputEvents {
	/// <summary>
	/// 画面下部にあるナビゲーションバーでの入力操作を受け付けるクラス
	/// </summary>
	public class NavigationDetector : MonoBehaviour, IGestureDetector {
		//Updateと同じイメージでOK
		public void Enqueue (CustomInput currentInput) {
			//ナビゲーションバーが操作されたときの動作を記述する
			//もしバックボタンが押されたら、TouchManagerのOnTouchBack ()を実行みたいな
			if (currentInput.IsNavigationBackDown) {
				TouchManager.Instance.OnTouchNavigationBack (currentInput);
			}
		}
	}
}

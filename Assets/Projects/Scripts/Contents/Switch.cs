using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// iphoneっぽいスイッチを動作させるために必要なコンポーネント
/// アニメーションを動作させるのに使用
/// </summary>
public class Switch : MonoBehaviour {

	[SerializeField]Animator switchAnimator;	//スイッチをコントロールするためのAnimator

	//アニメーターの状態を切り替えてスイッチを動かす
	public void SetSwitchState (bool isOn) {
		switchAnimator.SetBool ("isOn", isOn);
	}
}

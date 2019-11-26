using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Graph {
	/// <summary>
	/// データの割合を簡単に表せるようにした横向きの棒グラフ
	/// </summary>
	public class PercetageBarChart : BarChart {

		/// <summary>
		/// グラフにデータを設定します。
		/// 全てのデータを設定してください。
		/// </summary>
		/// <param name="wholeValue">Whole value.</param>
		/// <param name="contents">Contents.</param>
		public void SetPercentageData (float wholeValue, List<LabelData> contents) {
			ClearBarElements ();
			float currentSum = 0;
			for (int i = 0; i < contents.Count; i++) {
				Vector2 xRangeRate = new Vector2 (currentSum, currentSum + contents [i].GetValue ()) / wholeValue;	//始点～終点
				currentSum += contents [i].GetValue ();
				float yValueRate = 1f;	//固定で1
				LabelData.Label label = contents [i].GetLabel ();
				AddBarElement (xRangeRate, yValueRate, label);
			}
		}
	}
}
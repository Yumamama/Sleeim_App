using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace Graph {
	public class AnalyzeBarChart : BarChart {

		public Color DataColor;				//何色でデータを表示するか
		public Text Output_Detail;			//データの詳細の出力先
		public bool isReverse;				//バーグラフの原点を右から開始にするか

		/// <summary>
		/// データを割合いによって設定します
		/// </summary>
		/// <param name="rate">割合(0~1f)</param>
		public void SetDataPercentage (float rate) {
			List<Vector2> xValueRangeList = new List<Vector2> () {new Vector2 (isReverse ? 1f - rate : 0, isReverse ? 1f : rate)};
			List<float> yValueRateList = new List<float> () {1f};
			List<LabelData.Label> labelList = new List<LabelData.Label> () {new LabelData.Label ("", DataColor)};
			SetData (xValueRangeList, yValueRateList, labelList);
		}

		/// <summary>
		/// データの詳細をテキストで設定します
		/// </summary>
		public void SetDataDetail (string detail) {
			this.Output_Detail.text = detail;
		}
	}
}

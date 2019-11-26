using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// グラフのシリーズ(折れ線グラフなら一つの折れ線となるもの)に共通の操作でデータを設定できるようにするインターフェース
/// </summary>
public interface IGraphSeries {

	/// <summary>
	/// グラフにデータを設定する
	/// </summary>
	/// <param name="values">値のリスト</param>
	/// <param name="xRange">X軸の値のとりうる範囲(Min,Max)</param>
	/// <param name="yRange">Y軸の値のとりうる範囲(Min,Max)</param>
	void SetData (List<Vector2> values, Vector2 xAxisRange, Vector2 yAxisRange);
}

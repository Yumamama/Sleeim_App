using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graph;
using System.Linq;
using System.IO;

/// <summary>
/// CSVファイルから睡眠データを取得するためのクラス
/// </summary>
public class CSVSleepDataReader {

	/// <summary>
	/// 睡眠データのリストを取得します
	/// </summary>
	public static List<SleepData> GetSleepDatas (string filepath) {
		using (var reader = new CSVReader <SleepData> (filepath, 3)) {
			return reader.ToList ();
		}
	}

	/// <summary>
	///	CSVのヘッダーに記述された睡眠データを取得します
	/// </summary>
	public static SleepHeaderData GetSleepHeaderData (string filepath) {
		string[] sleepRecordStartTimeLine = ReadCSVHeader(filepath, skipLine: 2);
		return new SleepHeaderData(sleepRecordStartTimeLine);
	}

	//CSVファイルのデータを一行取得する
	static string[] ReadCSVHeader (string filepath, int skipLine) {
        var stream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite); //他プロセスがファイルアクセス中でも読込を行う
        StreamReader reader = new StreamReader(stream);
		for (int i = 0; i < skipLine; i++)
			reader.ReadLine ();		//読み飛ばす
        return reader.ReadLine ().Split (',');
	}
}

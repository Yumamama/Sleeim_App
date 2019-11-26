using UnityEngine;
using System.Text;

public class DummyCaptureData : AbstractData {
	public int id = 0;
	public string dummyText = "";
	public bool dummyBool = false;

	public override void DebugPrint() {
		Debug.Log("DummyCaptureData id=" + id + ", dummyText=" + dummyText + ", dummyBool=" + (dummyBool ? "true" : "false"));
	}
}

public class DummyCaptureTable : AbstractDbTable<DummyCaptureData> {
	private static readonly string COL_ID = "id";
	private static readonly string COL_DUMMYTEXT = "dummyText";
	private static readonly string COL_DUMMYBOOL = "dummyBool";

	public DummyCaptureTable(ref SqliteDatabase db) : base(ref db) {
	}

	protected override string TableName {
		get {
			return "DummyCapture";
		}
	}

	public override void MargeData(ref SqliteDatabase oldDb) {
		DummyCaptureTable oldTable = new DummyCaptureTable(ref oldDb);
		foreach (DummyCaptureData oldData in oldTable.SelectAll()) {
			Update(oldData);
		}
	}

	public override void Update(DummyCaptureData data) {
		if (data.id <= DbDefine.DB_INVALID_PRIMARY_ID) {
			return;
		}

		StringBuilder query = new StringBuilder();
		DummyCaptureData selectData = SelectFromPrimaryKey(data.id);
		if (selectData == null) {
			query.Append("INSERT INTO ");
			query.Append(TableName);
			query.Append(" VALUES(");
			query.Append(data.id);
			query.Append(",");
			query.Append("'");
			query.Append(data.dummyText);
			query.Append("'");
			query.Append(",");
			query.Append(data.dummyBool ? DbDefine.DB_VALUE_TRUE : DbDefine.DB_VALUE_FALSE);
			query.Append(");");
		} else {
			query.Append("UPDATE ");
			query.Append(TableName);
			query.Append(" SET ");
			query.Append(COL_DUMMYTEXT);
			query.Append("=");
			query.Append("'");
			query.Append(data.dummyText);
			query.Append("'");
			query.Append(",");
			query.Append(COL_DUMMYBOOL);
			query.Append("=");
			query.Append(data.dummyBool ? DbDefine.DB_VALUE_TRUE : DbDefine.DB_VALUE_FALSE);
			query.Append(" WHERE ");
			query.Append(COL_ID);
			query.Append("=");
			query.Append(data.id);
			query.Append(";");
		}
		mDb.ExecuteNonQuery(query.ToString());
	}

	protected override DummyCaptureData PutData(DataRow row) {
		DummyCaptureData data = new DummyCaptureData();
		data.id = GetIntValue(row, "id");
		data.dummyText = GetStringValue(row, "dummyText");
		data.dummyBool = GetBoolValue(row, "dummyBool");
		return data;
	}
}

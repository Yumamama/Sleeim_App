using UnityEngine;
using System.Text;

public class DummyMasterData : AbstractData {
	public int id = 0;
	public string dummyText = "";
	public bool dummyBool = false;

	public override void DebugPrint() {
		Debug.Log("DummyMasterData id=" + id + ", dummyText=" + dummyText + ", dummyBool=" + (dummyBool ? "true" : "false"));
	}
}

public class DummyMasterTable : AbstractDbTable<DummyMasterData> {
	private static readonly string COL_ID = "id";
	private static readonly string COL_DUMMYTEXT = "dummyText";
	private static readonly string COL_DUMMYBOOL = "dummyBool";

	public DummyMasterTable(ref SqliteDatabase db) : base(ref db) {
	}

	protected override string TableName {
		get {
			return "DummyMaster";
		}
	}

	public override void MargeData(ref SqliteDatabase oldDb) {
	}

	public override void Update(DummyMasterData data) {
		if (data.id <= DbDefine.DB_INVALID_PRIMARY_ID) {
			return;
		}

		StringBuilder query = new StringBuilder();
		DummyMasterData selectData = SelectFromPrimaryKey(data.id);
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

	protected override DummyMasterData PutData(DataRow row) {
		DummyMasterData data = new DummyMasterData();
		data.id = GetIntValue(row, "id");
		data.dummyText = GetStringValue(row, "dummyText");
		data.dummyBool = GetBoolValue(row, "dummyBool");
		return data;
	}
}

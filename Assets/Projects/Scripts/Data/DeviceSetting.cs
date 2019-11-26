using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// デバイス設定クラス
/// </summary>
public class DeviceSetting {

    /// <summary>
    /// BLEコマンドコード
    /// </summary>
    public static readonly byte CommandCode = 0xC6;

    /// <summary>
    /// 動作モード
    /// </summary>
    public ActionMode ActionMode { get; set; } = ActionMode.NormalMode;

    /// <summary>
    /// いびき感度
    /// </summary>
    public SnoreSensitivity SnoreSensitivity { get; set; }
        = SnoreSensitivity.Mid;

    /// <summary>
    /// 抑制強度
    /// </summary>
    public SuppressionStrength SuppressionStrength { get; set; }
        = SuppressionStrength.Mid;

    /// <summary>
    /// 抑制動作最大継続時間
    /// </summary>
    public SuppressionOperationMaxTime SuppressionOperationMaxTime { get; set; }
        = SuppressionOperationMaxTime.TenMin;

    /// <summary>
    /// デバイス設定コマンド
    /// </summary>
    public byte[] Command {
        get {
            return new byte[] {
                CommandCode,
                (byte)ActionMode,
                (byte)SnoreSensitivity,
                (byte)SuppressionStrength,
                (byte)SuppressionOperationMaxTime
            };
        }
    }
}

/// <summary>
/// 動作モード定数
/// </summary>
public enum ActionMode : byte {
    NormalMode = 0,
    MonitoringMode
}

/// <summary>
/// いびき感度
/// </summary>
public enum SnoreSensitivity : byte {
    Low = 0,
    Mid,
    High
}

/// <summary>
/// 抑制強度
/// </summary>
public enum SuppressionStrength : byte {
    Low = 0,
    Mid,
    High
}

/// <summary>
/// 抑制動作最大継続時間
/// </summary>
public enum SuppressionOperationMaxTime : byte {
    FiveMin = 0,
    TenMin,
    NoSettings
}

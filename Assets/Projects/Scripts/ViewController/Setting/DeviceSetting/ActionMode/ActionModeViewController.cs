using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 動作モード設定画面
/// </summary>
public class ActionModeViewController : ViewControllerBase {

    /// <summary>
    /// 通常モードトグル
    /// </summary>
    public Toggle NormalModeToggle;

    /// <summary>
    /// モニタリングモードトグル
    /// </summary>
    public Toggle MonitoringModeToggle;

    /// <summary>
    /// シーンタグ
    /// </summary>
    /// <value>動作モードタグ</value>
    public override SceneTransitionManager.LoadScene SceneTag {
        get {
            return SceneTransitionManager.LoadScene.ActionMode;
        }
    }

    /// <summary>
    /// シーン開始イベントハンドラ
    /// </summary>
    protected override void Start() {
        base.Start();
        LoadActionModeSetting();
    }

    /// <summary>
    /// 動作モード設定を読み込む
    /// </summary>
    private void LoadActionModeSetting() {
        switch (DeviceSettingViewController.TempDeviceSetting.ActionMode) {
            case ActionMode.NormalMode:
                NormalModeToggle.isOn = true;
                break;
            case ActionMode.MonitoringMode:
                MonitoringModeToggle.isOn = true;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 戻るボタン押下イベントハンドラ
    /// </summary>
    public void OnReturnButtonTap() {
        SceneTransitionManager.LoadLevel(SceneTransitionManager.LoadScene.DeviceSetting);
    }

    /// <summary>
    /// 通常モード値変化イベントハンドラ
    /// </summary>
    /// <param name="isOn"></param>
    public void OnNormalModeToggleValueChanged(bool isOn) {
        if (isOn) {
            DeviceSettingViewController.TempDeviceSetting.ActionMode
                = ActionMode.NormalMode;
        }
    }

    /// <summary>
    /// モニタリングモード値変化イベントハンドラ
    /// </summary>
    /// <param name="isOn"></param>
    public void OnMonitoringModeToggleValueChanged(bool isOn) {
        if (isOn) {
            DeviceSettingViewController.TempDeviceSetting.ActionMode
                = ActionMode.MonitoringMode;
        }
    }
}

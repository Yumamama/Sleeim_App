//
//  AlarmManager.swift
//  KaiminApp
//
//  Created by Hashimoto on 2018/09/18.
//  Copyright © 2018年 sleeim. All rights reserved.
//

import UIKit
import UserNotifications
import AVFoundation
import AudioToolbox

var isVibrating = false

class AlarmManager: NSObject {
    enum Vibration: Int {
        case off = 0
        case on = 1
    }
    enum Fade: Int {
        case off = 0
        case on = 1
    }
    enum CallTime: Int {
        case off = 0
    }
    enum Alarm: Int {
        // いびき
        case snore = 0
        // 低呼吸
        case breath = 1
    }
    // 通知のID
    let NOTIFICATION_CATEGORY_ID = "ALARM_NOTIFICATION_CATEGORY_ID"
    let NOTIFICATION_ID = "ALARM_NOTIFICATION_ID"
    let NOTIFICATION_ATTACHMENT_ID = "ALARM_NOTIFICATION_ATTACHMENT_ID"
    // 鳴動時間一覧（0の場合はループ）
    private let CALL_TIMES: [Int] = [0, 5, 10, 15, 30]
    // 音楽ファイル一覧
    private let MUSIC_FILE_NAMES = ["alarm01.mp3", "alarm02.mp3", "alarm03.mp3", "alarm04.mp3", "alarm05.mp3", "alarm06.mp3"]
    // バイブレーション（デフォルト(1)ON）：
    private let USERDEFAULTS_KEY_VIBRATION = "SETTING_VIBRATION_ISENABLE";
    // 音楽（デフォルトindex=0）
    private let USERDEFAULTS_KEY_MUSIC = "KAIMIN_SETTING_SELECT_ALERM";
    // フェードイン（デフォルト(1)ON)：
    private let USERDEFAULTS_KEY_FADE = "SETTING_FEEDIN_ISENABLE";
    // 鳴動時間(デフォルトindex=0)：
    private let USERDEFAULTS_KEY_CALLTIME = "KAIMIN_SETTING_ALERM_CALLTIME";
    // 音楽ファイルのディレクトリ
    private let MUSIC_DIRECTORY = "Music/Template"
    
    private var audioPlayer: AVAudioPlayer?
    
    static let shared = AlarmManager()
    private override init() {
        super.init()
        let center = UNUserNotificationCenter.current()
        center.delegate = self
        // UNNotificationCategory を作成
        let category = UNNotificationCategory(identifier: NOTIFICATION_CATEGORY_ID,
                                              actions: [],
                                              intentIdentifiers: [],
                                              options: [.customDismissAction])
        
        // UNUserNotificationCenter に追加
        UNUserNotificationCenter.current().setNotificationCategories([category])
    }
    
    func requestAuthorization() {
        let center = UNUserNotificationCenter.current()
        center.requestAuthorization(options: [.badge, .sound, .alert],
                                    completionHandler: { (granted, error) in
                                        if error != nil {
                                            return
                                        }
                                        
                                        if granted {
                                            
                                        } else {
                                            
                                        }
        })
    }
    
    /**
     * 設定画面を開く（ローカル通知用）
     */
    func openLocalNotificationSetting() {
        // アプリ固有の設定画面を開く（細かい設定画面の指定はreject対象）
        UIApplication.shared.open(URL(string: UIApplicationOpenSettingsURLString)!,
                                  options: [:],
                                  completionHandler: nil)
    }
    
    /**
     * ローカル通知の設定を取得する
     */
    func checkLocalNotificationSetting() {
        UNUserNotificationCenter.current().getNotificationSettings { (settings) in
            BluetoothPlugin.shared.callBackNotificationStatus(settings.authorizationStatus.rawValue)
        }
    }
    
    /// 通知表示（アラーム再生）
    ///
    /// - Parameter type: アラームのタイプ
    func showNotification(type: Alarm) {
        let content = UNMutableNotificationContent()
        if type == .snore {
            content.title = "いびきアラーム"
            content.body = "いびきを検知しました。"
        } else {
            content.title = "低呼吸アラーム"
            content.body = "低呼吸を検知しました。"
        }
        
        // 通知のアラームを設定する場合
//        let url = saveAlarmIcon()
//        do {
//            content.attachments = [try UNNotificationAttachment(identifier: NOTIFICATION_ATTACHMENT_ID,
//                                                                url: url,
//                                                                options: nil)]
//        } catch {
//            print("notification image error")
//        }
        
        content.sound = UNNotificationSound.default()
        content.categoryIdentifier = NOTIFICATION_CATEGORY_ID
        
        let trigger = UNTimeIntervalNotificationTrigger(timeInterval: 0.1, repeats: false)
        let request = UNNotificationRequest(identifier: NOTIFICATION_ID,
                                            content: content,
                                            trigger: trigger)
        // 通知の登録
        UNUserNotificationCenter.current().add(request, withCompletionHandler: nil)
        
        if isAlarmPlaying() {
            return
        }
        startAlarm()
    }
    
    /// アラーム停止
    func stopAlarm() {
        isVibrating = false
        audioPlayer?.stop()
        audioPlayer = nil
        UNUserNotificationCenter.current().removeDeliveredNotifications(withIdentifiers: [NOTIFICATION_ID])
    }
    
    /// アラーム再生中か確認
    ///
    /// - Returns: true:再生中
    func isAlarmPlaying() -> Bool {
        if let audioPlayer = audioPlayer {
            if audioPlayer.isPlaying {
                return true
            }
        }
        return isVibrating
    }
    
    /// アラーム再生
    private func startAlarm() {
        setupPlayer()
        audioPlayer?.play()
        let callTime = getCallTime()
        if callTime != CallTime.off.rawValue {
            DispatchQueue.main.asyncAfter(deadline: .now() + Double(callTime)) {
                self.stopAlarm()
            }
        }
        if isVibration() {
            startVibration()
        }
    }

    // 通知のアイコン設定する場合に呼ぶ
    private func saveAlarmIcon() -> URL {
        let url = URL(fileURLWithPath: "\(NSHomeDirectory())/Documents/icon_alarm.png")
        guard let data =  NSDataAsset(name: "icon_alarm") else {
            return url
        }

        do {
            try data.data.write(to: url)
        } catch {
            
        }
        return url
    }
    
    /// バイブ設定
    private func startVibration() {
        // どのバイブレーションを鳴らすか
        let systemSoundID = SystemSoundID(kSystemSoundID_Vibrate)
        // 繰り返し用のコールバックをセット
        AudioServicesAddSystemSoundCompletion(systemSoundID, nil, nil, { (systemSoundID, _) -> Void in
            if isVibrating {
                // 繰り返し再生
                AudioServicesPlaySystemSound(systemSoundID)
            } else {
                // コールバックを解除
                AudioServicesRemoveSystemSoundCompletion(systemSoundID)
            }
        }, nil)
        // 初回のバイブレーションを鳴らす
        isVibrating = true
        AudioServicesPlaySystemSound(systemSoundID)
    }
    
    /// プレイヤー設定
    private func setupPlayer() {
        stopAlarm()

        // 再生する audio ファイルのパスを取得
        let audioUrl = getMusicFileURL()
        // auido を再生するプレイヤーを作成する
        var audioError:NSError?
        do {
            audioPlayer = try AVAudioPlayer(contentsOf: audioUrl)
        } catch let error as NSError {
            audioError = error
            audioPlayer = nil
        }
        do {
            let audioSession = AVAudioSession.sharedInstance()
            try audioSession.setCategory(AVAudioSessionCategoryPlayback)
            try audioSession.setActive(true)
        } catch {
            
        }
        // エラーが起きたとき
        if let _ = audioError {
            
        }
        
        audioPlayer?.delegate = self
        audioPlayer?.prepareToPlay()
        if isFade() {
            audioPlayer?.volume = 0.1
            audioPlayer?.setVolume(1.0, fadeDuration: 5)// 鳴動時間の設定に関係なく５秒かけてフェード
        } else {
            audioPlayer?.volume = 1.0
        }
        audioPlayer?.numberOfLoops = -1// 常にループ再生
    }
    
    // MARK: - Get Setting
    /// バイブONフラグ取得
    ///
    /// - Returns: バイブONフラグ
    private func isVibration() -> Bool {
        if let vibration = UserDefaults.standard.object(forKey: USERDEFAULTS_KEY_VIBRATION) as? Int {
            return vibration == Vibration.on.rawValue
        }
        
        // デフォルト設定
        return true
    }
    
    /// フェードONフラグ取得
    ///
    /// - Returns: フェードONフラグ
    private func isFade() -> Bool {
        if let fade = UserDefaults.standard.object(forKey: USERDEFAULTS_KEY_FADE) as? Int {
            return fade == Fade.on.rawValue
        }
        
        // デフォルト設定
        return true
    }
    
    /// 鳴動時間取得
    ///
    /// - Returns: 鳴動時間
    private func getCallTime() -> Int {
        if let index = UserDefaults.standard.object(forKey: USERDEFAULTS_KEY_CALLTIME) as? Int {
            if index > 0 && index < CALL_TIMES.count {
                return CALL_TIMES[index]
            }
        }
        
        // デフォルト設定
        return CALL_TIMES[0]
    }
    
    /// 音楽ファイル名取得
    ///
    /// - Returns: 音楽ファイル名
    private func getMusicFileName() -> String {
        if let index = UserDefaults.standard.object(forKey: USERDEFAULTS_KEY_MUSIC) as? Int {
            if index > 0 && index < MUSIC_FILE_NAMES.count {
                return MUSIC_FILE_NAMES[index]
            }
        }
        
        // デフォルト設定
        return MUSIC_FILE_NAMES[0]
    }
    
    /// 音楽ファイルURL取得
    ///
    /// - Returns: 音楽ファイルURL
    private func getMusicFileURL() -> URL {
        let documentPath = NSSearchPathForDirectoriesInDomains(.documentDirectory, .userDomainMask, true)[0]
        let fileName = getMusicFileName()
        return URL(fileURLWithPath: "\(documentPath)/\(MUSIC_DIRECTORY)/\(fileName)")
    }
}

extension AlarmManager: UNUserNotificationCenterDelegate {
    func userNotificationCenter(_ center: UNUserNotificationCenter,
                                willPresent notification: UNNotification,
                                withCompletionHandler completionHandler: @escaping (UNNotificationPresentationOptions) -> Void) {
        //...
        completionHandler([.alert])
    }
    
    func userNotificationCenter(_ center: UNUserNotificationCenter,
                                didReceive response: UNNotificationResponse,
                                withCompletionHandler completionHandler: @escaping () -> Void) {
        if response.actionIdentifier == UNNotificationDismissActionIdentifier {
            // 通知削除時
            AlarmManager.shared.stopAlarm()
        }
        completionHandler()
    }
}

extension AlarmManager: AVAudioPlayerDelegate {
}

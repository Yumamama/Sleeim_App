using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Graph {
    public class Time {
        DateTime value;

        public Time (DateTime value) {
            this.value = value;
        }

        public DateTime Value {
            get {
                return value;
            }
        }

        /// <summary>
        /// 指定した時刻が、開始時刻と終了時刻の間のどの位置に相当するかを0~1fで返します
        /// </summary>
        /// <returns>The position rate.</returns>
        /// <param name="val">Value.</param>
        /// <param name="start">Start.</param>
        /// <param name="end">End.</param>
        public static float GetPositionRate (DateTime val, DateTime start, DateTime end) {
            return (float)GetDateDifferencePerSecond (start, val) / (float)GetDateDifferencePerSecond (start, end);
        }

        /// <summary>
        /// 開始時刻と終了時刻の時間差を秒単位にして返します
        /// </summary>
        public static int GetDateDifferencePerSecond (DateTime start, DateTime end) {
            var timeDiff = end.Subtract (start);
            return (int)timeDiff.TotalSeconds;
        }

        /// <summary>
        /// 引数で与えられた秒数をH時間M分、S秒に変換する
        /// </summary>
        /// <param name="sec">秒数</param>
        /// <returns>[時, 分, 秒]</returns>
        public static List<int> SecToHMSList(int sec) {
            int hour = sec / 3600;
            int min = sec / 60 % 60;
            sec %= 60;
            return new List<int>() { hour, min, sec };
        }

        /// <summary>
        /// 時間を1時間単位となるように切り上げして返します
        ///	例：5/25 23:10 → 5/26 00:00
        /// </summary>
        /// <returns>The up time.</returns>
        /// <param name="targetTime">Target time.</param>
        public static DateTime RoundUpTime (DateTime targetTime) {
            int sec = targetTime.Second;
            int min = targetTime.Minute;
            int hour = targetTime.Hour ;
            int day = targetTime.Day;
            int month = targetTime.Month;
            int year = targetTime.Year;
            //繰り上げ処理
            if (min > 0) {
                hour++;
                min = 0;
            }
            if (hour > 24) {
                day++;
                hour = 1;
            }
            if (day > DateTime.DaysInMonth (year, month)) {
                month++;
                day = 1;
            }
            if (month > 12) {
                year++;
                month = 1;
            }
            return new DateTime (year, month, day, hour, min, sec);
        }
        /// <summary>
        /// 時間を1時間単位となるように切り下げして返します
        /// 例：23:40　→ 23:00
        /// </summary>
        /// <returns>The down time.</returns>
        /// <param name="targetTime">Target time.</param>
        public static DateTime RoundDownTime (DateTime targetTime) {
            return new DateTime (targetTime.Year, targetTime.Month, targetTime.Day, targetTime.Hour, 0, 0);
        }

        /// <summary>
        /// 時分秒を表す文字列を生成する
        /// </summary>
        /// <param name="sec">秒数</param>
        /// <returns>"HH時間MM分SS秒"</returns>
        public static string CreateHMSString(int sec) {
            const int unitSize = 24;    // 単位のフォントサイズ
            TimeSpan ts = new TimeSpan(hours:0, minutes:0, seconds: sec);
            int hourWithDay = 24 * ts.Days + ts.Hours;      // 24時間超えた場合の時間を考慮
            return $"{hourWithDay}<size={unitSize}>時間</size>{ts.Minutes}<size={unitSize}>分</size>{ts.Seconds}<size={unitSize}>秒</size>";
        }
    }
}

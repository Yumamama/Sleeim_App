using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 睡眠データ要素クラス
/// </summary>
namespace Kaimin.Data
{
    sealed class GsDataModel
    {
        /** 日時 */
        public DateTime mDate;
        /** 呼吸 */
        public int mRespiration;
        /** 睡眠ステータス */
        public int mSleep;
        /** いびき有無[0,1] */
        public Boolean mSnore;
        /** いびきの大きさ[0-65535] */
        public int mSnoreValue;
        /** 脈拍[0-255] */
        public int mPulse;
        /** SpO2[%] */
        public int mSpO2;
        /** 首の角度[度] */
        public int mNeckAngleDeg;
    }
}

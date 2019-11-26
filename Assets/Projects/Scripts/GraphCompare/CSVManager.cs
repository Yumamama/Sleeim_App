using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class CSVManager
{
    const int MAX_MUKOKYU_CONTINUOUS_TIME = 300; //無呼吸状態が５分(300s)以上続いている箇所は不明状態にする

    /**
     * Get list of all csv files in app order by name (date)
     * Return array(index -> filePath)
     */
    public static string[] getCsvFileList()
    {
        string[] fileList = Kaimin.Common.Utility.GetAllFiles(Kaimin.Common.Utility.GsDataPath(), "*.csv");

        return fileList;
    }

    /**
     * Get list of csv files by page (used to display chartWeek)
     * Param page: Calculated from 1
     * Return array(index -> filePath)
     */
    public static string[] getCsvFileListByPage(string[] fileList, int page)
    {
        int fileNum = fileList.Length;
        List<string> pageFileList = new List<string>();

        if (fileNum > 0)
        {
            if (page == 1)
            {
                for (int i = 0; i < Mathf.Min(7, fileNum); i++)
                {
                    pageFileList.Add(fileList[i]);
                }
            }
            else if (page > 1)
            {
                int end = (fileNum % 7 == 0) ? page * 7 : ((page - 1) * 7 + fileNum / 7);
                for (int i = end - 7; i < end; i++)
                {
                    pageFileList.Add(fileList[i]);
                }
            }
        }

        return pageFileList.ToArray();
    }

    /**
     * Get list of csv files that unread and unsaved to everage chart
     * Return array(index -> filePath)
     */
    public static string[] getUnreadCsvFileList(string[] fileList, string savedLastFileName)
    {
        if (string.IsNullOrEmpty(savedLastFileName) || fileList.Length == 0)
        {
            return fileList;
        }
        else
        {
            List<string> unreadFileList = new List<string>();

            var lastDateTime = System.DateTime.Parse(savedLastFileName);
            foreach (var filePath in fileList)
            {
                var dateTime = Kaimin.Common.Utility.TransFilePathToDate(filePath);
                if (dateTime > lastDateTime)
                {
                    unreadFileList.Add(filePath);
                }
            }

            return unreadFileList.ToArray();
        }
    }

    public static List<SleepData> readSleepDataFromCsvFile(string filePath)
    {
        return CSVSleepDataReader.GetSleepDatas(filePath);
    }

    public static ChartInfo convertSleepHeaderToChartInfo(ChartInfo chartInfo, string filePath)
    {
        var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        StreamReader reader = new StreamReader(stream);

        string[] profileInfo = reader.ReadLine().Split(','); //CsvFileの1行目 (Row 0)
        reader.ReadLine();　//Skip Row 1
        string[] sleepRecordStartTimeLine = reader.ReadLine().Split(','); //CsvFileの3行目 (Row 2)

        string sleepTime = "";
        if (profileInfo.Length >= 7)
        {
            string sleepStartTime = profileInfo[5];
            string sleepEndTime = profileInfo[6];

            sleepTime = getDurationTime(sleepStartTime, sleepEndTime);
        }

        System.DateTime fileDateTime = Kaimin.Common.Utility.TransFilePathToDate(filePath);
        chartInfo.fileName = fileDateTime.ToString();
        chartInfo.sleepTime = sleepTime;

        string tmpTime = fileDateTime.ToString("HH:mm:ss");
        if (string.Compare(tmpTime, "00:00:00") >= 0 && string.Compare(tmpTime, "09:00:00") <= 0)
        {
            //データ開始時刻がAM00:00～09:00までのデータに前日の日付として表示
            chartInfo.date = fileDateTime.AddDays(-1).ToString("M/d");
        } else
        {
            chartInfo.date = fileDateTime.ToString("M/d");
        }
        
        if (sleepRecordStartTimeLine.Length > 9) //New format
        {
            chartInfo.sleepMode = int.Parse(sleepRecordStartTimeLine[8]);
            chartInfo.vibrationStrength = int.Parse(sleepRecordStartTimeLine[9]);
        } else
        { 
            //Default
            chartInfo.sleepMode = (int) SleepMode.Suppress;
            chartInfo.vibrationStrength = (int)VibrationStrength.Medium;
        }
        

        return chartInfo;
    }

    public static ChartInfo convertSleepDataToChartInfo(List<SleepData> sleepData)
    {
        float numKaiMin = 0;
        float numIbiki = 0;
        float numMukokyu = 0;
        float numFumei = 0;

        //無呼吸状態が５分(300s)以上続いている箇所は不明状態にする
        float numFumeiAble = 0;
        float mukokyuContinuousTime = 0;
        float mukokyuStartTime = 0;

        foreach (var item in sleepData)
        {
            int[] states = { item.BreathState1, item.BreathState2, item.BreathState3 };
            foreach (var state in states)
            {
                if (state == (int)SleepData.BreathState.Apnea)
                {
                    numMukokyu++;
                    numFumeiAble++;

                    System.DateTime tmpDateTime = item.GetDateTime();
                    if (mukokyuStartTime == 0)
                    {
                        mukokyuStartTime = ((System.DateTimeOffset)tmpDateTime).ToUnixTimeSeconds();
                    } else
                    {
                        mukokyuContinuousTime = ((System.DateTimeOffset)tmpDateTime).ToUnixTimeSeconds() - mukokyuStartTime;
                    }
                } else
                {
                    if (mukokyuContinuousTime > MAX_MUKOKYU_CONTINUOUS_TIME) //無呼吸状態が５分(300s)以上続いている場合
                    {
                        numMukokyu -= numFumeiAble;
                        numFumei += numFumeiAble;
                    }
                    //Reset
                    numFumeiAble = 0;
                    mukokyuContinuousTime = 0;
                    mukokyuStartTime = 0;

                    if (state == (int)SleepData.BreathState.Normal)
                    {
                        numKaiMin++;
                    }
                    else if (state == (int)SleepData.BreathState.Snore)
                    {
                        numIbiki++;
                    }
                    else if (state == (int)SleepData.BreathState.Empty)
                    {
                        numFumei++;
                    }
                }
            }
        }

        float numTotal = numKaiMin + numIbiki + numMukokyu + numFumei;
        if (numTotal > 0)
        {
            ChartInfo chartInfo = new ChartInfo();
            chartInfo.pKaiMin = numKaiMin / numTotal;
            chartInfo.pIbiki = numIbiki / numTotal;
            chartInfo.pMukokyu = numMukokyu / numTotal;
            chartInfo.pFumei = 1 - (chartInfo.pKaiMin + chartInfo.pIbiki + chartInfo.pMukokyu);

            return chartInfo;
        }
        else
        {
            return null;
        }
    }

    /**
     * Get duration time (HH:mm)
     * Parmams startTime (HH:mm), endTime (HH:mm)
     */
    public static string getDurationTime(string startTime, string endTime)
    {
        if (startTime != endTime && startTime.Contains(":") && endTime.Contains(":"))
        {
            string[] arrS = startTime.Split(':');
            string[] arrE = endTime.Split(':');

            if(arrS.Length == 2 && arrE.Length == 2)
            {
                int sHour = int.Parse(arrS[0]);
                int sMinute = int.Parse(arrS[1]);
                int eHour = int.Parse(arrE[0]);
                int eMinute = int.Parse(arrE[1]);
                int diffDate = (eHour * 60 + eMinute) > (sHour * 60 + sMinute) ? 0 : 1;

                System.TimeSpan start = new System.TimeSpan(0, sHour, sMinute, 0);
                System.TimeSpan end = new System.TimeSpan(diffDate, eHour, eMinute, 0);
                System.TimeSpan diff = end - start;

                return string.Format("{0:00}:{1:00}", diff.Hours, diff.Minutes);
            }
        }

        return "00:00";
    }
}
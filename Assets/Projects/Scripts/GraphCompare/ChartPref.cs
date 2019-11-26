using UnityEngine;

public class ChartPref
{
    public int savedNumMonitor = 0;
    public int savedNumSuppress = 0;
    public string savedEverageMonitor = ""; //Ex: 0.25_0.25_0.25 (pKaimin_pIbiki_pMukokyu)
    public string savedEverageSuppress = "";
    public string savedLastFileName = "";

    public void saveData(bool isMonitor, ChartInfo chartEverage, int chartNum)
    {
        string key = isMonitor ? "Monitor" : "Suppress";
        PlayerPrefs.SetInt("savedNum" + key, chartNum);
        PlayerPrefs.SetString("savedEverage" + key, chartEverage.pKaiMin + "_" + chartEverage.pIbiki + "_" + chartEverage.pMukokyu);
    }

    public void saveLastFileName(string lastFileName)
    {
        PlayerPrefs.SetString("savedLastFileName", lastFileName);
    }

    public void loadData()
    {
        savedNumMonitor = PlayerPrefs.GetInt("savedNumMonitor", 0);
        savedNumSuppress = PlayerPrefs.GetInt("savedNumSuppress", 0);
        savedEverageMonitor = PlayerPrefs.GetString("savedEverageMonitor", "");
        savedEverageSuppress = PlayerPrefs.GetString("savedEverageSuppress", "");
        savedLastFileName = PlayerPrefs.GetString("savedLastFileName", "");
    }
}
using UnityEngine;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System;

public class HistoryScore
{
    public long time;       // 玩的时间
    public int score;       // 分数

    public HistoryScore(long t, int s)
    {
        time = t;
        score = s;
    }
}


public class HistoryScoresManager : Singleton<HistoryScoresManager>
{
    private readonly string HistoryScoresFilePath = Application.persistentDataPath + "/HistoryScores.txt";

    List<HistoryScore> _historyScores;

    public List<HistoryScore> HistoryScores
    {
        get
        {
            return _historyScores;
        }
    }

    public override void Init()
    {
        base.Init();
        _InitHistoryScores();
    }

    void _InitHistoryScores()
    {
        _historyScores = new List<HistoryScore>();
        try
        {
            using (FileStream fs = new FileStream(HistoryScoresFilePath, FileMode.OpenOrCreate))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        HistoryScore hs = JsonUtility.FromJson<HistoryScore>(line);
                        _historyScores.Add(hs);
                    }
                }
            }
        }
        catch (Exception e)
        {
            DebugUtil.Error(e.Message);
        }
    }
    
    public void AddHistoryScore(HistoryScore hs)
    {
        if (hs == null)
        {
            DebugUtil.Warning("hs is NULL !!!");
            return;
        }

        _historyScores.Add(hs);
        try
        {
            using (StreamWriter sw = new StreamWriter(HistoryScoresFilePath, true))
            {
                string line = JsonUtility.ToJson(hs);
                sw.WriteLine(line);
            }
        }
        catch (Exception e)
        {
            DebugUtil.Error(e.Message);
        }
    }
}

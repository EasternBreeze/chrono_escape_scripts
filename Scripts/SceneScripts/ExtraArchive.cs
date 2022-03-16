using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ChangeableDatabase.ArchiveDataBase;

public sealed class ExtraArchive : MonoBehaviour
{
    [SerializeField] private Text tTotalPlayTime;
    [SerializeField] private Text tTotalCount;
    [SerializeField] private Text tBestPlayTime;
    [SerializeField] private Text tBestCount;
    [SerializeField] private Text tBestLevel;

    private void Awake()
    {
        JArchiveData d = ArchiveData;
        {
            int sec = d.total_playtime_sec;
            tTotalPlayTime.text = $"{sec / 3600:00}:{sec % 3600 / 60:00}:{sec % 60:00}";
        }

        tTotalCount.text = $"{d.total_count:#,0}";

        {
            int sec = d.best_playtime_sec;
            tBestPlayTime.text = $"{sec % 3600 / 60:00}:{sec % 60:00}";
        }

        tBestCount.text = $"{d.best_count:#,0}";

        tBestLevel.text = $"{d.best_level:#,0}"; ;
    }
}

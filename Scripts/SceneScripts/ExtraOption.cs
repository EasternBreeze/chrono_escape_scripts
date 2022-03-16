using ChangeableDatabase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class ExtraOption : MonoBehaviour
{
    [SerializeField] private Text tVolumeBGM;
    [SerializeField] private Text tVolumeSE;
    [SerializeField] private Text tVolumeCountDown;
    [SerializeField] private Text tFieldBGMSong;
    [SerializeField] private Text tFieldBGMArtist;

    private void Awake()
    {
        Refresh();
    }

    private void Refresh()
    {
        tVolumeBGM.text = $"{SystemSettings.VolumeBGM}";
        tVolumeSE.text = $"{SystemSettings.VolumeSE}";
        tVolumeCountDown.text = $"{SystemSettings.VolumeCountDown}";
        if (SystemSettings.IndexFieldBGM == -1)
        {
            tFieldBGMSong.text = "<b>Random</b>";
            tFieldBGMArtist.text = "";
        }
        else
        {
            tFieldBGMSong.text = $"<b>{Sound.Instance.GetFieldBGMSong(SystemSettings.IndexFieldBGM)}</b>";
            tFieldBGMArtist.text = $"{Sound.Instance.GetFieldBGMArtist(SystemSettings.IndexFieldBGM)}";
        }
    }

    /** ******************************
     * OptionButtonMethods
     ** ******************************/
    public void ChangeOptionVolumeBGM(bool pressUp)
    {
        SystemSettings.VolumeBGM += pressUp ? 5 : -5;
        Sound.Instance.Play(SE.SelectSub);
        Refresh();
    }
    public void ChangeOptionVolumeSE(bool pressUp)
    {
        SystemSettings.VolumeSE += pressUp ? 5 : -5;
        Sound.Instance.Play(SE.SelectSub);
        Refresh();
    }
    public void ChangeOptionVolumeCountDown(bool pressUp)
    {
        SystemSettings.VolumeCountDown += pressUp ? 5 : -5;
        Sound.Instance.Play(SE.CountDown);
        Refresh();
    }
    public void ChangeOptionFieldBGM(bool pressUp)
    {
        SystemSettings.IndexFieldBGM += pressUp ? 1 : -1;
        Sound.Instance.Play(SE.SelectSub);
        Refresh();
    }
}

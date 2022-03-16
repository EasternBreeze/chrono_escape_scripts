using ChangeableDatabase;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UserDatabase;

public sealed class ResultAdmin : MonoBehaviour
{
    [SerializeField] private CanvasResult cResult;
    [SerializeField] private CanvasRecord cRecord;
    [SerializeField] private GameObject blocker;

    private async void Awake()
    {
        int newRecord = RecordDataBase.AddRanking(UDB.DifficultyData.name, CDB.FieldInfo.totalCount);
        ArchiveDataBase.Refresh(CDB.FieldInfo);

        cResult.LoadResultData();
        cRecord.LoadRecordData(UDB.DifficultyData.name, newRecord);
        Sound.Instance.Play(BGM.Result);
        SceneChanger.Instance.FadeOut();
        Sound.Instance.Play(SE.ShutterOpen);
        await UniTask.DelayFrame(60);
        blocker.SetActive(false);
    }

    public void BackSelectScene()
    {
        blocker.SetActive(true);
        Sound.Instance.Play(SE.ShutterClose);
        SceneChanger.Instance.SceneChange(Scene.SelectScene);
    }

    public void Retry()
    {
        Sound.Instance.Stop();
        blocker.SetActive(true);

        if (UDB.DifficultyData.difficulty_type == DifficultyType.OVERDRIVE)
        {
            Sound.Instance.Play(SE.DesideOD);
            SceneChanger.Instance.SceneChange(Scene.GameScene, FadeType.OverDrive);
        }
        else
        {
            Sound.Instance.Play(SE.Deside);
            SceneChanger.Instance.SceneChange(Scene.GameScene, FadeType.GameStart);
        }
    }
}

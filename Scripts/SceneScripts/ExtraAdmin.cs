using ChangeableDatabase;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ExtraType
{
    Option,
    Record,
    Archive
}

public sealed class ExtraAdmin : MonoBehaviour
{
    [SerializeField] private GameObject blocker;
    [SerializeField, EnumIndex(typeof(ExtraType))] private List<GameObject> gExtraTypes;
    [SerializeField, EnumIndex(typeof(ExtraType))] private List<Text> tExtraTypes;
    [SerializeField] private ExtraOption option;
    [SerializeField] private ExtraRecord record;

    private async void Awake()
    {
        blocker.SetActive(true);
        gExtraTypes.ForEach(s => s.SetActive(false));
        gExtraTypes[0].SetActive(true);
        tExtraTypes.ForEach(s => s.color = new Color(0.5f, 0.5f, 0.5f, 0.5f));
        tExtraTypes[0].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        SceneChanger.Instance.FadeOut();
        Sound.Instance.Play(SE.ShutterOpen);
        Sound.Instance.Play(BGM.Extra);
        await UniTask.DelayFrame(30);
        blocker.SetActive(false);
    }

    public void ChangeTab(int target)
    {
        gExtraTypes.ForEach(s => s.SetActive(false));
        gExtraTypes[target].SetActive(true);
        tExtraTypes.ForEach(s => s.color = new Color(0.5f, 0.5f, 0.5f, 0.5f));
        tExtraTypes[target].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        Sound.Instance.Play(SE.SelectMain);
    }

    public void Select()
    {
        SystemSettings.Save();
        Sound.Instance.Stop();
        Sound.Instance.Play(SE.ShutterClose);
        blocker.SetActive(true);
        SceneChanger.Instance.SceneChange(Scene.SelectScene);
    }

    public void Intro()
    {
        SystemSettings.Save();
        Sound.Instance.Stop();
        Sound.Instance.Play(SE.ShutterClose);
        blocker.SetActive(true);
        SceneChanger.Instance.SceneChange(Scene.IntroScene);
    }

    public void Credit()
    {
        SystemSettings.Save();
        Sound.Instance.Stop();
        Sound.Instance.Play(SE.ShutterClose);
        blocker.SetActive(true);
        SceneChanger.Instance.SceneChange(Scene.CreditScene);
    }
}
using ChangeableDatabase;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UserDatabase;

public sealed class SelectSceneAdmin : MonoBehaviour
{
    [SerializeField] private List<Canvas> panelTypeTabs;
    [SerializeField] private GameObject blocker;
    [SerializeField] private CanvasRecord cRecord;
    [SerializeField] private Image iPanelType;
    [SerializeField] private Image iDifficulty;
    [SerializeField] private GameObject startButton;

    private int orderPanelTabs;
    private bool isSelected;
    private int lastTab;

    private async void Awake()
    {
        blocker.SetActive(true);
        orderPanelTabs = 499 + panelTypeTabs.Count;
        ResetTab();
        SceneChanger.Instance.FadeOut();
        Sound.Instance.Play(SE.ShutterOpen);
        Sound.Instance.Play(BGM.Select);
        await UniTask.DelayFrame(30);
        blocker.SetActive(false);
    }

    public void ChangeTab(int tab)
    {
        if (tab >= panelTypeTabs.Count || tab < 0)
        {
            Debug.LogError($"PanelTabsIndexOver {tab} (max:{panelTypeTabs.Count - 1})");
            return;
        }
        if (tab == lastTab)
        {
            return;
        }

        for (int i = 0; i < panelTypeTabs.Count; i++)
        {
            if (i == tab)
            {
                panelTypeTabs[i].sortingOrder = orderPanelTabs + 1;
            }
            else
            {
                panelTypeTabs[i].sortingOrder = orderPanelTabs - i;
            }
        }
        Sound.Instance.Play(SE.SelectMain);
        lastTab = tab;
    }

    public void ChangeDifficulty(int difficulty)
    {
        Sound.Instance.Play(SE.SelectSub);

#if UNITY_EDITOR
        if (CDB.Difficulty == difficulty)
        {
            CDB.Difficulty = (int)UDB.DifficultyData.panel_type + 9;
            cRecord.LoadRecordData(UDB.DifficultyData.name);

            iPanelType.sprite = UDB.GetSpritePanelType(UDB.DifficultyData.panel_type);
            iDifficulty.sprite = UDB.GetSpriteDifficulty(UDB.DifficultyData.difficulty_type);
            iPanelType.enabled = true;
            iDifficulty.enabled = true;

            isSelected = true;
            return;
        }
#endif

        CDB.Difficulty = difficulty;
        cRecord.LoadRecordData(UDB.DifficultyData.name);

        iPanelType.sprite = UDB.GetSpritePanelType(UDB.DifficultyData.panel_type);
        iDifficulty.sprite = UDB.GetSpriteDifficulty(UDB.DifficultyData.difficulty_type);
        iPanelType.enabled = true;
        iDifficulty.enabled = true;

        isSelected = true;
        startButton.GetComponent<Image>().color = new Color(0.65f, 0.91f, 0.915f, 1.0f);
        startButton.GetComponentsInChildren<Image>()[1].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    public void Gamestart()
    {
        if (!isSelected)
        {
            return;
        }

        Sound.Instance.Stop();
        blocker.SetActive(true);

        if (Input.GetKey(KeyCode.E) && Input.GetKey(KeyCode.B))
        {
            CDB.Difficulty = (int)UDB.DifficultyData.panel_type + 9;
        }

        if (UDB.DifficultyData.difficulty_type == DifficultyType.OVERDRIVE)
        {
            Sound.Instance.Play(SE.DesideOD);
            if (Input.GetKey(KeyCode.E) && Input.GetKey(KeyCode.B)) // TODO:テスト版のみ演出省略
            {
                SceneChanger.Instance.SceneChange(Scene.GameScene, FadeType.OverDrive);
            }
            else
            {
                SceneChanger.Instance.SceneChange(Scene.GameScene, FadeType.GameStart);
            }
        }
        else
        {
            Sound.Instance.Play(SE.Deside);
            SceneChanger.Instance.SceneChange(Scene.GameScene, FadeType.GameStart);
        }
    }

    public void Extra()
    {
        Sound.Instance.Stop();
        Sound.Instance.Play(SE.ShutterClose);
        blocker.SetActive(true);
        SceneChanger.Instance.SceneChange(Scene.ExtraScene);
    }

    private void ResetTab()
    {
        for (int i = 0; i < panelTypeTabs.Count; i++)
        {
            if (i == 0)
            {
                panelTypeTabs[i].sortingOrder = orderPanelTabs + 1;
            }
            else
            {
                panelTypeTabs[i].sortingOrder = orderPanelTabs - i;
            }
        }
    }
}

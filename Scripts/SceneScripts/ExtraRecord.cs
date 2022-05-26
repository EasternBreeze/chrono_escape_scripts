using ChangeableDatabase;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UserDatabase;

public sealed class ExtraRecord : MonoBehaviour
{
    private static readonly List<string> tablePanelType = new List<string>()
    {
        "3x3", "4x4", "5x5"
    };
    private static readonly List<string> sPanelTypePattern1 = new List<string>()
    {
        "3x3", "4x4", "5x5"
    };

    private static readonly List<string> tableDifficultyType = new List<string>()
    {
        "basic", "normal", "expert", "overdrive", "blackout"
    };
    private static readonly List<string> sDifficultyPattern1 = new List<string>()
    {
        "basic", "normal", "expert"
    };
    private static readonly List<string> sDifficultyPattern2 = new List<string>()
    {
        "basic", "normal", "expert", "overdrive"
    };

    private List<Sprite> spritesPanelType = new List<Sprite>();
    private List<Sprite> spritesDifficulty = new List<Sprite>();

    [SerializeField] private Transform parent;
    [SerializeField] private GameObject prefabTextRank;
    [SerializeField] private Image iPanelType;
    [SerializeField] private List<Image> iDifficulty;

    private List<Text> tRanks = new List<Text>();

    private int panelTypeIndex;

    private void Awake()
    {
        foreach (PanelType p in Enum.GetValues(typeof(PanelType)))
        {
            spritesPanelType.Add(UDB.GetSpritePanelType(p));
        }
        foreach (DifficultyType p in Enum.GetValues(typeof(DifficultyType)))
        {
            spritesDifficulty.Add(UDB.GetSpriteDifficulty(p));
        }

        for (int i = 0; i < 25; i++)
        {
            GameObject g = Instantiate(prefabTextRank);
            g.transform.SetParent(parent);
            RectTransform r = g.GetComponent<RectTransform>();

            r.position = new Vector3()
            {
                x = -120 + 170 * (i / 5),
                y = 75 - 80 * (i % 5),
                z = 0
            };

            tRanks.Add(g.GetComponent<Text>());
        }

        Refresh();
    }

    private void Refresh()
    {
        DrawRecordData("3x3");
    }

    /** ******************************
     * RecordButtonMethods
     ** ******************************/
    public void ChangeRecordPanelType(bool isUp)
    {
        panelTypeIndex += isUp ? 1 : -1;
        panelTypeIndex = panelTypeIndex < tablePanelType.Count ? panelTypeIndex : 0;
        panelTypeIndex = panelTypeIndex < 0 ? tablePanelType.Count - 1 : panelTypeIndex;
        Sound.Instance.Play(SE.SelectSub);
        DrawRecordData(tablePanelType[panelTypeIndex]);
    }

    private void DrawRecordData(string panelType)
    {
        if (sPanelTypePattern1.Contains(panelType))
        {
            iPanelType.sprite = spritesPanelType[panelTypeIndex];

            List<int> list = new List<int>();
            foreach (string s in tableDifficultyType)
            {
                list.AddRange(RecordDataBase.GetRecords($"{panelType}{s}"));
            }

            iDifficulty.ForEach(s => s.gameObject.SetActive(false));
            tRanks.ForEach(s => s.gameObject.SetActive(false));
            for (int i = 0; i < Enum.GetValues(typeof(DifficultyType)).Length; i++)
            {
                iDifficulty[i].sprite = spritesDifficulty[i];
                if (i <= 2 || list[i * 5] != -1)
                {
                    iDifficulty[i].gameObject.SetActive(true);
                }

                for (int j = 0; j < 5; j++)
                {
                    tRanks[i * 5 + j].text = list[i * 5 + j] != -1 ? $"{list[i * 5 + j]}" : "---";
                    tRanks[i * 5 + j].gameObject.SetActive(true);
                }
            }
        }
    }
}

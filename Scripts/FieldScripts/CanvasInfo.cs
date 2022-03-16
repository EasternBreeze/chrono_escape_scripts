using ChangeableDatabase;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UserDatabase;

public sealed class CanvasInfo : MonoBehaviour
{
    private List<Sprite> sGauge;

    [SerializeField] private Text tSpeedLevel;
    [SerializeField] private Text tMaxLevel;
    [SerializeField] private Text tLimitOver;
    [SerializeField] private Text tCountDownSeconds;
    [SerializeField] private Text tTotalCount;
    [SerializeField] private Text tTotalTime;
    [SerializeField] private Text tCursorLife;
    [SerializeField] private Text tCursorInvincibleSeconds;

    [SerializeField] private RectTransform mNextSpeedupGauge;
    [SerializeField] private RectTransform rCursorInv;

    [SerializeField] private Image iPanelType;
    [SerializeField] private Image iDifficulty;
    [SerializeField] private Image iGauge;
    [SerializeField] private Image iCursorInv;

    private int spriteCount;

    private void Awake()
    {
        if (sGauge == null)
        {
            sGauge = Resources.LoadAll<Sprite>("Graphics/gauge_Nextspeedup").ToList();
        }
    }

    public void Init(FieldAdmin.FieldInfo m)
    {
        iPanelType.sprite = UDB.GetSpritePanelType(UDB.DifficultyData.panel_type);
        iDifficulty.sprite = UDB.GetSpriteDifficulty(UDB.DifficultyData.difficulty_type);

        tMaxLevel.text = $"/<size=45>{m.maxSpeedLevel}</size>";
        tLimitOver.text = "";
        InfoUpdate(m);
    }

    public void InfoUpdate(FieldAdmin.FieldInfo m)
    {
        tSpeedLevel.text = $"{m.speedLevel}";

        {
            if (m.isFullLevelup)
            {
                tLimitOver.text = "MAX";
            }
            else if (m.isMaxSpeedLevel)
            {
                tLimitOver.text = $"+{m.limitOver}";
            }
        }

        {
            string sec = $"{(int)m.countDownSeconds}";
            string ms = $"{m.countDownSeconds % 1.0f:0.000}".Substring(2, 3);
            tCountDownSeconds.text = $"<b>{sec}.<size=75>{ms}</size></b>";
        }

        tTotalCount.text = $"{m.totalCount}";

        {
            string minute = $"{(int)(m.totalSecond / 60.0f):#0}";
            string sec = $"{(int)(m.totalSecond % 60.0f):00}";
            string ms = $"{(m.totalSecond % 1.0f):0.000}".Substring(2, 3);
            tTotalTime.text = $"<b>{minute}:{sec}.<size=50>{ms}</size></b>";
        }

        {
            string s = "";
            for (int i = 0; i < m.cursorLife; i++)
            {
                s += i > 4 && i % 5 == 0 ? "\nÅö" : "Åö";
            }
            tCursorLife.text = $"{s}";
        }

        {
            if (m.cursorInvincibleSeconds <= 0.0f)
            {
                tCursorInvincibleSeconds.text = "";
                iCursorInv.fillAmount = 0.0f;
            }
            else
            {
                tCursorInvincibleSeconds.text = $"{m.cursorInvincibleSeconds:0.0}";
                iCursorInv.fillAmount = m.cursorInvincibleSeconds / 5.0f;
                iCursorInv.color = new Color(1.0f, 1.0f, 1.0f, Random.Range(0.5f, 1.0f));
            }
            rCursorInv.position = m.cursorPosition;
        }

        {
            if (m.isFullLevelup)
            {
                mNextSpeedupGauge.sizeDelta = new Vector2(630.0f, 50.0f);
                spriteCount++;
                spriteCount %= 40;
                iGauge.sprite = sGauge[spriteCount < 24 ? spriteCount / 2 : 0];
                iGauge.color = new Color(1.0f, 1.0f, 0.0f, m.cursorPosition.y > 275.0f ? 0.5f : 1.0f);
            }
            else if (m.isMaxSpeedLevel)
            {
                mNextSpeedupGauge.sizeDelta = new Vector2(Mathf.Clamp(630.0f * (m.nextSpeedupWait / 30.0f), 0.0f, 630.0f), 50.0f);
                spriteCount++;
                spriteCount %= 120;
                iGauge.sprite = sGauge[spriteCount < 24 ? 11 - spriteCount / 2 : 0];
                iGauge.color = new Color(0.0f, 1.0f, 1.0f, m.cursorPosition.y > 275.0f ? 0.5f : 1.0f);
            }
            else
            {
                mNextSpeedupGauge.sizeDelta = new Vector2(Mathf.Clamp(630.0f - 630.0f * (m.nextSpeedupWait / m.nextSpeedupSeconds), 0.0f, 630.0f), 50.0f);
                spriteCount++;
                spriteCount %= 90;
                iGauge.sprite = sGauge[spriteCount < 24 ? spriteCount / 2 : 0];
                iGauge.color = new Color(1.0f, 1.0f, 1.0f, m.cursorPosition.y > 275.0f ? 0.5f : 1.0f);
            }
        }
    }
}

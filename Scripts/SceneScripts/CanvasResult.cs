using ChangeableDatabase;
using UnityEngine;
using UnityEngine.UI;
using UserDatabase;

public sealed class CanvasResult : MonoBehaviour
{
    [SerializeField] private Text tTotalCount;
    [SerializeField] private Text tSpeedLevel;
    [SerializeField] private Text tTotalTime;
    [SerializeField] private Image iPanelType;
    [SerializeField] private Image iDifficulty;

    public void LoadResultData()
    {
        FieldAdmin.FieldInfo f = CDB.FieldInfo;

        if (f == null)
        {
            return;
        }

        tTotalCount.text = $"{f.totalCount}";
        tSpeedLevel.text = $"{f.speedLevel}";
        tTotalTime.text = $"<b>{f.totalTime}</b>";

        iPanelType.sprite = UDB.GetSpritePanelType(UDB.DifficultyData.panel_type);
        iDifficulty.sprite = UDB.GetSpriteDifficulty(UDB.DifficultyData.difficulty_type);
    }
}

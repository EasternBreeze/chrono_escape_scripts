using ChangeableDatabase;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class CanvasRecord : MonoBehaviour
{
    [SerializeField] private List<Text> tRanks;
    [SerializeField] private GameObject pNewRecord;

    private bool isUpdate;
    private Image iNewRecord;

    public void LoadRecordData(string key)
    {
        List<int> list = RecordDataBase.GetRecords(key);

        if (list == null)
        {
            Debug.LogError("RecordData is null.");
            return;
        }

        for (int i = 0; i < tRanks.Count; i++)
        {
            tRanks[i].text = list[i] != -1 ? $"{list[i]}" : "---";
        }
    }

    public void LoadRecordData(string key, int newRecord)
    {
        LoadRecordData(key);

        if (newRecord < 1 || newRecord > 5)
        {
            return;
        }

        GameObject g = Instantiate(pNewRecord);
        g.transform.SetParent(transform);
        g.GetComponent<RectTransform>().position = new Vector2()
        {
            x = 320 - (20 * newRecord),
            y = 135 - (85 * newRecord)
        };

        iNewRecord = g.GetComponent<Image>();
        isUpdate = true;
    }

    public void HiddenRecordData()
    {
        for (int i = 0; i < tRanks.Count; i++)
        {
            tRanks[i].text = "---";
        }
    }

    private void Update()
    {
        if (!isUpdate)
        {
            return;
        }

        iNewRecord.color = new Color(1, 1, 1, Time.frameCount % 10 < 5 ? 0.75f : 1.0f);
    }
}

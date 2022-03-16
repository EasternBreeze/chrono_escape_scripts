using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PanelAdmin : MonoBehaviour
{
    [SerializeField] private GameObject prefabPanel;

    private Cursor cursor;

    private List<Panel> panels = new List<Panel>();

    private Vector2 panelPosOrigin;
    private float panelSize;
    private int panelRow;
    private int panelCol;
    private Vector2 fieldSize;

    public Cursor SetField(int row, int col)
    {
        cursor = Instantiate(Resources.Load<GameObject>("Prefabs/Cursor")).GetComponent<Cursor>();

        panelSize = 600 / (row > col ? row : col);
        Debug.Log(panelSize + " " + row + col);
        panelRow = row;
        panelCol = col;

        // パネル設置+リスト追加
        panelPosOrigin = new Vector2
        {
            x = -1.0f * panelSize * (col - 1) / 2.0f,
            y = -1.0f * panelSize * (row - 1) / 2.0f
        };
        for (int i = 0; i < row; i++)
        {
            Vector2 pos = new Vector2
            {
                x = 0.0f,
                y = panelPosOrigin.y + panelSize * i
            };

            for (int j = 0; j < col; j++)
            {
                pos.x = panelPosOrigin.x + panelSize * j;

                Panel p = Instantiate(prefabPanel).GetComponent<Panel>();

                p.Init(panelSize, pos);
                panels.Add(p);
            }
        }

        // cursorLimitMin/Max設定
        fieldSize = new Vector2
        {
            x = panelSize * col,
            y = panelSize * row
        };
        Vector2 max = fieldSize / 2.0f;
        Vector2 min = max * -1.0f;
        cursor.Init(min, max);

        return cursor;
    }

    public int GetCursorPanel()
    {
        Vector2 pos = cursor.GetCursorPosition() + fieldSize / 2.0f;

        int col = (int)(pos.x / panelSize);
        int row = (int)(pos.y / panelSize);
        col = Mathf.Clamp(col, 0, panelCol - 1);
        row = Mathf.Clamp(row, 0, panelRow - 1);

        return row * panelCol + col;
    }

    public void PanelCountDown()
    {
        panels.ForEach(p => p.CountDown());
    }

    public void PanelEffectError()
    {
        panels.ForEach(p => p.EffectError());
    }

    public void FieldUpdate()
    {
        // 無敵時間内であればreturn
        if (cursor.GetIsInvincible())
        {
            return;
        }
        if (CursorOnZeroCheck())
        {
            panels[GetCursorPanel()].SpriteFlash();
            cursor.TouchZero();
        }
    }

    public bool CursorOnZeroCheck()
    {
        return panels[GetCursorPanel()].GetCount() == 0;
    }
}

using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class Panel : MonoBehaviour
{
    private static List<Sprite> spritesPanel;
    private static Sprite spriteGray;

    private static int countLowerLimit = 1;
    private static int countUpperLimit = 9;

    private SpriteRenderer render;
    private int count = -1;

    private void Awake()
    {
        if (spritesPanel == null)
        {
            spritesPanel = Resources.LoadAll<Sprite>("Graphics/panel_Default").ToList();
            spriteGray = Resources.Load<Sprite>("Graphics/panel_Gray");
        }
    }

    public static void SetCountLimit(int lowerLimit, int upperLimit)
    {
        countLowerLimit = lowerLimit;
        countUpperLimit = upperLimit;
    }

    public static bool MinusCountLimit()
    {
        countLowerLimit -= countLowerLimit > 1 ? 1 : 0;
        countUpperLimit -= countUpperLimit > 3 ? 1 : 0;
        Debug.Log(countLowerLimit + " " + countUpperLimit);
        return countLowerLimit == 1 && countUpperLimit == 3;
    }

    public void Init(float panelSize, Vector2 panelPosition)
    {
        transform.position = panelPosition;
        transform.localScale = new Vector2(panelSize / 2, panelSize / 2);

        render = gameObject.GetComponent<SpriteRenderer>();
        render.sortingOrder = 100;
        render.sprite = spriteGray;
    }

    public int GetCount() { return count; }

    public void CountDown()
    {
        count--;
        if (count < 0)
        {
            count = Random.Range(countLowerLimit, countUpperLimit + 1);
        }
        SpriteRefresh();
    }

    public async void SpriteFlash()
    {
        for (int i = 0; i < 60; i++)
        {
            render.color = new Color(1.0f, 1.0f, 1.0f, Random.Range(0.5f, 1.0f));
            await UniTask.WaitForEndOfFrame();
        }
        render.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    public async void EffectError()
    {
        while (render != null)
        {
            render.sprite = spritesPanel[Random.Range(0, 2)];
            render.color = new Color(1.0f, 1.0f, 1.0f, Random.Range(0.5f, 1.0f));
            await UniTask.DelayFrame(2);
        }
    }

    private void SpriteRefresh()
    {
        render.sprite = spritesPanel[count];
    }

}

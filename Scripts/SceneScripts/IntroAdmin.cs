using ChangeableDatabase;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public sealed class IntroAdmin : MonoBehaviour
{
    [SerializeField] private GameObject blocker;
    [SerializeField] private GameObject gNavi;
    [SerializeField] private GameObject gShadow;
    [SerializeField] private RectTransform rCover;
    [SerializeField] private Image iCover;
    [SerializeField] private Image iSectionUI;

    [SerializeField] private GameObject cursor;
    [SerializeField] private GameObject textInvis, imageInvis;

    [SerializeField] private Text tSpeedLevel, tMaxLevel, tCountDownSeconds, tTotalCount, tTotalTime, tCursorLife;
    [SerializeField] private GameObject mask;
    [SerializeField] private Image iGauge;

    private List<string> naviTexts;
    private List<Sprite> spritesCover;
    private List<Sprite> spritesUI;

    private GameObject prefabPanel;
    private List<Panel> panels = new List<Panel>();

    private MousePointer pointer;

    private async void Awake()
    {
        blocker.SetActive(true);
        naviTexts = JsonUtility.FromJson<JNavigationTexts>(Resources.Load("System/NavigationTexts").ToString()).texts;
        spritesCover = new List<Sprite>()
        {
             Resources.Load<Sprite>("Graphics/shutter_IntroCoverAll"),
             Resources.Load<Sprite>("Graphics/shutter_IntroCoverCenter"),
             Resources.Load<Sprite>("Graphics/shutter_IntroCoverLeft"),
             Resources.Load<Sprite>("Graphics/shutter_IntroCoverSide")
        };
        spritesUI = new List<Sprite>()
        {
             Resources.Load<Sprite>("Graphics/ui_IntroSection1"),
             Resources.Load<Sprite>("Graphics/ui_IntroSection2"),
             Resources.Load<Sprite>("Graphics/ui_Mainfield")
        };
        prefabPanel = Resources.Load<GameObject>("Prefabs/Panel_Intro");
        pointer = Instantiate(Resources.Load<GameObject>("Prefabs/MousePointer")).AddComponent<MousePointer>();
        pointer.Init();

        SceneChanger.Instance.FadeOut();
        Sound.Instance.Play(SE.ShutterOpen);
        Sound.Instance.Play(BGM.Intro);
        await UniTask.DelayFrame(30);
        blocker.SetActive(false);

        Intro();
    }

    private async void Intro()
    {
        try
        {
            UpdateInfo();
            await Navi(0);
            await Navi(1);

            /** ***************
             * Section 1/3『操作方法』
             ** ***************/
            await Navi(2);

            await CloseCover(spritesCover[1]);
            cursor.AddComponent<Cursor>().Init(textInvis, imageInvis);
            cursor.SetActive(true);
            await OpenCover();
            await Navi(3);

            _ = Navi(4);
            cursor.GetComponent<Cursor>().targetPos = new Vector2()
            {
                x = -75.0f,
                y = 0.0f
            };
            pointer.Set(new Vector2(-75.0f, 75.0f), MousePointer.Arrow.Down, 100);
            await UniTask.DelayFrame(100);

            cursor.GetComponent<Cursor>().targetPos = new Vector2()
            {
                x = 75.0f,
                y = 0.0f
            };
            pointer.Set(new Vector2(75.0f, 75.0f), MousePointer.Arrow.Down, 100);
            await UniTask.DelayFrame(100);

            cursor.GetComponent<Cursor>().targetPos = new Vector2()
            {
                x = 225.0f,
                y = 0.0f
            };
            pointer.Set(new Vector2(225.0f, 75.0f), MousePointer.Arrow.Down, 100);
            await UniTask.DelayFrame(100);

            await CloseCover(spritesCover[1]);
            iSectionUI.sprite = spritesUI[0];
            iSectionUI.enabled = true;
            await OpenCover();

            _ = Navi(5);

            cursor.GetComponent<Cursor>().targetPos = new Vector2()
            {
                x = -250.0f,
                y = 200.0f
            };
            pointer.Set(new Vector2(-250.0f, 275.0f), MousePointer.Arrow.Down, 100);
            await UniTask.DelayFrame(100);

            cursor.GetComponent<Cursor>().targetPos = new Vector2()
            {
                x = 200.0f,
                y = 250.0f
            };
            pointer.Set(new Vector2(200.0f, 325.0f), MousePointer.Arrow.Down, 100);
            await UniTask.DelayFrame(100);

            cursor.GetComponent<Cursor>().targetPos = new Vector2()
            {
                x = 0.0f,
                y = 0.0f
            };
            pointer.Set(new Vector2(0.0f, 75.0f), MousePointer.Arrow.Down, 100);
            await UniTask.DelayFrame(100);

            /** ***************
            * Section 2/3『フィールド』
            ** ***************/
            await Navi(6);
            await CloseCover(spritesCover[1]);
            {
                int panelRow = 3;
                int panelCol = 3;

                Vector2 panelPosOrigin = new Vector2
                {
                    x = -1.0f * 200 * 2 / 2.0f,
                    y = -1.0f * 200 * 2 / 2.0f
                };
                for (int i = 0; i < panelRow; i++)
                {
                    Vector2 pos = new Vector2
                    {
                        x = 0.0f,
                        y = panelPosOrigin.y + 200 * i
                    };

                    for (int j = 0; j < panelCol; j++)
                    {
                        pos.x = panelPosOrigin.x + 200 * j;

                        Panel p = Instantiate(prefabPanel).AddComponent<Panel>();

                        p.Init(200, pos, i == 1 && j == 1);
                        panels.Add(p);
                    }
                }
            }
            await OpenCover();

            await Navi(7);
            CountDown(6);
            await Navi(8); // 300 +60 +60 +300   480

            await CloseCover(spritesCover[2]);
            iSectionUI.sprite = spritesUI[1];
            tCursorLife.enabled = true;
            await OpenCover();

            _ = Navi(9);
            await UniTask.DelayFrame(60);
            cursor.GetComponent<Cursor>().TouchZero();
            tCursorLife.text = "★★";
            await UniTask.DelayFrame(240);

            await Navi(10);
            await Navi(11);
            await Navi(12);

            /** ***************
            * Section 3/3『画面説明』
            ** ***************/
            await Navi(13);
            await CloseCover(spritesCover[3]);
            iSectionUI.sprite = spritesUI[2];
            tSpeedLevel.enabled = true;
            tMaxLevel.enabled = true;
            tCountDownSeconds.enabled = true;
            tTotalCount.enabled = true;
            tTotalTime.enabled = true;
            await OpenCover();
            await Navi(14);

            pointer.Set(new Vector2(-300.0f, 215.0f), MousePointer.Arrow.Left, 300);
            await Navi(15);

            pointer.Set(new Vector2(-300.0f, 40.0f), MousePointer.Arrow.Left, 300);
            await Navi(16);

            pointer.Set(new Vector2(-350.0f, -220.0f), MousePointer.Arrow.Left, 300);
            await Navi(17);

            pointer.Set(new Vector2(300.0f, 200.0f), MousePointer.Arrow.Right, 300);
            await Navi(18);

            pointer.Set(new Vector2(300.0f, 35.0f), MousePointer.Arrow.Right, 300);
            await Navi(19);

            await CloseCover(spritesCover[1]);
            mask.SetActive(true);
            await OpenCover();

            pointer.Set(new Vector2(0.0f, 260.0f), MousePointer.Arrow.Up, 300);
            await Navi(20);

            /** ***************
            * Section End『おわりに』
            ** ***************/
            await Navi(21);

            PressSkip();
        }
        catch (MissingReferenceException)
        {
            return;
        }
    }

    public void PressSkip()
    {
        Sound.Instance.Stop();
        Sound.Instance.Play(SE.ShutterClose);
        blocker.SetActive(true);
        if (ArchiveDataBase.GetIsFirstPlay())
        {
            SceneChanger.Instance.SceneChange(Scene.SelectScene);
        }
        else
        {
            SceneChanger.Instance.SceneChange(Scene.ExtraScene);
        }
    }

    private async UniTask Navi(int index)
    {
        RectTransform rNavi = gNavi.GetComponent<RectTransform>();
        Text t = gNavi.GetComponent<Text>();
        RectTransform rShadow = gShadow.GetComponent<RectTransform>();
        Image iShadow = gShadow.GetComponent<Image>();
        t.text = $"{naviTexts[index]}";

        for (int i = 1; i <= 120; i++)
        {
            float f = 20.0f * Mathf.Pow(1.0f - (i / 120.0f), 3.0f);
            rNavi.localPosition = new Vector2()
            {
                x = rNavi.localPosition.x,
                y = -200.0f - f
            };
            rShadow.localPosition = new Vector2()
            {
                x = rNavi.localPosition.x,
                y = -150.0f - f
            };

            Color c = new Color(1.0f, 1.0f, 1.0f, i / 120.0f);
            t.color = c;
            iShadow.color = c;
            await UniTask.WaitForEndOfFrame();
        }
        await UniTask.DelayFrame(160);
        for (int i = 1; i <= 20; i++)
        {
            Color c = new Color(1.0f, 1.0f, 1.0f, 1.0f - i / 20.0f);
            t.color = c;
            iShadow.color = c;
            await UniTask.WaitForEndOfFrame();
        }
    }

    private async UniTask CloseCover(Sprite sprite)
    {
        iCover.enabled = true;
        iCover.sprite = sprite;
        for (int i = 1; i < 60; i++)
        {
            rCover.sizeDelta = new Vector2()
            {
                x = 1280.0f,
                y = 720.0f * (1.0f - Mathf.Pow(1.0f - (i / 60.0f), 3.0f))
            };
            await UniTask.WaitForEndOfFrame();
        }
    }

    private async UniTask OpenCover()
    {
        for (int i = 1; i < 60; i++)
        {
            float f = 720.0f * Mathf.Pow(1.0f - (i / 60.0f), 3.0f);
            rCover.sizeDelta = new Vector2()
            {
                x = 1280.0f,
                y = f > 5.0f ? f : -1.0f
            };

            await UniTask.WaitForEndOfFrame();
        }
        iCover.enabled = false;
    }

    private async void CountDown(int counts)
    {
        for (int i = 0; i < counts; i++)
        {
            panels.ForEach(s => s.CountDown());
            Sound.Instance.Play(SE.CountDown);
            await UniTask.DelayFrame(120);
        }
    }

    private async void UpdateInfo()
    {
        float t = 0.0f;
        int c = 0;
        List<Sprite> sGauge = Resources.LoadAll<Sprite>("Graphics/gauge_Nextspeedup").ToList();
        try
        {
            while (true)
            {
                t += Time.deltaTime;
                tTotalTime.text = $"{(int)(t / 60.0f):00}:{(int)(t % 60.0f):00}.{(int)(t % 1.0f * 1000.0f):000}";

                c++;
                c %= 90;
                iGauge.sprite = sGauge[c < 24 ? c / 2 : 0];
                await UniTask.WaitForEndOfFrame();
            }
        }
        catch (MissingReferenceException)
        {
            return;
        }
    }

    [Serializable]
    public sealed class JNavigationTexts
    {
        public List<string> texts;
    }

    private sealed class Cursor : MonoBehaviour
    {
        private readonly float constInvincibleSeconds = 5.0f;

        internal Vector2 targetPos;

        private SpriteRenderer render;
        private GameObject prefabParticleCrash;

        private List<Sprite> sDefault;
        private List<Sprite> sNoise;

        private float invincibleSeconds;
        private bool isInvincible;

        private int spriteCount;

        private Text tCursorInvincibleSeconds;
        private Image iCursorInv;
        private RectTransform rCursorInv;

        internal void Init(GameObject textInvis, GameObject imageInvis)
        {
            render = GetComponent<SpriteRenderer>();
            prefabParticleCrash = Resources.Load<GameObject>("Prefabs/particle_CursorCrash");
            sDefault = Resources.LoadAll<Sprite>("Graphics/cursor_Default").ToList();
            sNoise = Resources.LoadAll<Sprite>("Graphics/cursor_Noise").ToList();

            tCursorInvincibleSeconds = textInvis.GetComponent<Text>();
            iCursorInv = imageInvis.GetComponent<Image>();
            rCursorInv = imageInvis.GetComponent<RectTransform>();

            targetPos = Vector2.zero;
            CursorPositionUpdate();
        }

        internal void TouchZero()
        {
            isInvincible = true;

            invincibleSeconds = constInvincibleSeconds;

            Sound.Instance.Play(SE.Miss);
            DrawParticleCrash(24);
        }

        private void Update()
        {
            spriteCount++;
            spriteCount %= sDefault.Count * 2;

            if (isInvincible)
            {
                invincibleSeconds -= Time.deltaTime;
                if (invincibleSeconds <= 0.0f)
                {
                    isInvincible = false;
                    render.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

                    tCursorInvincibleSeconds.text = "";
                    iCursorInv.fillAmount = 0.0f;
                }
                else
                {
                    render.sprite = sNoise[spriteCount / 2];
                    render.color = new Color(1.0f, 1.0f, 1.0f, Random.Range(0.75f, 1.0f));

                    tCursorInvincibleSeconds.text = $"{invincibleSeconds:0.0}";
                    iCursorInv.fillAmount = invincibleSeconds / 5.0f;
                    iCursorInv.color = new Color(1.0f, 1.0f, 1.0f, Random.Range(0.5f, 1.0f));

                    rCursorInv.position = transform.position;
                }
            }
            else
            {
                render.sprite = sDefault[spriteCount / 2];
            }
        }

        private async void CursorPositionUpdate()
        {
            try
            {
                while (true)
                {
                    transform.position = Vector2.Lerp(transform.position, targetPos, 0.05f);
                    await UniTask.WaitForEndOfFrame();
                }
            }
            catch
            {
                return;
            }
        }

        private void DrawParticleCrash(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                GameObject obj = Instantiate(prefabParticleCrash);
                obj.AddComponent<Particle>().Init(transform.position);
            }
        }

        private class Particle : MonoBehaviour
        {
            private SpriteRenderer render;
            private Vector2 vector;
            private Vector3 rotate;

            private int time;

            internal void Init(Vector2 position)
            {
                transform.position = position;
                transform.rotation = new Quaternion(0, 0, Random.Range(0.0f, 1.0f), 0);
                render = GetComponent<SpriteRenderer>();

                int scale = Random.Range(20, 40);
                transform.localScale = new Vector2()
                {
                    x = scale,
                    y = scale
                };

                vector = new Vector2()
                {
                    x = Random.Range(0.2f, 10.0f) * (Random.Range(0, 2) == 0 ? 1 : -1),
                    y = Random.Range(0.2f, 10.0f) * (Random.Range(0, 2) == 0 ? 1 : -1)
                };

                rotate = new Vector3()
                {
                    z = Random.Range(-5.0f, 5.0f)
                };
            }

            private void Update()
            {
                if (time > 90)
                {
                    Destroy(gameObject);
                }
                else if (time > 70)
                {
                    transform.localScale *= 0.95f;
                }
                transform.Translate(vector);
                transform.Rotate(rotate);
                render.color = new Color(1.0f, 1.0f, 1.0f, Random.Range(0.5f, 1.0f));

                vector *= 0.98f;
                rotate *= 0.98f;
                time++;
            }
        }
    }

    private sealed class Panel : MonoBehaviour
    {
        private static List<Sprite> spritesPanel;

        private static int countLowerLimit = 5;
        private static int countUpperLimit = 9;

        private SpriteRenderer render;
        private int count = -1;

        private void Awake()
        {
            if (spritesPanel == null)
            {
                spritesPanel = Resources.LoadAll<Sprite>("Graphics/panel_Default").ToList();
            }
        }

        internal void Init(float panelSize, Vector2 panelPosition, bool isConst)
        {
            transform.position = panelPosition;
            transform.localScale = new Vector2(panelSize / 2, panelSize / 2);

            render = gameObject.GetComponent<SpriteRenderer>();
            render.sortingOrder = 101;
            count = isConst ? 5 : Random.Range(countLowerLimit, countUpperLimit + 1);
            SpriteRefresh();
        }

        internal void CountDown()
        {
            count--;
            if (count < 0)
            {
                count = Random.Range(countLowerLimit, countUpperLimit + 1);
            }
            SpriteRefresh();
        }

        internal async void SpriteFlash()
        {
            for (int i = 0; i < 60; i++)
            {
                render.color = new Color(1.0f, 1.0f, 1.0f, Random.Range(0.5f, 1.0f));
                await UniTask.WaitForEndOfFrame();
            }
            render.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }

        private void SpriteRefresh()
        {
            render.sprite = spritesPanel[count];
        }
    }

    private sealed class MousePointer : MonoBehaviour
    {
        internal enum Arrow
        {
            Left, Down, Up, Right
        }

        private List<Sprite> sprites;
        private SpriteRenderer render;
        private GameObject prefab;

        internal void Init()
        {
            sprites = Resources.LoadAll<Sprite>("Graphics/button_LeftDownUpRight").ToList();

            prefab = Resources.Load<GameObject>("Prefabs/particle_Back");

            render = gameObject.GetComponent<SpriteRenderer>();
            render.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        }

        internal async void Set(Vector2 postision, Arrow dir, int frame)
        {
            try
            {
                transform.localPosition = postision;
                render.sprite = sprites[(int)dir];
                render.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                AttentionCircle(transform.localPosition);

                for (int i = 1; i <= 15; i++)
                {
                    render.color = new Color(1.0f, 1.0f, 1.0f, i / 15.0f);
                    await UniTask.WaitForEndOfFrame();
                }
                await UniTask.DelayFrame(frame - 30);
                for (int i = 1; i <= 15; i++)
                {
                    render.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - i / 15.0f);
                    await UniTask.WaitForEndOfFrame();
                }
            }
            catch (MissingReferenceException)
            {
                return;
            }
        }

        private async void AttentionCircle(Vector2 postision)
        {
            try
            {
                GameObject g = Instantiate(prefab);
                g.transform.position = postision;
                SpriteRenderer r = g.GetComponent<SpriteRenderer>();

                r.transform.localScale = new Vector2()
                {
                    x = 0.0f,
                    y = 0.0f
                };
                r.sprite = Resources.Load<Sprite>("Graphics/particle_CircleDot");
                r.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                r.sortingOrder = 251;

                for (int i = 1; i < 180; i++)
                {
                    if (i <= 30)
                    {
                        r.color = new Color(1.0f, 1.0f, 1.0f, i / 30.0f);
                    }
                    else if (i > 90)
                    {
                        r.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - (i - 90) / 90.0f);
                    }

                    float size = 200.0f * (1.0f - Mathf.Pow(1.0f - i / 180.0f, 3.0f));
                    g.transform.localScale = new Vector2()
                    {
                        x = size,
                        y = size
                    };

                    await UniTask.WaitForEndOfFrame();
                }

            }
            catch (MissingReferenceException)
            {
                return;
            }
        }
    }
}
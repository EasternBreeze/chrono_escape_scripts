/** ******************************
 * イージング関数チートシート
 * https://easings.net/ja
 ** ******************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using UserDatabase;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public enum Scene
{
    TitleScene, SelectScene, GameScene, ResultScene, ExtraScene, IntroScene, CreditScene
}

public enum FadeType
{
    Standard, GameStart, Failed, OverDrive
}

public sealed class SceneChanger : SingletonMonoBehaviour<SceneChanger>
{
    private readonly int inDelayframe = 90;

    [SerializeField] private GameObject shutterPrefab;
    [SerializeField] private GameObject shutterBasePrefab;
    [SerializeField] private GameObject shutterLinesPrefab;
    [SerializeField] private GameObject shutterGearPrefab;

    private List<Sprite> spritesShutterBase;
    private List<Sprite> spritesShutterBaseGray;
    private List<Sprite> spritesShutterBaseRed;
    private Sprite spriteTextReady;

    private List<Shutter> shutters = new List<Shutter>();

    protected override void Awake()
    {
        if (CheckInstance())
        {
            transform.position = new Vector2
            {
                x = -640,
                y = 360
            };
            spritesShutterBase = Resources.LoadAll<Sprite>("Graphics/shutter_StandardBase").ToList();
            spritesShutterBaseGray = Resources.LoadAll<Sprite>("Graphics/shutter_StandardBaseGray").ToList();
            spritesShutterBaseRed = Resources.LoadAll<Sprite>("Graphics/shutter_StandardBaseRed").ToList();

            spriteTextReady = Resources.Load<Sprite>("Graphics/text_Ready");
        }
    }

    public void SceneChange(Scene targetScene)
    {
        SceneChange(targetScene, FadeType.Standard);
    }

    public async void SceneChange(Scene targetScene, FadeType fadeType)
    {
        switch (fadeType)
        {
            case FadeType.Standard:
                Sound.Instance.Stop();
                FadeStandard();
                break;
            case FadeType.GameStart:
                FadeStandard();
                FadeDifficultyText();
                break;
            case FadeType.Failed:
                FadeFailed();
                break;
            case FadeType.OverDrive:
                FadeOverDrive();
                FadeDifficultyText();
                break;
        }

        shutters.ForEach(s => s.In());
        await UniTask.DelayFrame(inDelayframe);
        await UniTask.WaitForEndOfFrame();
        SceneManager.LoadScene($"{targetScene}");
    }

    public void FadeOut()
    {
        if (shutters.Count == 0)
        {
            return;
        }

        shutters.ForEach(s => s.Out());
        shutters.Clear();
    }

    public async UniTask GetReady()
    {
        GameObject g1 = Instantiate(shutterPrefab);
        g1.transform.SetParent(transform);
        g1.AddComponent<ShutterCoverReady>().Init();
        Shutter s1 = g1.GetComponent<ShutterCoverReady>();

        GameObject g2 = Instantiate(shutterPrefab);
        g2.transform.SetParent(transform);
        g2.AddComponent<ShutterReady321>().Init();
        Shutter s2 = g2.GetComponent<ShutterReady321>();

        s1.In();
        await UniTask.DelayFrame(60);
        s2.Out();
        await UniTask.DelayFrame(180);
        s1.Out();
        Sound.Instance.Play(SE.Go);
    }

    public async UniTask Gameover()
    {
        GameObject g = Instantiate(shutterPrefab);
        g.transform.SetParent(transform);
        g.AddComponent<ShutterBlackOut>().Init();
        Shutter s = g.GetComponent<ShutterBlackOut>();
        s.In();
        await UniTask.DelayFrame(30);
        s.Out();
    }

    private void FadeStandard() // 16 * 9 (80*80)
    {
        {
            List<int> delays = new List<int>();
            for (int i = 0; i < spritesShutterBase.Count; i++)
            {
                delays.Add((i / 16 + i % 16) * 2);
            }
            delays = delays.OrderBy(a => Guid.NewGuid()).ToList();
            for (int i = 0; i < spritesShutterBase.Count; i++)
            {
                GameObject g = Instantiate(shutterBasePrefab);
                g.transform.SetParent(transform);
                Vector2 pos = new Vector2
                {
                    x = i % 16 * 80 + 40,
                    y = i / 16 * -80 - 40
                };
                g.AddComponent<ShutterBase>().Init(spritesShutterBase[i], pos, i / 16 + i % 16, delays[0]);
                delays.RemoveAt(0);
                shutters.Add(g.GetComponent<ShutterBase>());
            }
        }

        {
            GameObject g = Instantiate(shutterLinesPrefab);
            g.transform.SetParent(transform);

            g.AddComponent<ShutterLines>().Init();
            shutters.Add(g.GetComponent<ShutterLines>());
        }

        {
            GameObject g = Instantiate(shutterGearPrefab);
            g.transform.SetParent(transform);
            Vector2 sPos = new Vector2
            {
                x = -300.0f,
                y = -940.0f
            };
            Vector2 gPos = new Vector2
            {
                x = 130.0f,
                y = -575.0f
            };

            g.AddComponent<ShutterGear>().Init(sPos, gPos, 100, true);
            shutters.Add(g.GetComponent<ShutterGear>());
        }

        {
            GameObject g = Instantiate(shutterGearPrefab);
            g.transform.SetParent(transform);
            Vector2 sPos = new Vector2
            {
                x = 570.0f,
                y = -950.0f
            };
            Vector2 gPos = new Vector2
            {
                x = 500.0f,
                y = -720.0f
            };

            g.AddComponent<ShutterGear>().Init(sPos, gPos, 80, false);
            shutters.Add(g.GetComponent<ShutterGear>());
        }
    }

    private void FadeFailed() // 16 * 9 (80*80)
    {
        {
            List<int> inDelays = new List<int>();
            for (int i = 0; i < spritesShutterBase.Count; i++)
            {
                inDelays.Add((16 - i / 16) * 2 + Random.Range(0, 5));
            }

            List<int> outDelays = new List<int>();
            for (int i = 0; i < spritesShutterBase.Count; i++)
            {
                outDelays.Add((i / 16 + i % 16) * 2);
            }
            outDelays = outDelays.OrderBy(a => Guid.NewGuid()).ToList();

            for (int i = 0; i < spritesShutterBase.Count; i++)
            {
                GameObject g = Instantiate(shutterBasePrefab);
                g.transform.SetParent(transform);
                Vector2 pos = new Vector2
                {
                    x = i % 16 * 80 + 40,
                    y = i / 16 * -80 - 40
                };

                g.AddComponent<ShutterBase>().Init(spritesShutterBaseGray[i], pos, inDelays[0], outDelays[0]);
                inDelays.RemoveAt(0);
                outDelays.RemoveAt(0);
                shutters.Add(g.GetComponent<ShutterBase>());
            }
        }

        {
            GameObject g = Instantiate(shutterLinesPrefab);
            g.transform.SetParent(transform);

            g.AddComponent<ShutterLines>().Init();
            shutters.Add(g.GetComponent<ShutterLines>());
        }

        {
            GameObject g = Instantiate(shutterGearPrefab);
            g.transform.SetParent(transform);
            Vector2 sPos = new Vector2
            {
                x = -300.0f,
                y = -940.0f
            };
            Vector2 gPos = new Vector2
            {
                x = 130.0f,
                y = -575.0f
            };

            g.AddComponent<ShutterGear>().Init(sPos, gPos, 100, true);
            g.GetComponent<ShutterGear>().render.sprite = Resources.Load<Sprite>("Graphics/gear_Gray");
            shutters.Add(g.GetComponent<ShutterGear>());
        }

        {
            GameObject g = Instantiate(shutterGearPrefab);
            g.transform.SetParent(transform);
            Vector2 sPos = new Vector2
            {
                x = 570.0f,
                y = -950.0f
            };
            Vector2 gPos = new Vector2
            {
                x = 500.0f,
                y = -720.0f
            };

            g.AddComponent<ShutterGear>().Init(sPos, gPos, 80, false);
            g.GetComponent<ShutterGear>().render.sprite = Resources.Load<Sprite>("Graphics/gear_Gray");
            shutters.Add(g.GetComponent<ShutterGear>());
        }
    }

    private void FadeOverDrive() // 32 * 18 (40*40)
    {
        {
            List<int> inDelays = new List<int>();
            for (int i = 0; i < spritesShutterBaseRed.Count; i++)
            {
                inDelays.Add((i % 3 == 0 ? 0 : (i % 3 == 1 ? 12 : 24)) + i / 32 + Random.Range(0, 5));
            }

            List<int> outDelays = new List<int>();
            for (int i = 0; i < spritesShutterBaseRed.Count; i++)
            {
                outDelays.Add((i / 32 + i % 32) * 2);
            }
            outDelays = outDelays.OrderBy(a => Guid.NewGuid()).ToList();
            for (int i = 0; i < spritesShutterBaseRed.Count; i++)
            {
                GameObject g = Instantiate(shutterBasePrefab);
                g.transform.SetParent(transform);
                Vector2 pos = new Vector2
                {
                    x = i % 32 * 40 + 20,
                    y = i / 32 * -40 - 20
                };
                g.AddComponent<ShutterBase>().Init(spritesShutterBaseRed[i], pos, inDelays[0], outDelays[0]);
                inDelays.RemoveAt(0);
                outDelays.RemoveAt(0);
                shutters.Add(g.GetComponent<ShutterBase>());
            }
        }

        {
            GameObject g = Instantiate(shutterLinesPrefab);
            g.transform.SetParent(transform);

            g.AddComponent<ShutterLines>().Init();
            shutters.Add(g.GetComponent<ShutterLines>());
        }

        {
            GameObject g = Instantiate(shutterGearPrefab);
            g.transform.SetParent(transform);
            Vector2 sPos = new Vector2
            {
                x = -300.0f,
                y = -940.0f
            };
            Vector2 gPos = new Vector2
            {
                x = 130.0f,
                y = -575.0f
            };

            g.AddComponent<ShutterGear>().Init(sPos, gPos, 100, true);
            g.GetComponent<ShutterGear>().render.sprite = Resources.Load<Sprite>("Graphics/gear_Red");
            shutters.Add(g.GetComponent<ShutterGear>());
        }

        {
            GameObject g = Instantiate(shutterGearPrefab);
            g.transform.SetParent(transform);
            Vector2 sPos = new Vector2
            {
                x = 570.0f,
                y = -950.0f
            };
            Vector2 gPos = new Vector2
            {
                x = 500.0f,
                y = -720.0f
            };

            g.AddComponent<ShutterGear>().Init(sPos, gPos, 80, false);
            g.GetComponent<ShutterGear>().render.sprite = Resources.Load<Sprite>("Graphics/gear_Red");
            shutters.Add(g.GetComponent<ShutterGear>());
        }
    }

    private void FadeDifficultyText()
    {
        {
            GameObject g = Instantiate(shutterPrefab);
            g.transform.SetParent(transform);
            Vector2 pos = new Vector2
            {
                x = 75.0f,
                y = -150.0f
            };
            Sprite s = UDB.GetSpritePanelType(UDB.DifficultyData.panel_type);
            g.AddComponent<ShutterTextDif>().Init(s, pos, 20);
            shutters.Add(g.GetComponent<ShutterTextDif>());
        }

        {
            GameObject g = Instantiate(shutterPrefab);
            g.transform.SetParent(transform);
            Vector2 pos = new Vector2
            {
                x = 200.0f,
                y = -225.0f
            };
            Sprite s = UDB.GetSpriteDifficulty(UDB.DifficultyData.difficulty_type);
            g.AddComponent<ShutterTextDif>().Init(s, pos, 0);
            shutters.Add(g.GetComponent<ShutterTextDif>());
        }

        {
            GameObject g = Instantiate(shutterPrefab);
            g.transform.SetParent(transform);
            Vector2 pos = new Vector2
            {
                x = 275.0f,
                y = -117.5f
            };
            g.AddComponent<ShutterReady>().Init(spriteTextReady, pos);
            shutters.Add(g.GetComponent<ShutterReady>());
        }
    }

    private abstract class Shutter : MonoBehaviour
    {
        internal SpriteRenderer render;
        internal abstract void In();
        internal abstract void Out();
    }

    private sealed class ShutterBase : Shutter
    {
        /** ******************************
         * ShutterBase
         ** ******************************/
        private int delayIn;
        private int delayOut;

        internal void Init(Sprite sprite, Vector2 position, int delayIn, int delayOut)
        {
            transform.localPosition = position;
            transform.localScale = Vector2.zero;
            render = GetComponent<SpriteRenderer>();
            render.sprite = sprite;

            SpriteRenderer child = GetComponentsInChildren<SpriteRenderer>()[1];
            child.sprite = sprite;

            this.delayIn = delayIn;
            this.delayOut = delayOut;
        }

        internal override async void In()
        {
            int wait = delayIn;
            while (wait > 0)
            {
                wait--;
                await UniTask.WaitForEndOfFrame();
            }

            for (int i = 1; i <= 30; i++)
            {
                transform.localScale = new Vector2
                {
                    x = 100 * i / 30.0f,
                    y = 100 * i / 30.0f
                };
                transform.Rotate(new Vector3(0.0f, 0.0f, -12.0f));
                await UniTask.WaitForEndOfFrame();
            }
            Destroy(transform.GetChild(0).gameObject);
        }

        internal override async void Out()
        {
            int wait = delayOut;
            while (wait > 0)
            {
                wait--;
                await UniTask.WaitForEndOfFrame();
            }

            for (int i = 1; i <= 30; i++)
            {
                float n = i / 30.0f;
                transform.localScale = new Vector2
                {
                    x = 100.0f + 30.0f * (1.0f - Mathf.Pow(1.0f - n, 3.0f)),
                    y = 100.0f + 30.0f * (1.0f - Mathf.Pow(1.0f - n, 3.0f))
                };
                render.color = new Color
                {
                    r = render.color.r,
                    g = render.color.g,
                    b = render.color.b,
                    a = 1.0f - i / 30.0f
                };
                await UniTask.WaitForEndOfFrame();
            }

            Destroy(gameObject);
        }
    }

    private sealed class ShutterLines : Shutter
    {
        /** ******************************
        * ShutterLines
        ** ******************************/
        internal void Init()
        {
            transform.localScale = new Vector2
            {
                x = 0.0f,
                y = 100.0f
            };
            transform.localPosition = new Vector2
            {
                x = 640.0f,
                y = -360.0f
            };
            render = GetComponent<SpriteRenderer>();
        }

        internal override async void In()
        {
            await UniTask.DelayFrame(20);
            for (int i = 1; i <= 30; i++)
            {
                transform.localScale = new Vector2
                {
                    x = 100.0f * (1.0f - Mathf.Pow(1.0f - i / 30.0f, 3.0f)),
                    y = 100.0f
                };
                await UniTask.WaitForEndOfFrame();
            }
        }

        internal override async void Out()
        {
            await UniTask.DelayFrame(10);
            for (int i = 1; i <= 60; i++)
            {
                float n = Mathf.Pow(i / 60.0f, 3.0f);
                transform.localScale = new Vector2
                {
                    x = 100.0f + 10.0f * n,
                    y = 100.0f + 17.0f * n
                };
                await UniTask.WaitForEndOfFrame();
            }
            Destroy(gameObject);
        }
    }

    private sealed class ShutterGear : Shutter
    {
        /** ******************************
          * ShutterGear
         ** ******************************/
        private Vector2 startPosition;
        private Vector2 goalPosition;
        private bool isClockwise;

        internal void Init(Vector2 startPosition, Vector2 goalPosition, int scale, bool isClockwise)
        {
            this.startPosition = startPosition;
            this.goalPosition = goalPosition;
            transform.localPosition = startPosition;
            transform.Rotate(0.0f, 0.0f, 60.0f);
            transform.localScale = new Vector2
            {
                x = scale,
                y = scale
            };
            this.isClockwise = isClockwise;
            render = GetComponent<SpriteRenderer>();
            render.sortingOrder = isClockwise ? 1011 : 1010;
        }

        internal override async void In()
        {
            await UniTask.DelayFrame(15);

            Vector2 move = goalPosition - startPosition;
            for (int i = 1; i <= 60; i++)
            {
                float n = i / 60.0f;
                transform.localPosition = startPosition + move * (1.0f - Mathf.Pow(1.0f - n, 3.0f));
                if (isClockwise)
                {
                    transform.Rotate(0.0f, 0.0f, -5.0f * (1.0f - Mathf.Sin(n * Mathf.PI / 2)));
                }
                else
                {
                    transform.Rotate(0.0f, 0.0f, 5.0f * (1.0f - Mathf.Sin(n * Mathf.PI / 2)));
                }

                await UniTask.WaitForEndOfFrame();
            }
        }

        internal override async void Out()
        {
            await UniTask.DelayFrame(15);

            Vector2 move = startPosition - goalPosition;
            for (int i = 1; i <= 60; i++)
            {
                float n = Mathf.Pow(i / 60.0f, 3.0f);
                transform.localPosition = goalPosition + move * n;
                if (isClockwise)
                {
                    transform.Rotate(0.0f, 0.0f, -5.0f * n);
                }
                else
                {
                    transform.Rotate(0.0f, 0.0f, 5.0f * n);
                }

                await UniTask.WaitForEndOfFrame();
            }

            Destroy(gameObject);
        }
    }

    private sealed class ShutterTextDif : Shutter
    {
        /** ******************************
        * ShutterText
        ** ******************************/
        private Vector2 origin;
        private int delay;
        internal void Init(Sprite sprite, Vector2 position, int delay)
        {
            render = GetComponent<SpriteRenderer>();
            render.sprite = sprite;
            render.sortingOrder = 1005;
            transform.localPosition = position;
            origin = position;
            this.delay = delay;

            render.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        }

        internal override async void In()
        {
            int wait = delay;
            while (wait > 0)
            {
                wait--;
                await UniTask.WaitForEndOfFrame();
            }
            await UniTask.DelayFrame(15);

            for (int i = 0; i < 90; i++)
            {
                transform.localPosition = new Vector2
                {
                    x = origin.x + 50.0f * (1.0f - Mathf.Pow(1.0f - i / 90.0f, 3.0f)),
                    y = origin.y
                };
                render.color = new Color(1.0f, 1.0f, 1.0f, i / 90.0f);
                await UniTask.WaitForEndOfFrame();
            }
        }

        internal override async void Out()
        {
            for (int i = 0; i < 60; i++)
            {
                render.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - i / 60.0f);
                await UniTask.WaitForEndOfFrame();
            }
            Destroy(gameObject);
        }

    }

    private sealed class ShutterReady : Shutter
    {
        /** ******************************
        * ShutterReady
        ** ******************************/
        private Vector2 origin;
        private bool isLoop;
        private int tick;

        internal void Init(Sprite sprite, Vector2 position)
        {
            render = GetComponent<SpriteRenderer>();
            render.sprite = sprite;
            render.sortingOrder = 1004;
            render.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            transform.localPosition = position;
            origin = position;
        }

        internal override async void In()
        {
            await UniTask.DelayFrame(15);
            for (int i = 0; i < 90; i++)
            {
                transform.localPosition = new Vector2
                {
                    x = origin.x,
                    y = origin.y - 50.0f * (1.0f - Mathf.Pow(1.0f - i / 90.0f, 3.0f))
                };
                render.color = new Color(1.0f, 1.0f, 1.0f, i % 4 < 2 ? 0.5f * i / 90.0f : i / 90.0f);
                await UniTask.WaitForEndOfFrame();
            }
            isLoop = true;
            Loop();
        }

        internal override async void Out()
        {
            isLoop = false;
            for (int i = 0; i < 60; i++)
            {
                render.color = new Color(1.0f, 1.0f, 1.0f, i % 4 < 2 ? 0.5f - (0.5f * i / 60.0f) : 1.0f - i / 60.0f);
                await UniTask.WaitForEndOfFrame();
            }

            Destroy(gameObject);
        }

        private async void Loop()
        {
            while (isLoop)
            {
                tick++;
                render.color = new Color(1.0f, 1.0f, 1.0f, tick % 4 < 2 ? 0.5f : 1.0f);
                await UniTask.WaitForEndOfFrame();
            }
        }
    }

    private sealed class ShutterCoverReady : Shutter
    {
        /** ******************************
        * ShutterCoverReady
        ** ******************************/
        internal void Init()
        {
            render = GetComponent<SpriteRenderer>();
            transform.localScale = new Vector2
            {
                x = 0.0f,
                y = 100.0f
            };
            transform.localPosition = new Vector2
            {
                x = 640.0f,
                y = -360.0f
            };
            render.sprite = Resources.Load<Sprite>("Graphics/shutter_GetReady");
            render.sortingOrder = 1001;
        }

        internal override async void In()
        {
            await UniTask.DelayFrame(20);
            for (int i = 1; i <= 30; i++)
            {
                transform.localScale = new Vector2
                {
                    x = 100.0f * (1.0f - Mathf.Pow(1.0f - i / 30.0f, 3.0f)),
                    y = 100.0f
                };
                await UniTask.WaitForEndOfFrame();
            }
        }

        internal override async void Out()
        {
            for (int i = 0; i < 20; i++)
            {
                render.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - i / 20.0f);
                await UniTask.WaitForEndOfFrame();
            }
            Destroy(gameObject);
        }
    }

    private sealed class ShutterReady321 : Shutter
    {
        /** ******************************
        * ShutterReady321
        ** ******************************/
        private List<Sprite> sprites;

        internal void Init()
        {
            render = GetComponent<SpriteRenderer>();
            transform.localPosition = new Vector2
            {
                x = 640.0f,
                y = -400.0f
            };
            sprites = Resources.LoadAll<Sprite>("Graphics/text_Ready321").ToList();
            render.sprite = sprites[0];
            render.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            render.sortingOrder = 1002;
        }

        internal override void In()
        {
        }

        internal override async void Out()
        {
            render.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            for (int i = 0; i <= 2; i++)
            {
                render.sprite = sprites[i];
                Sound.Instance.Play(SE.Ready);
                await Wait();
            }

            Destroy(gameObject);
        }

        private async UniTask Wait()
        {
            for (int i = 0; i < 60; i++)
            {
                render.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - i / 60.0f);
                await UniTask.WaitForEndOfFrame();
            }
        }
    }

    private sealed class ShutterBlackOut : Shutter
    {
        /** ******************************
        * ShutterBlackOut
        ** ******************************/
        internal void Init()
        {
            render = GetComponent<SpriteRenderer>();
            transform.localPosition = new Vector2
            {
                x = 640.0f,
                y = -360.0f
            };
            render.sprite = Resources.Load<Sprite>("Graphics/shutter_BlackoutOverlay");
            render.sortingOrder = 1001;
        }

        internal override async void In()
        {
            for (int i = 1; i <= 90; i++)
            {
                render.color = new Color(1.0f, 1.0f, 1.0f, i / 90.0f);
                await UniTask.WaitForEndOfFrame();
            }
        }

        internal override async void Out()
        {
            await UniTask.DelayFrame(90);

            Destroy(gameObject);
        }
    }
}

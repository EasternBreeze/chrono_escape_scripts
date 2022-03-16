using ChangeableDatabase;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class TitleAdmin : MonoBehaviour
{
    [SerializeField] private SpriteRenderer renderClick;
    [SerializeField] private List<GameObject> randomShards;
    [SerializeField] private GameObject blocker;
    [SerializeField] private Text info;
    [SerializeField] private SpriteRenderer cover;

    private List<Sprite> spritesRandomShard = new List<Sprite>();

    private async void Awake()
    {
        info.text = $"ver.{Application.version}\n(C) 2021-2022 EasternBreeze";
        for (int i = 1; i <= 5; i++)
        {
            spritesRandomShard.Add(Resources.Load<Sprite>($"Graphics/particle_RandomShard{i}"));
        }
        EffectClickToStart();
        for (int i = 0; i < randomShards.Count; i++)
        {
            EffectRandomShard(randomShards[i], 300 + i * 45);
        }

        Sound.Instance.Play(BGM.Title);

        for (int i = 1; i <= 60; i++)
        {
            cover.color = new Color(0.0f, 0.0f, 0.0f, 1.0f - i / 60.0f);
            await UniTask.WaitForEndOfFrame();
        }
        blocker.SetActive(false);
    }

    public void PressStart()
    {
        Sound.Instance.Stop();
        Sound.Instance.Play(SE.TitleGate);
        blocker.SetActive(true);
        if (ArchiveDataBase.GetIsFirstPlay())
        {
            SceneChanger.Instance.SceneChange(Scene.IntroScene);
        }
        else
        {
            SceneChanger.Instance.SceneChange(Scene.SelectScene);
        }
    }

    private async void EffectClickToStart()
    {
        try
        {
            while (true)
            {
                renderClick.color = new Color(1.0f, 1.0f, 1.0f, Random.Range(0.5f, 1.0f));
                await UniTask.WaitForEndOfFrame();
            }
        }
        catch (MissingReferenceException)
        {
            return;
        }
    }

    private async void EffectRandomShard(GameObject randomShard, int frame)
    {
        Transform t = randomShard.transform;
        SpriteRenderer r = randomShard.GetComponent<SpriteRenderer>();

        int fadeIn = frame * 4 / 10;
        int fadeOut = frame - fadeIn;

        while (t != null || r != null)
        {
            r.sprite = spritesRandomShard[Random.Range(0, spritesRandomShard.Count)];
            t.localScale = new Vector2
            {
                x = 100.0f,
                y = 100.0f
            };
            r.color = new Color
            {
                r = 1.0f,
                g = 1.0f,
                b = 1.0f,
                a = 0.0f
            };

            for (int i = 1; i <= frame; i++)
            {
                if (t == null || r == null)
                {
                    return;
                }

                t.localScale = new Vector2
                {
                    x = 100.0f + 10.0f * i / frame,
                    y = 100.0f + 10.0f * i / frame
                };
                if (i < fadeIn)
                {
                    r.color = new Color
                    {
                        r = 1.0f,
                        g = 1.0f,
                        b = 1.0f,
                        a = 1.0f * i / fadeIn
                    };
                }
                else
                {
                    r.color = new Color
                    {
                        r = 1.0f,
                        g = 1.0f,
                        b = 1.0f,
                        a = 1.0f - 1.0f * (i - fadeIn) / fadeOut
                    };
                }
                await UniTask.WaitForEndOfFrame();
            }
        }
    }
}

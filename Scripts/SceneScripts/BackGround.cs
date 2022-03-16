using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UserDatabase;

public sealed class BackGround : MonoBehaviour
{
    [SerializeField] private Scene scene;

    private GameObject prefab;

    private void Awake()
    {
        prefab = Resources.Load<GameObject>("Prefabs/particle_Back");
        switch (scene)
        {
            case Scene.TitleScene:
                break;
            case Scene.SelectScene:
                SelectScene();
                break;
            case Scene.GameScene:
                GameScene();
                break;
            case Scene.ResultScene:
                ResultScene();
                break;
            case Scene.ExtraScene:
                SelectScene();
                break;
            case Scene.IntroScene:
                GameScene();
                break;
            case Scene.CreditScene:
                SelectScene();
                break;
        }
    }

    private async void SelectScene()
    {
        Sprite squDot = Resources.Load<Sprite>("Graphics/particle_SquareDot");
        try
        {
            while (true)
            {
                Vector2 pos = new Vector2()
                {
                    x = Random.Range(-640.0f, 640.0f),
                    y = Random.Range(-360.0f, 360.0f)
                };
                float angle = Random.Range(0.0f, 360.0f);

                for (int i = 0; i < 5; i++)
                {
                    GameObject g = Instantiate(prefab);
                    g.AddComponent<SquareDot>().SetSprite(squDot, transform);
                    g.GetComponent<SquareDot>().Init(240, pos, i * 40 + 50, angle + i * 30);
                    g.GetComponent<SquareDot>().Move();
                    await UniTask.DelayFrame(15);
                }
                await UniTask.DelayFrame(60);
            }
        }
        catch (MissingReferenceException)
        {
            return;
        }

    }

    private async void GameScene()
    {
        if (UDB.DifficultyData.difficulty_type == DifficultyType.OVERDRIVE)
        {
            GetComponentInChildren<SpriteRenderer>().color = new Color(1.0f, 0.33f, 0.66f);
        }

        Sprite squDot = Resources.Load<Sprite>("Graphics/particle_SquareDot");
        try
        {
            float mul = 1;
            while (true)
            {
                mul *= -1;

                GameObject g = Instantiate(prefab);
                g.AddComponent<SquareWide>().SetSprite(squDot, transform);
                g.GetComponent<SquareWide>().Init(Random.Range(0.0f, 360.0f), mul);
                g.GetComponent<SquareWide>().Move();

                await UniTask.DelayFrame(600 - Sound.Instance.GetPitchRank() * 60);
            }
        }
        catch (MissingReferenceException)
        {
            return;
        }
    }

    private async void ResultScene()
    {
        if (UDB.DifficultyData.difficulty_type == DifficultyType.OVERDRIVE)
        {
            GetComponentInChildren<SpriteRenderer>().color = new Color(1.0f, 0.33f, 0.66f);
        }

        Sprite squFill = Resources.Load<Sprite>("Graphics/particle_SquareFill");

        try
        {
            for (int i = 1; i <= 5; i++)
            {
                GameObject g = Instantiate(prefab);
                g.AddComponent<SquareFall>().SetSprite(squFill, transform);
                g.GetComponent<SquareFall>().Init(Random.Range(-640.0f, 640.0f), Random.Range(20.0f, 100.0f), Random.Range(-1.0f, 1.0f));
                g.GetComponent<SquareFall>().Fall(i * 120);
                g.GetComponent<SquareFall>().Move();
            }
            while (true)
            {
                GameObject g1 = Instantiate(prefab);
                g1.AddComponent<SquareFall>().SetSprite(squFill, transform);
                g1.GetComponent<SquareFall>().Init(Random.Range(-640.0f, 320.0f), Random.Range(20.0f, 100.0f), Random.Range(-1.0f, 1.0f));
                g1.GetComponent<SquareFall>().Move();

                await UniTask.DelayFrame(Random.Range(60, 120));

                GameObject g2 = Instantiate(prefab);
                g2.AddComponent<SquareFall>().SetSprite(squFill, transform);
                g2.GetComponent<SquareFall>().Init(Random.Range(-320.0f, 640.0f), Random.Range(20.0f, 100.0f), Random.Range(-1.0f, 1.0f));
                g2.GetComponent<SquareFall>().Move();

                await UniTask.DelayFrame(Random.Range(60, 120));
            }
        }
        catch (MissingReferenceException)
        {
            return;
        }
    }

    /** ******************************
     * ParticleClass
     ** ******************************/
    private abstract class Particle : MonoBehaviour
    {
        internal SpriteRenderer render;

        protected static readonly Color inv = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        internal void SetSprite(Sprite sprite, Transform parent)
        {
            transform.SetParent(parent);
            render = GetComponent<SpriteRenderer>();
            render.sprite = sprite;
        }
        internal abstract void Move();
    }

    private sealed class SquareDot : Particle
    {
        private int frame;

        internal void Init(int frame, Vector2 pos, float size, float angle)
        {
            this.frame = frame;
            render.color = inv;

            transform.position = pos;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            transform.localScale = new Vector2()
            {
                x = size,
                y = size
            };
        }

        internal override async void Move()
        {
            try
            {
                Vector3 scaleDown = new Vector3()
                {
                    x = 0.3f,
                    y = 0.3f,
                    z = 0.0f
                };

                for (int i = 1; i <= frame; i++)
                {

                    if (i <= 30)
                    {
                        render.color = new Color(1.0f, 1.0f, 1.0f, i / 30.0f);
                    }
                    else if (i > frame - 30)
                    {
                        render.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - (i - (frame - 30)) / 30.0f);
                    }

                    transform.localScale -= scaleDown;
                    await UniTask.WaitForEndOfFrame();
                }

                Destroy(gameObject);
            }
            catch (MissingReferenceException)
            {
                return;
            }
        }
    }

    private sealed class SquareFall : Particle
    {
        private Vector3 rotate;
        private float deadLine;
        private float fallSpeed;

        internal void Init(float posX, float size, float rotate)
        {
            this.rotate = new Vector3()
            {
                z = rotate
            };
            this.deadLine = -450.0f - size;
            this.fallSpeed = Random.Range(1.0f, 2.0f);

            transform.localPosition = new Vector2()
            {
                x = posX,
                y = 450.0f + size
            };
            transform.localRotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(0.0f, 360.0f)));
            transform.localScale = new Vector2()
            {
                x = size,
                y = size
            };
        }

        internal override async void Move()
        {
            try
            {
                int tick = 0;

                while (transform.localPosition.y > deadLine)
                {
                    tick++;
                    Fall(1);
                    render.color = new Color(1.0f, 1.0f, 1.0f, Alpha(tick));
                    await UniTask.WaitForEndOfFrame();
                }
                Destroy(gameObject);
            }
            catch (MissingReferenceException)
            {
                return;
            }
        }

        internal void Fall(int mul)
        {
            transform.Translate(0.0f, -1.0f * fallSpeed * mul, 0.0f, Space.World);
            transform.Rotate(rotate * mul);
        }

        private float Alpha(int tick)
        {
            tick %= 180;
            return tick < 90 ? tick / 90.0f : 1.0f - (tick - 90) / 90.0f;
        }
    }

    private sealed class SquareWide : Particle
    {
        private static readonly float upSize = 1.0f;
        private static readonly Vector3 rotate = new Vector3()
        {
            z = 0.05f
        };

        private float mul;

        internal void Init(float angle, float mul)
        {
            this.mul = mul;
            render.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);

            transform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            transform.localScale = new Vector2()
            {
                x = 0.0f,
                y = 0.0f
            };
        }

        internal override async void Move()
        {
            try
            {
                for (int i = 0; i < 900; i++)
                {
                    transform.localScale = new Vector2()
                    {
                        x = transform.localScale.x + upSize,
                        y = transform.localScale.y + upSize
                    };
                    transform.Rotate(rotate * mul);

                    await UniTask.WaitForEndOfFrame();
                }

                Destroy(gameObject);
            }
            catch (MissingReferenceException)
            {
                return;
            }
        }
    }
}
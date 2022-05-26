using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UserDatabase;

public sealed class Cursor : MonoBehaviour
{
    private readonly float constInvincibleSeconds = 5.0f;
    private readonly Vector2 positionAdjust = new Vector2(640.0f, 360.0f);

    [SerializeField] private SpriteRenderer render;
    [SerializeField] private GameObject prefabParticleCrash;

    private List<Sprite> sDefault;
    private List<Sprite> sNoise;

    private Vector2 cursorLowerLimit;
    private Vector2 cursorUpperLimit;

    private int life;
    private float invincibleSeconds;
    private bool isInvincible;

    private int spriteCount;

    private void Awake()
    {
        sDefault = Resources.LoadAll<Sprite>("Graphics/cursor_Default").ToList();
        sNoise = Resources.LoadAll<Sprite>("Graphics/cursor_Noise").ToList();
    }

    public float GetInvincibleSeconds() { return invincibleSeconds; }
    public int GetLife() { return life; }
    public bool GetIsInvincible() { return isInvincible; }
    public Vector2 GetCursorPosition() { return transform.position; }

    public void Init(Vector2 cursorLowerLimit, Vector2 cursorUpperLimit)
    {
        this.cursorLowerLimit = cursorLowerLimit;
        this.cursorUpperLimit = cursorUpperLimit;
        life = UDB.DifficultyData.player_life;

        render.sortingOrder = 200;
    }
    public void TouchZero()
    {
        life--;
        isInvincible = true;
        if (life > 0)
        {
            invincibleSeconds = constInvincibleSeconds;
            if (life == 1)
            {
                Sound.Instance.Play(SE.LifeAlert);
            }
            Sound.Instance.Play(SE.Miss);
            DrawParticleCrash(24);
        }
        else
        {
            Sound.Instance.Play(SE.Miss);
            DrawParticleCrash(72);
            render.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        }
    }

    public void CursorUpdate(float delta)
    {
        if (life <= 0)
        {
            return;
        }

        CursorPositionUpdate();
        spriteCount++;
        spriteCount %= sDefault.Count * 2;

        if (isInvincible)
        {
            invincibleSeconds -= delta;
            if (invincibleSeconds <= 0.0f)
            {
                isInvincible = false;
                render.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
            else
            {
                render.sprite = sNoise[spriteCount / 2];
                render.color = new Color(1.0f, 1.0f, 1.0f, Random.Range(0.75f, 1.0f));
            }
        }
        else
        {
            render.sprite = sDefault[spriteCount / 2];
        }
    }

    private void CursorPositionUpdate()
    {
        Vector2 pos = Input.mousePosition;
        pos *= 1.0f * 1280 / Screen.width;
        pos -= positionAdjust;
        pos.x = Mathf.Clamp(pos.x, cursorLowerLimit.x, cursorUpperLimit.x);
        pos.y = Mathf.Clamp(pos.y, cursorLowerLimit.y, cursorUpperLimit.y);

        transform.position = Vector2.Lerp(transform.position, pos, 0.25f);
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

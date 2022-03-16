using ChangeableDatabase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UserDatabase;
using static UserDatabase.JDifficultyDataList;

public sealed class FieldAdmin : MonoBehaviour
{
    [SerializeField] private PanelAdmin panels;
    [SerializeField] private CanvasInfo canvas;
    private Cursor cursor;

    private List<float> speedTable;

    private int totalCount; // �J�E���g��
    private float totalSecond;�@// �ϋv����(sec)

    private int speedLevel; // ���݂̃X�s�[�h���x��
    private int minSpeedLevel; // �����X�s�[�h���x��(const)
    private int maxSpeedLevel; // �ő�X�s�[�h���x��(const)
    private int limitOver;
    private float nextSpeedupSeconds; // �X�s�[�h�A�b�v�܂ł̐ݒ�l(sec)
    private float addSpeedupSeconds;
    private float countDownSeconds; // �J�E���g�_�E���܂ł̐ݒ�l(sec)

    private float nextSpeedupWait; // ���X�s�[�h�A�b�v�܂�(sec)
    private float countDownWait; // ���J�E���g�_�E���܂�(sec)
    private bool isSpeedup; // ���J�E���g�_�E���ɂăX�s�[�h�A�b�v�\��t���O
    private bool isMaxSpeedLevel; // �ő僌�x���ł��邩
    private bool isFullLevelup;

    public class FieldInfo
    {
        public readonly int totalCount;
        public readonly float totalSecond;
        public readonly string totalTime;

        public readonly int speedLevel;
        public readonly int maxSpeedLevel;
        public readonly int limitOver;
        public readonly float nextSpeedupSeconds;
        public readonly float countDownSeconds;
        public readonly float nextSpeedupWait;
        public readonly float countDownWait;
        public readonly bool isSpeedup;
        public readonly bool isMaxSpeedLevel;
        public readonly bool isFullLevelup;

        public readonly int cursorLife;
        public readonly float cursorInvincibleSeconds;
        public readonly Vector2 cursorPosition;
        internal FieldInfo(FieldAdmin m)
        {
            this.totalCount = m.totalCount;
            this.totalSecond = m.totalSecond;
            float t = m.totalSecond;
            this.totalTime = $"{(int)(t / 60.0f):00}:{(int)(t % 60.0f):00}.{(int)(t % 1.0f * 1000.0f):000}";

            this.speedLevel = m.speedLevel;
            this.maxSpeedLevel = m.maxSpeedLevel;
            this.limitOver = m.limitOver;
            this.nextSpeedupSeconds = m.nextSpeedupSeconds;
            this.countDownSeconds = m.countDownSeconds;
            this.nextSpeedupWait = m.nextSpeedupWait;
            this.countDownWait = m.countDownWait;
            this.isSpeedup = m.isSpeedup;
            this.isMaxSpeedLevel = m.isMaxSpeedLevel;
            this.isFullLevelup = m.isFullLevelup;

            this.cursorLife = m.cursor.GetLife();
            this.cursorInvincibleSeconds = m.cursor.GetInvincibleSeconds();
            this.cursorPosition = m.cursor.GetCursorPosition();
        }
    }

    public void GameInit()
    {
        Difficulty difficulty = UDB.DifficultyData;

        speedTable = UDB.SpeedTable;
        speedLevel = difficulty.speedlevel_start;
        minSpeedLevel = difficulty.speedlevel_start;
        maxSpeedLevel = difficulty.speedlevel_max;
        nextSpeedupSeconds = difficulty.seconds_speedup_start;
        addSpeedupSeconds = difficulty.add_speedup_seconds;
        countDownSeconds = speedTable[speedLevel];

        nextSpeedupWait = nextSpeedupSeconds;

        cursor = panels.SetField(difficulty.panel_row, difficulty.panel_col); // 200 3 3
        Panel.SetCountLimit(difficulty.panel_count_lower_limit, difficulty.panel_count_upper_limit);

        canvas.Init(new FieldInfo(this));
    }

    public FieldInfo GetFieldInfo() { return new FieldInfo(this); }
    public Cursor GetCursor() { return cursor; }


    /** ********** ********** **********
     * GameUpdate���\�b�h�Q
     ** ********** ********** **********/
    public bool GameUpdate()
    {
        float delta = Time.deltaTime;
        totalSecond += delta;
        NextSpeedUp(delta);
        CountDown(delta);

        cursor.CursorUpdate(delta);
        panels.FieldUpdate();
        canvas.InfoUpdate(new FieldInfo(this));

        if (cursor.GetLife() <= 0)
        {
            panels.PanelEffectError();
            return true;
        }
        return false;
    }

    private void NextSpeedUp(float delta)
    {
        nextSpeedupWait -= delta;
        if (nextSpeedupWait <= 0.0f && !isSpeedup)
        {
            isSpeedup = true;
        }
    }
    private void CountDown(float delta)
    {
        countDownWait -= delta;
        if (countDownWait <= 0.0f)
        {
            panels.PanelCountDown();
            Sound.Instance.Play(SE.CountDown);

            totalCount++;
            if (isSpeedup && !isFullLevelup)
            {
                if (isMaxSpeedLevel)
                {
                    nextSpeedupWait = 30.0f;
                    limitOver++;
                    isFullLevelup = Panel.MinusCountLimit();
                }
                else
                {
                    speedLevel++;
                    isMaxSpeedLevel = speedLevel >= maxSpeedLevel;
                    countDownSeconds = speedTable[speedLevel];
                    nextSpeedupSeconds += addSpeedupSeconds;
                    nextSpeedupWait = isMaxSpeedLevel ? 30.0f : nextSpeedupSeconds;

                    if (speedLevel % 5 == 0)
                    {
                        Sound.Instance.AdjustPitch(speedLevel, minSpeedLevel);
                    }
                }
                isSpeedup = false;
                Sound.Instance.Play(SE.Speedup);
            }

            countDownWait += countDownSeconds;
        }
    }

}



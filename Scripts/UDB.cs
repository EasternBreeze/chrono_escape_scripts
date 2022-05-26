using ChangeableDatabase;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UserDatabase.JDifficultyDataList;

namespace UserDatabase
{
    public enum PanelType
    {
        P3x3, P4x4, P5x5
    }
    public enum DifficultyType
    {
        BASIC, NORMAL, EXPERT, OVERDRIVE, BLACKOUT
    }

    /** ******************************
     * UserDatabase (ユーザデータベース)
     ** ******************************/
    public static class UDB
    {
        private static readonly List<float> speedTable =
            JsonUtility.FromJson<JSpeedDataList>(Resources.Load("System/SpeedDataList").ToString()).speedTable;
        private static readonly List<Difficulty> difficultyTable =
            JsonUtility.FromJson<JDifficultyDataList>(Resources.Load("System/DifficultyDataList").ToString()).difficultyTable;

        private static readonly List<Sprite> spritesPanelType = new List<Sprite>()
        {
            Resources.Load<Sprite>("Graphics/text_PanelType3x3"),
            Resources.Load<Sprite>("Graphics/text_PanelType4x4"),
            Resources.Load<Sprite>("Graphics/text_PanelType5x5")
        };
        private static readonly List<Sprite> spritesDifficulty = new List<Sprite>()
        {
            Resources.Load<Sprite>("Graphics/text_DifficultyBasic"),
            Resources.Load<Sprite>("Graphics/text_DifficultyNormal"),
            Resources.Load<Sprite>("Graphics/text_DifficultyExpert"),
            Resources.Load<Sprite>("Graphics/text_DifficultyOverDrive"),
            Resources.Load<Sprite>("Graphics/text_DifficultyBlackOut")
        };

        public static List<float> SpeedTable
        {
            get => new List<float>(speedTable);
        }

        public static List<Difficulty> DifficultyTable
        {
            get => new List<Difficulty>(difficultyTable);
        }

        public static Difficulty DifficultyData
        {
            get => difficultyTable[CDB.Difficulty].Copy();
        }

        public static int DifficultyCount
        {
            get => difficultyTable.Count;
        }

        public static Sprite GetSpritePanelType(PanelType panelType)
        {
            return spritesPanelType[(int)panelType];
        }
        public static Sprite GetSpriteDifficulty(DifficultyType difficulty)
        {
            return spritesDifficulty[(int)difficulty];
        }
    }

    /** ******************************
     * jsonデータ読み込み用クラス
     * ・JSpeedDataList
     * ・JDifficultyDataList
     ** ******************************/
    [Serializable]
    public sealed class JSpeedDataList
    {
        public List<float> speedTable;
    }

    [Serializable]
    public sealed class JDifficultyDataList
    {
        public List<Difficulty> difficultyTable;

        [Serializable]
        public sealed class Difficulty
        {
            public string name;
            public PanelType panel_type;
            public DifficultyType difficulty_type;
            public int player_life;
            public int panel_row;
            public int panel_col;
            public int panel_count_lower_limit;
            public int panel_count_upper_limit;
            public int speedlevel_start;
            public int speedlevel_max;
            public float seconds_speedup_start;
            public float add_speedup_seconds;

            internal Difficulty Copy()
            {
                return (Difficulty)this.MemberwiseClone();
            }
        }
    }
}
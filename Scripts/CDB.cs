using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UserDatabase;
using static UserDatabase.JDifficultyDataList;
using static FieldAdmin;
using static ChangeableDatabase.RecordDataBase.JRecordDataList;

namespace ChangeableDatabase
{
    /** ******************************
     * ChangeableDatabase (可変データベース)
     ** ******************************/
    public static class CDB
    {
        private static int difficulty;
        private static FieldInfo info;

        public static int Difficulty
        {
            get => difficulty;
            set
            {
                if (value >= UDB.DifficultyCount || value < 0)
                {
                    Debug.LogError($"DifficultyIndexOver {value} (max:{UDB.DifficultyCount - 1})");
                    return;
                }
                difficulty = value;
            }
        }

        public static FieldInfo FieldInfo
        {
            get => info;
            set => info = value;
        }
    }

    /** ******************************
     * SystemSettings (システム設定)
     ** ******************************/
    public static class SystemSettings
    {
        private static JSystemSettings settings;

        public static int VolumeBGM
        {
            get => settings.volume_bgm;
            set
            {
                settings.volume_bgm = Mathf.Clamp(value, 0, 100);
                Sound.Instance.RefreshVolume();
            }
        }
        public static int VolumeSE
        {
            get => settings.volume_se;
            set => settings.volume_se = Mathf.Clamp(value, 0, 100);
        }
        public static int VolumeCountDown
        {
            get => settings.volume_countdown;
            set => settings.volume_countdown = Mathf.Clamp(value, 0, 100);
        }
        public static int IndexFieldBGM
        {
            get => settings.index_field_bgm;
            set => settings.index_field_bgm = Mathf.Clamp(value, -1, Sound.Instance.FieldBGMCount - 1);
        }

        public static void Load()
        {
            try
            {
                using (var sr = new StreamReader($"{Application.dataPath}/SystemSettings.json"))
                {
                    settings = JsonUtility.FromJson<JSystemSettings>(sr.ReadToEnd());
                }
                if (settings == null)
                {
                    settings = new JSystemSettings();
                    settings.Reset();
                }
            }
            catch
            {
                Debug.Log("Create Settings");
                settings = new JSystemSettings();
                settings.Reset();
            }

            Save();
        }

        public static void Save()
        {
            if (settings == null)
            {
                Debug.LogError("SystemSettings is null.");
                return;
            }

            using (var sw = new StreamWriter($"{Application.dataPath}/SystemSettings.json"))
            {
                sw.Write(JsonUtility.ToJson(settings, true));
            }
            Debug.Log("Settings Saved!");
        }

        /** ******************************
         * システム設定読み込み/保存用クラス
         ** ******************************/
        [Serializable]
        private sealed class JSystemSettings
        {
            public int volume_bgm = 100;
            public int volume_se = 100;
            public int volume_countdown = 100;
            public int index_field_bgm = -1;

            internal void Reset()
            {
                volume_bgm = 100;
                volume_se = 100;
                volume_countdown = 100;
                index_field_bgm = -1;
            }
        }
    }

    /** ******************************
     * RecordDataBase (レコードデータベース)
     ** ******************************/
    public static class RecordDataBase
    {
        private static JRecordDataList recordData;

        public static void Load()
        {
            try
            {
                using (var sr = new StreamReader($"{Application.dataPath}/RecordData.json"))
                {
                    recordData = JsonUtility.FromJson<JRecordDataList>(sr.ReadToEnd());
                }
            }
            catch
            {
                Debug.Log("Create RecordData");
                recordData = new JRecordDataList();
                recordData.ResetAll();
            }

            if (!CheckRecordData())
            {
                Debug.Log("Reset RecordData");
                recordData.ResetAll();
            }

            Save();
        }

        public static void Save()
        {
            if (recordData == null)
            {
                Debug.LogError("RecordData is null.");
                return;
            }
            DateTime d = DateTime.Now;
            recordData.date = $"{d.Year}-{d.Month:00}-{d.Day:00}_" + //
                $"{d.Hour:00}:{d.Minute:00}:{d.Second:00}";

            foreach (Record r in recordData.recordsList)
            {
                r.parity = r.GetParity();
            }

            using (var sw = new StreamWriter($"{Application.dataPath}/RecordData.json"))
            {
                sw.Write(JsonUtility.ToJson(recordData, true));
            }
            Debug.Log("Data Saved!");
        }

        public static int AddRanking(string key, int score)
        {
            List<int> record = GetRecords(key);
            if (record == null)
            {
                return -1;
            }

            record.Add(score);
            record.Sort((n1, n2) => n2 - n1);

            while (record.Count > 5)
            {
                record.RemoveAt(record.Count - 1);
            }

            Save();
            return record.IndexOf(score) + 1;
        }

        public static List<int> GetRecords(string key)
        {
            foreach (Record r in recordData.recordsList)
            {
                if (r.name.Equals(key))
                {
                    return r.records;
                }
            }
            return null;
        }

        private static bool CheckRecordData()
        {
            foreach (Record r in recordData.recordsList)
            {
                if (r.records.Count != 5)
                {
                    return false;
                }
            }

            List<Record> sub = new List<Record>();
            foreach (Difficulty d in UDB.DifficultyTable)
            {
                for (int i = 0; i < recordData.recordsList.Count; i++)
                {
                    Record data = recordData.recordsList[i];
                    if (data.name.Equals(d.name))
                    {
                        if (data.parity == data.GetParity())
                        {
                            sub.Add(data);
                        }
                        else
                        {
                            Record r = new Record();
                            r.Reset(d.name);
                            sub.Add(r);
                        }
                        break;
                    }
                    if (i == recordData.recordsList.Count - 1)
                    {
                        Record r = new Record();
                        r.Reset(d.name);
                        sub.Add(r);
                    }
                }
            }
            recordData.recordsList = sub;

            return true;
        }

        /** ******************************
         * レコードデータ読み込み/保存用クラス
         ** ******************************/
        [Serializable]
        public sealed class JRecordDataList
        {
            public string date;
            public List<Record> recordsList = new List<Record>();

            [Serializable]
            public sealed class Record
            {
                public string name;
                public List<int> records = new List<int>();
                public int parity;

                internal void Reset(string name)
                {
                    this.name = name;
                    records.Clear();
                    for (int i = 0; i < 5; i++)
                    {
                        records.Add(-1);
                    }
                    parity = GetParity();
                }

                internal int GetParity()
                {
                    int p = 1;
                    int s = name.ToCharArray()[0] * name.ToCharArray()[name.Length - 1];
                    foreach (int n in records)
                    {
                        p = p * s * n * n * 73;
                    }
                    return p;
                }
            }

            internal void ResetAll()
            {
                recordsList.Clear();
                foreach (Difficulty data in UDB.DifficultyTable)
                {
                    Record r = new Record();
                    r.Reset(data.name);
                    recordsList.Add(r);
                }
            }
        }
    }

    /** ******************************
     * ArchiveDataBase (アーカイブデータベース)
     ** ******************************/
    public static class ArchiveDataBase
    {
        private static JArchiveData archiveData;

        public static JArchiveData ArchiveData
        {
            get => archiveData.Copy();
        }

        public static void Load()
        {
            try
            {
                using (var sr = new StreamReader($"{Application.dataPath}/ArchiveData.json"))
                {
                    archiveData = JsonUtility.FromJson<JArchiveData>(sr.ReadToEnd());
                    if (archiveData.parity != archiveData.GetParity())
                    {
                        archiveData = new JArchiveData();
                    }
                }
            }
            catch
            {
                Debug.Log("Create ArchiveData");
                archiveData = new JArchiveData();
            }

            Save();
        }

        public static void Save()
        {
            if (archiveData == null)
            {
                Debug.LogError("ArchiveData is null.");
                return;
            }
            DateTime d = DateTime.Now;
            archiveData.date = $"{d.Year}-{d.Month:00}-{d.Day:00}_" + //
                $"{d.Hour:00}:{d.Minute:00}:{d.Second:00}";

            archiveData.parity = archiveData.GetParity();

            using (var sw = new StreamWriter($"{Application.dataPath}/ArchiveData.json"))
            {
                sw.Write(JsonUtility.ToJson(archiveData, true));
            }
            Debug.Log("Archive Saved!");
        }

        public static void Refresh(FieldInfo info)
        {
            archiveData.total_playtime_sec += (int)info.totalSecond;
            archiveData.total_count += info.totalCount;
            archiveData.best_playtime_sec = (int)info.totalSecond > archiveData.best_playtime_sec ? //
                (int)info.totalSecond : archiveData.best_playtime_sec;
            archiveData.best_count = info.totalCount > archiveData.best_count ? //
                info.totalCount : archiveData.best_count;
            archiveData.best_level = info.speedLevel > archiveData.best_level ? //
                info.speedLevel : archiveData.best_level;

            Save();
        }

        public static bool GetIsFirstPlay()
        {
            return archiveData.total_playtime_sec == 0;
        }

        /** ******************************
         * アーカイブデータ読み込み/保存用クラス
         ** ******************************/
        [Serializable]
        public sealed class JArchiveData
        {
            public string date;
            public int parity;
            public int total_playtime_sec;
            public int total_count;
            public int best_playtime_sec;
            public int best_count;
            public int best_level;

            internal JArchiveData Copy()
            {
                return (JArchiveData)this.MemberwiseClone();
            }

            internal int GetParity()
            {
                int p = (total_playtime_sec + 11) * (total_count + 13) * //
                    (best_playtime_sec + 17) * (best_count + 19) * (best_level + 23);
                return p;
            }
        }
    }
}
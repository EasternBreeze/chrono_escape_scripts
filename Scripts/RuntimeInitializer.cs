using ChangeableDatabase;
using System.Collections.Generic;
using UnityEngine;


namespace Initialize
{
    public static class RuntimeInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            Debug.Log("Startup...");
            Application.targetFrameRate = 60;
            Screen.SetResolution(1280, 720, false, 60);
            SystemSettings.Load();
            RecordDataBase.Load();
            ArchiveDataBase.Load();

            List<Font> fonts = new List<Font>()
            {
                Resources.Load<Font>("System/CormorantSC-Semi"),
                Resources.Load<Font>("System/Amagro-bold"),
                Resources.Load<Font>("System/Caudex-Bold"),
                Resources.Load<Font>("System/KaiseiTokumin-Regular"),
                Resources.Load<Font>("System/TitilliumWeb-Regular")
            };

            Object.Instantiate(Resources.Load<GameObject>("Prefabs/Singleton_SceneChanger"));
            Object.Instantiate(Resources.Load<GameObject>("Prefabs/Singleton_SoundPlayer"));
        }
    }
}
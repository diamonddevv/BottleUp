using Godot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BottleUp.asset.script.Util
{
    internal class SaveManager // i copied this from labyrithmetine lol
    {
        public struct SaveData
        {
            public int BestRating;
        }

        public const string SAVE_LOCATION = "user://bottleup_save.dat";

        public static SaveData saveData;

        public static void Save()
        {
            using var file = FileAccess.Open(SAVE_LOCATION, FileAccess.ModeFlags.Write);

            string data = JsonConvert.SerializeObject(saveData, Formatting.Indented); // save using newtonsoft
            file.StoreString(data);

            file.Flush();
        }

        public static void Load()
        {
            InitialiseFile();

            using var file = FileAccess.Open(SAVE_LOCATION, FileAccess.ModeFlags.Read);

            string data = file.GetAsText();
            try
            {
                saveData = JsonConvert.DeserializeObject<SaveData>(data); // read using newtonsoft
            }
            catch
            {
                GD.Print("Failed to read SaveData; Reverting to original state.");
            }

            file.Flush();
        }

        public static void InitialiseFile()
        {
            if (FileAccess.FileExists(SAVE_LOCATION)) return;
            else
            {
                GD.Print($"SaveData File [Godot Path: {SAVE_LOCATION}] does not exist; Creating new SaveData");
                ResetSaveData();
            }
        }

        private static void ResetSaveData()
        {
            saveData = new SaveData()
            {
                BestRating = 0
            };

            Save();
        }

    }
}

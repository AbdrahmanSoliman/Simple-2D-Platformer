using System;
using System.IO;
using UnityEngine;

namespace Platformer.SaveLoad
{
    public static class SaveLoadManager
    {
        private static readonly string SaveFileName = "save.json";

        public static string SaveFilePath => Path.Combine(Application.persistentDataPath, SaveFileName);

        public static void Save(SaveData data)
        {
            try
            {
                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(SaveFilePath, json);
                Debug.Log($"[SaveLoadManager] Game saved to: {SaveFilePath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveLoadManager] Failed to save game: {ex.Message}");
            }
        }

        public static SaveData Load()
        {
            if (!HasSave())
            {
                Debug.LogWarning("[SaveLoadManager] No save file found.");
                return null;
            }

            try
            {
                string json = File.ReadAllText(SaveFilePath);
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                Debug.Log($"[SaveLoadManager] Game loaded successfully from: {SaveFilePath}");
                return data;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[SaveLoadManager] Failed to load save file (corrupted or unreadable): {ex.Message}");
                return null;
            }
        }

        public static bool HasSave()
        {
            return File.Exists(SaveFilePath);
        }

        public static void DeleteSave()
        {
            if (HasSave())
            {
                try
                {
                    File.Delete(SaveFilePath);
                    Debug.Log("[SaveLoadManager] Save file deleted.");
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[SaveLoadManager] Failed to delete save file: {ex.Message}");
                }
            }
        }
    }
}

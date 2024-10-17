using System.IO;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.CustomMenu
{
	[InitializeOnLoad]
	public static class CustomMenuPrefs
	{
		const string dataPath = "Assets/Editor/CustomMenu/Data/data.json";
		const string dirPath = "Assets/Editor/CustomMenu/Data";
		private static CustomMenuData dataObject;
		static CustomMenuPrefs ()
		{
			if (Directory.Exists(dirPath) == false)
			{
				Directory.CreateDirectory(dirPath);
			}
			dataObject = new();
			LoadData();
		}
		public static void Add(string key, string value)
		{
			dataObject.Add(key, value);
			SaveData();
		}
		public static void Add(string[] keys, string value)
		{
			int count = keys.Length;
			for (int i = 0 ; i < count ; i++)
			{
				dataObject.Add(keys[i], value);
			}
			SaveData();
		}
		public static bool TryGetValue(string key, out string value)
		{
			return dataObject.TryGetValue(key, out value);
		}
		public static void Remove(string key)
		{
			dataObject.Remove(key);
			SaveData();
		}
		public static void Remove(string[] keys)
		{
			foreach(string key in keys)
			{
				dataObject.Remove(key);
			}
			SaveData();
		}
		private static void LoadData()
		{
			if (File.Exists(dataPath) == false)
			{
				FileStream fs = new(dataPath, FileMode.Create, FileAccess.Write);
				dataObject = new();
				string data = JsonUtility.ToJson(dataObject);
				byte[] dataBytes = Encoding.UTF8.GetBytes(data);
				fs.Write(dataBytes);
				fs.Close();
			}
			else
			{
				string data = File.ReadAllText(dataPath);
				dataObject = JsonUtility.FromJson<CustomMenuData>(data);
			}
		}
		private static void SaveData()
		{
			string data = JsonUtility.ToJson(dataObject, true);
			File.WriteAllText(dataPath, data);
		}
	}
}

#endif
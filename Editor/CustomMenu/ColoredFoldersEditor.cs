
using UnityEngine;
using System.Linq;
using System.Collections.Generic;



#if UNITY_EDITOR
using UnityEditor;

namespace Assets.Editor.CustomMenu
{
	[InitializeOnLoad]
	static class ColoredFoldersEditor
	{
		private static Color column1Background = new(.2196f, .2196f, .2196f);
		private static Color column1BackgroundSelected = new(0.1725f, 0.3647f, 0.5294f);
		private static Color column2Background = new(.2f, .2f, .2f);
		private static Color column2BackgroundSelected = new(0.1725f, 0.3647f, 0.5294f);
		static ColoredFoldersEditor()
		{
			// 중복 등록을 방지하기 위한 방법.
			EditorApplication.projectWindowItemOnGUI -= OnGUI;
			EditorApplication.projectWindowItemOnGUI += OnGUI;
		}

		private static void OnGUI(string guid, Rect selectionRect)
		{
			bool isSelected = Selection.assetGUIDs.Contains<string>(guid) == true;
			if (CustomMenuPrefs.TryGetValue(guid, out string iconGUID) == false)
			{
				return;
			}

			Rect backgroundRect = GetBackgroundRect(selectionRect, out Color backgroundColor, isSelected);
			Rect folderRect = GetFolderRect(selectionRect);
			string iconPath = AssetDatabase.GUIDToAssetPath(iconGUID);
			Texture2D folderTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);

			if (folderTexture != null)
			{
				EditorGUI.DrawRect(backgroundRect, backgroundColor);
				GUI.DrawTexture(folderRect, folderTexture);
			}
		}

		public static void SetFolderTexture(string name)
		{
			if (TryGetSelectedFolderGUID(out string[] folderGUIDs) == false)
			{
				return ;
			}
			if (TryGetFolderIconTextureGUID(name, out string iconGUID) == false)
			{
				return ;
			}
			CustomMenuPrefs.Add(folderGUIDs, iconGUID);
		}

		public static void ResetFolderTexture()
		{
			if (TryGetSelectedFolderGUID(out string[] folderGUIDs) == false)
			{
				Debug.LogWarning("?");
				return ;
			}
			CustomMenuPrefs.Remove(folderGUIDs);
		}

		private static Rect GetBackgroundRect(Rect selectionRect, out Color backgroundColor, bool isSelected = false)
		{
			// Second Column, small scale
			if (selectionRect.x < 15)
			{
				backgroundColor = isSelected ? column2BackgroundSelected : column2Background;
				return new(selectionRect.x + 3, selectionRect.y, selectionRect.height, selectionRect.height);
			}
			// First Column
			else if (selectionRect.x >= 15 && selectionRect.height < 30)
			{
				backgroundColor = isSelected ? column1BackgroundSelected : column1Background;
				return new(selectionRect.x, selectionRect.y, selectionRect.height, selectionRect.height);
			}
			// Second Column, big scale
			else
			{
				backgroundColor = isSelected ? column2BackgroundSelected : column2Background;
				return new(selectionRect.x, selectionRect.y, selectionRect.width, selectionRect.width);
			}
		}
		private static Rect GetFolderRect(Rect selectionRect)
		{
			// Second Column, small scale
			if (selectionRect.x < 15)
			{
				return new(selectionRect.x + 3, selectionRect.y, selectionRect.height, selectionRect.height);
			}
			// First Column
			else if (selectionRect.x >= 15 && selectionRect.height < 30)
			{
				return new(selectionRect.x, selectionRect.y, selectionRect.height, selectionRect.height);
			}
			// Second Column, big scale
			else
			{
				return new(selectionRect.x, selectionRect.y, selectionRect.width, selectionRect.width);
			}
		}
		private static bool TryGetSelectedFolderGUID(out string[] folderGUIDs)
		{
			List<string> folderGUIDList = Selection.assetGUIDs.ToList<string>();
			int size = folderGUIDList.Count;
			for(int i = size - 1; i >= 0 ; i--)
			{
				string folderPath = AssetDatabase.GUIDToAssetPath(folderGUIDList[i]);
				if (AssetDatabase.IsValidFolder(folderPath) == false)
				{
					folderGUIDList.RemoveAt(i);
				}
			}
			folderGUIDs = folderGUIDList.ToArray();
			return folderGUIDs.Length > 0;
		}
		private static bool TryGetFolderIconTextureGUID(string iconName, out string iconGUID)
		{
			string iconPath = "Assets/Editor/CustomMenu/Icons/" + iconName + ".png";
			iconGUID = AssetDatabase.GUIDFromAssetPath(iconPath).ToString();

			return !(iconGUID == "" || iconGUID == "00000000000000000000000000000000");
		}
	}
}
#endif

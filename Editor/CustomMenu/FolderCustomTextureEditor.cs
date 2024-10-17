
// using UnityEngine;

// #if UNITY_EDITOR
// using UnityEditor;

// namespace Assets.Editor.CustomMenu
// {
// 	[InitializeOnLoad]
// 	static class FolderCustomTexture
// 	{
// 		static string selectedFolderGUID;
// 		static int controlID;
// 		static FolderCustomTexture()
// 		{
// 			EditorApplication.projectWindowItemOnGUI -= OnGUI;
// 			EditorApplication.projectWindowItemOnGUI += OnGUI;
// 		}

// 		private static void OnGUI(string guid, Rect selectionRect)
// 		{
// 			if (guid != selectedFolderGUID)
// 			{
// 				return ;
// 			}

// 			if (Event.current.commandName == "ObjectSelectorUpdated" && EditorGUIUtility.GetObjectPickerControlID() == controlID)
// 			{
// 				Object selectedObject = EditorGUIUtility.GetObjectPickerObject();

// 				if (selectedObject == null)
// 				{
// 					return ;
// 				}
// 				string folderTextureGUID = AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(selectedObject)).ToString();

// 				CustomMenuPrefs.Add(selectedFolderGUID, folderTextureGUID);
// 			}
// 		}

// 		public static void ChooseCustomIcon()
// 		{
// 			selectedFolderGUID = Selection.assetGUIDs[0];

// 			controlID = EditorGUIUtility.GetControlID(FocusType.Passive);
// 			EditorGUIUtility.ShowObjectPicker<Sprite>(null, false, "", controlID);
// 		}
// 	}
// }

// #endif

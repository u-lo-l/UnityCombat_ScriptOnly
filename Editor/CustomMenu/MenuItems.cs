
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

namespace Assets.Editor.CustomMenu
{
	static class MenuItems
	{
		enum IconColors {Black, Blue, Cyan, Green, Indigo, Lime, Magenta, Orange, Pink, Purple, Red, White, Yellow};
		const int Priority = 1000;

		[MenuItem("Assets/Custom Folder Color/Reset Icon", false, Priority)]
		static void ResetIcon()
		{
			ColoredFoldersEditor.ResetFolderTexture();
		}

		[MenuItem("Assets/Custom Folder Color/Black", false, Priority + 11)]
		static void Black()
		{
			ColoredFoldersEditor.SetFolderTexture("Black");
		}
		[MenuItem("Assets/Custom Folder Color/Blue", false, Priority + 12)]
		static void Blue()
		{
			ColoredFoldersEditor.SetFolderTexture("Blue");
		}
		[MenuItem("Assets/Custom Folder Color/Cyan", false, Priority + 13)]
		static void Cyan()
		{
			ColoredFoldersEditor.SetFolderTexture("Cyan");
		}
		[MenuItem("Assets/Custom Folder Color/Green", false, Priority + 14)]
		static void Green()
		{
			ColoredFoldersEditor.SetFolderTexture("Green");
		}
		[MenuItem("Assets/Custom Folder Color/Indigo", false, Priority + 15)]
		static void Indigo()
		{
			ColoredFoldersEditor.SetFolderTexture("Indigo");
		}
		[MenuItem("Assets/Custom Folder Color/Lime", false, Priority + 16)]
		static void Lime()
		{
			ColoredFoldersEditor.SetFolderTexture("Lime");
		}
		[MenuItem("Assets/Custom Folder Color/Magenta", false, Priority + 17)]
		static void Magenta()
		{
			ColoredFoldersEditor.SetFolderTexture("Magenta");
		}
		[MenuItem("Assets/Custom Folder Color/Orange", false, Priority + 18)]
		static void Orange()
		{
			ColoredFoldersEditor.SetFolderTexture("Orange");
		}
		[MenuItem("Assets/Custom Folder Color/Pink", false, Priority + 19)]
		static void Pink()
		{
			ColoredFoldersEditor.SetFolderTexture("Pink");
		}
		[MenuItem("Assets/Custom Folder Color/Purple", false, Priority + 20)]
		static void Purple()
		{
			ColoredFoldersEditor.SetFolderTexture("Purple");
		}
		[MenuItem("Assets/Custom Folder Color/Red", false, Priority + 21)]
		static void Red()
		{
			ColoredFoldersEditor.SetFolderTexture("Red");
		}
		[MenuItem("Assets/Custom Folder Color/White", false, Priority + 22)]
		static void White()
		{
			ColoredFoldersEditor.SetFolderTexture("White");
		}
		[MenuItem("Assets/Custom Folder Color/Yellow", false, Priority + 23)]
		static void Yellow()
		{
			ColoredFoldersEditor.SetFolderTexture("Yellow");
		}
		
		[MenuItem("Assets/Custom Folder Color/Reset Icon", true)] 
		[MenuItem("Assets/Custom Folder Color/Black", true)]
		[MenuItem("Assets/Custom Folder Color/Blue", true)]
		[MenuItem("Assets/Custom Folder Color/Cyan", true)]
		[MenuItem("Assets/Custom Folder Color/Green", true)]
		[MenuItem("Assets/Custom Folder Color/Indigo", true)]
		[MenuItem("Assets/Custom Folder Color/Lime", true)]
		[MenuItem("Assets/Custom Folder Color/Magenta", true)]
		[MenuItem("Assets/Custom Folder Color/Orange", true)]
		[MenuItem("Assets/Custom Folder Color/Pink", true)]
		[MenuItem("Assets/Custom Folder Color/Purple", true)]
		[MenuItem("Assets/Custom Folder Color/Red", true)]
		[MenuItem("Assets/Custom Folder Color/White", true)]
		[MenuItem("Assets/Custom Folder Color/Yellow", true)]
		static bool ValidateFolder()
		{
			Object selectedObject = Selection.activeObject;
			if (selectedObject == null)
			{
				return false;
			}
			string objectPath = AssetDatabase.GetAssetPath(selectedObject);
			return AssetDatabase.IsValidFolder(objectPath);
		}
	}
}
#endif

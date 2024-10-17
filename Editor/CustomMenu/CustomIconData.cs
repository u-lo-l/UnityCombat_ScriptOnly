using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Editor.CustomMenu
{
	[Serializable]
	class CustomMenuData
	{
		[SerializeField] private int size;
		[SerializeField] private List<string> keys;
		[SerializeField] private List<string> values;

		public CustomMenuData()
		{
			size = 0;
			keys = new();
			values = new();
		}

		internal void Add(string key, string value)
		{
			for(int i = 0 ; i < size ; i++)
			{
				if (keys[i] == key)
				{
					values[i] = value;
					return ;
				}
			}
			keys.Add(key);
			values.Add(value);
			size++;
		}

		internal bool TryGetValue(string key, out string value)
		{
			for(int i = 0 ; i < size ; i++)
			{
				if (keys[i] == key)
				{
					value = values[i];
					return true;
				}
			}
			value = null;
			return false;
		}

		internal void Remove(string key)
		{
			for(int i = 0 ; i < size ; i++)
			{
				if (keys[i] == key)
				{
					keys.RemoveAt(i);
					values.RemoveAt(i);
					size--;
					return;
				}
			}
		}
	};
}
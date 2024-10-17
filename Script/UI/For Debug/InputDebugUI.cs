using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputDebugUI : MonoBehaviour
{
	private enum Key { ESCAPE, NUM1, NUM2, NUM3, NUM4, NUM5, NUM6, NUM7, Q, W, E, R, T, Y, A, S, D, F, G, H, Z, X, C, V, B, N, LEFTSHIFT, LEFTCTRL, LEFTALT, SPACE, MAX };
	private enum MouseButton{ LEFT, MIDDLE, RIGHT };
	[SerializeField ] private Transform keyboardTF;
	private Dictionary<Key, Image> keyboard;
	[SerializeField] private Transform mouseTF;
	private Dictionary<MouseButton, Image> mouse;
	private void Awake()
	{
		if (keyboardTF != null)
		{
			keyboard = new();
			for (int i = 0 ; i < keyboardTF.childCount ; i++)
			{
				GameObject keyObject = keyboardTF.GetChild(i).gameObject;
				Key key = (Key)Enum.Parse(typeof(InputDebugUI.Key), keyObject.name);
				keyboard.Add(key, keyObject.GetComponent<Image>());
			}
		}
		if (mouseTF != null)
		{
			mouse = new();
			for (int i = 0 ; i < mouseTF.childCount ; i++)
			{
				GameObject mouseObj = mouseTF.GetChild(i).gameObject;
				MouseButton button = (MouseButton)Enum.Parse(typeof(MouseButton), mouseObj.name);
				mouse.Add(button, mouseObj.GetComponent<Image>());
			}
		}
		
	}
	private void Start()
	{
		
	}
	private void Update()
	{
		if (keyboard != null)
		{
			GetKeyboardInput();
		}
		if (mouse != null)
		{
			GetMouseInput();
		}
	}

	Color pressedColor = new Color(1f,0.5f,0.5f,1);
	Color releasedColor = new Color(1,1,1,1);
	private void pressed(Image img)
	{
		img.color = pressedColor;
	}
	private void released(Image img)
	{
		img.color = releasedColor;
	}

	private void GetKeyboardInput()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			pressed(keyboard[Key.ESCAPE]);
		else if (Input.GetKeyDown(KeyCode.Alpha1))
			pressed(keyboard[Key.NUM1]);
		else if (Input.GetKeyDown(KeyCode.Alpha2))
			pressed(keyboard[Key.NUM2]);
		else if (Input.GetKeyDown(KeyCode.Alpha3))
			pressed(keyboard[Key.NUM3]);
		else if (Input.GetKeyDown(KeyCode.Alpha4))
			pressed(keyboard[Key.NUM4]);
		else if (Input.GetKeyDown(KeyCode.Alpha5))
			pressed(keyboard[Key.NUM5]);
		else if (Input.GetKeyDown(KeyCode.Alpha6))
			pressed(keyboard[Key.NUM6]);
		else if (Input.GetKeyDown(KeyCode.Alpha7))
			pressed(keyboard[Key.NUM7]);
		else if (Input.GetKeyDown(KeyCode.Q))
			pressed(keyboard[Key.Q]);
		else if (Input.GetKeyDown(KeyCode.W))
			pressed(keyboard[Key.W]);
		else if (Input.GetKeyDown(KeyCode.E))
			pressed(keyboard[Key.E]);
		else if (Input.GetKeyDown(KeyCode.R))
			pressed(keyboard[Key.R]);
		else if (Input.GetKeyDown(KeyCode.T))
			pressed(keyboard[Key.T]);
		else if (Input.GetKeyDown(KeyCode.Y))
			pressed(keyboard[Key.Y]);
		else if (Input.GetKeyDown(KeyCode.A))
			pressed(keyboard[Key.A]);
		else if (Input.GetKeyDown(KeyCode.S))
			pressed(keyboard[Key.S]);
		else if (Input.GetKeyDown(KeyCode.D))
			pressed(keyboard[Key.D]);
		else if (Input.GetKeyDown(KeyCode.F))
			pressed(keyboard[Key.F]);
		else if (Input.GetKeyDown(KeyCode.G))
			pressed(keyboard[Key.G]);
		else if (Input.GetKeyDown(KeyCode.H))
			pressed(keyboard[Key.H]);
		else if (Input.GetKeyDown(KeyCode.Z))
			pressed(keyboard[Key.Z]);
		else if (Input.GetKeyDown(KeyCode.X))
			pressed(keyboard[Key.X]);
		else if (Input.GetKeyDown(KeyCode.C))
			pressed(keyboard[Key.C]);
		else if (Input.GetKeyDown(KeyCode.V))
			pressed(keyboard[Key.V]);
		else if (Input.GetKeyDown(KeyCode.B))
			pressed(keyboard[Key.B]);
		else if (Input.GetKeyDown(KeyCode.LeftAlt))
			pressed(keyboard[Key.LEFTALT]);
		else if (Input.GetKeyDown(KeyCode.LeftControl))
			pressed(keyboard[Key.LEFTCTRL]);
		else if (Input.GetKeyDown(KeyCode.Space))
			pressed(keyboard[Key.SPACE]);
		else if (Input.GetKeyDown(KeyCode.LeftShift))
			pressed(keyboard[Key.LEFTSHIFT]);

		if (Input.GetKeyUp(KeyCode.Escape))
			released(keyboard[Key.ESCAPE]);
		else if (Input.GetKeyUp(KeyCode.Alpha1))
			released(keyboard[Key.NUM1]);
		else if (Input.GetKeyUp(KeyCode.Alpha2))
			released(keyboard[Key.NUM2]);
		else if (Input.GetKeyUp(KeyCode.Alpha3))
			released(keyboard[Key.NUM3]);
		else if (Input.GetKeyUp(KeyCode.Alpha4))
			released(keyboard[Key.NUM4]);
		else if (Input.GetKeyUp(KeyCode.Alpha5))
			released(keyboard[Key.NUM5]);
		else if (Input.GetKeyUp(KeyCode.Alpha6))
			released(keyboard[Key.NUM6]);
		else if (Input.GetKeyUp(KeyCode.Alpha7))
			released(keyboard[Key.NUM7]);
		else if (Input.GetKeyUp(KeyCode.Q))
			released(keyboard[Key.Q]);
		else if (Input.GetKeyUp(KeyCode.W))
			released(keyboard[Key.W]);
		else if (Input.GetKeyUp(KeyCode.E))
			released(keyboard[Key.E]);
		else if (Input.GetKeyUp(KeyCode.R))
			released(keyboard[Key.R]);
		else if (Input.GetKeyUp(KeyCode.T))
			released(keyboard[Key.T]);
		else if (Input.GetKeyUp(KeyCode.Y))
			released(keyboard[Key.Y]);
		else if (Input.GetKeyUp(KeyCode.A))
			released(keyboard[Key.A]);
		else if (Input.GetKeyUp(KeyCode.S))
			released(keyboard[Key.S]);
		else if (Input.GetKeyUp(KeyCode.D))
			released(keyboard[Key.D]);
		else if (Input.GetKeyUp(KeyCode.F))
			released(keyboard[Key.F]);
		else if (Input.GetKeyUp(KeyCode.G))
			released(keyboard[Key.G]);
		else if (Input.GetKeyUp(KeyCode.H))
			released(keyboard[Key.H]);
		else if (Input.GetKeyUp(KeyCode.Z))
			released(keyboard[Key.Z]);
		else if (Input.GetKeyUp(KeyCode.X))
			released(keyboard[Key.X]);
		else if (Input.GetKeyUp(KeyCode.C))
			released(keyboard[Key.C]);
		else if (Input.GetKeyUp(KeyCode.V))
			released(keyboard[Key.V]);
		else if (Input.GetKeyUp(KeyCode.B))
			released(keyboard[Key.B]);
		else if (Input.GetKeyUp(KeyCode.LeftAlt))
			released(keyboard[Key.LEFTALT]);
		else if (Input.GetKeyUp(KeyCode.LeftControl))
			released(keyboard[Key.LEFTCTRL]);
		else if (Input.GetKeyUp(KeyCode.Space))
			released(keyboard[Key.SPACE]);
		else if (Input.GetKeyUp(KeyCode.LeftShift))
			released(keyboard[Key.LEFTSHIFT]);
	}
	private void GetMouseInput()
	{
		if (Input.GetMouseButtonDown(0))
			pressed(mouse[MouseButton.LEFT]);
		else if (Input.GetMouseButtonDown(1))
			pressed(mouse[MouseButton.RIGHT]);
		else if (Input.GetMouseButtonDown(2))
			pressed(mouse[MouseButton.MIDDLE]);
			
		else if (Input.GetMouseButtonUp(0))
			released(mouse[MouseButton.LEFT]);
		else if (Input.GetMouseButtonUp(1))
			released(mouse[MouseButton.RIGHT]);
		else if (Input.GetMouseButtonUp(2))
			released(mouse[MouseButton.MIDDLE]);
	}
}

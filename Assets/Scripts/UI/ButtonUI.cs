/**
 * ButtonUI.cs
 * 
 * Description:
 *  
 * 
 * Author: 
 *  Pierre-Luc Poirier
 * 
 * © 2020 Pierre-Luc Poirier. All Rights Reserved.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonUI : MonoBehaviour
{
	#region Members and Properties
	// constants

	// public

	// protected

	// private
	[SerializeField] private TextMeshProUGUI _text = null;
	[SerializeField] private Color _textDisabledColor = Color.gray;

	private Button _button;
	private Color _textNormalColor = Color.white;

	// properties
	#endregion

	#region Unity API
	private void Awake()
	{
		_button = GetComponentInChildren<Button>();
		if (_text != null)
		{
			_textNormalColor = _text.color;
		}
	}
	#endregion

	#region Public Functions
	public void SetInteractable(bool enabled)
	{
		_button.interactable = enabled;

		if (_text != null)
		{
			_text.color = enabled ? _textNormalColor : _textDisabledColor;
		}
	}
	#endregion

	#region Protected Functions
	#endregion

	#region Private Functions
	#endregion
}

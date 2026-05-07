using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollUVs : MonoBehaviour
{
	[SerializeField] private Vector2 _Speed;

	private Renderer _renderer;
	private Vector2 _offset;

	private void Awake()
	{
		_renderer = GetComponentInChildren<Renderer>();
	}

	private void Update()
	{
		_offset += _Speed * Time.deltaTime;
		_renderer.material.SetTextureOffset("_MainTex", _offset);
	}
}

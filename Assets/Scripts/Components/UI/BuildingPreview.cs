using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pseudo;

public class BuildingPreview : PComponent
{
	[EntityRequires(typeof(GrowerBase))]
	public PEntity Building;
	public Color Valid = Color.green;
	public Color Invalid = Color.red;
	public SpriteRenderer Renderer;

	bool isValid;
	Color currentColor;

	void Awake()
	{
		CachedTransform.localScale = Building.Transform.localScale * LevelManager.Instance.CurrentLevel.Modifier;
	}

	void Update()
	{
		UpdatePosition();
		UpdateColor();
	}

	void UpdatePosition()
	{
		Entity.Transform.localPosition = Camera.main.GetMouseWorldPosition().Round();
		Point2 size = Building.Transform.localScale * LevelManager.Instance.CurrentLevel.Modifier;
		isValid = BuildingManager.Instance.IsValidPosition(CachedTransform.localPosition, size, 0);
	}

	void UpdateColor()
	{
		if (isValid)
			currentColor = Valid;
		else
			currentColor = Invalid;

		Renderer.color = currentColor;
	}

	public void OnDeselected()
	{
		if (isValid)
			BuildingManager.Instance.CreateBuilding(Building, CachedTransform.localPosition);

		Entity.GameObject.Destroy();
	}
}
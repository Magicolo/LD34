using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pseudo;

public class GameCamera : PComponent
{
	public float ZoomSpeed = 3f;
	public TimeComponent Time;

	void Update()
	{
		UpdatePosition();
	}

	void UpdatePosition()
	{
		var currentLevel = LevelManager.Instance.CurrentLevel;

		if (currentLevel == null)
			return;

		var targetPosition = new Vector3(0f, currentLevel.Width / 5f, -currentLevel.Width);
		Entity.Transform.TranslateLocalTowards(targetPosition, ZoomSpeed * Time.DeltaTime, Axes.YZ);
	}
}

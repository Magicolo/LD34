using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pseudo;

public class LevelFloor : PComponent
{
	public float ScaleSpeed = 3f;
	public TimeComponent Time;

	void Update()
	{
		var currentLevel = LevelManager.Instance.CurrentLevel;

		if (currentLevel == null)
			return;

		Entity.Transform.ScaleLocalTowards(new Vector2(currentLevel.Width, currentLevel.Width / 10f), ScaleSpeed * Time.DeltaTime, Axes.XY);
	}
}

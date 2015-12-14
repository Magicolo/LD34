using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pseudo;

public class LevelManager : Singleton<LevelManager>
{
	public Level[] Levels;

	public Level CurrentLevel { get; private set; }

	protected override void Awake()
	{
		base.Awake();

		for (int i = 0; i < Levels.Length; i++)
		{
			var level = Levels[i];

			level.Index = i;
			level.CachedGameObject.SetActive(false);
		}
	}

	protected override void Start()
	{
		base.Start();

		NextLevel();
	}

	public void NextLevel()
	{
		if (CurrentLevel == null && Levels.Length > 0)
			SwitchLevel(0);
		else if (Levels.Length > CurrentLevel.Index + 1)
			SwitchLevel(Levels[CurrentLevel.Index + 1]);
	}

	protected void SwitchLevel(int levelIndex)
	{
		SwitchLevel(Levels[levelIndex]);
	}

	protected void SwitchLevel(Level level)
	{
		if (CurrentLevel != null)
			CurrentLevel.CachedGameObject.SetActive(false);

		CurrentLevel = level;
		CurrentLevel.CachedGameObject.SetActive(true);
		EntityManager.BroadcastMessage(EntityMessages.OnLevelChanged, CurrentLevel);
	}
}

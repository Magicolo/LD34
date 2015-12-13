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

	int currentLevelIndex = -1;
	IEntityGroup managerGroup = EntityManager.GetEntityGroup(EntityGroups.Manager);

	protected override void Awake()
	{
		base.Awake();

		for (int i = 0; i < Levels.Length; i++)
			Levels[i].CachedGameObject.SetActive(false);
	}

	protected override void Start()
	{
		base.Start();

		NextLevel();
	}

	public void NextLevel()
	{
		if (Levels.Length > currentLevelIndex + 1)
		{
			currentLevelIndex++;
			SwitchLevel(Levels[currentLevelIndex]);
		}
	}

	protected void SwitchLevel(Level level)
	{
		if (CurrentLevel != null)
			CurrentLevel.CachedGameObject.SetActive(false);

		CurrentLevel = level;
		CurrentLevel.CachedGameObject.SetActive(true);
		managerGroup.BroadcastMessage(EntityMessages.OnLevelChanged, CurrentLevel);
	}
}

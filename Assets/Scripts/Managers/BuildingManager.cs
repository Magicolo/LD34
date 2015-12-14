using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pseudo;

public class BuildingManager : Singleton<BuildingManager>
{
	public PEntity Building;

	Level CurrentLevel { get { return LevelManager.Instance.CurrentLevel; } }
	int CurrentModifier { get { return CurrentLevel.Modifier; } }

	readonly IEntityGroup buildingGroup = EntityManager.GetEntityGroup(EntityGroups.Building).Filter(typeof(GrowerBase));
	readonly List<IEntity> toUpdate = new List<IEntity>();
	readonly Dictionary<int, IEntity> idBuildings = new Dictionary<int, IEntity>();

	void Update()
	{
		UpdatePosition();
		UpdateBuildings();
	}

	void UpdatePosition()
	{
		//CachedTransform.OscillateLocalPosition(0.75f, currentLevel.Width * 0.1f, 0f, TimeManager.World.Time, Axes.Y);
	}

	void UpdateBuildings()
	{
		for (int i = 0; i < toUpdate.Count; i++)
		{
			var building = toUpdate[i];
			var grower = building.GetComponent<GrowerBase>();

			// Growth
			if (grower.ShouldGrow())
			{
				if (Grow(grower))
					building.SendMessage(EntityMessages.OnGrow);
				else
					toUpdate.RemoveAt(i--);
			}

			// Motion
			if (grower.ShouldMove())
			{
				if (Move(grower))
					building.SendMessage(EntityMessages.OnMove);
			}
		}
	}

	public IEntity CreateBuilding(PEntity building, Point2 position)
	{
		var entity = Instantiate(building);
		var grower = entity.GetComponent<GrowerBase>();

		idBuildings[grower.Id] = entity;
		entity.Transform.parent = CachedTransform;
		entity.Transform.localPosition = position;
		grower.CurrentPosition = position;
		grower.CurrentSize = grower.Size * CurrentModifier;

		if (!grower.ShouldMove() || !Move(grower))
			SetIds(grower);

		if (grower.ShouldManage())
			toUpdate.Add(entity);

		return entity;
	}

	public IEntity GetBuilding(int id)
	{
		IEntity building;
		idBuildings.TryGetValue(id, out building);

		return building;
	}

	public IEntity GetBuilding(Point2 position)
	{
		return GetBuilding(GetId(position));
	}

	public void DestroyBuilding(IEntity building)
	{
		var grower = building.GetComponent<GrowerBase>();
		SetIds(grower.CurrentPosition, grower.CurrentSize, 0);
		building.GameObject.Destroy();
	}

	public void DestroyAllBuildings()
	{
		for (int i = 0; i < buildingGroup.Entities.Count; i++)
			DestroyBuilding(buildingGroup.Entities[i]);
	}

	public bool CanCreateBuilding(PEntity building, Point2 position)
	{
		return CanGrow(position, building.GetComponent<GrowerBase>().Size, 0);
	}

	public bool Grow(GrowerBase grower)
	{
		var growth = grower.GetGrowth() * CurrentModifier;

		if (CanGrow(grower, growth))
		{
			SetIds(grower.CurrentPosition, grower.CurrentSize, 0);
			grower.CurrentSize += growth;
			SetIds(grower);
		}
		else
			return false;

		return true;
	}

	public bool Move(GrowerBase grower)
	{
		var position = GetLowerPosition(grower);

		if (position != grower.CurrentPosition)
		{
			SetIds(grower.CurrentPosition, grower.CurrentSize, 0);
			grower.CurrentPosition = position;
			SetIds(grower);

			return true;
		}

		return false;
	}

	public bool CanGrow(GrowerBase grower, Point2 amount)
	{
		return CanGrow(grower.CurrentPosition, grower.CurrentSize + amount, grower.Id);
	}

	public bool CanGrow(Point2 position, Point2 targetSize, int id)
	{
		var bottomLeft = GetBottomLeft(position, targetSize);

		for (int x = 0; x < targetSize.X; x++)
		{
			for (int y = 0; y < targetSize.Y; y++)
			{
				if (!CanGrow(bottomLeft + new Point2(x, y), id))
					return false;
			}
		}

		return true;
	}

	public bool CanGrow(Point2 targetPosition, int id)
	{
		if (!CurrentLevel.IsWithinBounds(targetPosition))
			return false;

		int currentId = CurrentLevel.GetId(targetPosition);

		return currentId == 0 || currentId == id;
	}

	public Point2 GetLowerPosition(GrowerBase grower)
	{
		return GetLowerPosition(grower.CurrentPosition, grower.CurrentSize, grower.Id);
	}

	public Point2 GetLowerPosition(Point2 position, Point2 size, int id)
	{
		if (position.Y <= 0)
			return position;

		bool isValid = true;

		do
		{
			var bottomLeft = GetBottomLeft(position + Point2.Down, size);

			for (int x = 0; x < size.X; x++)
			{
				var currentPosition = bottomLeft + new Point2(x, 0);

				if (CurrentLevel.IsWithinBounds(currentPosition))
				{
					int currentId = CurrentLevel.GetId(currentPosition);
					isValid &= currentId == 0 || currentId == id;
				}
				else
				{
					isValid = false;
					break;
				}
			}

			if (isValid)
				position += Point2.Down;
		}
		while (isValid && position.Y > 0);

		return position;
	}

	public bool IsValidPosition(Point2 position, Point2 size, int id)
	{
		var currentLevel = LevelManager.Instance.CurrentLevel;

		if (currentLevel == null || !currentLevel.IsWithinBounds(position))
			return false;

		var bottomLeft = GetBottomLeft(position, size);

		for (int x = 0; x < size.X; x++)
		{
			for (int y = 0; y < size.Y; y++)
			{
				var currentPosition = bottomLeft + new Point2(x, y);

				if (currentLevel.IsWithinBounds(currentPosition))
				{
					int currentId = currentLevel.GetId(currentPosition);

					if (currentId != 0 && currentId != id)
						return false;
				}
				else
					return false;
			}
		}

		return true;
	}

	public int GetId(Point2 position)
	{
		if (CurrentLevel.IsWithinBounds(position))
			return CurrentLevel.GetId(position);

		return 0;
	}

	public void SetIds(GrowerBase grower)
	{
		SetIds(grower.CurrentPosition, grower.CurrentSize, grower.Id);
	}

	public void SetIds(Point2 position, Point2 size, int id)
	{
		var bottomLeft = new Point2(position.X - Mathf.FloorToInt((size.X) * 0.5f), position.Y);

		for (int x = 0; x < size.X; x++)
		{
			for (int y = 0; y < size.Y; y++)
				SetId(bottomLeft + new Point2(x, y), id);
		}
	}

	public void SetId(Point2 position, int id)
	{
		if (CurrentLevel.IsWithinBounds(position))
			CurrentLevel.SetId(position, id);
	}

	Point2 GetBottomLeft(Point2 position, Point2 size)
	{
		return new Point2(position.X - Mathf.FloorToInt((size.X) * 0.5f), position.Y);
	}

	void OnLevelChanged(Level level)
	{
		for (int i = 0; i < buildingGroup.Entities.Count; i++)
		{
			var building = buildingGroup.Entities[i];
			var grower = building.GetComponent<GrowerBase>();

			if (!toUpdate.Contains(building))
				toUpdate.Add(building);

			SetIds(grower);
		}
	}
}

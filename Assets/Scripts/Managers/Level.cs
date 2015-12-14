using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pseudo;

public class Level : PComponent
{
	public int Width;
	public int TargetHeight;
	public int Modifier = 1;
	public bool Draw;

	public int Index { get; set; }

	int[,] map;
	int highest;
	int height;

	void Awake()
	{
		height = TargetHeight + 50;
		map = new int[Width, height];
	}

	void LateUpdate()
	{
		if (highest >= TargetHeight)
			LevelManager.Instance.NextLevel();
	}

	void OnDrawGizmos()
	{
		if (!Draw || map == null)
			return;

		for (int x = 0; x < Width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				var position = new Point2(x, y);
				var id = map.Get(position);

				Gizmos.color = id == 0 ? Color.gray : Color.blue;
				Gizmos.DrawCube(new Vector3(x, y, 0f), Vector3.one);
			}
		}
	}

	public int GetId(Point2 position)
	{
		return map.Get(GetAdjustedPosition(position));
	}

	public void SetId(Point2 position, int id)
	{
		map.Set(GetAdjustedPosition(position), id);

		if (id > 0)
			highest = Mathf.Max(highest, position.Y + 1);
	}

	public bool IsWithinBounds(Point2 position)
	{
		position = GetAdjustedPosition(position);

		return position.X >= 0 && position.X < Width && position.Y >= 0 && position.Y < height;
	}

	Point2 GetAdjustedPosition(Point2 position)
	{
		return position + new Point2(Mathf.FloorToInt(Width * 0.5f), 0);
	}
}

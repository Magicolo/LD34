using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pseudo;

public class CactusGrower : SequenceIntervalGrower
{
	public bool Moveable = true;
	public PEntity Branch;

	bool stopGrowth;

	protected override void OnGrow()
	{
		base.OnGrow();

		var position = CurrentPosition + GetRandomPosition();

		if (sequenceIndex == 0 && BuildingManager.Instance.CanCreateBuilding(Branch, position))
		{
			BuildingManager.Instance.CreateBuilding(Branch, position);
			stopGrowth = true;
		}
	}

	public override bool ShouldGrow()
	{
		return counter > Interval && !stopGrowth;
	}

	public override bool ShouldMove()
	{
		return Moveable;
	}

	Point2 GetRandomPosition()
	{
		var randomValue = PRandom.Generator.NextDouble();
		var randomHeight = Mathf.FloorToInt(PRandom.Range(0.75f, 1f) * CurrentSize.Y);

		if (randomValue >= 0.5)
			return new Point2(Mathf.FloorToInt(CurrentSize.X * 0.5f + 1), randomHeight);
		else
			return new Point2(-Mathf.FloorToInt(CurrentSize.X * 0.5f + 1), randomHeight);
	}
}

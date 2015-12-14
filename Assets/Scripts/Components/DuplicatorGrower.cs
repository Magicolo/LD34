using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pseudo;

public class DuplicatorGrower : GrowerBase
{
	public float Interval = 3f;
	[EntityRequires(typeof(DuplicatorSlave))]
	public PEntity Slave;

	public Point2 Center
	{
		get { return CurrentPosition + new Point2(0, Mathf.FloorToInt(CurrentSize.Y * 0.5f)); }
	}

	readonly List<IEntity> slaves = new List<IEntity>();
	float counter;

	protected override void Update()
	{
		base.Update();

		counter += Time.DeltaTime;
	}

	public override bool ShouldGrow()
	{
		return counter > Interval;
	}

	public override bool ShouldMove()
	{
		return true;
	}

	public override Point2 GetGrowth()
	{
		return Point2.Zero;
	}

	public IEntity CreateSlave(Point2 position, Point2 direction, int id)
	{
		IEntity slave = null;
		var targetPosition = position + direction;

		if (BuildingManager.Instance.CanGrow(targetPosition, CurrentSize, id))
		{
			slave = BuildingManager.Instance.CreateBuilding(Slave, targetPosition);
			slave.GetComponent<DuplicatorSlave>().Initialize(this);
		}

		return slave;
	}

	void OnGrow()
	{
		counter = 0;

		for (int i = 0; i < slaves.Count; i++)
		{
			if (slaves[i].GetComponent<DuplicatorSlave>().Duplicate())
				return;
		}

		var child = CreateSlave(Center, GetRandomDirection() * CurrentSize, Id);

		if (child != null)
			slaves.Add(child);
	}

	public static Point2 GetRandomDirection()
	{
		Point2 direction;
		var randomValue = PRandom.Generator.NextDouble();

		if (randomValue >= 0.75)
			direction = Point2.Left;
		else if (randomValue > 0.5)
			direction = Point2.Right;
		else if (randomValue > 0.25)
			direction = Point2.Up;
		else
			direction = Point2.Down;

		return direction;
	}
}

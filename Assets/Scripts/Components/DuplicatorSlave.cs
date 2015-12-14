using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pseudo;

public class DuplicatorSlave : GrowerBase
{
	DuplicatorGrower master;
	readonly List<IEntity> slaves = new List<IEntity>();

	public void Initialize(DuplicatorGrower master)
	{
		this.master = master;
	}

	public bool Duplicate()
	{
		for (int i = 0; i < slaves.Count; i++)
		{
			if (slaves[i].GetComponent<DuplicatorSlave>().Duplicate())
				return true;
		}

		var child = master.CreateSlave(CurrentPosition, DuplicatorGrower.GetRandomDirection() * CurrentSize, Id);

		if (child == null)
			return false;
		else
		{
			slaves.Add(child);
			return true;
		}
	}

	public override bool ShouldGrow()
	{
		return false;
	}

	public override bool ShouldMove()
	{
		return false;
	}

	public override bool ShouldManage()
	{
		return false;
	}

	public override Point2 GetGrowth()
	{
		return Point2.Zero;
	}
}

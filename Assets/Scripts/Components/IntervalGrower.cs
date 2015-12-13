using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pseudo;

public class IntervalGrower : GrowerBase
{
	public float Interval = 3f;
	[Tooltip("X value must be even.")]
	public Point2 Amount = Point2.One;

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

	public override Point2 GetGrowth()
	{
		return Amount;
	}

	void OnGrow()
	{
		counter = 0f;
	}
}

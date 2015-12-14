using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pseudo;

public class SequenceIntervalGrower : GrowerBase
{
	public float Interval = 2f;
	public Point2[] Sequence;

	protected float counter;
	protected int sequenceIndex;

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
		return Sequence[sequenceIndex];
	}

	protected virtual void OnGrow()
	{
		counter = 0;
		sequenceIndex = (sequenceIndex + 1) % Sequence.Length;
	}
}

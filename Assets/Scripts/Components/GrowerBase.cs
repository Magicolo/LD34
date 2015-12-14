using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pseudo;

public abstract class GrowerBase : PComponent
{
	static int idCounter;

	public float MoveSpeed = 5f;
	public float GrowSpeed = 5f;
	public Point2 Size = Point2.One;
	public TimeComponent Time;

	public int Id
	{
		get { return id; }
	}
	public Point2 CurrentPosition { get; set; }
	public Point2 CurrentSize { get; set; }
	public Point2 Left
	{
		get { return CurrentPosition - new Point2(Mathf.FloorToInt(CurrentSize.X * 0.5f), 0); }
	}
	public Point2 Right
	{
		get { return CurrentPosition + new Point2(Mathf.FloorToInt(CurrentSize.X * 0.5f), 0); }
	}
	public Point2 Top
	{
		get { return CurrentPosition + new Point2(CurrentSize.Y - 1, 0); }
	}

	int id = ++idCounter;

	[Button]
	public bool destroyBuilding;
	public void DestroyBuilding()
	{
		BuildingManager.Instance.DestroyBuilding(Entity);
	}

	protected virtual void Awake()
	{
		CachedTransform.localScale = Vector3.forward;
	}

	protected virtual void Update()
	{
		UpdatePosition();
		UpdateSize();
	}

	protected virtual void UpdatePosition()
	{
		Entity.Transform.TranslateLocalTowards(CurrentPosition, MoveSpeed * Time.DeltaTime, Axes.XY);
	}

	protected virtual void UpdateSize()
	{
		Entity.Transform.ScaleLocalTowards(CurrentSize, GrowSpeed * Time.DeltaTime, Axes.XY);
	}

	public abstract bool ShouldGrow();

	public abstract bool ShouldMove();

	public virtual bool ShouldManage()
	{
		return true;
	}

	public abstract Point2 GetGrowth();
}

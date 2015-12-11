using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pseudo;

public class MotionComponentBase : PComponent
{
	[Polar]
	public Vector2 Direction;

	void Update()
	{
		TimeComponent time;

		if (!Entity.TryGetComponent(out time))
			return;

		Entity.Transform.Translate(Direction * time.DeltaTime);
	}
}

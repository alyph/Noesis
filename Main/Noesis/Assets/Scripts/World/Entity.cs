using UnityEngine;
using System.Collections;

public class Entity
{
	public World World { get; private set; }
	public string DisplayName { get; set; }

	public virtual void Init(World world)
	{
		World = world;
	}
}

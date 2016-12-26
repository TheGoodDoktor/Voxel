using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxel
{
	// base class for world builder scriptable objects
	public class WorldBuilder : ScriptableObject, IWorldBuilder 
	{
		public virtual void Init(WorldData world){}
		public virtual void BuildWorldChunk(WorldData world, Chunk chunk){}
		
	}
}

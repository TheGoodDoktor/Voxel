using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxel
{
	public class TestWorldBuilder : IWorldBuilder 
	{
		public void BuildWorld(WorldData world)
		{
			IntVec3 size = world.WorldSizeBlocks;

			for(int x=0;x<size.x;x++)
			{
				for(int z=0;z<size.z;z++)
				{
					int height = Random.Range(1,10);
					for(int y=0;y<height;y++)
						world.SetBlock(new IntVec3(x,y,z),BlockType.Solid);
					
				}
			}
		}
	}
}

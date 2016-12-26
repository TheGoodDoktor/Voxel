using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// An alternative mesh builder which uses the greedy mesh building algorithm
// It needs to use texture arrays as it requires texture repeats
namespace Voxel
{
	
	public class GreedyMeshBuilder : MeshBuilder 
	{
		// these should go in a base class
		private WorldData 	m_WorldData;	// voxel world data - needed because we need to look outside current chunk
		private float       m_BlockSize;    // Size of block unit         
	
		public GreedyMeshBuilder(WorldData worldData, float blockSize)
		{
			m_WorldData = worldData;
			m_BlockSize = blockSize;

		}
	
		// Build mesh for supplied chunk & store in internal chunk mesh data
		public override void BuildMeshFromChunk(Chunk chunk)
		{
		}
	}
}

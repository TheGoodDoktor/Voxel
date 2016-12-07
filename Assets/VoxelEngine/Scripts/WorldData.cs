using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxel
{

// Class to encapsulate the voxel world data
public class WorldData 
{
	// World & chunk sizes - set defaults
	private IntVec3 m_WorldSizeChunks = new IntVec3(16,16,16);
	private IntVec3 m_ChunkSizeBlocks = new IntVec3(32,32,32);
	private IntVec3 m_WorldSize;
	
	private Chunk[,,]	m_Chunks;
	private Block		m_OutsideBlock;
	
	// this list is used to store a list of dirty chunks - this will be used to rebuild the voxel meshes
	private List<Chunk>	m_DirtyChunks = new List<Chunk>();

	// public properties
	public IntVec3 WorldSizeChunks { get { return m_WorldSizeChunks;}}
	public IntVec3 ChunkSizeBlocks { get { return m_ChunkSizeBlocks;}}
	
	public WorldData()
	{
		m_OutsideBlock.m_Type = BlockType.Air;	// block used to return invalid types etc.
	}

	public void InitChunks()
	{
		m_Chunks = new Chunk[m_WorldSizeChunks.x,m_WorldSizeChunks.y,m_WorldSizeChunks.z];
		
		m_WorldSize = new IntVec3(m_WorldSizeChunks.x * m_ChunkSizeBlocks.x,m_WorldSizeChunks.y * m_ChunkSizeBlocks.y,m_WorldSizeChunks.z * m_ChunkSizeBlocks.z);
	}
	
	public bool PosOutsideWorld(IntVec3 pos)
	{
		if(	pos.x < 0 || pos.y < 0 || pos.z < 0 || 
			pos.x >= m_WorldSize.x || pos.y >= m_WorldSize.y || pos.z >= m_WorldSize.z)
			return true;
			
		return false;
 	}

	// coords passed in air voxel world coords
	// air block is returned for out of world or empty chunk
	public Block GetBlock(IntVec3 pos)
    {
		if(PosOutsideWorld(pos))
			return m_OutsideBlock;
		
		int chunkX = pos.x / m_ChunkSizeBlocks.x;
        int chunkY = pos.y / m_ChunkSizeBlocks.y;
        int chunkZ = pos.z / m_ChunkSizeBlocks.z;
        int blockX = pos.x % m_ChunkSizeBlocks.x;
        int blockY = pos.y % m_ChunkSizeBlocks.y;
        int blockZ = pos.z % m_ChunkSizeBlocks.z;

		Chunk chunk = m_Chunks[chunkX, chunkY, chunkZ];
		if(chunk == null)
			return m_OutsideBlock;
		else
			return chunk.Blocks[blockX, blockY, blockZ];
    }
	
	// Set a block in the world with optional dirty maring
	// position is passed in as voxel world coords
	public void SetBlock(IntVec3 pos, Block block, bool bMarkDirty = true)
	{
		if(PosOutsideWorld(pos))
			return;
		
		// calc chunk and block coords
		int chunkX = pos.x / m_ChunkSizeBlocks.x;
        int chunkY = pos.y / m_ChunkSizeBlocks.y;
        int chunkZ = pos.z / m_ChunkSizeBlocks.z;
        int blockX = pos.x % m_ChunkSizeBlocks.x;
        int blockY = pos.y % m_ChunkSizeBlocks.y;
        int blockZ = pos.z % m_ChunkSizeBlocks.z;

		Chunk chunk = m_Chunks[chunkX, chunkY, chunkZ];
		
		// create new chunk if this one is empty 
		// (we could check against setting air as block type and early out ?)
		if(chunk == null)	
		{
			chunk = new Chunk(new IntVec3(chunkX * m_ChunkSizeBlocks.x,chunkY * m_ChunkSizeBlocks.y,chunkZ * m_ChunkSizeBlocks.z));
			chunk.InitBlocks(m_ChunkSizeBlocks);
			
			m_Chunks[chunkX, chunkY, chunkZ] = chunk;
		}
		
		chunk.Blocks[blockX, blockY, blockZ] = block;
		
		// Mark block dirty and process surrounding blocks if needed
		if(bMarkDirty)
		{
			if(chunk.MarkDirty())
				m_DirtyChunks.Add(chunk);
			
			// If we set a block on the chunk edge set neigbouring chunk as dirty
			if(blockX == 0 && chunkX > 0)
			{
				Chunk neighbour = m_Chunks[chunkX - 1, chunkY, chunkZ];
				if(neighbour != null && neighbour.MarkDirty())
					m_DirtyChunks.Add(neighbour);
			}
			
			if(blockX == m_ChunkSizeBlocks.x - 1 && chunkX < m_WorldSizeChunks.x - 1)
			{
				Chunk neighbour = m_Chunks[chunkX + 1, chunkY, chunkZ];
				if(neighbour != null && neighbour.MarkDirty())
					m_DirtyChunks.Add(neighbour);
			}
			
			// TODO: Y neigbours
			
			// TODO: Z neigbours
			
		}
			
	}
}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxel
{

public class Chunk
{
	public Chunk(IntVec3 pos)
	{
		m_WorldPos = pos;
	}
	
	public void InitBlocks(IntVec3 size)
	{
		m_BlockData = new Block[size.x,size.y,size.z];
	}	
	
	public bool MarkDirty()
	{
		if(m_bDirty == false)
		{
			m_bDirty = true;
			return true;
		}
		
		return false;
	}

	public void MarkRebuilt()
	{
		if(m_bDirty == true)
		{
			OnChunkRebuilt(this);
			m_bDirty = false;
		}
	}
	
	// Properties
	public Block[,,] Blocks{get{return m_BlockData;} }
	public IntVec3 WorldPos {get{return m_WorldPos;}}

	// Event Handling
	public delegate void ChunkDelegate(Chunk chk);	// multi-purpose delegate that passes in this chunk

	public event ChunkDelegate		OnChunkBecameDirty;	// called when chunk becomes dirty
	public event ChunkDelegate		OnChunkRebuilt;		// called when chunk needs to be rebuilt

	// Private Data
	private bool		m_bDirty = false;	// Chunk is dirty
	private IntVec3 	m_WorldPos;		// World position of chunk bottom corner in voxel coords
	private Block[,,] 	m_BlockData;	// 3d array of blocks
	
	// Mesh Data
	// These will be filled up and then written out to Unity mesh filters
	// this is here for convienience and may not be the best design
    private List<int> 		m_Indices = new List<int>();
    private List<Vector2> 	m_UVs = new List<Vector2>();
    private List<Vector3> 	m_Vertices = new List<Vector3>();
    private List<Color> 	m_Colours = new List<Color>();
	
	// property accessors
	public List<int> Indices 		{get{return m_Indices;} set{m_Indices = value;}}
	public List<Vector2> UVs		{get{return m_UVs;} set{m_UVs = value;}}
	public List<Vector3> Vertices	{get{return m_Vertices;} set{m_Vertices = value;}}
	public List<Color> Colours 		{get{return m_Colours;} set{m_Colours = value;}}

}

}	// namespace Voxel

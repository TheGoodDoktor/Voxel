using System.Collections;
using System.Collections.Generic;

using UnityEngine;

// The purpose of this class it to generate mesh for chunks
// typically one instance wil be instantiated in the world and it will be passed a series of chunks
// which need meshes built. As well as initially creating meshes it will also update a current chunk mesh

// Future ideas
// We could create a mesh builder interface have various mesh builders for greedy mesh, marching cubes etc.

namespace Voxel
{

public class MeshBuilder
{
	private WorldData 	m_WorldData;	// voxel world data - needed because we need to look outside current chunk
	
	public MeshBuilder(WorldData worldData)
	{
		m_WorldData = worldData;
	}
	
	// Build mesh for supplied chunk & store in internal chunk mesh data
	public void BuildMeshFromChunk(Chunk chunk)
	{
        int index = 0;
        chunk.Vertices = new List<Vector3>();
        chunk.Indices = new List<int>();
        chunk.UVs = new List<Vector2>();
        chunk.Colours = new List<Color>();

        for (int x = 0; x < m_WorldData.ChunkSizeBlocks.x; x++)
        {
            int blockX = (chunk.WorldPos.x * m_WorldData.ChunkSizeBlocks.x) + x;
            for (int y = 0; y < m_WorldData.ChunkSizeBlocks.y; y++)
            {
                int blockY = (chunk.WorldPos.y * m_WorldData.ChunkSizeBlocks.y) + y;
                for (int z = 1; z < m_WorldData.ChunkSizeBlocks.z - 1; z++)
                {
                    int blockZ = (chunk.WorldPos.z * m_WorldData.ChunkSizeBlocks.z) + z;
                    // x,y,z is co-ord of block inside chunk
                    // blockX,blockY,blockZ is co-ord of block in world
                    index = BuildMeshForBlock(blockX, blockY, blockZ, x, y, z, chunk, index);
                }
            }
        }
	}

    // Buid the mesh for a given block within a chunk
    private int BuildMeshForBlock(int blockX, int blockY, int blockZ, int x, int y, int z, Chunk chunk, int index)
    {
        Block currentBlock = chunk.Blocks[x, y, z];

        // if it isn't an air block then bail because it won't have any faces to build
        if ((currentBlock.m_Type != BlockType.Air))
            return index;

        byte lightAmount = 255;//currentBlock.LightAmount;

        // "South" side
        BlockType blockType = m_WorldData.GetBlock(new IntVec3(blockX, blockY - 1, blockZ)).m_Type;

        if (blockType != BlockType.Air)
        {
            // The block is solid. Just add its info to the mesh,
            // using our current air block's light amount for its lighting.

            AddBlockFace(   new IntVec3(x + 1, y, z), new IntVec3(x + 1, y, z + 1),
                            new IntVec3(x, y, z + 1), new IntVec3(x, y, z), 
                            0.5f, chunk, index, blockType, BlockFace.Side, lightAmount);
            index += 4;
        }
        // TODO: build mesh for each face

        return index;
    }

	// Add a face to the chunk
	// This function could be in a base class if we need it for more mesh generators
	private void AddBlockFace(IntVec3 va, IntVec3 vb, IntVec3 vc, IntVec3 vd, float colour, Chunk chunk, int index, BlockType blockType,
                              BlockFace blockFace,
                              byte blockLight)
    {
        float actualColour = (colour * blockLight) / 255;
        const float epsilon = 0.001f;
        chunk.Vertices.Add( va.ToVector3() );
        chunk.Vertices.Add( vb.ToVector3() );
        chunk.Vertices.Add( vc.ToVector3() );
        chunk.Vertices.Add( vd.ToVector3() );

        var item = new Color(actualColour, actualColour, actualColour, 1.0f);
        chunk.Colours.Add(item);
        chunk.Colours.Add(item);
        chunk.Colours.Add(item);
        chunk.Colours.Add(item);

        chunk.Indices.Add(index);
        chunk.Indices.Add(index + 1);
        chunk.Indices.Add(index + 2);

        chunk.Indices.Add(index + 2);
        chunk.Indices.Add(index + 3);
        chunk.Indices.Add(index);

		// TODO: Sort out UV generation
		// we might want to use texture arrays but will probably start off with atlases
		
        Rect worldTextureAtlasUv = new Rect(0,0,1,1);   // TODO: get from atlas
            //m_WorldData.BlockUVCoordinates[(int) blockType].BlockFaceUvCoordinates[(int) blockFace];

        chunk.UVs.Add(new Vector2(worldTextureAtlasUv.x + epsilon, worldTextureAtlasUv.y + epsilon));
        chunk.UVs.Add(new Vector2(worldTextureAtlasUv.x + epsilon,
                                  worldTextureAtlasUv.y + worldTextureAtlasUv.height - epsilon));
        chunk.UVs.Add(new Vector2(worldTextureAtlasUv.x + worldTextureAtlasUv.width - epsilon,
                                  worldTextureAtlasUv.y + worldTextureAtlasUv.height - epsilon));
        chunk.UVs.Add(new Vector2(worldTextureAtlasUv.x + worldTextureAtlasUv.width - epsilon,
                                  worldTextureAtlasUv.y + epsilon));
		
    }

}
	
}//namespace Voxel
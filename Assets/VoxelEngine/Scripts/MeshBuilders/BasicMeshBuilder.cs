using System.Collections;
using System.Collections.Generic;

using UnityEngine;

// The purpose of this class it to generate mesh for chunks
// typically one instance wil be instantiated in the world and it will be passed a series of chunks
// which need meshes built. As well as initially creating meshes it will also update a current chunk mesh

// This is a basic builder which builds polys for externally facing block faces

namespace Voxel
{
    public class BasicMeshBuilder : IMeshBuilder
    {
	    private WorldData 	m_WorldData;	// voxel world data - needed because we need to look outside current chunk
        private float       m_BlockSize;    // Size of block unit   
        private Vector3[]   m_FaceNormals = new Vector3[(int)BlockFace.Count];      
	
	    public BasicMeshBuilder(WorldData worldData, float blockSize)
	    {
		    m_WorldData = worldData;
            m_BlockSize = blockSize;

            m_FaceNormals[(int)BlockFace.Top]       = new Vector3(0, 1, 0);
            m_FaceNormals[(int)BlockFace.Bottom]    = new Vector3(0, -1, 0);
            m_FaceNormals[(int)BlockFace.North]     = new Vector3(0, 0, 1);
            m_FaceNormals[(int)BlockFace.South]     = new Vector3(0, 0, -1);
            m_FaceNormals[(int)BlockFace.East]      = new Vector3(1, 0, 0);
            m_FaceNormals[(int)BlockFace.West]      = new Vector3(-1, 0, 0);
        }
	
	    // Build mesh for supplied chunk & store in internal chunk mesh data
	    public void BuildMeshFromChunk(Chunk chunk)
	    {
            int index = 0;
            chunk.Vertices = new List<Vector3>();
            chunk.Normals = new List<Vector3>();
            chunk.Indices = new List<int>();
            chunk.UVs = new List<Vector2>();
            chunk.Colours = new List<Color>();

            Debug.Log("Building mesh for chunk at "+ chunk.WorldPos.ToString());

            for (int x = 0; x < m_WorldData.ChunkSizeBlocks.x; x++)
            {
                int blockX = chunk.WorldPos.x + x;
                for (int y = 0; y < m_WorldData.ChunkSizeBlocks.y; y++)
                {
                    int blockY = chunk.WorldPos.y + y;
                    for (int z = 0; z < m_WorldData.ChunkSizeBlocks.z; z++)
                    {
                        int blockZ = chunk.WorldPos.z + z;
                        // x,y,z is co-ord of block inside chunk
                        // blockX,blockY,blockZ is co-ord of block in world
                        index = BuildMeshForBlock(blockX, blockY, blockZ, x, y, z, chunk, index);
                    }
                }
            }

            Debug.Log("Vertices: " + chunk.Vertices.Count);
	    }

        // Buid the mesh for a given block within a chunk
        private int BuildMeshForBlock(int blockX, int blockY, int blockZ, int x, int y, int z, Chunk chunk, int index)
        {
            Block currentBlock = chunk.Blocks[x, y, z];

            // if it isn't a transparent block then bail because it won't have any faces to build
            if ((currentBlock.IsTransparent() == false))
                return index;

            byte lightAmount = 255;//currentBlock.LightAmount;

            // Check surround blocks, if they aren't transparent then they have an outside face

            // Bottom
            Block block = m_WorldData.GetBlock(new IntVec3(blockX, blockY - 1, blockZ));

            if (block.IsTransparent() == false)
            {
                AddBlockFace(   new IntVec3(x + 1, y, z), new IntVec3(x + 1, y, z + 1),
                                new IntVec3(x, y, z + 1), new IntVec3(x, y, z), 
                                0.5f, chunk, index, block.m_Type, BlockFace.Bottom, lightAmount);
                index += 4;
            }

            // West
            block = m_WorldData.GetBlock(new IntVec3(blockX - 1, blockY, blockZ));
            if (block.IsTransparent() == false)
            {
                AddBlockFace(new IntVec3(x, y, z),
                                new IntVec3(x, y, z + 1),
                                new IntVec3(x, y + 1, z + 1),
                                new IntVec3(x, y + 1, z), 
                                0.8f, chunk, index, block.m_Type, BlockFace.West, lightAmount);
                index += 4;
            }

            // Top
            block = m_WorldData.GetBlock(new IntVec3(blockX, blockY + 1, blockZ));
            if (block.IsTransparent() == false)
            {
                AddBlockFace(new IntVec3(x, y + 1, z),
                                new IntVec3(x, y + 1, z + 1),
                                new IntVec3(x + 1, y + 1, z + 1),
                                new IntVec3(x + 1, y + 1, z),
                                0.9f, chunk, index, block.m_Type, BlockFace.Top, lightAmount);

                index += 4;
            }

            // East 
            block = m_WorldData.GetBlock(new IntVec3(blockX + 1, blockY, blockZ));
            if (block.IsTransparent() == false)
            {
                AddBlockFace(new IntVec3(x + 1, y + 1,z), 
                            new IntVec3(x + 1, y + 1,z + 1),  
                            new IntVec3(x + 1, y,z + 1), 
                            new IntVec3(x + 1, y, z), 
                            0.7f, chunk, index, block.m_Type, BlockFace.East, lightAmount);

                index += 4;
            }

            // North
            block = m_WorldData.GetBlock(new IntVec3(blockX, blockY, blockZ + 1));
            if (block.IsTransparent() == false)
            {
                AddBlockFace(new IntVec3(x + 1, y,z + 1), 
                            new IntVec3(x + 1, y + 1,z + 1), 
                            new IntVec3(x, y + 1,z + 1), 
                            new IntVec3(x, y, z + 1), 
                            0.4f, chunk, index, block.m_Type, BlockFace.North, lightAmount);

                index += 4;
            }

            // South
            block = m_WorldData.GetBlock(new IntVec3(blockX, blockY, blockZ - 1));
            if (block.IsTransparent() == false)
            {
                AddBlockFace(new IntVec3(x, y,z),
                            new IntVec3(x, y + 1,z),
                            new IntVec3(x + 1, y + 1,z),
                            new IntVec3(x + 1, y, z),
                                1.0f, chunk, index, block.m_Type, BlockFace.South, lightAmount);

                index += 4;
            }

            return index;
        }

	    // Add a face to the chunk
	    // This function could be in a base class if we need it for more mesh generators
	    private void AddBlockFace(IntVec3 va, IntVec3 vb, IntVec3 vc, IntVec3 vd, float colour, Chunk chunk, int index, byte blockType,
                                  BlockFace blockFace,
                                  byte blockLight)
        {
            float actualColour = (colour * blockLight) / 255;
            const float epsilon = 0.001f;

            // Vertices
            chunk.Vertices.Add( va.ToVector3() * m_BlockSize);
            chunk.Vertices.Add( vb.ToVector3() * m_BlockSize);
            chunk.Vertices.Add( vc.ToVector3() * m_BlockSize);
            chunk.Vertices.Add( vd.ToVector3() * m_BlockSize);

            // Normals
            chunk.Normals.Add(m_FaceNormals[(int)blockFace]);
            chunk.Normals.Add(m_FaceNormals[(int)blockFace]);
            chunk.Normals.Add(m_FaceNormals[(int)blockFace]);
            chunk.Normals.Add(m_FaceNormals[(int)blockFace]);

            // Colours
            var item = new Color(actualColour, actualColour, actualColour, 1.0f);
            chunk.Colours.Add(item);
            chunk.Colours.Add(item);
            chunk.Colours.Add(item);
            chunk.Colours.Add(item);

            // Index quad of 2 tris
            chunk.Indices.Add(index + 2);
            chunk.Indices.Add(index + 1);
            chunk.Indices.Add(index + 0);

            chunk.Indices.Add(index + 0);
            chunk.Indices.Add(index + 3);
            chunk.Indices.Add(index + 2);

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
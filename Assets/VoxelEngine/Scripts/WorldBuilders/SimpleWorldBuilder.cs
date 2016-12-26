using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxel
{
    [CreateAssetMenu(fileName = "SimpleWorldBuilder", menuName = "Voxel/World Builders/Simple", order = 1)]
	public class SimpleWorldBuilder : WorldBuilder 
	{
        // prperties exposed in data asset
        private bool m_BuildCaves = true;
        private float m_CaveThreshold = 0.2f;
        public byte m_AirBlock = (byte)BlockType.Air;
        public byte m_SolidBlock = (byte)BlockType.Solid;

        // private data
        private float[,]  m_HeightField;    // heightfield for landscape
  
        // Initialise the worldbuilder by creating a heightfield
        public override void Init(WorldData world)
        {
            m_HeightField = new float[world.WorldSizeBlocks.x, world.WorldSizeBlocks.z];
            int worldHeightInBlocks = world.WorldSizeBlocks.y;
            int minimumGroundheight = worldHeightInBlocks / 4;
            int groundheightRange = (int)(worldHeightInBlocks * 0.75f);

            for(int blockX = 0;blockX<world.WorldSizeBlocks.x;blockX++)
            {
                for (int blockZ = 0; blockZ < world.WorldSizeBlocks.z; blockZ++)
                {
                    float octave1 = PerlinSimplexNoise.noise(blockX * 0.0001f, blockZ * 0.0001f) * 0.5f;
                    float octave2 = PerlinSimplexNoise.noise(blockX * 0.0005f, blockZ * 0.0005f) * 0.25f;
                    float octave3 = PerlinSimplexNoise.noise(blockX * 0.005f, blockZ * 0.005f) * 0.12f;
                    float octave4 = PerlinSimplexNoise.noise(blockX * 0.01f, blockZ * 0.01f) * 0.12f;
                    float octave5 = PerlinSimplexNoise.noise(blockX * 0.03f, blockZ * 0.03f) * octave4;
                    float lowerGroundHeight = octave1 + octave2 + octave3 + octave4 + octave5;
                    lowerGroundHeight = (lowerGroundHeight * groundheightRange) + minimumGroundheight;
                    m_HeightField[blockX, blockZ] = lowerGroundHeight;
                }
            }
        }

        // build the data for a specific block in the world
        public override void BuildWorldChunk(WorldData world, Chunk chunk)
		{
			IntVec3 size = world.ChunkSizeBlocks;

            if (m_HeightField == null)
                return;

			for(int x=0;x<size.x;x++)
			{
				int blockX = chunk.WorldPos.x + x;

				for(int z=0;z<size.z;z++)
				{
					int blockZ = chunk.WorldPos.z + z;
					GenerateTerrain(chunk,x,z,blockX,blockZ,world.WorldSizeBlocks.y);
				}
			}

            if (chunk.MarkDirty())
                world.AddDirtyChunk(chunk);
		}

		private void GenerateTerrain(Chunk chunk, int blockXInChunk, int blockZInChunk, int blockX, int blockZ, int worldHeightInBlocks)
        {
            int groundHeightInChunk = Mathf.FloorToInt(m_HeightField[blockX,blockZ]) - chunk.WorldPos.y;

            for (int y = 0; y < chunk.World.ChunkSizeBlocks.y; y++)
            {
                bool bUnderground = y < groundHeightInChunk;
                float density = bUnderground ? 1.0f : 0.0f;
                byte blockType = bUnderground ? m_SolidBlock : m_AirBlock;

                if(bUnderground)    // are we underground - check for caves
                {
                    if (m_BuildCaves)
                    {
                        int worldY = y + chunk.WorldPos.y;
                        float octave1 = PerlinSimplexNoise.noise(blockX * 0.009f, blockZ * 0.009f, worldY * 0.009f) * 0.25f;

                        float initialNoise = octave1 + PerlinSimplexNoise.noise(blockX * 0.04f, blockZ * 0.04f, worldY * 0.04f) * 0.15f;
                        initialNoise += PerlinSimplexNoise.noise(blockX * 0.08f, blockZ * 0.08f, worldY * 0.08f) * 0.05f;

                        if (initialNoise > m_CaveThreshold)
                        {
                            blockType = m_AirBlock; // cave
                            density = 0;
                        }
                        else
                        {
                            // TODO: work out cave density
                            //density = 1.0f - initialNoise;
                        }
                    }

                }
                else
                {
                    // Work out above ground density
                    if(y == groundHeightInChunk)
                        density = m_HeightField[blockX, blockZ] - Mathf.Floor(m_HeightField[blockX, blockZ]);
                }
                
                chunk.Blocks[blockXInChunk, y, blockZInChunk].m_Type = blockType;
                chunk.Blocks[blockXInChunk, y, blockZInChunk].m_Density = density;
            }
        }
	}
}

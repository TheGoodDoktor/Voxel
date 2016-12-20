using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxel
{
	public class SimpleWorldBuilder : IWorldBuilder 
	{
		public void BuildWorldChunk(WorldData world, Chunk chunk)
		{
			IntVec3 size = world.ChunkSizeBlocks;

			for(int x=0;x<size.x;x++)
			{
				int blockX = chunk.WorldPos.x + x;

				for(int z=0;z<size.z;z++)
				{
					int blockZ = chunk.WorldPos.z + z;
					GenerateTerrain(chunk,x,z,blockX,blockZ,world.WorldSizeBlocks.y);
				}
			}
		}

		private static void GenerateTerrain(Chunk chunk, int blockXInChunk, int blockZInChunk, int blockX, int blockZ, int worldHeightInBlocks)
        {
            // The lower ground level is at least this high.
            int minimumGroundheight = worldHeightInBlocks / 4;
            int minimumGroundDepth =(int)(worldHeightInBlocks * 0.75f);

            float octave1 = PerlinSimplexNoise.noise(blockX * 0.0001f, blockZ * 0.0001f) * 0.5f;
            float octave2 = PerlinSimplexNoise.noise(blockX * 0.0005f, blockZ * 0.0005f) * 0.25f;
            float octave3 = PerlinSimplexNoise.noise(blockX * 0.005f, blockZ * 0.005f) * 0.12f;
            float octave4 = PerlinSimplexNoise.noise(blockX * 0.01f, blockZ * 0.01f) * 0.12f;
            float octave5 = PerlinSimplexNoise.noise(blockX * 0.03f, blockZ * 0.03f) * octave4;
            float lowerGroundHeight = octave1 + octave2 + octave3 + octave4 + octave5;
            lowerGroundHeight = lowerGroundHeight * minimumGroundDepth + minimumGroundheight;
            bool sunlit = true;
            BlockType blockType = BlockType.Air;
            for (int y = worldHeightInBlocks - 1; y >= 0; y--)
            {
                if (y <= lowerGroundHeight)
                {
                    blockType = BlockType.Solid;
                }
                
                chunk.Blocks[blockXInChunk, y, blockZInChunk].m_Type = blockType;
            }
        }
	}
}

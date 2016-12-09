using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxel
{

    public class WorldObject : MonoBehaviour
    {
        public float m_BlockSize = 1.0f;
        public IntVec3 m_WorldSizeChunks = new IntVec3(16, 16, 16);
        public IntVec3 m_ChunkSizeBlocks = new IntVec3(32, 32, 32);
        public GameObject m_ChunkPrefab;

        private WorldData m_WorldData = new WorldData();
        private MeshBuilder m_MeshBuilder;

        private GameObject m_ChunkRoot;

        // World Dimensions
        private IntVec3 m_WorldBlockSize;
        private Vector3 m_ChunkSize;
        private Vector3 m_WorldSize;
        private Vector3 m_WorldMin;
        private Vector3 m_WorldMax;

        // Use this for initialization
        void Start ()
        {
            // Init world data
            m_WorldData.SetDimensions(m_WorldSizeChunks, m_ChunkSizeBlocks);
            m_WorldData.InitChunks();

            // Calc world dimensions
            m_WorldBlockSize = new IntVec3(
                m_WorldSizeChunks.x * m_ChunkSizeBlocks.x, 
                m_WorldSizeChunks.y * m_ChunkSizeBlocks.y, 
                m_WorldSizeChunks.z * m_ChunkSizeBlocks.z);

            // real world units
            m_ChunkSize = new Vector3(m_ChunkSizeBlocks.x * m_BlockSize, m_ChunkSizeBlocks.y * m_BlockSize, m_ChunkSizeBlocks.z * m_BlockSize);
            m_WorldSize = new Vector3(m_WorldBlockSize.x * m_BlockSize, m_WorldBlockSize.y * m_BlockSize, m_WorldBlockSize.z * m_BlockSize);
            m_WorldMin = Vector3.zero - (m_WorldSize * 0.5f);
            m_WorldMax = m_WorldMin + m_WorldSize;

            m_MeshBuilder = new MeshBuilder(m_WorldData);
            // Test
            m_WorldData.SetBlock(new IntVec3(0, 0, 0), BlockType.Solid);

            CreateChunkObjects();
	    }

        void CreateChunkObjects()
        {
            m_ChunkRoot = new GameObject("ChunkRoot");
            m_ChunkRoot.transform.parent = transform;

            // TODO: build chunk objects under root from Prefab
            Vector3 chunkPos = m_WorldMin;
            for (int x = 0; x < m_WorldSizeChunks.x; x++)
            {
                for (int y = 0; y < m_WorldSizeChunks.y; y++)
                {
                    for (int z = 0; z < m_WorldSizeChunks.z; z++)
                    {
                        // Create new chunk and place in hierachy
                        GameObject newChunk = GameObject.Instantiate(m_ChunkPrefab, chunkPos, new Quaternion());
                        newChunk.name = string.Format("Chunk {0}{1}{2}", x.ToString(), y.ToString(), z.ToString());
                        newChunk.transform.parent = m_ChunkRoot.transform;
                        chunkPos += new Vector3(0,0,m_ChunkSize.z);
                    }
                    chunkPos += new Vector3(0,m_ChunkSize.y,0);
                }

                chunkPos += new Vector3(m_ChunkSize.y,0,0);
            }
        }
	
	    // Update is called once per frame
	    void Update () {
		
	    }
    }

}//namespace Voxel

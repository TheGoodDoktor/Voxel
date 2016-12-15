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
        private IMeshBuilder m_MeshBuilder;

        private GameObject m_ChunkRoot;

        private static WorldObject m_Instance;

        // World Dimensions
        private IntVec3 m_WorldBlockSize;
        private Vector3 m_ChunkSize;
        private Vector3 m_WorldSize;
        private Vector3 m_WorldMin;
        private Vector3 m_WorldMax;

        public static WorldObject Instance { get{ return m_Instance;}}

        void Awake()
        {
            m_Instance = this;
        }

        // Use this for initialization
        void Start ()
        {
            // Init world data
            m_WorldData.SetDimensions(m_WorldSizeChunks, m_ChunkSizeBlocks);
            m_WorldData.InitChunks();
            m_WorldData.OnNewChunk += OnNewChunk;

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

            // Create a basic mesh builder
            // TODO: use a specified mesh builder?
            m_MeshBuilder = new BasicMeshBuilder(m_WorldData, m_BlockSize);

            // Hierachy
            m_ChunkRoot = new GameObject("ChunkRoot");
            m_ChunkRoot.transform.parent = transform;

            TestFillVoxels();
        }

        void TestFillVoxels()
        {
           // Test
            m_WorldData.SetBlock(new IntVec3(0, 0, 0), BlockType.Solid);
            m_WorldData.SetBlock(new IntVec3(1, 1, 1), BlockType.Solid);
            m_WorldData.SetBlock(new IntVec3(1, 2, 1), BlockType.Solid);
            m_WorldData.SetBlock(new IntVec3(2, 2, 2), BlockType.Solid);
        }

        // Handler for when new chunk is created
        void OnNewChunk(Chunk chunk)
        {
            Vector3 chunkPos = m_WorldMin + new Vector3(chunk.WorldPos.x * m_BlockSize, chunk.WorldPos.y * m_BlockSize, chunk.WorldPos.z * m_BlockSize);
            GameObject newChunkGameObj = GameObject.Instantiate(m_ChunkPrefab, chunkPos, new Quaternion());

            newChunkGameObj.name = string.Format("Chunk {0}", chunk.ChunkPos.ToString());
            newChunkGameObj.transform.parent = m_ChunkRoot.transform;

            chunk.GameObject = newChunkGameObj.GetComponent<ChunkObject>();
        }

	    // Update is called once per frame
	    void Update ()
        {
            // TODO: Check world data for dirty chunks & rebuild them
            var dirtyChunks = m_WorldData.DirtyChunks;
            foreach(var chunk in dirtyChunks)
            {
                m_MeshBuilder.BuildMeshFromChunk(chunk);
                chunk.GameObject.CreateMeshFromChunk(chunk);
            }
            dirtyChunks.Clear();
        }

        // functions to get & set voxels at world positions
        public Block GetBlockAt(Vector3 worldPos)
        {
            worldPos -= m_WorldMin; // offset from origin

            return m_WorldData.GetBlock(new IntVec3(
                (int)(worldPos.x / m_BlockSize),
                (int)(worldPos.y / m_BlockSize),
                (int)(worldPos.z / m_BlockSize)
            ));
        }

        
        public void SetBlockAt(Vector3 worldPos,Block block)
        {
            worldPos -= m_WorldMin; // offset from origin

            Debug.Log("Setting block at: " + worldPos.ToString());
            
            m_WorldData.SetBlock(new IntVec3(
                (int)(worldPos.x / m_BlockSize),
                (int)(worldPos.y / m_BlockSize),
                (int)(worldPos.z / m_BlockSize)
             ), block);
        }

        public Block GetBlockFromRaycastHit(RaycastHit hit)
        {
            Vector3 point = hit.point + (hit.normal * m_BlockSize * 0.5f);
            return GetBlockAt(point);
        }

        public void SetBlockFromRaycastHit(RaycastHit hit, Block block)
        {
            Vector3 point = hit.point + (hit.normal * m_BlockSize * 0.5f);
            SetBlockAt(point,block);
        }

    }

}//namespace Voxel

﻿using System.Collections;
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
        public BlockInfoList m_BlockInfoList;
        public bool m_TestMeshBuilder = false;
        public MeshBuilder m_MeshBuilder;   // which mesh builder we want to use
        public WorldBuilder m_WorldBuilder;   // which world builder we want to use
        
        private WorldData m_WorldData = new WorldData();

        private BasicMeshBuilder m_BasicMeshBuilder;
        private MarchingCubesMeshBuilder m_MarchingCubesMeshBuilder;
        //private IWorldBuilder m_WorldBuilder;

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
            m_BasicMeshBuilder = new BasicMeshBuilder(m_WorldData, m_BlockSize);
            m_MarchingCubesMeshBuilder = new MarchingCubesMeshBuilder(m_WorldData, m_BlockSize);
            m_MeshBuilder = m_BasicMeshBuilder;

            // Hierachy
            m_ChunkRoot = new GameObject("ChunkRoot");
            m_ChunkRoot.transform.parent = transform;

            // Setup & run world builder
            m_WorldBuilder = new SimpleWorldBuilder();
            //m_WorldBuilder = new TestWorldBuilder();

            m_WorldBuilder.Init(m_WorldData);
            // build all chunks
            for (int chunkX =0;chunkX<m_WorldSizeChunks.x ;chunkX++)
            {
                for(int chunkY =0;chunkY<m_WorldSizeChunks.y ;chunkY++)
                {
                    for(int chunkZ =0;chunkZ<m_WorldSizeChunks.z ;chunkZ++)
                    {
                        Chunk chunk = m_WorldData.CreateChunk(chunkX,chunkY,chunkZ);
                        m_WorldBuilder.BuildWorldChunk(m_WorldData,chunk);
                    }
                }
            }

            //TestFillVoxels();
        }

        void TestFillVoxels()
        {
           // Test
            m_WorldData.SetBlock(0, 0, 0, (byte)BlockType.Solid);
            m_WorldData.SetBlock(1, 1, 1, (byte)BlockType.Solid);
            m_WorldData.SetBlock(1, 2, 1, (byte)BlockType.Solid);
            m_WorldData.SetBlock(2, 2, 2, (byte)BlockType.Solid);
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
            MeshBuilder newMeshBuilder = null;
            if (m_TestMeshBuilder)
                newMeshBuilder = m_MarchingCubesMeshBuilder;
            else
                newMeshBuilder = m_BasicMeshBuilder;

            if(newMeshBuilder != m_MeshBuilder)
            {
                m_MeshBuilder = newMeshBuilder;
                m_WorldData.MarkAllChunksDirty();
            }

            // TODO: Check world data for dirty chunks & rebuild them
            var dirtyChunks = m_WorldData.DirtyChunks;
            int chunkCount = 0;

            while(dirtyChunks.Count > 0 && chunkCount < 4)
            {
                Chunk chunk = dirtyChunks[0];
                m_MeshBuilder.BuildMeshFromChunk(chunk);
                chunk.GameObject.CreateMeshFromChunk(chunk);
                dirtyChunks.RemoveAt(0);
                chunkCount++;
            }
        }

        // functions to get & set voxels at world positions
        public Block GetBlockAt(Vector3 worldPos)
        {
            worldPos -= m_WorldMin; // offset from origin

            return m_WorldData.GetBlock(
                (int)(worldPos.x / m_BlockSize),
                (int)(worldPos.y / m_BlockSize),
                (int)(worldPos.z / m_BlockSize)
            );
        }

        
        public void SetBlockAt(Vector3 worldPos,byte blockType)
        {
            worldPos -= m_WorldMin; // offset from origin

            Debug.Log("Setting block at: " + worldPos.ToString());
            
            m_WorldData.SetBlock(
                (int)(worldPos.x / m_BlockSize),
                (int)(worldPos.y / m_BlockSize),
                (int)(worldPos.z / m_BlockSize)
             , blockType);
        }

        public Block GetBlockFromRaycastHit(RaycastHit hit)
        {
            Vector3 point = hit.point + (hit.normal * m_BlockSize * 0.01f);
            return GetBlockAt(point);
        }

        public void SetBlockInFrontOfRayHit(RaycastHit hit, byte blockType)
        {
            Vector3 point = hit.point + (hit.normal * m_BlockSize * 0.01f);
            SetBlockAt(point,blockType);
        }

        public void SetBlockBehindRayHit(RaycastHit hit, byte blockType)
        {
            Vector3 point = hit.point - (hit.normal * m_BlockSize * 0.01f);
            SetBlockAt(point,blockType);
        }

    }

}//namespace Voxel

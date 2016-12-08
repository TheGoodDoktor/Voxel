using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxel
{

    public class WorldObject : MonoBehaviour
    {
        public IntVec3 m_WorldSize = new IntVec3(16, 16, 16);
        public IntVec3 m_ChunkSize = new IntVec3(32, 32, 32);

        private WorldData m_WorldData = new WorldData();
        private MeshBuilder m_MeshBuilder;

        // Use this for initialization
        void Start ()
        {
            // Init world data
            m_WorldData.SetDimensions(m_WorldSize, m_ChunkSize);
            m_WorldData.InitChunks();

            m_MeshBuilder = new MeshBuilder(m_WorldData);
            // Test
            m_WorldData.SetBlock(new IntVec3(0, 0, 0), BlockType.Solid);
	    }
	
	    // Update is called once per frame
	    void Update () {
		
	    }
    }

}//namespace Voxel

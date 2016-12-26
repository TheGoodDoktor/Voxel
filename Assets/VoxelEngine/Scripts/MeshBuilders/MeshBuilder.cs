using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this is a base class to inherit mesh builder scriptable objects from
// The purpose of this class it to generate mesh for chunks
// typically one scriptable object will be referenced in the world and it will be passed a series of chunks
// which need meshes built. As well as initially creating meshes it will also update a current chunk mesh

namespace Voxel
{
    public class MeshBuilder : ScriptableObject, IMeshBuilder
    {
        public virtual void BuildMeshFromChunk(Chunk chunk)
		{

		}
    }
}

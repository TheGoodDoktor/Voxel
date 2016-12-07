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
		/*
        Rect worldTextureAtlasUv =
            m_WorldData.BlockUvCoordinates[(int) blockType].BlockFaceUvCoordinates[(int) blockFace];

        chunk.UVs.Add(new Vector2(worldTextureAtlasUv.x + epsilon, worldTextureAtlasUv.y + epsilon));
        chunk.UVs.Add(new Vector2(worldTextureAtlasUv.x + epsilon,
                                  worldTextureAtlasUv.y + worldTextureAtlasUv.height - epsilon));
        chunk.UVs.Add(new Vector2(worldTextureAtlasUv.x + worldTextureAtlasUv.width - epsilon,
                                  worldTextureAtlasUv.y + worldTextureAtlasUv.height - epsilon));
        chunk.UVs.Add(new Vector2(worldTextureAtlasUv.x + worldTextureAtlasUv.width - epsilon,
                                  worldTextureAtlasUv.y + epsilon));
		*/
    }

}
	
}//namespace Voxel
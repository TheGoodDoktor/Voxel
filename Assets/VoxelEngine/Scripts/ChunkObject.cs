using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxel
{

public class ChunkObject : MonoBehaviour 
{
    public Shader Shader;
    public Texture Texture;
    public Texture2DArray TextureArray;

    private MeshFilter m_MeshFilter;
    private MeshCollider m_MeshCollider;
    private MeshRenderer m_MeshRenderer;
    private Chunk m_Chunk;  // which chunk we belong too

    // public properties
    public Chunk Chunk{ get{return m_Chunk;}}

	// Use this for initialization
	void Start () 
	{
	    gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        gameObject.AddComponent<MeshCollider>();
        m_MeshFilter = gameObject.GetComponent<MeshFilter>();
        m_MeshCollider = gameObject.GetComponent<MeshCollider>();
        m_MeshRenderer = gameObject.GetComponent<MeshRenderer>();
        m_MeshRenderer.material.shader = Shader;
        m_MeshRenderer.material.mainTexture = Texture;	
	}
	
	// Create the mesh from a chunk
	public void CreateMeshFromChunk(Chunk chunk)
    {
        m_Chunk = chunk;    // maybe change to a once only?

		Debug.Log ("Creating Mesh For Chunk" + chunk.ToString());
        m_MeshFilter.mesh.Clear();
        m_MeshFilter.mesh.vertices = chunk.Vertices.ToArray();
        m_MeshFilter.mesh.normals = chunk.Normals.ToArray();
        m_MeshFilter.mesh.uv = chunk.UVs.ToArray();
        //m_MeshFilter.mesh.colors = chunk.Colours.ToArray();
        m_MeshFilter.mesh.triangles = chunk.Indices.ToArray();
        m_MeshCollider.sharedMesh = null;
        m_MeshCollider.sharedMesh = m_MeshFilter.mesh;

        chunk.Vertices = new List<Vector3>();
        chunk.Normals = new List<Vector3>();
        chunk.UVs = new List<Vector2>();
        chunk.Colours = new List<Color>();
        chunk.Indices = new List<int>();
        chunk.MarkRebuilt();
    }
}

} //namespace Voxel

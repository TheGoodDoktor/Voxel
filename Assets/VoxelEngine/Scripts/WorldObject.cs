using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxel
{

public class WorldObject : MonoBehaviour {

	private WorldData m_WorldData = new WorldData();

	// Use this for initialization
	void Start () {
		m_WorldData.InitChunks();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

}

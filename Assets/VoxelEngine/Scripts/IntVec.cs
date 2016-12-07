
using UnityEngine;

// general purpose integer Vector3
public struct IntVec3
{
	public IntVec3(int vx,int vy,int vz)
	{
		x = vx;
		y = vy;
		z = vz;
	}
	
	public Vector3	ToVector3()
	{
		return new Vector3(x,y,z);
	}
	
	public int x,y,z;
}

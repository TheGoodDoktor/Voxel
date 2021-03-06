
using UnityEngine;

// general purpose integer Vector3
[System.Serializable]
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

    public override string ToString()
    {
        return string.Format("{0},{1},{2}", x.ToString(), y.ToString(), z.ToString());
    }

    public int x,y,z;
}


namespace Voxel
{
	
public enum BlockType : byte
{
    Air = 0,
	Solid,

    Count
}

public enum BlockFace : byte
{
    Top = 0,
    Bottom,
    North,
    South,
    East,
    West,

    Count
}

public struct Block
{
	public byte m_Type;
    public float m_Density;

    public bool IsTransparent() {return m_Type == (int)BlockType.Air;}
}

}
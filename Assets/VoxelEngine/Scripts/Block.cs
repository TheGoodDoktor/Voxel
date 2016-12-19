
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
	public BlockType m_Type;

    public bool IsTransparent() {return m_Type == BlockType.Air;}
}

}
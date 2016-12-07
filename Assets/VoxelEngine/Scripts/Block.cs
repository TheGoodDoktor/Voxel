
namespace Voxel
{
	
public enum BlockType : byte
{
    Air = 0,
	Solid = 1,
}

public enum BlockFace : byte
{
    Top = 0,
    Side = 1,
    Bottom = 2
}

public struct Block
{
	public BlockType m_Type;
}

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlockInfo
{
    public string       m_BlockName;
    public Texture2D    m_TopTexture;
    public Texture2D    m_BottomTexture;
    public Texture2D    m_SideTexture;
}

[CreateAssetMenu(fileName = "BlockInfoList", menuName = "Voxel/BlockInfoList", order = 1)]
public class BlockInfoList : ScriptableObject
{
    public List<BlockInfo>     m_Blocks;
}

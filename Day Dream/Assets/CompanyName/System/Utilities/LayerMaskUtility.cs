using UnityEngine;

public class LayerMaskUtility
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="mask">You layer mask set up in the inspector</param>
    /// <param name="layer">Pass in the layer we are checking</param>
    /// <returns></returns>
    public static bool Contains(LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }
}

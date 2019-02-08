using UnityEngine;

public class LayerMaskUtility
{
    public static bool IsWithinLayerMask(LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }
}

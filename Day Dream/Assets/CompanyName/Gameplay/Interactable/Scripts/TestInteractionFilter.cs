using UnityEngine;

public class TestInteractionFilter : IInteractionFilter
{
    public bool Filter()
    {
        Debug.Log("This is the test one");
        return true;
    }

}

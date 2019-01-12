using UnityEngine;

public class TestInteractionFilter : IInteractionFilter
{
    public bool Filter()
    {
        Debug.Log("This is the test interaction filter");
        return true;
    }

}

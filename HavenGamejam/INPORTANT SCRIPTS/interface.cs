public interface ICanBeActivated
{
    void Activate(bool isActive);
}



public void Activate(bool isActive)
{
    IsActive = isActive;
    if (isActive)
    {
        Debug.Log("Light Activated");
    }
    else
    {
        Debug.Log("Light Deactivated");
    }
}
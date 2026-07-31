// Assets/Scripts/Interaction/IInteractable.cs
namespace ExitR8.Interaction
{
    public interface IInteractable
    {
        string GetPrompt();
        void Interact(PlayerInteractor interactor);
    }
}

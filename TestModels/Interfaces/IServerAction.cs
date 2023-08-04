namespace TestModels.Interfaces
{
    public interface IServerAction
    {
        ServerAction CreateAction<T>(EnableAction action, T data);
        ServerAction GetAction(string action);
        T GetObject<T>(ServerAction serverAction);
    }
}

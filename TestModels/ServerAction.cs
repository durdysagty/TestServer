namespace TestModels
{
    public enum EnableAction { Login, Test, Check }
    public class ServerAction
    {
        public EnableAction Name { get; set; }
        public string JsonObject { get; set; }

    }
}

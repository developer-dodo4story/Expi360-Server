using EnhancedDodoServer;
/// <summary>
/// Stores information about game
/// </summary>
[System.Serializable]
public class Config 
{
    public Config()
    {
        inputType = Consts.InputType.PhonePad;
        server = Consts.Server.Room1;
    }
    public Consts.InputType inputType;
    public Consts.Server server;
}

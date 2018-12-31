using MessagePack;

namespace UNetUI.Asteroids.NetworkedData.Player
{
    [MessagePackObject]
    public class InputSendPackage
    {
        [Key(0)] public float horizontal;
        [Key(1)] public float vertical;
        
        [Key(2)] public float timestamp;
    }
}
using MessagePack;

namespace UNetUI.Asteroids.NetworkedData
{
    [MessagePackObject]
    public class InputSendPackage
    {
        [Key(0)] public float horizontal;
        [Key(1)] public float vertical;

        [Key(2)] public float leftPoint;
        [Key(3)] public float rightPoint;
        [Key(4)] public float topPoint;
        [Key(5)] public float bottomPoint;
        
        [Key(6)] public float timestamp;
    }
}
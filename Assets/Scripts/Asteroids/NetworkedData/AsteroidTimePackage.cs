using MessagePack;

namespace UNetUI.Asteroids.NetworkedData
{
    [MessagePackObject]
    public class AsteroidTimePackage
    {
        [Key(1)] public float timestamp;
    }
}
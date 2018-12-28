using MessagePack;

namespace UNetUI.Asteroids.NetworkedData
{
    [MessagePackObject]
    public class AsteroidPositionPackage
    {
        [Key(0)] public float percentX;
        [Key(1)] public float percentY;

        [Key(2)] public float rotationZ;
        [Key(3)] public float timestamp;
    }
}
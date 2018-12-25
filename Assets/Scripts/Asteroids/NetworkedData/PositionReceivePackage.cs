using MessagePack;

namespace UNetUI.Asteroids.NetworkedData
{
    [MessagePackObject]
    public class PositionReceivePackage
    {
        [Key(0)] public float positionX;
        [Key(1)] public float positionY;

        [Key(2)] public float velocityX;
        [Key(3)] public float velocityY;

        [Key(4)] public float rotationZ;
        [Key(5)] public float roll;

        [Key(6)] public float timestamp;
    }
}
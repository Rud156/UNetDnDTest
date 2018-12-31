using MessagePack;

namespace UNetUI.Asteroids.NetworkedData.Player
{
    [MessagePackObject]
    public class PositionReceivePackage
    {
        [Key(0)] public float percentX;
        [Key(1)] public float percentY;

        [Key(2)] public float velocityX;
        [Key(3)] public float velocityY;

        [Key(4)] public float rotationZ;
        [Key(5)] public float roll;

        [Key(6)] public bool movementAnimation;

        [Key(7)] public float timestamp;
    }
}
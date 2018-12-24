using MessagePack;

namespace UNetUI.Asteroids.NetworkedData
{
    [MessagePackObject]
    public class PositionReceivePackage
    {
        [Key(0)]
        public float positionX;
        [Key(1)]
        public float positionY;
        [Key(2)]
        public float positionZ;

        [Key(3)]
        public float rotationZ;
        [Key(4)]
        public float roll;

        [Key(5)]
        public float timestamp;
    }
}
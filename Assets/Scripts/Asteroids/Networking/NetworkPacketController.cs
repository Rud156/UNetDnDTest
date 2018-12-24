using MessagePack;
using UnityEngine.Networking;

namespace Asteroids.Networking
{
    public class NetworkPacketController : NetworkBehaviour
    {
        [MessagePackObject]
        public class Package
        {
            [Key(0)]
            public float horizontal;
            [Key(1)]
            public float vertical;
            [Key(2)]
            public float timestamp;
        }

        [MessagePackObject]
        public class ReceivePackage
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

        private NetworkPacketManager<Package> _packetManager;

        protected NetworkPacketManager<Package> PacketManager
        {
            get
            {
                if (_packetManager != null)
                    return _packetManager;

                _packetManager = new NetworkPacketManager<Package>();

                if (isLocalPlayer)
                    _packetManager.OnRequirePackageTransmit += TransmitPackageToServer;

                return _packetManager;
            }
        }

        private NetworkPacketManager<ReceivePackage> _serverPacketManager;

        protected NetworkPacketManager<ReceivePackage> ServerPacketManager
        {
            get
            {
                if (_serverPacketManager!= null)
                    return _serverPacketManager;

                _serverPacketManager= new NetworkPacketManager<ReceivePackage>();

                if (isServer)
                    _serverPacketManager.OnRequirePackageTransmit += TransmitPackageToClient;

                return _serverPacketManager;
            }
        }

        private void TransmitPackageToServer(byte[] data) => CmdTransmitPackages(data);

        [Command]
        void CmdTransmitPackages(byte[] data) => PacketManager.ReceiveData(data);

        private void TransmitPackageToClient(byte[] data) => RpcReceiveDataClient(data);

        [ClientRpc]
        void RpcReceiveDataClient(byte[] data) => ServerPacketManager.ReceiveData(data);

        protected virtual void FixedUpdate()
        {
            PacketManager.Tick();
            ServerPacketManager.Tick();
        }
    }
}
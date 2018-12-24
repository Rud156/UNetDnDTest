using MessagePack;
using UnityEngine.Networking;
using UNetUI.Asteroids.NetworkedData;

namespace UNetUI.Asteroids.Networking
{
    public class NetworkPacketController : NetworkBehaviour
    {
        private NetworkPacketManager<InputSendPackage> _packetManager;

        protected NetworkPacketManager<InputSendPackage> PacketManager
        {
            get
            {
                if (_packetManager != null)
                    return _packetManager;

                _packetManager = new NetworkPacketManager<InputSendPackage>();

                if (isLocalPlayer)
                    _packetManager.OnRequirePackageTransmit += TransmitPackageToServer;

                return _packetManager;
            }
        }

        private NetworkPacketManager<PositionReceivePackage> _serverPacketManager;

        protected NetworkPacketManager<PositionReceivePackage> ServerPacketManager
        {
            get
            {
                if (_serverPacketManager!= null)
                    return _serverPacketManager;

                _serverPacketManager= new NetworkPacketManager<PositionReceivePackage>();

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
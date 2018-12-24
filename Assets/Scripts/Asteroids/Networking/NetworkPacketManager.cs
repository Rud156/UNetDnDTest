using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MessagePack;
using UnityEngine;

namespace Asteroids.Networking
{
    public class NetworkPacketManager<T> where T : class
    {
        public event System.Action<byte[]> OnRequirePackageTransmit;

        private float _sendSpeed = 0.2f;

        public float SendSpeed
        {
            private get
            {
                if (_sendSpeed < 0.1f)
                    return _sendSpeed = 0.1f;

                return _sendSpeed;
            }
            set { _sendSpeed = value; }
        }

        private float _nextTick;

        private List<T> _packages;
        private List<T> Packages => _packages ?? (_packages = new List<T>());

        private Queue<T> _receivedPackages;

        public void AddPackage(T package) => Packages.Add(package);

        public void ReceiveData(byte[] data)
        {
            if (_receivedPackages == null)
                _receivedPackages = new Queue<T>();

            T[] packages = ReadBytes(data).ToArray();

            foreach (var package in packages)
                _receivedPackages.Enqueue(package);
        }

        public void Tick()
        {
            _nextTick += 1 / SendSpeed * Time.fixedDeltaTime;
            if (_nextTick > 1 && Packages.Count > 0)
            {
                _nextTick = 0;

                if (OnRequirePackageTransmit != null)
                {
                    byte[] bytes = CreateBytes();
                    Packages.Clear();

                    OnRequirePackageTransmit?.Invoke(bytes);
                }
            }
        }

        public T GetNextDataReceived()
        {
            if (_receivedPackages == null || _receivedPackages.Count == 0)
                return default(T);

            return _receivedPackages.Dequeue();
        }

        private byte[] CreateBytes() => MessagePackSerializer.Serialize(Packages);

        private List<T> ReadBytes(byte[] bytes) => MessagePackSerializer.Deserialize<List<T>>(bytes);
    }
}
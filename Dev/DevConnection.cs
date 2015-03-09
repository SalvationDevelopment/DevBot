using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using DevBot.Dev.Enums;

namespace DevBot.Dev
{
    public class DevConnection
    {
        private TcpClient m_client;
        private BinaryReader m_reader;
        private Thread m_thread;

        private Queue<DevPacket> m_sendQueue;
        private Queue<DevPacket> m_receiveQueue;

        public bool IsConnected { get; private set; }

        public DevConnection(IPAddress address, int port)
        {
            m_sendQueue = new Queue<DevPacket>();
            m_receiveQueue = new Queue<DevPacket>();
            m_client = new TcpClient(address.ToString(), port);
            IsConnected = true;
            m_reader = new BinaryReader(m_client.GetStream());
            m_thread = new Thread(NetworkTick);
            m_thread.Start();
        }

        public void Send(DevPacket packet)
        {
            lock (m_sendQueue)
                m_sendQueue.Enqueue(packet);
        }

        public void Send(DevServerPacket type)
        {
            Send(new DevPacket((int)type));
        }

        public void Send(DevServerPacket type, string data)
        {
            Send(new DevPacket((int)type, data));
        }

        public bool HasPacket()
        {
            lock (m_receiveQueue)
                return m_receiveQueue.Count > 0;
        }

        public DevPacket Receive()
        {
            lock (m_receiveQueue)
            {
                if (m_receiveQueue.Count == 0)
                    return null;
                return m_receiveQueue.Dequeue();
            }
        }

        private void NetworkTick()
        {
            try
            {
                while (IsConnected)
                {
                    if (CheckDisconnected())
                    {
                        IsConnected = false;
                        m_client.Close();
                    }

                    InternalTick();
                    Thread.Sleep(1);
                }
            }
            catch (Exception)
            {
                IsConnected = false;
                m_client.Close();
            }
        }

        private bool CheckDisconnected()
        {
            return (m_client.Client.Poll(1, SelectMode.SelectRead) && m_client.Available == 0);
        }

        private void InternalTick()
        {
            lock (m_sendQueue)
            {
                while (m_sendQueue.Count > 0)
                    InternalSend(m_sendQueue.Dequeue());
            }
            while (m_client.Available > 0)
            {
                DevPacket packet = InternalReceive();
                lock (m_receiveQueue)
                {
                    m_receiveQueue.Enqueue(packet);
                }
            }
        }

        private void InternalSend(DevPacket packet)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(ms);
            writer.Write((byte)packet.Id);
            if (packet.Data != null)
            {
                writer.Write((short) packet.Data.Length);
                writer.Write(packet.Data);
            }
            byte[] data = ms.ToArray();
            m_client.Client.Send(data);
        }

        private DevPacket InternalReceive()
        {
            DevClientPacket id = (DevClientPacket)m_reader.ReadByte();
            int len = 0;
            if (!DevPacket.IsOneByte(id))
            {
                if (DevPacket.IsLarge(id))
                    len = m_reader.ReadInt32();
                else
                    len = m_reader.ReadInt16();
            }
            DevPacket packet;
            if (len > 0)
                packet = new DevPacket((int)id, m_reader.ReadBytes(len));
            else
                packet = new DevPacket((int)id);
            return packet;
        }
    }
}
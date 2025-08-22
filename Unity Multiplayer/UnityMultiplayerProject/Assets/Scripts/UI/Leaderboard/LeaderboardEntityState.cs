using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct LeaderboardEntityState : INetworkSerializable, IEquatable<LeaderboardEntityState>
{
    public ulong clientId;
    public FixedString32Bytes playerName;
    public int coins;
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref coins);
    }

    public bool Equals(LeaderboardEntityState other)
    {
        return clientId == other.clientId && playerName.Equals(other.playerName) && coins == other.coins;
    }
}

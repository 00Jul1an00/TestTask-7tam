using Unity.Netcode;

public struct CharacterNetworkData : INetworkSerializable
{
    public string Name;
    public int CoinsCount;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Name);
        serializer.SerializeValue(ref CoinsCount);
    }
}

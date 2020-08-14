namespace OctoAwesome.Network
{
    public enum OfficialCommand : ushort
    {
        //0 - 100; System-Commdans
        //100 - 200; General-Commands
        Whoami = 11,
        GetUniverse = 12,
        GetPlanet = 13,
        LoadColumn = 14,
        SaveColumn = 15,
        //400 - 500; Entity-Updates
        NewEntity = 401,
        RemoveEntity = 402,
        PositionUpdate = 403
    }
}

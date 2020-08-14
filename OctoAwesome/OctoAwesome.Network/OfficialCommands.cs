namespace OctoAwesome.Network
{
    public enum OfficialCommands : ushort
    {
        //0 - 100; System-Commdans
        //100 - 200; General-Commands
        Whoami = 11,
        GetUniverse = 12,
        GetPlanet = 13,
        LoadColumn = 14,
        SaveColumn = 15,
        //400 - 500; Entity-Updates
        PositionUpdate = 401
    }
}

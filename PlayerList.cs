using System.Collections.Generic;

namespace SE_PlayerGrace
{
    public class PlayersLists
    {
        /// <summary>
        /// All non NPC Players not already on leave
        /// </summary>
        public static List<PlayerData> PlayerList { get; set; } = new List<PlayerData>();

        /// <summary>
        /// All Players in config file currently on leave
        /// </summary>
        public static List<PlayerData> GraceList { get; set; } = new List<PlayerData>();
    }
}
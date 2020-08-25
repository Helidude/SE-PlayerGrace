using NLog;
using Sandbox.Game.World;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SE_PlayerGrace
{
    public class Helpers
    {
        public static readonly Logger Log = LogManager.GetLogger("PlayerGrace");

        // Refresh GraceList for it to reflect the latest changes
        public static List<PlayerData> RefreshGraceList()
        {
            PlayersLists.GraceList.Clear();
            foreach (var data in GracePlugin.Plugin.Config.PlayersOnLeave)
            {
                // Add Players from file to List<PlayerData>
                PlayersLists.GraceList.Add(new PlayerData
                {
                    PlayerId = data.PlayerId,
                    PlayerName = data.PlayerName,
                    GraceGrantedAt = data.GraceGrantedAt,
                    PersistPlayer = data.PersistPlayer
                });
            }

            return PlayersLists.GraceList;
        }

        public static void AddPlayer(PlayerData player)
        {
            GraceControl.UiInstance.Dispatcher.Invoke(() =>
            {
                GracePlugin.Plugin.Config.PlayersOnLeave.Add(new PlayerData
                {
                    PlayerId = player.PlayerId,
                    PlayerName = player.PlayerName,
                    GraceGrantedAt = DateTime.Now,
                    PersistPlayer = player.PersistPlayer
                });

                GracePlugin.Plugin.Save();
            });

            RefreshGraceList();
        }

        public static void RemovePlayer(PlayerData player)
        {
            var itemToRemove = GracePlugin.Plugin.Config.PlayersOnLeave.SingleOrDefault(p => p.PlayerId == player.PlayerId);

            GraceControl.UiInstance.Dispatcher.Invoke(() =>
            {
                GracePlugin.Plugin.Config.PlayersOnLeave.Remove(itemToRemove);
                GracePlugin.Plugin.Save();
            });

            RefreshGraceList();
        }

        // Get all human players not already added to PlayerGrace
        public static List<MyIdentity> GetAllPlayers()
        {
            if (MySession.Static == null)
                return new List<MyIdentity>();

            RefreshGraceList();

            // Populate the list
            var idents = MySession.Static.Players.GetAllIdentities().ToList();
            var npcs = MySession.Static.Players.GetNPCIdentities().ToList();

            // Remove NPCs and already added players in Config
            var result = idents.Where(i => (npcs.All(n => n != i.IdentityId)))
                .OrderBy(i => i.DisplayName)
                .ToList();

            // Remove players already added to PlayerGrace
            return result.Where(i => (PlayersLists.GraceList.All(g => g.PlayerId != i.IdentityId)))
                .OrderBy(i => i.DisplayName)
                .ToList();
        }

        public static long GetPlayerIdByName(string name)
        {
            if (!long.TryParse(name, out long id))
            {
                foreach (var identity in MySession.Static.Players.GetAllIdentities())
                {
                    if (identity.DisplayName == name)
                    {
                        return identity.IdentityId;
                    }
                }
            }

            return 0;
        }
    }
}
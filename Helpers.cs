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

        // Applies the GraceList at server start.
        // Removes players who has logged back in and sets new LastLoginTime to remaining players.
        public static void ApplySession()
        {
            if (GracePlugin.Plugin.Config.PlayersOnLeave == null || MySession.Static == null)
                return;

            foreach (MyIdentity identity in MySession.Static.Players.GetAllIdentities())
            {
                foreach (var playerData in GracePlugin.Plugin.Config.PlayersOnLeave.ToList())
                {
                    if (playerData.PlayerId == identity.IdentityId)
                    {
                        identity.LastLoginTime = DateTime.Now;
                    }

                    // Remove Players that has logged back in
                    if (playerData.PlayerId == identity.IdentityId
                        && identity.LastLogoutTime > playerData.GraceGrantedAt // Player has logged back in
                        && GracePlugin.Plugin.Config.AutoRemove // Global setting
                        && !playerData.PersistPlayer) // Player Setting
                    {
                        GracePlugin.Plugin.Config.PlayersOnLeave.Remove(playerData);
                        GracePlugin.Plugin.Save();
                        Log.Info($"Player {playerData.PlayerName} was removed. Last logout was {identity.LastLogoutTime}");
                    }
                }
            }
        }

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
            GracePlugin.Plugin.Config.PlayersOnLeave.Add(new PlayerData
            {
                PlayerId = player.PlayerId,
                PlayerName = player.PlayerName,
                GraceGrantedAt = DateTime.Now,
                PersistPlayer = player.PersistPlayer
            });

            GracePlugin.Plugin.Save();
            RefreshGraceList();
        }

        public static void RemovePlayer(PlayerData player)
        {
            var itemToRemove = GracePlugin.Plugin.Config.PlayersOnLeave.SingleOrDefault(p => p.PlayerId == player.PlayerId);

            GracePlugin.Plugin.Config.PlayersOnLeave.Remove(itemToRemove);
            GracePlugin.Plugin.Save();
            RefreshGraceList();
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
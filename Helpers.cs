using NLog;
using Sandbox.Game.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE_PlayerGrace
{
    public class Helpers
    {
        public static readonly Logger Log = LogManager.GetCurrentClassLogger();

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

        //public static void RefreshGraceList()
        //{
        //    //if (GracePlugin.Plugin.Config.PlayersOnLeave == null || MySession.Static == null)
        //    //    return;
        //    PlayersLists.GraceList.Clear();

        //    foreach (var player in GracePlugin.Plugin.Config.PlayersOnLeave)
        //    {
        //        // Add Players from file to List<PlayerData>
        //        PlayersLists.GraceList.Add(new PlayerData
        //        {
        //            PlayerId = player.PlayerId,
        //            PlayerName = player.PlayerName,
        //            GraceGrantedAt = player.GraceGrantedAt
        //        });
        //    }

        //    //if (comboboxPlayers != null)
        //    //comboboxPlayers.ItemsSource = GetPlayers.GetAllPlayers().Select(x => x.DisplayName);
        //}

        public static void ApplySession()
        {
            //if (GracePlugin.Plugin.Config.PlayersOnLeave == null || MySession.Static == null)
            //    return;

            // TODO - Rewrite with Linq
            foreach (MyIdentity identity in MySession.Static.Players.GetAllIdentities())
            {
                foreach (var playerData in GracePlugin.Plugin.Config.PlayersOnLeave.ToList())
                {
                    if (playerData.PlayerId == identity.IdentityId)
                    {
                        identity.LastLoginTime = DateTime.Now;
                        Log.Info($"Player {playerData.PlayerName} applied new gracedate. Last logout was {identity.LastLogoutTime}");
                    }

                    // Remove Players that has logged back in
                    if (playerData.PlayerId == identity.IdentityId
                        && identity.LastLogoutTime > playerData.GraceGrantedAt
                        && GracePlugin.Plugin.Config.AutoRemove)
                    {
                        GracePlugin.Plugin.Config.PlayersOnLeave.Remove(playerData);
                        GracePlugin.Plugin.Save();
                        Log.Warn(
                            $"Player {playerData.PlayerName} was removed. Last logout was {identity.LastLogoutTime}");
                    }
                }
            }
        }

        public static List<PlayerData> GraceList()
        {
            PlayersLists.GraceList.Clear();
            foreach (var data in GracePlugin.Plugin.Config.PlayersOnLeave)
            {
                // Add Players from file to List<PlayerData>
                PlayersLists.GraceList.Add(new PlayerData
                {
                    PlayerId = data.PlayerId,
                    PlayerName = data.PlayerName,
                    GraceGrantedAt = data.GraceGrantedAt
                });
            }

            return PlayersLists.GraceList;
        }

        public static void RemovePlayerFromConf(PlayerData player)
        {
            var playerData = GracePlugin.Plugin.Config.PlayersOnLeave.ToList();

            {
                foreach (var data in playerData)
                {
                    if (player.PlayerId == data.PlayerId)
                    {
                        GracePlugin.Plugin.Config.PlayersOnLeave.Remove(data);
                    }
                }

                GracePlugin.Plugin.Save();
                GraceList();
            }
        }

        public static void AddPlayerToConf(PlayerData player)
        {
            {
                GracePlugin.Plugin.Config.PlayersOnLeave.Add(new PlayerData // Add the new player
                {
                    PlayerId = player.PlayerId,
                    PlayerName = player.PlayerName,
                    GraceGrantedAt = DateTime.Now
                });

                GracePlugin.Plugin.Save();
                GraceList();
            }
        }
    }
}
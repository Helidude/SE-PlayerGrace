using NLog;
using System;
using System.Linq;
using Torch.Commands;
using Torch.Commands.Permissions;
using VRage.Game.ModAPI;

namespace SE_PlayerGrace
{
    public class GraceCommands : CommandModule
    {
        public static readonly Logger Log = LogManager.GetLogger("PlayerGrace");

        [Command("grace toggle", "Toggle Global AutoRemove State")]
        [Permission(MyPromoteLevel.SpaceMaster)]
        public void GraceToggle()
        {
            if (GracePlugin.Plugin.Config.AutoRemove)
            {
                GracePlugin.Plugin.Config.AutoRemove = false;
                Context.Respond($"AutoRemove set to false");
            }
            else
            {
                GracePlugin.Plugin.Config.AutoRemove = true;
                Context.Respond($"AutoRemove set to true");
            }

            GracePlugin.Plugin.Save();
        }

        [Command("grace add", "Grant a player extended leave")]
        [Permission(MyPromoteLevel.SpaceMaster)]
        public void GraceAdd(string playerName)
        {
            var playerId = Helpers.GetPlayerIdByName(playerName);

            if (Helpers.RefreshGraceList().Any(i => i.PlayerId == playerId) || playerId == 0)
            {
                Context.Respond($"{playerName} already added or does not exist");
                return;
            }

            // Format input as PlayerData
            Helpers.AddPlayer(new PlayerData // Add the new player
            {
                PlayerId = playerId,
                PlayerName = playerName,
                GraceGrantedAt = DateTime.Now,
                PersistPlayer = false
            });

            Context.Respond($"{playerName} successfully added");
            Log.Info($"{playerName} successfully added");
        }

        [Command("grace add persist", "Grant a player extended leave. Player will not be affected by AutoRemove")]
        [Permission(MyPromoteLevel.SpaceMaster)]
        public void GracePersist(string playerName)
        {
            var playerId = Helpers.GetPlayerIdByName(playerName);

            if (Helpers.RefreshGraceList().Any(i => i.PlayerId == playerId) || playerId == 0)
            {
                Context.Respond($"{playerName} already added or does not exist");
                return;
            }

            // Format input as PlayerData
            Helpers.AddPlayer(new PlayerData // Add the new player
            {
                PlayerId = playerId,
                PlayerName = playerName,
                GraceGrantedAt = DateTime.Now,
                PersistPlayer = true
            }); ;

            Context.Respond($"{playerName} successfully added");
            Log.Info($"{playerName} successfully added");
        }

        [Command("grace remove", "Revoke a players extended leave")]
        [Permission(MyPromoteLevel.SpaceMaster)]
        public void GraceRemove(string playerName)
        {
            var playerId = Helpers.GetPlayerIdByName(playerName);

            if (Helpers.RefreshGraceList().Any(i => i.PlayerId == playerId) != true || playerId == 0)
            {
                Context.Respond($"Could not find player {playerName}");
                return;
            }

            foreach (var playerData in Helpers.RefreshGraceList().ToList())
            {
                if (playerData.PlayerId != playerId) continue;
                Helpers.RemovePlayer(playerData);

                Context.Respond($"Player {playerName} successfully removed");
                Log.Info($"{playerName} successfully removed");
            }
        }

        [Command("grace list", "List players on leave")]
        [Permission(MyPromoteLevel.SpaceMaster)]
        public void GraceList()
        {
            if (Helpers.RefreshGraceList().Count == 0)
            {
                Context.Respond($"Could not find any players. AutoRemove is {GracePlugin.Plugin.Config.AutoRemove}");
                return;
            }

            Context.Respond($"AutoRemove is {GracePlugin.Plugin.Config.AutoRemove}! Players on leave:");
            foreach (var players in Helpers.RefreshGraceList())
            {
                Context.Respond($"{players.PlayerName} ");
            }
        }
    }
}
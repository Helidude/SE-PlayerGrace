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

        [Command("grace add", "Grant a player extended leave")]
        [Permission(MyPromoteLevel.SpaceMaster)]
        public void GraceAdd(string playerName)
        {
            var playerId = Helpers.GetPlayerIdByName(playerName);

            if (Helpers.GraceList().Any(i => i.PlayerId == playerId) || playerId == 0)
            {
                Context.Respond($"{playerName} already added or does not exist");
                return;
            }

            Helpers.AddPlayerToConf(new PlayerData // Add the new player
            {
                PlayerId = playerId,
                PlayerName = playerName,
                GraceGrantedAt = DateTime.Now,
                PersistPlayer = false
            });

            Context.Respond($"{playerName} successfully added");
            Log.Info($"{playerName} successfully added");
        }

        [Command("grace add persist", "Grant a player extended leave that will not be automaticly removed (if enabled)")]
        [Permission(MyPromoteLevel.SpaceMaster)]
        public void GracePersist(string playerName)
        {
            var playerId = Helpers.GetPlayerIdByName(playerName);

            if (Helpers.GraceList().Any(i => i.PlayerId == playerId) || playerId == 0)
            {
                Context.Respond($"{playerName} already added or does not exist");
                return;
            }

            Helpers.AddPlayerToConf(new PlayerData // Add the new player
            {
                PlayerId = playerId,
                PlayerName = playerName,
                GraceGrantedAt = DateTime.Now,
                PersistPlayer = true
            }); ;

            Context.Respond($"{playerName} successfully added");
            Log.Info($"{playerName} successfully added");
        }

        [Command("grace remove", "Revoke players extended leave")]
        [Permission(MyPromoteLevel.SpaceMaster)]
        public void GraceRemove(string playerName)
        {
            var playerId = Helpers.GetPlayerIdByName(playerName);

            if (Helpers.GraceList().Any(i => i.PlayerId == playerId) != true || playerId == 0)
            {
                Context.Respond($"Could not find player {playerName}");
                return;
            }

            foreach (var playerData in PlayersLists.GraceList.ToList())
            {
                if (playerData.PlayerId != playerId) continue;
                Helpers.RemovePlayerFromConf(playerData);
                Context.Respond($"Player {playerName} successfully removed");
                Log.Info($"{playerName} successfully removed");
            }
        }

        [Command("grace list", "List players on leave")]
        [Permission(MyPromoteLevel.SpaceMaster)]
        public void GraceList()
        {
            if (Helpers.GraceList().Count == 0)
            {
                Context.Respond($"Could not find any players");
                return;
            }

            Context.Respond($"Players on leave:");
            foreach (var players in Helpers.GraceList())
            {
                Context.Respond($"{players.PlayerName} ");
            }
        }
    }
}
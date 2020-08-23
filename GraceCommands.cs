﻿using NLog;
using System;
using System.Linq;
using Torch.Commands;
using Torch.Commands.Permissions;
using VRage.Game.ModAPI;

namespace SE_PlayerGrace
{
    public class GraceCommands : CommandModule
    {
        public static readonly Logger Log = LogManager.GetLogger("GraceCommands");

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
                GraceGrantedAt = DateTime.Now
            });

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
            }
        }
    }
}
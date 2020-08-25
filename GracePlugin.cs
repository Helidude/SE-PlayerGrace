using NLog;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using Torch;
using Torch.API;
using Torch.API.Plugins;
using Torch.API.Session;
using Torch.API.Managers;
using Torch.Session;
using System.Threading;
using Sandbox.Game.World;
using System.Linq;

namespace SE_PlayerGrace
{
    public class GracePlugin : TorchPluginBase, IWpfPlugin
    {
        public static readonly Logger Log = LogManager.GetLogger("PlayerGrace");

        public static GracePlugin Plugin { get; set; }

        private GraceControl _control;

        public UserControl GetControl() => _control ?? (_control = new GraceControl(this));

        // Include config
        private Persistent<GraceConfig> _config;

        public GraceConfig Config => _config?.Data;

        public override void Init(ITorchBase torch)
        {
            base.Init(torch);

            var sessionManager = Torch.Managers.GetManager<TorchSessionManager>();
            if (sessionManager != null)
                sessionManager.SessionStateChanged += SessionChanged;
            else
                Log.Warn("No session manager loaded!");

            SetupConfig();
            Plugin = this; // Needed for NoGui
        }

        private void SessionChanged(ITorchSession session, TorchSessionState state)
        {
            switch (state)
            {
                // Runs when game is fully loaded
                case TorchSessionState.Loaded:
                    Task.Run((Action)(() =>
                    {
                        Thread.Sleep(5000);
                        ApplySession();
                    }));
                    break;

                // Runs when server is shutting down
                case TorchSessionState.Unloading:
                    break;
            }
        }

        private void SetupConfig()
        {
            var configFile = Path.Combine(StoragePath, "PlayerGrace.cfg");

            try
            {
                _config = Persistent<GraceConfig>.Load(configFile);
            }
            catch (Exception e)
            {
                Log.Warn(e);
            }

            if (_config?.Data == null)
            {
                Log.Info("Create Default Config, because none was found!");
                _config = new Persistent<GraceConfig>(configFile, new GraceConfig());
                _config.Save();
            }
        }

        public void Save()
        {
            try
            {
                _config.Save();
                Log.Info("Configuration Saved.");
            }
            catch (IOException e)
            {
                Log.Warn(e, "Configuration failed to save");
            }
        }

        // Applies the GraceList at server start.
        // Removes players who has logged back in and sets new LastLoginTime to remaining players.
        private void ApplySession()
        {
            if (Plugin.Config.PlayersOnLeave == null || MySession.Static == null)
                return;

            foreach (var identity in MySession.Static.Players.GetAllIdentities())
            {
                foreach (var playerData in Plugin.Config.PlayersOnLeave.ToList())
                {
                    // Apply current DateTime for players in PlayerGrace
                    if (playerData.PlayerId == identity.IdentityId)
                        identity.LastLoginTime = DateTime.Now;

                    // Remove Players that has logged back in (if matching criteria)
                    if (playerData.PlayerId == identity.IdentityId
                        && identity.LastLogoutTime > playerData.GraceGrantedAt // Player has logged back in
                        && Plugin.Config.AutoRemove     // Global Setting
                        && !playerData.PersistPlayer)   // Player Setting
                    {
                        if (Torch.Config.NoGui)
                        {
                            Plugin.Config.PlayersOnLeave.Remove(playerData);
                            Plugin.Save();
                            Log.Info($"Player {playerData.PlayerName} removed!");
                            break;
                        }

                        GraceControl.UiInstance.Dispatcher.Invoke(() => // Make sure this runs on UI thread
                        {
                            Plugin.Config.PlayersOnLeave.Remove(playerData);
                            Plugin.Save();
                            Log.Info($"Player {playerData.PlayerName} removed!");
                        });
                    }
                }
            }
        }
    }
}
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

namespace SE_PlayerGrace
{
    public class GracePlugin : TorchPluginBase, IWpfPlugin
    {
        // Attach NLog
        public static readonly Logger Log = LogManager.GetLogger("PlayerGrace");

        private GraceControl _control;

        public UserControl GetControl() => _control ?? (_control = new GraceControl(this));

        public static GracePlugin Plugin { get; set; }

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
            Plugin = this;
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
                        Helpers.RefreshGraceList();
                        Helpers.ApplySession();
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
    }
}
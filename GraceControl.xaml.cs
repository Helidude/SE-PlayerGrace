using NLog;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SE_PlayerGrace
{
    public partial class GraceControl : UserControl
    {
        public static GraceControl UiInstance { get; private set; }

        public static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private PlayerData _selectedDatagridRow = null;
        private static ComboBox comboboxPlayers;

        //public static GraceControl PluginInstance { get; set; }

        public GraceControl()
        {
            InitializeComponent();
            UiInstance = this;
        }

        public GraceControl(GracePlugin plugin) : this()
        {
            GracePlugin.Plugin = plugin;
            DataContext = plugin.Config;
        }

        private void ComboBoxPlayers_OnLoaded(object sender, RoutedEventArgs e)
        {
            // Gets all players on server and loads them into ComboBox
            comboboxPlayers = sender as ComboBox;
            if (comboboxPlayers != null)
                comboboxPlayers.ItemsSource = Helpers.GetAllPlayers().Select(x => x.DisplayName);
        }

        private void SaveConfig_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedPlayer = ComboBoxPlayers.SelectedValue.ToString();
            var playerId = Helpers.GetPlayerIdByName(selectedPlayer);

            Helpers.AddPlayer(new PlayerData // Add the new player
            {
                PlayerId = playerId,
                PlayerName = selectedPlayer,
                GraceGrantedAt = DateTime.Now,
                PersistPlayer = false
            });

            ComboBoxPlayers.SelectedIndex = -1;
            comboboxPlayers.ItemsSource = Helpers.GetAllPlayers().Select(x => x.DisplayName);
        }

        private void Button_Click_Delete(object sender, RoutedEventArgs e)
        {
            if (_selectedDatagridRow == null)
                return;

            var itemToRemove = _selectedDatagridRow;
            if (itemToRemove != null)
                GracePlugin.Plugin.Config.PlayersOnLeave.Remove(itemToRemove);

            ButtonDelete.IsEnabled = false;

            GracePlugin.Plugin.Save();
            ComboBoxPlayers.SelectedIndex = -1;
            comboboxPlayers.ItemsSource = Helpers.GetAllPlayers().Select(x => x.DisplayName);
        }

        private void ComboBoxPlayers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonSave.IsEnabled = true;

            ComboBox cb = sender as ComboBox;
            if (cb.SelectedValue == null)
            {
                ButtonSave.IsEnabled = false;
                return;
            }

            var playerName = cb.SelectedValue.ToString();

            ButtonSave.IsEnabled = true;
            ButtonDelete.IsEnabled = false;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            var pd = dg.SelectedItem as PlayerData;

            if (pd != null)
            {
                _selectedDatagridRow = pd;
            }

            ButtonDelete.IsEnabled = true;
            ButtonSave.IsEnabled = false;
        }
    }
}
using System;
using System.Collections.ObjectModel;
using Torch;

namespace SE_PlayerGrace
{
    public class GraceConfig : ViewModel
    {
        private bool _autoRemove;
        public bool AutoRemove { get => _autoRemove; set => SetValue(ref _autoRemove, value); }

        public ObservableCollection<PlayerData> PlayersOnLeave { get; } = new ObservableCollection<PlayerData>();
    }

    public class PlayerData : ViewModel
    {
        private long _playerId;
        public long PlayerId { get => _playerId; set => SetValue(ref _playerId, value); }

        private string _playerName;
        public string PlayerName { get => _playerName; set => SetValue(ref _playerName, value); }

        private DateTime _playerGraceGrantedAt;
        public DateTime GraceGrantedAt { get => _playerGraceGrantedAt; set => SetValue(ref _playerGraceGrantedAt, value); }

        private bool _persistPlayer;
        public bool PersistPlayer { get => _persistPlayer; set => SetValue(ref _persistPlayer, value); }
    }
}
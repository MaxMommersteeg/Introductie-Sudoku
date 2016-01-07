using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SudokuBasis;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System;

namespace Sudoku_WPF.ViewModel {
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        /// 
        private SudokuGame _sudoku;

        public ObservableCollection<Location> Locations { get; set; }
        public short CurrentValue { get; set; }

        public ICommand NewGameCommand { get; private set; }
        public ICommand CheatCommand { get; private set; }
        public ICommand CheckCommand { get; private set; }

        public MainViewModel() {
            _sudoku = new SudokuGame();

            NewGame();

            NewGameCommand = new RelayCommand(NewGame, CanNewGame);
            CheckCommand = new RelayCommand(CheckGame, CanCheckGame);
            CheatCommand = new RelayCommand(CheatGame, CanCheatGame);
        }

        public bool CanNewGame() {
            return true;
        }

        public bool CanCheckGame() {
            return true;
        }

        public bool CanCheatGame() {
            return true;
        }

        public void NewGame() {
            //Let class library create a new game
            _sudoku.NewGame();
            //Iterate over all existing locations
            var locations = new List<Location>();
            for (int i = 0; i < 81; i++) {
                var p = new Position();
                p.X = Convert.ToInt16(((i / 9) + 1));
                p.Y = Convert.ToInt16(((i % 9) + 1));
                _sudoku.GetValue(p);
                //Lock default values
                p.IsEditable = p.Value == 0;
                locations.Add(new Location(_sudoku, p));
            }
            Locations = new ObservableCollection<Location>(locations);
            //Make notification of changed locations
            RaisePropertyChanged("Locations");
        }

        public void CheckGame() {
            //Check if current board values are correct, notify user with a messagebox
            string message;
            message = _sudoku.IsValid() ? "De huidige cijfers kloppen." : "�en of meerder cijfers zijn incorrect.";
            MessageBox.Show(message, "Spel controle");
        }

        public void CheatGame() {
            //var pos = _sudoku.GetHint();
            int unsolvedCount = 0;
            foreach (var l in Locations) {
                if (l.Value != 0)
                    continue;
                unsolvedCount++;
            }

            //Alles behalve 2 moet opgelost worden
            int solveCount = unsolvedCount - 2;
            for (int x = 0; x < solveCount; x++) {
                var position = _sudoku.GetHint();
                var loc = Locations.Where(l => l.X == position.X && l.Y == position.Y).First();
                loc.Value = position.Value;
            }
        }
    }
}
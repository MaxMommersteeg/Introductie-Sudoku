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

            #region Commands

            NewGameCommand = new RelayCommand(NewGame, CanNewGame);
            CheckCommand = new RelayCommand(CheckGame, CanCheckGame);
            CheatCommand = new RelayCommand(CheatGame, CanCheatGame);

            #endregion
        }

        #region ValidateCommands

        public bool CanNewGame() {
            return true;
        }

        public bool CanCheckGame() {
            return true;
        }

        public bool CanCheatGame() {
            return true;
        }

        #endregion

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
            int toSolve = Locations.Where(x => x.Value == 0).Count() - 2;
            for (int i = 0; i < toSolve; i++) {
                //Ask for a hint
                var position = _sudoku.GetHint();
                //Check at what location the hint is positioned
                //We are always able to grab the first, since it is a hint from the dll
                var loc = Locations.Where(x => x.X == position.X && x.Y == position.Y).First();
                loc.Value = position.Value;
            }
        }
    }
}
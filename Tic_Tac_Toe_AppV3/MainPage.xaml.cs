using Plugin.Maui.Audio;

namespace Tic_Tac_Toe_App
{
    public partial class MainPage : ContentPage
    {
        private readonly TicTacToeGame _game = new();
        private readonly IAudioManager _audioManager;

        private IAudioPlayer? _humanPlayer;
        private IAudioPlayer? _computerPlayer;

        // Equivalent to the Android "computerTurn" flag: prevents the human from
        // moving while the computer's delayed move is pending.
        private bool _computerTurn;
        private bool _gameOver;

        public DifficultyLevel DifficultyLevel
        {
            get => _game.DifficultyLevel;
            set
            {
                _game.DifficultyLevel = value;
                DifficultyLabel.Text = $"Difficulty: {value}";
            }
        }

        public MainPage()
        {
            InitializeComponent();

            _audioManager = AudioManager.Current;

            Board.SetGame(_game);

            StartNewGame();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadSounds();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ReleaseSounds();
        }

        // Equivalent to onResume(): create the MediaPlayer-like objects for the sound effects.
        private void LoadSounds()
        {
            try
            {
                _humanPlayer ??= _audioManager.CreatePlayer(
                    FileSystem.OpenAppPackageFileAsync("sonido_perro.m4a").GetAwaiter().GetResult());

                _computerPlayer ??= _audioManager.CreatePlayer(
                    FileSystem.OpenAppPackageFileAsync("gato.m4a").GetAwaiter().GetResult());
            }
            catch
            {
                // Sound files not present yet - the game still works silently.
            }
        }

        // Equivalent to onPause(): release the MediaPlayer-like objects.
        private void ReleaseSounds()
        {
            _humanPlayer?.Dispose();
            _humanPlayer = null;

            _computerPlayer?.Dispose();
            _computerPlayer = null;
        }

        private void StartNewGame()
        {
            _gameOver = false;
            _computerTurn = false;

            _game.ClearBoard();
            Board.Invalidate();

            StatusLabel.Text = "Your turn (X)";
            PlayAgainBtn.IsVisible = false;
        }

        private void OnResetClicked(object sender, EventArgs e)
        {
            StartNewGame();
        }

        private async void OnDifficultyClicked(object sender, EventArgs e)
        {
            string easy = "Easy";
            string harder = "Harder";
            string expert = "Expert";

            // Show the current selection by putting it first isn't quite the same as a
            // native single-choice dialog, but DisplayActionSheet is the MAUI equivalent
            // of AlertDialog.Builder.setSingleChoiceItems for presenting a list of choices.
            string current = DifficultyLevel switch
            {
                DifficultyLevel.Easy => easy,
                DifficultyLevel.Harder => harder,
                _ => expert
            };

            string choice = await DisplayActionSheet(
                $"Choose difficulty (current: {current})",
                "Cancel",
                null,
                easy, harder, expert);

            if (choice == easy)
                DifficultyLevel = DifficultyLevel.Easy;
            else if (choice == harder)
                DifficultyLevel = DifficultyLevel.Harder;
            else if (choice == expert)
                DifficultyLevel = DifficultyLevel.Expert;
            else
                return; // Cancelled

            await DisplayAlert("Difficulty", $"Difficulty set to {choice}", "OK");
        }

        private async void OnAboutClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AboutPage());
        }

        private async void OnQuitClicked(object sender, EventArgs e)
        {
            bool confirmed = await DisplayAlert("Quit", "Are you sure you want to quit?", "Yes", "No");

            if (confirmed)
                Application.Current?.Quit();
        }

        // Handles taps on the BoardView (equivalent to the Android OnTouchListener that
        // calculates row/col from the touch coordinates).
        private void OnBoardTapped(object sender, TappedEventArgs e)
        {
            if (_gameOver || _computerTurn)
                return;

            var position = e.GetPosition(Board);
            if (position is null)
                return;

            int cellWidth = Board.GetBoardCellWidth();
            int cellHeight = Board.GetBoardCellHeight();

            if (cellWidth <= 0 || cellHeight <= 0)
                return;

            int col = (int)(position.Value.X / cellWidth);
            int row = (int)(position.Value.Y / cellHeight);

            col = Math.Clamp(col, 0, 2);
            row = Math.Clamp(row, 0, 2);

            int location = row * 3 + col;

            HandleHumanMove(location);
        }

        private void HandleHumanMove(int location)
        {
            if (!_game.SetMove(TicTacToeGame.HumanPlayer, location))
                return; // Illegal move (spot occupied) - no sound, no board update.

            Board.Invalidate();
            _humanPlayer?.Play();

            int result = _game.CheckForWinner();
            if (result != 0)
            {
                EndGame(result);
                return;
            }

            // Computer's turn: block human input, show status, and wait ~1 second
            // without blocking the UI thread (equivalent to Handler.postDelayed).
            _computerTurn = true;
            StatusLabel.Text = "Computer is thinking...";

            ScheduleComputerMove();
        }

        private async void ScheduleComputerMove()
        {
            await Task.Delay(1000);

            // If the page was navigated away from or the game ended in the meantime, bail out.
            if (_gameOver)
                return;

            int move = _game.GetComputerMove();
            if (move != -1)
            {
                Board.Invalidate();
                _computerPlayer?.Play();
            }

            int result = _game.CheckForWinner();
            if (result != 0)
            {
                EndGame(result);
                return;
            }

            _computerTurn = false;
            StatusLabel.Text = "Your turn (X)";
        }

        private void EndGame(int result)
        {
            _gameOver = true;
            _computerTurn = false;

            StatusLabel.Text = result switch
            {
                1 => "It's a tie!",
                2 => $"{TicTacToeGame.HumanPlayer} wins!",
                3 => $"{TicTacToeGame.ComputerPlayer} wins!",
                _ => "There is a logic problem!"
            };

            PlayAgainBtn.IsVisible = true;
        }
    }
}

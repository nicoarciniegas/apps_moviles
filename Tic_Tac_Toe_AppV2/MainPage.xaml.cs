namespace Tic_Tac_Toe_App
{
    public enum DifficultyLevel { Easy, Harder, Expert }

    public partial class MainPage : ContentPage
    {
        private const char HumanPlayer = 'X';
        private const char ComputerPlayer = 'O';
        private const int BoardSize = 9;

        private readonly char[] _board = new char[BoardSize];
        private readonly Button[] _buttons;
        private readonly Random _rand = new();
        private bool _gameOver;

        private DifficultyLevel _difficultyLevel = DifficultyLevel.Expert;

        public DifficultyLevel DifficultyLevel
        {
            get => _difficultyLevel;
            set
            {
                _difficultyLevel = value;
                DifficultyLabel.Text = $"Difficulty: {value}";
            }
        }

        public MainPage()
        {
            InitializeComponent();

            _buttons = new[]
            {
                Btn0, Btn1, Btn2,
                Btn3, Btn4, Btn5,
                Btn6, Btn7, Btn8
            };

            StartNewGame();
        }

        private void StartNewGame()
        {
            _gameOver = false;

            for (int i = 0; i < BoardSize; i++)
            {
                _board[i] = ' ';
                _buttons[i].Text = string.Empty;
                _buttons[i].TextColor = Colors.Black;
                _buttons[i].IsEnabled = true;
            }

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

        private async void OnCellClicked(object sender, EventArgs e)
        {
            if (_gameOver)
                return;

            var button = (Button)sender;
            int index = Array.IndexOf(_buttons, button);

            if (index == -1 || _board[index] == HumanPlayer || _board[index] == ComputerPlayer)
                return;

            // Human move
            MakeMove(index, HumanPlayer);

            int result = CheckForWinner();
            if (result != 0)
            {
                EndGame(result);
                return;
            }

            StatusLabel.Text = "Computer is thinking...";

            // Small delay so the human move is visible before the computer moves
            await Task.Delay(300);

            GetComputerMove();

            result = CheckForWinner();
            if (result != 0)
            {
                EndGame(result);
                return;
            }

            StatusLabel.Text = "Your turn (X)";
        }

        private void MakeMove(int index, char player)
        {
            _board[index] = player;
            _buttons[index].Text = player.ToString();
            _buttons[index].TextColor = player == HumanPlayer ? Colors.Green : Colors.Red;
            _buttons[index].IsEnabled = false;
        }

        private void EndGame(int result)
        {
            _gameOver = true;

            foreach (var button in _buttons)
                button.IsEnabled = false;

            StatusLabel.Text = result switch
            {
                1 => "It's a tie!",
                2 => $"{HumanPlayer} wins!",
                3 => $"{ComputerPlayer} wins!",
                _ => "There is a logic problem!"
            };

            PlayAgainBtn.IsVisible = true;
        }

        // Check for a winner. Return
        //  0 if no winner or tie yet
        //  1 if it's a tie
        //  2 if X won
        //  3 if O won
        private int CheckForWinner()
        {
            // Check horizontal wins
            for (int i = 0; i <= 6; i += 3)
            {
                if (_board[i] == HumanPlayer && _board[i + 1] == HumanPlayer && _board[i + 2] == HumanPlayer)
                    return 2;
                if (_board[i] == ComputerPlayer && _board[i + 1] == ComputerPlayer && _board[i + 2] == ComputerPlayer)
                    return 3;
            }

            // Check vertical wins
            for (int i = 0; i <= 2; i++)
            {
                if (_board[i] == HumanPlayer && _board[i + 3] == HumanPlayer && _board[i + 6] == HumanPlayer)
                    return 2;
                if (_board[i] == ComputerPlayer && _board[i + 3] == ComputerPlayer && _board[i + 6] == ComputerPlayer)
                    return 3;
            }

            // Check diagonal wins
            if ((_board[0] == HumanPlayer && _board[4] == HumanPlayer && _board[8] == HumanPlayer) ||
                (_board[2] == HumanPlayer && _board[4] == HumanPlayer && _board[6] == HumanPlayer))
                return 2;

            if ((_board[0] == ComputerPlayer && _board[4] == ComputerPlayer && _board[8] == ComputerPlayer) ||
                (_board[2] == ComputerPlayer && _board[4] == ComputerPlayer && _board[6] == ComputerPlayer))
                return 3;

            // Check for tie
            for (int i = 0; i < BoardSize; i++)
            {
                if (_board[i] != HumanPlayer && _board[i] != ComputerPlayer)
                    return 0;
            }

            return 1;
        }

        private void GetComputerMove()
        {
            int move = -1;

            if (DifficultyLevel == DifficultyLevel.Easy)
            {
                move = GetRandomMove();
            }
            else if (DifficultyLevel == DifficultyLevel.Harder)
            {
                move = GetWinningMove();
                if (move == -1)
                    move = GetRandomMove();
            }
            else if (DifficultyLevel == DifficultyLevel.Expert)
            {
                move = GetWinningMove();
                if (move == -1)
                    move = GetBlockingMove();
                if (move == -1)
                    move = GetRandomMove();
            }

            if (move != -1)
                MakeMove(move, ComputerPlayer);
        }

        // Returns a move that lets the computer win, or -1 if there isn't one.
        // Leaves the board unchanged.
        private int GetWinningMove()
        {
            for (int i = 0; i < BoardSize; i++)
            {
                if (_board[i] != HumanPlayer && _board[i] != ComputerPlayer)
                {
                    char curr = _board[i];
                    _board[i] = ComputerPlayer;
                    bool wins = CheckForWinner() == 3;
                    _board[i] = curr;

                    if (wins)
                        return i;
                }
            }

            return -1;
        }

        // Returns a move that blocks the human from winning, or -1 if there isn't one.
        // Leaves the board unchanged.
        private int GetBlockingMove()
        {
            for (int i = 0; i < BoardSize; i++)
            {
                if (_board[i] != HumanPlayer && _board[i] != ComputerPlayer)
                {
                    char curr = _board[i];
                    _board[i] = HumanPlayer;
                    bool blocksWin = CheckForWinner() == 2;
                    _board[i] = curr;

                    if (blocksWin)
                        return i;
                }
            }

            return -1;
        }

        // Returns a random empty spot on the board.
        private int GetRandomMove()
        {
            int move;
            do
            {
                move = _rand.Next(BoardSize);
            } while (_board[move] == HumanPlayer || _board[move] == ComputerPlayer);

            return move;
        }
    }
}

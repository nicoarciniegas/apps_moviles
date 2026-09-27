namespace Tic_Tac_Toe_App
{
    public enum DifficultyLevel { Easy, Harder, Expert }

    /// <summary>
    /// Encapsulates all Tic-Tac-Toe game rules and AI logic, independent of any UI.
    /// Equivalent to TicTacToeGame.java from the original Android workshop.
    /// </summary>
    public class TicTacToeGame
    {
        public const char HumanPlayer = 'X';
        public const char ComputerPlayer = 'O';
        public const char OpenSpot = ' ';
        public const int BoardSize = 9;

        private readonly char[] _board = new char[BoardSize];
        private readonly Random _rand = new();

        public DifficultyLevel DifficultyLevel { get; set; } = DifficultyLevel.Expert;

        public TicTacToeGame()
        {
            ClearBoard();
        }

        public void ClearBoard()
        {
            for (int i = 0; i < BoardSize; i++)
                _board[i] = OpenSpot;
        }

        public char GetBoardOccupant(int location) => _board[location];

        // Attempts to set the given player at the given location.
        // Returns true if the move was legal (spot was open), false otherwise.
        public bool SetMove(char player, int location)
        {
            if (location < 0 || location >= BoardSize || _board[location] != OpenSpot)
                return false;

            _board[location] = player;
            return true;
        }

        // Check for a winner. Return
        //  0 if no winner or tie yet
        //  1 if it's a tie
        //  2 if X won
        //  3 if O won
        public int CheckForWinner()
        {
            for (int i = 0; i <= 6; i += 3)
            {
                if (_board[i] == HumanPlayer && _board[i + 1] == HumanPlayer && _board[i + 2] == HumanPlayer)
                    return 2;
                if (_board[i] == ComputerPlayer && _board[i + 1] == ComputerPlayer && _board[i + 2] == ComputerPlayer)
                    return 3;
            }

            for (int i = 0; i <= 2; i++)
            {
                if (_board[i] == HumanPlayer && _board[i + 3] == HumanPlayer && _board[i + 6] == HumanPlayer)
                    return 2;
                if (_board[i] == ComputerPlayer && _board[i + 3] == ComputerPlayer && _board[i + 6] == ComputerPlayer)
                    return 3;
            }

            if ((_board[0] == HumanPlayer && _board[4] == HumanPlayer && _board[8] == HumanPlayer) ||
                (_board[2] == HumanPlayer && _board[4] == HumanPlayer && _board[6] == HumanPlayer))
                return 2;

            if ((_board[0] == ComputerPlayer && _board[4] == ComputerPlayer && _board[8] == ComputerPlayer) ||
                (_board[2] == ComputerPlayer && _board[4] == ComputerPlayer && _board[6] == ComputerPlayer))
                return 3;

            for (int i = 0; i < BoardSize; i++)
            {
                if (_board[i] == OpenSpot)
                    return 0;
            }

            return 1;
        }

        // Picks the computer's move according to the current DifficultyLevel and
        // applies it to the board. Returns the chosen location, or -1 if the board is full.
        public int GetComputerMove()
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
                _board[move] = ComputerPlayer;

            return move;
        }

        // Returns a move that lets the computer win, or -1 if there isn't one.
        // Leaves the board unchanged.
        private int GetWinningMove()
        {
            for (int i = 0; i < BoardSize; i++)
            {
                if (_board[i] == OpenSpot)
                {
                    _board[i] = ComputerPlayer;
                    bool wins = CheckForWinner() == 3;
                    _board[i] = OpenSpot;

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
                if (_board[i] == OpenSpot)
                {
                    _board[i] = HumanPlayer;
                    bool blocksWin = CheckForWinner() == 2;
                    _board[i] = OpenSpot;

                    if (blocksWin)
                        return i;
                }
            }

            return -1;
        }

        // Returns a random empty spot on the board, or -1 if the board is full.
        private int GetRandomMove()
        {
            if (!HasOpenSpot())
                return -1;

            int move;
            do
            {
                move = _rand.Next(BoardSize);
            } while (_board[move] != OpenSpot);

            return move;
        }

        private bool HasOpenSpot()
        {
            for (int i = 0; i < BoardSize; i++)
            {
                if (_board[i] == OpenSpot)
                    return true;
            }

            return false;
        }
    }
}

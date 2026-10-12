using Microsoft.Maui.Graphics.Platform;

namespace Tic_Tac_Toe_App
{
    /// <summary>
    /// Custom drawable board, equivalent to Android's BoardView (View + onDraw + Canvas + Paint).
    /// Draws the 3x3 grid lines and the X/O images based on the TicTacToeGame state.
    /// Does not contain any game rules; it is purely responsible for the graphical representation.
    /// </summary>
    public class BoardView : GraphicsView, IDrawable
    {
        public const int GridWidth = 6;

        private TicTacToeGame? _game;

        private readonly Microsoft.Maui.Graphics.IImage? _humanImage;
        private readonly Microsoft.Maui.Graphics.IImage? _computerImage;

        public BoardView()
        {
            Drawable = this;

            // Load the X/O images from Resources\Images (equivalent to BitmapFactory.decodeResource).
            _humanImage = LoadImage("imagen_x.png");
            _computerImage = LoadImage("imagen_o.png");
        }

        private static Microsoft.Maui.Graphics.IImage? LoadImage(string fileName)
        {
            try
            {
                using var stream = FileSystem.OpenAppPackageFileAsync(fileName).GetAwaiter().GetResult();
                return PlatformImage.FromStream(stream);
            }
            catch
            {
                // Image not found - the board will simply draw text as a fallback.
                return null;
            }
        }

        public void SetGame(TicTacToeGame game)
        {
            _game = game;
        }

        public int GetBoardCellWidth() => (int)(Width / 3);

        public int GetBoardCellHeight() => (int)(Height / 3);

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            float boardWidth = dirtyRect.Width;
            float boardHeight = dirtyRect.Height;

            float cellWidth = boardWidth / 3f;
            float cellHeight = boardHeight / 3f;

            // Draw the grid lines
            canvas.StrokeColor = Colors.LightGray;
            canvas.StrokeSize = GridWidth;

            canvas.DrawLine(cellWidth, 0, cellWidth, boardHeight);
            canvas.DrawLine(cellWidth * 2, 0, cellWidth * 2, boardHeight);

            canvas.DrawLine(0, cellHeight, boardWidth, cellHeight);
            canvas.DrawLine(0, cellHeight * 2, boardWidth, cellHeight * 2);

            if (_game is null)
                return;

            // Draw the X's and O's
            for (int i = 0; i < TicTacToeGame.BoardSize; i++)
            {
                int col = i % 3;
                int row = i / 3;

                float left = col * cellWidth + GridWidth;
                float top = row * cellHeight + GridWidth;
                float right = (col + 1) * cellWidth - GridWidth;
                float bottom = (row + 1) * cellHeight - GridWidth;

                var occupant = _game.GetBoardOccupant(i);

                if (occupant == TicTacToeGame.HumanPlayer)
                    DrawMark(canvas, _humanImage, "X", Colors.Green, left, top, right, bottom);
                else if (occupant == TicTacToeGame.ComputerPlayer)
                    DrawMark(canvas, _computerImage, "O", Colors.Red, left, top, right, bottom);
            }
        }

        private static void DrawMark(ICanvas canvas, Microsoft.Maui.Graphics.IImage? image, string fallbackText, Color fallbackColor,
            float left, float top, float right, float bottom)
        {
            if (image is not null)
            {
                canvas.DrawImage(image, left, top, right - left, bottom - top);
            }
            else
            {
                // Fallback: draw colored text if the image resource wasn't found.
                canvas.FontColor = fallbackColor;
                canvas.FontSize = (bottom - top) * 0.7f;
                canvas.DrawString(fallbackText, left, top, right - left, bottom - top,
                    HorizontalAlignment.Center, VerticalAlignment.Center);
            }
        }
    }
}

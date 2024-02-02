using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace chessGame
{
    public abstract class Pieces
    {
        public int pieceID;
        public string name { get; set; }
        // Pole odpowiadajace za kolor figury: 1 - bialy, -1 - czarny
        public int color { get; set; }
        public char positionX { get; set; }
        public int positionY { get; set; }

        public int columnX { get; set; }
        public int rowY { get; set; }
        public Pieces(int pieceID, int color, string name, char positionX, int positionY)
        {
            this.pieceID = pieceID;
            this.color = color;
            this.name = name;
            this.positionX = positionX;
            this.positionY = positionY;
        }
        // Funkcja do przeksztalcania pozycji na szachownicy do indeksow odpowiadajacej tablicy
        public (int, int) Position2Index(char positionX, int positionY)
        {
            int column = Convert.ToInt32(positionX) - 97;
            int row = 8 - positionY;
            return (row, column);
        }
        // Funkcja do przeksztalcania indeksow tablicy na pozycje na szachownicy
        public (char, int) Index2Position(int row, int column)
        {
            char positionX = Convert.ToChar(column + 97);
            int positionY = 8 - row;
            return (positionX, positionY);
        }
        // Funkcja do ustawiania koloru figur
        public void ColorSet(int color)
        {
            if (color == 1)
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
            }
        }
        // Funkcja do sprawdzania czy mozna uzyc figury danego koloru
        public void IsColorRight(int color)
        {
            if (color != this.color)
                throw new Exception("You can't move opponent's pieces!");
        }
        // Funkcja do sprawdzania czy figura nie znajdzie sie poza szachownica
        public void IsOutsideBoard(char positionX, int positionY)
        {
            (int nextRow, int nextColumn) = Position2Index(positionX, positionY);
            if (nextColumn > 7 || nextColumn < 0 || nextRow > 7 || nextRow < 0)
                throw new Exception("Wrong move - outside the board");
        }
        // Funkcja pomocnicza do przesuwania figury na nowe miejsce
        public void MoveSwap(Board board, int row, int nextRow, int column, int nextColumn)
        {
            Pieces tmp = board.board[row, column];
            board.board[row, column] = board.board[nextRow, nextColumn];
            board.board[nextRow, nextColumn] = tmp;
            (this.positionX, this.positionY) = Index2Position(nextRow, nextColumn);
        }
        // Funkcja pomocnicza do bicia innej figury
        public void Capture(Board board, int row, int nextRow, int column, int nextColumn)
        {
            board.board[nextRow, nextColumn] = board.board[row, column];
            board.board[row, column] = new Empty(positionX, positionY);
            (this.positionX, this.positionY) = Index2Position(nextRow, nextColumn);
        }
        public void CheckingMechanism(Board board, int row, int column, int color)
        {
            if (board.board[row, column].GetType() == typeof(King))
            {
                if (color == -1)
                    board.isCheckedWhite = true;
                else
                    board.isCheckedBlack = true;
            }
        }

        public abstract string PrintOutput();
        public abstract void Move(Board board, char positionX, int positionY, int color);

        public virtual void ControlledFields(Board board, int column, int row, int color) { }
        // Plan:
        // 1. Do kazdej figury nadpisac ta funkcje, tak aby uzupelnila tablice controlledFieldsBy__ w board
        // wartościami true jesli sa kontrolowane
        // Wywołać tą funkcję przy każdym wywołaniu Move(), po posunięciu
        // 2. Napisać druga funkcję (można w klasie Board), która po każdym posunięciu iteruje się po tablicy i dla każdej figury 
        // danego koloru uaktualnia wartości controlledFields (za pomocą wywołania poprzedniej funkcji) 
        // i również wywołać ją w Move.
        // 3. Dodatkowa funkcjonalność w ControlledFields(): Jesli na któryms z kontrolowanych pol znajdzie sie krol 
        // przeciwnego koloru, nadpisac wartosc IsChecked(Kolor) wartoscia true.
    }


    public class Pawn : Pieces
    {
        public Pawn(int id, int color, char positionX, int positionY) :
            base(id, color, "Pawn", positionX, positionY)
        { }

        public override string PrintOutput()
        {
            ColorSet(this.color);
            return "P ";
        }

        public override void Move(Board board, char positionX, int positionY, int color)
        {
            (int row, int column) = Position2Index(this.positionX, this.positionY);
            (int nextRow, int nextColumn) = Position2Index(positionX, positionY);

            IsColorRight(color);
            IsOutsideBoard(positionX, positionY);

            // Przypadek ruchu pionka o dwa ruchy do przodu
            if (Math.Abs(nextRow - row) == 2)
            {
                if ((this.color == 1 && row != 6) || (this.color == -1 && row != 1))
                {
                    throw new Exception("Wrong pawn move. Pawn can't now move 2 squares forward");
                }
                int sgn = Math.Sign(nextRow - row);
                if (board.board[nextRow - (sgn * 1), nextColumn].GetType() != typeof(Empty))
                {
                    throw new Exception("Wrong pawn move. Pawn can't jump over other pieces");
                }
                MoveSwap(board, row, nextRow, column, nextColumn);
            }
            else if (board.board[nextRow, nextColumn].GetType() != typeof(Empty))
            {
                // Przypadek bicia
                if (Math.Abs(nextColumn - column) == 1 && Math.Abs(nextRow - row) == 1 && board.board[nextRow, nextColumn].color != this.color)
                {
                    Capture(board, row, nextRow, column, nextColumn);
                }
                else
                {
                    throw new Exception("Wrong pawn move - square taken");
                }
            }
            // Zly ruch pionka
            else if (column != nextColumn || Math.Abs(nextRow - row) > 2 || Math.Abs(nextRow - row) < 0)
            {
                throw new Exception("Wrong pawn move");
            }
            else
            {
                MoveSwap(board, row, nextRow, column, nextColumn);
            }

            ControlledFields(board, nextColumn, nextRow, color);
            board.UpdateControlledFields();
        }

        public override void ControlledFields(Board board, int column, int row, int color)
        {
            bool[,] controlled = (color == -1) ? board.controlledFieldsByBlack : board.controlledFieldsByWhite;

            if (column == 0)
            {
                if (board.board[row - color, column + 1].GetType() == typeof(Empty))
                    controlled[row - color, column + 1] = true;
            }
            else if (column == 7)
            {
                if (board.board[row - color, column - 1].GetType() == typeof(Empty))
                    controlled[row - color, column - 1] = true;
            }
            else
            {
                if (board.board[row - color, column - 1].GetType() == typeof(Empty))
                    controlled[row - color, column - 1] = true;
                if (board.board[row - color, column + 1].GetType() == typeof(Empty))
                    controlled[row - color, column + 1] = true;
            }
        }
    }
    

    public class Rook : Pieces
    {
        public Rook(int id, int color, char positionX, int positionY) :
            base(id, color, "Rook", positionX, positionY)
        { }

        public override string PrintOutput()
        {
            ColorSet(this.color);
            return "R ";
        }
        public override void Move(Board board, char positionX, int positionY, int color)
        {
            (int row, int column) = Position2Index(this.positionX, this.positionY);
            (int nextRow, int nextColumn) = Position2Index(positionX, positionY);
            bool move = true;

            IsColorRight(color);
            IsOutsideBoard(positionX, positionY);
            if (nextColumn == column)
            {
                int rowDiff = nextRow - row;
                for (int i = 1; i < Math.Abs(rowDiff); i++)
                {
                    if (board.board[nextRow - Math.Sign(rowDiff) * i, nextColumn].GetType() == typeof(Empty))
                    {
                        move = true;
                    }
                    else
                    {
                        move = false;
                        break;
                    }
                }
            }
            else if (nextRow == row)
            {
                int columnDiff = nextColumn - column;
                for (int i = 1; i < Math.Abs(columnDiff); i++)
                {
                    if (board.board[nextRow, nextColumn - Math.Sign(columnDiff) * i].GetType() == typeof(Empty))
                    {
                        move = true;
                    }
                    else
                    {
                        move = false;
                        break;
                    }
                }
            }
            // Błąd - ruch wieży nie po liniach prostych
            else
            {
                throw new Exception("Wrong rook move - Not through straight lines.");
            }
            // Przypadek bicia
            if (move && board.board[nextRow, nextColumn].GetType() != typeof(Empty)
                && board.board[nextRow, nextColumn].color != this.color)
            {
                Capture(board, row, nextRow, column, nextColumn);
            }
            // Przypadek poprawnego ruchu
            else if (move && board.board[nextRow, nextColumn].GetType() == typeof(Empty))
            {
                MoveSwap(board, row, nextRow, column, nextColumn);
            }
            // Błąd - droga figury zablokowana
            else
            {
                throw new Exception("Wrong rook move - rook blocked.");
            }
            ControlledFields(board, nextColumn, nextRow, color);
            board.UpdateControlledFields();
        }

        public override void ControlledFields(Board board, int column, int row, int color)
        {
            bool[,] controlled = (color == -1) ? board.controlledFieldsByBlack : board.controlledFieldsByWhite;
            for (int i = row + 1; i <= 7; i++)
            {
                if (board.board[i, column].GetType() != typeof(Empty))
                    break;
                else
                    controlled[i, column] = true;
            }
            for (int i = row - 1; i >= 0; i--)
            {
                if (board.board[i, column].GetType() != typeof(Empty))
                    break;
                else
                    controlled[i, column] = true;
            }
            for (int i = column + 1; i <= 7; i++)
            {
                if (board.board[row, i].GetType() != typeof(Empty))
                    break;
                else
                    controlled[row, i] = true;
            }
            for (int i = column - 1; i >= 0; i--)
            {
                if (board.board[row, i].GetType() != typeof(Empty))
                    break;
                else
                    controlled[row, i] = true;
            }
        }
    }

    public class Bishop : Pieces
    {
        public Bishop(int id, int color, char positionX, int positionY) :
            base(id, color, "Bishop", positionX, positionY)
        { }

        public override string PrintOutput()
        {
            ColorSet(this.color);
            return "B ";
        }
        public override void Move(Board board, char positionX, int positionY, int color)
        {
            (int row, int column) = Position2Index(this.positionX, this.positionY);
            (int nextRow, int nextColumn) = Position2Index(positionX, positionY);
            bool move = true;

            IsColorRight(color);
            IsOutsideBoard(positionX, positionY);
            int columnDiff = nextColumn - column;
            int rowDiff = nextRow - row;
            if (Math.Abs(rowDiff) == Math.Abs(columnDiff))
            {
                for (int i = 1; i < Math.Abs(rowDiff); i++)
                {
                    if (board.board[nextRow - Math.Sign(rowDiff) * i, nextColumn - Math.Sign(columnDiff) * i].GetType() == typeof(Empty))
                    {
                        move = true;
                    }
                    else
                    {
                        move = false;
                        break;
                    }
                }
            }
            else
            {
                throw new Exception("Wrong bishop move - Not through diagonals.");
            }
            if (move && board.board[nextRow, nextColumn].GetType() != typeof(Empty)
                && board.board[nextRow, nextColumn].color != this.color)
            {
                Capture(board, row, nextRow, column, nextColumn);
            }
            // Przypadek poprawnego ruchu
            else if (move && board.board[nextRow, nextColumn].GetType() == typeof(Empty))
            {
                MoveSwap(board, row, nextRow, column, nextColumn);
            }
            // Błąd - droga figury zablokowana
            else
            {
                throw new Exception("Wrong bishop move - bishop blocked.");
            }

            ControlledFields(board, nextColumn, nextRow, color);
            board.UpdateControlledFields();
        }

        public override void ControlledFields(Board board, int column, int row, int color)
        {
            bool[,] controlled = (color == -1) ? board.controlledFieldsByBlack : board.controlledFieldsByWhite;
            for(int i = 1; i <= 7; i++)
            {
                if (row + i > 7 || column + i > 7)
                    break;
                else if (board.board[row + i, column + i].GetType() == typeof(Empty))
                {
                    controlled[row + i, column + i] = true;
                    CheckingMechanism(board, row + i, column + i, color);
                }
                else
                    break;
            }
            for (int i = 1; i <= 7; i++)
            {
                if (row + i > 7 || column - i < 0)
                    break;
                else if (board.board[row + i, column - i].GetType() == typeof(Empty))
                {
                    controlled[row + i, column - i] = true;
                    CheckingMechanism(board, row + i, column - i, color);
                }
                else
                    break;
            }
            for (int i = 1; i <= 7; i++)
            {
                if (row - i < 0 || column + i > 7)
                    break;
                else if (board.board[row - i, column + i].GetType() == typeof(Empty))
                {
                    controlled[row - i, column + i] = true;
                    CheckingMechanism(board, row - i, column + i, color);
                }
                else
                    break;
            }
            for (int i = 1; i <= 7; i++)
            {
                if (row - i < 0 || column - i < 0)
                    break;
                else if (board.board[row - i, column - i].GetType() == typeof(Empty))
                {
                    controlled[row - i, column - i] = true;
                    CheckingMechanism(board, row - i, column - i, color);
                }
                else
                    break;
            }
        }
    }

    public class Knight : Pieces
    {
        public Knight(int id, int color, char positionX, int positionY) :
            base(id, color, "Knight", positionX, positionY)
        { }

        public override string PrintOutput()
        {
            ColorSet(this.color);
            return "N ";
        }
        public override void Move(Board board, char positionX, int positionY, int color)
        {
            (int row, int column) = Position2Index(this.positionX, this.positionY);
            (int nextRow, int nextColumn) = Position2Index(positionX, positionY);

            IsColorRight(color);
            IsOutsideBoard(positionX, positionY);

            // Przypadek zlego ruchu skoczka
            if (Math.Abs(Math.Pow((double)nextRow - (double)row, 2) + Math.Pow((double)nextColumn - (double)column, 2) - 5.0) > 10e-9)
            {
                throw new Exception("Wrong knight move");
            }
            // Przypadek zajetego pola
            if (board.board[nextRow, nextColumn].GetType() != typeof(Empty) && this.color == board.board[nextRow, nextColumn].color)
            {
                throw new Exception("Wrong knight move - square taken");
            }
            // Przypadek bicia
            if (board.board[nextRow, nextColumn].GetType() != typeof(Empty) && this.color != board.board[nextRow, nextColumn].color)
            {
                Capture(board, row, nextRow, column, nextColumn);
            }
            else
            {
                MoveSwap(board, row, nextRow, column, nextColumn);
            }

            ControlledFields(board, nextColumn, nextRow, color);
            board.UpdateControlledFields();
        }
        //public override void ControlledFields(Board board, int column, int row, int color)
        //{
        //    bool[,] controlled = (color == -1) ? board.controlledFieldsByBlack : board.controlledFieldsByWhite;
        //    if (row + 2 < 7 && column + 1 < 7 && board.board[row + 2, column + 1].GetType() != typeof(Empty))
        //}
    }

    public class King : Pieces
    {
        public King(int id, int color, in char positionX, int positionY) :
            base(id, color, "King", positionX, positionY)
        { }

        public override string PrintOutput()
        {
            ColorSet(this.color);
            return "K ";
        }
        public override void Move(Board board, char positionX, int positionY, int color) { }
    }

    public class Queen : Pieces
    {
        public Queen(int id, int color, char positionX, int positionY) :
            base(id, color, "Queen", positionX, positionY)
        { }

        public override string PrintOutput()
        {
            ColorSet(this.color);
            return "Q ";
        }
        public override void Move(Board board, char positionX, int positionY, int color)
        {
            (int row, int column) = Position2Index(this.positionX, this.positionY);
            (int nextRow, int nextColumn) = Position2Index(positionX, positionY);
            bool move = true;

            IsColorRight(color);
            IsOutsideBoard(positionX, positionY);
            int columnDiff = nextColumn - column;
            int rowDiff = nextRow - row;
            if (nextColumn == column)
            {
                for (int i = 1; i < Math.Abs(rowDiff); i++)
                {
                    if (board.board[nextRow - Math.Sign(rowDiff) * i, nextColumn].GetType() == typeof(Empty))
                    {
                        move = true;
                    }
                    else
                    {
                        move = false;
                        break;
                    }
                }
            }
            else if (nextRow == row)
            {
                for (int i = 1; i < Math.Abs(columnDiff); i++)
                {
                    if (board.board[nextRow, nextColumn - Math.Sign(columnDiff) * i].GetType() == typeof(Empty))
                    {
                        move = true;
                    }
                    else
                    {
                        move = false;
                        break;
                    }
                }
            }
            else if (Math.Abs(rowDiff) == Math.Abs(columnDiff))
            {
                for (int i = 1; i < Math.Abs(rowDiff); i++)
                {
                    if (board.board[nextRow - Math.Sign(rowDiff) * i, nextColumn - Math.Sign(columnDiff) * i].GetType() == typeof(Empty))
                    {
                        move = true;
                    }
                    else
                    {
                        move = false;
                        break;
                    }
                }
            }
            else
            {
                throw new Exception("Wrong queen move - Not through diagonals or straight lines");
            }
            // Przypadek bicia
            if (move && board.board[nextRow, nextColumn].GetType() != typeof(Empty)
                && board.board[nextRow, nextColumn].color != this.color)
            {
                Capture(board, row, nextRow, column, nextColumn);
                return;
            }
            // Przypadek poprawnego ruchu
            else if (move && board.board[nextRow, nextColumn].GetType() == typeof(Empty))
            {
                MoveSwap(board, row, nextRow, column, nextColumn);
                return;
            }
            // Błąd - droga figury zablokowana
            else
            {
                throw new Exception("Wrong queen move - queen blocked.");
            }
        }
    }

    public class Empty : Pieces
    {
        public Empty(char positionX, int positionY) :
            base(0, 0, "Empty", positionX, positionY)
        { }

        public override string PrintOutput()
        {
            return "  ";
        }
        public override void Move(Board board, char positionX, int positionY, int color) { }
    }

}

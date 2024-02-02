using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chessGame
{
    public class Board
    {
        public Pieces[,] board { get; set; }
        private int boardsize { get; set; }

        private static int piecesID = 1;

        public bool[,] controlledFieldsByBlack { get; set; }
        public bool[,] controlledFieldsByWhite { get; set; }
        public bool isCheckedWhite { get; set; }
        public bool isCheckedBlack { get; set; }
        public Board()
        {
            this.boardsize = 8;
            this.board = new Pieces[boardsize, boardsize];
            this.controlledFieldsByBlack = new bool[boardsize, boardsize];
            this.controlledFieldsByWhite = new bool[boardsize, boardsize];
            for (int i = 0; i < boardsize; i++)
            {
                for (int j = 0; j < boardsize; j++)
                {
                    this.controlledFieldsByBlack[i, j] = false;
                    this.controlledFieldsByWhite[i, j] = false;
                }
            }
            this.isCheckedBlack = false;
            this.isCheckedWhite = false;
            
            InitializeBoard();
        }
        public void PrintBoard()
        {
            for (int i = 0; i < boardsize; i++)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("  _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _");
                Console.Write((8 - i).ToString() + " ");
                for (int j = 0; j < boardsize; j++)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write("| ");
                    Console.Write(board[i, j].PrintOutput());
                }
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write("|\n");
            }
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("  _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _");
            Console.WriteLine("    a   b   c   d   e   f   g   h");
            Console.ForegroundColor= ConsoleColor.White;
        }

        public void UpdateControlledFields()
        {
            for(int i = 0; i < boardsize; i++)
            {
                for (int j = 0; j < boardsize; j++)
                {
                    this.controlledFieldsByWhite[i, j] = false;
                    this.controlledFieldsByBlack[i, j] = false;
                }
            }
            for (int i = 0; i < boardsize; i++)
            {
                for (int j = 0; j < boardsize; j++)
                {
                    this.board[i, j].ControlledFields(this, j, i, this.board[i, j].color);
                }
            }
        }

        public void InitializeBoard()
        {
            // Utworzenie pustych pól
            for (int i = 2; i < boardsize - 2; i++)
            {
                for (int j = 0; j < boardsize; j++)
                {
                    Empty empty = new Empty(Convert.ToChar(j + 97), 8 - i);
                    this.board[i, j] = empty;
                }
            }

            // Utworzenie białych figur
            for (int i = 0; i < boardsize; i++)
            {
                Pawn pawnWhite = new Pawn(piecesID++, 1, Convert.ToChar(i + 97), 2);
                this.board[6, i] = pawnWhite;
                this.board[6, i].ControlledFields(this, i, 6, 1);
            }
            Rook rookWhite1 = new Rook(piecesID++, 1, 'a', 1);
            this.board[7, 0] = rookWhite1;
            Knight knightWhite1 = new Knight(piecesID++, 1, 'b', 1);
            this.board[7, 1] = knightWhite1;
            Bishop bishopWhite1 = new Bishop(piecesID++, 1, 'c', 1);
            this.board[7, 2] = bishopWhite1;
            Queen queenWhite = new Queen(piecesID++, 1, 'd', 1);
            this.board[7, 3] = queenWhite;
            King kingWhite = new King(piecesID++, 1, 'e', 1);
            this.board[7, 4] = kingWhite;
            Bishop bishopWhite2 = new Bishop(piecesID++, 1, 'f', 1);
            this.board[7, 5] = bishopWhite2;
            Knight knightWhite2 = new Knight(piecesID++, 1, 'g', 1);
            this.board[7, 6] = knightWhite2;
            Rook rookWhite2 = new Rook(piecesID++, 1, 'h', 1);
            this.board[7, 7] = rookWhite2;

            // Utworzenie czarnych figur
            for (int i = 0; i < boardsize; i++)
            {
                Pawn pawnBlack = new Pawn(piecesID++, -1, Convert.ToChar(i + 97), 7);
                this.board[1, i] = pawnBlack;
                this.board[1, i].ControlledFields(this, i, 1, -1);
            }
            Rook rookBlack1 = new Rook(piecesID++, -1, 'a', 8);
            this.board[0, 0] = rookBlack1;
            Knight knightBlack1 = new Knight(piecesID++, -1, 'b', 8);
            this.board[0, 1] = knightBlack1;
            Bishop bishopBlack1 = new Bishop(piecesID++, -1, 'c', 8);
            this.board[0, 2] = bishopBlack1;
            Queen queenBlack = new Queen(piecesID++, -1, 'd', 8);
            this.board[0, 3] = queenBlack;
            King kingBlack = new King(piecesID++, -1, 'e', 8);
            this.board[0, 4] = kingBlack;
            Bishop bishopBlack2 = new Bishop(piecesID++, -1, 'f', 8);
            this.board[0, 5] = bishopBlack2;
            Knight knightBlack2 = new Knight(piecesID++, -1, 'g', 8);
            this.board[0, 6] = knightBlack2;
            Rook rookBlack2 = new Rook(piecesID++, -1, 'h', 8);
            this.board[0, 7] = rookBlack2;


            // Ustawienie poczatkowych kontrolowanych pol
            // Wieze czarne
            this.board[0, 0].ControlledFields(this, 0, 0, -1);
            this.board[0, 7].ControlledFields(this, 7, 0, -1);
            // Gonce czarne
            this.board[0, 2].ControlledFields(this, 2, 0, -1);
            this.board[0, 5].ControlledFields(this, 5, 0, -1);
            // Wieze biale
            this.board[7, 0].ControlledFields(this, 0, 7, 1);
            this.board[7, 7].ControlledFields(this, 7, 7, 1);
            // Gonce biale 
            this.board[7, 2].ControlledFields(this, 2, 7, 1);
            this.board[7, 5].ControlledFields(this, 5, 7, 1);
        }
    }
}

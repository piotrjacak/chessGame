using System;
using System.Text;

namespace chessGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Board gameBoard = new Board();
            Program program = new Program();
            string? option;
            int color = 1;

            while(true)
            {
                Console.WriteLine("CHESS GAME");
                Console.WriteLine("1. Start the game.");
                Console.WriteLine("2. See the instruction.");
                Console.WriteLine("3. Exit.");
                Console.WriteLine("4. Testing");
                option = Console.ReadLine();
                switch (option)
                {
                    case "1":
                        gameBoard.PrintBoard();
                        while (true)
                        {
                            try
                            {
                                program.Testing(gameBoard);
                                program.Game(gameBoard, color);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                Console.WriteLine();
                                continue;
                            }
                            if (color == 1) { color = -1; }
                            else { color = 1; }
                        }
                    case "2":
                        program.Instruction();
                        break;
                    case "3":
                        return;
                    case "4":
                        program.Testing(gameBoard);
                        break;
                }
            }
        }


        public void Game(Board gameBoard, int color)
        {
            string? input;
            char[] symbols;
            if (color == 1)
            {
                Console.WriteLine("White to move: ");
            }
            else
            {
                Console.WriteLine("Black to move: ");
            }

            input = Console.ReadLine();
            if (input == null || input.Length != 4)
            {
                throw new Exception("Wrong input.");
            }

            symbols = input.ToCharArray();
            int column = Convert.ToInt32(symbols[0]) - 97;
            int row = 8 - (Convert.ToInt32(symbols[1]) - 48);
            char nextColumn = symbols[2];
            int nextRow = Convert.ToInt32(symbols[3]) - 48;
            try
            {
                gameBoard.board[row, column].Move(gameBoard, nextColumn, nextRow, color);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            gameBoard.PrintBoard();

        }

        public void Instruction()
        {
            Console.WriteLine();
            Console.WriteLine("INSTRUCTION:");
            Console.WriteLine("1. Symbols:\n   P - pawn\n   R - rook\n   N - knight\n   B - bishop\n   Q - queen\n   K - king");
            Console.WriteLine("2. To move a piece from one square to another type for example 'a2a3', \n   where a2 is the initial square and 'a3' is target square.");
            Console.WriteLine("3. To exit the game while playing press Ctrl-C");
            Console.WriteLine();
            Console.WriteLine();
        }

        public void Testing(Board board)
        {
            Console.WriteLine("Kontrolowane przez biale:");
            for (int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    if (board.controlledFieldsByWhite[i, j] == true)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                    }
                    Console.Write(board.controlledFieldsByWhite[i, j].ToString() + "\t");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.Write("\n");
            }
            Console.WriteLine("Czy szach na bialego?: ");
            Console.WriteLine(board.isCheckedWhite.ToString());
            Console.Write("\n\n");

            Console.WriteLine("Kontrolowane przez czarne:");
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board.controlledFieldsByBlack[i, j] == true)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                    }
                    Console.Write(board.controlledFieldsByBlack[i, j].ToString() + "\t");
                    Console.ForegroundColor = ConsoleColor.White;

                }
                Console.Write("\n");
            }
            Console.WriteLine("Czy szach na czarnego?: ");
            Console.WriteLine(board.isCheckedBlack.ToString());
            Console.Write("\n");
        }
    }
}

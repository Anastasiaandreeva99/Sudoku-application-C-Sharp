using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokoSisi
{
    public enum GameLevel
    {
        Easy,
        Medium,
        Difficult
    }
    public class SudokuGenerator
    {
        #region Properties
        public SingleField[,] Grid { get; set; }
        public SingleField[,] EmptyFieldsSolution { get; set; }
        public GameLevel Difficulty { get; set; }
        public int NumberOfSolutions { get; set; }

        private Stack<Tuple<int, int, SingleField, SingleField>> undoStack;

        public Stack<Tuple<int, int, SingleField, SingleField>> UndoStack//da napravia dulboko kopirane
        {
            get
            {
                Stack<Tuple<int, int, SingleField, SingleField>> temp = new Stack<Tuple<int, int, SingleField, SingleField>>(new Stack<Tuple<int, int, SingleField, SingleField>>(undoStack));
                return temp;



            }
            set { undoStack = new Stack<Tuple<int, int, SingleField, SingleField>>(new Stack<Tuple<int, int, SingleField, SingleField>>(value)); }
        }
        private Stack<Tuple<int, int, SingleField, SingleField>> redoStack;

        public Stack<Tuple<int, int, SingleField, SingleField>> RedoStack
        {
            get
            {
                Stack<Tuple<int, int, SingleField, SingleField>> temp = new Stack<Tuple<int, int, SingleField, SingleField>>(new Stack<Tuple<int, int, SingleField, SingleField>>(redoStack));
                return temp;


            }
            set { redoStack = new Stack<Tuple<int, int, SingleField, SingleField>>(new Queue<Tuple<int, int, SingleField, SingleField>>(value)); }
        }

        #endregion

        #region Constructor
        public SudokuGenerator(string _difficulty)
        {
            Console.WriteLine(_difficulty);
            GameLevel result;
            Enum.TryParse(_difficulty, out result);
            Difficulty = result;
            EmptyFieldsSolution = new SingleField[9, 9];
            Grid = new SingleField[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Grid[i, j] = new SingleField(0, true);
                }
            }
            UndoStack = new Stack<Tuple<int, int, SingleField, SingleField>>();
            RedoStack = new Stack<Tuple<int, int, SingleField, SingleField>>();

        }
        #endregion

        #region StartGame
        /* public void CopySolvedGridToGrid()
         {
             for (int i = 0; i < 9; i++)
             {
                 for (int j = 0; j < 9; j++)
                 {
                     Grid[i, j] = new SingleField(SolvedGrid[i, j]);
                 }
             }
         }*/
        public void StartGame()
        {
            InitializeSolvedGrid();
            //CopySolvedGridToGrid();
            InitializeGridBlankFields();

        }
        public void InitializeSolvedGrid()
        {
            DiagonalSquares();
            FillOther(0, 3);
        }
        public void InitializeGridBlankFields()
        {

            int emptyFieldsCount = 0;
            List<Tuple<int, int>> coordinatesOfBlankedFields = new List<Tuple<int, int>>();
            switch (Difficulty)
            {
                case GameLevel.Easy:
                    emptyFieldsCount = 40;
                    break;
                case GameLevel.Medium:
                    emptyFieldsCount = 43;
                    break;
                case GameLevel.Difficult:
                    emptyFieldsCount = 47;
                    break;
                default:
                    emptyFieldsCount = 40;
                    break;
            }
            SingleField[,] board = new SingleField[9, 9];

            do
            {
                board = new SingleField[9, 9];
                while (emptyFieldsCount > 0)
                {
                    Random rand = new Random();
                    int row = rand.Next(0, 9);
                    int col = rand.Next(0, 9);
                    while (Grid[row, col].Value == 0)
                    {
                        Console.WriteLine("in");
                        row = rand.Next(0, 9);
                        col = rand.Next(0, 9);
                    }
                    EmptyFieldsSolution[row, col] = new SingleField(Grid[row, col]);
                    Grid[row, col].Value = 0;
                    Grid[row, col].Constant = false;
                    coordinatesOfBlankedFields.Add(new Tuple<int, int>(row, col));
                    emptyFieldsCount--;
                }
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        board[i, j] = new SingleField(Grid[i, j]);
                    }
                }
                Console.WriteLine(NumberOfSolutions);
                NumberOfSolutions = 0;
            }
            while (!solveBacktrack(board, 0, 0));
        }
        #endregion

        #region GridInitializationHelpFunctions
        private void DiagonalSquares()
        {

            for (int i = 0; i < 9; i = i + 3)
                FillField(i, i);
        }
        private void FillField(int row, int col)
        {
            int num;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    do
                    {
                        Random rnd = new Random();
                        num = rnd.Next(1, 10);
                    }
                    while (!IsNumberNotInBox(Grid, row, col, num));

                    Grid[row + i, col + j] = new SingleField(num, true);
                }
            }
        }
        private bool IsNumberNotInBox(SingleField[,] board, int rowStart, int colStart, int num)
        {

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (board[rowStart + i, colStart + j].Value == num)
                        return false;

            return true;
        }
        private bool NotInRow(SingleField[,] board, int i, int num)
        {
            for (int j = 0; j < 9; j++)
                if (Grid[i, j].Value == num)
                    return false;
            return true;
        }

        private bool NotInCol(SingleField[,] board, int j, int num)
        {
            for (int i = 0; i < 9; i++)
                if (board[i, j].Value == num)
                    return false;
            return true;
        }
        private bool CheckIfSafe(SingleField[,] board, int i, int j, int num)
        {

            return (NotInRow(board, i, num) &&
                    NotInCol(board, j, num) &&
                    IsNumberNotInBox(board, i - i % 3, j - j % 3, num));
        }
        private bool FillOther(int i, int j)
        {
            if (j >= 9 && i < 8)
            {
                i = i + 1;
                j = 0;
            }
            if (i >= 9 && j >= 9)
                return true;

            if (i < 3)
            {
                if (j < 3)
                    j = 3;
            }
            else if (i < 6)
            {
                if (j == (int)(i / 3) * 3)
                    j = j + 3;
            }
            else
            {
                if (j == 6)
                {
                    i = i + 1;
                    j = 0;
                    if (i >= 9)
                        return true;
                }
            }

            for (int num = 1; num <= 9; num++)
            {
                if (CheckIfSafe(Grid, i, j, num))
                {
                    Grid[i, j].Value = num;
                    if (FillOther(i, j + 1))
                        return true;

                    Grid[i, j].Value = 0;
                }
            }
            return false;
        }
       public int solve(int i, int j, SingleField[,] cells, int count /*initailly called with 0*/)
        {
            if (i == 9)
            {
                i = 0;
                if (++j == 9)
                    return 1 + count;
            }
            if (cells[i,j].Value != 0)  // skip filled cells
                return solve(i + 1, j, cells, count);
            // search for 2 solutions instead of 1
            // break, if 2 solutions are found
            for (int val = 1; val <= 9 && count < 2; ++val)
            {
                if (CheckIfSafe(cells,i,j,val))
                {
                    cells[i,j].Value = val;
                    // add additional solutions
                    count = solve(i + 1, j, cells, count);
                }
             }
           cells[i,j].Value= 0; // reset on backtrack
           return count;
        }


public  bool solveBacktrack(SingleField[,] board, int row,int col)
        {
            bool solved = false;

            if (board[row,col].Value != 0)
            {
                if (col == board.GetLength(1) - 1)
                    if (row == Grid.GetLength(0) - 1)
                    {
                         NumberOfSolutions++;
                        solved = true;
                    }
                    else
                        return solveBacktrack(board, row + 1, 0); // case 2
                else
                    return solveBacktrack( board,row, col + 1); // case 3
            }


            for (int k = 1; k <= 9; k++) //iterates through all numbers from 1 to N
            {
                // If a certain number is a legal for a cell - use it
                if (CheckIfSafe(board, row, col, k))
                {
                    board[row,col].Value = k;
                    if (col == board.GetLength(1) - 1) // if it's the rightmost column
                    {
                        if (row == board.GetLength(0) - 1) // and the lowest row - the sudoku is solved
                        {
                            //printgame.board(b);
                            NumberOfSolutions++;
                            solved = true;
                        }
                        else
                            solved = solveBacktrack(board, row + 1, 0); // if its not the lowest row - keep solving for next row
                    }
                    else // keep solving for the next cell
                        solved = solveBacktrack(board, row, col + 1);
                }
                if (solved)
                     NumberOfSolutions++;
                else //if down the recursion sudoku isn't solved-> remove the number (backtrack)
                {
                    board[row,col].Value = 0;
                }
            }
            return solved;
        }
        #endregion

        #region InitiliazitionByUser
        public bool InitializeFieldByUser(int row, int col, string num)
        {
            if (num != "")
            {
                int newValue;
                if (!int.TryParse(num, out newValue)) return false;
                if (!CheckIfSafe(Grid, row, col, newValue)) return false;
                Grid[row, col] = new SingleField(newValue, false);
                return true;
            }
            return true;

        }
        #endregion


        #region Unod&&RedoStack
        public void ChangeFieldValue(int row, int col, SingleField newValue)
        {
            Grid[row, col] = newValue;
        }

        public void SetUndoStack()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (Grid[i, j].Constant == false) undoStack.Push(new Tuple<int, int, SingleField, SingleField>(i, j, new SingleField(0, false), new SingleField(0, false)));
                }
            }
        }

        public void AddToUndoStack(Tuple<int, int, SingleField, SingleField> newElement)
        {
            undoStack.Push(newElement);
            Console.WriteLine(undoStack.Peek());
        }

        public Tuple<int, int, SingleField, SingleField> UndoStackPop()
        {
            Tuple<int, int, SingleField, SingleField> undoElement = undoStack.Peek();
            if (!(undoElement.Item3.Value == 0 && undoElement.Item4.Value == 0))
            {
                undoStack.Pop();
            }
            return undoElement;

        }
        public void SetRedoStack()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (Grid[i, j].Constant == false) redoStack.Push(new Tuple<int, int, SingleField, SingleField>(i, j, new SingleField(0, false), new SingleField(0, false)));
                }
            }
        }

        public void AddToRedoStack(Tuple<int, int, SingleField, SingleField> newElement)
        {
            redoStack.Push(newElement);
            // Console.WriteLine(redoStack.Peek());
        }

        public Tuple<int, int, SingleField, SingleField> RedoStackPop()
        {
            Tuple<int, int, SingleField, SingleField> redoElement = redoStack.Peek();
            if (!(redoElement.Item3.Value == 0 && redoElement.Item4.Value == 0))
            {
                redoStack.Pop();
            }
            return redoElement;

        }
        #endregion

        #region IsItTrue
        public bool IsItTrue()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (Grid[i, j].Constant == false && Grid[i, j].Value != EmptyFieldsSolution[i, j].Value) return false;
                }
            }
            return true;
        }
        #endregion

        public bool InitializeFieldByField(int row, int col, SingleField field)
        {
            if (field!= null)
            {
                int newValue = field.Value;
                if (newValue<0 || newValue>9) return false;
                if (!CheckIfSafe(Grid, row, col, newValue)) return false;
                Grid[row, col] = field;
                return true;
            }
            return true;

        }
        /*  public void InitializeGrid()
          {

              GameBoard tempGrid = ConstantGrid();
              tempGrid.ShuffleRows();
              SolvedGrid = tempGrid;
              Grid = tempGrid;


          }
          /*
          public void InitializeGrid2()
          {
              GameBoard tempGrid = new GameBoard();
              bool[,] columns = new bool[10, 10];
              for(int c=0;c<10;c++)
              {
                  for(int cc=0;cc<10;cc++)
                  {
                      columns[c, cc] = false;
                  }
              }
              bool[,] rows = new bool[10, 10];
              for (int r = 0; r < 10; r++)
              {
                  for (int rr = 0; rr < 10; rr++)
                  {
                      columns[r, rr] = false;
                  }
              }
              Dictionary<int, List<bool>> squares = new Dictionary<int, List<bool>>();
              squares.Add(1,new List<bool> { false, false , false, false, false, false, false, false, false, false });
              squares.Add(2, new List<bool> { false, false, false, false, false, false, false, false, false, false });
              squares.Add(3, new List<bool> { false, false, false, false, false, false, false, false, false, false });
              squares.Add(4, new List<bool> { false, false, false, false, false, false, false, false, false, false });
              squares.Add(5, new List<bool> { false, false, false, false, false, false, false, false, false, false });
              squares.Add(6, new List<bool> { false, false, false, false, false, false, false, false, false, false });
              squares.Add(7, new List<bool> { false, false, false, false, false, false, false, false, false, false });
              squares.Add(8, new List<bool> { false, false, false, false, false, false, false, false, false, false });
              squares.Add(9, new List<bool> { false, false, false, false, false, false, false, false, false, false });
              for (int i = 0; i < 9; i++)
              {
                  for (int j = 0; j < 9; j++)
                  {
                      Random rand = new Random();
                      int value = rand.Next(1, 10);
                      int squareIndex=1;
                      if (i < 3 && j < 3) squareIndex = 1;
                      if (i<3 && j < 6 && j > 2) squareIndex = 2;
                      if (i < 3 && j > 5 ) squareIndex = 3;
                      if (i > 2 && i < 6 && j < 3  ) squareIndex = 4;
                      if (i > 2 && i<6 && j > 2 && j < 6) squareIndex = 5;
                      if (i > 2&& i<6 && j>5) squareIndex = 6;
                      if (i > 5 && j < 3 ) squareIndex = 7;
                      if (i > 5 &&  j > 2 && j < 6) squareIndex = 8;
                      if (i > 5 && j > 5) squareIndex = 9;
                      List<bool> numbersInSquares = new List<bool>();
                      squares.TryGetValue(squareIndex,out numbersInSquares);
                      int count=0;
                      while(columns[j+1,value]==true || rows[i+1,value] == true || numbersInSquares[value] == true)
                      {
                          count++;
                          value = rand.Next(1, 10);


                      }
                      tempGrid.SetSingleField(i,j,value);
                      Console.WriteLine(tempGrid.Grid[i, j]);
                      columns[j + 1,value]= true;
                      rows[i + 1,value] = true;
                      squares[squareIndex][value] = true;


                  }

              }

              SolvedGrid = tempGrid;
              Grid = tempGrid;
          }
          public GameBoard ConstantGrid()
          {
              List<int> line1 = new List<int> { 2,8,3,7,9,5,4,1,6 };
              List<int> line2 = new List<int> { 6, 9, 1, 8, 4, 2, 5, 3, 7 };
              List<int> line3 = new List<int> { 4, 7, 5, 6, 3, 1, 2, 9, 8 };
              List<int> line4 = new List<int> { 7, 5, 6, 9, 8, 4, 3, 2, 1 };
              List<int> line5 = new List<int> { 1, 3, 9, 5, 2, 6, 7, 8, 4 };
              List<int> line6 = new List<int> { 8, 2, 4, 1, 7, 3, 6, 5, 9 };
              List<int> line7 = new List<int> { 9, 4, 2, 3, 6, 8, 1, 7, 5 };
              List<int> line8 = new List<int> { 5, 6, 7, 2, 1, 9, 8, 4, 3 };
              List<int> line9 = new List<int> { 3, 1, 8, 4, 3, 7, 9, 6, 2 };
              List<List<int>> newGrid = new List<List<int>> { line1, line2, line3, line4, line5, line6, line7, line8, line9 };
              return new GameBoard(newGrid);

          }
         */
    }
}

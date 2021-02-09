using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokoSisi
{
    public class GameBoard
    {
        private SingleField[,] grid;
        public SingleField[,] Grid
        {
            get
            {
                SingleField[,] temp = grid.Clone() as SingleField[,];
                return temp;

            }
            set
            { if (value != null)
                    grid = value.Clone() as SingleField[,];
                else
                    grid = new SingleField[10, 10];

            }
        }
        public SingleField this[int index1, int index2]
        {
            get
            {

                if (index1 < grid.GetLength(0) && index2 < grid.GetLength(1))
                    return grid[index1, index2];
                else return grid[0, 0];

            }

            // set accessor 
            set
            {

                if(value!=null && index1<grid.GetLength(0) && index2<grid.GetLength(1))
                grid[index1, index2] = value;

            }
        }

        public GameBoard(SingleField[,] _grid)
        {
            Grid = _grid;

        }
        public GameBoard() : this(new SingleField[10,10])
        { }
        public GameBoard(int[,] _grid)
        {
            SingleField[,] newGrid = new SingleField[_grid.GetLength(0), _grid.GetLength(1)];
            for (int i = 0;i<_grid.GetLength(0);i++)
            {
                for(int j=0;j<_grid.GetLength(1);j++)
                {
                   
                    newGrid[i,j]=new SingleField(_grid[i,j], true);
                }
                

            }
            Grid = newGrid;
        }
        public bool SetSingleField(int row,int col,int value)
        {
            if (row > 9 || row < 1 || col > 9 || col < 1 || value < 1 || value > 9) return false;
            SingleField newValue = new SingleField(value, true);
            Grid[row,col] = newValue;
            return true;
        }
      /*  public void ShuffleRows()
        {
            List<List<SingleField>> firstThreeLines = new List<List<SingleField>> { grid[0], grid[1], grid[2] };
            List<List<SingleField>> secondThreeLines = new List<List<SingleField>> { grid[3], grid[4], grid[5] };
            List<List<SingleField>> thirdThreeLines = new List<List<SingleField>> { grid[6], grid[7], grid[8] };
            Random rnd = new Random();
            List<List<SingleField>> firstThreeResult = firstThreeLines.Select(x => new { value = x, order = rnd.Next() })
            .OrderBy(x => x.order).Select(x => x.value).ToList();
            List<List<SingleField>> secondThreeResult = secondThreeLines.Select(x => new { value = x, order = rnd.Next() })
           .OrderBy(x => x.order).Select(x => x.value).ToList();
            List<List<SingleField>> thirdThreeResult = thirdThreeLines.Select(x => new { value = x, order = rnd.Next() })
           .OrderBy(x => x.order).Select(x => x.value).ToList();
            List<List<SingleField>> newGrid = new List<List<SingleField>>();
            foreach(var item in firstThreeResult)
            {
                newGrid.Add(item);
            }
            foreach (var item in secondThreeResult)
            {
                newGrid.Add(item);
            }
            foreach (var item in thirdThreeResult)
            {
                newGrid.Add(item);
            }
            Grid = newGrid;
           
        }*/
    }
}

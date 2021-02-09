using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SudokoSisi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties,Fields
        DispatcherTimer dispatcher = new DispatcherTimer();
        Stopwatch stopwatch = new Stopwatch();
        string currentTime = string.Empty;
        public int numberOfGames = 0;
        public int numberOfTrueGames = 0;
        public SudokuGenerator Sudoku { get; set; }

        #endregion

        #region Constructor-Main

        public MainWindow()
        {

            InitializeComponent();
            InitializeFieldsReadOnly();
            dispatcher.Tick += dispatcher_Tick;
            dispatcher.Interval = new TimeSpan(0, 0, 0, 0, 1);

        }
        #endregion

        #region Timer
        private void dispatcher_Tick(object sender, EventArgs e)
        {
            if (stopwatch.IsRunning)
            {
                TimeSpan timeSpan = stopwatch.Elapsed;
                currentTime = String.Format("{0:00}:{1:00}:{2:00}",
                timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
                labelClock.Content = currentTime;
            }
        }
        #endregion

        #region StartButtonClicks
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {

            stopwatch.Start();
            dispatcher.Start();
            stopwatch.Restart();
            ComboBoxItem selectedDifficulty = (ComboBoxItem)btnLevel.SelectedItem;
            string difficulty = selectedDifficulty.Content.ToString();
            Console.WriteLine(difficulty);
            Sudoku = new SudokuGenerator(difficulty);
            Sudoku.StartGame();
            Sudoku.SetUndoStack();
            Sudoku.SetRedoStack();
            InsertIntoGameBoard(Sudoku);
            numberOfGames++;
            TextBoxSolutionsNumber.Text = $"True Solutions:{numberOfTrueGames}";



        }
        #endregion

        #region FinishButtonClicks
        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            if (stopwatch.IsRunning)
            {
                stopwatch.Stop();
            }
            if (Sudoku.IsItTrue())
            {
                TextBoxError.Text = "Solved";
                numberOfTrueGames++;
                
            }
            else TextBoxError.Text = "False Solution!" + $"True Solutions:{numberOfTrueGames}";
        }
        #endregion

        #region btnUndoClick&&btnRedoClick
        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            if (Sudoku.UndoStack.Count != 0)
            {
                Tuple<int, int, SingleField, SingleField> lastMove = Sudoku.UndoStackPop();

                int row = lastMove.Item1;
                int col = lastMove.Item2;
                if (lastMove.Item3.Value != 0 || lastMove.Item4.Value != 0)
                {
                    Tuple<int, int, SingleField, SingleField> redoMove = new Tuple<int, int, SingleField, SingleField>(row, col, new SingleField(lastMove.Item3.Value, lastMove.Item3.Constant), new SingleField(lastMove.Item4.Value, lastMove.Item4.Constant));
                    Sudoku.AddToRedoStack(redoMove);
                }
                Sudoku.ChangeFieldValue(row, col, lastMove.Item3);
                StringBuilder fieldName = new StringBuilder("TextBox");
                fieldName.Append(row);
                fieldName.Append(col);
                TextBox cell = (TextBox)FindName(fieldName.ToString());
                cell.Text = lastMove.Item3.Value == 0 ? "" : lastMove.Item3.Value.ToString();
                //Sudoku.RedoStack.Push(redoMove);
            }
            else TextBoxError.Text = "Not Valid";

        }
        private void btnRedo_Click(object sender, RoutedEventArgs e)
        {
            if (Sudoku.RedoStack.Count != 0)
            {
                Tuple<int, int, SingleField, SingleField> moveToRedo = Sudoku.RedoStackPop();

                int row = moveToRedo.Item1;
                int col = moveToRedo.Item2;
                if (moveToRedo.Item3.Value != 0 || moveToRedo.Item4.Value != 0)
                {
                    Tuple<int, int, SingleField, SingleField> undoMove = new Tuple<int, int, SingleField, SingleField>(row, col, new SingleField(moveToRedo.Item3.Value, moveToRedo.Item3.Constant), new SingleField(moveToRedo.Item4.Value, moveToRedo.Item4.Constant));
                    Sudoku.AddToUndoStack(undoMove);
                }
                Sudoku.ChangeFieldValue(row, col, moveToRedo.Item4);
                StringBuilder fieldName = new StringBuilder("TextBox");
                fieldName.Append(row);
                fieldName.Append(col);
                TextBox cell = (TextBox)FindName(fieldName.ToString());
                cell.Text = moveToRedo.Item4.Value == 0 ? "" : moveToRedo.Item4.Value.ToString();
            }
        }
        #endregion

        #region btnSolveClick
        private void btnSolve_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (Sudoku.EmptyFieldsSolution[i, j] != null)
                    {
                        StringBuilder fieldName = new StringBuilder("TextBox");
                        fieldName.Append(i);
                        fieldName.Append(j);
                        TextBox cell = (TextBox)FindName(fieldName.ToString());
                        cell.Text = Sudoku.EmptyFieldsSolution[i, j].Value.ToString();

                    }
                }
            }
            if (stopwatch.IsRunning)
            {
                stopwatch.Stop();
            }
        } 
        #endregion

        #region GameBoardInitialization
        private void InsertIntoGameBoard(SudokuGenerator sudoku)
        {

            for (int i = 0; i < 9; i++)
            {

                for (int j = 0; j < 9; j++)
                {
                    string fieldContent = sudoku.Grid[i, j].Value == 0 ? "" : sudoku.Grid[i, j].Value.ToString();
                    StringBuilder fieldName = new StringBuilder("TextBox");
                    fieldName.Append(i);
                    fieldName.Append(j);
                    TextBox cell = (TextBox)FindName(fieldName.ToString());
                    cell.Text = fieldContent;
                    if (sudoku.Grid[i, j].Constant == true) cell.IsReadOnly = true;
                    else cell.IsReadOnly = false;

                }
            }
        }
        private void InitializeFieldsReadOnly()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    StringBuilder fieldName = new StringBuilder("TextBox");
                    fieldName.Append(i);
                    fieldName.Append(j);
                    TextBox cell = (TextBox)FindName(fieldName.ToString());
                    cell.IsReadOnly = true;
                }
            }
        }



        #endregion

        #region FieldsValidationWhenUserInputNewValue
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            TextBoxError.Text = "";
            TextBox changedField = (TextBox)sender;//validate that it is number;
            if (changedField.IsFocused)
            {
                string num = changedField.Text.ToString();
                int lenght = changedField.Name.ToString().Count();
                int row = Int32.Parse(changedField.Name[lenght - 2].ToString());
                int col = Int32.Parse(changedField.Name[lenght - 1].ToString());
                SingleField oldValue = Sudoku.Grid[row, col];
                if (!Sudoku.InitializeFieldByUser(row, col, num))
                {
                    changedField.Text = "";
                    TextBoxError.Text = "Not Valid";
                }
                else
                {
                    SingleField newValue = num == "" ? new SingleField(0, false) : new SingleField(int.Parse(num), false);
                    Sudoku.AddToUndoStack(new Tuple<int, int, SingleField, SingleField>(row, col, oldValue, newValue));
                }

            }

        }

        #endregion

        #region LoadJson
        private void menuLoad_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxFileName.Text != "")
            {
                string partialName = TextBoxFileName.Text;
                DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo(@"D:\");
                FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles("*" + partialName + "*.*");  
                    stopwatch.Start();
                    dispatcher.Start();
                    stopwatch.Restart();
                    ComboBoxItem selectedDifficulty = (ComboBoxItem)btnLevel.SelectedItem;
                    string difficulty = selectedDifficulty.Content.ToString();
                    Console.WriteLine(difficulty);
                    Sudoku = new SudokuGenerator(difficulty);
                    Sudoku.SetUndoStack();
                    Sudoku.SetRedoStack();
                int i = 0;
                while (i < filesInDir.Length && !LoadJson(filesInDir[i].FullName))
                {
                    i++;
                }
                if (i >= filesInDir.Length ) TextBoxError.Text = "Invalid FileName!";
                else
                {
                    InsertIntoGameBoard(Sudoku);
                    numberOfGames++;
                    TextBoxSolutionsNumber.Text = $"True Solutions:{numberOfTrueGames}";
                }
                
            }
            else TextBoxError.Text="Invalid FileName!";
        }
        private bool LoadJson(string name)
        {
            
            using (StreamReader r = new StreamReader(name))
            {

                string json = r.ReadToEnd();
                SingleField[,] loadedGrid;
                try
                {
                     loadedGrid = JsonConvert.DeserializeObject<SingleField[,]>(json);
                }
                catch (Newtonsoft.Json.JsonException)
                {
                    return false;
                }
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        Sudoku.InitializeFieldByField(i, j, loadedGrid[i, j]);
                    }
                }

            }
            return true;
        }
        #endregion

        #region SaveJson
        private void menuSave_Click(object sender, RoutedEventArgs e)
        {

            if (Sudoku != null)
            {
                using (StreamWriter file = File.CreateText($@"D:\game{numberOfGames}.txt"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    //serialize object directly into file stream
                    serializer.Serialize(file, Sudoku.Grid);

                }
            }

        }
        #endregion

        #region HelpButton
        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            if (Sudoku != null)
            {
                Random rand = new Random();
                int i = rand.Next(0, 9);
                int j = rand.Next(0, 9);
                while (Sudoku.EmptyFieldsSolution[i, j] == null || Sudoku.EmptyFieldsSolution[i, j].Value == 0 && Sudoku.Grid[i, j].Value != 0)
                {
                    i = rand.Next(0, 9);
                    j = rand.Next(0, 9);
                }
                Sudoku.InitializeFieldByUser(i, j, Sudoku.EmptyFieldsSolution[i, j].Value.ToString());
                StringBuilder fieldName = new StringBuilder("TextBox");
                fieldName.Append(i);
                fieldName.Append(j);
                TextBox cell = (TextBox)FindName(fieldName.ToString());
                cell.Text = Sudoku.EmptyFieldsSolution[i, j].Value.ToString();
            }
        }
        #endregion

        #region ExitMenuClick
        private void menuExitGame_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        } 
        #endregion
    }
}

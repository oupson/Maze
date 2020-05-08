using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using MazeLib;
using MazeLib.Explorers;
using MazeLib.generator;

namespace MazeApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Maze maze;
        private UserExplorer user;
        private System.Windows.Threading.DispatcherTimer gameTickTimer = new System.Windows.Threading.DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void GameTickTimer_Tick(object sender, EventArgs e)
        {
            if ((user as IExplorer).Arrived())
            {
                this.KeyDown -= new KeyEventHandler(MainWindow_KeyDown);
            }
            if (user.CanRenderNext() && maze.NotFullyExplored())
            {
                MazeImage.Source = BitmapToBitmapImage(maze.Explore());
            } else if (!maze.NotFullyExplored())
            {
                gameTickTimer.Stop();
                StartGame();
            }
        }

        private void StartGame()
        {
            maze = new PrimsMazeGenerator(20, 20).GenerateMaze();
            maze.CellSize = 50;
            maze.Explorers.Add(new LeftHandExplorer
            {
                ColorPen = Pens.Green
            });
            maze.Explorers.Add(new RightHandExplorer
            {
                ColorPen = Pens.Blue
            });
            user = new UserExplorer();
            maze.Explorers.Add(user);
            maze.Setup();

            Debug.WriteLine("Finished setup");
            MazeImage.Source = BitmapToBitmapImage(maze.RenderCurrent());
            Debug.WriteLine("Finished Setting Source");

            gameTickTimer.Tick += GameTickTimer_Tick;
            gameTickTimer.Interval = TimeSpan.FromMilliseconds(1);
            gameTickTimer.Start();

            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);
        }

        private void MazeImage_Loaded(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down || e.Key == Key.Up || e.Key == Key.Left || e.Key == Key.Right)
            {
                if (user.isAwaiting && !(user as IExplorer).Arrived())
                    user.OnKeyPress(e.Key);
            }
        }

        /// <summary>
        /// Takes a bitmap and converts it to an image that can be handled by WPF ImageBrush
        /// </summary>
        /// <param name="src">A bitmap image</param>
        /// <returns>The image as a BitmapImage for WPF</returns>
        public BitmapImage BitmapToBitmapImage(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            src.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }
    }
}

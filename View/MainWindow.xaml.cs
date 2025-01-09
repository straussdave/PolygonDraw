using System.Windows;
using System.Windows.Input;

namespace PolygonDraw
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        MainViewModel mainViewModel = new MainViewModel();
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this.mainViewModel;
            this.MouseMove += this.mainViewModel.OnMouseMove;
            this.MouseLeftButtonDown += this.mainViewModel.OnMouseLeftClick;
            this.KeyDown += OnKeyDown;

        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Z)
            {
                // Ctrl+Z was pressed
                this.mainViewModel.Undo();
            }
            else if(Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Y)
            {
                // Ctrl+Y was pressed
                this.mainViewModel.Redo();
            }
        }
    }
}
using System;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;


namespace PolygonDraw
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        string? mousePosition;

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            var position = Mouse.GetPosition(Application.Current.MainWindow);
            MousePosition = $"X: {position.X}, Y: {position.Y}";
        }
    }
}




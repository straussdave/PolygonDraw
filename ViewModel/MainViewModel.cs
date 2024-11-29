using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.ComponentModel;
using Point = System.Windows.Point;

namespace PolygonDraw
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        string? mousePosition;

        Stack<Polygon>? polygons = new Stack<Polygon>();

        bool currentlyEditing = false;

        //public void OnMouseMove(object sender, MouseEventArgs e)
        //{
        //    var position = Mouse.GetPosition(Application.Current.MainWindow);
        //    MousePosition = $"X: {position.X}, Y: {position.Y}";
        //}

        public void OnClick(object sender, MouseEventArgs e)
        {
            Point currentPosition = Mouse.GetPosition(Application.Current.MainWindow);
            MousePosition = $"X: {currentPosition.X}, Y: {currentPosition.Y}";
            if (currentlyEditing)
            {
                Polygon currentPolygon = polygons.Peek();
                if (currentPolygon != null)
                {
                    currentPolygon.AddPoint(currentPosition);
                }
                else
                {
                    polygons.Push(new Polygon(currentPosition));
                }
            }
            else
            {
                polygons.Push(new Polygon(currentPosition));
                currentlyEditing = true;
            }
        }

        public void OnDoubleClick(object sender, MouseEventArgs e)
        {
            MousePosition = "Double Clicked";
            currentlyEditing = false;
        }

        public void Undo()
        {
            Polygon currentPolygon = polygons.Peek();
            if (currentPolygon != null)
            {
                currentPolygon.Undo();
            }
        }

        public void Redo()
        {
            Polygon currentPolygon = polygons.Peek();
            if (currentPolygon != null)
            {
                currentPolygon.Redo();
            }

        }


    }
}




using System;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using Point = System.Windows.Point;
using Timer = System.Timers.Timer;
using System.Collections.ObjectModel;
using System.Windows.Ink;

namespace PolygonDraw
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        string? mousePosition;

        public ObservableCollection<UIElement> lines { get; set; }

        [ObservableProperty]
        public UIElement previewLine;

        Stack<Polygon>? polygons = new Stack<Polygon>();

        bool currentlyEditing = false;
        private Timer clickTimer;
        private bool isDoubleClick;


        public MainViewModel()
        {
            clickTimer = new Timer
            {
                Interval = 150,
                AutoReset = false
            };
            clickTimer.Elapsed += OnSingleClickTimeout;
            lines = new ObservableCollection<UIElement>();
            AddLine(10, 10, 100, 100, Brushes.Black, 2);
        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            if(polygons.Count > 0)
            {
                if(currentlyEditing) 
                {
                    Polygon currentPolygon = polygons.Peek();
                    Point lastPoint = currentPolygon.GetLastPoint();
                    Point currentPosition = Mouse.GetPosition(Application.Current.MainWindow);
                    previewLine = new Line
                    {
                        X1 = lastPoint.X,
                        Y1 = lastPoint.Y,
                        X2 = currentPosition.X,
                        Y2 = currentPosition.Y,
                        Stroke = Brushes.Black,
                        StrokeThickness = 2,
                        Visibility = Visibility.Visible
                    };
                }
            }
        }

        public void OnClick()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Point currentPosition = Mouse.GetPosition(Application.Current.MainWindow);
                MousePosition = $"X: {currentPosition.X}, Y: {currentPosition.Y}";
                if (currentlyEditing)
                {
                    Polygon currentPolygon = polygons.Peek();
                    Point lastPoint = currentPolygon.GetLastPoint();
                    currentPolygon.AddPoint(currentPosition);
                    AddLine(lastPoint.X, lastPoint.Y, currentPosition.X, currentPosition.Y, Brushes.Black, 2);
                }
                else
                {
                    polygons.Push(new Polygon(currentPosition));
                    currentlyEditing = true;
                }
            });
        }

        public void OnDoubleClick()
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

        public void OnMouseLeftClick(object sender, MouseEventArgs e)
        {
            if (clickTimer.Enabled)
            {
                clickTimer.Stop();
                isDoubleClick = true;
                OnDoubleClick();
            }
            else
            {
                isDoubleClick = false;
                clickTimer.Start();
            }
        }

        private void OnSingleClickTimeout(object sender, ElapsedEventArgs e)
        {
            if (!isDoubleClick)
            {
                OnClick();
            }
        }

        public void AddLine(double x1, double y1, double x2, double y2, Brush stroke, double thickness)
        {
            var line = new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = stroke,
                StrokeThickness = thickness,
                Visibility = Visibility.Visible
            };

            lines.Add(line);
        }
    }


}




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

        public ObservableCollection<Line> lines { get; set; }
        public ObservableCollection<Line> removedLines { get; set; }

        [ObservableProperty]
        public UIElement previewLine;

        Stack<Polygon>? polygons = new Stack<Polygon>();

        bool currentlyEditing = false; //to track 
        private Timer clickTimer;
        private bool isDoubleClick;

        bool clicked = false;

        private Stack<Point> _displayedPoints = new Stack<Point>();
        private Stack<Point> _removedPoints = new Stack<Point>();


        public MainViewModel()
        {
            clickTimer = new Timer
            {
                Interval = 150,
                AutoReset = false
            };
            clickTimer.Elapsed += OnSingleClickTimeout;
            lines = new ObservableCollection<Line>();
            removedLines = new ObservableCollection<Line>();
        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {

            if (_displayedPoints.Count > 0)
            {
                if (currentlyEditing)
                {
                    Point lastPoint = _displayedPoints.Peek();
                    Point currentPosition = Mouse.GetPosition(Application.Current.MainWindow);
                    if (clicked)
                    {
                        AddLine(lastPoint.X, lastPoint.Y, currentPosition.X, currentPosition.Y, Brushes.Black, 2);
                        clicked = false;
                    }
                    else
                    {
                        Line line = lines.Last();
                        line.X2 = currentPosition.X;
                        line.Y2 = currentPosition.Y;
                    }
                        
                }
            }
            //if(polygons.Count > 0)
            //{
            //    if(currentlyEditing) 
            //    {
            //        Polygon currentPolygon = polygons.Peek();
            //        Point lastPoint = currentPolygon.GetLastPoint();
            //        Point currentPosition = Mouse.GetPosition(Application.Current.MainWindow);
            //        previewLine = new Line
            //        {
            //            X1 = lastPoint.X,
            //            Y1 = lastPoint.Y,
            //            X2 = currentPosition.X,
            //            Y2 = currentPosition.Y,
            //            Stroke = Brushes.Black,
            //            StrokeThickness = 2,
            //            Visibility = Visibility.Visible
            //        };
            //    }
            //}
        }

        public void OnClick()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Point currentPosition = Mouse.GetPosition(Application.Current.MainWindow);
                MousePosition = $"X: {currentPosition.X}, Y: {currentPosition.Y}";
                if (currentlyEditing)
                {
                    //Point lastPoint = currentPolygon.GetLastPoint();
                    _displayedPoints.Push(currentPosition);
                    //Line line = lines.Last();
                    //line.X2 = currentPosition.X;
                    //line.Y2 = currentPosition.Y;
                    clicked = true;
                }
                else
                {
                    _displayedPoints.Push(currentPosition);
                    currentlyEditing = true;
                    clicked = true;
                }
                removedLines.Clear();
            });
        }

        public void OnDoubleClick()
        {
            MousePosition = "Double Clicked";
            currentlyEditing = false;
            lines.RemoveAt(lines.Count - 1);
        }

        public void Undo()
        {
            //if (_displayedPoints.TryPop(out Point lastAddedPoint))
            //{
            //    _removedPoints.Push(lastAddedPoint);
            //    lines.RemoveAt(lines.Count-1);
            //}
            if(lines.Count > 0)
            {
                removedLines.Add(popLines(lines));
            }
                
                
        }

        public void Redo()
        {
            //if (_removedPoints.TryPop(out Point lastAddedPoint))
            //    _displayedPoints.Push(lastAddedPoint);
            if (removedLines.Count > 0)
            {
                lines.Add(popLines(removedLines));
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

        private Line popLines(ObservableCollection<Line> lines)
        {
            Line deletedLine = lines.ElementAt(lines.Count - 1);
            lines.Remove(deletedLine);
            return deletedLine;
        }
    }


}




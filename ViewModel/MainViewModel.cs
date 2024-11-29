using System;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.ComponentModel;
using Point = System.Windows.Point;
using Timer = System.Timers.Timer;
using System.Collections.ObjectModel;

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

        bool currentlyEditing = false;
        private Timer clickTimer;
        private bool isDoubleClick;

        bool clicked = false;

        private Stack<Point> _displayedPoints = new Stack<Point>();
        private Stack<Point> _removedPoints = new Stack<Point>();


        public MainViewModel()
        {
            clickTimer = new Timer
            {
                Interval = 180,
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
        }

        public void OnClick()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Point currentPosition = Mouse.GetPosition(Application.Current.MainWindow);
                if (currentlyEditing)
                {
                    _displayedPoints.Push(currentPosition);
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
            if(lines.Count > 0)
            {
                removedLines.Add(popLines(lines));
            }
        }

        public void Redo()
        {
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




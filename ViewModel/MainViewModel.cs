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

        //public List<ObservableCollection<Line>> Polygons { get; set; }
        public ObservableCollection<Line> Lines { get; set; }
        public Line? PreviewLine { get; set; }

        public ObservableCollection<Line> removedLines { get; set; }

        public int numberOfFinishedPolygons = 0;

        

        bool startPointIsVisible = false;
        private Timer clickTimer;
        private bool isDoubleClick;

       
        private bool addNewPreviewLineOnMouseMove = false; //addNewPreviewLineOnMouseMove is used to check to do the preview line just once

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
            // Beispiel-Linien hinzufügen
            Lines = new ObservableCollection<Line>();

            PreviewLine = null;
            removedLines = new ObservableCollection<Line>();
        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_displayedPoints.Count > 0)
            {
                if (startPointIsVisible)
                {
                    Point lastPoint = _displayedPoints.Peek();
                    Point currentPosition = Mouse.GetPosition(Application.Current.MainWindow);
                    if (addNewPreviewLineOnMouseMove)
                    {
                        //clicked is used to check to do the preview line just once
                        AddLine(lastPoint.X, lastPoint.Y, currentPosition.X, currentPosition.Y, Brushes.Black, 2);
                        PreviewLine = Lines.Last();
                        addNewPreviewLineOnMouseMove = false;
                    }
                    else
                    {
                        if(Lines.Count > 0)
                        {
                            Line line = Lines.Last();
                            line.X2 = currentPosition.X;
                            line.Y2 = currentPosition.Y;
                        }
                    }
                }
            }
        }

        public void OnClick()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Point currentPosition = Mouse.GetPosition(Application.Current.MainWindow);
                if (!startPointIsVisible)
                {
                    startPointIsVisible = true;
                }

                _displayedPoints.Push(currentPosition);
                addNewPreviewLineOnMouseMove = true;
                PreviewLine = null;
                removedLines.Clear();
            });
        }

        public void OnDoubleClick()
        {
            if (startPointIsVisible)
            {
                if (Lines.Count > 0)
                {
                    Lines.RemoveAt(Lines.Count - 1);
                }
                //Polygons.Add(Lines);
                //Lines = new ObservableCollection<Line>();
                numberOfFinishedPolygons++;
                
            }
            PreviewLine = null;
            startPointIsVisible = false; //firstPoint of the newest Polygon was already set
            addNewPreviewLineOnMouseMove = false;
        }

        public void Undo()
        {
            if(Lines.Count > 0)
            {
                if(PreviewLine != null)
                {
                    popLines(Lines);
                    PreviewLine = null;
                    if (Lines.Count + numberOfFinishedPolygons == _displayedPoints.Count())
                    {
                        removedLines.Add(popLines(Lines));
                    }
                }
                else
                {
                    if (Lines.Count + numberOfFinishedPolygons + 1 == _displayedPoints.Count())
                    {
                        removedLines.Add(popLines(Lines));
                    }
                }
                
                
                removeOneDisplayedPoint();
            }
            addNewPreviewLineOnMouseMove = true;
            startPointIsVisible = true;
        }

        public void Redo()
        {
            if (PreviewLine != null)
            {
                popLines(Lines);
                PreviewLine = null;
            }
            if (removedLines.Count > 0)
            {
                Lines.Add(popLines(removedLines));
                addOneRemovedPoint();
            }
            addNewPreviewLineOnMouseMove = true;
            startPointIsVisible = true;

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

            Lines.Add(line);
        }

        private Line popLines(ObservableCollection<Line> lines)
        {
            if(lines.Count > 0)
            {
                Line deletedLine = lines.ElementAt(lines.Count - 1);
                lines.Remove(deletedLine);
                return deletedLine;
            }
            
            return null;
        }

        private void removeOneDisplayedPoint()
        {
            if (_displayedPoints.Count > 0)
            {
                Point point = _displayedPoints.Pop();
                _removedPoints.Push(point);
            }
        }

        private void addOneRemovedPoint()
        {
            if (_removedPoints.Count > 0)
            {
                Point point = _removedPoints.Pop();
                _displayedPoints.Push(point);
            }
        }
    }


}




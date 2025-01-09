using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
using Point = System.Windows.Point;
using System.Windows.Input;

namespace PolygonDraw
{
    public class Polygon2
    {
        public ObservableCollection<Line> Lines { get; set; }
        private Stack<Line> _removedLines;
        private Stack<Point> _displayedPoints;
        private Stack<Point> _removedPoints;
        private Line? _previewLine;

        bool startPointIsVisible = false;
        bool addNewPreviewLineOnMouseMove = false; //addNewPreviewLineOnMouseMove is used to check to do the preview line just once

        public Polygon2()
        {
            Lines = new ObservableCollection<Line>();
            _removedLines = new Stack<Line>();
            _displayedPoints = new Stack<Point>();
            _removedPoints = new Stack<Point>();
            _previewLine = null;
        }
        
      
        public bool Undo()
        {
            if (Lines.Count > 0)
            {
                if (_previewLine != null)
                {
                    popLines(Lines);
                    _previewLine = null;
                }
                //else
                //{
                Line removedLine = popLines(Lines);
                if(removedLine != null)
                {
                    _removedLines.Push(removedLine);
                }
                
                removeOneDisplayedPoint();
                //}
                addNewPreviewLineOnMouseMove = true;
                /*if (Lines.Count == 0)
                {
                    return false;
                }*/
                return true;
            }
            else
            {
                addNewPreviewLineOnMouseMove = true;
                //startPointIsVisible = false;
                return false; //last line got removed
            }
            
            
        }

        public bool Redo()
        {
            if (startPointIsVisible)
            {
                //if previewline is there remove it
                if (_previewLine != null)
                {
                    popLines(Lines);
                    _previewLine = null;
                }
                //if there are lines to redo do so
                if (_removedLines.Count > 0)
                {
                    Line lastRemovedLine = _removedLines.Pop();
                    if (lastRemovedLine != null)
                    {
                        Lines.Add(lastRemovedLine);
                        addOneRemovedPoint();
                        addNewPreviewLineOnMouseMove = true;
                        //startPointIsVisible = true;

                    }
                    else
                    {
                        Console.WriteLine("hey");
                    }
                    return true;
                }
                else
                {
                    
                    addNewPreviewLineOnMouseMove = true;
                    //startPointIsVisible = false;
                    return false;
                }
            }
            else
            {
                //StartPoint gets added
                addOneRemovedPoint();
                addNewPreviewLineOnMouseMove = true;
                //startPointIsVisible = true;
                return true;
            }
            

        }


        public void OnClick(Point currentPosition)
        {
            //adding the first point of a Polygon
            if (!startPointIsVisible)
            {
                startPointIsVisible = true;
            }

            _displayedPoints.Push(currentPosition);
            addNewPreviewLineOnMouseMove = true;
            _previewLine = null;
            _removedLines.Clear();
        }
        public void OnDoubleClick(Point currentPosition)
        {
            OnClick(currentPosition);
            /*if (startPointIsVisible)
            {
                if (Lines.Count > 0)
                {
                    Lines.RemoveAt(Lines.Count - 1);
                }
            }*/
            _previewLine = null;
            addNewPreviewLineOnMouseMove = true;
            //startPointIsVisible = true; //firstPoint of the newest Polygon was already set
        }

        public void OnMouseMove(Point currentPosition)
        {
            if (_displayedPoints.Count > 0)
            {
                if (startPointIsVisible)
                {
                    Point lastPoint = _displayedPoints.Peek();
                    if (addNewPreviewLineOnMouseMove)
                    {
                        //addNewPreviewLineOnMouseMove is used to check to do the preview line just once
                        AddLine(lastPoint.X, lastPoint.Y, currentPosition.X, currentPosition.Y, Brushes.Black, 2);
                        _previewLine = Lines.Last();
                        addNewPreviewLineOnMouseMove = false;
                    }
                    else
                    {
                        if (Lines.Count > 0)
                        {
                            _previewLine = Lines.Last();
                            _previewLine.X2 = currentPosition.X;
                            _previewLine.Y2 = currentPosition.Y;
                        }
                    }
                }
            }
        }


        public void AddLine(Line newLine)
        {
            Lines.Add(newLine);
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
            if (lines.Count > 0)
            {
                Line deletedLine = lines.ElementAt(lines.Count - 1);
                if(deletedLine == null)
                {
                    Console.WriteLine("hey");
                }
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
                if(_displayedPoints.Count == 0)
                {
                    startPointIsVisible = false;
                    addNewPreviewLineOnMouseMove = true;
                }
            }
        }

        private void addOneRemovedPoint()
        {
            if (_removedPoints.Count > 0)
            {
                Point point = _removedPoints.Pop();
                _displayedPoints.Push(point);
                startPointIsVisible= true;
            }
        }
    }
}

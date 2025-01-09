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
    public partial class MainViewModel2 : ObservableObject
    {
        public ObservableCollection<Polygon2> Polygons { get; set; } = new ObservableCollection<Polygon2>();
        private Polygon2 _currentPolygon;
        private Stack<Polygon2> _removedPolygons;
        
        private Timer clickTimer;
        private bool isDoubleClick;


        public MainViewModel2()
        {
            clickTimer = new Timer
            {
                Interval = 180,
                AutoReset = false
            };
            clickTimer.Elapsed += OnSingleClickTimeout;

            _currentPolygon = new Polygon2();
            _removedPolygons = new Stack<Polygon2>();
            Polygons.Add(_currentPolygon);

        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            Point currentPosition = Mouse.GetPosition(Application.Current.MainWindow);
            _currentPolygon.OnMouseMove(currentPosition);
        }
        public void OnDoubleClick()
        {
            Point currentPosition = Mouse.GetPosition(Application.Current.MainWindow);
            _currentPolygon.OnDoubleClick(currentPosition);
            _currentPolygon = new Polygon2();
            Polygons.Add(_currentPolygon);
            /*if (startPointIsVisible)
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
            startPointIsVisible = false; //firstPoint of the newest Polygon was already set*/
        }
        public void OnClick()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Point currentPosition = Mouse.GetPosition(Application.Current.MainWindow);
                _currentPolygon.OnClick(currentPosition);
            });
        }

        public void Undo()
        {
            if (_currentPolygon.Undo())
            {

            }
            else
            {
                
                if (Polygons.Count > 1)
                {
                    _removedPolygons.Push(_currentPolygon);
                    Polygons.Remove(_currentPolygon);
                    _currentPolygon = Polygons.Last();
                }

            }
            
        }

        public void Redo()
        {
            
            if (_currentPolygon.Redo())
            {
                
            }
            else
            {
                if (_removedPolygons.Count>0)
                {
                    _currentPolygon = _removedPolygons.Pop();
                    Polygons.Add(_currentPolygon);
                }
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

        

    
    }


}




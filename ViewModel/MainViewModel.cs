﻿using System;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Point = System.Windows.Point;
using Timer = System.Timers.Timer;
using System.Collections.ObjectModel;

namespace PolygonDraw
{
    public partial class MainViewModel : ObservableObject
    {
        public ObservableCollection<Polygon> Polygons { get; set; } = new ObservableCollection<Polygon>();
        private Polygon _currentPolygon;
        private Stack<Polygon> _removedPolygons;
        
        private Timer clickTimer;
        private bool isDoubleClick;

        public MainViewModel()
        {
            clickTimer = new Timer
            {
                Interval = 180,
                AutoReset = false
            };
            clickTimer.Elapsed += OnSingleClickTimeout;

            _currentPolygon = new Polygon();
            _removedPolygons = new Stack<Polygon>();
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
            _currentPolygon = new Polygon();
            Polygons.Add(_currentPolygon);
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
            if (!_currentPolygon.Undo())
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
            if (!_currentPolygon.Redo())
            {
                if (_removedPolygons.Count > 0)
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




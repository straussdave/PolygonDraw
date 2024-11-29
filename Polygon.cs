using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Point = System.Windows.Point;

namespace PolygonDraw
{
    internal class Polygon
    {
        private Stack<Point> _displayedPoints = new Stack<Point>();
        private Stack<Point> _removedPoints = new Stack<Point>();

        public Polygon(Point startPoint)
        {
            _displayedPoints.Push(startPoint);
        }


        public void Undo()
        {

            Point lastAddedPoint = _displayedPoints.Pop();
            if (lastAddedPoint != null)
            {
                _removedPoints.Push(lastAddedPoint);
            }
        }

        public void Redo()
        {
            Point lastAddedPoint = _removedPoints.Pop();
            if (lastAddedPoint != null)
            {
                _displayedPoints.Push(lastAddedPoint);
            }
        }

        public void AddPoint(Point point)
        {
            _displayedPoints.Push(point);
        }

        public Point GetLastPoint()
        {
            return _displayedPoints.Peek();
        }
    }
}

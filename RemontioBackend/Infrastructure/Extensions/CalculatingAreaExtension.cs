using Application.Interfaces.AdditionalInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Extensions
{
    public class CalculatingAreaExtension
    {
        public double CalculatePolygonArea(List<IPoint> points)
        {
            int n = points.Count;

            if(n < 3)
                throw new ArgumentException("At least three points are required to calculate area of a polygon.");

            double area = 0;

            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n; 
                area += points[i].X * points[j].Y;
                area -= points[j].X * points[i].Y;
            }

            area = Math.Abs(area) / 2.0;

            return area;
        }
    }
}

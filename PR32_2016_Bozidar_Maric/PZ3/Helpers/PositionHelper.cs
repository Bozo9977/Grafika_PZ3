using PZ3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PZ3.Helpers
{
    public class PositionHelper
    {

        private static double minX = 19.793909;
        private static double maxX = 19.894459;
        private static double minY = 45.2325;
        private static double maxY = 45.277031;

        public static void TranslatePositions(NetworkModel model)
        {
            double newX, newY;
            for(int i =0; i < model.substations.Count; i++)
            {

                ToLatLon(model.substations[i].X, model.substations[i].Y, 34, out newY, out newX);
                model.substations[i].X = newX;
                model.substations[i].Y = newY;

                if(model.substations[i].X>=minX && model.substations[i].X<=maxX && model.substations[i].Y >=minY && model.substations[i].Y <= maxY)
                {
                    MainWindow.elements.Add(model.substations[i].Id, new Tuple<string, Entity>("substation", model.substations[i]));
                }
            }

            for(int i=0; i < model.nodes.Count; i++)
            {
                ToLatLon(model.nodes[i].X, model.nodes[i].Y, 34, out newY, out newX);
                model.nodes[i].X = newX;
                model.nodes[i].Y = newY;

                if (model.nodes[i].X >= minX && model.nodes[i].X <= maxX && model.nodes[i].Y >= minY && model.nodes[i].Y <= maxY)
                {
                    MainWindow.elements.Add(model.nodes[i].Id, new Tuple<string, Entity>("node", model.nodes[i]));
                }
            }

            for (int i = 0; i < model.switches.Count; i++)
            {
                ToLatLon(model.switches[i].X, model.switches[i].Y, 34, out newY, out newX);
                model.switches[i].X = newX;
                model.switches[i].Y = newY;

                if (model.switches[i].X >= minX && model.switches[i].X <= maxX && model.switches[i].Y >= minY && model.switches[i].Y <= maxY)
                {
                    MainWindow.elements.Add(model.switches[i].Id, new Tuple<string, Entity>("switch", model.switches[i]));
                }
            }


            List<Point> currVertices = new List<Point>();
            List<Point> translatedVertices = new List<Point>();

            for(int i=0; i<model.lines.Count; i++)
            {
                currVertices = model.lines[i].Vertices;

                foreach(var pointNode in currVertices)
                {
                    Point p = new Point();

                    p.X = pointNode.X;
                    p.Y = pointNode.Y;

                    ToLatLon(p.X, p.Y, 34, out newY, out newX);

                    if(newX >= minX && newX <= maxX && newY >= minY && newY <= maxY)
                    {
                        translatedVertices.Add(new Point(newX, newY));
                    }
                }

                model.lines[i].Vertices = translatedVertices;
                currVertices = new List<Point>();
                translatedVertices = new List<Point>();
            }
        }

        public static void ToLatLon(double utmX, double utmY, int zoneUTM, out double latitude, out double longitude)
        {
            bool isNorthHemisphere = true;

            var diflat = -0.00066286966871111111111111111111111111;
            var diflon = -0.0003868060578;

            var zone = zoneUTM;
            var c_sa = 6378137.000000;
            var c_sb = 6356752.314245;
            var e2 = Math.Pow((Math.Pow(c_sa, 2) - Math.Pow(c_sb, 2)), 0.5) / c_sb;
            var e2cuadrada = Math.Pow(e2, 2);
            var c = Math.Pow(c_sa, 2) / c_sb;
            var x = utmX - 500000;
            var y = isNorthHemisphere ? utmY : utmY - 10000000;

            var s = ((zone * 6.0) - 183.0);
            var lat = y / (c_sa * 0.9996);
            var v = (c / Math.Pow(1 + (e2cuadrada * Math.Pow(Math.Cos(lat), 2)), 0.5)) * 0.9996;
            var a = x / v;
            var a1 = Math.Sin(2 * lat);
            var a2 = a1 * Math.Pow((Math.Cos(lat)), 2);
            var j2 = lat + (a1 / 2.0);
            var j4 = ((3 * j2) + a2) / 4.0;
            var j6 = ((5 * j4) + Math.Pow(a2 * (Math.Cos(lat)), 2)) / 3.0;
            var alfa = (3.0 / 4.0) * e2cuadrada;
            var beta = (5.0 / 3.0) * Math.Pow(alfa, 2);
            var gama = (35.0 / 27.0) * Math.Pow(alfa, 3);
            var bm = 0.9996 * c * (lat - alfa * j2 + beta * j4 - gama * j6);
            var b = (y - bm) / v;
            var epsi = ((e2cuadrada * Math.Pow(a, 2)) / 2.0) * Math.Pow((Math.Cos(lat)), 2);
            var eps = a * (1 - (epsi / 3.0));
            var nab = (b * (1 - epsi)) + lat;
            var senoheps = (Math.Exp(eps) - Math.Exp(-eps)) / 2.0;
            var delt = Math.Atan(senoheps / (Math.Cos(nab)));
            var tao = Math.Atan(Math.Cos(delt) * Math.Tan(nab));

            longitude = ((delt * (180.0 / Math.PI)) + s) + diflon;
            latitude = ((lat + (1 + e2cuadrada * Math.Pow(Math.Cos(lat), 2) - (3.0 / 2.0) * e2cuadrada * Math.Sin(lat) * Math.Cos(lat) * (tao - lat)) * (tao - lat)) * (180.0 / Math.PI)) + diflat;
        }
    }
}

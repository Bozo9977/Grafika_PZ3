using PZ3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PZ3.Helpers
{
    public class DrawHelper
    {
        

        public static void DrawElements(Model3DGroup map, double minX, double maxX, double minY, double maxY, int xDimension, int yDimension, int zDimension)
        {
            foreach(Tuple<string, Entity> entity in MainWindow.elements.Values)
            {
                DrawEntity(entity, map, minX, maxX, minY, maxY, xDimension, yDimension, zDimension);
            }

            foreach (var line in MainWindow.allLines.Values)
            {
                DrawLine(line, map, minX, maxX, minY, maxY, xDimension, yDimension, zDimension);
            }
        }

        private static void DrawEntity(Tuple<string, Entity> entity, Model3DGroup map, double minX, double maxX, double minY, double maxY, int xDimension, int yDimension, int zDimension)
        {
            double multiply = 0.02;
            double addsub = 0.005;

            Point3D point = new Point3D();
            point.X = Math.Round(Scale(entity.Item2.X, minX, maxX, 0, xDimension - 1));
            point.Z = zDimension - 1 - Math.Round(Scale(entity.Item2.Y, minY, maxY, 0, zDimension - 1));
            point.Y = 0;

            while(MainWindow.matrix[(int)point.X, (int)point.Z, (int)point.Y] != null)
            {
                point.Y++;
            }

            MainWindow.matrix[(int)point.X, (int)point.Z, (int)point.Y] = new Tuple<string, object>(entity.Item1, entity.Item2);

            GeometryModel3D node = new GeometryModel3D();
            MeshGeometry3D mesh = new MeshGeometry3D();

            mesh.Positions.Add(new Point3D(point.X * multiply - addsub, (point.Y + 1) * multiply - addsub, point.Z * multiply - addsub));
            mesh.Positions.Add(new Point3D(point.X * multiply + addsub, (point.Y + 1) * multiply - addsub, point.Z * multiply - addsub));
            mesh.Positions.Add(new Point3D(point.X * multiply - addsub, (point.Y + 1) * multiply + addsub, point.Z * multiply - addsub));
            mesh.Positions.Add(new Point3D(point.X * multiply + addsub, (point.Y + 1) * multiply + addsub, point.Z * multiply - addsub));
            mesh.Positions.Add(new Point3D(point.X * multiply - addsub, (point.Y + 1) * multiply - addsub, point.Z * multiply + addsub));
            mesh.Positions.Add(new Point3D(point.X * multiply + addsub, (point.Y + 1) * multiply - addsub, point.Z * multiply + addsub));
            mesh.Positions.Add(new Point3D(point.X * multiply - addsub, (point.Y + 1) * multiply + addsub, point.Z * multiply + addsub));
            mesh.Positions.Add(new Point3D(point.X * multiply + addsub, (point.Y + 1) * multiply + addsub, point.Z * multiply + addsub));

            // Back
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);

            // Right
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(7);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(7);
            mesh.TriangleIndices.Add(5);


            // Front
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(7);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(7);
            mesh.TriangleIndices.Add(6);

            // Left
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(2);

            // Top
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(7);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(7);
            mesh.TriangleIndices.Add(3);

            // Bottom
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(4);

            node.Geometry = mesh;

            if (entity.Item1 == "node")
                node.Material = new DiffuseMaterial(Brushes.Green);
            else if (entity.Item1 == "substation")
                node.Material = new DiffuseMaterial(Brushes.Red);
            else if (entity.Item1 == "switch")
                node.Material = new DiffuseMaterial(Brushes.Blue);

            map.Children.Add(node);
            MainWindow.nodes.Add(entity.Item2.Id, node);
        }

        private static void DrawLine(LineEntity lineEntity, Model3DGroup map, double minX, double maxX, double minY, double maxY, int xDimension, int yDimension, int zDimension)
        {
            double multiply = 0.02;
            double addsub = 0.0025;

            GeometryModel3D line = new GeometryModel3D();
            MeshGeometry3D mesh = new MeshGeometry3D();

            for(int i=0; i< lineEntity.Vertices.Count; i++)
            {
                Point3D point = new Point3D();

                point.X = Math.Round(Scale(lineEntity.Vertices[i].X, minX, maxX, 0, xDimension - 1));
                point.Z = zDimension - 1 - Math.Round(Scale(lineEntity.Vertices[i].Y, minY, maxY, 0, zDimension - 1));
                point.Y = 0;

                mesh.Positions.Add(new Point3D(point.X * multiply, (point.Y + 1) * multiply + addsub, point.Z * multiply));
                mesh.Positions.Add(new Point3D(point.X * multiply, (point.Y + 1) * multiply - addsub, point.Z * multiply));
            }

            for (int i = 0; i < mesh.Positions.Count - 2; i++)
            {
                mesh.TriangleIndices.Add(i);
                mesh.TriangleIndices.Add(i + 2);
                mesh.TriangleIndices.Add(i + 1);
                mesh.TriangleIndices.Add(i);
                mesh.TriangleIndices.Add(i + 1);
                mesh.TriangleIndices.Add(i + 2);
            }


            line.Geometry = mesh;

            line.Material = new DiffuseMaterial(Brushes.Purple);

            map.Children.Add(line);
            MainWindow.lines.Add(lineEntity.Id, line);
        }



        private static double Scale(double value, double min, double max, int minScale, int maxScale)
        {
            return minScale + (double)(value - min) / (max - min) * (maxScale - minScale);
        }
    }
}

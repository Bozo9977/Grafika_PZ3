using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PZ3.Model
{
    [Serializable]
    [XmlRoot("NetworkModel")]
    public class LineEntity
    {
        private ulong id;
        private string name;
        private bool isUnderground;
        private double r;
        private string conductorMaterial;
        private string lineType;
        private int thermalConstantHeat;
        private ulong firstEnd;
        private ulong secondEnd;
        private List<System.Windows.Point> vertices = new List<System.Windows.Point>();

        public ulong Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public bool IsUnderground { get => isUnderground; set => isUnderground = value; }
        public double R { get => r; set => r = value; }
        public string ConductorMaterial { get => conductorMaterial; set => conductorMaterial = value; }
        public string LineType { get => lineType; set => lineType = value; }
        public int ThermalConstantHeat { get => thermalConstantHeat; set => thermalConstantHeat = value; }
        public ulong FirstEnd { get => firstEnd; set => firstEnd = value; }
        public ulong SecondEnd { get => secondEnd; set => secondEnd = value; }

        
        public List<System.Windows.Point> Vertices { get => vertices; set => vertices = value; }

        public LineEntity()
        {
            this.vertices = new List<System.Windows.Point>();
        }

        public override string ToString()
        {
            return String.Format($"{Id}, {Name}, {ConductorMaterial}, {LineType}");
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PZ3.Model
{
    [Serializable]
    [XmlRoot("NetworkModel")]
    public class NodeEntity: Entity
    {
        private ulong id;
        private string name;
        private double x;
        private double y;
        private int row;
        private int column;

        [XmlIgnore]
        public int Row { get => row; set => row = value; }
        [XmlIgnore]
        public int Column { get => column; set => column = value; }


        public NodeEntity() { }

        public NodeEntity(ulong id, string name, double x, double y)
        {
            this.id = id;
            this.name = name;
            this.x = x;
            this.y = y;
        }
    }
}

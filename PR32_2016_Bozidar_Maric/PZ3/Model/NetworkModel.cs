using PZ3.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PZ3.Model
{
    [Serializable]
    public class NetworkModel
    {
        public static NetworkModel InitModel(string path)
        {
            NetworkModel networkModel = LoadXMLHelper.Load<NetworkModel>(path);
            PositionHelper.TranslatePositions(networkModel);
            LineRedundancyCheck(networkModel);
            return networkModel;
        }

        [XmlArray("Substations")]
        [XmlArrayItem("SubstationEntity", typeof(SubstationEntity))]
        public List<SubstationEntity> substations { get; set; }

        [XmlArray("Nodes")]
        [XmlArrayItem("NodeEntity", typeof(NodeEntity))]
        public List<NodeEntity> nodes { get; set; }

        [XmlArray("Switches")]
        [XmlArrayItem("SwitchEntity", typeof(SwitchEntity))]
        public List<SwitchEntity> switches { get; set; }

        [XmlArray("Lines")]
        [XmlArrayItem("LineEntity", typeof(LineEntity))]
        public List<LineEntity> lines { get; set; }


        public static void LineRedundancyCheck(NetworkModel model)
        {
            
            foreach(var line in model.lines)
            {
                if (!MainWindow.elements.ContainsKey(line.FirstEnd) || !MainWindow.elements.ContainsKey(line.SecondEnd))
                {
                    continue;
                }
                    

                bool exist = false;

                foreach(LineEntity item in MainWindow.allLines.Values)
                {
                    if((item.FirstEnd == line.FirstEnd || item.FirstEnd==line.SecondEnd) &&
                        (item.SecondEnd == line.FirstEnd || item.SecondEnd == line.SecondEnd))
                    {
                        exist = true;
                        break;
                    }
                }

                if (exist)
                {
                    continue;
                }
                    

                MainWindow.allLines.Add(line.Id, line);
                MainWindow.elements[line.FirstEnd].Item2.Connections++;
                MainWindow.elements[line.SecondEnd].Item2.Connections++;
                Console.WriteLine(line.Id);
            }

            
            //model.lines = new List<LineEntity>();
            
        }
    }
}

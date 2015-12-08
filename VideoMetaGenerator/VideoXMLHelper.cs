using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace VideoMetaGenerator
{
    class VideoXMLHelper
    {
        private static readonly VideoXMLHelper _instance = new VideoXMLHelper("data.xml");

        private string _BaseString = @"<?xml version='1.0' encoding='UTF-8' ?>
<videos>
</videos>
<markers>
</markers>
";
        private XmlDocument _MetaData;
        public XmlNode VideoNode;
        public XmlNode MarkerNode;

        public XmlDocument MetaData
        {
            get;
        }

        public static VideoXMLHelper Instance
        {
            get
            {
                return _instance;
            }
        }

        private VideoXMLHelper(string file_name)
        {            
            _MetaData = new XmlDocument();
            _MetaData.LoadXml(_BaseString);

            VideoNode = _MetaData.GetElementsByTagName("videos")[0];
            MarkerNode = _MetaData.GetElementsByTagName("markers")[0];
        }

        public void AddVideoNode(List<OpenCvSharp.VideoCapture> video_list, List<string> video_name)
        {
            foreach(OpenCvSharp.VideoCapture video in video_list)
            {
                XmlNode video_node = _MetaData.CreateNode(XmlNodeType.Element, "video", null);
                XmlAttribute attribute = _MetaData.CreateAttribute("seq");
                attribute.Value = video_list.IndexOf(video).ToString();
                video_node.Attributes.Append(attribute);
                XmlAttribute attribute_frame = _MetaData.CreateAttribute("frame");
                attribute.Value = video.FrameCount.ToString();
                video_node.Attributes.Append(attribute_frame);
                video_node.InnerText = "";
                VideoNode.AppendChild(video_node);
            }
        }

        public void AddMarkerNode()
        {
            XmlNode marker_node = _MetaData.CreateNode(XmlNodeType.Element, "marker", null);
            XmlAttribute marker_id_attr = marker_node.OwnerDocument.CreateAttribute("id");
            marker_id_attr.Value = "";
            marker_node.Attributes.Append(marker_id_attr);

            XmlNode track_node = marker_node.OwnerDocument.CreateNode(XmlNodeType.Element, "track", null);
            XmlAttribute track_video = track_node.OwnerDocument.CreateAttribute("video");
            XmlAttribute track_position_x = track_node.OwnerDocument.CreateAttribute("position_x");
            XmlAttribute track_position_y = track_node.OwnerDocument.CreateAttribute("position_y");
            XmlAttribute track_frame = track_node.OwnerDocument.CreateAttribute("frame");

            track_node.Attributes.Append(track_video);
            track_node.Attributes.Append(track_position_x);
            track_node.Attributes.Append(track_position_y);
            track_node.Attributes.Append(track_frame);
            MarkerNode.AppendChild(marker_node);
        }

        public void XMLSave()
        {
            _MetaData.Save("data.xml");
        }
    }
}

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
        private static VideoXMLHelper _instance = new VideoXMLHelper("data.xml");

        private string _BaseString = @"<?xml version='1.0' encoding='UTF-8' ?>
<data>
<videos>
</videos>
<markers>
</markers>
</data>
";
        private XmlDocument _MetaData;
        public XmlNode VideoNode;
        public XmlNode MarkerNode;

        public XmlDocument MetaData
        {
            get
            {
                return _MetaData;
            }
        }

        public static VideoXMLHelper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new VideoXMLHelper("data.xml");
                return _instance;
            }
        }

        private VideoXMLHelper(string file_name)
        {            
            _MetaData = new XmlDocument();
            try
            {
                _MetaData.LoadXml(_BaseString);
            }
            catch (XmlException ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.StackTrace);
            }

            VideoNode = _MetaData.GetElementsByTagName("videos")[0];
            MarkerNode = _MetaData.GetElementsByTagName("markers")[0];
        }

        public void AddVideoNode(OpenCvSharp.VideoCapture video, string video_name, int index)
        {
            XmlNode video_node = _MetaData.CreateNode(XmlNodeType.Element, "video", null);
            XmlAttribute attribute = _MetaData.CreateAttribute("seq");
            attribute.Value = index.ToString();
            video_node.Attributes.Append(attribute);
            XmlAttribute attribute_frame = _MetaData.CreateAttribute("frame");
            attribute_frame.Value = video.Fps.ToString();
            video_node.Attributes.Append(attribute_frame);
            video_node.InnerText = video_name;
            VideoNode.AppendChild(video_node);
        }

        public void AddMarkerNode(VideoMarkerTracker.MarkerTrackerStructure s)
        {
            XmlNode marker_node = _MetaData.CreateNode(XmlNodeType.Element, "marker", null);
            XmlAttribute marker_id_attr = marker_node.OwnerDocument.CreateAttribute("id");
            marker_id_attr.Value = s.marker_id.ToString();
            marker_node.Attributes.Append(marker_id_attr);

            foreach(MarkerStructure t in s.marker_list) {
                XmlNode track_node = marker_node.OwnerDocument.CreateNode(XmlNodeType.Element, "track", null);
                XmlAttribute track_video = track_node.OwnerDocument.CreateAttribute("video");
                track_video.Value = t.videoId.ToString();
                XmlAttribute track_position_x = track_node.OwnerDocument.CreateAttribute("position_x");
                track_position_x.Value = t.marker_position.X.ToString();
                XmlAttribute track_position_y = track_node.OwnerDocument.CreateAttribute("position_y");
                track_position_y.Value = t.marker_position.Y.ToString();
                XmlAttribute track_frame = track_node.OwnerDocument.CreateAttribute("frame");
                track_frame.Value = t.frameSeq.ToString();

                track_node.Attributes.Append(track_video);
                track_node.Attributes.Append(track_position_x);
                track_node.Attributes.Append(track_position_y);
                track_node.Attributes.Append(track_frame);
                marker_node.AppendChild(track_node);
            }

            MarkerNode.AppendChild(marker_node);
        }

        public void XMLSave()
        {
            _MetaData.Save("data.xml");
        }
    }
}

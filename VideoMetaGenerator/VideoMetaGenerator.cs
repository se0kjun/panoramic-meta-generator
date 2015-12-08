using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoMetaGenerator
{
    public class VideoPacking
    {
        public string FilePath;
        public int VideoIdx;
        public OpenCvSharp.VideoCapture VideoInstance;

        public VideoPacking(string _filePath, int _idx, OpenCvSharp.VideoCapture _video)
        {
            FilePath = _filePath;
            VideoIdx = _idx;
            VideoInstance = _video;
        }
    };

    class VideoMetaGenerator : VideoMarkerTracker
    {
        private List<string> m_text_data;
        private List<OpenCvSharp.VideoCapture> m_data;
        private List<VideoPacking> InstanceList;

        public VideoMetaGenerator(List<OpenCvSharp.VideoCapture> input, List<string> filePath, List<VideoPacking> pack)
            : base(pack, 5)
        {
            InstanceList = pack;
            m_data = input;
            m_text_data = filePath;
            Worker();
        }

        public void Worker()
        {
            foreach (VideoPacking p in InstanceList)
            {
                VideoXMLHelper.Instance.AddVideoNode(p.VideoInstance, p.FilePath, p.VideoIdx);
            }
            Dictionary<int, List<MarkerStructure>> result = new Dictionary<int, List<MarkerStructure>>();
            GetMarkerList();
            GetTrackerList();

            foreach (MarkerTrackerStructure s in tracker_result)
            {
                VideoXMLHelper.Instance.AddMarkerNode(s);
            }
        }
    }
}

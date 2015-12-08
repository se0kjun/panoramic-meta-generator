using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Aruco.Net;

namespace VideoMetaGenerator
{
    class VideoMarkerTracker
    {
        protected Dictionary<int, List<MarkerStructure>> marker_result;
        private List<VideoPacking> m_data;

        public VideoMarkerTracker(List<VideoPacking> pack)
        {
            m_data = pack;
        }

        private Dictionary<int, List<MarkerStructure>> MarkerTracker(OpenCvSharp.Mat frame, int video_idx, int frame_number)
        {
            using (var detector = new Aruco.Net.MarkerDetector())
            {
                Dictionary<int, List<MarkerStructure>> MarkerDict = new Dictionary<int, List<MarkerStructure>>();

                var cameraMatrix = new OpenCV.Net.Mat(3, 3, OpenCV.Net.Depth.F32, 1);
                var distortion = new OpenCV.Net.Mat(1, 4, OpenCV.Net.Depth.F32, 1);

                detector.ThresholdMethod = ThresholdMethod.AdaptiveThreshold;
                detector.Param1 = 7.0;
                detector.Param2 = 7.0;
                detector.MinSize = 0.04f;
                detector.MaxSize = 0.5f;
                detector.CornerRefinement = CornerRefinementMethod.Lines;

                // Detect markers in a sequence of camera images.
                var markerSize = 10;
                var image2 = 
                    new OpenCV.Net.Mat(new OpenCV.Net.Size(frame.Width, frame.Height), 
                    (OpenCV.Net.Depth)frame.Depth(), frame.Channels(), frame.Data);

                var detectedMarkers = detector.Detect(image2, cameraMatrix, distortion, markerSize);
                foreach (var marker in detectedMarkers)
                {
                    //event trigger
                    List<MarkerStructure> tmp = new List<MarkerStructure>();
                    if (MarkerDict.TryGetValue(marker.Id, out tmp))
                    {
                        tmp.Add(new MarkerStructure(marker.Id, video_idx, frame_number,
                            new OpenCV.Net.Point2f(marker.Center.X - (marker.Size / 2), marker.Center.Y - (marker.Size / 2)),
                            new OpenCV.Net.Size((int)marker.Size, (int)marker.Size)));
                        MarkerDict[marker.Id] = tmp;
                    }
                    else
                    {
                        List<MarkerStructure> new_list = new List<MarkerStructure>();
                        tmp.Add(new MarkerStructure(marker.Id, video_idx, frame_number,
                            new OpenCV.Net.Point2f(marker.Center.X - (marker.Size / 2), marker.Center.Y - (marker.Size / 2)),
                            new OpenCV.Net.Size((int)marker.Size, (int)marker.Size)));
                        MarkerDict.Add(marker.Id, new_list);
                    }
                }

                return MarkerDict;
            }
        }
        
        protected Dictionary<int, List<MarkerStructure>> GetMarkerList()
        {
            int frame_number = 0;

            foreach (VideoPacking p in m_data)
            {
                OpenCvSharp.Mat read_image = 
                    new OpenCvSharp.Mat(new OpenCvSharp.Size(p.VideoInstance.FrameWidth, p.VideoInstance.FrameHeight), OpenCvSharp.MatType.CV_8U);
                frame_number = 0;
                while (true)
                {
                    p.VideoInstance.Read(read_image);
                    Dictionary<int, List<MarkerStructure>> tmp = MarkerTracker(read_image, p.VideoIdx, frame_number);
                    marker_result = AddDictionary(marker_result, tmp);
                    frame_number++;
                    if (read_image.Empty())
                        break;
                }
            }
            
            foreach(int key in marker_result.Keys)
            {
                marker_result[key].Sort();
            }

            return marker_result;
        }

        private Dictionary<int, List<MarkerStructure>> AddDictionary(
            Dictionary<int, List<MarkerStructure>> a, Dictionary<int, List<MarkerStructure>> b)
        {
            foreach(int key in b.Keys)
            {
                List<MarkerStructure> tmp = new List<MarkerStructure>();
                if (a.ContainsKey(key))
                {
                    a[key].AddRange(b[key]);
                }
                else
                {
                    List<MarkerStructure> new_list = new List<MarkerStructure>();
                    b.TryGetValue(key, out new_list);
                    a.Add(key, new_list);
                }
            }
            
            return a;
        }
    }
}

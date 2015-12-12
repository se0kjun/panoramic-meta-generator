#define DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Aruco.Net;

namespace VideoMetaGenerator
{
    class VideoMarkerTracker
    {
        public class MarkerTrackerStructure
        {
            public int marker_id;
            public List<MarkerStructure> marker_list;

            public MarkerTrackerStructure(int id, List<MarkerStructure> m)
            {
                marker_id = id;
                marker_list = m;
            }

            public MarkerTrackerStructure(int id)
            {
                marker_id = id;
                marker_list = new List<MarkerStructure>();
            }
        };

        protected Dictionary<int, List<MarkerStructure>> marker_result;
        protected List<MarkerTrackerStructure> tracker_result;
        private List<VideoPacking> m_data;
        private int min_tracker_size;

        public VideoMarkerTracker(List<VideoPacking> pack, int min_size)
        {
            marker_result = new Dictionary<int, List<MarkerStructure>>();
            tracker_result = new List<MarkerTrackerStructure>();
            min_tracker_size = min_size;
            m_data = pack;
        }

        private Dictionary<int, List<MarkerStructure>> MarkerTracker(OpenCvSharp.Mat frame, int video_idx, int frame_number)
        {
            if (frame.Empty())
                return null;

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
                    if (!MarkerDict.TryGetValue(marker.Id, out tmp))
                    {
                        if (tmp != null)
                        {
                            tmp.Add(new MarkerStructure(marker.Id, video_idx, frame_number,
                                new OpenCV.Net.Point2f(marker.Center.X - (marker.Size / 2), marker.Center.Y - (marker.Size / 2)),
                                new OpenCV.Net.Size((int)marker.Size, (int)marker.Size)));
                            MarkerDict[marker.Id] = tmp;
                        }
                        else
                        {
                            List<MarkerStructure> tmp2 = new List<MarkerStructure>();
                            tmp2.Add(new MarkerStructure(marker.Id, video_idx, frame_number,
                                new OpenCV.Net.Point2f(marker.Center.X - (marker.Size / 2), marker.Center.Y - (marker.Size / 2)),
                                new OpenCV.Net.Size((int)marker.Size, (int)marker.Size)));
                            MarkerDict[marker.Id] = tmp2;
                        }
                    }
                    else
                    {
                        List<MarkerStructure> new_list = new List<MarkerStructure>();
                        new_list.Add(new MarkerStructure(marker.Id, video_idx, frame_number,
                            new OpenCV.Net.Point2f(marker.Center.X - (marker.Size / 2), marker.Center.Y - (marker.Size / 2)),
                            new OpenCV.Net.Size((int)marker.Size, (int)marker.Size)));
                        MarkerDict.Add(marker.Id, new_list);
                    }
                }

                return MarkerDict;
            }
        }

        private Dictionary<int, List<MarkerStructure>> AddDictionary(
    Dictionary<int, List<MarkerStructure>> a, Dictionary<int, List<MarkerStructure>> b)
        {
            foreach (int key in b.Keys)
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


        protected Dictionary<int, List<MarkerStructure>> GetMarkerList()
        {
            int frame_number = 0;

            foreach (VideoPacking p in m_data)
            {
                OpenCvSharp.Mat read_image = 
                    new OpenCvSharp.Mat(new OpenCvSharp.Size(p.VideoInstance.FrameWidth, p.VideoInstance.FrameHeight), OpenCvSharp.MatType.CV_8U);
                frame_number = 0;
                if (!p.VideoInstance.IsOpened())
                {
                    System.Windows.Forms.MessageBox.Show("not opened");
                    break;
                }

#if DEBUG
                var window = new OpenCvSharp.Window("debug");
                int sleepTime = (int)Math.Round(1000 / p.VideoInstance.Fps);
#endif
                while (true)
                {
                    p.VideoInstance.Grab();
                    OpenCvSharp.NativeMethods.videoio_VideoCapture_operatorRightShift_Mat(p.VideoInstance.CvPtr, read_image.CvPtr);
                    Dictionary<int, List<MarkerStructure>> tmp = MarkerTracker(read_image, p.VideoIdx, frame_number);
#if DEBUG
                    if (!read_image.Empty())
                    {
                        window.ShowImage(read_image);
                        foreach (int key in tmp.Keys)
                        {
                            foreach (MarkerStructure aa in tmp[key])
                            {
                                OpenCvSharp.Cv2.Rectangle(read_image, new OpenCvSharp.Rect(
                                    new OpenCvSharp.Point(aa.marker_position.X, aa.marker_position.Y), new OpenCvSharp.Size(aa.marker_size.Height, aa.marker_size.Width)), new OpenCvSharp.Scalar(255.0f, 0.0f, 0.0f), 3);
                            }
                        }
                        OpenCvSharp.Cv2.WaitKey(sleepTime);
                    }
#endif
                    if(tmp != null)
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

        protected List<MarkerTrackerStructure> GetTrackerList()
        {
            foreach (int key in marker_result.Keys)
            {
                MarkerStructure prev = null;
                MarkerTrackerStructure result = new MarkerTrackerStructure(key);
                foreach(MarkerStructure s in marker_result[key]) 
                {
                    if (s.Equals(prev) || result.marker_list.Count == 0)
                    {
                        result.marker_list.Add(s);
                    }
                    else
                    {
                        tracker_result.Add(result);
                        result = new MarkerTrackerStructure(key);
                        result.marker_list.Add(s);
                    }
                    prev = s;
                }
            }

            tracker_result.RemoveAll(item => item.marker_list.Count < 5);
            return tracker_result;
        }
    }
}

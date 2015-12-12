using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OpenCvSharp;

namespace VideoMetaGenerator
{
    public partial class Form1 : Form
    {
        public List<string> m_textbox;
        public List<VideoCapture> m_videoList;

        public Form1()
        {
            m_textbox = new List<String>();
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
                m_textbox.Add(openFileDialog1.FileName);
                listBox1.Items.Add(openFileDialog1.FileName);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            m_videoList = new List<VideoCapture>();
            List<VideoPacking> instance_list = new List<VideoPacking>();

            foreach(string a in m_textbox)
            {
                VideoCapture tmp = new VideoCapture(a);
                MessageBox.Show(tmp.Fps.ToString());
                m_videoList.Add(tmp);
            }

            for (int i = 0; i < m_videoList.Count; i++)
            {
                instance_list.Add(new VideoPacking(m_textbox[i], i, m_videoList[i]));
            }
            VideoMetaGenerator meta = new VideoMetaGenerator(m_videoList, m_textbox, instance_list);
            meta.Worker();
        }
    }
}

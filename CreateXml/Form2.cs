using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace CreateXml
{

    public partial class Form2 : Form
    {
        public List<System.String> volArry = new List<System.String>();
        public List<System.String> bayArry = new List<System.String>();
        public List<System.String> devArry = new List<System.String>();
        public string moudleBay;
        public string newBay;
        public string baysn;
        public string devsn;
        public string patsn;
        public XmlElement tmpBayNode;
  
        public Form2(List<System.String> vol, List<System.String> bay)
        {    
            foreach(string v in vol)
            {
                volArry.Add(v);
            }
            foreach(string b in bay)
            {
                bayArry.Add(b);
            }

            InitializeComponent();

            foreach (string v in volArry)
            {
                this.comboBox1.Items.Add(v);
            }

            foreach (string b in bayArry)
            {
                this.comboBox2.Items.Add(b);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Application.StartupPath + "/module.xml");
            
            moudleBay = comboBox2.Text;
            newBay = textBox1.Text;
            XmlNodeList bayList = doc.GetElementsByTagName("bay");
            foreach(XmlElement baynode in bayList)
            {
                if (baynode.GetAttribute("name").Equals(moudleBay))
                {
                    tmpBayNode = doc.CreateElement("bay") ;                    
                    baysn = baynode.GetAttribute("sn").Split('.')[0] + "." + baynode.GetAttribute("sn").Split('.')[1] + "." + "B" + newBay;
                    tmpBayNode.SetAttribute("type", "bay");
                    tmpBayNode.SetAttribute("name", newBay);
                    tmpBayNode.SetAttribute("sn", baysn);
                    tmpBayNode.InnerXml = baynode.InnerXml;
                    SetCopyBay(tmpBayNode);
                    baynode.ParentNode.AppendChild(tmpBayNode);
                    doc.Save(Application.StartupPath + "/module.xml");

                    MessageBox.Show("新间隔" + newBay + "添加成功");
                    break;
                }
            }

        //   Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void SetCopyBay(XmlElement copybay)
        {
            XmlNodeList devList = copybay.GetElementsByTagName("device");
            XmlNodeList patList = copybay.GetElementsByTagName("patrolpoint");
            foreach (XmlElement dev in devList)
            {
                devsn = baysn + "." + dev.GetAttribute("sn").Split('.')[3];
                dev.SetAttribute("sn", devsn);
            }
            foreach (XmlElement pat in patList)
            {
                patsn = devsn + "." + pat.GetAttribute("sn").Split('.')[4];
                pat.SetAttribute("sn", patsn);
            }
        }
    }
}

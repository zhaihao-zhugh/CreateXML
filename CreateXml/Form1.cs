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
using System.IO;
using System.Reflection;






namespace CreateXml
{
    public partial class Form1 : Form
    {
        public string subName;
        public string snName;
        public string bayName;
        public string volLevel;
        public string devName;
        public string devDescribe;
        public string patPoint;
        public string patrolheight;
        public string patroltype;
        public string ivstype;
        public string unitname;
        public string path;
        public string volLevelDes;
        public int devCount = 0;
        public int patCount;
        public Boolean isNewDev;
        public Boolean isNewPat;
        public Boolean isNewTmp;
        public int changeCount = 0;
        public int index = 0;
        public enum element { substation, voltagelevel, bay, device, patrolpoint, patrolheight, patroltype, ivstype, unitname }
        public string[] typename = new string[] { "analogmeterwithneedle", "breaker", "desiccant", "digitmeterwithbit", "electronicfence",
            "framedifference", "identifycolor", "infraredthermometer", "oilleakage", "signallight", "switch", "liquidlevel"};
        public List<System.String> volArry = new List<System.String>();
        public List<System.String> bayArry = new List<System.String>();
        public List<System.String> devArry = new List<System.String>();

        public Form1()
        {
            InitializeComponent();
          
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CreateXML();
            CreateColumn();
        }

        public void CreateXML()
        {
            XmlDocument xmlDoc = new XmlDocument();
            //创建类型声明节点
            XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "");
            xmlDoc.AppendChild(node);
            //创建substation节点
            XmlElement sub = xmlDoc.CreateElement("substation");
            sub.SetAttribute("type", null);
            sub.SetAttribute("name", null);
            sub.SetAttribute("sn", null);
            xmlDoc.AppendChild(sub);

            //生成XML文件
            xmlDoc.Save(Application.StartupPath + "/module.xml");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!textBox1.Text.Equals(""))
            {
                subName = textBox1.Text;

            }
            else
            {
                MessageBox.Show("请填入变电站名称");
                return;
            }

            if (!textBox5.Text.Equals(""))
            {
                snName = textBox5.Text;

            }
            else
            {
                MessageBox.Show("请填入sn");
                return;

            }

            if (!textBox2.Text.Equals(""))
            {
                bayName = textBox2.Text;

            }
            else
            {
                MessageBox.Show("请填入间隔名称（不要出现中文字符）");
                return;

            }

            if (!(comboBox1.SelectedIndex == -1))
            {
                volLevel = comboBox1.Text;
                volLevelDes = volLevel.Substring(0,volLevel.Length-2);
               // volLevelDes = volLevel.Remove(volLevel.Length - 2, 2);

            }
            else
            {
                volLevel = comboBox1.Text;
                volLevelDes = volLevel;
            }
            
            if (!(comboBox2.SelectedIndex == -1))
            {
                if (comboBox2.Text.Equals("DSTF-站用变")|| comboBox2.Text.Equals("DBUS-母线") || comboBox2.Text.Equals("DETC-其他"))
                {
                    devName = comboBox2.Text.Substring(0, 4);
                }
                else
                {
                    devName = comboBox2.Text.Substring(0, 3);
                }               
            }
            else
            {
                devName = comboBox2.Text;
            }

            if (!textBox4.Text.Equals(""))
            {
                devDescribe = textBox4.Text;
            }
            else
            {
                MessageBox.Show("请填入装置描述");
                return;
            }

            patPoint = textBox3.Text;

            patrolheight = textBox6.Text;
            patroltype = comboBox3.SelectedIndex.ToString();
            ivstype = comboBox4.SelectedIndex.ToString();
            unitname = textBox7.Text;

            path = Application.StartupPath + "\\" + "module-template" + "\\" + volLevel + "\\" + devName + ".xml";

            if (File.Exists(path))
            {
                AddTmp(path);
                AddDataView();
                isNewTmp = false;
            }
            else
            {
                if (isNewDev)
                {
                    AddNode(volLevel, bayName, devName, patPoint);
                    AddDataView();
                    isNewDev = false;
                    isNewPat = false;
                }
                else
                {
                    if (isNewPat)
                    {
                        AddPat();
                        AddDataView();
                        isNewPat = false;
                    }
                }
                
            }

            //  GetXmlInfo();
            

        }

        public void AddTmp(string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Application.StartupPath + "/module.xml");
            XmlDocument tmpDoc = new XmlDocument();
            tmpDoc.Load(path);
            XmlElement subNode = (XmlElement)doc.SelectSingleNode("substation");
            XmlElement volNode;
            XmlElement bayNode;
            XmlNode devNode = doc.CreateElement("device");
            XmlNode tmpNode = tmpDoc.SelectSingleNode("template");
            devNode.InnerXml = tmpNode.InnerXml;

            if (volArry.IndexOf(comboBox1.Text) == -1)  //新的电压等级
            {
                volArry.Add(volLevel);
                volNode = doc.CreateElement("voltagelevel");
                doc.DocumentElement.AppendChild(volNode);
                if (bayArry.IndexOf(textBox2.Text) == -1)  //新间隔
                {
                    bayNode = doc.CreateElement("bay");
                    bayArry.Add(bayName);
                    volNode.AppendChild(bayNode);
                    bayNode.AppendChild(devNode);
                    AppendDev(subNode, volNode, bayNode, devNode);
                    doc.Save(Application.StartupPath + "/module.xml");

                }
                else
                {
                    bayNode = (XmlElement)volNode.GetElementsByTagName("bay").Item(bayArry.IndexOf(textBox2.Text));
                    bayNode.AppendChild(devNode);

                    AppendDev(subNode, volNode, bayNode, devNode);
                    doc.Save(Application.StartupPath + "/module.xml");
                }

            }
            else
            {
                volNode = (XmlElement)subNode.GetElementsByTagName("voltagelevel").Item(volArry.IndexOf(comboBox1.Text));

                if (bayArry.IndexOf(textBox2.Text) == -1)  //新间隔
                {
                    bayNode = doc.CreateElement("bay");
                    bayArry.Add(bayName);
                    volNode.AppendChild(bayNode);
                    bayNode.AppendChild(devNode);
                    AppendDev(subNode, volNode, bayNode, devNode);
                    doc.Save(Application.StartupPath + "/module.xml");

                }
                else
                {
                    bayNode = (XmlElement)subNode.GetElementsByTagName("bay").Item(bayArry.IndexOf(textBox2.Text));
                    bayNode.AppendChild(devNode);
                    AppendDev(subNode, volNode, bayNode, devNode);
                    doc.Save(Application.StartupPath + "/module.xml");
                }

            }

          
            
    /*        XmlNode devNode = doc.CreateElement("device");
            XmlNode tmpNode = tmpDoc.SelectSingleNode("template");
            devNode.InnerXml = tmpNode.InnerXml;
            bayNode.AppendChild(devNode);            
            XmlElement heightNode = (XmlElement)doc.SelectSingleNode("substation/voltagelevel/bay/device/patrolpoint/patrolheight");
            XmlElement typeNode = (XmlElement)doc.SelectSingleNode("substation/voltagelevel/bay/device/patrolpoint/patroltype");
            XmlElement ivsNode = (XmlElement)doc.SelectSingleNode("substation/voltagelevel/bay/device/patrolpoint/ivstype");
            XmlElement pointidNode = (XmlElement)doc.SelectSingleNode("substation/voltagelevel/bay/device/patrolpoint/pathpointid");
            XmlElement unitNode = (XmlElement)doc.SelectSingleNode("substation/voltagelevel/bay/device/patrolpoint/unitname");
            XmlElement lockNode = (XmlElement)doc.SelectSingleNode("substation/voltagelevel/bay/device/patrolpoint/lock");
            XmlNodeList patList = devNode.ChildNodes;
            SetEleAttribute(subNode, volNode, bayNode, (XmlElement)devNode);
            patCount = 0;
            foreach (XmlElement patNode in patList)
            {
                patNode.SetAttribute("sn", snName + "." + "V" + volLevelDes + "." + "B" + bayName + "." + devName + devCount + "." + "P" + patCount);
                patCount += 1;
            }          
            devCount += 1;          
            isNewTmp = true;
            
            doc.Save(Application.StartupPath + "/module.xml");  */

        }

        public void AddNode(string vol, string bay, string dev, string pat)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Application.StartupPath + "/module.xml");
            XmlElement subNode =(XmlElement) doc.SelectSingleNode("substation");

            if (!subNode.HasChildNodes)   //新的文件
            {
                XmlElement volNode = doc.CreateElement("voltagelevel");
                XmlElement bayNode = doc.CreateElement("bay");
                XmlElement devNode = doc.CreateElement("device");
                XmlElement patNode = doc.CreateElement("patrolpoint");
                XmlElement heightNode = doc.CreateElement("patrolheight");
                XmlElement typeNode = doc.CreateElement("patroltype");
                XmlElement ivsNode = doc.CreateElement("ivstype");
                XmlElement pointidNode = doc.CreateElement("pathpointid");
                XmlElement unitNode = doc.CreateElement("unitname");
                XmlElement lockNode = doc.CreateElement("lock");
                devCount = 0;
                patCount = 0;
                SetEleAttribute(subNode, volNode, bayNode, devNode, patNode);
                SetInnerText(heightNode, typeNode, ivsNode, pointidNode, unitNode, lockNode);        
                doc.DocumentElement.AppendChild(volNode);
                volNode.AppendChild(bayNode);
                bayNode.AppendChild(devNode);
                devNode.AppendChild(patNode);
                patNode.AppendChild(heightNode);
                patNode.AppendChild(typeNode);
                patNode.AppendChild(ivsNode);
                patNode.AppendChild(pointidNode);
                patNode.AppendChild(unitNode);
                patNode.AppendChild(lockNode);
                volArry.Add(volLevel);
                bayArry.Add(bayName);
                devArry.Add(comboBox2.Text);

                //  devCount += 1;
                doc.Save(Application.StartupPath + "/module.xml");
            }
            else
            {
                XmlNodeList volList = subNode.GetElementsByTagName("voltagelevel");
                XmlNodeList bayList = subNode.GetElementsByTagName("bay");
                if (volArry.IndexOf(comboBox1.Text) == -1) //新的电压等级
                {
                    XmlElement newVolNode = doc.CreateElement("voltagelevel");
                    volArry.Add(volLevel);
                    doc.DocumentElement.AppendChild(newVolNode);
                    if (bayArry.IndexOf(textBox2.Text) == -1) //新的间隔
                    {
                        bayArry.Add(bayName);
                        XmlElement newBayNode = doc.CreateElement("bay");
                        newVolNode.AppendChild(newBayNode);

                        XmlElement devNode = doc.CreateElement("device");
                        XmlElement patNode = doc.CreateElement("patrolpoint");
                        XmlElement heightNode = doc.CreateElement("patrolheight");
                        XmlElement typeNode = doc.CreateElement("patroltype");
                        XmlElement ivsNode = doc.CreateElement("ivstype");
                        XmlElement pointidNode = doc.CreateElement("pathpointid");
                        XmlElement unitNode = doc.CreateElement("unitname");
                        XmlElement lockNode = doc.CreateElement("lock");
                        devCount = 0;
                        patCount = 0;
                        SetEleAttribute(subNode, newVolNode, newBayNode, devNode, patNode);
                        SetInnerText(heightNode, typeNode, ivsNode, pointidNode, unitNode, lockNode);

                     //   volList.Item(volArry.IndexOf(comboBox1.Text)).AppendChild(newBayNode);
                        newBayNode.AppendChild(devNode);
                        devNode.AppendChild(patNode);
                        patNode.AppendChild(heightNode);
                        patNode.AppendChild(typeNode);
                        patNode.AppendChild(ivsNode);
                        patNode.AppendChild(pointidNode);
                        patNode.AppendChild(unitNode);
                        patNode.AppendChild(lockNode);
                        doc.Save(Application.StartupPath + "/module.xml");
                       
                    }
                    else
                    {
                        XmlElement bayNode = (XmlElement)doc.GetElementsByTagName("bay").Item(bayArry.IndexOf(textBox2.Text));
                        XmlElement devNode = doc.CreateElement("device");
                        XmlElement patNode = doc.CreateElement("patrolpoint");
                        XmlElement heightNode = doc.CreateElement("patrolheight");
                        XmlElement typeNode = doc.CreateElement("patroltype");
                        XmlElement ivsNode = doc.CreateElement("ivstype");
                        XmlElement pointidNode = doc.CreateElement("pathpointid");
                        XmlElement unitNode = doc.CreateElement("unitname");
                        XmlElement lockNode = doc.CreateElement("lock");
                        devCount = 0;
                        patCount = 0;
                        SetEleAttribute(subNode, newVolNode, bayNode, devNode, patNode);
                        SetInnerText(heightNode, typeNode, ivsNode, pointidNode, unitNode, lockNode);
                        doc.DocumentElement.AppendChild(newVolNode);
                        newVolNode.AppendChild(bayNode);
                        bayNode.AppendChild(devNode);
                        devNode.AppendChild(patNode);
                        patNode.AppendChild(heightNode);
                        patNode.AppendChild(typeNode);
                        patNode.AppendChild(ivsNode);
                        patNode.AppendChild(pointidNode);
                        patNode.AppendChild(unitNode);
                        patNode.AppendChild(lockNode);
                        doc.Save(Application.StartupPath + "/module.xml");
                    }
                }
                else
                {
                    XmlElement volNode = (XmlElement)doc.GetElementsByTagName("voltagelevel").Item(volArry.IndexOf(comboBox1.Text));
                    if (bayArry.IndexOf(textBox2.Text) == -1)
                    {
                        bayArry.Add(bayName);
                        XmlElement newBayNode = doc.CreateElement("bay");
                        volNode.AppendChild(newBayNode);                        
                        XmlElement devNode = doc.CreateElement("device");
                        XmlElement patNode = doc.CreateElement("patrolpoint");
                        XmlElement heightNode = doc.CreateElement("patrolheight");
                        XmlElement typeNode = doc.CreateElement("patroltype");
                        XmlElement ivsNode = doc.CreateElement("ivstype");
                        XmlElement pointidNode = doc.CreateElement("pathpointid");
                        XmlElement unitNode = doc.CreateElement("unitname");
                        XmlElement lockNode = doc.CreateElement("lock");
                        devCount = 0;
                        patCount = 0;
                        SetEleAttribute(subNode, volNode, newBayNode, devNode, patNode);
                        SetInnerText(heightNode, typeNode, ivsNode, pointidNode, unitNode, lockNode);
                        newBayNode.AppendChild(devNode);
                        devNode.AppendChild(patNode);
                        patNode.AppendChild(heightNode);
                        patNode.AppendChild(typeNode);
                        patNode.AppendChild(ivsNode);
                        patNode.AppendChild(pointidNode);
                        patNode.AppendChild(unitNode);
                        patNode.AppendChild(lockNode);
                        doc.Save(Application.StartupPath + "/module.xml");
                    }
                    else
                    {
                        XmlElement bayNode = (XmlElement)doc.GetElementsByTagName("bay").Item(bayArry.IndexOf(textBox2.Text));
                        XmlElement devNode = doc.CreateElement("device");
                        XmlElement patNode = doc.CreateElement("patrolpoint");
                        XmlElement heightNode = doc.CreateElement("patrolheight");
                        XmlElement typeNode = doc.CreateElement("patroltype");
                        XmlElement ivsNode = doc.CreateElement("ivstype");
                        XmlElement pointidNode = doc.CreateElement("pathpointid");
                        XmlElement unitNode = doc.CreateElement("unitname");
                        XmlElement lockNode = doc.CreateElement("lock");
                        if (devArry.IndexOf(comboBox2.Text) == -1)
                        {
                            devArry.Add(comboBox2.Text);
                            devCount = 0;
                        }
                        else
                        {
                            devCount += 1;
                        }
                        
                        patCount = 0;
                        SetEleAttribute(subNode, volNode, bayNode, devNode, patNode);
                        SetInnerText(heightNode, typeNode, ivsNode, pointidNode, unitNode, lockNode);
                        bayNode.AppendChild(devNode);
                        devNode.AppendChild(patNode);
                        patNode.AppendChild(heightNode);
                        patNode.AppendChild(typeNode);
                        patNode.AppendChild(ivsNode);
                        patNode.AppendChild(pointidNode);
                        patNode.AppendChild(unitNode);
                        patNode.AppendChild(lockNode);
                        doc.Save(Application.StartupPath + "/module.xml");
                    }
                }           
            }      
        }

        public void AddPat()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Application.StartupPath + "/module.xml");
            XmlElement patNode = doc.CreateElement("patrolpoint");
            XmlElement heightNode = doc.CreateElement("patrolheight");
            XmlElement typeNode = doc.CreateElement("patroltype");
            XmlElement ivsNode = doc.CreateElement("ivstype");
            XmlElement pointidNode = doc.CreateElement("pathpointid");
            XmlElement unitNode = doc.CreateElement("unitname");
            XmlElement lockNode = doc.CreateElement("lock");
            XmlNode devNode = doc.LastChild.LastChild.LastChild.LastChild;
            devNode.AppendChild(patNode);
            patNode.AppendChild(heightNode);
            patNode.AppendChild(typeNode);
            patNode.AppendChild(ivsNode);
            patNode.AppendChild(pointidNode);
            patNode.AppendChild(unitNode);
            patNode.AppendChild(lockNode);
            patNode.SetAttribute("type", "patrolpoint");
            patNode.SetAttribute("name", patPoint);
            patCount += 1;
            patNode.SetAttribute("sn", snName + "." + "V" + volLevelDes + "." + "B" + bayName + "." + devName + devCount + "." + "P" + patCount);
            SetInnerText(heightNode, typeNode, ivsNode, pointidNode, unitNode, lockNode);            
            doc.Save(Application.StartupPath + "/module.xml");
        }

        public void SetEleAttribute(XmlElement sub, XmlElement vol, XmlElement bay, XmlElement dev, XmlElement pat)
        {
            if (sub.GetAttribute("name").Equals(""))
            {
                sub.SetAttribute("type", "substation");
                sub.SetAttribute("name", subName);
                sub.SetAttribute("sn", snName);
            }

            vol.SetAttribute("type", "voltagelevel");
            vol.SetAttribute("name", volLevel);
            vol.SetAttribute("sn", snName + "." + "V" + volLevelDes );

            bay.SetAttribute("type", "bay");
            bay.SetAttribute("name", bayName);
            bay.SetAttribute("sn", snName + "." + "V" + volLevelDes + "." + "B" + bayName);

            dev.SetAttribute("type", "device");
            dev.SetAttribute("name", devDescribe);
            dev.SetAttribute("sn", snName + "." + "V" + volLevelDes+ "." + "B" + bayName + "." + devName + devCount);

            pat.SetAttribute("type", "patrolpoint");
            pat.SetAttribute("name", patPoint);
            pat.SetAttribute("sn", snName + "." + "V" + volLevelDes + "." + "B" + bayName + "." + devName + devCount + "." + "P" + patCount);
        }

        public void SetEleAttribute(XmlElement sub, XmlElement vol, XmlElement bay, XmlElement dev)
        {
            if (sub.GetAttribute("name").Equals(""))
            {
                sub.SetAttribute("type", "substation");
                sub.SetAttribute("name", subName);
                sub.SetAttribute("sn", snName);
            }
            vol.SetAttribute("type", "voltagelevel");
            vol.SetAttribute("name", volLevel);
            vol.SetAttribute("sn", snName + "." + "V" + volLevelDes);

            bay.SetAttribute("type", "bay");
            bay.SetAttribute("name", bayName);
            bay.SetAttribute("sn", snName + "." + "V" + volLevelDes + "." + "B" + bayName);

            dev.SetAttribute("type", "device");
            dev.SetAttribute("name", devDescribe);
            dev.SetAttribute("sn", snName + "." + "V" + volLevelDes + "." + "B" + bayName + "." + devName + devCount);

        }

        /*    设置巡检点的参数    */
        public void SetInnerText(XmlElement heigh,XmlElement type,XmlElement ivs,XmlElement pointid,XmlElement unit,XmlElement l)
        {
            heigh.InnerText = patrolheight;
            type.InnerText = patroltype;
            if (comboBox3.SelectedIndex == -1)
            {
                ivs.InnerText = ivstype;
            }
            else
            {
                ivs.InnerText = typename[comboBox4.SelectedIndex];
            }      
            pointid.InnerText = "";
            unit.InnerText = unitname;
            l.InnerText = "0";
        }

        public void CreateColumn()
        {
            dataGridView1.Columns.Add("0", "厂站名");
            dataGridView1.Columns.Add("1", "电压等级");
            dataGridView1.Columns.Add("2", "间隔名");
            dataGridView1.Columns.Add("3", "装置名");
            dataGridView1.Columns.Add("4", "巡检点");
            dataGridView1.Columns.Add("5", "高度");
            dataGridView1.Columns.Add("6", "巡检类型");
            dataGridView1.Columns.Add("7", "识别类型");
            dataGridView1.Columns.Add("8", "单位");
           // DataGridViewComboBoxColumn col = new DataGridViewComboBoxColumn();
           // dataGridView1.Columns.Add(col);
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            dataGridView1.DoubleBuffered(true);
        }

        /*   在DataGridView中显示XML里的内容   */

        public void AddDataView()
        {
            if (File.Exists(path))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNodeList patlist = doc.DocumentElement.ChildNodes;
                foreach(XmlElement patnode in patlist)
                {
                    index = dataGridView1.Rows.Add();
                    dataGridView1.Rows[index].Cells[0].Value = subName;
                    dataGridView1.Rows[index].Cells[1].Value = comboBox1.Text;
                    dataGridView1.Rows[index].Cells[2].Value = bayName;
                    dataGridView1.Rows[index].Cells[3].Value = devDescribe;
                    dataGridView1.Rows[index].Cells[4].Value = patnode.GetAttribute("name");
                    dataGridView1.Rows[index].Cells[5].Value = patnode.ChildNodes.Item(0).InnerText;
                    dataGridView1.Rows[index].Cells[6].Value = patnode.ChildNodes.Item(1).InnerText;
                    dataGridView1.Rows[index].Cells[7].Value = patnode.ChildNodes.Item(2).InnerText;
                    dataGridView1.Rows[index].Cells[8].Value = patnode.ChildNodes.Item(4).InnerText;
                }
                dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

            }
            else
            {
                index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells[0].Value = subName;
                dataGridView1.Rows[index].Cells[1].Value = comboBox1.Text;
                dataGridView1.Rows[index].Cells[2].Value = bayName;
                dataGridView1.Rows[index].Cells[3].Value = devDescribe;
                dataGridView1.Rows[index].Cells[4].Value = patPoint;
                dataGridView1.Rows[index].Cells[5].Value = patrolheight;
                dataGridView1.Rows[index].Cells[6].Value = patroltype;
                if(comboBox4.SelectedIndex == -1)
                {
                    dataGridView1.Rows[index].Cells[7].Value = ivstype;
                }
                else
                {
                    dataGridView1.Rows[index].Cells[7].Value = typename[comboBox4.SelectedIndex];
                }             
                dataGridView1.Rows[index].Cells[8].Value = unitname;
                dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }

        }

        /*    给DataGridView每一行前添加行号    */
        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Color color = dataGridView1.RowHeadersDefaultCellStyle.ForeColor;
            if (dataGridView1.Rows[e.RowIndex].Selected)
                color = dataGridView1.RowHeadersDefaultCellStyle.SelectionForeColor;
            else
                color = dataGridView1.RowHeadersDefaultCellStyle.ForeColor;

            using (SolidBrush b = new SolidBrush(color))
            {
                e.Graphics.DrawString((e.RowIndex + 1).ToString(), e.InheritedRowStyle.Font, b, e.RowBounds.Location.X + 20, e.RowBounds.Location.Y + 6);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Application.StartupPath + "/module.xml");

           foreach(DataGridViewRow row in dataGridView1.Rows)
            {
                foreach(DataGridViewCell cell in row.Cells)
                {
                    if(cell.Style.ForeColor == Color.Red)
                    {
                        int colIndex = cell.ColumnIndex;
                        int rowIndex = cell.RowIndex;    
                        string elementName = Enum.GetName(typeof(element), colIndex); //获得要更改的 element 的标签名                        
                        XmlNodeList elist = doc.GetElementsByTagName(elementName);
                        XmlElement ement;
                        if (colIndex <= 4)
                        {
                            if(rowIndex == 0)
                            {
                                ement = (XmlElement)elist.Item(0);
                                ement.SetAttribute("name", cell.Value.ToString());
                            }
                            else
                            {
                                XmlNodeList plist = doc.GetElementsByTagName("patrolpoint");
                                XmlElement patnode = (XmlElement)plist.Item(rowIndex);
                                switch (colIndex)
                                {
                      
                                    case 1:                                        
                                        ement = (XmlElement)patnode.ParentNode.ParentNode.ParentNode;
                                        ement.SetAttribute("name", cell.Value.ToString());
                                        break;
                                    case 2:                                    
                                        ement = (XmlElement)patnode.ParentNode.ParentNode;
                                        ement.SetAttribute("name", cell.Value.ToString());
                                        break;
                                    case 3:
                                        ement = (XmlElement)patnode.ParentNode;
                                        ement.SetAttribute("name", cell.Value.ToString());
                                        break;
                                    case 4:
                                        ement = (XmlElement)elist.Item(rowIndex);
                                        ement.SetAttribute("name", cell.Value.ToString());
                                        break;
                                }
                                
                            }
                        }
          
                        else
                        {
                            ement = (XmlElement)elist.Item(rowIndex);
                            ement.InnerText = cell.Value.ToString();
                        }

                    }
                }
            }

            doc.Save(Application.StartupPath + "/module.xml");
            MessageBox.Show("保存成功");
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Style.ForeColor = Color.Black;
                }
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            string tmpname;
            if (comboBox2.Text.Equals("DSTF-站用变") || comboBox2.Text.Equals("DBUS-母线") || comboBox2.Text.Equals("DETC-其他"))
            {
                tmpname = comboBox2.Text.Substring(0, 4);
            }
            else
            {
                tmpname = comboBox2.Text.Substring(0, 3);
            }

            string url = Application.StartupPath + "\\" + "module-template" + "\\" + comboBox1.Text + "\\" + tmpname + ".xml";
            if (File.Exists(url))
            {
                isNewDev = false;
                
            }
            else
            {
                isNewDev = true;
                
            }
            
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            isNewDev = true;
            
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            isNewDev = true;
            
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            isNewPat = true;
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!(isNewDev || isNewPat || isNewTmp))
            {
                if (dataGridView1.CurrentCell.Style.ForeColor != Color.Red)
                {
                    dataGridView1.CurrentCell.Style.ForeColor = Color.Red;
                    changeCount++;
                }
            }
            else
            {
                return;
            }                  
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            
            
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string tmpname;
            if (comboBox2.Text.Equals("DSTF-站用变") || comboBox2.Text.Equals("DBUS-母线") || comboBox2.Text.Equals("DETC-其他"))
            {
                tmpname = comboBox2.Text.Substring(0, 4);
            }
            else
            {
                tmpname = comboBox2.Text.Substring(0, 3);             
            }

            string url = Application.StartupPath + "\\" + "module-template" + "\\" + comboBox1.Text + "\\" + tmpname + ".xml";
            if (File.Exists(url))
            {
                this.textBox3.Enabled = false;
                this.textBox6.Enabled = false;
                this.comboBox3.Enabled = false;
                this.comboBox4.Enabled = false;
                this.textBox7.Enabled = false;
                isNewDev = false;
                devCount = 0;
            }
            else
            {
                textBox3.Enabled = true;
                textBox6.Enabled = true;
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
                textBox7.Enabled = true;
                isNewDev = true;
                devCount = 0;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar >= (int)'a' && (int)e.KeyChar <= (int)'z') || ((int)e.KeyChar >= (int)'A' && (int)e.KeyChar <= (int)'Z') || ((int)e.KeyChar >= 48 && (int)e.KeyChar <= 57) ||
                ((int)e.KeyChar == 8 ))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string tmpname = "";
            if (comboBox2.SelectedIndex != -1)
            {             
                if (comboBox2.Text.Equals("DSTF-站用变") || comboBox2.Text.Equals("DBUS-母线") || comboBox2.Text.Equals("DETC-其他"))
                {
                    tmpname = comboBox2.Text.Substring(0, 4);
                }
                else
                {
                    tmpname = comboBox2.Text.Substring(0, 3);
                }
            }
            string url = Application.StartupPath + "\\" + "module-template" + "\\" + comboBox1.Text + "\\" + tmpname + ".xml";
            if (File.Exists(url))
            {
                this.textBox3.Enabled = false;
                this.textBox6.Enabled = false;
                this.comboBox3.Enabled = false;
                this.comboBox4.Enabled = false;
                this.textBox7.Enabled = false;
                isNewDev = false;
                devCount = 0;
            }
            else
            {
                textBox3.Enabled = true;
                textBox6.Enabled = true;
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
                textBox7.Enabled = true;
                isNewDev = true;
                devCount = 0;
            }
        }
        
        public void AppendDev(XmlElement subNode, XmlElement volNode, XmlElement bayNode, XmlNode devNode)
        {                    
            bayNode.AppendChild(devNode);
            XmlNodeList patList = devNode.ChildNodes;
            SetEleAttribute(subNode, volNode, bayNode, (XmlElement)devNode);
            patCount = 0;
            foreach (XmlElement patNode in patList)
            {
                patNode.SetAttribute("sn", snName + "." + "V" + volLevelDes + "." + "B" + bayName + "." + devName + devCount + "." + "P" + patCount);
                patCount += 1;
            }
            devCount += 1;
            isNewTmp = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2(volArry, bayArry);
            if(f.ShowDialog() == DialogResult.Yes)
            {
                XmlElement xlt = f.tmpBayNode;
                XmlNodeList xnl = xlt.GetElementsByTagName("patrolpoint");
                foreach(XmlElement pat in xnl)
                {
                    XmlElement dev = (XmlElement)pat.ParentNode;
                    index = dataGridView1.Rows.Add();
                    dataGridView1.Rows[index].Cells[0].Value = subName;
                    dataGridView1.Rows[index].Cells[1].Value = xlt.GetAttribute("sn").Split('.')[1].Substring(1) + "kV";
                    dataGridView1.Rows[index].Cells[2].Value = xlt.GetAttribute("name");
                    dataGridView1.Rows[index].Cells[3].Value = dev.GetAttribute("name");
                    dataGridView1.Rows[index].Cells[4].Value = pat.GetAttribute("name");
                    dataGridView1.Rows[index].Cells[5].Value = pat.ChildNodes.Item(0).InnerText;
                    dataGridView1.Rows[index].Cells[6].Value = pat.ChildNodes.Item(1).InnerText;
                    dataGridView1.Rows[index].Cells[7].Value = pat.ChildNodes.Item(2).InnerText;
                    dataGridView1.Rows[index].Cells[8].Value = pat.ChildNodes.Item(4).InnerText;
                }
                dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                bayArry.Add(xlt.GetAttribute("name"));
            }
            

        }
    }


    /* 设置DataGridView双缓存格式  */

    public static class ExtensionMethods
    {
        public static void DoubleBuffered(this DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }
    }
    

}

 
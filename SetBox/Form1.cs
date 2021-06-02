using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SetBox
{
    public partial class Main : Form
    {

        public Main()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
            InitializeComponent();
            Type dgvType = this.dataGridView1.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
            BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(this.dataGridView1, true, null);
        }
        DataTable dt = new DataTable();
        string jsonstr;
        public string ToJson(DataTable dt)
        {
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            javaScriptSerializer.MaxJsonLength = Int32.MaxValue; //取得最大数值
            ArrayList arrayList = new ArrayList();
            foreach (DataRow dataRow in dt.Rows)
            {
                Dictionary<string, object> dictionary = new Dictionary<string, object>();  //实例化一个参数集合
                foreach (DataColumn dataColumn in dt.Columns)
                {
                    if (dataColumn.ColumnName.ToString() == "name")
                    { 
                        dictionary.Add(dataColumn.ColumnName, dataRow[dataColumn.ColumnName].ToString());
                    }
                    else
                    {
                        dictionary.Add(dataColumn.ColumnName, dataRow[dataColumn.ColumnName]);
                    }
                }
                arrayList.Add(dictionary); //ArrayList集合中添加键值
            }
            return javaScriptSerializer.Serialize(arrayList);  //返回一个json字符串
        }
        public   DataTable ToDataTable(string json)
        {
            DataTable dataTable = new DataTable();  //实例化
            DataTable result;
            try
            {
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                javaScriptSerializer.MaxJsonLength = Int32.MaxValue; //取得最大数值
                ArrayList arrayList = javaScriptSerializer.Deserialize<ArrayList>(json);
                if (arrayList.Count > 0)
                {
                    foreach (Dictionary<string, object> dictionary in arrayList)
                    {
                        if (dictionary.Keys.Count<string>() == 0)
                        {
                            result = dataTable;
                            return result;
                        }
                        if (dataTable.Columns.Count == 0)
                        {
                            foreach (string current in dictionary.Keys)
                            {
                                dataTable.Columns.Add(current, dictionary[current].GetType());
                            }
                        }
                        DataRow dataRow = dataTable.NewRow();
                        foreach (string current in dictionary.Keys)
                        {
                            dataRow[current] = dictionary[current];
                        }

                        dataTable.Rows.Add(dataRow); //循环添加行到DataTable中
                    }
                }

                //dataGridView1.DataSource = dt;
            }
            catch
            {
                MessageBox.Show("呀！返回的数据好像不太对呢......");
                button1.Enabled = true;
                button2.Enabled = false;
            }
            result = dataTable;
            button1.Enabled = true;
            button2.Enabled = true;
            return result;

        }



        public void JsonToTable(string jsondata)
    {
        dt =ToDataTable(jsondata);
    }
        private void Main_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            try
            {
                UID_TEXT.Text = System.IO.File.ReadAllText(System.Windows.Forms.Application.StartupPath + "\\user.txt");
                Token_Text.Text = System.IO.File.ReadAllText(System.Windows.Forms.Application.StartupPath + "\\token.txt");
            }
            catch
            {

            }
            //dt.Columns.Add("Unit ID", typeof(Int32));
            //dt.Columns.Add("角色名称", typeof(String));
            //dt.Columns.Add("等级", typeof(Int32));
            //dt.Columns.Add("星级", typeof(Int32));
            //dt.Columns.Add("RANK", typeof(Int32));
            //dt.Columns.Add("装备1", typeof(Int32));
            //dt.Columns.Add("装备2", typeof(Int32));
            //dt.Columns.Add("装备3", typeof(Int32));
            //dt.Columns.Add("装备4", typeof(Int32));
            //dt.Columns.Add("装备5", typeof(Int32));
            //dt.Columns.Add("装备6", typeof(Int32));
            //dt.Columns.Add("专武等级", typeof(Int32));
            //dt.Columns.Add("UB", typeof(Int32));
            //dt.Columns.Add("技能1", typeof(Int32));
            //dt.Columns.Add("技能2", typeof(Int32));
            //dt.Columns.Add("EX技能", typeof(Int32));
            //dt.Columns.Add("好感度", typeof(Int32));
        }
        private async void getdata(string uid , string token)
        {
            dt.Clear();
            var web_url = "https://s1.pcrbot.fun:2087/api/getbox?uid="+uid+"&token="+token;
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(web_url);
            var body = await response.Content.ReadAsByteArrayAsync();
            //MessageBox.Show(Encoding.UTF8.GetString(body));
            var body1 = Encoding.UTF8.GetString(body).Replace("\r", string.Empty);
            //MessageBox.Show("OK!");
            //JObject obj = Newtonsoft.Json.Linq.JObject.Parse(body1);
            if (Encoding.UTF8.GetString(body) == "[]")
            {
                MessageBox.Show("UID或Token不对哦，再检查看看？");
                button1.Enabled = true;
            }
            else
            {
                JsonToTable(Encoding.UTF8.GetString(body));
                dataGridView1.DataSource = dt;
                //dataGridView1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }
        public DataTable GetDgvToTable(DataGridView dgv)
        {
            DataTable dt = new DataTable();
            int check;

            // 列强制转换
            for (int count = 0; count < dgv.Columns.Count; count++)
            {
                DataColumn dc = new DataColumn(dgv.Columns[count].Name.ToString());
                if (dgv.Columns[count].Name.ToString() == "name")
                {
                    dt.Columns.Add(dgv.Columns[count].Name.ToString());
                }
                else
                {
                    dt.Columns.Add(dgv.Columns[count].Name.ToString(), typeof(Int32));
                }
            }

            // 循环行
            for (int count = 0; count < dgv.Rows.Count; count++)
            {
                DataRow dr = dt.NewRow();
                for (int countsub = 0; countsub < dgv.Columns.Count; countsub++)
                {
                    try
                    {
                        dr[countsub] = Convert.ToInt32(dgv.Rows[count].Cells[countsub].Value);
                    }
                    catch
                    {
                        dr[countsub] = Convert.ToString(dgv.Rows[count].Cells[countsub].Value);
                    }

                    //dr[countsub] = dgv.Rows[count].Cells[countsub].Value;
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;

            getdata(UID_TEXT.Text, Token_Text.Text);

        }
        public string DataTableToJsonWithJsonNet(DataTable table)
        {
            string JsonString = string.Empty;
            JsonString = JsonConvert.SerializeObject(table);
            return JsonString;
        }
        private async void postdata(string mainbody)
        {
            //dt.Clear();
            var web_url = "https://s1.pcrbot.fun:2087/api/setbox";//?uid=" + uid + "&token=" + token;
            HttpClient httpClient = new HttpClient();
            HttpContent content = new ByteArrayContent(Encoding.UTF8.GetBytes(mainbody));
            content.Headers.ContentType=new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            HttpResponseMessage response = await httpClient.PostAsync(web_url,content);
            var body = await response.Content.ReadAsByteArrayAsync();
            var body1 = Encoding.UTF8.GetString(body).Replace("\r", string.Empty);
            MessageBox.Show("提交请求已发送，点击确定以同步数据");
            getdata(UID_TEXT.Text, Token_Text.Text);
            button2.Enabled = true;
            button1.Enabled = true;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button1.Enabled = false;
            DataTable dt = new DataTable();
            dt = GetDgvToTable(dataGridView1);
            string poststr;
            poststr = "uid=" + UID_TEXT.Text + "&token=" + Token_Text.Text + "&data=" + DataTableToJsonWithJsonNet(dt);
            //MessageBox.Show(DataTableToJsonWithJsonNet(dt));
            postdata(poststr);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            if (!(textBox1.Text=="")) {
                for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                {
                    if (dataGridView1.Rows[i].Cells[1].Value.ToString().Contains(textBox1.Text))
                    {
                        //dataGridView1.CurrentCell = dataGridView1.Rows[1].Cells[0];
                        //dataGridView1.Rows[i].Selected = false;
                        //找到不匹配行 ，隐藏掉
                        //dataGridView1.Rows[i].Selected = true;
                        dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[0];
                        dataGridView1.ClearSelection();
                        dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[1];
                        return;
                    }
                }
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(textBox1.Text == ""))
                {
                    for (int i = dataGridView1.CurrentRow.Index - 1; i > -1; i--)
                    {
                        if (dataGridView1.Rows[i].Cells[1].Value.ToString().Contains(textBox1.Text))
                        {
                            //dataGridView1.CurrentCell = dataGridView1.Rows[1].Cells[0];
                            //dataGridView1.Rows[i].Selected = false;
                            //找到不匹配行 ，隐藏掉
                            //dataGridView1.Rows[i].Selected = true;
                            dataGridView1.ClearSelection();

                            dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[1];
                            return;
                        }
                        //MessageBox.Show(i.ToString());
                    }
                }
            }
            catch
            {

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(textBox1.Text == ""))
                {
                    for (int i = dataGridView1.CurrentRow.Index + 1; i < dataGridView1.Rows.Count - 1; i++)
                    {
                        if (dataGridView1.Rows[i].Cells[1].Value.ToString().Contains(textBox1.Text))
                        {
                            //dataGridView1.CurrentCell = dataGridView1.Rows[1].Cells[0];
                            //dataGridView1.Rows[i].Selected = false;
                            //找到不匹配行 ，隐藏掉
                            //dataGridView1.Rows[i].Selected = true;
                            dataGridView1.ClearSelection();
                            dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[1];
                            return;
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void Main_Resize(object sender, EventArgs e)
        {
            dataGridView1.Width = this.Width - 44;
            dataGridView1.Height  = this.Height - 97;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
    }
}

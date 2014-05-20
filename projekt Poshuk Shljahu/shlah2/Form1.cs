using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace shlah2
{
    public partial class Form1 : Form
    {
        //Змінні
        static int n = 500;
        Microsoft.VisualBasic.PowerPacks.OvalShape[] ovalshape = new Microsoft.VisualBasic.PowerPacks.OvalShape[n];
        Microsoft.VisualBasic.PowerPacks.OvalShape dz_ovalshape;//Містить коло яке викликало контекстне меню
        Microsoft.VisualBasic.PowerPacks.LineShape[] lineshape = new Microsoft.VisualBasic.PowerPacks.LineShape[n];
        bool mouseRight = true,
            mouseLeft = false;
        int[] x_location = new int[n];
        int[] y_location = new int[n];
        int n_ovalshape = -1,
            x, y,//mouse point
            x1, y1, x2, y2,//координати для об’єднання лінією
            n_conekt = -1, //кількість з’єднань
            n_shljahiv, //кількість шляхів
            n_shlja_naw; //номер шляху який відображений
        int[,] segment_name = new int[n, n];//назва відрізків
        TextBox[] tb = new TextBox[n];
        TextBox[] tb_length = new TextBox[n];
        object conect_ovalshape1, conect_ovalshape2;//об’єкти готові до з’єднання
        double[,] matrix_sumizn = new double[n, n];//Матриця суміжності
        int begin = 0,//Початкова точка шляху
            end = 0;//кінцева
        int[][] sort_shljah;//Відсортовані шляхи
        double[] len_shljah;//Довжина відсортованих шляхів
        //----------------------------------------------------------------------------------------------------------------
 
        //Форма
        public Form1()
        {
            InitializeComponent();
            x1 = y1 = x2 = y2 = 0;
            for (int i = 0; i < 50; i++)
                for (int j = 0; j < 50; j++)
                    matrix_sumizn[i, j] = 0;

            //створення точок
            for (int i = 0; i < n; i++)
            {
                ovalshape[i] = new Microsoft.VisualBasic.PowerPacks.OvalShape();
                ovalshape[i].BackColor = Color.Red;
                ovalshape[i].BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
                ovalshape[i].Size = new Size(25, 25);
                ovalshape[i].Visible = true;
                ovalshape[i].ContextMenuStrip = contextMenuStrip2;
                ovalshape[i].Name = i.ToString();
                ovalshape[i].MouseClick += new MouseEventHandler(ovalshape_MouseClick);
            }
            //-----------------------------------------------------------------------------
            //Створення полів дя вuводу назв населених пунктів
            for (int i = 0; i < n; i++)
            {
                tb[i] = new TextBox();
                tb[i].Size = new Size(50, 20);
                tb[i].Name = i.ToString();
                tb[i].Text = "Пункт " + ((char)('A' + i)).ToString();
            }
            //------------------------------------------------------------------------------
            //Створення ліній
            for (int i = 0; i < n; i++)
            {
                lineshape[i] = new Microsoft.VisualBasic.PowerPacks.LineShape();
                lineshape[i].Name = i.ToString();
            }
            //------------------------------------------------------------------------------
            //Створення текстових полів для введення величин
            for (int i = 0; i < n; i++)
            {
                tb_length[i] = new TextBox();
                tb_length[i].Size = new Size(50, 20);
                tb_length[i].Name = i.ToString();
                tb_length[i].Text = "1";
            }
            //------------------------------------------------------------------------------
            MouseClick += new MouseEventHandler(Form1_MouseClick);
        }
        void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                x = MousePosition.X - Location.X - 10;
                y = MousePosition.Y - Location.Y - 35;
                contextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
            }
            else if (mouseLeft && (e.Button == MouseButtons.Left))
            {
                x = MousePosition.X - Location.X - 10;
                y = MousePosition.Y - Location.Y - 35;
                новийНаселенийПунктToolStripMenuItem_Click(sender, e);
            }
        }
        private void закритиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogrezult = MessageBox.Show("Точно вийти?", "Вихід", MessageBoxButtons.YesNo);
            if (dialogrezult == DialogResult.Yes)
                Close();
        }
        private void рестартToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        //вигляд
        private void сховатиМатрицюСуміжностіToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            сховатиМатрицюСуміжностіToolStripMenuItem.Visible = false;
            показатиМатрицюСуміжностіToolStripMenuItem.Visible = true;
        }
        private void показатиМатрицюСуміжностіToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
            panel1.Visible = true;
            сховатиМатрицюСуміжностіToolStripMenuItem.Visible = true;
            показатиМатрицюСуміжностіToolStripMenuItem.Visible = false;
            сховатиІнформаціюПроШляхToolStripMenuItem.Visible = false;
            показатиІнформаціюПроШляхToolStripMenuItem.Visible = true;
        }
        private void сховатиІнформаціюПроШляхToolStripMenuItem_Click(object sender, EventArgs e)
        {
            показатиІнформаціюПроШляхToolStripMenuItem.Visible = true;
            сховатиІнформаціюПроШляхToolStripMenuItem.Visible = false;
            panel2.Visible = false;
        }
        private void фонФормиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            BackColor = colorDialog1.Color;
        }
        private void вставитиКартінкуНаФонToolStripMenuItem_Click(object sender, EventArgs e)
        { 
            openFileDialog1.ShowDialog();
            string fileName = openFileDialog1.FileName;
            try
            {
                imageFon = Image.FromFile(fileName);
                вставитиКартінкуНаФонToolStripMenuItem.Visible = false;
                BackgroundImage = imageFon;
                trackBar1.Visible = true;
                button2.Visible = true;
                button2.Location = new Point(281, 27);
                trackBar1.Location = new Point(0, 27);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Файл не знайдено", "Error", MessageBoxButtons.OK);
            }
        }
        Image imageFon;
        private void trackBar1_Scroll(object sender, EventArgs e)//Зміна розміру картинки
        {
            
            Bitmap bitmap = new Bitmap(imageFon);
            Rectangle rectangle = new Rectangle((int)(imageFon.Width * (trackBar1.Value * 0.05)), (int)(imageFon.Height * (trackBar1.Value * 0.05)),
                (int)(imageFon.Width * ((10.0 - trackBar1.Value) / 10.0)), (int)(imageFon.Height * ((10.0 - trackBar1.Value) / 10.0)));
            BackgroundImage = bitmap.Clone(rectangle, imageFon.PixelFormat);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            button2.Visible = false;
            trackBar1.Visible = false;
        }
        private void показатиІнформаціюПроШляхToolStripMenuItem_Click(object sender, EventArgs e)
        {
            сховатиІнформаціюПроШляхToolStripMenuItem.Visible = true;
            показатиІнформаціюПроШляхToolStripMenuItem.Visible = false;
            сховатиМатрицюСуміжностіToolStripMenuItem.Visible = false;
            показатиМатрицюСуміжностіToolStripMenuItem.Visible = true;
            panel2.Visible = true;
            panel1.Visible = false;
        }
        //--------------------------------------------------------------------------------------------------------     

        //для точки
        private void змінитиКолірToolStripMenuItem_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            ovalshape[int.Parse(dz_ovalshape.Name)].BackColor = colorDialog1.Color;
        }
        void ovalshape_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                dz_ovalshape = (Microsoft.VisualBasic.PowerPacks.OvalShape)sender;
                conect_ovalshape1 = sender;
            }
            else if (!mouseRight && (e.Button == MouseButtons.Left))
            {
                dz_ovalshape = (Microsoft.VisualBasic.PowerPacks.OvalShape)sender;
                conect_ovalshape1 = sender;
                dz_ovalshape.BackColor = Color.Green;
                this.зєднатиToolStripMenuItem_Click(sender, e);
            }
        }
        private void новийНаселенийПунктToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i;
            i = ++n_ovalshape;
            ovalshape[i].Location = new Point(x, y);
            rectangleShape1.Parent.Shapes.Add(ovalshape[i]);//добавлення точки на форму
            tb[i].Location = new Point(x, y + 20);
            Controls.Add(tb[i]);//Добавлення на форму текстового поля
            x_location[i] = x;//запам’ятовує положення точки
            y_location[i] = y;
        }
        private void початковаТочкаШляхуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox2.Text += "(Матриця суміжності створена,";
            textBox2.Text += Environment.NewLine;
            textBox2.Text += "відкрити можна в вкладці Вигляд)";
            textBox2.Text += Environment.NewLine;
            Microsoft.VisualBasic.PowerPacks.OvalShape ovalshape1 =
                (Microsoft.VisualBasic.PowerPacks.OvalShape)conect_ovalshape1;
            ovalshape1.BackColor = Color.Blue;
            begin = int.Parse(ovalshape1.Name);
            textBox2.Text += "Початок: " + tb[begin].Text;
            початковаТочкаШляхуToolStripMenuItem.Visible = false;
            кінцеваТочкаШляхуToolStripMenuItem.Visible = true;
        }
        private void кінцеваТочкаШляхуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Microsoft.VisualBasic.PowerPacks.OvalShape ovalshape1 =
                (Microsoft.VisualBasic.PowerPacks.OvalShape)conect_ovalshape1;
            ovalshape1.BackColor = Color.Yellow;
            end = int.Parse(ovalshape1.Name);
            textBox2.Text += Environment.NewLine;
            textBox2.Text += "Кінець: " + tb[end].Text;
            textBox2.Text += Environment.NewLine;
            запускToolStripMenuItem.Enabled = true;
            кінцеваТочкаШляхуToolStripMenuItem.Visible = false;
        }

        //Настройки
        private void зєднуватиЗаДопомогоюЛівоїКлавішіToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mouseRight = false;
            зєднуватиЗаДопомогоюЛівоїКлавішіToolStripMenuItem.Visible = false;
            зєднуватиПравоюКлавішоюToolStripMenuItem.Visible = true;
        }
        private void зєднуватиПравоюКлавішоюToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mouseRight = true;
            зєднуватиЗаДопомогоюЛівоїКлавішіToolStripMenuItem.Visible = true;
            зєднуватиПравоюКлавішоюToolStripMenuItem.Visible = false;
        }
        private void ставитиТочкиЛівоюКлівішоюToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mouseLeft = true;
            ставитиТочкиЛівоюКлівішоюToolStripMenuItem.Visible = false;
            ставитиТочкиПравоюКлівішоюToolStripMenuItem.Visible = true;
        }
        private void ставитиТочкиПравоюКлівішоюToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mouseLeft = false;
            ставитиТочкиЛівоюКлівішоюToolStripMenuItem.Visible = true;
            ставитиТочкиПравоюКлівішоюToolStripMenuItem.Visible = false;
        }
        //Проект
        private void зберегтиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fileName;
            saveFileDialog1.ShowDialog();
            fileName = saveFileDialog1.FileName;
            SerializeFile serializeFile = new SerializeFile();
            bool boolBackgroundImage = false;
            if (BackgroundImage != null)
            {
                serializeFile.Serialize(fileName + " BackgroundImage", BackgroundImage);
                boolBackgroundImage = true;
            }
            serializeFile.Serialize(fileName, boolBackgroundImage);
            serializeFile.Serialize(fileName + " x_location", x_location);
            serializeFile.Serialize(fileName + " y_location", y_location);
            serializeFile.Serialize(fileName + " n_ovalshape", n_ovalshape);
            serializeFile.Serialize(fileName + " n_conekt", n_conekt);
            //serializeFile.Serialize(fileName + " n_shljahiv", n_shljahiv);
            serializeFile.Serialize(fileName + " segment_name", segment_name);
            serializeFile.Serialize(fileName + " matrix_sumizn", matrix_sumizn);
            //serializeFile.Serialize(fileName + " begin", begin);
            //serializeFile.Serialize(fileName + " end", end);
            //serializeFile.Serialize(fileName + " sort_shljah", sort_shljah);
            //serializeFile.Serialize(fileName + " len_shljah", len_shljah);
            string[] ovalshapeName=new string[n_ovalshape+1];
            Point[] ovalshapeLocation = new Point[n_ovalshape + 1];
            string[] tbName = new string[n_ovalshape + 1];
            string[] tbText = new string[n_ovalshape + 1];
            Point[] tbLocation = new Point[n_ovalshape + 1];
            string[] tb_lengthName = new string[n_conekt + 1];
            string[] tb_lengthText = new string[n_conekt + 1];
            Point[] tb_lengthLocation = new Point[n_conekt + 1];
            string[] lineshapeName = new string[n_conekt+1];
            Point[] lineshapeStartPoint = new Point[n_conekt+1];
            Point[] lineshapeEndPoint = new Point[n_conekt+1];
            for (int i = 0; i <= n_ovalshape; i++)
            {
                ovalshapeName[i] = ovalshape[i].Name;
                ovalshapeLocation[i] = ovalshape[i].Location;

                tbName[i] = tb[i].Name;
                tbText[i] = tb[i].Text;
                tbLocation[i] = tb[i].Location;
            }
            for (int i = 0; i <= n_conekt; i++)
            {
                tb_lengthName[i] = tb_length[i].Name;
                tb_lengthText[i] = tb_length[i].Text;
                tb_lengthLocation[i] = tb_length[i].Location;

                lineshapeName[i] = lineshape[i].Name;
                lineshapeStartPoint[i] = lineshape[i].StartPoint;
                lineshapeEndPoint[i] = lineshape[i].EndPoint;
            }
            serializeFile.Serialize(fileName + " lineshapeName", lineshapeName);
            serializeFile.Serialize(fileName + " lineshapeStartPoint", lineshapeStartPoint);
            serializeFile.Serialize(fileName + " lineshapeEndPoint", lineshapeEndPoint);
            serializeFile.Serialize(fileName + " ovalshapeName", ovalshapeName);
            serializeFile.Serialize(fileName + " ovalshapeLocation", ovalshapeLocation);
            serializeFile.Serialize(fileName + " tbName", tbName);
            serializeFile.Serialize(fileName + " tbText", tbText);
            serializeFile.Serialize(fileName + " tbLocation", tbLocation);
            serializeFile.Serialize(fileName + " tb_lengthName", tb_lengthName);
            serializeFile.Serialize(fileName + " tb_lengthText", tb_lengthText);
            serializeFile.Serialize(fileName + " tb_lengthLocation", tb_lengthLocation);
            DialogResult dialogrezult = MessageBox.Show("Збереження завершено!", "Збереження",
                MessageBoxButtons.OK);
        }
        private void завантажитиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogrezult1 = MessageBox.Show("Проект повинен бути пустим!", "Інформація",
               MessageBoxButtons.OK);
            DialogResult dialogrezult = MessageBox.Show("Вказати потрібно перше слово файлів", "Інформація",
               MessageBoxButtons.OK);
            string fileName;
            openFileDialog1.ShowDialog();
            fileName = openFileDialog1.FileName;
            SerializeFile serializeFile = new SerializeFile();            
            x_location = (int[])serializeFile.Deserialize(fileName + " x_location");
            y_location = (int[])serializeFile.Deserialize(fileName + " y_location");
            n_ovalshape = (int)serializeFile.Deserialize(fileName + " n_ovalshape");
            n_conekt = (int)serializeFile.Deserialize(fileName + " n_conekt");
            segment_name = (int[,])serializeFile.Deserialize(fileName + " segment_name");
            matrix_sumizn = (double[,])serializeFile.Deserialize(fileName + " matrix_sumizn");
            string[] ovalshapeName = (string[])serializeFile.Deserialize(fileName + " ovalshapeName");
            Point[] ovalshapeLocation = (Point[])serializeFile.Deserialize(fileName + " ovalshapeLocation");
            string[] tbName = (string[])serializeFile.Deserialize(fileName + " tbName");
            string[] tbText = (string[])serializeFile.Deserialize(fileName + " tbText");
            Point[] tbLocation = (Point[])serializeFile.Deserialize(fileName + " tbLocation");
            string[] tb_lengthName = (string[])serializeFile.Deserialize(fileName + " tb_lengthName");
            string[] tb_lengthText = (string[])serializeFile.Deserialize(fileName + " tb_lengthText");
            Point[] tb_lengthLocation = (Point[])serializeFile.Deserialize(fileName + " tb_lengthLocation");
             string[] lineshapeName = (string[])serializeFile.Deserialize(fileName + " lineshapeName");
            Point[] lineshapeStartPoint = (Point[])serializeFile.Deserialize(fileName + " lineshapeStartPoint");
            Point[] lineshapeEndPoint = (Point[])serializeFile.Deserialize(fileName + " lineshapeEndPoint");
            //створення точок
            for (int i = 0; i <=n_ovalshape; i++)
            {
                ovalshape[i].Name = ovalshapeName[i];
                ovalshape[i].Location = ovalshapeLocation[i];
                rectangleShape1.Parent.Shapes.Add(ovalshape[i]);
            }
            //-----------------------------------------------------------------------------
            //Створення полів дя вuводу назв населених пунктів
            for (int i = 0; i <= n_ovalshape; i++)
            {
                tb[i].Name = tbName[i];
                tb[i].Text = tbText[i];
                tb[i].Location = tbLocation[i];
                Controls.Add(tb[i]);
            }
            //------------------------------------------------------------------------------
            //Створення ліній
            for (int i = 0; i <=n_conekt; i++)
            {
                lineshape[i].Name = lineshapeName[i];
                lineshape[i].StartPoint = lineshapeStartPoint[i];
                lineshape[i].EndPoint = lineshapeEndPoint[i];
                rectangleShape1.Parent.Shapes.Add(lineshape[i]);
            }
            //------------------------------------------------------------------------------
            //Створення текстових полів для введення величин
            for (int i = 0; i <=n_conekt; i++)
            {
                tb_length[i].Name = tb_lengthName[i];
                tb_length[i].Text = tb_lengthText[i];
                tb_length[i].Location = tb_lengthLocation[i];
                Controls.Add(tb_length[i]);
            }
            if ((bool)serializeFile.Deserialize(fileName))
                BackgroundImage = (Image)serializeFile.Deserialize(fileName + " BackgroundImage");
        }
        //--------------------------------------------------------------------------------------------------------

        //З'єднання точок лінією
        private void зєднатиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Microsoft.VisualBasic.PowerPacks.OvalShape ovalshape1, ovalshape2;
            if (conect_ovalshape2 == null)
                conect_ovalshape2 = conect_ovalshape1;
            else
            {
                //Лінії
                n_conekt++;
                ovalshape1 = (Microsoft.VisualBasic.PowerPacks.OvalShape)conect_ovalshape1;
                ovalshape2 = (Microsoft.VisualBasic.PowerPacks.OvalShape)conect_ovalshape2;
                x1 = ovalshape1.Location.X;
                y1 = ovalshape1.Location.Y;
                x2 = ovalshape2.Location.X;
                y2 = ovalshape2.Location.Y;
                rectangleShape1.Parent.Shapes.Add(lineshape[n_conekt]);//Вивід лінії з’єдння пунктів
                lineshape[n_conekt].StartPoint = new Point(x1 + 12, y1 + 12);
                lineshape[n_conekt].EndPoint = new Point(x2 + 12, y2 + 12);
                //--------------------------------------------------------------------------------
                //Вивід поля для введення відстані
                tb_length[n_conekt].Location = new Point(Math.Abs(x2 + x1) / 2, Math.Abs(y2 + y1) / 2);
                Controls.Add(tb_length[n_conekt]);
                // Заповнення матриці суміжності 
                matrix_sumizn[int.Parse(ovalshape2.Name), int.Parse(ovalshape1.Name)] = n_conekt + 1;
                matrix_sumizn[int.Parse(ovalshape1.Name), int.Parse(ovalshape2.Name)] = n_conekt + 1;
                //Запам'ятовує ім'я відрізка
                segment_name[int.Parse(ovalshape2.Name), int.Parse(ovalshape1.Name)] =
                    int.Parse(lineshape[n_conekt].Name);
                segment_name[int.Parse(ovalshape1.Name), int.Parse(ovalshape2.Name)] =
                    int.Parse(lineshape[n_conekt].Name);
                if (!mouseRight)
                {
                    ovalshape1.BackColor = Color.Red;
                    ovalshape2.BackColor = Color.Red;
                }

                conect_ovalshape2 = null;
            }
        }
        //--------------------------------------------------------------------------------------------------------

        //Запуск пошуку шляху
        private void запускToolStripMenuItem_Click(object sender, EventArgs e)
        {
            проектToolStripMenuItem.Enabled = false;
            panel3.Visible = true;
            запускToolStripMenuItem.Enabled = false;
            файлToolStripMenuItem.Enabled = true;
            //Створення матриці суміжності з величинами
            for (int i = 0; i <= n_ovalshape; i++)
            {
                for (int j = 0; j <= n_ovalshape; j++)
                    if (matrix_sumizn[i, j] > 0)
                        matrix_sumizn[i, j] = double.Parse(tb_length[(int)(matrix_sumizn[i, j]) - 1].Text);
            }
            textBox1.Text += '\t';
            for (int i = 0; i <= n_ovalshape; i++)
                textBox1.Text += (char)('A' + i) + "\t";
            textBox1.Text += Environment.NewLine;
            textBox1.Text += Environment.NewLine;
            for (int i = 0; i <= n_ovalshape; i++)
            {
                textBox1.Text += (char)('A' + i) + "\t";
                for (int j = 0; j <= n_ovalshape; j++)
                    textBox1.Text += matrix_sumizn[i, j].ToString() + "\t";
                textBox1.Text += Environment.NewLine;
                textBox1.Text += Environment.NewLine;
            }
            //------------------------------------------------------------------------------------------
            panel2.Visible = true;
            panel2.Location = new Point(12, 27);
            сховатиІнформаціюПроШляхToolStripMenuItem.Visible = true;
            показатиМатрицюСуміжностіToolStripMenuItem.Visible = true;
            //Пошук шляхів
            Shljah.vvid(matrix_sumizn, n_ovalshape + 1, begin, end);
            Shljah.vuvid(out sort_shljah, out len_shljah, out n_shljahiv);
            //заповнення інформації про початок і кінець шляху
            textBox2.Text += "Кількість всіх точок: " + (n_ovalshape + 1).ToString();
            textBox2.Text += Environment.NewLine;
            textBox2.Text += "Кількість всіх можлівих шляхів: " + (n_shljahiv).ToString();
            textBox2.Text += Environment.NewLine;
            //------------------------------------------------------------------------------------------
            //Вивід найкоротшого шляху
            if (n_shljahiv > 0)
            {
                for (int i = 0; i < sort_shljah[0].Length; i++)
                {
                    if (sort_shljah[0][i + 1] != 0)
                    {
                        int j = segment_name[(int)sort_shljah[0][i] - 1, (int)sort_shljah[0][i + 1] - 1];
                        lineshape[j].BorderColor = Color.Green;
                        lineshape[j].BorderWidth = 10;
                    }
                    else break;
                }
                label4.Text = "№ 1/" + n_shljahiv.ToString();
                label5.Text = "Довжина шляху: " + len_shljah[0].ToString();
                n_shlja_naw++;
                textBox2.Text += "Найкоротший шлях:";
                textBox2.Text += Environment.NewLine;
                for (int i = 0; i < sort_shljah[0].Length; i++)
                    if (sort_shljah[0][i] != 0) textBox2.Text += tb[(int)sort_shljah[0][i] - 1].Text + ",  ";
                    else break;
            }
            else
            {
                textBox2.Text += "Шлях відсутній";
            }
            if (n_shljahiv < 2)
            {
                button1.Enabled = false;
            }
            textBox2.Text += Environment.NewLine;
            textBox2.Text += label5.Text;
            зберегтиМатрицюВФайлToolStripMenuItem.Visible = true;
            зберегтиІнформаціюПроШляхВФайлToolStripMenuItem.Visible = true;
            зберегтиВсіМожливіШляхиВФайлToolStripMenuItem.Visible = true;
        }
        //--------------------------------------------------------------------------------------------------------

        //Перехід на наступний шлях
        private void button1_Click(object sender, EventArgs e)
        {
            if (n_shlja_naw != n_shljahiv)
            {
                for (int i = 0; i < sort_shljah[n_shlja_naw - 1].Length; i++)
                {
                    if (sort_shljah[n_shlja_naw - 1][i + 1] != 0)
                    {
                        int j = segment_name[(int)sort_shljah[n_shlja_naw - 1][i] - 1, (int)sort_shljah[n_shlja_naw - 1][i + 1] - 1];
                        lineshape[j].BorderColor = Color.Black;
                        lineshape[j].BorderWidth = 1;
                    }
                    else break;
                }
                for (int i = 0; i < sort_shljah[n_shlja_naw].Length; i++)
                {
                    if (sort_shljah[n_shlja_naw][i + 1] != 0)
                    {
                        int j = segment_name[(int)sort_shljah[n_shlja_naw][i] - 1, (int)sort_shljah[n_shlja_naw][i + 1] - 1];
                        lineshape[j].BorderColor = Color.Green;
                        lineshape[j].BorderWidth = 10;
                    }
                    else break;
                }
                label4.Text = "№ " + (n_shlja_naw + 1).ToString() + "/" + n_shljahiv.ToString();
                label5.Text = "Довжина шляху: " + len_shljah[n_shlja_naw].ToString();
            }
            else if (n_shlja_naw == n_shljahiv)
                button1.Enabled = false;
            n_shlja_naw++;
        }

        //Файл
        private void зберегтиМатрицюВФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fileName;
            saveFileDialog1.ShowDialog();
            fileName = saveFileDialog1.FileName;
            FileStream failS = new FileStream(fileName, FileMode.CreateNew);
            StreamWriter sWriter = new StreamWriter(failS);
            sWriter.WriteLine(textBox1.Text);
            sWriter.Close();
            failS.Close();
        }
        private void зберегтиІнформаціюПроШляхВФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fileName;
            saveFileDialog1.ShowDialog();
            fileName = saveFileDialog1.FileName;
            FileStream failS = new FileStream(fileName, FileMode.CreateNew);
            StreamWriter sWriter = new StreamWriter(failS);
            sWriter.WriteLine(textBox2.Text);
            sWriter.Close();
            failS.Close();
        }
        private void зберегтиВсіМожливіШляхиВФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fileName,
                dzs = "";
            saveFileDialog1.ShowDialog();
            fileName = saveFileDialog1.FileName;
            FileStream failS = new FileStream(fileName, FileMode.CreateNew);
            StreamWriter sWriter = new StreamWriter(failS);
            for (int i = 0; i < n_shljahiv; i++)
            {
                for (int j = 0; j < sort_shljah[i].Length; j++)
                    if (sort_shljah[i][j] != 0)
                        dzs += tb[sort_shljah[i][j]-1].Text + ", ";
                    else break;
                sWriter.WriteLine("{0}) {1} \t  - {2}", i + 1, dzs, len_shljah[i]);
                dzs = "";
            }
            sWriter.Close();
            failS.Close();
        }
        private void зберегтиІнформаціюПроРезультатВHtmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel1.Visible = panel2.Visible = false;
            string fileName;
            saveFileDialog1.ShowDialog();
            fileName = saveFileDialog1.FileName;
            System.Threading.Thread.Sleep(500);
            Bitmap bmpScreenshot;
            Graphics gfxScreenshot;
            bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height - 70);
            gfxScreenshot = Graphics.FromImage(bmpScreenshot);
            System.Drawing.Size size = Screen.PrimaryScreen.Bounds.Size;
            gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y + 40, 0, 0,
               size, CopyPixelOperation.SourceCopy);
            bmpScreenshot.Save(fileName + " scrin.jpeg", System.Drawing.Imaging.ImageFormat.Jpeg);
            string dzFileName = fileName;
            dzFileName =Path.GetFileNameWithoutExtension(fileName);
            string htmlText = @"<HTML><HEAD><TITLE>Результат</TITLE></HEAD>
                                <BODY><CENTER>
                                        <H2>Результат</H2>
                                      </CENTER>
                                      <IMG SRC=" + "\"" + dzFileName + " scrin.jpeg\">" +
                                     @"<CENTER>
	                                  <H2>Інформація про шляхи</H2>
                                       </CENTER>
                                       Кількість всіх точок: " + (n_ovalshape + 1).ToString() + @" <BR>" +
                                      @"Кількість всіх можлівих шляхів: " + (n_shljahiv).ToString() + @" <BR>" +
                                      @"Початкова точка шляху: " + tb[begin].Text + @" <BR>" +
                                     @"Кінцева точка шляху:" + tb[end].Text + @" <BR>" +
                                     @"<CENTER>
	                                        <H4>Найкоротший шлях</H4>
                                       </CENTER>";
            for (int i = 0; i < sort_shljah[0].Length; i++)
                if (sort_shljah[0][i] != 0) htmlText += tb[(int)sort_shljah[0][i] - 1].Text + @" --> ";
                else break;
            htmlText += "Довжина шляху: " + len_shljah[0].ToString() + @" <BR>";
            htmlText += @"<CENTER>
	                          <H4>Всі можливі шляхи</H4>
                        </CENTER>";
            string dzs = "";
            for (int i = 0; i < n_shljahiv; i++)
            {
                for (int j = 0; j < sort_shljah[i].Length; j++)
                    if (sort_shljah[i][j] != 0)
                        dzs += tb[sort_shljah[i][j] - 1].Text + @" --> ";
                    else break;
                htmlText += (i + 1).ToString() + ") " + dzs + "Довжина шляху: " +
                    len_shljah[i].ToString() + @" <BR>";
                dzs = "";
            }
            htmlText += @"</BODY>
                        </HTML>";

            FileStream failS = new FileStream(fileName+".htm", FileMode.CreateNew);
            StreamWriter sWriter = new StreamWriter(failS,Encoding.UTF8);
            sWriter.WriteLine(htmlText);
            sWriter.Close();
            failS.Close();
            DialogResult dialogrezult1 = MessageBox.Show("Збереження завершено! Відкрити файл?", "Збереження",
              MessageBoxButtons.YesNo);
            if (dialogrezult1 == DialogResult.Yes)
                Process.Start(fileName + ".htm");
        }
        //---------------------------------------------------------------------------------------------------------
    }
}
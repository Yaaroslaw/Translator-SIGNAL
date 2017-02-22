using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Parser_Generator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeTreeView();
        }
        private void InitializeTreeView()
        {
            textBox2.Text = "TEST PROGRAM\r\n";
            textBox2.Text += "\r\n";
            string[] lines = System.IO.File.ReadAllLines(@"C:\Users\Yaroslaw\Documents\Visual Studio 2015\Projects\Женя\Parser_Generator\Parser_Generator\bin\Debug\test.dat");
            foreach (string line in lines)
            {
                // Use a tab to indent each line of the file.
                textBox2.Text += line;
                textBox2.Text += "\r\n";
            }
            textBox1.Text = "SCANNER\r\n";
            Scanner scanner = new Scanner("test.dat");
            Parser parser = new Parser();
            Tables t1 = new Tables();
            Dictionary<string, int> variables = t1.variables;
            Dictionary<int, List<string>> t = t1.table();
            Dictionary<string, int> constTable = t1.constTable;
            Generator generator = new Generator();

            try
            {

                List<int> lexCode = scanner.Scan(textBox1, ref variables, ref constTable);
                parser.Parse(lexCode, ref treeView1, ref variables, ref constTable);

            }
            catch (Exception e)
            {
                Task task = Task.Run(() => { MessageBox.Show(e.Message); });
            }
            generator.Generate(treeView1.Nodes[0]);
            generator.fileToWrite.Close();

        }
    }
}

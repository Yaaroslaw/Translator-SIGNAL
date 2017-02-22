using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

namespace Parser_Generator
{
    class Generator
    {
        public StreamWriter fileToWrite;
        List<string> vars;
        string s1 = "";

        public Generator()
        {
            fileToWrite = new StreamWriter("1.asm");
            vars = new List<string>();
        }

        ~Generator()
        {
            fileToWrite.Close();
        }

        public void Generate(TreeNode tree)
        {

            List<int> s = new List<int>();
            string rule = tree.Text;


            switch (rule)
            {
                case "<signal-program>":
                    {                      
                        Generate(tree.Nodes[0]);
                        break;
                    }

                case "<program>":
                    {
                        if (tree.Nodes[0].Text == "PROGRAM")
                        {
                            fileToWrite.WriteLine("MACRO " + tree.Nodes[1].Nodes[0].Nodes[0].Text);
                            Generate(tree.Nodes[3]);
                        }
                        else
                        {
                            //Generate(tree.Nodes[1]);
                            fileToWrite.WriteLine(tree.Nodes[1].Nodes[0].Nodes[0].Text + " PROC");
                            Generate(tree.Nodes[2]);
                            Generate(tree.Nodes[4]);
                            fileToWrite.WriteLine(tree.Nodes[1].Nodes[0].Nodes[0].Text + " ENDP");
                        }
                        break;
                    }

                case "<block>":
                    {
                        Generate(tree.Nodes[0]);
                        fileToWrite.WriteLine(".CODE");
                        fileToWrite.WriteLine("START:");
                        Generate(tree.Nodes[2]);
                        fileToWrite.WriteLine("END START");
                        fileToWrite.WriteLine("END");
                        break;
                    }
                case "<declarations>":
                    {
                        Generate(tree.Nodes[0]);
                        Generate(tree.Nodes[1]); 
                        break;
                    }

                case "<variables-declarations>":
                    {
                        for (int i = 0; i < tree.Nodes.Count; i++)
                        {
                            if (tree.Nodes[i].Text == "<variable>")
                            {
                                if (vars.Contains(tree.Nodes[i].Nodes[0].Nodes[0].Text))
                                    throw new Exception("Double declaration");
                                if (tree.Nodes[i+2].Nodes[0].Text == "INT")
                                {
                                    vars.Add(tree.Nodes[i].Nodes[0].Nodes[0].Text);
                                    fileToWrite.WriteLine("\t" + tree.Nodes[i].Nodes[0].Nodes[0].Text + "\tDD\t?");
                                    
                                }
                                else if (tree.Nodes[i+2].Nodes[0].Text == "REAL")
                                {
                                    vars.Add(tree.Nodes[i].Nodes[0].Nodes[0].Text);
                                    fileToWrite.WriteLine("\t" + tree.Nodes[i].Nodes[0].Nodes[0].Text + "\treal4\t?");

                                }
                            }
                                
                        }
                        break;
                    }
                case "<label-declarations>":
                    {
                        Generate(tree.Nodes[1]);
                        break;
                    }
                case "<label-list>":
                    {
                        for (int i = 0; i < tree.Nodes.Count; i++)
                        {
                            if (tree.Nodes[i].Text == "<label>")
                                Generate(tree.Nodes[i]);
                        }
                        break;
                    }
                case "<label>":
                    {
                        Generate(tree.Nodes[0]);                       
                        break;
                    }
                case "<unsigned-integer>":
                    {
                        fileToWrite.WriteLine(tree.Nodes[0].Text + " LABEL WORD");
                        break;
                    }
                case "<statements-list>":
                    {
                        break;
                    }
               
            }
        }
    }
}

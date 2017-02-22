using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using static Parser_Generator.Tables;
using System.Windows.Forms;


namespace Parser_Generator
{
    class Scanner
    {
        StreamWriter fileToWrite;
        Stream file;
        Tables tables = new Tables();

        public Scanner(string fileName)
        {
            file = new FileStream(fileName, FileMode.Open);
            fileToWrite = new StreamWriter(fileName.Remove((int)fileName.Length - 4, 4) + ".txt");
        }

        tsymbol Gets()
        {
            tsymbol newSymbol;
            int newByte = file.ReadByte();

            if (newByte == -1)
            {
                newSymbol.value = ' ';
                newSymbol.attr = 6;
                return newSymbol;
            }

            newSymbol.value = Convert.ToChar(newByte);
            newSymbol.attr = tables.AttrTabSearch(newSymbol.value);
            return newSymbol;
        }

        public List<int> Scan(TextBox textBox1, ref Dictionary<string, int> variables, ref Dictionary<string, int> constTable)
        {
            tsymbol newSymbol = Gets();
            bool outputFlag;
            bool pointFlag;
            List<int> lexCode = new List<int>();
            StringBuilder buffer = new StringBuilder("");
            string strBuffer = "";
            int rawCount = 1;
            int i = 0;

            while (newSymbol.attr != 6)
            {
                buffer.Clear();
                outputFlag = false;
                switch (newSymbol.attr)
                {
                    case 0:
                        {
                            while ((newSymbol.attr != 6) && (newSymbol.attr == 0))
                            {
                                if (newSymbol.value == '\n')
                                    rawCount++;
                                newSymbol = Gets();
                            }
                            outputFlag = true;
                            break;
                        }
                    case 1:
                        {
                            pointFlag = false;
                            while ((newSymbol.attr != 6) && ((newSymbol.attr == 1) || ((newSymbol.value == '.')
                                                                                  && (pointFlag != true))))
                            {
                                if (newSymbol.value == '.')
                                    pointFlag = true;
                                buffer.Append(newSymbol.value);
                                newSymbol = Gets();
                            }
                            strBuffer = buffer.ToString();

                            if (constTable.ContainsValue(newSymbol.value))
                                lexCode.Add(constTable[strBuffer]);
                            else
                            {
                                lexCode.Add(constTable.Count + 301);
                                constTable.Add(strBuffer, constTable.Count + 301);
                            }
                            break;

                        }
                    case 2:
                        {
                            while ((newSymbol.attr != 6) && ((newSymbol.attr == 1) || (newSymbol.attr == 2)))
                            {
                                buffer.Append(newSymbol.value);
                                newSymbol = Gets();
                            }
                            strBuffer = buffer.ToString();

                            if (tables.keyWords.ContainsKey(strBuffer))
                                lexCode.Add(tables.keyWords[strBuffer]);
                            else
                            {
                                if (variables.ContainsKey(strBuffer))
                                    lexCode.Add(variables[strBuffer]);
                                else
                                {
                                    lexCode.Add(variables.Count + tables.keyWords.Count + 501);
                                    variables.Add(strBuffer, variables.Count + tables.keyWords.Count + 501);
                                }
                            }
                            break;
                        }
                    case 3:
                        {
                            newSymbol = Gets();
                            if (newSymbol.attr == 4)
                            {
                                strBuffer = "(";                         
                                
                                if (tables.delTab.ContainsKey(strBuffer))
                                    lexCode.Add(tables.delTab[strBuffer]);
                                else
                                {
                                    tables.delTab.Add(strBuffer, tables.delTab.Count);
                                    lexCode.Add(tables.delTab.Count + 40 - 1);
                                }
                                strBuffer = newSymbol.value.ToString();
                                strBuffer = "(";
                            }
                            else
                            {
                                if (newSymbol.value == '*')
                                {
                                    do
                                    {
                                        do
                                        {
                                            newSymbol = Gets();

                                            if (newSymbol.attr == 6)
                                            {
                                                fileToWrite.WriteLine("'*)' expected, but end of file found; Line: " + rawCount);
                                                newSymbol.value = '+';
                                                outputFlag = true;
                                                break;
                                            }
                                        } while (newSymbol.value != '*');
                                        if (newSymbol.attr == 6) break;
                                        newSymbol = Gets();
                                    } while (newSymbol.value != ')');

                                    if (newSymbol.value == ')')
                                    {
                                        outputFlag = true;
                                        newSymbol = Gets();
                                    }
                                }
                                else
                                {
                                    if (tables.delTab.ContainsKey(strBuffer))
                                        lexCode.Add(tables.delTab[strBuffer] + 40);
                                    else
                                    {
                                        tables.delTab.Add(strBuffer, tables.delTab.Count);
                                        lexCode.Add(tables.delTab.Count + 40 - 1);
                                    }
                                    strBuffer = newSymbol.value.ToString();
                                    newSymbol = Gets();
                                    strBuffer = "(";
                                }
                            }

                            break;
                        }
                    case 4:
                        {
                            strBuffer = newSymbol.value.ToString();

                            if (tables.delTab.ContainsKey(strBuffer))
                                lexCode.Add(tables.delTab[strBuffer]);
                            else
                            {
                                tables.delTab.Add(strBuffer, tables.delTab.Count);
                                lexCode.Add(tables.delTab.Count + 40 - 1);
                            }
                            strBuffer = newSymbol.value.ToString();
                            newSymbol = Gets();
                            break;
                        }
                    case 5:
                        {
                            fileToWrite.WriteLine("Illegal symbol; Line: " + rawCount);
                            strBuffer = Convert.ToString(newSymbol.value);
                            lexCode.Add(666);
                            newSymbol = Gets();
                            break;
                        }
                }

                if (!outputFlag)
                {
                    fileToWrite.WriteLine("{0} : {1}", strBuffer, lexCode[i]);
                    i++;
                }
                    
            }

            fileToWrite.WriteLine();
            fileToWrite.WriteLine();
            fileToWrite.WriteLine("Constants table");
            fileToWrite.WriteLine("----------------------------");
            fileToWrite.WriteLine("|Name\t\t|\t\t\tCode|");
            fileToWrite.WriteLine("----------------------------");

            foreach (KeyValuePair<string, int> kvp in constTable)
                fileToWrite.WriteLine("|\t{0}\t\t\t{1}\t", kvp.Key, kvp.Value);
            fileToWrite.WriteLine("----------------------------");



            fileToWrite.WriteLine();
            fileToWrite.WriteLine();
            fileToWrite.WriteLine("Variables table");
            fileToWrite.WriteLine("----------------------------");
            fileToWrite.WriteLine("|Name\t\t|\t\t\tCode|");
            fileToWrite.WriteLine("----------------------------");

            foreach (KeyValuePair<string, int> kvp in variables)
                fileToWrite.WriteLine("|\t{0}\t\t\t{1}\t", kvp.Key, kvp.Value);
            fileToWrite.WriteLine("----------------------------");

            //ff
            fileToWrite.WriteLine();
            fileToWrite.WriteLine();
            fileToWrite.WriteLine("Keywords table");
            fileToWrite.WriteLine("----------------------------");
            fileToWrite.WriteLine("|Name\t\t|\t\t\tCode|");
            fileToWrite.WriteLine("----------------------------");

            foreach (KeyValuePair<string, int> kvp in tables.keyWords)
                fileToWrite.WriteLine("|\t{0}\t\t\t{1}\t", kvp.Key, kvp.Value);
            fileToWrite.WriteLine("----------------------------");
            for (int j = 0; j < lexCode.Count; j++)
            {
                textBox1.Text += String.Format(" " + lexCode[j]);
            }
            //ff

            file.Dispose();
            fileToWrite.Dispose();
            return lexCode;
        }
    }
}

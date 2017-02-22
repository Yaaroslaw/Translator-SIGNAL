using System.Collections.Generic;
using System.Windows.Forms;
using System;


/*PROCEDURE EVGEN();
LABEL 1, 2
BEGIN
(* ANOTHER COMMENT *)
END;*/



namespace Parser_Generator
{
    class Parser
    {
        public int findPosition(Dictionary<int, List<string>> Knuth, string str) 
        {
            int pos = 0;
            for (int i = 1; i < Knuth.Count; i++)
            {
                if (Knuth[i][0] == str) // AT-field of the Knuth table
                {
                    pos = i;
                    break;
                }
            }
            return pos;
        }
        public string searchConst(List<int> lexCode, ref Dictionary<string, int> constTable, int pos)
        {
            string s = "";
            foreach (KeyValuePair<string, int> kvp in constTable)
            {
                if (kvp.Value == lexCode[pos])
                {
                    s = kvp.Key;
                    break;
                }
            }
            return s;
        }

        Tables tables = new Tables();
        public void Parse(List<int> lexCode, ref TreeView treeView1, 
            ref Dictionary<string, int> variables, ref Dictionary<string, int> constTable)
        {
            Dictionary<int, List<string>> Knuth = tables.table();
            bool end = false; // bool variable for the correct exit
            int line = 1; //curent line in Knuth table
            int pos = 0; // position in the string of lexemes
            int back = 0; // position to return
            int start = 0; 
            int errorCount = 0; // for saving the number of line, which contains the error
            TreeNode programNode = treeView1.Nodes.Add("<signal-program>").Nodes.Add("<program>"); // tree-stuff
            TreeNode currentNode = new TreeNode("<labels-list>");
            TreeNode permamentNode = new TreeNode("");
            while (!end)
            {
                switch (Knuth[line][1])
                {   
                    case "program":
                        if (lexCode.Count != 0) // if the string of lexems isn't empty
                        {                            
                            if (lexCode[pos] == 501)// "PROGRAM"
                            {
                                programNode.Nodes.Add("PROGRAM");
                                start = 0;
                                line = 3;
                                pos++;
                            }
                            else if (lexCode[pos] == 505) // "PROCEDURE"
                            {
                                programNode.Nodes.Add("PROCEDURE");
                                start = 1;
                                line = 8;
                                pos++;
                                
                            }
                            else
                            {
                                throw new Exception("ERROR: Key word 'PROGRAM' or 'PROCEDURE' expected. On line: " + errorCount);
                            }
                        }
                        else
                        {
                            throw new Exception("ERROR: NO INPUT!");
                        }
                        break;
                    case "procedure-identifier":
                        {
                            back = line;
                            line = findPosition(Knuth, "procedure-identifier");
                            break;
                        }
                    case "identifier":
                        {
                            string s = "";
                            if (lexCode[pos] >= 506)
                            {
                                foreach (KeyValuePair<string, int> kvp in variables)
                                {
                                    if (kvp.Value == lexCode[pos])
                                    {
                                        s = kvp.Key;
                                        break;
                                    }
                                }
                                if ((s != "") & (back != 21))
                                {
                                    TreeNode identifierNode = programNode.Nodes.Add("<procedure-identifier>").Nodes.Add("<identifier>").Nodes.Add("" + lexCode[pos]);
                                    identifierNode.Nodes.Add(s);
                                    pos++;
                                }                           
                                if (back == 3)
                                {
                                    line = 4;
                                    back = line;
                                }
                                else if (back == 8)
                                {
                                    line = 9;
                                }
                                else if (back == 21)
                                {
                                    if (start == 0)
                                        programNode.Nodes[3].Nodes[0].Nodes[1].Nodes.Add("<variable>").Nodes.Add("" + lexCode[pos]).Nodes.Add(s);
                                    else
                                        programNode.Nodes[4].Nodes[0].Nodes[1].Nodes.Add("<variable>").Nodes.Add("" + lexCode[pos]).Nodes.Add(s);
                                    pos++;
                                    line = 22; //NOT TO CHANGE
                                }                                
                            }
                            else if (lexCode[pos] == 503)
                            {
                                line = 14;
                            }
                            else
                            {
                                throw new Exception("ERROR: Identifier expected. On line: " + errorCount);
                            }
                            break;                            
                        }
                    case "666":
                        {
                            if (lexCode[pos] == 666)
                            {
                                if (start == 0)
        
                                    programNode.Nodes[3].Nodes[0].Nodes[1].Nodes.Add(":");
                                else
                                    programNode.Nodes[4].Nodes[0].Nodes[1].Nodes.Add(":");
                                pos++;
                                line = 23;
                                back = line;
                            }
                            else
                            {
                                throw new Exception("ERROR! ON LINE: " + errorCount);
                            }
                            break;
                        }
                    case "parameters-list":
                        {
                            line = findPosition(Knuth, "parameters-list");
                            break;
                        }
                    case "40": 
                        {
                            if ((lexCode[pos] != 40) & (lexCode[pos] != 507) & (lexCode[pos] != 508))
                                throw new Exception("ERROR: Delimeter ';' expected! On line: " + errorCount);
                            else if ((lexCode[pos] == 40) & (back != 23))
                            {
                                programNode.Nodes.Add(";");
                                pos++;
                                errorCount++;
                            }
                            if (back == 4)
                            {
                                line = 5; 
                            }
                            else if (back == 10)
                            {
                                line = 11;
                            }
                            else if (back == 12)
                            {
                                end = true; 
                            }
                            else if (back == 23)
                            {

                                if (lexCode[pos] == 503)
                                {
                                    line = 14;
                                }
                                else
                                {
                                    if (lexCode[pos] == 507)
                                    {
                                        if (start == 0)
                                            programNode.Nodes[3].Nodes[0].Nodes[1].Nodes.Add("<type>").Nodes.Add("INT");
                                        else
                                            programNode.Nodes[4].Nodes[0].Nodes[1].Nodes.Add("<type>").Nodes.Add("INT");
                                        pos++;
                                    }
                                    else if (lexCode[pos] == 508)
                                    {
                                        if (start == 0) 
                                            programNode.Nodes[3].Nodes[0].Nodes[1].Nodes.Add("<type>").Nodes.Add("REAL");
                                        
                                        else
                                            programNode.Nodes[4].Nodes[0].Nodes[1].Nodes.Add("<type>").Nodes.Add("REAL"); ;
                                        pos++;
                                    }
                                    else
                                    {
                                        throw new Exception("ERROR: KEY WORDS 'REAL' OR 'INT' " + errorCount);
                                    }
                                    if (start == 0)
                                        programNode.Nodes[3].Nodes[0].Nodes[1].Nodes.Add(";");
                                    else
                                        programNode.Nodes[4].Nodes[0].Nodes[1].Nodes.Add(";");
                                    line = 20;
                                    pos++;
                                }
                            }
                            break;
                        }
                    case "42": // It's fine, just a  ","
                        {
                            string s = "";
                            if ((lexCode[pos] == 42) & (lexCode[pos + 1 ] != 503))
                            {
                                if (start == 0)
                                    programNode.Nodes[3].Nodes[0].Nodes[0].Nodes[1].Nodes.Add(","); 
                                else
                                    programNode.Nodes[4].Nodes[0].Nodes[0].Nodes[1].Nodes.Add(","); 
                                pos++;
                                s = searchConst(lexCode, ref constTable, pos); // IS S EMPTY
                                if (s != "")
                                {
                                    if (start == 0)
                                        programNode.Nodes[3].Nodes[0].Nodes[0].Nodes[1].Nodes.Add("<label>").Nodes.Add("<unsigned-integer>").Nodes.Add(s);
                                    else
                                        programNode.Nodes[4].Nodes[0].Nodes[0].Nodes[1].Nodes.Add("<label>").Nodes.Add("<unsigned-integer>").Nodes.Add(s);
                                    pos++;
                                }
                                if (lexCode[pos] == 42) // ,
                                {
                                    line = 28;
                                }
                                else
                                {
                                    if (lexCode[pos] == 40)
                                    {
                                        if (start == 0)
                                            programNode.Nodes[3].Nodes[0].Nodes[0].Nodes[1].Nodes.Add(";");
                                        else
                                            programNode.Nodes[4].Nodes[0].Nodes[0].Nodes[1].Nodes.Add(";");
                                        pos++;
                                    }
                                    errorCount++;
                                    line = 18;
                                }                   
                               
                            }
                            else throw new Exception("ERROR. On line: " + errorCount);
                            break;
                        }
                    case "variable-declarations":
                        {
                            if (lexCode[pos] == 506) // VAR
                            {
                                if (start == 0)
                                    programNode.Nodes[3].Nodes[0].Nodes.Add("<variables-declarations>").Nodes.Add("VAR");
                                else
                                    programNode.Nodes[4].Nodes[0].Nodes.Add("<variables-declarations>").Nodes.Add("VAR");
                                line = 20;
                                pos++;
                            }
                            else
                            {
                                throw new Exception("ERROR: KEY WORD 'VAR EXPECTED! ON LINE:' " + errorCount);
                            }
                            break;
                        }
                    case "variable":
                        {

                            
                            line = 21; // LINE OF IDENTIFIER
                            back = line;
                            break;
                        }
                    case "43": // Relax. It's just "("
                        {

                            if ((lexCode[pos] == 43) & (lexCode[pos + 1] == 44))
                            {
                                TreeNode parametersNode = programNode.Nodes.Add("<parameters-list>");
                                parametersNode.Nodes.Add("(");
                                parametersNode.Nodes.Add("<declarations-list>").Nodes.Add("<empty>");
                                parametersNode.Nodes.Add(")");
                                pos += 2;
                                line = 10;

                            }
                            else if (lexCode[pos] == 40)
                            {
                                line = 10;
                                TreeNode parametersNode = programNode.Nodes.Add("<parameters-list>").Nodes.Add("<empty>");
                            }
                            
                            else throw new Exception("ERROR: 'empty' expected. On line: " + errorCount);
                            back = line;
                            break;
                        }

                    case "503": // BEGIN
                        {
                            if (lexCode[pos] == 503)
                            {
                                if (start == 0)
                                    programNode.Nodes[3].Nodes.Add("BEGIN");
                                else
                                    programNode.Nodes[4].Nodes.Add("BEGIN");
                                errorCount++;
                                pos++;
                                if (lexCode[pos] == 504) // END
                                {
                                    if (start == 0)
                                    {
                                        programNode.Nodes[3].Nodes.Add("<statements-list>").Nodes.Add("<empty>");
                                        programNode.Nodes[3].Nodes.Add("END");
                                    }
                                    else
                                    {
                                        programNode.Nodes[4].Nodes.Add("<statements-list>").Nodes.Add("<empty>");
                                        programNode.Nodes[4].Nodes.Add("END");
                                    }

                                    pos++;
                                    if (pos >= lexCode.Count)
                                    {
                                        throw new Exception("ERROR! On line: " + errorCount);
                                    }
                                    else if ((lexCode[pos] == 41) & ( start == 0))
                                    {
                                        if (start == 0)
                                        {
                                            programNode.Nodes[3].Nodes.Add(".");
                                        }
                                        else
                                        {
                                            programNode.Nodes[4].Nodes.Add(".");
                                        }
                                        
                                        end = true; 
                                    }  
                                    else if ((lexCode[pos] == 40) & (start == 1))
                                    {
                                        if (start == 0)
                                        {
                                            programNode.Nodes[3].Nodes.Add(";");
                                        }
                                        else
                                        {
                                            programNode.Nodes[4].Nodes.Add(";");
                                        }
                                        
                                        end = true;
                                    }
                                    else throw new Exception("ERROR. On line: " + errorCount);
                                }
                                else throw new Exception("ERROR: Key word 'END' expected. On line: " + errorCount);
                            }
                            else throw new Exception("ERROR. On line: " + errorCount);
                            
                            
                            break;
                        }
                    case "block":
                        {
                            programNode.Nodes.Add("<block>");
                            line = findPosition(Knuth, "block");
                            break;
                        }
                    case "declarations":
                        {
                            if (start == 0)
                                programNode.Nodes[3].Nodes.Add("<declarations>");
                            else
                                programNode.Nodes[4].Nodes.Add("<declarations>");
                            line = findPosition(Knuth, "declarations");
                            break;
                        }
                    case "label-declarations":
                        {
                            if (start == 0)
                                programNode.Nodes[3].Nodes[0].Nodes.Add("<label-declarations>");
                            else
                                programNode.Nodes[4].Nodes[0].Nodes.Add("<label-declarations>");
                            if (lexCode[pos] == 502)  //LABEL
                            {
                                if (start == 0)
                                {
                                    programNode.Nodes[3].Nodes[0].Nodes[0].Nodes.Add("LABEL");
                                    programNode.Nodes[3].Nodes[0].Nodes[0].Nodes.Add("<label-list>");
                                }
                                else
                                {
                                    programNode.Nodes[4].Nodes[0].Nodes[0].Nodes.Add("LABEL");
                                    programNode.Nodes[4].Nodes[0].Nodes[0].Nodes.Add("<label-list>");
                                }

                                string s = "";
                                s = searchConst(lexCode, ref constTable, pos + 1); 
                                if (s != "")
                                {
                                    if (start == 0)
                                        programNode.Nodes[3].Nodes[0].Nodes[0].Nodes[1].Nodes.Add("<label>").Nodes.Add("<unsigned-integer>").Nodes.Add(s);
                                    else
                                        programNode.Nodes[4].Nodes[0].Nodes[0].Nodes[1].Nodes.Add("<label>").Nodes.Add("<unsigned-integer>").Nodes.Add(s);
                                    pos += 2;
                                }
                                else throw new Exception("ERROR: At least one unsigned-integer expected! On line: " + errorCount);
                                line = 28;                            
                            }
                            else if (lexCode[pos] == 503)
                            {
                                if (start == 0)
                                    programNode.Nodes[3].Nodes[0].Nodes[0].Nodes.Add("<empty>");
                                else
                                    programNode.Nodes[4].Nodes[0].Nodes[0].Nodes.Add("<empty>");
                                line = 14;
                            }
                            else throw new Exception("ERROR: Key word 'LABEL' expected! On line: " + errorCount);
                            break;
                        }
                }              
            }
        }
    }
}

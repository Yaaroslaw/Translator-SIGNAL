using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Parser_Generator
{
    class Tables
    {
        public struct tsymbol
        {
            public char value;
            public byte attr;
        }

        //Dict of reserved words
        public readonly Dictionary<string, int> keyWords = new Dictionary<string, int>()
        { { "PROGRAM", 501 }, {"LABEL", 502 }, {"BEGIN", 503 }, {"END", 504 }, {"PROCEDURE", 505 }, {"VAR", 506 }, {"INT", 507 }, {"REAL", 508 } }; /*         CHANGES!!!          */

        //Dict of constants
        public Dictionary<string, int> constTable = new Dictionary<string, int>() { };



        //Dict of all character (alphabet)
        public ReadOnlyDictionary<char, byte> attributes = new ReadOnlyDictionary<char, byte>(
            new Dictionary<char, byte>()
            {
                {' ',  0 }, {'0', 1 }, {'a', 2 }, {'A', 2 }, {'(', 3 }, {',', 4 },
                {'\r', 0 }, {'1', 1 }, {'b', 2 }, {'B', 2 },            {';', 4 },
                {'\t', 0 }, {'2', 1 }, {'c', 2 }, {'C', 2 },            {')', 4 },
                {'\n', 0 }, {'3', 1 }, {'d', 2 }, {'D', 2 },            {'.', 4 },
                            {'4', 1 }, {'e', 2 }, {'E', 2 },
                            {'5', 1 }, {'f', 2 }, {'F', 2 },
                            {'6', 1 }, {'g', 2 }, {'G', 2 },
                            {'7', 1 }, {'h', 2 }, {'H', 2 },
                            {'8', 1 }, {'i', 2 }, {'I', 2 },
                            {'9', 1 }, {'j', 2 }, {'J', 2 },
                                       {'k', 2 }, {'K', 2 },
                                       {'l', 2 }, {'L', 2 },
                                       {'m', 2 }, {'M', 2 },
                                       {'n', 2 }, {'N', 2 },
                                       {'o', 2 }, {'O', 2 },
                                       {'p', 2 }, {'P', 2 },
                                       {'q', 2 }, {'Q', 2 },
                                       {'r', 2 }, {'R', 2 },
                                       {'s', 2 }, {'S', 2 },
                                       {'t', 2 }, {'T', 2 },
                                       {'u', 2 }, {'U', 2 },
                                       {'v', 2 }, {'V', 2 },
                                       {'w', 2 }, {'W', 2 },
                                       {'x', 2 }, {'X', 2 },
                                       {'y', 2 }, {'Y', 2 },
                                       {'z', 2 }, {'Z', 2 },
            }
        );

        //Dict for variables
            public Dictionary<string, int> variables = new Dictionary<string, int>() { };

        //public Dictionary<string, int> variables = new Dictionary<string, int>() { };

        public byte AttrTabSearch(char symbol)
        {
            if (attributes.ContainsKey(symbol))
                return attributes[symbol];
            return 5;
        }

        //Dict of delimeters        
        public Dictionary<string, int> delTab = new Dictionary<string, int>()
        { { ";", 40 }, {".", 41 }, {",", 42 }, {"(", 43 }, {")", 44 },  {"*", 45 }, {":", 46 } };


        public Dictionary<int, List<string>> table()
        {
            Dictionary<int, List<string>> Knuth = new Dictionary<int, List<string>>();
            Knuth.Add(1, new List<string> { "signal-program", "program", "2", "F" });
            Knuth.Add(2, new List<string> { "program", "501", "3", "F" });   //PROGRAM
            Knuth.Add(3, new List<string> { "null", "procedure-identifier", "4", "F" });
            Knuth.Add(4, new List<string> { "null", "40", "5", "F" });   //;
            Knuth.Add(5, new List<string> { "null", "block", "6", "F" });
            Knuth.Add(6, new List<string> { "null", "41", "7", "F" });  //.
            Knuth.Add(7, new List<string> { "null", "505", "8", "F" });  //PROCEDURE
            Knuth.Add(8, new List<string> { "null", "procedure-identifier", "9", "F" });
            Knuth.Add(9, new List<string> { "null", "parameters-list", "10", "F" });
            Knuth.Add(10, new List<string> { "null", "40", "11", "F" });   //;
            Knuth.Add(11, new List<string> { "null", "block", "12", "F" }); 
            Knuth.Add(12, new List<string> { "null", "40", "T", "F" });   //;
            Knuth.Add(13, new List<string> { "block", "declarations", "14", "F" });
            Knuth.Add(14, new List<string> { "null", "503", "15", "F" }); //BEGIN
            Knuth.Add(15, new List<string> { "block", "statements-list", "16", "F" });
            Knuth.Add(16, new List<string> { "null", "504", "T", "F" }); //END
            Knuth.Add(17, new List<string> { "declarations", "label-declarations", "18", "F" });
            /*                                  CHANGES                                                */
            Knuth.Add(18, new List<string> { "null", "variable-declarations", "T", "F" });
            Knuth.Add(19, new List<string> { "variable-declarations", "506", "20", "F" }); // VAR
            Knuth.Add(20, new List<string> { "null", "variable", "T", "F" });
            Knuth.Add(21, new List<string> { "variable", "identifier", "22", "F" });
            Knuth.Add(22, new List<string> { "null", "666", "23", "F" }); // :
            Knuth.Add(23, new List<string> { "null", "40", "T", "F" }); // ;
            Knuth.Add(24, new List<string> { "label-declarations", "502", "25", "27" });  //LABEL
            Knuth.Add(25, new List<string> { "null", "unsigned-integer", "26", "F" });
            Knuth.Add(26, new List<string> { "null", "labels-list", "27", "F" });
            Knuth.Add(27, new List<string> { "null", "empty", "T", "F" });
            Knuth.Add(28, new List<string> { "labels-list", "42", "29", "31" });  //,
            Knuth.Add(29, new List<string> { "null", "unsigned-integer", "30", "F" });
            Knuth.Add(30, new List<string> { "null", "labels-list", "31", "F" });
            Knuth.Add(31, new List<string> { "null", "empty", "T", "F" });
            Knuth.Add(32, new List<string> { "parameters-list", "43", "33", "35" }); //(
            Knuth.Add(33, new List<string> { "null", "declaration-list", "34", "F" });
            Knuth.Add(34, new List<string> { "null", "44", "35", "F" });  //)
            Knuth.Add(35, new List<string> { "null", "empty", "T", "F" });
            Knuth.Add(36, new List<string> { "declarations-list", "empty", "T", "36" });
            Knuth.Add(37, new List<string> { "statements-list", "empty", "T", "37" });
            Knuth.Add(38, new List<string> { "procedure-identifier", "identifier", "T", "38" });
            Knuth.Add(39, new List<string> { "identifier", "letter", "40", "F" });
            Knuth.Add(40, new List<string> { "null", "string", "T", "F" });
            Knuth.Add(41, new List<string> { "string", "letter", "42", "45" });
            Knuth.Add(42, new List<string> { "null", "string", "43", "F" });
            Knuth.Add(43, new List<string> { "null", "digit", "44", "F" });
            Knuth.Add(44, new List<string> { "null", "string", "45", "F" });
            Knuth.Add(45, new List<string> { "null", "empty", "T", "F" });
            Knuth.Add(46, new List<string> { "unsigned-integer", "digit", "47", "40" });
            Knuth.Add(47, new List<string> { "null", "digits-string", "T", "F" });
            Knuth.Add(48, new List<string> { "digits-string", "digits", "49", "50" });
            Knuth.Add(49, new List<string> { "null", "digits-string", "50", "F" });
            Knuth.Add(50, new List<string> { "null", "empty", "T", "F" });
            return Knuth;
        }
    }
}


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        BNF LDForm;

        const string numPattern = @"^-?[0-9]+$";
        const string digitPattern = @"^[0-9]$";
        const string variablePattern = @"^[A-Za-zА-Яа-я][0-9A-Za-zА-Яа-я]*$";
        string bnf;

        static int curSymIndex = 0;
        static bool isError = false;

        Dictionary<string, double> variables = new Dictionary<string, double>();


        private bool ParseUravnenie()
        {
            string set = "";
            // int semicolonLastIndex = richTextBox1.Text.IndexOf(';');
            //int semicolonLastIndex = bnf.IndexOf(';');
            int semicolonLastIndex = bnf.IndexOf(';', curSymIndex);
            int SeacrhAnaliz = bnf.IndexOf('а', curSymIndex);

            if ((semicolonLastIndex == -1) && (SeacrhAnaliz != -1))
                semicolonLastIndex = SeacrhAnaliz;
            //while (curSymIndex < richTextBox1.Text.Length - 1 && richTextBox1.Text[curSymIndex] != ';' && curSymIndex < semicolonLastIndex)
            while (curSymIndex < bnf.Length - 1 && bnf[curSymIndex] != ';' && curSymIndex < semicolonLastIndex)
            {
                set += bnf[curSymIndex];
                curSymIndex++;
            }

            set = set.ToLower();

           string ur = bnf.Substring(curSymIndex).Trim().ToLower();
            int endIndex = set.IndexOf('\n');
            if (endIndex != -1)
            {
                ur = ur.Substring(0, endIndex).Trim();
            }
            else
            {
                //curSymIndex += 1;
                return true;
            }

            int colonIndex = set.IndexOf(":");
            int curLexemIndex = 0;

            if (colonIndex != -1)
            {
                string label = "";
                while (curLexemIndex < set.Length && set[curLexemIndex] != ':')
                {
                    label += set[curLexemIndex];
                    curLexemIndex++;
                }
                //curSymIndex += label.Length;
               // curSymIndex++;
                curLexemIndex++;
                label = label.Trim();
                if (!Regex.IsMatch(label, numPattern))
                {
                    int highlightIndex = curSymIndex - curLexemIndex;
                    if (label.Length == 0)
                        richTextBox2.Text = "Ошибка: отсутствует метка перед знаком :";
                    else
                        richTextBox2.Text = "Ошибка: метка не является целочисленной";
                    richTextBox1.Select(highlightIndex, 1);
                    richTextBox1.SelectionBackColor = Color.Red;
                    isError = true;
                    return false;
                }
            }

            int equalSignIndex = set.IndexOf("=");

            if (equalSignIndex == -1)
            {
                int highlightIndex = curSymIndex;
                richTextBox2.Text = "Ошибка: в операторе отсутствует знак =";
                richTextBox1.Select(highlightIndex, 1);
                richTextBox1.SelectionBackColor = Color.Red;
                isError = true;
                return false;
            }

            string variable = "";

            while (curLexemIndex < set.Length && set[curLexemIndex] != '=')
            {
                variable += set[curLexemIndex];
                curLexemIndex++;
            }

            //curSymIndex += variable.Length;

            variable = variable.Trim();

            if (!Regex.IsMatch(variable, variablePattern))
            {
                int highlightIndex = curSymIndex - variable.Length - 1;
                if (variable.Length == 0)
                    richTextBox2.Text = "Ошибка: отсутствует переменная перед знаком =";
                else
                    richTextBox2.Text = "Ошибка: неверный формат переменной";
                richTextBox1.Select(highlightIndex, 1);
                richTextBox1.SelectionBackColor = Color.Red;
                isError = true;
                return false;
            }

            curLexemIndex++;
            //curSymIndex++;

            double urResult = 0;

            bool isRightPartCorrect = ParseRightPart(set.Substring(curLexemIndex), out urResult);

            double residueLong = urResult;

            string residueString = residueLong < 0 ? "-" + Convert.ToString(Math.Abs(residueLong)) : Convert.ToString(residueLong);

            if (!isError && isRightPartCorrect)
            {
                richTextBox2.Text += String.Format("{0} = {1}\n", variable, residueString);
                variables[variable] = urResult;
            }

            //curSymIndex += set.Substring(curLexemIndex).Length + 1;

            curSymIndex++; //УБРАТЬ ЕСЛИ ЧТО

            return true;
        }
        private bool ParseSets()
        {
            while (curSymIndex < richTextBox1.Text.Length - 1)
            {
                bool check_set = false;
                //int semicolonLastIndex = bnf.LastIndexOf(';');
                int semicolonLastIndex = bnf.IndexOf('\n', curSymIndex);

                if (bnf.Length-3<=curSymIndex)
                    return true;
                if (curSymIndex < semicolonLastIndex)
                {
                    
                    check_set = ParseSubset(semicolonLastIndex);
                    semicolonLastIndex = bnf.IndexOf('\n', curSymIndex);
                }
                if (!check_set)
                    break;
                curSymIndex++;
            }
            return true;
        }
        
        private bool ParseSubset(int semicolonLastIndex)
        {
            string set1 = "";
            string set = "";
            ////////////

            set1 = bnf.Remove(semicolonLastIndex, bnf.Length- semicolonLastIndex);

            set = set1.Remove(0, curSymIndex-1);

            ///////////////

            set = set.ToLower();

            int curLexemIndex = 0;
            string curLexem = "";

            //while (curLexemIndex < set.Length && (set[curLexemIndex] == '\n' || set[curLexemIndex] == ' '))
              //  curLexemIndex++;

            while (curLexemIndex < set.Length && set[curLexemIndex] != '\n' && set[curLexemIndex] != ' ')
            {
                curLexem += set[curLexemIndex];
                curLexemIndex++;
            }

            if (curLexem.ToLower() != "анализ" && curLexem.ToLower() != "синтез")
            {
                int highlightIndex = curSymIndex - set.TrimStart().Length + 15;
                richTextBox2.Text = "Ошибка: ожидались слова 'анализ' или 'синтез' в начале множества";
                richTextBox1.Select(highlightIndex, 1);
                richTextBox1.SelectionBackColor = Color.Red;
                isError = true;
                return false;
            }

            bool is_first = false;

            if (curLexem.ToLower() == "анализ")
                is_first = true;

           /* curLexem = "";

            while (curLexemIndex < set.Length && set[curLexemIndex] == '\n' || set[curLexemIndex] == ' ')
            {
                curLexemIndex++;
            }

            while (curLexemIndex < set.Length && set[curLexemIndex] != '\n' && set[curLexemIndex] != ' ')
            {
                curLexem += set[curLexemIndex];
                curLexemIndex++;
            }*/

            int numCount = 1;

            if (is_first)
            {
                while (curLexemIndex < set.Length)
                {
                    curLexem = "";

                    while (set[curLexemIndex] == ' ' || set[curLexemIndex] == '\n')
                        curLexemIndex++;

                    while (curLexemIndex < set.Length && set[curLexemIndex] != ' ' && set[curLexemIndex] != '\n')
                    {
                        curLexem += set[curLexemIndex];
                        curLexemIndex++;
                    }

                    if (numCount == 0 && !Regex.IsMatch(curLexem.Trim(), variablePattern))
                    {
                        int highlightIndex = curSymIndex - set.Length + curLexemIndex - curLexem.TrimStart().Length;
                        richTextBox2.Text = "Ошибка: ожидалась переменная после слова 'анализ'";
                        richTextBox1.Select(highlightIndex, 1);
                        richTextBox1.SelectionBackColor = Color.Red;
                        isError = true;
                        return false;
                    }

                    numCount++;

                    if (!Regex.IsMatch(curLexem.Trim(), variablePattern))
                    {
                        int highlightIndex = curSymIndex - set.Length + curLexemIndex - curLexem.TrimStart().Length;
                        if (curLexem.Trim() == "")
                            richTextBox2.Text = "Ошибка: после запятой должна идти переменная";
                        else if (curLexem.Trim().Split(new char[] { ' ', '\n' }).Length > 1)
                        {
                            richTextBox2.Text = "Ошибка: выражения в множестве не разделены запятой";
                        }
                        else
                            richTextBox2.Text = "Ошибка: неверный формат переменной";
                        richTextBox1.Select(highlightIndex, 1);
                        richTextBox1.SelectionBackColor = Color.Red;
                        isError = true;
                        return false;
                    }
                    else
                        curLexemIndex++;
                }
            }
            else
            {
                while (curLexemIndex < set.Length)
                {
                    curLexem = "";

                    while (set[curLexemIndex] == ' ' || set[curLexemIndex] == '\n')
                        curLexemIndex++;

                    while (curLexemIndex < set.Length && set[curLexemIndex] != ' ' && set[curLexemIndex] != '\n')
                    {
                        curLexem += set[curLexemIndex];
                        curLexemIndex++;
                    }

                    if (numCount == 0 && !Regex.IsMatch(curLexem.Trim(), variablePattern))
                    {
                        int highlightIndex = curSymIndex - set.Length + curLexemIndex - curLexem.TrimStart().Length;
                        richTextBox2.Text = "Ошибка: ожидалась переменная после слова 'синтез'";
                        richTextBox1.Select(highlightIndex, 1);
                        richTextBox1.SelectionBackColor = Color.Red;
                        isError = true;
                        return false;
                    }

                    numCount++;

                    if (!Regex.IsMatch(curLexem.Trim(), variablePattern))
                    {
                        int highlightIndex = curSymIndex - set.Length + curLexemIndex - curLexem.TrimStart().Length;
                        richTextBox2.Text = "Ошибка: неверный формат переменной";
                        richTextBox1.Select(highlightIndex, 1);
                        richTextBox1.SelectionBackColor = Color.Red;
                        isError = true;
                        return false;
                    }
                    else
                        curLexemIndex++;
                }
            }

            curSymIndex = curSymIndex + curLexemIndex - 1;
            return true;
        }
        private bool ParseRightPart(string rightPartString, out double result)
        {
            result = 0;

            if (rightPartString.Length == 0)
            {
                isError = true;
                return false;
            }

            int curChIndex = 0;
            bool hasFirstMinus = false;
            char ch = rightPartString[0];

            while (ch == ' ')
            {
                curChIndex++;
                ch = rightPartString[curChIndex];
            }

            if (ch == '-')
            {
                hasFirstMinus = true;
                curChIndex++;
            }

            string block1_str = "";
            List<string> blocks1 = new List<string>();
            List<char> AddOperations = new List<char>();

            while (curChIndex < rightPartString.Length)
            {
                ch = rightPartString[curChIndex];
                if (ch != '-' && ch != '+')
                {
                    block1_str += ch;
                    curChIndex++;
                }
                else
                {
                    if (block1_str.Trim() == "")
                    {
                        richTextBox2.Text = "Ошибка: два знака операций подряд";
                        int highlightIndex = curSymIndex + curChIndex - 6;
                        richTextBox1.Select(highlightIndex, 1);
                        richTextBox1.SelectionBackColor = Color.Red;
                        isError = true;
                        return false;
                    }
                    else
                    {
                        blocks1.Add(block1_str);
                        AddOperations.Add(ch);
                        curChIndex++;
                        block1_str = "";
                    }
                }
            }

            if (block1_str.Trim() == "")
            {
                richTextBox2.Text = "Ошибка: отсутствует операнд";
                int highlightIndex = curSymIndex + curChIndex - 1;
                richTextBox1.Select(highlightIndex, 1);
                richTextBox1.SelectionBackColor = Color.Red;
                isError = true;
                return false;
            }
            else
            {
                blocks1.Add(block1_str);
                block1_str = "";
            }

            List<double> Block1Results = new List<double>();

            foreach (string b1 in blocks1)
            {
                double b1Result = 0;
                bool isB1Right = ParseBlock1(b1, out b1Result);

                if (!isError && isB1Right)
                {
                    Block1Results.Add(b1Result);
                }
                else
                    return false;
            }

            double res = Block1Results[0];

            if (hasFirstMinus)
                res = -res;

            for (int i = 0; i < AddOperations.Count; i++)
            {
                if (AddOperations[i] == '+')
                {
                    res = res + Block1Results[i + 1];
                }
                else if (AddOperations[i] == '-')
                {
                    res = res - Block1Results[i + 1];
                }
            }

            result = res;

            return true;
        }

        private bool ParseBlock1(string Block1String, out double result)
        {
            result = 0;

            if (Block1String.Length == 0)
            {
                isError = true;
                return false;
            }

            int curChIndex = 0;
            char ch = Block1String[0];

            while (ch == ' ')
            {
                curChIndex++;
                ch = Block1String[curChIndex];
            }

            string block2_str = "";
            List<string> blocks2 = new List<string>();
            List<char> MultOperations = new List<char>();

            while (curChIndex < Block1String.Length)
            {
                ch = Block1String[curChIndex];
                if (ch != '*' && ch != '/')
                {
                    block2_str += ch;
                    curChIndex++;
                }
                else
                {
                    if (block2_str.Trim() == "")
                    {
                        richTextBox2.Text = "Ошибка: два знака операций подряд";
                        int highlightIndex = curSymIndex + curChIndex - 6;
                        richTextBox1.Select(highlightIndex, 1);
                        richTextBox1.SelectionBackColor = Color.Red;
                        isError = true;
                        return false;
                    }
                    else
                    {
                        blocks2.Add(block2_str);
                        MultOperations.Add(ch);
                        curChIndex++;
                        block2_str = "";
                    }
                }
            }

            if (block2_str.Trim() == "")
            {
                richTextBox2.Text = "Ошибка: отсутствует операнд";
                //int highlightIndex = curSymIndex + curChIndex - 1;
                int highlightIndex = curSymIndex + curChIndex - 7;
                richTextBox1.Select(highlightIndex, 1);
                richTextBox1.SelectionBackColor = Color.Red;
                isError = true;
                return false;
            }
            else
            {
                blocks2.Add(block2_str);
                block2_str = "";
            }

            List<double> Block2Results = new List<double>();

            foreach (string b2 in blocks2)
            {
                double b2Result = 0;
                bool isB2Right = ParseBlock2(b2, out b2Result);

                if (!isError && isB2Right)
                {
                    Block2Results.Add(b2Result);
                }
                else
                    return false;
            }

            double res = Block2Results[0];

            for (int i = 0; i < MultOperations.Count; i++)
            {
                if (MultOperations[i] == '*')
                {
                    res = res * Block2Results[i + 1];
                }
                else if (MultOperations[i] == '/')
                {
                    if (Block2Results[i + 1] == 0)
                    {
                        richTextBox2.Text = "Ошибка: деление на 0";
                        int highlightIndex = curSymIndex + curChIndex;
                        richTextBox1.Select(highlightIndex, 1);
                        richTextBox1.SelectionBackColor = Color.Red;
                        isError = true;
                        return false;
                    }
                    else
                        res = res / Block2Results[i + 1];
                }
            }

            result = res;

            return true;
        }

        private bool ParseBlock2(string Block2String, out double result)
        {
            result = 0;
            if (Block2String.Length == 0)
            {
                isError = true;
                return false;
            }

            int curChIndex = 0;
            char ch = Block2String[0];

            while (ch == ' ')
            {
                curChIndex++;
                ch = Block2String[curChIndex];
            }

            string block3_str = "";
            List<string> blocks3 = new List<string>();
            List<string> LogicOperations = new List<string>();

            bool isPreviousOperator = false;

            string currentBlock = "";

            while (curChIndex < Block2String.Length)
            {
                ch = Block2String[curChIndex];
                if (ch != ' ' && ch != '\n')
                {
                    block3_str += ch;
                    curChIndex++;
                }
                else
                {
                    if (block3_str.Trim() == "и" || block3_str.Trim() == "или")
                    {
                        if (isPreviousOperator)
                        {
                            richTextBox2.Text = "Ошибка: две логические операции подряд";
                            int highlightIndex = curSymIndex + curChIndex - 6;
                            richTextBox1.Select(highlightIndex, 1);
                            richTextBox1.SelectionBackColor = Color.Red;
                            isError = true;
                            return false;
                        }
                        isPreviousOperator = true;
                        LogicOperations.Add(block3_str.Trim());
                        block3_str = "";
                        curChIndex++;
                        if (currentBlock.Trim() != string.Empty)
                        {
                            blocks3.Add(currentBlock.Trim());
                            currentBlock = "";
                        }
                    }
                    else
                    {
                        currentBlock += block3_str + " ";
                        curChIndex++;
                        block3_str = "";
                        isPreviousOperator = false;
                    }
                }
            }

            if (block3_str.Trim() != string.Empty)
                currentBlock += block3_str;

            if (currentBlock.Trim() == "" && isPreviousOperator)
            {
                richTextBox2.Text = "Ошибка: отсутствует операнд";
                int highlightIndex = curSymIndex + curChIndex - 1;
                richTextBox1.Select(highlightIndex, 1);
                richTextBox1.SelectionBackColor = Color.Red;
                isError = true;
                return false;
            }
            else
            {
                if (currentBlock.Trim() != "")
                {
                    blocks3.Add(currentBlock);
                    currentBlock = "";
                }
            }

            List<double> Block3Results = new List<double>();

            foreach (string b3 in blocks3)
            {
                double b3Result = 0;
                bool isB3Right = ParseBlock3(b3, out b3Result);

                if (!isError)
                {
                    Block3Results.Add(b3Result);
                }
                else
                    return false;
            }

            double res = Block3Results[0];

            for (int i = 0; i < LogicOperations.Count; i++)
            {
                if (LogicOperations[i] == "и")
                {
                    res = (res - (int)res) + ((int)res & (int)Block3Results[i + 1]);
                }
                else if (LogicOperations[i] == "или")
                {
                    res = (res - (int)res) + ((int)res | (int)Block3Results[i + 1]);
                }
            }

            result = res;

            return true;
        }

        private bool ParseBlock3(string Block3String, out double result)
        {
            result = 0;

            if (Block3String.Length == 0)
            {
                isError = true;
                return false;
            }

            double res = 0;

            if (Block3String.Length > 2 && Block3String.TrimStart().Substring(0, 2) == "не")
            {
                double b4Result = 0;
                bool isB4Right = ParseBlock4(Block3String.TrimStart().Substring(2), out b4Result);
                if (isB4Right)
                    res = (b4Result - (int)b4Result) + ~(int)b4Result;
                else
                    return false;
            }
            else
            {
                bool isB4Right = ParseBlock4(Block3String.Trim(), out res);
                if (!isB4Right)
                    return false;
            }

            result = res;

            return true;
        }

        private bool ParseBlock4(string Block4String, out double result)
        {
            result = 0;
            if (Block4String.Length == 0)
            {
                isError = true;
                return false;
            }

            int curChIndex = 0;
            char ch = Block4String[0];

            while (ch == ' ')
            {
                curChIndex++;
                ch = Block4String[curChIndex];
            }

            string block5_str = "";
            List<string> blocks5 = new List<string>();
            List<string> funcs = new List<string>();

            while (curChIndex < Block4String.Length)
            {
                ch = Block4String[curChIndex];
                if (ch != ' ' && ch != '\n')
                {
                    block5_str += ch;
                    curChIndex++;
                }
                else
                {
                    if (block5_str.Trim() == "sin" || block5_str.Trim() == "cos" || block5_str.Trim() == "abs")
                    {
                        funcs.Add(block5_str.Trim());
                        block5_str = "";
                    }
                    curChIndex++;
                }
            }

            if (block5_str.Trim() == "")
            {
                richTextBox2.Text = "Ошибка: отсутствует операнд";
                int highlightIndex = curSymIndex + curChIndex - 1;
                richTextBox1.Select(highlightIndex, 1);
                richTextBox1.SelectionBackColor = Color.Red;
                isError = true;
                return false;
            }

            double b5Result = 0;
            bool isB5Right = ParseBlock5(block5_str, out b5Result);

            if (!isB5Right)
                return false;

            double res = b5Result;

            for (int i = funcs.Count - 1; i >= 0; i--)
            {
                if (funcs[i] == "sin")
                    res = Math.Sin(res);
                else if (funcs[i] == "cos")
                    res = Math.Cos(res);
                else if (funcs[i] == "abs")
                    res = Math.Abs(res);
            }

            result = res;

            return true;
        }

        private bool ParseBlock5(string Block4String, out double result)
        {
            result = 1;

            if (Block4String == "")
            {
                isError = true;
                return false;
            }

            if (Regex.IsMatch(Block4String.Trim(), numPattern))
            {
                result = Convert.ToInt32(Block4String.Trim(), 10);
                return true;
            }
            else if (Regex.IsMatch(Block4String.Trim(), variablePattern))
            {
                if (variables.ContainsKey(Block4String.Trim()))
                {
                    result = variables[Block4String.Trim()];
                    return true;
                }
                else
                {
                    richTextBox2.Text = "Ошибка: переменной не присвоено значение";
                    int highlightIndex = richTextBox1.Text.IndexOf(Block4String.TrimStart()) > curSymIndex ? richTextBox1.Text.IndexOf(Block4String.TrimStart()) : curSymIndex-2;
                    richTextBox1.Select(highlightIndex, 1);
                    richTextBox1.SelectionBackColor = Color.Red;
                    isError = true;
                    return false;
                }
            }
            else
            {
                richTextBox2.Text = "Ошибка: операнд не является переменной или целочисленным";
                int highlightIndex = richTextBox1.Text.IndexOf(Block4String.TrimStart()) > curSymIndex ? richTextBox1.Text.IndexOf(Block4String.TrimStart()) : curSymIndex-2;
                richTextBox1.Select(highlightIndex, 1);
                richTextBox1.SelectionBackColor = Color.Red;
                isError = true;
                return false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            isError = false;
            variables.Clear();

            richTextBox1.Select(0, richTextBox1.TextLength);
            richTextBox1.SelectionBackColor = Color.White;
            richTextBox2.Clear();

            if (richTextBox1.Text.Length == 0)
            {
                richTextBox2.Text = "Ошибка: поле ввода текста пусто";
                return;
            }

            if (richTextBox1.Text.Length < 5)
            {
                richTextBox2.Text = "Ошибка: пропущено слово 'begin'";
                richTextBox1.Select(0, 1);
                richTextBox1.SelectionBackColor = Color.Red;
                return;
            }

            string curLexem = "";

            curLexem = richTextBox1.Text.TrimStart().Substring(0, 5);
            if (curLexem.ToLower() != "begin")
            {
                richTextBox2.Text = "Ошибка: пропущено слово 'begin'";
                richTextBox1.Select(0, 1);
                richTextBox1.SelectionBackColor = Color.Red;
                return;
            }
            else
                curSymIndex = 5;

            bnf = richTextBox1.Text;
            //bnf=bnf.Remove(0, 5);
            //while (((bnf.ElementAt(curSymIndex) != 'а') && (bnf.ElementAt(curSymIndex + 1) != 'н') && (bnf.ElementAt(curSymIndex + 2) != 'a') && (bnf.ElementAt(curSymIndex + 3) != 'л') && (bnf.ElementAt(curSymIndex + 4) != 'и') && (bnf.ElementAt(curSymIndex + 5) != 'з')) || ((bnf.ElementAt(curSymIndex) != 'с') && (bnf.ElementAt(curSymIndex + 1) != 'и') && (bnf.ElementAt(curSymIndex + 2) != 'н') && (bnf.ElementAt(curSymIndex + 3) != 'т') && (bnf.ElementAt(curSymIndex + 4) != 'е') && (bnf.ElementAt(curSymIndex + 5) != 'з')))
            //while (((bnf.ElementAt(curSymIndex-1) != 'а') && (bnf.ElementAt(curSymIndex) != 'н') && (bnf.ElementAt(curSymIndex + 1) != 'a') && (bnf.ElementAt(curSymIndex + 2) != 'л') && (bnf.ElementAt(curSymIndex + 3) != 'и') && (bnf.ElementAt(curSymIndex + 4) != 'з')) || ((bnf.ElementAt(curSymIndex) != 'с') && (bnf.ElementAt(curSymIndex + 1) != 'и') && (bnf.ElementAt(curSymIndex + 2) != 'н') && (bnf.ElementAt(curSymIndex + 3) != 'т') && (bnf.ElementAt(curSymIndex + 4) != 'е') && (bnf.ElementAt(curSymIndex + 5) != 'з')))
            while (bnf.ElementAt(curSymIndex - 1) != 'а')
                ParseUravnenie(); 
                      

            if (isError)
                return;

            int endIndex = richTextBox1.Text.ToLower().LastIndexOf("end");
            while (curSymIndex < richTextBox1.Text.Length && (endIndex == -1 || curSymIndex < endIndex) && !isError)
                ParseSets();

            if (isError)
                return;

            string endString = richTextBox1.Text.Substring(curSymIndex-1, richTextBox1.Text.Length - curSymIndex);

            if (endString.ToLower().Trim() != "end")
            {
                richTextBox2.Text = "Ошибка: отсутствует слово 'end' или лишнее после слова 'end'";
                richTextBox1.Select(curSymIndex, 1);
                richTextBox1.SelectionBackColor = Color.Red;
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(LDForm == null || LDForm.IsDisposed)
            {
                LDForm = new BNF();
                LDForm.Show();
            }
        }

    }
}

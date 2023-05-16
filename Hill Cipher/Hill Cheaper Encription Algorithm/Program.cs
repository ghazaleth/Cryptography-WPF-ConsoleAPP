﻿namespace Hill_Cheaper_Encription_Algorithm
{
    internal class Program
    {
        static private int[,] key = new int[3, 3];
        static private int[,] keyReverse = new int[3, 3];
        static private int[,] adjointKey = new int[3, 3];
        static private int[] plainText = new int[3];
        static private int[] encriptedText = new int[3];
        static private int[] decriptedText = new int[3];
        static int determinant;
        static List<string> words = new List<string>();
        static string resultEncription = "", resultDecription = "";
        static int lessCharacterCount = 0, extraLettersCount = 0;

        static void Main(string[] args)
        {
            getKey();
            getPlainText();
            getDeterminant();
            if(determinant == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Key is wrong!");
                Console.ForegroundColor = ConsoleColor.White;
            }
            getAdjointKey();
            getKeyReverse();

            foreach (var item in words)
            {
                getPlainNumber(item);
                getEncriptedText();
                getDecriptedText();
            }

            correctionExtraCharacters();
            Console.Write("Do u want encription or decription (e or d) : ");
            if (Console.ReadLine() == "e")
                Console.WriteLine("Text Encripted : " + resultEncription);
            else
                Console.WriteLine("Text Decripted : " + resultDecription);


            Console.WriteLine("--------------------------");
            Console.WriteLine("Key is :");
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                    Console.Write($"{key[i, j]} ");
                Console.WriteLine();
            }
            Console.WriteLine("--------------------------");
            Console.ReadKey();
        }

        //fill key matrix from user
        private static void getKey()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Console.Write($"Enter row {i + 1} and column {j + 1} : ");
                    key[i, j] = int.Parse(Console.ReadLine());
                }
            }
        }
        //get plain text and set it to a 3 by 3 matrix
        private static void getPlainText()
        {
            Console.Write("Enter your text : ");
            string text = Console.ReadLine().Replace(" ", "");
            //remove ?
            //count letters more than 3
            List<char> temp = text.ToCharArray().ToList();
            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i] == '?')
                {
                    temp.Remove('?');
                    extraLettersCount++;
                    i--;
                }
            }

            //count letters less than 3 and replace white-space item with 'a' character
            text = new String(temp.ToArray());
            if(text.Length % 3 != 0)
            {
                for (int i = 0; i < (text.Length - (text.Length / 3 * 3)); i++)
                {
                    text += "a";
                    lessCharacterCount++;
                }
            }

            //separate letters 3 by 3 and save in the words list
            char[] textChars = text.ToCharArray();
            for (int i = 0; i < textChars.Length; i += 3)
                words.Add(textChars[i].ToString() + textChars[i + 1].ToString() + textChars[i + 2].ToString());
        }
        //get ASCI Code from the character in plain text
        private static void getPlainNumber(string word)
        {
            char[] textChars = word.ToCharArray();
            for (int i = 0; i < 3; i++)
                plainText[i] = Convert.ToInt32(textChars[i]) - 97;
        }
        private static void getEncriptedText()
        {
            //Encripting ...
            for (int i = 0; i < 3; i++)
            {
                encriptedText[i] = 0;
                for (int j = 0; j < 3; j++)
                    encriptedText[i] += key[j, i] * plainText[j];
                encriptedText[i] %= 26;
            }
            readPlainText(true);
        }
        private static void getDeterminant()
        {
            //Find determinant of key matrix
            determinant = ((key[0, 0] * (key[1, 1] * key[2, 2] - key[1, 2] * key[2, 1]))
                - (key[0, 1] * (key[1, 0] * key[2, 2] - key[1, 2] * key[2, 0]))
                + (key[0, 2] * (key[1, 0] * key[2, 1] - key[1, 1] * key[2, 0]))) % 26;
            if (determinant < 0)
                determinant += 26;
            getMultiplicativeInverse(ref determinant);
        }
        private static void getMultiplicativeInverse(ref int determinant)
        {
            for (int i = 1; i <= 26; i++)
            {
                if (((i * determinant) % (26)) == 1)
                {
                    determinant = i;
                    break;
                }
            }
        }
        private static void getAdjointKey()
        {
            int[,] stepOneKey = new int[5, 5];
            int[,] stepTwoKey = new int[4, 4];

            //step 1
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    stepOneKey[i, j] = key[(i > 2) ? i - 3 : i, (j > 2) ? j - 3 : j];

            //step 2
            for (int i = 1; i < 5; i++)
                for (int j = 1; j < 5; j++)
                    stepTwoKey[i - 1, j - 1] = stepOneKey[i, j];

            //step 3
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    adjointKey[i, j] = (stepTwoKey[j, i] * stepTwoKey[j + 1, i + 1] - stepTwoKey[j, i + 1] * stepTwoKey[j + 1, i]) % 26;
                    if (adjointKey[i, j] < 0)
                        adjointKey[i, j] += 26;
                }
            }
        }
        private static void getKeyReverse()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    keyReverse[i, j] = (determinant * adjointKey[i, j]) % 26;
        }
        private static void getDecriptedText()
        {
            decriptedText = new int[3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                    decriptedText[i] += plainText[j] * keyReverse[j, i];
                decriptedText[i] %= 26;
            }
            readPlainText(false);
        }
        private static void readPlainText(bool? isEncript = true)
        {
            if (isEncript == true)
                for (int i = 0; i < 3; i++)
                    resultEncription += Convert.ToChar(encriptedText[i] + 97).ToString();
            else
                for (int i = 0; i < 3; i++)
                    resultDecription += Convert.ToChar(decriptedText[i] + 97).ToString();

        }
        //remove extra letters
        private static void correctionExtraCharacters()
        {
            //add '?' to result for specify the less letters
            //This is to determine the number of extra letters to remove them later
            for (int i = 0; i < lessCharacterCount; i++)
                resultEncription += "?";

            char[] text = resultDecription.ToCharArray();

            for (int i = 0; extraLettersCount!=0; extraLettersCount--,i++)
                text[text.Length - (1+i)] = ' ';


            if (lessCharacterCount != 0)
                for (int i = text.Length - 1; i >= 0 && lessCharacterCount-- > 0; i--)
                    text[i] = ' ';
            resultDecription = new String(text).Replace(" ", "");

        }
    }
}
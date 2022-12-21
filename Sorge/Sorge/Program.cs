using System;
using System.Collections.Generic;
using System.Numerics;

namespace Sorge
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Do you want to encrypt or decrypt?\n" +
                "Please, reaply with 'E' or 'D'");

            string userChoice = Console.ReadLine();

            while (!userChoice.Equals("E") && !userChoice.Equals("D"))
            {
                Console.WriteLine("\nDear user, you have made a mistake.\n" +
                    "Please, type again 'E' or 'D'");

                userChoice = Console.ReadLine();
            }

            if (userChoice.Equals("E"))
            {
                Encrypt();
            }
            else
            {
                Decrypt();
            }
        }

        public static void Decrypt()
        {
            Console.WriteLine("\nDear user, what do you want to be decypted");
            string toDecrypt = Console.ReadLine();

            string indicatorGroup = toDecrypt.Substring(0, 5);
            string encryptedMessage = toDecrypt.Substring(5);

            string fourthGroupFromFront = GetFirstFourth(encryptedMessage);
            string fourthGroupFromBack = GetLastFourth(encryptedMessage);

            string bookLocation = DecryptWithNonCarryingAddition(indicatorGroup, fourthGroupFromFront);
            bookLocation = DecryptWithNonCarryingAddition(bookLocation, fourthGroupFromBack);

            Console.WriteLine("\nDear user, if the calculation is correct, you should have gotten 62314 and you actual result is: " + bookLocation);
            Console.WriteLine("\nIf this is not the answer, THEN SOMETHING IS WRONG AND THE PROGRAM WILL NOT WORK AS INTENDED");
            Console.WriteLine("\nNow, as we are expected to open a book and whatnot, please, type in again the key you used for encryption.\n" +
                "It should be " + encryptedMessage.ToString().Length + " characters long");

            string userKey = Console.ReadLine();
            while (userKey.ToString().Length != encryptedMessage.ToString().Length)
            {
                Console.WriteLine("\nDear user, you have inputted the wrong key as its length is not the same as the length of the message.\nPlease, try again: ");

                userKey = Console.ReadLine();
            }

            string originalEncryptedMessage = DecryptWithNonCarryingAddition(encryptedMessage, userKey);

            string decryptedMessage = DecryptMessageFromEncrypted(originalEncryptedMessage);
            Console.WriteLine(decryptedMessage);
        }

        public static string DecryptMessageFromEncrypted(string originalEncryptedMessage)
        {
            Dictionary<BigInteger, string> keyValuePairs = new Dictionary<BigInteger, string>();
            FillDictionaryNumberForLetter(keyValuePairs);

            string toReturn = "";

            for (int i = 0; i < originalEncryptedMessage.Length; i++)
            {
                if (BigInteger.Parse(originalEncryptedMessage[i].ToString()) >= 0 && BigInteger.Parse(originalEncryptedMessage[i].ToString()) <= 7)
                {
                    toReturn = toReturn + keyValuePairs[BigInteger.Parse(originalEncryptedMessage[i].ToString())];
                }
                else
                {

                    BigInteger actualNumber = BigInteger.Parse(originalEncryptedMessage[i].ToString() + originalEncryptedMessage[i + 1].ToString());

                    if (keyValuePairs[actualNumber].Equals("/"))
                    {
                        int copyLocationStartNumbers = i + 2;
                        BigInteger locationNumber = BigInteger.Parse(originalEncryptedMessage[i + 2].ToString() + originalEncryptedMessage[i + 3].ToString());
                        string toAdd = "/";
                        int numberOfIterations = 2;

                        for (int j = copyLocationStartNumbers; locationNumber != actualNumber; j = j + 2)
                        {
                            toAdd = toAdd + originalEncryptedMessage[j];
                            numberOfIterations = numberOfIterations + 2;
                            locationNumber = BigInteger.Parse(originalEncryptedMessage[j + 2].ToString() + originalEncryptedMessage[j + 3].ToString());
                        }

                        toAdd = toAdd + "/";
                        toReturn = toReturn + toAdd;
                        i = i + numberOfIterations + 1;
                    }
                    else
                    {
                        toReturn = toReturn + keyValuePairs[actualNumber];
                        i = i + 1;
                    }
                }
            }

            while (toReturn[toReturn.Length - 2].Equals('.'))
            {
                toReturn = toReturn.Substring(0, toReturn.Length - 1);
            }

            return toReturn;
        }

        public static string DecryptWithNonCarryingAddition(string indicatorGroup, string subtractor)
        {
            string indicatorGroupAsString = indicatorGroup.ToString();
            string subtractorAsString = subtractor.ToString();

            string toReturn = "";

            for (int i = 0; i < indicatorGroupAsString.Length; i++)
            {
                int number = int.Parse(indicatorGroupAsString[i].ToString()) - int.Parse(subtractorAsString[i].ToString());

                if (number < 0)
                {
                    number = number + 10;
                }

                toReturn = toReturn + number;
            }

            return toReturn;
        }

        public static void FillDictionaryNumberForLetter(Dictionary<BigInteger, string> keyValuePairs)
        {
            string[] linesOfLetters = System.IO.File.ReadAllLines(@"D:\KursovaKripto\letters.txt");
            string[] linesOfNumbers = System.IO.File.ReadAllLines(@"D:\KursovaKripto\numbers.txt");


            for (int i = 0; i < linesOfLetters.Length; i++)
            {
                string[] arrayLetters = linesOfLetters[i].Split(", ");
                string[] arrayNumbers = linesOfNumbers[i].Split(", ");

                for (int j = 0; j < arrayLetters.Length; j++)
                {
                    keyValuePairs.Add(BigInteger.Parse(arrayNumbers[j]), arrayLetters[j]);
                }
            }
        }
        public static void FillDictionaryLetterForNumber(Dictionary<string, BigInteger> keyValuePairs)
        {
            string[] linesOfLetters = System.IO.File.ReadAllLines(@"D:\KursovaKripto\letters.txt");
            string[] linesOfNumbers = System.IO.File.ReadAllLines(@"D:\KursovaKripto\numbers.txt");


            for (int i = 0; i < linesOfLetters.Length; i++)
            {
                string[] arrayLetters = linesOfLetters[i].Split(", ");
                string[] arrayNumbers = linesOfNumbers[i].Split(", ");

                for (int j = 0; j < arrayLetters.Length; j++)
                {
                    keyValuePairs.Add(arrayLetters[j], BigInteger.Parse(arrayNumbers[j]));
                }
            }
        }

        public static void Encrypt()
        {
            Console.WriteLine("\nDear user, what do you want to be encypted");
            string toEncrypt = Console.ReadLine();

            toEncrypt = AddSlashes(toEncrypt);

            string encryptedAfterTablification = EncyptWithDictionary(toEncrypt).ToString();

            Console.WriteLine("\nDear user, at this point your code is turned into digits and the total number of digits is " + encryptedAfterTablification.ToString().Length + "\n" +
                              "The next part requires you to open a book and to input some numbers but for the sake of " +
                              "\nthe algorithm please write an int EXACTLY as big as the number of digits of your message.");

            string newUserKey = Console.ReadLine();

            while (newUserKey.ToString().Length != encryptedAfterTablification.ToString().Length)
            {
                Console.WriteLine("\nDear user, you have to input exactly " + encryptedAfterTablification.ToString().Length + " digits. " +
                                  "Any other number will fail the encryption. Please, try again");

                newUserKey = Console.ReadLine();
            }

            encryptedAfterTablification = EncryptWithNonCarryingAddition(encryptedAfterTablification, newUserKey);

            Console.WriteLine("\nDear user, now as another key we will need for you to assume that you have opened page 6, line 23, column 14, thus,\n" +
                "making a new key = 62314");

            string bookLocation = "62314";

            string fourthGroupFromFront = GetFirstFourth(encryptedAfterTablification);
            string fourthGroupFromBack = GetLastFourth(encryptedAfterTablification);

            string indicatorGroup = EncryptWithNonCarryingAddition(bookLocation, fourthGroupFromFront);
            indicatorGroup = EncryptWithNonCarryingAddition(indicatorGroup, fourthGroupFromBack);

            encryptedAfterTablification = indicatorGroup + encryptedAfterTablification;

            Console.WriteLine("\nDear user, this is your final encoding of your message: " + encryptedAfterTablification);
        }

        public static string GetFirstFourth(string encryptedAfterTablification)
        {
            string resultToReturn = "";

            for (int i = 15; i < 20; i++)
            {
                resultToReturn = resultToReturn + encryptedAfterTablification[i];
            }

            return resultToReturn;
        }

        public static string GetLastFourth(string encryptedAfterTablification)
        {
            string resultToReturn = "";

            for (int i = encryptedAfterTablification.Length - 20; i < encryptedAfterTablification.Length - 15; i++)
            {
                resultToReturn = resultToReturn + encryptedAfterTablification[i];
            }

            return resultToReturn;
        }

        public static string EncryptWithNonCarryingAddition(string encryptedAfterTablification, string newUserKey)
        {
            string encryptedAfterTablificationString = encryptedAfterTablification;
            string newUserKeyString = newUserKey.ToString();

            string resultToBeReturned = "";

            for (int i = 0; i < encryptedAfterTablificationString.Length; i++)
            {
                BigInteger digitFromEncyption = BigInteger.Parse(encryptedAfterTablificationString[i].ToString());
                BigInteger digitFromNewKey = BigInteger.Parse(newUserKeyString[i].ToString());

                BigInteger toBeAddedToNewEncode = (digitFromEncyption + digitFromNewKey) % 10;

                resultToBeReturned = resultToBeReturned + toBeAddedToNewEncode.ToString();
            }

            return resultToBeReturned;
        }

        public static BigInteger EncyptWithDictionary(string toEncrypt)
        {
            Dictionary<string, BigInteger> keyValuePairs = new Dictionary<string, BigInteger>();
            FillDictionaryLetterForNumber(keyValuePairs);

            string resultOfTablification = "";

            for (int i = 0; i < toEncrypt.Length; i++)
            {
                if (Char.IsLetter(toEncrypt[i]) || toEncrypt[i].Equals('/') || toEncrypt[i].Equals('.'))
                {
                    resultOfTablification = resultOfTablification + keyValuePairs[toEncrypt[i].ToString()];
                }
                else
                {
                    resultOfTablification = resultOfTablification + toEncrypt[i] + toEncrypt[i];
                }
            }

            return BigInteger.Parse(resultOfTablification);
        }

        public static string AddSlashes(string toEncrypt)
        {
            string toReturn = "";

            string messageAsCharArray = toEncrypt.Replace(" ", "");

            foreach (char character in messageAsCharArray)
            {
                if (Char.IsDigit(character))
                {
                    toReturn = toReturn + '/' + character + '/';
                }
                else
                {
                    toReturn = toReturn + character;
                }
            }

            while (toReturn.Length < 20)
            {
                toReturn = toReturn + '.';
            }

            return toReturn;
        }
    }
}
using System.Security.Cryptography;
using System.Text;

namespace AoC2016
{
    public class Day05 : IDayProgram
    {
        private const string INPUT_TEST = "abc";
        private const string INPUT = "abbhdwsy";
        private const string HASH_PREFIX = "00000";
        private const int PASSWORD_LENGTH = 8;

        private static readonly MD5 md5 = MD5.Create();

        private static string GetMD5Hash(string text)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(text);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            StringBuilder sb = new();
            foreach (byte b in hashBytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        public static string GeneratePasswordOne(string doorID)
        {
            string password = string.Empty;
            for (int number = 0; password.Length < PASSWORD_LENGTH; number++)
            {
                string hash = GetMD5Hash($"{doorID}{number}");
                if (hash.StartsWith(HASH_PREFIX))
                {
                    char character = hash[5];
                    password += character;
                    $" - Found [{password.Length - 1}]='{character}' with number {number,7}".Log();
                }
            }
            return password;
        }

        public static string GeneratePasswordTwo(string doorID)
        {
            char[] password = Enumerable.Range(0, PASSWORD_LENGTH).Select(_ => '_').ToArray();
            for (int number = 0; password.Any(ch => ch == '_'); number++)
            {
                string hash = GetMD5Hash($"{doorID}{number}");
                if (hash.StartsWith(HASH_PREFIX))
                {
                    int index = Convert.ToInt32(hash.Substring(5, 1), 16);
                    char character = hash[6];
                    $" - Found '{character}' (from '{hash}') with number {number,8}".LogNNL();
                    if (index >= 8)
                    {
                        $", but skipping because index={index}".Log();
                    }
                    else if (password[index] != '_')
                    {
                        $", but skipping because there's already a valid value at index={index} ('{password[index]}')".Log();
                    }
                    else
                    {
                        password[index] = character;
                        $": [{index}]='{character}', and therefore password looks like '{new(password)}'".Log();
                    }
                }
            }
            return new(password);
        }

        public override void Run()
        {
            const string doorID = INPUT;

            string password1 = GeneratePasswordOne(doorID);
            $"\n > Given the input '{doorID}', the first password is '{password1}'.\n".Log();
            string password2 = GeneratePasswordTwo(doorID);
            $"\n > The second, better password is '{password2}'.\n".Log();
        }
    }
}
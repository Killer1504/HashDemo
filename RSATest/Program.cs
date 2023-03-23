using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Xml;
using System.Xml.Linq;
using System.Runtime.Remoting.Messaging;

namespace RSATest
{
    internal class Program
    {
        public class RSAEncryption
        {
            private static RSACryptoServiceProvider rsaCSP;
            public RSAParameters _privateKey;
            public RSAParameters _publicKey;

            public RSAEncryption()
            {
                rsaCSP = new RSACryptoServiceProvider();
                _privateKey = rsaCSP.ExportParameters(true);
                _publicKey = rsaCSP.ExportParameters(false);
            }

            public string GetPublicKey()
            {
                return GetKey(_publicKey);
            }

            private string GetKey(RSAParameters rSAParameters)
            {
                var sw = new StringWriter();
                var xs = new XmlSerializer(typeof(RSAParameters));
                xs.Serialize(sw, rSAParameters);
                return sw.ToString();
            }

            public string GetSecretKey()
            {
                return GetKey(_privateKey);
            }

            public string Encrypt(string plainText)
            {
                rsaCSP = new RSACryptoServiceProvider();
                rsaCSP.ImportParameters(_publicKey);

                var data = Encoding.Unicode.GetBytes(plainText);
                var cypher = rsaCSP.Encrypt(data, false);
                return Convert.ToBase64String(cypher);
            }

            public string Decrypt(string cypherText, RSAParameters privateKey)
            {
                var dateBytes = Convert.FromBase64String(cypherText);
                rsaCSP.ImportParameters(privateKey);
                var plainText = rsaCSP.Decrypt(dateBytes, false);
                return Encoding.Unicode.GetString(plainText);
            }

            public RSAParameters GetParameters(string key)
            {
                var xs = new XmlSerializer(typeof(RSAParameters));
                var file = "hung.xml";
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
                ;
                File.WriteAllText(file, key);
                try
                {
                    using (StreamReader reader = new StreamReader(file, true))
                    {
                        RSAParameters rSAParameters = (RSAParameters)xs.Deserialize(reader);
                        File.Delete(file);
                        return rSAParameters;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }



            }

        }
        static void Main(string[] args)
        {
            var separator = "---------------------------------";
            RSAEncryption rsa = new RSAEncryption();

            var pulicKey = rsa.GetPublicKey();
            var privateKey = rsa.GetSecretKey();
            Console.WriteLine($"Public key: \r\n{pulicKey}");
            Console.WriteLine(separator);
            Console.WriteLine($"Private key: \r\n{privateKey}");
            Console.WriteLine(separator);


            Console.WriteLine("Enter your text to enctypt:");
            var text = Console.ReadLine();
            if (!string.IsNullOrEmpty(text))
            {
                Console.WriteLine($"Your text: \r\n{text}");
                var cypher = rsa.Encrypt(text);
                Console.WriteLine($"Cypher: \r\n{cypher}");

                var privateKeyPara = rsa.GetParameters(privateKey);
                var plainText = rsa.Decrypt(cypher, privateKeyPara);
                Console.WriteLine($"plainText: \r\n{plainText}");
                Console.WriteLine();
            }
            Console.ReadKey();
        }
    }
}

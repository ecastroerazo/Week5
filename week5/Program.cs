using System.IO;
using System.Xml;
using static System.Console;
using static System.IO.Path;
using static System.Environment;

namespace Exercise02
{
    public class Customer
    {
        public string Name { get; set; }
        public string CreditCard { get; set; }
        public string Password { get; set; }

        public Customer(string name, string creditCard, string password)
        {
            Name = name;
            CreditCard = creditCard;
            Password = password;
        }
    }

    public static class Protector
    {
        public static string Encrypt(string plainText, string password)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(plainText + password);
            return System.Convert.ToBase64String(bytes);
        }

        public static (string SaltedHashedPassword, string Salt) Register(string userName, string password)
        {
            string salt = "SALT1234";
            string salted = password + salt;
            string hash = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(salted));
            return (hash, salt);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Write("Password to encrypt data in the document: ");
            string password = ReadLine()!;

            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            {
                WriteLine(string.IsNullOrWhiteSpace(password)
                    ? "You must enter a password."
                    : "The password must be at least 8 characters long.");
                return;
            }

            var customer = new Customer("Bob Smith", "1234-5678-9012-3456", "Pa$$w0rd");

            string xmlFile = Combine(CurrentDirectory, "..", "protected-customers.xml");

            var xmlWriter = XmlWriter.Create(xmlFile, new XmlWriterSettings { Indent = true });

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("customers");

            xmlWriter.WriteStartElement("customer");
            xmlWriter.WriteElementString("name", customer.Name);
            xmlWriter.WriteElementString("creditcard", Protector.Encrypt(customer.CreditCard, password));

            var user = Protector.Register(customer.Name, customer.Password);
            xmlWriter.WriteElementString("password", user.SaltedHashedPassword);
            xmlWriter.WriteElementString("salt", Protector.Encrypt(user.Salt, password));

            xmlWriter.WriteEndElement(); 
            xmlWriter.WriteEndElement(); 
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();

            WriteLine();
            WriteLine("Contents of the protected file:");
            WriteLine();
            WriteLine(File.ReadAllText(xmlFile));
        }
    }
}


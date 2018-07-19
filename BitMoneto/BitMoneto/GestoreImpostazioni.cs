using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Reflection;
using System.Security.Cryptography;
using System.IO;

namespace BitMoneto
{
    public static class GestoreImpostazioni
    {
        // Password utilizzata per la cifratura/decifratura dei dati
        private static string configPassword = "SecretKey";
        private static byte[] _salt = Encoding.ASCII.GetBytes("0123456789abcdef");

        private static string LeggiPassword()
        {
            ConfigurationManager.RefreshSection("appSettings");
            
            var passwordCifrata = ConfigurationManager.AppSettings["password"];
            
            return passwordCifrata == null ? null : DecifraStringa(passwordCifrata, configPassword);
        }

        public static void SalvaPassword(string password)
        {
            ConfigurationManager.RefreshSection("appSettings");
            var configurazione = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            string passwordCifrata = CifraStringa(password, configPassword);
            if(configurazione.AppSettings.Settings["password"] == null)
                configurazione.AppSettings.Settings.Add("password", passwordCifrata);
            else
                configurazione.AppSettings.Settings["password"].Value = passwordCifrata;
            configurazione.Save();
        }

        public static bool ControllaPassword(string passwordDaControllare)
        {
            return passwordDaControllare == LeggiPassword();
        }

        public static string CifraStringa(string strInChiaro, string secret)
        {
            string stringa = null;
            RijndaelManaged aesAlg = null;

            try
            {
                Rfc2898DeriveBytes chiave = new Rfc2898DeriveBytes(secret, _salt);
                aesAlg = new RijndaelManaged();
                aesAlg.Key = chiave.GetBytes(aesAlg.KeySize / 8);

                ICryptoTransform cifratore = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, cifratore, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(strInChiaro);
                        }
                    }
                    stringa = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            finally
            {
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            return stringa;
        }

        public static string DecifraStringa(string strCifrata, string secret)
        {
            RijndaelManaged aesAlg = null;
            string stringa = null;

            try
            {
                Rfc2898DeriveBytes chiave = new Rfc2898DeriveBytes(secret, _salt);
                byte[] bytes = Convert.FromBase64String(strCifrata);
                using (MemoryStream msDecrypt = new MemoryStream(bytes))
                {
                    aesAlg = new RijndaelManaged();
                    aesAlg.Key = chiave.GetBytes(aesAlg.KeySize / 8);
                    aesAlg.IV = LeggiArrayDiByte(msDecrypt);
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            stringa = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            finally
            {
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            return stringa;
        }

        private static byte[] LeggiArrayDiByte(Stream s)
        {
            byte[] tmp = new byte[sizeof(int)];
            if (s.Read(tmp, 0, tmp.Length) != tmp.Length)
            {
                throw new SystemException("Errore nella lettura dei byte");
            }

            byte[] bytes = new byte[BitConverter.ToInt32(tmp, 0)];
            if (s.Read(bytes, 0, bytes.Length) != bytes.Length)
            {
                throw new SystemException("Errore nella lettura dei byte");
            }
            return bytes;
        }

        public static void SalvaDatiApi<T>(string[] valori)
        {
            ConfigurationManager.RefreshSection("appSettings");
            var configurazione = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            string nomeClasse = typeof(T).FullName;
            string strValori = String.Join(";", valori);
            string valoriCifrati = CifraStringa(strValori, configPassword);
            if (configurazione.AppSettings.Settings[nomeClasse] == null)
                configurazione.AppSettings.Settings.Add(nomeClasse, valoriCifrati);
            else
                configurazione.AppSettings.Settings[nomeClasse].Value = valoriCifrati;
            configurazione.Save();
        }

        public static T LeggiDatiApi<T>(object[] parametriAggiuntivi)
        {
            ConfigurationManager.RefreshSection("appSettings");
            string nomeClasse = typeof(T).FullName;

            string strValoriCifrati = ConfigurationManager.AppSettings[nomeClasse];

            if (strValoriCifrati == null)
                return default(T);
            else
            {
                string strValoriDecifrati = DecifraStringa(strValoriCifrati, configPassword);
                string[] valori = strValoriDecifrati.Split(new char[] { ';' });
                object[] parametriCostruttore = new object[valori.Length + parametriAggiuntivi.Length];
                Array.Copy(valori, parametriCostruttore, valori.Length);
                Array.Copy(parametriAggiuntivi, 0, parametriCostruttore, valori.Length, parametriAggiuntivi.Length);
                T oggetto = (T)Activator.CreateInstance(typeof(T), parametriCostruttore);
                return oggetto;
            }
        }

    }
}

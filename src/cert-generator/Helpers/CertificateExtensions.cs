using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CertGenerator.Helpers
{
    static class CertificateExtensions
    {
        /// <summary>
        /// Get the PEM version of the certificate.
        /// </summary>
        /// <param name="certificate"></param>
        /// <returns></returns>
        public static string ToPem(this X509Certificate2 certificate)
        {
            byte[] certBytes = certificate.Export(X509ContentType.Cert);
            
            return PrettyPrint("CERTIFICATE", certBytes);
        }

        /// <summary>
        /// Pretty-print the contents with BEGIN header and END footer and max-line-length of 64 characters.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="contents"></param>
        /// <returns></returns>
        private static string PrettyPrint(string tag, byte[] contents)
        {
            StringBuilder sb = new StringBuilder();            
            string base64 = Convert.ToBase64String(contents);

            sb.AppendLine("-----BEGIN " + tag + "-----");
            int lineLength = 64;
            for (int index = 0; index < base64.Length; index += lineLength)
            {
                int to = base64.Length - index;
                if (to > lineLength)
                    to = lineLength;

                sb.AppendLine(base64.Substring(index, to));
            }
            
            sb.AppendLine("-----END " + tag + "-----");

            return sb.ToString();
        }

        /// <summary>
        /// Get the string version of the RSA key in PEM format.
        /// </summary>
        /// <param name="certificate"></param>
        /// <returns></returns>
        public static string ToPrivateKeyPkcs(this X509Certificate2 certificate)
        {
            RSA rsa = certificate.GetRSAPrivateKey();
            byte[] rsaBytes = rsa.ExportRSAPrivateKey();

            return PrettyPrint("RSA PRIVATE KEY", rsaBytes);
        }
    }
}

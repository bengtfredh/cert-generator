using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CertGenerator.Interfaces
{
    interface ICertificateAuthority
    {
        /// <summary>
        /// Generate and sign a new certificate. 
        /// </summary>
        /// <param name="subjectName"></param>
        /// <returns></returns>
        X509Certificate2 GenerateAndSignCertificate(string subjectName);
    }
}

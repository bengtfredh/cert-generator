using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CertGenerator.Interfaces
{
    interface ICertificateRootAuthority : ICertificateAuthority
    {
        /// <summary>
        /// Generate a new root certificate and sign it with its own certificate.
        /// </summary>
        /// <param name="subjectName"></param>
        /// <returns></returns>
        X509Certificate2 GenerateRootCertificate(string subjectName);

        /// <summary>
        /// Geneerate a new intermediate certificate and sign it with this root certificate.
        /// </summary>
        /// <param name="subjectName"></param>
        /// <returns></returns>
        X509Certificate2 GenerateAndSignIntermediateCertificate(string subjectName);
    }
}

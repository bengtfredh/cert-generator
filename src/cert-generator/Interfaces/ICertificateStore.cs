using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CertGenerator.Interfaces
{
    interface ICertificateStore
    {
        /// <summary>
        /// Save the certificate metadata in the database. 
        /// </summary>
        /// <param name="certificate"></param>
        /// <returns>The unique identifier for the metadata</returns>
        string SaveCertificateMetadata(X509Certificate2 certificate);


        /// <summary>
        /// Get the certificate metadata based on the identifier
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        CertificateMetadata GetCertificateMetadataByIdentifier(string identifier);        
    }
}

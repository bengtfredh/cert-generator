using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CertGenerator.Interfaces
{
    /// <summary>
    /// Class for storing metadata about certificates
    /// </summary>
    class CertificateMetadata
    {
        /// <summary>
        /// SubjectName 
        /// </summary>
        public string SubjectName { get; set; }
        
        /// <summary>
        /// Validity period of certificate
        /// </summary>
        public DateTime NotAfter { get; set; }

        /// <summary>
        /// Validity period of certificate
        /// </summary>
        public DateTime NotBefore { get; set; }

        /// <summary>
        /// The serial number of the certificate
        /// </summary>
        public string SerialNumber { get; set; }

        public static CertificateMetadata LoadFromCertificate(X509Certificate2 certificate)
        {
            CertificateMetadata metadata = new CertificateMetadata
            {
                NotBefore = certificate.NotBefore,
                NotAfter = certificate.NotAfter,
                SubjectName = certificate.Subject,
                SerialNumber = certificate.SerialNumber
            };

            return metadata;            
        }

        /// <summary>
        /// Load metadata from a metadata json file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task<CertificateMetadata> LoadFromJsonFile(string fileName)
        {
            string obj = await File.ReadAllTextAsync(fileName);
            return JsonSerializer.Deserialize<CertificateMetadata>(obj);
        }

        /// <summary>
        /// Save the metadata as json to a file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task SaveJsonAsync(string fileName)
        {            
            await File.WriteAllTextAsync(fileName, JsonSerializer.Serialize(this));
        }
    }
}

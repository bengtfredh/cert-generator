using CertGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CertGenerator.Implementation
{
    /// <summary>
    /// Implements basic support for a Certificate Authority. 
    /// </summary>
    class CertificateAuthority : ICertificateAuthority, IDisposable
    {
        protected X509Certificate2 _caCertificate;
        protected int _defaultValidToDays = 365 * 100;        
        protected int _defaultKeyLength = 2048;

        public CertificateAuthority()
        {


        }

        public CertificateAuthority(string pfxFile, string password)
        {
            this.SetupAuthorityFromFile(pfxFile, password);
        }

        protected void SetupAuthorityFromFile(string pfxFile, string password)
        {
            try
            {
                this._caCertificate = new X509Certificate2(pfxFile, password);
            }
            catch(Exception)
            {
                // Azure key vault returns pfx files in BASE64 encoded format. 
                this._caCertificate = new X509Certificate2(Convert.FromBase64String(File.ReadAllText(pfxFile)), password);
            }
        }        

        protected byte[] GenerateSerialNumber()
        {
            // We don't really care about the serial number in our limited implementation of CA. 
            // We are just interested in getting valid certificates that live in a very limited environment.
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                // 32 bytes = 256 bits. 
                byte[] serial = new byte[18];
                rng.GetBytes(serial);

                return serial;
            }            
        }

        /// <summary>
        /// Dispose and cleanup any resources used.
        /// </summary>
        public void Dispose()
        {            
            this._caCertificate?.Dispose();
            this._caCertificate = null;
        }

        /// <inheritdoc />
        public X509Certificate2 GenerateAndSignCertificate(string subjectName)
        {
            using (RSA newKey = RSA.Create(_defaultKeyLength))
            {
                // Create the reguest
                CertificateRequest request = new CertificateRequest(subjectName, newKey, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                var s  = new SubjectAlternativeNameBuilder();
                s.AddDnsName(subjectName.Substring("CN=".Length));

                request.CertificateExtensions.Add(s.Build());                
                request.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(request.PublicKey, true));
                
                // This is no CA
                request.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, false));

                // Add default TLS key usages
                request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment, false));


                request.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(new OidCollection
                {
                    new Oid("1.3.6.1.5.5.7.3.1"), // Server-auth
                    new Oid("1.3.6.1.5.5.7.3.2") // Client-auth
                }, true));

                X509Certificate2 certificate = request.Create(this._caCertificate, DateTimeOffset.UtcNow.AddDays(-1),
                    this._caCertificate.NotAfter.AddDays(-10), GenerateSerialNumber());

                return certificate.CopyWithPrivateKey(newKey);
            }
        }
    }
}

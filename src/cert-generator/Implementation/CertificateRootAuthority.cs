using CertGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CertGenerator.Implementation
{
    class CertificateRootAuthority : CertificateAuthority, ICertificateRootAuthority
    {
        private int _defaultValidRootDays = 365 * 20;

        public CertificateRootAuthority()
        {

        }

        public CertificateRootAuthority(string pfxFile, string password)
            : base(pfxFile, password)
        {

        }

        public X509Certificate2 GenerateAndSignIntermediateCertificate(string subjectName)
        {
            if (this._caCertificate == null)
                throw new Exception("Missing root certificate.");


            using (RSA key = RSA.Create(_defaultKeyLength))
            {
                CertificateRequest request = new CertificateRequest(subjectName, key, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                request.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(request.PublicKey, true));

                // This is a CA
                request.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, true, 1, true));

                // Add default TLS key usages
                request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.CrlSign | X509KeyUsageFlags.KeyCertSign, true));


                request.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(new OidCollection
                {
                    new Oid("1.3.6.1.5.5.7.3.1"), // Server-auth
                    new Oid("1.3.6.1.5.5.7.3.2") // Client-auth
                }, true));


                X509Certificate2 certificate = request.Create(_caCertificate, DateTimeOffset.UtcNow.AddDays(-1),
                    _caCertificate.NotAfter.AddDays(-5), GenerateSerialNumber());

                return certificate.CopyWithPrivateKey(key);
            }

        }

        public X509Certificate2 GenerateSelfSignedCertificate(string subjectName, bool doNotAddDnsName)
        {
            using (RSA key = RSA.Create(_defaultKeyLength))
            {
                CertificateRequest request = new CertificateRequest(subjectName, key, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                request.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(request.PublicKey, true));

                // This is CA
                request.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, true));

                // Add default TLS key usages
                request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment, true));


                request.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(new OidCollection
                {
                    new Oid("1.3.6.1.5.5.7.3.1"), // Server-auth
                    new Oid("1.3.6.1.5.5.7.3.2") // Client-auth
                }, true));

                if (false == doNotAddDnsName)
                {
                    var s = new SubjectAlternativeNameBuilder();
                    s.AddDnsName(subjectName.Substring("CN=".Length));
                    request.CertificateExtensions.Add(s.Build());
                }

                X509Certificate2 certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1),
                    DateTimeOffset.UtcNow.AddDays(_defaultValidRootDays));


                return certificate;
            }

        }
        public X509Certificate2 GenerateRootCertificate(string subjectName)
        {
            using (RSA key = RSA.Create(_defaultKeyLength))
            {
                CertificateRequest request = new CertificateRequest(subjectName, key, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                request.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(request.PublicKey, true));

                // This is CA
                request.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, true, 2, true));

                // Add default TLS key usages
                request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.CrlSign | X509KeyUsageFlags.KeyCertSign, true));


                request.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(new OidCollection
                {
                    new Oid("1.3.6.1.5.5.7.3.1"), // Server-auth
                    new Oid("1.3.6.1.5.5.7.3.2") // Client-auth
                }, true));

                X509Certificate2 certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1),
                    DateTimeOffset.UtcNow.AddDays(_defaultValidRootDays));


                return certificate;
            }
        }
    }
}

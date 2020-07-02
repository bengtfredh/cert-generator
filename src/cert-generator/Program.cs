using CertGenerator.Implementation;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using CommandLine;
using CertGenerator.Helpers;
using System.Security.Cryptography;

namespace CertGenerator
{
    class Program
    {
        static int Main(string[] args)
        {

            int result = CommandLine.Parser.Default.ParseArguments<RootOptions, IcaOptions, SelfSignedOptions, Options>(args)
                .MapResult(
                    (RootOptions ro) => RunRootAuthority(ro),
                    (IcaOptions io) => RunIntermediateOptions(io),
                    (SelfSignedOptions so) => RunSelfSignedOptions(so),
                    (Options o) => RunCertificateOptions(o),
                    errs => 1);

            return result;                        
        }

        static int RunRootAuthority(RootOptions ro)
        {
            using (CertificateRootAuthority rootAuthority = new CertificateRootAuthority())
            {
                X509Certificate2 cert = rootAuthority.GenerateRootCertificate(ro.SubjectName);
                byte[] pfxBytes = cert.Export(X509ContentType.Pfx, ro.PfxPassword);
                File.WriteAllBytes(ro.PfxFile + ".pfx", pfxBytes);
                
                if (false == ro.NoPem)
                    File.WriteAllText(ro.PfxFile + ".pem", cert.ToPem());
            }

            return 0;
        }

        static int RunIntermediateOptions(IcaOptions io)
        {
            using (CertificateRootAuthority rootAuthority = new CertificateRootAuthority(io.InPfxFile, io.InPfxPassword))
            {
                X509Certificate2 cert = rootAuthority.GenerateAndSignIntermediateCertificate(io.SubjectName);
                byte[] pfxBytes = cert.Export(X509ContentType.Pfx, io.PfxPassword);
                File.WriteAllBytes(io.PfxFile + ".pfx", pfxBytes);
                

                if (false == io.NoPem)
                    File.WriteAllText(io.PfxFile + ".pem", cert.ToPem());
               
            }

            return 0;
        }

        static int RunSelfSignedOptions(SelfSignedOptions so)
        {
            using (CertificateRootAuthority rootAuthority = new CertificateRootAuthority())
            {
                X509Certificate2 cert = rootAuthority.GenerateSelfSignedCertificate(so.SubjectName, so.DoNotAddDns, so.ValidDays);
                byte[] pfxBytes = cert.Export(X509ContentType.Pfx, so.PfxPassword);
                File.WriteAllBytes(so.PfxFile + ".pfx", pfxBytes);

                if (false == so.NoPem)
                    File.WriteAllText(so.PfxFile + ".pem", cert.ToPem());

                if (so.ExportKey)
                {                    
                    File.WriteAllText(so.PfxFile + ".key", cert.ToPrivateKeyPkcs());
                }
            }

            return 0;
        }

        static int RunCertificateOptions(Options o)
        {
            using (CertificateAuthority authority = new CertificateAuthority(o.InPfxFile, o.InPfxPassword))
            {
                X509Certificate2 cert = authority.GenerateAndSignCertificate(o.SubjectName);
                byte[] pfxBytes = cert.Export(X509ContentType.Pfx, o.PfxPassword);
                File.WriteAllBytes(o.PfxFile + ".pfx", pfxBytes);
               
                if (false == o.NoPem)
                    File.WriteAllText(o.PfxFile + ".pem", cert.ToPem());

                if (o.ExportKey)
                {                    
                    File.WriteAllText(o.PfxFile + ".key", cert.ToPrivateKeyPkcs());
                }
            }

            return 0;
        }
    }
}

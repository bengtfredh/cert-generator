using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CertGenerator.Interfaces
{
    interface IPrivateKeyGenerator
    {
        /// <summary>
        /// Generates a new RSA keypair with length <paramref name="keyLength"/>.
        /// </summary>
        /// <param name="keyLength"></param>
        /// <returns>A newly created RSA keypair</returns>
        RSA GeneratePrivateKey(int keyLength);
    }
}

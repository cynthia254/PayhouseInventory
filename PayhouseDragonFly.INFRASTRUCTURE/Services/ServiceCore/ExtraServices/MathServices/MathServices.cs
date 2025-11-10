using PayhouseDragonFly.INFRASTRUCTURE.Services.IServiceCoreInterfaces.IExtraServices.IMathServices;
using System.Security.Cryptography;
using System.Text;

namespace PayhouseDragonFly.INFRASTRUCTURE.Services.ServiceCore.ExtraServices.MathServices
{
    public  class MathServices: IMathServices
    {
        public async Task<string> GenerateTokenString()
        {

            Random random = new Random();
            var length = 1;
            const string chars = "aeiouabcdefghijklmnopqrstuvwxyz";
            var uniquecharacter = new string(Enumerable.Repeat(chars, length)
                 .Select(s => s[random.Next(s.Length)]).ToArray());

            var stringlength = 3;
            var upperletters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijkl";
            var uniquesstrings = new string(Enumerable.Repeat(upperletters, stringlength)
                 .Select(s => s[random.Next(s.Length)]).ToArray());

            var randomBytes = new byte[100];
            string token = "";

            using (var rng = RandomNumberGenerator.Create())
            {
                var key = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
                ASCIIEncoding encoding = new ASCIIEncoding();
                var keyByte = encoding.GetBytes(key);
                var hmacsha256 = new HMACSHA1(keyByte);
                //rng.GetBytes(randomBytes);
                token = uniquesstrings + (Convert.ToHexString(hmacsha256.ComputeHash(encoding.GetBytes(Convert.ToBase64String(randomBytes)))) + uniquecharacter).ToLower();
            }
            return token;
        }
    }
}

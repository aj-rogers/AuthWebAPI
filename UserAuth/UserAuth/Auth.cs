﻿using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UserAuth.Models;

namespace UserAuth
{
    public class Auth
    {
        private static string secret = "mySecretStringTahtNoOneCouldEverGues!!!Ever!!!";

        public static string GenerateJWT(User user)
        {
            var payload = new Dictionary<string, object>
            {
                {"username", user.Username },
                { "role", user.Role}
            };

            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            return encoder.Encode(payload, secret);
        }

        public static bool IsValidToken(string token)
        {
            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEnvoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEnvoder);

                var json = decoder.Decode(token, secret, verify: true);
                return true;
            }
            catch(Exception){
                return false;
            }
        }

        public static string Hash(string password, string salt)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(password, Encoding.ASCII.GetBytes(salt), 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            return Convert.ToBase64String(hash);
        }

        public static string GenerateSalt()
        {
            return Convert.ToBase64String(GenerateSaltBytes());
        }

        private static byte[] GenerateSaltBytes()
        {
            byte[] salt;
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt = new byte[16]);
            return salt;
        }
    }
}

﻿using System;
using System.IO;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Cms;

namespace JetBrains.SignatureVerifier.Crypt
{
    public class Pkcs7ProcessableObject : CmsProcessable
    {
        public DerObjectIdentifier ContentType { get; }
        public Asn1Encodable Content { get; }

        public Pkcs7ProcessableObject(DerObjectIdentifier contentType, Asn1Encodable content)
        {
            ContentType = contentType;
            Content = content;
        }

        public void Write(Stream outStream)
        {
            using var sw = new BinaryWriter(outStream);

            if (Content is Asn1Sequence)
            {
                Asn1Sequence s = Asn1Sequence.GetInstance(Content);

                foreach (Asn1Encodable enc in s)
                {
                    sw.Write(enc.ToAsn1Object().GetEncoded("DER"));
                }
            }
            else
            {
                byte[] encoded = Content.ToAsn1Object().GetEncoded("DER");
                int index = 1;
                while ((encoded[index] & 0xff) > 127)
                {
                    index++;
                }

                index++;
                sw.Write(encoded, index, encoded.Length - index);
            }
        }

        public object GetContent() => Content;
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;

namespace BodyArchitect.Shared
{
    public static class DataSigner
    {
        public static bool IsSigned(XmlDocument doc)
        {

            return getSignatureNode(doc) != null;
        }

        private static XmlElement getSignatureNode(XmlDocument doc)
        {
            XmlNamespaceManager xmlnsManager = new System.Xml.XmlNamespaceManager(doc.NameTable);

            xmlnsManager.AddNamespace("bk", "http://www.w3.org/2000/09/xmldsig#");

            var node = doc.SelectSingleNode("/*/bk:Signature", xmlnsManager);
            return (XmlElement)node;
        }
        // Verify the signature of an XML file against an asymmetric 
        // algorithm and return the result.
        public static Boolean VerifyXml(XmlDocument Doc, string signKey)
        {
            

            DSACryptoServiceProvider MySigner = new DSACryptoServiceProvider();
            MySigner.FromXmlString(signKey);
            // Create a new SignedXml object and pass it
            // the XML document class.
            SignedXml signedXml = new SignedXml(Doc);

            // Throw an exception if no signature was found.
            if (!IsSigned(Doc))
            {
                throw new CryptographicException("Verification failed: No Signature was found in the document.");
            }

            XmlElement node = getSignatureNode(Doc);
            // Load the first <signature> node.  
            signedXml.LoadXml(node);

            // Check the signature and return the result.
            return signedXml.CheckSignature(MySigner);
        }


        public static void SignXml(XmlDocument doc, string signKey)
        {
            
                // Create a SignedXml object.
                SignedXml signedXml = new SignedXml(doc);

                DSACryptoServiceProvider MySigner = new DSACryptoServiceProvider();
                MySigner.FromXmlString(signKey);
                // Add the key to the SignedXml document.
                signedXml.SigningKey = MySigner;

                // Create a reference to be signed.
                Reference reference = new Reference();
                reference.Uri = "";

                // Add an enveloped transformation to the reference.
                XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
                reference.AddTransform(env);

                // Add the reference to the SignedXml object.
                signedXml.AddReference(reference);

                // Compute the signature.
                signedXml.ComputeSignature();

                // Get the XML representation of the signature and save
                // it to an XmlElement object.
                XmlElement xmlDigitalSignature = signedXml.GetXml();

                // Append the element to the XML document.
                doc.DocumentElement.AppendChild(doc.ImportNode(xmlDigitalSignature, true));
        }

        public static void SignData(ISignedData data,string key)
        {
            data.Signature = null;
            DSACryptoServiceProvider MySigner = new DSACryptoServiceProvider();
            MySigner.FromXmlString(key);
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, data);
            stream.Seek(0, SeekOrigin.Begin);
            data.Signature = MySigner.SignData(stream);
        }

        public static bool VerifyData(ISignedData data,string key)
        {
            if(data.Signature==null)
            {
                return false;
            }
            byte[] signature = data.Signature;
            data.Signature = null;
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, data);
            stream.Seek(0, SeekOrigin.Begin);
            DSACryptoServiceProvider verifier = new DSACryptoServiceProvider();

            verifier.FromXmlString(key);
            data.Signature = signature;
            var b = stream.ToArray();
            return verifier.VerifyData(b, data.Signature);
        }
    }
}

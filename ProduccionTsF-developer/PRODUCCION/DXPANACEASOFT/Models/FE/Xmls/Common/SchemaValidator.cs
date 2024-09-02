using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

namespace DXPANACEASOFT.Models.FE.Xmls.Common
{
    public class SchemaValidator
    {
        private List<string> errors;

        public SchemaValidator()
        {
            errors = new List<string>();
        }

        public bool HaveErros()
        {
            return (errors.Count > 0);
        }

        public List<string> Validate(string xmlFileName, string xsdFileName)
        {
            errors.Clear();

            XmlReaderSettings booksSettings = new XmlReaderSettings();
            booksSettings.Schemas.Add(null, xsdFileName);
            booksSettings.ValidationType = ValidationType.Schema;

            booksSettings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            booksSettings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
            booksSettings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            booksSettings.ValidationFlags |= XmlSchemaValidationFlags.ProcessIdentityConstraints;
            booksSettings.ValidationFlags |= XmlSchemaValidationFlags.AllowXmlAttributes;

            booksSettings.ValidationEventHandler += new ValidationEventHandler(SettingsValidationEventHandler);

            XmlReader books = XmlReader.Create(xmlFileName, booksSettings);

            while (books.Read()) { }

            return errors;
        }

        private void SettingsValidationEventHandler(object sender, ValidationEventArgs e)
        {
            string message = string.Empty;
            if (e.Severity == XmlSeverityType.Warning)
            {
                message = string.Format("[Ln: {0}, Col: {1}] [WARNING]: {2}", e.Exception.LineNumber, e.Exception.LinePosition, e.Message);

            }
            else if (e.Severity == XmlSeverityType.Error)
            {
                message = string.Format("[Ln: {0}, Col: {1}] [ERROR]: {2}", e.Exception.LineNumber, e.Exception.LinePosition, e.Message);

            }

            if (message != null || message != "")
            {
                errors.Add(message);
                //Console.WriteLine(message);
            }
        }
    }
}
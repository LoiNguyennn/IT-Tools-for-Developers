using ITTools.Core.Models;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace ConversionTools
{
    public class JsonToXml : ITool
    {
        public string Name => "JSON to XML";
        public string Description => "Converts JSON to XML format";
        public string Category => "Conversion Tools";

        private static XElement JsonToXmlNode(string elementName, JToken jToken)
        {
            switch (jToken.Type)
            {
                case JTokenType.Object:
                    return new XElement(elementName,
                        jToken.Children<JProperty>().Select(prop =>
                            JsonToXmlNode(prop.Name, prop.Value)));

                case JTokenType.Array:
                    return new XElement(elementName,
                        jToken.Select(item => JsonToXmlNode("item", item)));

                case JTokenType.String:
                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.Boolean:
                case JTokenType.Date:
                    return new XElement(elementName, jToken.ToString());

                case JTokenType.Null:
                    return new XElement(elementName); // Empty tag for null values

                default:
                    throw new ArgumentException($"Unsupported token type: {jToken.Type}");
            }
        }

        public string Execute(string input)
        {
            // Parse JSON input
            JToken jToken = JToken.Parse(input);

            // Always use "root"
            string rootName = "root";

            // Convert to XML
            XElement xmlElement = JsonToXmlNode(rootName, jToken);

            // Return formatted XML string
            return xmlElement.ToString();
        }
    }
}
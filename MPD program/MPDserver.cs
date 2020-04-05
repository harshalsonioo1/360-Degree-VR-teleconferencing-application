﻿/* Function MPDserver.genFile is called by WebServer to produce XML in specified path. 
 * This function first gets videoID and secID of the chunk, and pass them to function MPDdata.getServerData to get MPDdata,
 * then it passes MPDdata to function WriteXml together with the path string.
 * Function WriteXml transform the MPDdata to Xml format and save it in given path.
 */
using System;
using System.Text;

using System.Xml;

namespace MPD
{
    public class MPDserver
    {
        public MPDserver()
        {

        }
       
        public static bool genFile(string sPhysicalFilePath) {
            try
            {
                int fileNameStart = sPhysicalFilePath.LastIndexOf('/');
                int videoID = int.Parse(sPhysicalFilePath.Substring(fileNameStart+1, 4));
                MPDdata data = MPDdata.getServerData(videoID);
                return WriteXml(data, sPhysicalFilePath);
            }
            catch
            {
                return false;
            }
                
        }
        public static bool WriteXml(MPDdata data, string sFilePath)
        {
            try
            {
                XmlTextWriter xml = new XmlTextWriter(sFilePath, Encoding.UTF8);
                xml.Formatting = Formatting.Indented;
                xml.WriteStartDocument();
                xml.WriteStartElement("MPD");

                xml.WriteStartElement("tilePosition");
                
                xml.WriteCData(UTF8ByteArrayToString(data.tilePosition));
                xml.WriteEndElement();

                xml.WriteStartElement("tileLumi");
                xml.WriteCData(UTF8ByteArrayToString(data.tileLumi));
                xml.WriteEndElement();

                xml.WriteStartElement("tileDoF");
                xml.WriteCData(UTF8ByteArrayToString(data.tileDoF));
                xml.WriteEndElement();

                xml.WriteStartElement("objectTraj");
                xml.WriteCData(UTF8ByteArrayToString(data.objectTraj));
                xml.WriteEndElement();

                xml.WriteStartElement("lookupTable");
                xml.WriteCData(UTF8ByteArrayToString(data.lookupTable));
                xml.WriteEndElement();

                xml.WriteEndElement();
                xml.WriteEndDocument();
                xml.Flush();
                xml.Close();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

       

        
        private static string UTF8ByteArrayToString(byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            string constructedString = encoding.GetString(characters);
            return (constructedString);
        }

    }
}

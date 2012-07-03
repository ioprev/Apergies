using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using Polenter.Serialization;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using Apergies.Model;
using System.IO;

namespace Apergies
{
    public class DataCache
    {
        private String filename;
        IsolatedStorageFile isoFile;
        IsolatedStorageFileStream isoStream;

        public DataCache(String filename)
        {
            this.filename = filename;
        }
        public void Marshall(List<StrikeRecord> list)
        {
            using (this.isoFile = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (this.isoStream = new IsolatedStorageFileStream(this.filename, FileMode.OpenOrCreate, this.isoFile))
                {
                    Serialize(this.isoStream, list);
                    this.isoStream.Close();
                }
            }
        }
        public List<StrikeRecord> Unmarshall()
        {
            List<StrikeRecord> temp;
            using (this.isoFile = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (this.isoFile.FileExists(this.filename))
                {
                    using (isoStream = new IsolatedStorageFileStream(this.filename, FileMode.Open, this.isoFile))
                    {
                        temp = (List<StrikeRecord>)Deserialize(this.isoStream);
                        this.isoStream.Close();
                    }
                }
                else
                {
                    temp = new List<StrikeRecord>();
                    temp.Add(new StrikeRecord("Δεν ήταν δυνατή η ανάκτηση των πληροφοριών",""));
                }
            }
            return temp;
        }


        private static void Serialize(Stream streamObject, object obj)
        {
            if (obj == null || streamObject == null)
                return;

            // true - binary serialization, default - xml ser.
            var serializer = new SharpSerializer(true);
            serializer.Serialize(obj, streamObject);
        }

        private static object Deserialize(Stream streamObject)
        {
            if (streamObject == null)
                return null;

            // true - binary serialization, default - xml ser.
            var serializer = new SharpSerializer(true);
            return serializer.Deserialize(streamObject);
        }
    }
}

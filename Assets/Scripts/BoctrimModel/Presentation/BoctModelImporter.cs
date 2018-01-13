using Boctrim.Domain;
using System.IO;
using System.Collections.Generic;
using System;
using System.Text;
using MPF.Infrastructure;
using UnityEngine;
using YamlDotNet.Serialization;
using Boctrim.Infrastructure;

namespace Boctrim.Presentation
{

    public static class BoctModelImporter
    {
        public static BoctModel Import(string path)
        {
            if (path.EndsWith(".ftrim"))
            {
                return ImportFtrim(path);
            }
            else if (path.EndsWith(".boctrim"))
            {
                return ImportYaml(path);
            }

            // Unknown Format
            return null;
        }
        
        #region Yaml Format

        static BoctModel ImportYaml(string path)
        {
            var sr = new StreamReader(path);
            var input = sr.ReadToEnd();
            sr.Close();

            var deserializer = new DeserializerBuilder().Build();
            var data = deserializer.Deserialize<BoctModelData>(input);

            Debug.Log(data.Regions.Count);
            
            var model = new BoctModel(data);
            
            model.Info.GUID = Guid.NewGuid();
            
            return model;
        }
        
        #endregion
        
        #region Ftrim Format (Old App)
        
        static BoctModel ImportFtrim(string path)
        {
            var model = new BoctModel();

            var info = new BoctModelInfo();

            info.GUID = Guid.NewGuid();
            info.Name = "Import Model(" + info.GUID.ToString().Substring(0, 8) + ")";
            info.DataPath = info.GUID + ".db";
            info.DataVersion = BoctModelInfo.CurrentVersion;

            model.Info = info;

            using (var sr = new StreamReader(path))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    var arr = line.Split(new char[] { ':' });
                    if (arr.Length > 1)
                    {
                        var key = arr[0].Trim();
                        var val = arr[1].Trim();

                        if (key == "name")
                        {
                            model.Info.Name = val;
                        }
                        else if (key == "boct")
                        {
                            InsertBoctFromBoctPacket30(val, model.Head);
                        }
                        else if (key == "material")
                        {
                            var colors = val.Split(new char[] { ',' });

                            for (int i = 0; i < colors.Length; i++)
                            {
                                var mat = new BoctMaterial();
                                mat.GUID = Guid.NewGuid();
                                mat.LUID = i;
                                mat.Color = colors[i].ToColor();
                                model.MaterialList.AddMaterial(mat);
                            }
                        }
                    }
                }
            }
            return model;
        }

        static bool InsertBoctFromBoctPacket30(string str, Boct target)
        {
            if (str.Length < 5 || str.Length % 5 != 0) 
            {
                return false;
            }

            int i = 0;
            while (i < str.Length)
            {
                uint packet = Base64Tools.ParseInt30Fast(str.Substring(i, 5));

                uint mid = BoctPacketTools.GetMaterialPart(packet);

                uint len = BoctPacketTools.GetAddressLengthPart(packet);

                var list = new List<byte>();
                for(uint j = 0; j < len; j++)
                {
                    uint pos = BoctPacketTools.GetPosition(packet, j + 1);
                    list.Add((byte)pos);
                }

                BoctTools.InsertBoct(list, target, (int)mid);

                i += 5;
            }
            return true;
        }

        public static void AnalyzePacketString(string str)
        {
            Debug.Log("Total Length: " + str.Length);
            Debug.Log("Block Size: " + str.Length / 5f);

            int i = 0;
            while (i < str.Length)
            {
                var sb = new StringBuilder();

                sb.Append("Index: " + i);
                uint packet = Base64Tools.ParseInt30Fast(str.Substring(i, 5));
                sb.Append(", Packet: " + packet.ToString());
                uint mid = BoctPacketTools.GetMaterialPart(packet);
                sb.Append(", Material ID: " + mid);
               
                uint len = BoctPacketTools.GetAddressLengthPart(packet);
                sb.Append(", Address Length: " + len);

                var list = new List<byte>();
                for (uint j = 0; j < len; j++)
                {
                    uint pos = BoctPacketTools.GetPosition(packet, j + 1);
                    list.Add((byte)pos);
                }

                Debug.Log(sb.ToString());

                i += 5;
            }

        }
        
        #endregion
    }

}

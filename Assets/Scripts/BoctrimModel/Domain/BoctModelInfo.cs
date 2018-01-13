using MPF.Domain;
using YamlDotNet.Serialization;

namespace Boctrim.Domain
{
    public class BoctModelInfo: DomainModel
    {

        public enum SourceType
        {
            Empty, Storage, Import
        }

        public const string CurrentVersion = "100";

        public string DataVersion { get; set; }
        
        public string Name {get; set;}

        [YamlIgnore]
        public string DataPath { get; set;}

        [YamlIgnore]
        public string PreviewImageData { get; set;}

        [YamlIgnore]
        public SourceType Source { get; set;}

        public override string ToString()
        {
            return GUID + " " + Name + " " + DataPath + " " + DataVersion;
        }

        public BoctModelInfo()
        {
        }

    }

}